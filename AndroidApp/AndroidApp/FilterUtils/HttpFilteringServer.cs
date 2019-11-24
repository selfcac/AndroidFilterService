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
        const int port = 8000;
        static readonly string TAG = typeof(HttpFilteringServer).Name.ToString();

        //https://github.com/1iveowl/SimpleHttpListener.Rx
        CancellationTokenSource cancellationTokenSource;
        TcpListener myListener;
        IDisposable myServerInstance;

        string ipString(IPEndPoint ip)
        {
            return ip.Address + "^" + ip.Port;
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

        string readReqBodyString(IHttpRequestResponse req)
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

        string echoBody(IHttpRequestResponse req)
        {
            string result = "";
            try
            {

                string headers = "";
                foreach (var key in req.Headers.Keys)
                {
                    headers += $"{key}: {req.Headers[key]}\r\n";
                }

                string sentBody = readReqBodyString(req);

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
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }

        string checkDomain(IHttpRequestResponse req)
        {
            string result = "";
            try
            {
                var reqJSON = readReqBodyString(req);
                string domain = SpinResultHelper.getDomainName(reqJSON).Trim(new char[] { ' ', '"' });
                domainHistory.Add(domain);

                bool block = domain.Contains("yoni");
                result = SpinResultHelper.domainCategoryResult(block);
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
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

        List<string> domainHistory = new List<string>();
        string domainHistoryList(IHttpRequestResponse req)
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

        const string ContentHTML = "text/html; charset=UTF-8";
        const string ContentJSON = "application/json";

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
                        responseBody = echoBody(request);
                    }
                    else if (path.StartsWith("/history"))
                    {
                        responseContentType = ContentHTML;
                        responseBody = domainHistoryList(request);
                    }
                    else if (path.StartsWith("/check"))
                    {
                        responseContentType = ContentJSON;
                        responseBody = checkDomain(request);
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
    }
}
