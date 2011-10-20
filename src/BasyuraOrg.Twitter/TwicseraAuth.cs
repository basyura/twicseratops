/*
 * original : http://d.hatena.ne.jp/nojima718/20100129/1264792636
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

using Codeplex.Data;
using BasyuraOrg.Twitter;

namespace BasyuraOrg.Twitter {
    /**
     *
     */
    public class TwicseraAuth {
        /** */
        private Random random = new Random();

        /** */
        public string ConsumerKey        { get; private set; }
        /** */
        public string ConsumerSecret     { get; private set; }
        /** */
        public string AccessToken        { get; private set; }
        /** */
        public string AccessTokenSecret  { get; private set; }
        /**
         *
         */
        public TwicseraAuth(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret) {
            ServicePointManager.Expect100Continue = false;
            ConsumerKey       = consumerKey;
            ConsumerSecret    = consumerSecret;
            AccessToken       = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }
        /*
         *
         */
        public static AuthRegister NewRegister(string consumerKey, string consumerSecret) {
            return new AuthRegister(consumerKey, consumerSecret);
        }
        /**
         *
         */
        public dynamic HttpGet(string url, IDictionary<string, string> parameters) {
            WebRequest req = null;
            if (!String.IsNullOrEmpty(AccessToken)) {
                req = WebRequest.Create(url + '?' + JoinParameters(MergeAccessParam(url, "GET", parameters)));
            }
            else {
                req = WebRequest.Create(url + '?' + JoinParameters(parameters));
            }
            WebResponse res = req.GetResponse();

            using (Stream stream = res.GetResponseStream()) {
                using (StreamReader reader = new StreamReader(stream)) {
                    if (url.EndsWith("json")) {
                        return DynamicJson.Parse(reader.ReadToEnd());
                    }
                    else {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        /**
         *
         */
        public dynamic HttpPost(string url, IDictionary<string, string> parameters) {
            byte[] data = Encoding.ASCII.GetBytes(JoinParameters(MergeAccessParam(url, "POST", parameters)));

            WebRequest req = WebRequest.Create(url);
            req.Method        = "POST";
            req.ContentType   = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;

            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            WebResponse res  = req.GetResponse();
            using (Stream stream = res.GetResponseStream()) {
                using (StreamReader reader = new StreamReader(stream)) {
                    if (url.EndsWith("json")) {
                        return DynamicJson.Parse(reader.ReadToEnd());
                    }
                    else {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        /**
         *
         */
        private IDictionary<string, string> MergeAccessParam(string url , string method, IDictionary<string, string> inParam) {
            SortedDictionary<string, string> param = GenerateParameters(AccessToken);
            foreach (var key in inParam.Keys) {
                if (!param.ContainsKey(key)) {
                    param.Add(key , UrlEncode(inParam[key]));
                }
            }
            if (!String.IsNullOrEmpty(AccessToken)) {
                string signature = GenerateSignature(AccessTokenSecret, method , url, param);
                param.Add("oauth_signature", UrlEncode(signature));
            }
            return param;
        }
        /**
         *
         */
        protected Dictionary<string, string> ParseResponse(string response) {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string s in response.Split('&')) {
                int index = s.IndexOf('=');
                if (index == -1) {
                    result.Add(s, "");
                }
                else {
                    result.Add(s.Substring(0, index), s.Substring(index + 1));
                }
            }
            return result;
        }
        /**
         *
         */
        private string JoinParameters(IDictionary<string, string> parameters) {
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (var parameter in parameters) {
                if (first) {
                    first = false;
                }
                else {
                    result.Append('&');
                }
                result.Append(parameter.Key);
                result.Append('=');
                result.Append(parameter.Value);
            }
            return result.ToString();
        }
        /**
         *
         */
        protected string GenerateSignature(string tokenSecret, string httpMethod, string url, SortedDictionary<string, string> parameters) {
            string signatureBase = GenerateSignatureBase(httpMethod, url, parameters);
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(UrlEncode(ConsumerSecret) + '&' + UrlEncode(tokenSecret));
            byte[] data = System.Text.Encoding.ASCII.GetBytes(signatureBase);
            byte[] hash = hmacsha1.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
        /**
         *
         */
        private string GenerateSignatureBase(string httpMethod, string url, SortedDictionary<string, string> parameters) {
            StringBuilder result = new StringBuilder();
            result.Append(httpMethod);
            result.Append('&');
            result.Append(UrlEncode(url));
            result.Append('&');
            result.Append(UrlEncode(JoinParameters(parameters)));
            return result.ToString();
        }
        /**
         *
         */
        protected SortedDictionary<string, string> GenerateParameters(string token) {
            SortedDictionary<string, string> result = new SortedDictionary<string, string>();
            result.Add("oauth_consumer_key"     , ConsumerKey);
            result.Add("oauth_signature_method" , "HMAC-SHA1");
            result.Add("oauth_timestamp"        , GenerateTimestamp());
            result.Add("oauth_nonce"            , GenerateNonce());
            result.Add("oauth_version"          , "1.0");
            if (!string.IsNullOrEmpty(token)) {
                result.Add("oauth_token", token);
            }
            return result;
        }
        /**
         *
         */
        protected string UrlEncode(string value) {
            string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder result = new StringBuilder();
            byte[] data = Encoding.UTF8.GetBytes(value);
            foreach (byte b in data) {
                if (b < 0x80 && unreserved.IndexOf((char)b) != -1) {
                    result.Append((char)b);
                }
                else {
                    result.Append('%' + String.Format("{0:X2}", (int)b));
                }
            }
            return result.ToString();
        }
        /**
         *
         */
        private string GenerateNonce() {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(8);
            for (int i = 0 ; i < 8 ; ++i) {
                result.Append(letters[random.Next(letters.Length)]);
            }
            return result.ToString();
        }
        /**
         *
         */
        private string GenerateTimestamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}
