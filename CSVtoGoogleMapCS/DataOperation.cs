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
        List<GPSData> gpsdatalist;

        public DataOperation(List<GPSData> gpsdatalist)
        {
            this.gpsdatalist = gpsdatalist;
        }

        //緯度と経度から最寄駅のリストを検索する
        public List<String> requestStationNameList(GPSData postion)
        {
            String url = "http://express.heartrails.com/api/json?method=getStations&x=135.0&y=35.0";
            var req = WebRequest.Create(url);
            req.Headers.Add("Accept-Language:ja,en-us;q=0.7,en;q=0.3");
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
                Debug.WriteLine(" distance={0}, line={1}, name={2}\n, next={3}, postal={4}, prev={5},\n x={6}, y={7}", r.distance, r.line, r.name,r.next,r.postal,r.prev,r.x,r.y);
            }
            return new List<string>{"StationName1","StationName2"};
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
