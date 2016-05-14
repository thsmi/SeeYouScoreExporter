using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace net.tschmid.scooring.exporter
{
    public class Loader
    {
        private string Url;

        public Loader(string url)
        {
            Url = url;
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private string CreateNonce(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new RNGCryptoServiceProvider();

            byte[] data = new byte[length];
            random.GetBytes(data);

            string result = "";

            for (var i = 0; i < length; i++)
            {
                result += chars[(data[i] % length)];
            }

            return result;
        }

        private string CreateToken(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();

            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);

            HMACSHA256 hmac = new HMACSHA256(keyByte);

            byte[] hashmessage = hmac.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }

        private Stream FetchData(string url)
        {
            // An random string of alphanumeric ascii
            string nonce = CreateNonce(16);
            string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            HMACSHA256 hmac = new HMACSHA256();

            string signature = CreateToken("" + nonce + date + Properties.Settings.Default.ExportCredentialsClientId, Properties.Settings.Default.ExportCredentialsSecret);

            string authorization = "http://api.soaringspot.com/v1/hmac/v1 ClientID=\"" + Properties.Settings.Default.ExportCredentialsClientId + "\", Signature=\"" + signature + "\", Nonce=\"" + nonce + "\", Created=\"" + date + "\"";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";

            request.Headers[HttpRequestHeader.Authorization] = authorization;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                response = ((HttpWebResponse)e.Response);
                Console.WriteLine("Error code: {0}", ((HttpWebResponse)e.Response).StatusCode);
            }

            return response.GetResponseStream();

        }


        public Dictionary<string, dynamic> Load()
        {
            // grab the response
            var responseStream = LoadRaw();
            string data = "";

            if (responseStream != null)
            {
                StreamReader reader = new StreamReader(responseStream);

                data = reader.ReadToEnd();
            }

            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                return ser.Deserialize<Dictionary<string, dynamic>>(data);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load " + this.Url);

                if (Properties.Settings.Default.ExportDebug)
                {
                    File.WriteAllText(".\\failedResponse.log", data);
                    Console.WriteLine("Response dumped to .\\failedResponse.log");
                }

                return new Dictionary<string, dynamic>();
            }
        }

        public Stream LoadRaw()
        {
            return FetchData(this.Url);
        }
    }

}
