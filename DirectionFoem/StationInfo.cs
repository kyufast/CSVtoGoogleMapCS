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
    public class StationInfo : Station
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
    }
}
