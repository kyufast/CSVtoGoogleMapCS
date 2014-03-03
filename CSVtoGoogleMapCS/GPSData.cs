using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    class GPSData
    {
        
        public DateTime Date{get; private set;}
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public GPSData(DateTime date, double latitude, double longitude){
            this.Date = date;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}
