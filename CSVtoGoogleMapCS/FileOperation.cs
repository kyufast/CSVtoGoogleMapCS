using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class FileOperation
    {
        public String InputFilePath { get; set; }
        public String OutputDirctoryPath { get; set; }
        List<GPSHistoryData> gpsdatalist= new List<GPSHistoryData>();

        public Boolean calcLogToGoogleMap()
        {
            setGPSDataList();
            DataOperation dataoperation = new DataOperation(gpsdatalist);
            csvToHTML();
            csvToHTMLGraph();

            return true;
        }

        public static System.IO.StreamWriter DebugStreamWriter(String debugfilename){
            String outputfile;
                outputfile = getDebugOutputDirctoryPath()+debugfilename;
            if(!System.IO.Directory.Exists(getDebugOutputDirctoryPath())){
                System.IO.Directory.CreateDirectory(getDebugOutputDirctoryPath());
            }
            return new System.IO.StreamWriter(outputfile);
        }


        private Boolean setGPSDataList()
        {
            if (this.InputFilePath == null)
            {
                return false;
            }

            System.IO.StreamReader srcsv = new System.IO.StreamReader(InputFilePath);
            //ヘッダー行
            String strcsv = srcsv.ReadLine();
            //内容1行目
            strcsv = srcsv.ReadLine();
            while (strcsv != null)
            {
                String[] contents = strcsv.Split(',');
                gpsdatalist.Add(new GPSHistoryData(DateTime.Parse(contents[0]),double.Parse(contents[1]),double.Parse(contents[2])));
                strcsv = srcsv.ReadLine();
            }
            srcsv.Close();
            return true;

        }        

        public Boolean csvToHTML()
        {
            String outputfile;
            if (this.InputFilePath == null)
            {
                return false;
            }
            else
            {
                outputfile = getRawOutputFilePath()+"_output.html";
            }          

            System.IO.StreamReader sr = new
              System.IO.StreamReader("..\\..\\gpssample.html");
            System.IO.StreamReader srcsv = new System.IO.StreamReader(InputFilePath);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputfile);

            String str = sr.ReadLine();
            //内容を一行ずつ読み込む
            while (sr.Peek() > -1)
            {
                sw.WriteLine(str);
                if (str.IndexOf("StartArray()") != -1)
                {
                    //ヘッダー行
                    String strcsv = srcsv.ReadLine();
                    //内容1行目
                    strcsv = srcsv.ReadLine();
                    long i = 0;
                    while (strcsv != null)
                    {
                        String[] contents = strcsv.Split(',');
                        sw.WriteLine("patharray[" + i
                                + "] = new google.maps.LatLng(" + contents[1]
                                + "," + contents[2] + ");");
                        i++;
                        strcsv = srcsv.ReadLine();
                    }                    
                }
                str = sr.ReadLine();
            }
            //閉じる
            sw.Close();
            srcsv.Close();
            sr.Close();
            return true;


        }

        private Boolean csvToHTMLGraph()
        {
            String outputfile;
            int pass=1;
            if (this.InputFilePath == null)
            {
                return false;
            }
            else
            {
                outputfile = getRawOutputFilePath() + "_Graph.html";
            }

            System.IO.StreamReader sr = new
              System.IO.StreamReader("..\\..\\Graph.html");
            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputfile);

            //行数によってある程度グラフのよみ飛ばし
            {
                System.IO.StreamReader srcsv = new System.IO.StreamReader(InputFilePath);
                String strcsv = srcsv.ReadToEnd();
                int csvline = strcsv.ToList().Where(c => c.Equals('\n')).Count();
                srcsv.Close();

                //長時間記録されてる場合は数を減らす
                if (csvline > 1000)
                {
                    int i;
                    for (i = 1; (csvline / i)>=1000; i++)
                    {
                    }
                    pass = i;
                }
                else
                {
                    pass = 1;
                }
            }

            String str = sr.ReadLine();
            while (sr.Peek() > -1)
            {
                sw.WriteLine(str);
                if (str.IndexOf("TimeInput") != -1)
                {
                    System.IO.StreamReader srcsv = new System.IO.StreamReader(InputFilePath);
                    // ヘッダー行
                    String strcsv = srcsv.ReadLine();
                    // 内容1行目
                    strcsv = srcsv.ReadLine();

                    while (srcsv.Peek() > -1)
                    {
                        String[] contents = strcsv.Split(',');
                        sw.WriteLine("<th scope=\"col\">" + contents[0] + "</th>");
                        //長時間記録されてる場合は数を減らす
                        for (int i = 0; i < pass; i++)
                        {
                            strcsv = srcsv.ReadLine();
                        }
                    }
                    srcsv.Close();
                }

                if (str.IndexOf("SpeedInput") != -1)
                {
                    System.IO.StreamReader srcsv = new System.IO.StreamReader(InputFilePath);
                    // ヘッダー行
                    String strcsv = srcsv.ReadLine();
                    // 内容1行目
                    strcsv = srcsv.ReadLine();

                    while (srcsv.Peek() > -1)
                    {
                        String[] contents = strcsv.Split(',');
                        sw.WriteLine("<td>" + contents[6] + "</td>");
                        //長時間記録されてる場合は数を減らす
                        for (int i = 0; i < pass; i++)
                        {
                            strcsv = srcsv.ReadLine();
                        }
                    }
                    srcsv.Close();
                }
                str = sr.ReadLine();

            }
            sw.Close();
            sr.Close();

            return true;

        }

        private String getRawOutputFilePath()
        {
            String outputfile;
            if (this.OutputDirctoryPath == null)
            {
                outputfile = this.InputFilePath.Substring(0, this.InputFilePath.LastIndexOf('.'));
            }
            else
            {
                int start = this.InputFilePath.LastIndexOf("\\") + 1;
                int end = this.InputFilePath.LastIndexOf('.') - start;
                outputfile = OutputDirctoryPath + "\\" + InputFilePath.Substring(start, end);
            }
            return outputfile;
        }

        private static String getDebugOutputDirctoryPath()
        {
            String outputdirctory = "C:\\Users\\tasopo\\Documents\\CSV\\Debug\\";
            //if (this.OutputDirctoryPath == null)
            //{
            //    outputdirctory = this.InputFilePath.Substring(0,this.InputFilePath.LastIndexOf("\\")-1)+"Debug"+"\\";
            //}
            //else
            //{
            //    outputdirctory = OutputDirctoryPath+"Debug"+"\\";
            //}
            
            return outputdirctory;
        }
    }
}
