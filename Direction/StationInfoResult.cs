using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic; // List
using System.Runtime.Serialization; // DataContract,DataMember
using System.Runtime.Serialization.Json;
namespace Direction
{
    [DataContract]
    public class StationInfoResult
    {
        [DataMember]
        public Response response { get; set; }

        [DataContract]
        public class Response
        {
            [DataMember]
            public List<StationInfo> stationinfo { get; set; }

            [DataContract]
            public class StationInfo
            {
                [DataMember]
                public string line { get; set; }
                [DataMember]
                public string name { get; set; }
                [DataMember]
                public string next { get; set; }
                [DataMember]
                public int postal { get; set; }
                [DataMember]
                public string prefecture { get; set; }
                [DataMember]
                public string prev { get; set; }
                [DataMember]
                public double x { get; set; }
                [DataMember]
                public double y { get; set; }
            }
        }
    }
}
