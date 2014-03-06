using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    class GPSHistoryData : GPSData
    {

        public DateTime Date { get; private set; }

        public GPSHistoryData(DateTime date, double latitude, double longitude)
            : base(latitude, longitude)
        {
            this.Date = date;
        }
    }
}
