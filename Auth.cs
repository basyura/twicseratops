/*
 * original : http://d.hatena.ne.jp/nojima718/20100129/1264792636
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;
using Codeplex.Data;

namespace Twitter {
    public class Auth {
        const string REQUEST_TOKEN_URL = "https://twitter.com/oauth/request_token";
        const string ACCESS_TOKEN_URL  = "https://twitter.com/oauth/access_token";
        const string AUTHORIZE_URL     = "https://twitter.com/oauth/authorize";
        const string API_URL           = "http://twitter.com";

        const string APIS = @"
          update_status           /statuses/update                post
          remove_status           /statuses/destroy/{0}           delete
          public_timeline         /statuses/public_timeline
          home_timeline           /statuses/home_timeline
          friends_timeline        /statuses/friends_timeline
          replies                 /statuses/replies
          mentions                /statuses/mentions
          user_timeline           /statuses/user_timeline/{0}
          show                    /statuses/show/{0}
          friends                 /statuses/friends/{0}
          followers               /statuses/followers/{0}
          retweet                 /statuses/retweet/{0}           post
          retweets                /statuses/retweets/{0}
          retweeted_by_me         /statuses/retweeted_by_me
          retweeted_to_me         /statuses/retweeted_to_me
          retweets_of_me          /statuses/retweets_of_me
          user                    /users/show/{0}
          direct_messages         /direct_messages
          sent_direct_messages    /direct_messages/sent
          send_direct_message     /direct_messages/new            post
          remove_direct_message   /direct_messages/destroy/{0}    delete
          follow                  /friendships/create/{0}         post
          leave                   /friendships/destroy/{0}        delete
          friendship_exists       /friendships/exists
          followers_ids           /followers/ids/{0}
          friends_ids             /friends/ids/{0}
          favorites               /favorites/{0}
          favorite                /favorites/create/{0}           post
          remove_favorite         /favorites/destroy/{0}          delete
          verify_credentials      /account/verify_credentials     get
          end_session             /account/end_session            post
          update_delivery_device  /account/update_delivery_device post
          update_profile_colors   /account/update_profile_colors  post
          limit_status            /account/rate_limit_status
          update_profile          /account/update_profile         post
          enable_notification     /notifications/follow/{0}       post
          disable_notification    /notifications/leave/{0}        post
          block                   /blocks/create/{0}              post
          unblock                 /blocks/destroy/{0}             delete
          block_exists            /blocks/exists/{0}              get
          blocking                /blocks/blocking                get
          blocking_ids            /blocks/blocking/ids            get
          saved_searches          /saved_searches                 get
          saved_search            /saved_searches/show/{0}        get
          create_saved_search     /saved_searches/create          post
          remove_saved_search     /saved_searches/destroy/{0}     delete
          create_list             /{0}/lists                      post
          update_list             /{0}/lists/{1}                  put
          delete_list             /{0}/lists/{1}                  delete
          list                    /{0}/lists/{1}
          lists                   /{0}/lists
          lists_followers         /{0}/lists/memberships
          list_statuses           /{0}/lists/{1}/statuses
          list_members            /{0}/{1}/members
          add_member_to_list      /{0}/{1}/members                post
          remove_member_from_list /{0}/{1}/members                delete
          list_following          /{0}/{1}/subscribers
          follow_list             /{0}/{1}/subscribers            post
          remove_list             /{0}/{1}/subscribers            delete
          ";

        private static Dictionary<string, string> API_MAP = new Dictionary<string, string>();

        static Auth() {
            Regex regex = new Regex(@"\s+");
            foreach (string line in APIS.Split('\n')) {
                if (String.IsNullOrEmpty(line.Trim())) {
                    continue;
                }
                string[] api = regex.Split(line.Trim());
                API_MAP[api[0]] = API_URL + api[1] + ".json";
            }
        }

        private Random random = new Random();

        public string ConsumerKey        { get; private set; }
        public string ConsumerSecret     { get; private set; }
        public string RequestToken       { get; private set; }
        public string RequestTokenSecret { get; private set; }
        public string AccessToken        { get; private set; }
        public string AccessTokenSecret  { get; private set; }
        public string UserId             { get; private set; }
        public string ScreenName         { get; private set; }


        public Auth(string consumerKey, string consumerSecret) {
            ServicePointManager.Expect100Continue = false;
            ConsumerKey    = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public Auth(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string userId, string screenName) {
            ServicePointManager.Expect100Continue = false;
            ConsumerKey       = consumerKey;
            ConsumerSecret    = consumerSecret;
            AccessToken       = accessToken;
            AccessTokenSecret = accessTokenSecret;
            UserId            = userId;
            ScreenName        = screenName;
        }

        public void GetRequestToken() {
            SortedDictionary<string, string> parameters = GenerateParameters("");
            string signature = GenerateSignature("", "GET", REQUEST_TOKEN_URL, parameters);
            parameters.Add("oauth_signature", UrlEncode(signature));

            dynamic response = HttpGet(REQUEST_TOKEN_URL, parameters);

            Dictionary<string, string> dic = ParseResponse(response);
            RequestToken       = dic["oauth_token"];
            RequestTokenSecret = dic["oauth_token_secret"];
        }

        public string GetAuthorizeUrl() {
            return AUTHORIZE_URL + "?oauth_token=" + RequestToken;
        }

        public void GetAccessToken(string pin) {
            SortedDictionary<string, string> parameters = GenerateParameters(RequestToken);
            parameters.Add("oauth_verifier" , pin);
            string signature = GenerateSignature(RequestTokenSecret, "GET", ACCESS_TOKEN_URL, parameters);
            parameters.Add("oauth_signature", UrlEncode(signature));
            dynamic response = HttpGet(ACCESS_TOKEN_URL, parameters);

            Dictionary<string, string> dic = ParseResponse(response);
            AccessToken       = dic["oauth_token"];
            AccessTokenSecret = dic["oauth_token_secret"];
            UserId            = dic["user_id"];
            ScreenName        = dic["screen_name"];
        }

        public dynamic Get(string method, string[] args) {

            string url = String.Format(API_MAP[method] , args);

            SortedDictionary<string, string> parameters = GenerateParameters(AccessToken);
            string signature = GenerateSignature(AccessTokenSecret, "GET", url, parameters);
            parameters.Add("oauth_signature", UrlEncode(signature));
            return HttpGet(url, parameters);
        }

        public string Post(string url, IDictionary<string, string> parameters) {
            SortedDictionary<string, string> parameters2 = GenerateParameters(AccessToken);
            foreach (var p in parameters) {
                parameters2.Add(p.Key, p.Value);
            }
            string signature = GenerateSignature(AccessTokenSecret, "POST", url, parameters2);
            parameters2.Add("oauth_signature", UrlEncode(signature));
            return HttpPost(url, parameters2);
        }

        private dynamic HttpGet(string url, IDictionary<string, string> parameters) {
            WebRequest  req = WebRequest.Create(url + '?' + JoinParameters(parameters));
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

        string HttpPost(string url, IDictionary<string, string> parameters) {
            byte[] data = Encoding.ASCII.GetBytes(JoinParameters(parameters));

            WebRequest req = WebRequest.Create(url);
            req.Method        = "POST";
            req.ContentType   = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;

            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            WebResponse res  = req.GetResponse();
            Stream resStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
            string result = reader.ReadToEnd();

            reader.Close();
            resStream.Close();
            return result;

        }

        private Dictionary<string, string> ParseResponse(string response) {
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

        private string GenerateSignature(string tokenSecret, string httpMethod, string url, SortedDictionary<string, string> parameters) {
            string signatureBase = GenerateSignatureBase(httpMethod, url, parameters);
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(UrlEncode(ConsumerSecret) + '&' + UrlEncode(tokenSecret));
            byte[] data = System.Text.Encoding.ASCII.GetBytes(signatureBase);
            byte[] hash = hmacsha1.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }

        private string GenerateSignatureBase(string httpMethod, string url, SortedDictionary<string, string> parameters) {
            StringBuilder result = new StringBuilder();
            result.Append(httpMethod);
            result.Append('&');
            result.Append(UrlEncode(url));
            result.Append('&');
            result.Append(UrlEncode(JoinParameters(parameters)));
            return result.ToString();
        }

        private SortedDictionary<string, string> GenerateParameters(string token) {
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

        public string UrlEncode(string value) {
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

        private string GenerateNonce() {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(8);
            for (int i = 0 ; i < 8 ; ++i) {
                result.Append(letters[random.Next(letters.Length)]);
            }
            return result.ToString();
        }

        private string GenerateTimestamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}
