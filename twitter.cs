using System;
using System.IO;
using System.Collections.Generic;
using Codeplex.Data;
using Twitter;

namespace Twitter {
    /**
     *
     */
    class Twitter {
        /** */
        private static string PROP_PATH = "./twitter.properties";
        /** */
        private Auth auth_ = null;
        /**
         *
         */
        public Twitter() {
            auth_ = newAuth();
        }
        /**
         *
         */
        public dynamic HomeTimeline() {
            return auth_.Get("home_timeline", new Dictionary<string, string>());
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
            dynamic res = twitter.HomeTimeline();
            foreach (dynamic status in res) {
                Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
            }

            /*
            // ポストしてみる
            Console.WriteLine("いまどうしてる？");
            string status = Console.ReadLine();
            parameters.Clear();
            parameters.Add("status", auth.UrlEncode(status));
            Console.WriteLine(auth.Post("http://twitter.com/statuses/update.xml", parameters));
            */
        }
    }
}
