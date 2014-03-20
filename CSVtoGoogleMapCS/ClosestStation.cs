using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class ClosestStation : Station
    {
        public ClosestStation(String distance, String line, String name, String next, int postal, String prev, double x, double y)
        {
            this.distance = distance;
            this.line = line;
            this.name = name;
            this.next = next;
            this.postal = postal;
            this.prev = prev;
            this.x = x;
            this.y = y;
        }

        public string distance { get; set; }
    }
}
