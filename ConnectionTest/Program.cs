using System;
using System.Net;
using System.IO;
using System.Text;

namespace ConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://www.atmarkit.co.jp/fdotnet/";


            System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\webdata.html");

            WebClient wc = new WebClient();
            Stream st = wc.OpenRead(url);


            StreamReader sr = new StreamReader(st,
                                Encoding.GetEncoding("Shift_JIS"));
            String temp = sr.ReadToEnd();

            sw.WriteLine(temp);
            sr.Close();
            st.Close();
            sw.Close();
        }
    }
}
