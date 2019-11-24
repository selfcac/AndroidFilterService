using ISimpleHttpListener.Rx.Enum;
using ISimpleHttpListener.Rx.Model;
using SimpleHttpListener.Rx.Extension;
using SimpleHttpListener.Rx.Model;
using SimpleHttpListener.Rx.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidApp.FilterUtils
{
    class HttpFilteringServer
    {
        static readonly string TAG = typeof(HttpFilteringServer).Name.ToString();

        const int port = 8000;
        const string ContentHTML = "text/html; charset=UTF-8";
        const string ContentJSON = "application/json";

        //https://github.com/1iveowl/SimpleHttpListener.Rx
        CancellationTokenSource cancellationTokenSource;
        TcpListener myListener;
        IDisposable myServerInstance;

        string ipString(IPEndPoint ip)
        {
            return ip.Address + "^" + ip.Port;
        }

        string stringHTML(string body = "", string title = "")
        {
            string result = "";
            try
            {
                result =
                    AndroidApp.Properties.Resources.EmptyHTML
                    .Replace("{title}", title)
                    .Replace("{body}", body)
                    ;
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        string readReqBodyAsString(IHttpRequestResponse req)
        {
            string result = "";
            try
            {
                if (req.Body != null && req.Body.Length > 0)
                {
                    StreamReader reader = new StreamReader(req.Body);
                    req.Body.Position = 0;
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        string reasonToHTML(string reason)
        {
            return reason.Replace("<*", "<b>").Replace("*>", "</b>").Replace("_<", "<u>").Replace(">_", "</u>");
        }

        public void Start(int port)
        {
            try
            {
                myListener = new TcpListener(IPAddress.Loopback, port)
                {
                    ExclusiveAddressUse = false
                };

                var httpSender = new HttpSender();

                cancellationTokenSource = new CancellationTokenSource();

                myServerInstance = myListener.ToHttpListenerObservable(cancellationTokenSource.Token)
                    .Do(r =>
                    {
                        AndroidBridge.d(TAG, "[START] \"" + r.Path + "\" IP: " + ipString(r.RemoteIpEndPoint));
                    })
                    // Send reply to browser
                    .Select(r => Observable.FromAsync(() => SendResponseAsync(r, httpSender)))
                    .Concat()
                    .Subscribe(r =>
                    {
                        //Console.WriteLine("Reply sent.");                     
                    },
                    ex =>
                    {
                        AndroidBridge.e(TAG, ex.ToString());
                    },
                    () =>
                    {
                        AndroidBridge.d(TAG,"Server completed");
                    });
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
        }   

        public void Stop()
        {
            try
            {
                AndroidBridge.d(TAG, "Closing server");
                cancellationTokenSource?.Cancel();
                myServerInstance?.Dispose();
                myListener?.Stop();
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
        }

        public void StartHttpServer()
        {
            Start(port);
        }
      
        public void StopHTTPServer()
        {
            Stop();
        }

        private static bool isBlockedFullFlow(Uri url, out string reason)
        {
            reason = "Not blocked.";
            bool isBlocked = false;
            if (FilteringObjects.isInWifiBlockZone && FilteringObjects.isFiltering)
            {
                string http_reason = "";
                if (FilteringObjects.timePolicy.isBlockedNow())
                {
                    isBlocked = true;
                    reason = "Timed blocked at " + DateTime.Now.ToString();
                }
                else if (!FilteringObjects.httpPolicy.isWhitelistedURL(url, out http_reason))
                {
                    isBlocked = true;
                    reason = http_reason;
                }
            }
            return isBlocked;
        }

        string EP_echo(IHttpRequestResponse req)
        {
            string result = "";
            try
            {

                string headers = "";
                foreach (var key in req.Headers.Keys)
                {
                    headers += $"{key}: {req.Headers[key]}\r\n";
                }

                string sentBody = readReqBodyAsString(req);

                result =
                    AndroidApp.Properties.Resources.Echo
                    .Replace("{0}", headers)
                    .Replace("{1}", sentBody)
                    .Replace("{2}", req.Method)
                    .Replace("{3}", req.Path)
                    ;
            }
            catch (Exception ex)
            {
                result = stringHTML(ex.ToString(), "Error");
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        string EP_show_reason(IHttpRequestResponse req)
        {
            string result = "";
            string prefix = "url=";
            try
            {
                string reason = "No url found";

                if (!string.IsNullOrEmpty(req.QueryString))
                {
                    int startIndex = req.QueryString.IndexOf(prefix);
                    if (startIndex > -1)
                    {
                        Uri url = new Uri(System.Net.WebUtility.UrlDecode(req.QueryString.Substring(startIndex + prefix.Length)));
                        isBlockedFullFlow(url, out reason);
                    }
                }

                result = AndroidApp.Properties.Resources.BlockedPage.Replace("{0}", reasonToHTML(reason));
            }
            catch (Exception ex)
            {
                result = stringHTML(ex.ToString(), "Error");
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        

        string EP_checkDomain(IHttpRequestResponse req)
        {
            string result = "";
            try
            {
                var reqJSON = readReqBodyAsString(req);
                string domain = SpinResultHelper.getDomainName(reqJSON).Trim(new char[] { ' ', '"' });
                domainHistory.Add(domain);

                string reason = "";
                bool block = isBlockedFullFlow(new Uri(domain),out reason);


                if (block)
                {
                    try
                    {
                        File.AppendAllLines(Filenames.BLOCK_LOG.getAppPublic(), new[] {
                            string.Format("[{0}] {1} {2}", DateTime.Now, domain, reason)
                        });
                    }
                    catch (Exception ex)
                    {
                        AndroidBridge.e(TAG, ex);
                    }
                }

                result = SpinResultHelper.domainCategoryResult(block);
            }
            catch (Exception ex)
            {
                result = "{\"error\":\"0\"}".Replace("0", ex.ToString());
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        List<string> domainHistory = new List<string>();
        string EP_domainHistory(IHttpRequestResponse req)
        {
            string result = "";
            try
            {
                string body = String.Join("<br />", domainHistory);
                result = stringHTML(body: body);
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        async Task SendResponseAsync(IHttpRequestResponse request, HttpSender httpSender)
        {
            try
            {
                if (request.RequestType == RequestType.TCP)
                {
                    string path = request.Path;
                    string responseContentType = ContentHTML;
                    string responseBody = $"<html>\r\n<body>Unknown Path:\r\n{path}</body>\r\n</html>";


                    if (path.StartsWith("/echo"))
                    {
                        responseContentType = ContentHTML;
                        responseBody = EP_echo(request);
                    }
                    else if (path.StartsWith("/history"))
                    {
                        responseContentType = ContentHTML;
                        responseBody = EP_domainHistory(request);
                    }
                    else if (path.StartsWith("/check"))
                    {
                        responseContentType = ContentJSON;
                        responseBody = EP_checkDomain(request);
                    }
                    else if (path.StartsWith("/reason"))
                    {
                        responseContentType = ContentHTML;
                        responseBody = EP_show_reason(request);
                    }

                    var response = new HttpResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ResponseReason = HttpStatusCode.OK.ToString(),
                        Headers = new Dictionary<string, string>
                    {
                        {"Date", DateTime.UtcNow.ToString("r")},
                        {"Content-Type",  responseContentType},
                        {"Connection","close" }
                    },
                        Body = new MemoryStream(Encoding.UTF8.GetBytes(responseBody))
                    };

                    AndroidBridge.d(TAG, "[RESP-SEND] \"" + request.Path + "\" IP: " + ipString(request.RemoteIpEndPoint));

                    await httpSender.SendTcpResponseAsync(request, response).ConfigureAwait(false);
                    request.TcpClient.Close(); // Force close!

                    AndroidBridge.d(TAG, "[END] \"" + request.Path + "\" IP: " + ipString(request.RemoteIpEndPoint));
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
        }
    }
}
