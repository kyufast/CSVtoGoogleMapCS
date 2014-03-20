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

        public DataOperation(List<GPSHistoryData> gpsdatalist)
        {
            this.gpsdatalist = gpsdatalist;
            formatgpsdatalist = new List<GPSFormatHistoryData>();
            formatGpsHistroyList();
            movestationhistory = calcStation();

        }

        private Boolean formatGpsHistroyList()
        {
            GPSHistoryData nowhistory = null;
            GPSHistoryData nexthistory = null;
            List<GPSHistoryData> formatgpslist = new List<GPSHistoryData>();
            foreach (var historydata in gpsdatalist)
            {
                if (nowhistory == null)
                {
                    nowhistory = historydata;
                    formatgpslist.Add(historydata);
                    continue;
                }
                nexthistory = historydata;
                if (!nowhistory.Datetime.AddSeconds(1).Equals(nexthistory.Datetime))
                {
                    TimeSpan ts = nexthistory.Datetime - nowhistory.Datetime;
                    int diff = (int)ts.TotalSeconds;
                    for (int i = 1; i <= diff; i++)
                    {
                        formatgpslist.Add(new GPSHistoryData(nowhistory.Datetime.AddSeconds(i), nowhistory.Latitude + (nexthistory.Latitude - nowhistory.Latitude) / (double)diff * i, nowhistory.Longitude + (nexthistory.Longitude - nowhistory.Longitude) / (double)diff * i));
                    }
                }
                else
                {
                    formatgpslist.Add(nexthistory);
                }

                nowhistory = nexthistory;
            }


            GPSHistoryData nowhistory2 = null;
            GPSHistoryData nexthistory2 = null;
            GPSHistoryData[] formatgpsarr = formatgpslist.ToArray();
            foreach (var historydata in formatgpsarr)
            {
                if (nowhistory2 == null)
                {
                    nowhistory2 = historydata;
                    formatgpsdatalist.Add(new GPSFormatHistoryData(historydata, GPSUtilities.speedKMPerhour(formatgpsarr[1],formatgpsarr[0])));
                    continue;
                }
                nexthistory2 = historydata;
                formatgpsdatalist.Add(new GPSFormatHistoryData(historydata, GPSUtilities.speedKMPerhour(nexthistory2, nowhistory2)));
                nowhistory2 = nexthistory2;
            }
            foreach (var tempformat in formatgpsdatalist)
            {
                Debug.WriteLine(" Latitude={0}, Longitude={1}, speed={2}", tempformat.Latitude, tempformat.Longitude, tempformat.speed);
            }
            return true;
        }

        private List<MoveGPSStationHistory> calcStation()
        {
            List<MoveGPSStationHistory> movestationlist = new List<MoveGPSStationHistory>();
            List<GPSFormatHistoryData> tempgpshis = new List<GPSFormatHistoryData>();
            Person person = new Person();

            GPSFormatHistoryData[] formatgpsdataarray = formatgpsdatalist.ToArray();

            if(formatgpsdataarray == null){
                return null;
            }

            foreach (var gpsdata in formatgpsdataarray)
            {
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
                    if (gpsdata.speed <= 0.5)
                    {
                        person.setStatusStopping();
                        tempgpshis.Add(gpsdata);
                    }
                }
                else if (Person.MovingStatus.stopping == person.getStatus())
                {
                    if (gpsdata.speed > 15.0)
                    {
                        person.setStatusOnTrain();
                    }
                }

                
            }


            foreach (var temp in tempgpshis)
            {
                Debug.WriteLine(" Latitude={0}, Longitude={1}, speed={2}", temp.Latitude, temp.Longitude, temp.speed);
            }


            return movestationlist;
        }

        //緯度と経度から最寄駅のリストを検索する
        //路線ごとに異なる駅と判断します
        public List<ClosestStation> requestStationList(GPSHistoryData postion)
        {
            String baseurl = "http://express.heartrails.com/api/json?method=getStations";
            //String x = postion.Latitude.ToString();
            //String y = postion.Longitude.ToString();
            //String x = "135.0";
            //String y = "35.0";
            String x = "139.766092";
            String y = "35.681283";
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
            Debug.WriteLine("message: " + closeststaion.response);
            Debug.WriteLine("record count: " + closeststaion.response.station.Count);
            foreach (var r in closeststaion.response.station)
            {
                //Debug.WriteLine(" distance={0}, line={1}, name={2},\n next={3}, postal={4}, prev={5},\n x={6}, y={7}", r.distance, r.line, r.name,r.next,r.postal,r.prev,r.x,r.y);
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
