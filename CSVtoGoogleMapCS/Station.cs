using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class Station
    {
        public Station() { }
        public Station(String line, String name, String next, int postal, String prev, double x, double y)
        {
            this.line = line;
            this.name = name;
            this.next = next;
            this.postal = postal;
            this.prev = prev;
            this.x = x;
            this.y = y;
        }
        public string line { get; set; }
        public string name { get; set; }
        public string next { get; set; }
        public int postal { get; set; }
        public string prev { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }
}
