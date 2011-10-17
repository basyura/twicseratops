using System;
using BasyuraOrg.Twitter;

class Class1 {
    static void Main(string[] args) {
        string consumerKey    = "IGWZ6nY3v3cHBh0yfj6RJw";
        string consumerSecret = "zfwTlwwjGSAKChGwJi5DpJSBsIlZ7HE3ZCMUfelCk";

        string url = Twicseratops.GetAuthorizeUrl(consumerKey, consumerSecret);

        Console.WriteLine("次のURLにアクセスして暗証番号を取得してください：");
        Console.WriteLine(url);
        Console.Write("暗証番号：");
        string pin = Console.ReadLine().Trim();

        string[] keys = Twicseratops.GetAccessTokenAndSecret(consumerKey, consumerSecret, pin);

        Console.WriteLine("access token        : " + keys[0]);
        Console.WriteLine("access token secret : " + keys[1]);
    }
}
