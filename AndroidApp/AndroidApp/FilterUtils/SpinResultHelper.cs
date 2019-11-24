using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class SpinResultHelper
    {

        static readonly string TAG = typeof(SpinResultHelper).Name.ToString();

        public static string getDomainName(string jsonBody)
        {
            string result = "";
            try
            {
                var jsonObject = JObject.Parse(jsonBody);
                var version = jsonObject["v"].ToString();
                if (version == "1")
                {
                    var domainName = jsonObject["domainName"];
                    result = domainName.ToString();
                }
                else
                {
                    throw new Exception("Unknown Version of domain request");
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }


        const int Category_Blocked = 1011;
        const int Category_Allowed = 1;

        public static string domainCategoryResult(bool block)
        {
            string result = "";
            try
            {
                int myCategory = block ? Category_Blocked : Category_Allowed;

                var jsonObject = new JObject();
                jsonObject["categories"] = new JArray(new[] { new JValue(myCategory) });
                jsonObject["resultCode"] = new JValue(0);

                result = jsonObject.ToString();
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex.ToString());
            }
            return result;
        }
    }
}
