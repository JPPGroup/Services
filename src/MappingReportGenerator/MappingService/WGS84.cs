using System;
using System.Collections.Generic;
using System.Text;

namespace Jpp.MappingReportGenerator
{
    public class WGS84
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string VerboseAddress { get; set; }

        public double WebMercatorX
        {
            get
            {
                return Longitude * 20037508.34 / 180;
            }
        }

        public double WebMercatorY
        {
            get
            {
                var y = Math.Log(Math.Tan((90 + Latitude) * Math.PI / 360)) / (Math.PI / 180);
                y = y * 20037508.34 / 180;
                return y;
            }
        }


        public WGS84(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;

        }

        

        public WGS84(string lat, string lng) : this(double.Parse(lat), double.Parse(lng))
        {
        }
    }
}
