using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class GPSHistoryData : GPSData
    {

        public DateTime Datetime { get; private set; }

        public GPSHistoryData(DateTime date, double latitude, double longitude)
            : base(latitude, longitude)
        {
            this.Datetime = date;
        }
    }
}
