using System;
using System.IO;
using System.Collections.Generic;
using Codeplex.Data;
using Twitter;

namespace Twitter {

    class Twitter {

        private static string PROP_PATH = "./twitter.properties";

        static void Main(string[] args) {
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

            // ↓ここらへんは後でちゃんとwrapしたい

            // タイムラインから3件取得してみる
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("count", "3");
            dynamic res = auth.Get("http://twitter.com/statuses/home_timeline.json", parameters);
            foreach (dynamic status in res) {
                Console.WriteLine(status.user.screen_name + " " + status.text);
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
        /*
         *
         */
        private static dynamic LoadConfig() {
            using (FileStream fs = new FileStream(PROP_PATH , FileMode.Open)) {
                using (StreamReader sr = new StreamReader(fs)) {
                    return DynamicJson.Parse(sr.ReadToEnd());
                }
            }
        }
        /*
         *
         */
        private static void SaveConfig(dynamic config) {
            using (FileStream fs = new FileStream(PROP_PATH , FileMode.Open)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.WriteLine(config.ToString());
                }
            }
        }
    }
}
