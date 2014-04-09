using CSVtoGoogleMapCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Direction
{
    public class StationInfoOperation
    {
        public String getUPorDown(String nowstationname, String nextstationname, String linename)
        {
            //上り,下り
            String direction = "不明";
            List<StationInfo> stationinfolist = requestStationInfoList(linename);
            int nowstationindex = -1;
            int nextstationindex = -1;
            for (int i = 0; i < stationinfolist.Count(); i++)
            {
                if (stationinfolist[i].name.Equals(nowstationname))
                {
                    nowstationindex = i;
                }
                else if (stationinfolist[i].name.Equals(nextstationname))
                {
                    nextstationindex = i;
                }
            }
            if (nowstationindex >= nextstationindex)
            {
                direction = "上り";
            }
            else
            {
                direction = "下り";
            }

            return direction;
        }

        //リストの始端に近いほうが東京より
        public List<StationInfo> requestStationInfoList(String linename)
        {
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
            return stationInfoResultConvertToStationInfoList(stationinfo);
        }

        public List<StationInfo> stationInfoResultConvertToStationInfoList(StationInfoResult sir)
        {
            List<StationInfo> stationinfolist = new List<StationInfo>();
            foreach (var r in sir.response.station)
            {
                stationinfolist.Add(stationInfoJOSNConvertToStationInfo(r));
            }
            return stationinfolist;
        }

        public StationInfo stationInfoJOSNConvertToStationInfo(StationInfoResult.Response.StationInfo sirrs)
        {
            return new StationInfo(sirrs.line, sirrs.name, sirrs.next, sirrs.postal, sirrs.prefecture, sirrs.prev, sirrs.x, sirrs.y);
        }
    }
}
