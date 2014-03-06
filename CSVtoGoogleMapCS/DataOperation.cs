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
    class DataOperation
    {
        List<GPSHistoryData> gpsdatalist { get; set; }
        List<GPSHistoryData> formatgpslist { get; set; }

        public DataOperation(List<GPSHistoryData> gpsdatalist)
        {
            this.gpsdatalist = gpsdatalist;
            formatgpslist = new List<GPSHistoryData>();
            formatGpsHistroyList();
        }

        private Boolean formatGpsHistroyList()
        {
            GPSHistoryData nowhistory = null;
            GPSHistoryData nexthistory = null;

            foreach (var historydata in gpsdatalist)
            {
                if (nowhistory == null)
                {
                    nowhistory = historydata;
                    formatgpslist.Add(historydata);
                    continue;
                }
                nexthistory = historydata;
                if (!nowhistory.Date.AddSeconds(1).Equals(nexthistory.Date))
                {
                    TimeSpan ts = nexthistory.Date - nowhistory.Date;
                    int diff = (int)ts.TotalSeconds;
                    for (int i = 1; i <= diff; i++)
                    {
                        formatgpslist.Add(new GPSHistoryData(nowhistory.Date.AddSeconds(i), nowhistory.Latitude + (nexthistory.Latitude - nowhistory.Latitude) / (double)diff * i, nowhistory.Longitude + (nexthistory.Longitude - nowhistory.Longitude) / (double)diff * i));
                    }
                }
                else
                {
                    formatgpslist.Add(nexthistory);
                }

                nowhistory = nexthistory;
            }
            return true;
        }

        private List<MoveGPSStationHistory> calcStation()
        {
            List<MoveGPSStationHistory> movestationlist = new List<MoveGPSStationHistory>();
            //初めに動いているかどうか
            
            
            //動き出す

            //止まる
            


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
                Debug.WriteLine(" distance={0}, line={1}, name={2},\n next={3}, postal={4}, prev={5},\n x={6}, y={7}", r.distance, r.line, r.name,r.next,r.postal,r.prev,r.x,r.y);
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
