using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class GPSFormatHistoryData : GPSHistoryData
    {
        //前の位置と今の位置で計算
        public double speed { get; private set; }

        public GPSFormatHistoryData(DateTime date, double latitude, double longitude, double speed): base(date,latitude, longitude){
            this.speed = speed;
        }
        public GPSFormatHistoryData(GPSHistoryData gpshistroydata, double speed)
            : base(gpshistroydata.Datetime, gpshistroydata.Latitude, gpshistroydata.Longitude)
        {
            this.speed = speed;
        }

    }
}
