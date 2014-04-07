// openread.cs
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

class WebOpenRead
{
    public static void Main()
    {

        //受け取れるようにしなくてはならないこと(駅名&路線名&時刻&○○方面)

        String baseurl1 = "http://www.ekikara.jp/cgi-bin/find.cgi?month=newdata&text=";
        String baseurl2 = "&kind=station&half=on&cut=on&direct=on&max=30&isTop=on&Submit=%E6%A4%9C%E7%B4%A2";
        String stationname = "身延";
        String linename = "身延線";
        //String stationname = "東京";
        //String linename = "京葉線";
        List<String> linestationlist = new List<String> { "東京", "八丁堀", "越中島", "潮見", "新木場", "葛西臨海公園", "舞浜", "新浦安", "市川塩浜", "二俣新町", "南船橋", "新習志野", "海浜幕張", "検見川浜", "稲毛海岸", "千葉みなと", "蘇我" };
        int hour;
        int minute;
        List<String> srlist = new List<String>();
        String srall = null;
        String nowURL = null;



        string url = "http://www.ekikara.jp/cgi-bin/find.cgi?month=newdata&text=%E6%9D%B1%E4%BA%AC&kind=station&half=on&cut=on&direct=on&max=30&isTop=on&Submit=%E6%A4%9C%E7%B4%A2";
        System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\webdata.html", false, Encoding.GetEncoding("Shift_JIS"));


        //駅名選択

        WebClient wc = new WebClient();
        Stream st = wc.OpenRead(baseurl1 + stationname + baseurl2);
        //Stream       st = wc.OpenRead(url);
        nowURL = baseurl1 + stationname + baseurl2;
        StreamReader sr = new StreamReader(st,
                            Encoding.GetEncoding("Shift_JIS"));

        while (sr.Peek() >= 0)
        {
            String temp = sr.ReadLine();
            srlist.Add(temp);
            srall += temp;

        }
        if (srall.IndexOf("が該当しました") < 0)
        {
            Debug.WriteLine("駅が一択に絞れる");
        }
        else
        {
            Debug.WriteLine("複数駅候補");
            int endsearch = srall.IndexOf(stationname + "(");
            if (endsearch >= 0)
            {
                int startsearch = srall.IndexOf("rsltSet01");
                int start = srall.IndexOf("http://", startsearch, endsearch - startsearch);
                int end = 4 + srall.IndexOf(".htm", startsearch, endsearch - startsearch);
                String StationURL = srall.Substring(start, end - start);
                WebClient wc2 = new WebClient();
                Stream st2 = wc2.OpenRead(StationURL);
                StreamReader sr2 = new StreamReader(st2,
                            Encoding.GetEncoding("Shift_JIS"));
                srall = sr2.ReadToEnd();
            }
        }


        //路線選択
        int startsearchline = srall.IndexOf(linename + "</");
        int startgraph = srall.IndexOf("【駅時刻表】", startsearchline);
        int endgraph = srall.IndexOf("【路線時刻表】", startgraph + 1);
        //int endstaurl = srall.IndexOf("蘇我"+"方面");
        int endstaurl = srall.IndexOf("西富士宮" + "方面");
        int stationgraphURLstart = 3 + srall.LastIndexOf("../ekijikoku/", endstaurl, endstaurl - startgraph);
        int stationgraphURLend = 4 + srall.LastIndexOf(".htm", endstaurl, endstaurl - stationgraphURLstart);
        String StationGraphURL = srall.Substring(stationgraphURLstart, stationgraphURLend - stationgraphURLstart);
        String baselineURL = "http://www.ekikara.jp/newdata/";
        String LineURL = baselineURL + StationGraphURL;

        //乗車列車選択
        WebClient wc3 = new WebClient();
        Stream st3 = wc3.OpenRead(LineURL);
        StreamReader sr3 = new StreamReader(st3,
            Encoding.GetEncoding("Shift_JIS"));
        String srall3 = sr3.ReadToEnd();


        hour = 15;
        minute = 34;
        //発射の場合は15時32,31,30分に発射するのものがあるか調べる

        int searchstarttimehour = srall3.IndexOf(hour.ToString() + "</span>");
        int searchendtimehour = srall3.IndexOf((hour + 1).ToString() + "</span>");
        String timeURL = null;
        for (int i = 0; i < 30; i++)
        {
            int searchtime = srall3.IndexOf((minute - i).ToString() + "</a>", searchstarttimehour, searchendtimehour - searchstarttimehour);
            if (searchtime >= 0)
            {
                int starttimeURL = 6 + srall3.LastIndexOf("../../", searchtime, searchtime - searchstarttimehour);
                int endtimeURL = 4 + srall3.LastIndexOf(".htm", searchtime, searchtime - searchstarttimehour);
                String temptimeURL = srall3.Substring(starttimeURL, endtimeURL - starttimeURL);
                timeURL = baselineURL + temptimeURL;
                break;
            }
        }

        //列車番号検索
        WebClient wc4 = new WebClient();
        Stream st4 = wc4.OpenRead(timeURL);
        StreamReader sr4 = new StreamReader(st4,
            Encoding.GetEncoding("Shift_JIS"));
        String srall4 = sr4.ReadToEnd();

        int trainnumbersearchstart = srall4.IndexOf("列車番号");
        int trainnumbersearchend = srall4.IndexOf("列車予約コード");
        int trainnumberend = -5 + srall4.LastIndexOf("</span>", trainnumbersearchend, trainnumbersearchend - trainnumbersearchstart);
        int trainnumberstart = 25 + srall4.LastIndexOf("<span class=\"l\">", trainnumberend, trainnumberend - trainnumbersearchstart);
        String temptrainnumber = srall4.Substring(trainnumberstart, trainnumberend - trainnumberstart);
        String trainnumber = temptrainnumber.Trim();


        sw.WriteLine(srlist);
        Console.WriteLine(srlist);

        sr.Close();
        st.Close();
        sw.Close();
    }

    public String getUPorDown(String nowstationname, String nextstationname, String linename)
    {
        //上り,下り
        String direction = "不明";
        String baseURL = "http://express.heartrails.com/api/json?method=getStations&line=";


        return direction;
    }

}
