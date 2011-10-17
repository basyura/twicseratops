using System;
using System.Collections.Generic;
using BasyuraOrg.Twitter;

class Client {
    static void Main(string[] args) {

        dynamic twitter = new Twicseratops();

        Dictionary<string, string> param = new Dictionary<string, string> {
            {"per_page" , "100"}
        };
        Console.WriteLine("----------- show home timeline -----------------");
        foreach (dynamic status in twitter.HomeTimeline()) {
            Console.WriteLine(status.user.screen_name + " : " + status.text);
        }
        Console.WriteLine("----------- show list status -----------------");
        foreach (dynamic status in twitter.ListStatuses("basyura" , "all" , param)) {
            Console.WriteLine(status.user.screen_name + " : " + status.text);
        }
        Console.WriteLine("------------ show replies ----------------");
        foreach (dynamic status in twitter.Replies()) {
            Console.WriteLine(status.user.screen_name + " : " + status.text);
        }
        Console.WriteLine("------------ post statuses ----------------");

        if (args.Length != 0) {
            Console.WriteLine("post ... " + args[0]);
            twitter.Update(args[0]);
        }
    }
}
