using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;


namespace AndroidApp.FilterUtils
{
    class MasterPassword
    {

        static readonly string TAG = typeof(MasterPassword).Name.ToString();

        private static string get256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static TaskResult SetPassword(string newPass)
        {
            TaskResult result = TaskResult.Fail("Init");
            try
            {
                newPass = get256Hash(newPass);
                File.WriteAllText(Filenames.MASTER_PASSWORD.getAppPrivate(),newPass);
                result = TaskResult.Success("Master password saved");
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
                result = TaskResult.Fail(ex.ToString());
            }
            return result;
        }

        public static TaskResult ComparePass(string checkPass)
        {
            TaskResult result = TaskResult.Fail("Init");
            try
            {
                checkPass = get256Hash(checkPass);
                string currentPass = File.ReadAllText(Filenames.MASTER_PASSWORD.getAppPrivate());
                bool arePasswordHashSame = checkPass.Equals(currentPass);

                result = new TaskResult();
                result.success = arePasswordHashSame;
                result.eventReason = "Are password same? " + arePasswordHashSame;
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
                result = TaskResult.Fail(ex.ToString());
            }
            return result;
        }
    }
}
