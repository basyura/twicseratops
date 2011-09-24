using Codeplex.Data;
using System;
using System.IO;
using System.Text;

class Class1 {
    private static string PROP_NAME = "./test.properties";
    static void Main(string[] args) {

        dynamic json = null;
        if (File.Exists(PROP_NAME)) {
            Console.WriteLine("read from " + PROP_NAME);
            using (FileStream fs = new FileStream(PROP_NAME, FileMode.Open)) {
                using (StreamReader sr = new StreamReader(fs)) {
                    json = DynamicJson.Parse(sr.ReadToEnd());
                }
            }
        }
        else {
           Console.WriteLine("write to " + PROP_NAME);
           json = new DynamicJson();
           json.name = "basyura";
           // 厳密には別ファイルを作成した後で mv するべきだよなぁ
           using (FileStream fs = new FileStream(PROP_NAME, FileMode.Create)) {
               using (StreamWriter sw = new StreamWriter(fs)) {
                   sw.WriteLine(json.ToString());
               }
           }
        }

        Console.WriteLine(json.name);
        json.name = "hoge";
        Console.WriteLine(json.name);
        Console.WriteLine(json.ToString());
    }
}
