using System;
using BasyuraOrg.Twitter;

class Class1 {
    static void Main(string[] args) {
        TwicseraRegister register = Twicseratops.NewRegister();
        string url = register.GetAuthorizeUrl();

        Console.WriteLine("次のURLにアクセスして暗証番号を取得してください：");
        Console.WriteLine(url);
        Console.Write("暗証番号：");
        string pin = Console.ReadLine().Trim();

        string[] keys = register.GetAccessTokenAndSecret(pin);

        Console.WriteLine("access token        : " + keys[0]);
        Console.WriteLine("access token secret : " + keys[1]);
    }
}
