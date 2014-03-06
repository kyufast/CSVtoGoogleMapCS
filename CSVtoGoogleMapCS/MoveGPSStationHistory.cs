using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    class MoveGPSStationHistory
    {
        public GPSHistoryData arrivehistory { get; set; }
        public GPSHistoryData leavehistory { get; set; }

        public MoveGPSStationHistory()
        {
            this.arrivehistory = null;
            this.leavehistory = null;
        }

    }
}
