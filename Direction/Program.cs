using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic; // List
using System.Runtime.Serialization; // DataContract,DataMember
using System.Runtime.Serialization.Json;

namespace Direction
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        public String getUPorDown(String nowstationname, String nextstationname, String linename)
        {
            //上り,下り
            String direction = "不明";
            String baseURL = "http://express.heartrails.com/api/json?method=getStations&line=";
            String url = baseURL + linename;

            var req = WebRequest.Create(url);
            var res = req.GetResponse();
            StationInfoResult stationinfo;
            using (res)
            {
                using (var resStream = res.GetResponseStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(StationInfoResult));
                    stationinfo = (StationInfoResult)serializer.ReadObject(resStream);
                }
            }

            return direction;
        }
    }
}
