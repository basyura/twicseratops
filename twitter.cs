using System;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;
using Codeplex.Data;
using Twitter;
using System.Text.RegularExpressions;

namespace Twitter {
    /**
     *
     */
    class Twitter : DynamicObject {
        /** */
        private const string API_URL   = "https://api.twitter.com/1";
        /** */
        private const string PROP_PATH = "./twitter.properties";
        /** */
        const string APIS = @"
          update_status           /statuses/update                POST
          remove_status           /statuses/destroy/{0}           DELETE
          public_timeline         /statuses/public_timeline
          home_timeline           /statuses/home_timeline
          friends_timeline        /statuses/friends_timeline
          replies                 /statuses/replies
          mentions                /statuses/mentions
          user_timeline           /statuses/user_timeline/{0}
          show                    /statuses/show/{0}
          friends                 /statuses/friends/{0}
          followers               /statuses/followers/{0}
          retweet                 /statuses/retweet/{0}           POST
          retweets                /statuses/retweets/{0}
          retweeted_by_me         /statuses/retweeted_by_me
          retweeted_to_me         /statuses/retweeted_to_me
          retweets_of_me          /statuses/retweets_of_me
          user                    /users/show/{0}
          direct_messages         /direct_messages
          sent_direct_messages    /direct_messages/sent
          send_direct_message     /direct_messages/new            POST
          remove_direct_message   /direct_messages/destroy/{0}    DELETE
          follow                  /friendships/create/{0}         POST
          leave                   /friendships/destroy/{0}        DELETE
          friendship_exists       /friendships/exists
          followers_ids           /followers/ids/{0}
          friends_ids             /friends/ids/{0}
          favorites               /favorites/{0}
          favorite                /favorites/create/{0}           POST
          remove_favorite         /favorites/destroy/{0}          DELETE
          verify_credentials      /account/verify_credentials     GET
          end_session             /account/end_session            POST
          update_delivery_device  /account/update_delivery_device POST
          update_profile_colors   /account/update_profile_colors  POST
          limit_status            /account/rate_limit_status
          update_profile          /account/update_profile         POST
          enable_notification     /notifications/follow/{0}       POST
          disable_notification    /notifications/leave/{0}        POST
          block                   /blocks/create/{0}              POST
          unblock                 /blocks/destroy/{0}             DELETE
          block_exists            /blocks/exists/{0}              GET
          blocking                /blocks/blocking                GET
          blocking_ids            /blocks/blocking/ids            GET
          saved_searches          /saved_searches                 GET
          saved_search            /saved_searches/show/{0}        GET
          create_saved_search     /saved_searches/create          POST
          remove_saved_search     /saved_searches/destroy/{0}     DELETE
          create_list             /{0}/lists                      POST
          update_list             /{0}/lists/{1}                  PUT
          DELETE_list             /{0}/lists/{1}                  DELETE
          list                    /{0}/lists/{1}
          lists                   /{0}/lists
          lists_followers         /{0}/lists/memberships
          list_statuses           /{0}/lists/{1}/statuses
          list_members            /{0}/{1}/members
          add_member_to_list      /{0}/{1}/members                POST
          remove_member_from_list /{0}/{1}/members                DELETE
          list_following          /{0}/{1}/subscribers
          follow_list             /{0}/{1}/subscribers            POST
          remove_list             /{0}/{1}/subscribers            DELETE
          ";

        private static Dictionary<string, string> API_MAP = new Dictionary<string, string>();
        /**
         *
         */
        static Twitter() {
            Regex regex = new Regex(@"\s+");
            foreach (string line in APIS.Split('\n')) {
                if (String.IsNullOrEmpty(line.Trim())) {
                    continue;
                }
                string[] api = regex.Split(line.Trim());
                API_MAP[api[0]] = API_URL + api[1] + ".json";
                string method = "GET";
                if (api.Length > 2) {
                    method = api[2];
                }
                API_MAP[api[0] + ":method"] = method;
            }
        }
        /** */
        private Auth auth_ = null;
        /**
         *
         */
        public Twitter() {
            auth_ = newAuth();
        }
        /*
         *
         */
        public dynamic Request(string api , params object[] args) {
            Dictionary<string, string> inParam = new Dictionary<string, string>();
            if (args.Length != 0 && args[args.Length - 1] is Dictionary<string, string>) {
                inParam = (Dictionary<string, string>)args[args.Length - 1];
                Array.Resize(ref args, args.Length - 1);
            }

            string url = String.Format(API_MAP[api] , args);

            if ("GET" == API_MAP[api + ":method"]) {
                return auth_.HttpGet(url  , inParam);
            }
            else {
                return auth_.HttpPost(url , inParam);
            }
        }
        /*
         *
         */
        public dynamic Update(string status) {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                {"status" , status}
            };
            return Request("update_status", param);
        }
        /**
         *
         */
        private Auth newAuth() {
            Auth auth;
            //var settings = Twitter.Properties.Settings.Default;
            dynamic config = LoadConfig();

            if (!config.IsDefined("access_token")) {
                auth = new Auth(config.consumer_key, config.consumer_secret);

                // リクエストトークンを取得する
                auth.GetRequestToken();

                // ユーザーにRequestTokenを認証してもらう
                Console.WriteLine("次のURLにアクセスして暗証番号を取得してください：");
                Console.WriteLine(auth.GetAuthorizeUrl());
                Console.Write("暗証番号：");
                string pin = Console.ReadLine().Trim();

                // アクセストークンを取得する
                auth.GetAccessToken(pin);

                // 結果を表示する
                Console.WriteLine("AccessToken       : " + auth.AccessToken);
                Console.WriteLine("AccessTokenSecret : " + auth.AccessTokenSecret);
                Console.WriteLine("UserId            : " + auth.UserId);
                Console.WriteLine("ScreenName        : " + auth.ScreenName);

                // アクセストークンを設定ファイルに保存する
                config.access_token        = auth.AccessToken;
                config.access_token_secret = auth.AccessTokenSecret;
                config.userId              = auth.UserId;
                config.screen_name         = auth.ScreenName;
                SaveConfig(config);
            }
            else {
                // 設定ファイルから読み込む
                auth = new Auth(config.consumer_key, 
                        config.consumer_secret,
                        config.access_token, 
                        config.access_token_secret,
                        config.userId ,
                        config.screen_name);
            }
            return auth;
        }
        /*
         *
         */
        private dynamic LoadConfig() {
            using (FileStream fs = new FileStream(PROP_PATH , FileMode.Open)) {
                using (StreamReader sr = new StreamReader(fs)) {
                    return DynamicJson.Parse(sr.ReadToEnd());
                }
            }
        }
        /*
         *
         */
        private void SaveConfig(dynamic config) {
            using (FileStream fs = new FileStream(PROP_PATH , FileMode.Open)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.WriteLine(config.ToString());
                }
            }
        }
        static void Main(string[] args) {

            Twitter twitter = new Twitter();

            dynamic res = twitter.Request("list_statuses" , "basyura" , "all");
            foreach (dynamic status in res) {
                Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
            }

                           
            res = twitter.Request("replies");
            foreach (dynamic status in res) {
                Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
            }

            //twitter.Request("update_status" , new Dictionary<string, string>() {{"status" , "ほむほむ"}});
        }
    }
}
