using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoGoogleMapCS
{
    class GPSUtilities
    {
        //  http://yamadarake.jp/trdi/report000001.html
        public const double GRS80_A = 6378137.000;
        public const double GRS80_E2 = 0.00669438002301188;
        public const double GRS80_MNUM = 6335439.32708317;


        public static double deg2rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public static double calcDistHubeny(double lat1, double lng1,
                                            double lat2, double lng2,
                                            double a, double e2, double mnum)
        {
            double my = deg2rad((lat1 + lat2) / 2.0);
            double dy = deg2rad(lat1 - lat2);
            double dx = deg2rad(lng1 - lng2);

            double sin = Math.Sin(my);
            double w = Math.Sqrt(1.0 - e2 * sin * sin);
            double m = mnum / (w * w * w);
            double n = a / w;

            double dym = dy * m;
            double dxncos = dx * n * Math.Cos(my);

            return Math.Sqrt(dym * dym + dxncos * dxncos);
        }

        public static double calcDistHubeny(double lat1, double lng1,
                                            double lat2, double lng2)
        {
            return calcDistHubeny(lat1, lng1, lat2, lng2,
                                  GRS80_A, GRS80_E2, GRS80_MNUM);
        }



        public static double postionToDistanceM(GPSData prepos ,GPSData nowpos)
        {
            return calcDistHubeny(prepos.Latitude, prepos.Longitude, nowpos.Latitude, nowpos.Longitude);
        }
    }
}
