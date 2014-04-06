using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace CSVtoGoogleMapCS
{
    
    public class DataOperation
    {
        List<GPSHistoryData> gpsdatalist { get; set; }
        List<GPSFormatHistoryData> formatgpsdatalist { get; set; }
        public  List<MoveGPSStationHistory> movestationhistory { get; private set;}
        public List<ClosestStationHistory> listrawcloseststationlist { get; set; }
        public List<ClosestStationHistory> listformatcloseststationlist { get; set; }
        public DataOperation(List<GPSHistoryData> gpsdatalist)
        {
            this.gpsdatalist = gpsdatalist;
            formatgpsdatalist = new List<GPSFormatHistoryData>();
            listrawcloseststationlist = new List<ClosestStationHistory>();
            formatGpsHistroyList();
            movestationhistory = calcStation();

        }

        private Boolean formatGpsHistroyList()
        {
            GPSHistoryData prehistory = null;
            GPSHistoryData nowhistory = null;
            GPSHistoryData[] gpsdataarray = gpsdatalist.ToArray();

            double prespeed = 0;
            double nowspeed = 0;
            foreach (var historydata in gpsdataarray)
            {
                if (prehistory == null)
                {
                    prespeed = GPSUtilities.speedKMPerhour(gpsdataarray[1], gpsdataarray[0]);
                    nowspeed = prespeed;
                    prehistory = historydata;
                    nowhistory = prehistory;
                    formatgpsdatalist.Add(new GPSFormatHistoryData(nowhistory, nowspeed));
                    continue;
                }
                nowhistory = historydata;
                nowspeed = GPSUtilities.speedKMPerhour(nowhistory, prehistory);
                if (!prehistory.Datetime.AddSeconds(1).Equals(nowhistory.Datetime))
                {
                    TimeSpan ts = nowhistory.Datetime - prehistory.Datetime;
                    int diff = (int)ts.TotalSeconds;
                    if ((prehistory.Latitude == nowhistory.Latitude) && (prehistory.Longitude == nowhistory.Longitude) && (prespeed >= 10))
                    {
                        for (int i = 1; i <= diff; i++)
                        {
                            formatgpsdatalist.Add(new GPSFormatHistoryData(
                                new GPSHistoryData(prehistory.Datetime.AddSeconds(i), prehistory.Latitude + (nowhistory.Latitude - prehistory.Latitude) / (double)diff * i, prehistory.Longitude + (nowhistory.Longitude - prehistory.Longitude) / (double)diff * i),
                                prespeed));
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= diff; i++)
                        {
                            formatgpsdatalist.Add(new GPSFormatHistoryData(
                                new GPSHistoryData(prehistory.Datetime.AddSeconds(i), prehistory.Latitude + (nowhistory.Latitude - prehistory.Latitude) / (double)diff * i, prehistory.Longitude + (nowhistory.Longitude - prehistory.Longitude) / (double)diff * i),
                                nowspeed));
                        }
                    }
                }
                else
                {
                    nowspeed = GPSUtilities.speedKMPerhour(nowhistory, prehistory);
                    if((nowspeed == 0)&&(prespeed>=10))
                    {
                        formatgpsdatalist.Add(new GPSFormatHistoryData(nowhistory, prespeed));
                    }
                    else
                    {
                        formatgpsdatalist.Add(new GPSFormatHistoryData(nowhistory, nowspeed));
                    }
                }
                prehistory = nowhistory;
                prespeed = nowspeed;
            }


            System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\test.csv");
            foreach (var tempformat in formatgpsdatalist)
            {
                sw.WriteLine(tempformat.Datetime+","+tempformat.Latitude+","+tempformat.Longitude+","+tempformat.speed);
                //Debug.WriteLine(" Latitude={0}, Longitude={1}, speed={2}", tempformat.Latitude, tempformat.Longitude, tempformat.speed);
            }
            //閉じる
            sw.Close();
            return true;
        }

        private List<MoveGPSStationHistory> calcStation()
        {
            List<MoveGPSStationHistory> movestationlist = new List<MoveGPSStationHistory>();
            List<GPSFormatHistoryData> tempgpshis = new List<GPSFormatHistoryData>();
            List<ClosestStationHistory> closeststationhistorylist = new List<ClosestStationHistory>();

            Person person = new Person();

            GPSFormatHistoryData[] formatgpsdataarray = formatgpsdatalist.ToArray();

            if (formatgpsdataarray == null)
            {
                return null;
            }
            {
                int i = 0;
                foreach (var gpsdata in formatgpsdataarray)
                {
                    i++;
                    //始め
                    if (person.getStatus() == Person.MovingStatus.start)
                    {
                        if (gpsdata.speed <= 1.0)
                        {
                            person.setStatusStopping();
                        }
                        else
                        {
                            person.setStatusOnTrain();
                        }
                        continue;
                    }

                    //2つ目以降
                    if (Person.MovingStatus.ontrain == person.getStatus())
                    {
                        //停止
                        if (gpsdata.speed <= 0.5)
                        {
                            person.setStatusStopping();
                            //tempgpshis.Add(gpsdata);
                            //Debug.WriteLine("line={0}", i);
                        }
                    }
                    else if (Person.MovingStatus.stopping == person.getStatus())
                    {
                        //動き出す
                        if (gpsdata.speed > 15.0)
                        {
                            tempgpshis.Add(gpsdata);
                            person.setStatusOnTrain();
                        }
                    }

                }
            }
            Debug.WriteLine(tempgpshis.Count());
            System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\test2.csv");
            foreach (var value in tempgpshis)
            {
                sw.WriteLine(value.Datetime + "," + value.Latitude + "," + value.Longitude + "," + value.speed);
            }
            sw.Close();
            foreach (var temp in tempgpshis)
            {

                //Debug.WriteLine(" Latitude={0}, Longitude={1}, speed={2}", temp.Latitude, temp.Longitude, temp.speed);
                listrawcloseststationlist.Add(new ClosestStationHistory(requestStationList(temp), temp.Datetime));
            }
            //int j = 0;
            //foreach (var closeststationlist in listrawcloseststationlist)
            //{
            //    j++;
            //    Debug.WriteLine(j+": "+closeststationlist[0].name);
            //}

            System.IO.StreamWriter sw2 = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\StationDebug.csv");
            sw2.WriteLine("最寄駅一覧" + "," + "駅名" + "," + "路線名" + "," + "距離" + "," + "前の駅" + "," + "次の駅" + "," + "駅の緯度" + "," + "駅の経度");
            foreach (var closeststationgroup in listrawcloseststationlist)
            {
                sw2.WriteLine("停止");
                DateTime attime = closeststationgroup.date;
                foreach (var closeststation in closeststationgroup.closeststationlist)
                {
                    sw2.WriteLine("," + closeststation.name + "," + closeststation.line + "," + closeststation.distance + "," + closeststation.prev + "," + closeststation.next + "," + closeststation.x + "," + closeststation.y + "," + attime);
                }
            }
            sw2.Close();


            closeststationhistorylist = formatClosestStationList(listrawcloseststationlist);
            ClosestStationHistory[] closeststationhistoryarray = closeststationhistorylist.ToArray();

            List<ClosestStationHistory> tocalccloseststationhistorylist = new List<ClosestStationHistory>();

            System.IO.StreamWriter swLEAVE = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\StationDebugLEAVE.csv");
            for (int j = 0; j < closeststationhistoryarray.Length; j++)
            {
                ClosestStationHistory closeststationhistroyset = closeststationhistoryarray[j];
                String useline;
                String usestation;
                DateTime leavetime;

                //次がない場合
                if (j+1 >= closeststationhistoryarray.Length)
                {
                    break;
                }
                //一つしか駅がない場合
                if (closeststationhistroyset.closeststationlist.Count == 1)
                {
                    tocalccloseststationhistorylist.Add(closeststationhistroyset);
                    //usestation = closeststationhistroyset.closeststationlist[0].name;
                    //useline = closeststationhistroyset.closeststationlist[0].line;
                    //leavetime = closeststationhistroyset.date;
                }
                else//駅や路線が複数ある場合
                {
                    List<ClosestStation> tempcloseststaionlist = new List<ClosestStation>();
                    List<ClosestStation> tempnextcloseststationlist = new List<ClosestStation>();
                    List<ClosestStation> nowcloseststationlist = new List<ClosestStation>();
                    nowcloseststationlist = closeststationhistroyset.closeststationlist;
                    foreach (ClosestStation nowcloseststation in nowcloseststationlist)
                    {
                        foreach (ClosestStation nextcloseststation in closeststationhistoryarray[j + 1].closeststationlist)
                        {
                            if (nowcloseststation.line.Equals(nextcloseststation.line))
                            {
                                tempcloseststaionlist.Add(nowcloseststation);
                                tempnextcloseststationlist.Add(nextcloseststation);
                                break;
                            }
                        }
                    }
                    ClosestStation[] tempcloseststationarray = tempcloseststaionlist.ToArray();
                    ClosestStation[] tempnextcloseststationarray = tempnextcloseststationlist.ToArray();
                    int templength = tempnextcloseststationarray.Length;
                    int[] sumdistance = new int[templength];
                    int minimum=-1;
                    int minimumindex=-1;
                    for (int k = 0; k < tempcloseststationarray.Length; k++)
                    {
                        sumdistance[k] = (int.Parse(tempcloseststationarray[k].distance.Replace("m","")) +int.Parse(tempnextcloseststationarray[k].distance.Replace("m","")));
                    }
                    for (int k = 0; k < sumdistance.Length; k++)
                    {
                        if (k == 0)
                        {
                            minimum = sumdistance[k];
                            minimumindex = k;
                        }
                        else
                        {
                            if (minimum > sumdistance[k])
                            {
                                minimum = sumdistance[k];
                                minimumindex = k;
                            }
                        }
                    }

                    //usestation = tempcloseststationarray[minimumindex].name;
                    //useline = tempcloseststationarray[minimumindex].line;
                    //leavetime = closeststationhistroyset.date;
                    List<ClosestStation> tempcloseststationlist = new List<ClosestStation>();
                    tempcloseststaionlist.Add(tempcloseststationarray[minimumindex]);
                    ClosestStationHistory tempans = new ClosestStationHistory(tempcloseststaionlist, closeststationhistroyset.date);
                    tocalccloseststationhistorylist.Add(tempans);
                }

            }
            foreach (var tocalccloseststationhistory in tocalccloseststationhistorylist)
            {
                swLEAVE.WriteLine("駅");
                swLEAVE.WriteLine("," + tocalccloseststationhistory.closeststationlist[0].name + "," + tocalccloseststationhistory.closeststationlist[0].line + "," + tocalccloseststationhistory.date);
            }
            swLEAVE.Close();

            return movestationlist;
        }

        public List<ClosestStationHistory> formatClosestStationList(List<ClosestStationHistory> rawcloseststationlist)
        {
            List<ClosestStationHistory> pre1formatClosestStationList = new List<ClosestStationHistory>();
            List<ClosestStationHistory> pre2formatClosestStationList = new List<ClosestStationHistory>();
            List<ClosestStationHistory> formatClosestStationList = new List<ClosestStationHistory>();
            //同じ路線は最も近い駅に設定する
            foreach (ClosestStationHistory rawcloseststationset in rawcloseststationlist)
            {
                List<ClosestStation> tempcloseststationset = new List<ClosestStation>();
                foreach (ClosestStation closeststation in rawcloseststationset.closeststationlist)
                {
                    Boolean isSameLine = false;
                    if (tempcloseststationset != null)
                    {
                        foreach (ClosestStation tempcloseststation in tempcloseststationset)
                        {
                            if (tempcloseststation.line.Equals(closeststation.line))
                            {
                                isSameLine = true;
                                break;
                            }
                        }
                    }
                    if (isSameLine == false)
                    {
                        tempcloseststationset.Add(closeststation);
                    }
                }
                pre1formatClosestStationList.Add(new ClosestStationHistory(tempcloseststationset,rawcloseststationset.date));
            }

            System.IO.StreamWriter swformat1 = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\StationDebugFormat1.csv");
            swformat1.WriteLine("最寄駅一覧" + "," + "駅名" + "," + "路線名" + "," + "距離" + "," + "前の駅" + "," + "次の駅" + "," + "駅の緯度" + "," + "駅の経度");
            foreach (var closeststationgroup in pre1formatClosestStationList)
            {
                swformat1.WriteLine("停止");
                DateTime attime = closeststationgroup.date;
                foreach (var closeststation in closeststationgroup.closeststationlist)
                {
                    swformat1.WriteLine("," + closeststation.name + "," + closeststation.line + "," + closeststation.distance + "," + closeststation.prev + "," + closeststation.next + "," + closeststation.x + "," + closeststation.y + "," + attime);
                }
            }
            swformat1.Close();

            //同じ停車駅Listの場合は最も遅いものを選択する
            ClosestStationHistory nowcloseststationhistoryset=null;
            ClosestStationHistory lastcloseststationhistroyset = null;
            foreach (ClosestStationHistory nextcloseststationset in pre1formatClosestStationList)
            {
                if (nowcloseststationhistoryset == null)
                {
                    nowcloseststationhistoryset = nextcloseststationset;
                    lastcloseststationhistroyset = nextcloseststationset;
                    continue;
                }
                else if (nowcloseststationhistoryset.closeststationlist.Count != nextcloseststationset.closeststationlist.Count)
                {
                    pre2formatClosestStationList.Add(nowcloseststationhistoryset);
                    nowcloseststationhistoryset = nextcloseststationset;
                    lastcloseststationhistroyset = nextcloseststationset;
                    continue;
                }
                else
                {
                    ClosestStation[] nowcloseststationarray = nowcloseststationhistoryset.closeststationlist.ToArray();
                    ClosestStation[] nextcloseststationarray = nextcloseststationset.closeststationlist.ToArray();
                    for (int i = 0; i < nowcloseststationhistoryset.closeststationlist.Count; i++)
                    {
                        if (!(nowcloseststationarray[i].name.Equals(nextcloseststationarray[i].name)))
                        {
                            pre2formatClosestStationList.Add(nowcloseststationhistoryset);
                            break;
                        }
                    }
                }
                nowcloseststationhistoryset = nextcloseststationset;
                lastcloseststationhistroyset = nextcloseststationset;

            }
            pre2formatClosestStationList.Add(lastcloseststationhistroyset);

            formatClosestStationList = pre2formatClosestStationList;
            //同じ


            System.IO.StreamWriter swformat = new System.IO.StreamWriter("C:\\Users\\tasopo\\Documents\\CSV\\StationDebugFormat2.csv");
            swformat.WriteLine("最寄駅一覧" + "," + "駅名" + "," + "路線名" + "," + "距離" + "," + "前の駅" + "," + "次の駅" + "," + "駅の緯度" + "," + "駅の経度");
            foreach (var closeststationgroup in formatClosestStationList)
            {
                swformat.WriteLine("停止");
                DateTime attime = closeststationgroup.date;
                foreach (var closeststation in closeststationgroup.closeststationlist)
                {
                    swformat.WriteLine("," + closeststation.name + "," + closeststation.line + "," + closeststation.distance + "," + closeststation.prev + "," + closeststation.next + "," + closeststation.x + "," + closeststation.y + "," + attime);
                }
            }
            swformat.Close();

            return formatClosestStationList;
        }

        //緯度と経度から最寄駅のリストを検索する
        //路線ごとに異なる駅と判断します
        public List<ClosestStation> requestStationList(GPSHistoryData postion)
        {
            String baseurl = "http://express.heartrails.com/api/json?method=getStations";
            String x = postion.Longitude.ToString();
            String y = postion.Latitude.ToString();
            //String x = "135.0";
            //String y = "35.0";
            //String x = "139.766092";
            //String y = "35.681283";
            String url = baseurl + "&x=" + x + "&y=" + y;

            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            ClosestStationResult closeststaion;
            using (res)
            {
                using (var resStream = res.GetResponseStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(ClosestStationResult));
                    closeststaion = (ClosestStationResult)serializer.ReadObject(resStream);
                }
            }
            //Debug.WriteLine("message: " + closeststaion.response);
            //Debug.WriteLine("record count: " + closeststaion.response.station.Count);
            foreach (var r in closeststaion.response.station)
            {
                //Debug.WriteLine(" distance={0}, line={1}, name={2},\n next={3}, postal={4}, prev={5},\n x={6}, y={7}", r.distance, r.line, r.name, r.next, r.postal, r.prev, r.x, r.y);
            }
            return ClosestStationResultConvertToClosestStationList(closeststaion);
        }

        public List<ClosestStation> ClosestStationResultConvertToClosestStationList(ClosestStationResult csr)
        {
            List<ClosestStation> closeststationlist = new List<ClosestStation>();
            foreach (var r in csr.response.station)
            {
                closeststationlist.Add(ClosestStationJSONConvertToClosestStation(r));
            }
            return closeststationlist;
        }

        public ClosestStation ClosestStationJSONConvertToClosestStation(ClosestStationResult.Response.Station csrrs)
        {
            return new ClosestStation(csrrs.distance, csrrs.line, csrrs.name, csrrs.next, csrrs.postal, csrrs.prev, csrrs.x, csrrs.y);
        }


    }

    


    [DataContract]
    public class ClosestStationResult
    {
        [DataMember]
        public Response response { get; set; }

        [DataContract]
        public class Response
        {
            [DataMember]
            public List<Station> station { get; set; }
            
            [DataContract]
            public class Station
            {
                [DataMember]
                public string distance { get; set; }
                [DataMember]
                public string line { get; set; }
                [DataMember]
                public string name { get; set; }
                [DataMember]
                public string next { get; set; }
                [DataMember]
                public int postal { get; set; }
                [DataMember]
                public string prev { get; set; }
                [DataMember]
                public double x { get; set; }
                [DataMember]
                public double y { get; set; }
            }
        }
    }

}
