using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    public class ClosestStationHistory
    {
        public DateTime date { get; set; }
        public List<ClosestStation> closeststationlist{get;set;}
        public ClosestStationHistory(List<ClosestStation> closeststationlist, DateTime date)
        {
            this.date = date;
            this.closeststationlist = closeststationlist;
        }
    }
}