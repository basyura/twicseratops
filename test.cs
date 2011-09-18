using Codeplex.Data;
using System;
using System.IO;
using System.Text;

class Class1 {
    private static string PROP_NAME = "./test.propertes";
    static void Main(string[] args) {

        string data = "{\"name\" : \"basyura\"}";
        if (File.Exists(PROP_NAME)) {
            Console.WriteLine("read from " + PROP_NAME);
            FileStream    fs = new FileStream(PROP_NAME , FileMode.Open);
            StreamReader  sr = new StreamReader(fs);
            StringBuilder sb = new StringBuilder();
            String line = null;
            while ((line = sr.ReadLine()) != null) {
                sb.Append(line);
            }
            data = sb.ToString();
            sr.Close();
            fs.Close();
        }
        else {
           Console.WriteLine("write to " + PROP_NAME);
           FileStream fs = File.Create(PROP_NAME);
           StreamWriter sw = new StreamWriter(fs);
           sw.WriteLine(data);
           sw.Close();
           fs.Close();
        }

        dynamic json = DynamicJson.Parse(data);
        Console.WriteLine(json.name);
        json.name = "hoge";
        Console.WriteLine(json.name);
        Console.WriteLine(json.ToString());
    }
}
