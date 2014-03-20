using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class Station
    {
        public string line { get; set; }
        public string name { get; set; }
        public string next { get; set; }
        public int postal { get; set; }
        public string prev { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }
}
