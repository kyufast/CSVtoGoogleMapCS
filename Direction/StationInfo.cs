using CSVtoGoogleMapCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direction
{
    class StationInfo : Station
    {
        public String prefecture { get; set;}
        public StationInfo(String line, String name, String next, int postal, String prefecture, String prev, double x, double y)
        {
            this.line = line;
            this.name = name;
            this.next = next;
            this.postal = postal;
            this.prefecture = prefecture;
            this.prev = prev;
            this.x = x;
            this.y = y;
        }

        public static List<StationInfo> stationInfoResultConvertToStationInfoList(StationInfoResult sir)
        {
            List<StationInfo> stationinfolist = new List<StationInfo>();
            foreach (var r in sir.response.stationinfo)
            {
                stationinfolist.Add(stationInfoJOSNConvertToStationInfo(r));
            }
            return stationinfolist;
        }

        public static StationInfo stationInfoJOSNConvertToStationInfo(StationInfoResult.Response.StationInfo sirrs)
        {
            return new StationInfo(sirrs.line,sirrs.name,sirrs.next,sirrs.postal,sirrs.prefecture,sirrs.prev,sirrs.x,sirrs.y);
        }
    }
}
