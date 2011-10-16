/* http://www.pxsta.net/blog/?p=603 */
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using OAuth;

class TwitterOAuth {
    static void Main(string[] args) {
        string consumer_key    = "IGWZ6nY3v3cHBh0yfj6RJw";
        string consumer_secret = "zfwTlwwjGSAKChGwJi5DpJSBsIlZ7HE3ZCMUfelCk";
        Class1 cls = new Class1();
        Console.WriteLine(cls.GetAccessToken(consumer_key , consumer_secret));

    }
    public TwitterOAuth(string consumer_key , string consumer_secret) {
    }
    public string GetAccessToken(string consumer_key, string consumer_secret)    
    {
        System.Net.ServicePointManager.Expect100Continue = false;
        OAuthBase oAuth = new OAuthBase();
        Uri uri = new Uri("http://twitter.com/oauth/request_token");
        //OAuthBace.csを用いてsignature生成
        string normalizedUrl, normalizedRequestParameters;
        string signature = oAuth.GenerateSignature(
                                    uri ,
                                    consumer_key, consumer_secret, "", "",
                                    "GET", GenerateTimestamp() , GenerateNonce(), OAuthBase.SignatureTypes.HMACSHA1, 
                                    out normalizedUrl, out normalizedRequestParameters);


        //oauth_token,oauth_token_secret取得
        string param = string.Format("http://twitter.com/oauth/request_token?{0}&oauth_signature={1}", 
                                      normalizedRequestParameters, signature);
        HttpWebRequest webreq = (System.Net.HttpWebRequest)WebRequest.Create(param);
        webreq.Method = "GET";
        HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

        Stream st = webres.GetResponseStream();
        StreamReader sr = new StreamReader(st, Encoding.GetEncoding(932));
               
        string result = sr.ReadToEnd();
               
        sr.Close();
        st.Close();
        Console.WriteLine(result);            
               
        //正規表現でoauth_token,oauth_token_secret取得
        string token       = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=.*?&oauth_callback.*").Groups[1].Value;
        string tokenSecret = Regex.Match(result, @"oauth_token=(.*?)&oauth_token_secret=(.*?)&oauth_callback.*").Groups[2].Value;
       

        //ブラウザからPIN確認
        string AuthorizeURL = (string.Format("http://twitter.com/oauth/authorize?{0}", result));
        System.Diagnostics.Process.Start(AuthorizeURL);
        Console.Write("PIN:");
        string PIN = Console.ReadLine();

               
        //oauth_token,oauth_token_secretを用いて再びsignature生成
        signature = oAuth.GenerateSignature(
                            uri, 
                            consumer_key, consumer_secret, token, tokenSecret, 
                            "POST", GenerateTimestamp(), GenerateNonce(), OAuthBase.SignatureTypes.HMACSHA1, 
                            out normalizedUrl, out normalizedRequestParameters);
        string req_param = string.Format("http://twitter.com/oauth/access_token?{3}&oauth_signature={0}&oauth_verifier={2}", 
                                          signature, result, PIN, normalizedRequestParameters);
        webreq = (System.Net.HttpWebRequest)WebRequest.Create(req_param);


        //oauth_token,oauth_token_secretの取得
        webreq.Method = "POST";
        webres = (System.Net.HttpWebResponse)webreq.GetResponse();

        st = webres.GetResponseStream();
        sr = new StreamReader(st, Encoding.GetEncoding(932));
               
        result = sr.ReadToEnd();
               
        sr.Close();
        st.Close();

        return result;
        //デスクトップ\oauth_token.txtに保存
        //File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\oauth_token.txt",result);
    }

    private string GenerateNonce()
    {
        string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder result = new StringBuilder(8);
        Random random = new Random();
        for (int i = 0; i < 8; ++i)
            result.Append(letters[random.Next(letters.Length)]);
        return result.ToString();
    }

    private string GenerateTimestamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
}
