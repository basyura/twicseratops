using System;
using System.Collections.Generic;
using BasyuraOrg.Twitter;

class Client {
    static void Main(string[] args) {

        dynamic twitter = new Twicseratops();

        Dictionary<string, string> param = new Dictionary<string, string> {
            {"per_page" , "100"}
        };
        foreach (dynamic status in twitter.ListStatuses("basyura" , "all" , param)) {
            Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
        }
        Console.WriteLine("----------------------------");
        foreach (dynamic status in twitter.Replies()) {
            Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
        }
        Console.WriteLine("----------------------------");
        foreach (dynamic status in twitter.HomeTimeline()) {
            Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
        }

        twitter.Update("(=^・^=)");
    }
}
