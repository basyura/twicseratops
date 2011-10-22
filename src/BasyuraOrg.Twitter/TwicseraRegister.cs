using System;
using System.Collections.Generic;
using BasyuraOrg.Twitter;

namespace BasyuraOrg.Twitter {
    /**
     *
     */
    public class TwicseraRegister : TwicseraAuth {
        /** */
        const string REQUEST_TOKEN_URL = "https://twitter.com/oauth/request_token";
        /** */
        const string ACCESS_TOKEN_URL  = "https://twitter.com/oauth/access_token";
        /** */
        const string AUTHORIZE_URL     = "https://twitter.com/oauth/authorize";
        /** */
        private string requestToken_       = null;
        /** */
        private string requestTokenSecret_ = null;
        /*
         *
         */
        public TwicseraRegister(TwicseraConf conf) : base(conf) {
        }
        /*
         *
         */
        public string GetAuthorizeUrl() {
            GetRequestToken();
            return AUTHORIZE_URL + "?oauth_token=" + requestToken_;
        }
        /*
         *
         */
        public String[] GetAccessTokenAndSecret(string pin) {
            SortedDictionary<string, string> parameters = GenerateParameters(requestToken_);
            parameters.Add("oauth_verifier" , pin);
            string signature = GenerateSignature(requestTokenSecret_, "GET", ACCESS_TOKEN_URL, parameters);
            parameters.Add("oauth_signature", UrlEncode(signature));
            dynamic response = HttpGet(ACCESS_TOKEN_URL, parameters);

            Dictionary<string, string> dic = ParseResponse(response);

            return new string[] { dic["oauth_token"], dic["oauth_token_secret"] };
        }
        /**
         *
         */
        private void GetRequestToken() {
            SortedDictionary<string, string> parameters = GenerateParameters("");
            string signature = GenerateSignature("", "GET", REQUEST_TOKEN_URL, parameters);
            parameters.Add("oauth_signature", UrlEncode(signature));

            dynamic response = HttpGet(REQUEST_TOKEN_URL, parameters);

            Dictionary<string, string> dic = ParseResponse(response);
            requestToken_       = dic["oauth_token"];
            requestTokenSecret_ = dic["oauth_token_secret"];
        }
    }
}
