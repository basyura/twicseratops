
namespace BasyuraOrg.Twitter {
    /*
     *
     */
    public class TwicseraConf {
        /** */
        private const string CONSUMER_KEY    = "IGWZ6nY3v3cHBh0yfj6RJw";
        /** */
        private const string CONSUMER_SECRET = "zfwTlwwjGSAKChGwJi5DpJSBsIlZ7HE3ZCMUfelCk";
        /*
         *
         */
        public TwicseraConf() {
        }
        /*
         *
         */
        public TwicseraConf(string accessToken, string accessTokenSecret) {
            AccessToken       = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }
        /*
         *
         */
        public string ConsumerKey {
            get {
                return CONSUMER_KEY;
            }
        }
        /*
         *
         */
        public string ConsumerSecret {
            get {
                return CONSUMER_SECRET;
            }
        }
        /* */
        public string AccessToken { get; private set; }
        /* */
        public string AccessTokenSecret { get; private set;}
    }
}
