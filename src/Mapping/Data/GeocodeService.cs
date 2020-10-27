using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mapping.Data
{
    public class GeocodeService
    {
        public async Task<LatLong> GetLatitudeLongitude(string searchString)
        {
            string inputString = $"{searchString}, UK".Replace(" ", "+");

            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.GetAsync(
                $"https://maps.googleapis.com/maps/api/geocode/json?address={inputString}&key=AIzaSyDnmzwQpUiskcXmeeYMziHrfsDDWaDR_DY");

            using JsonDocument response = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            JsonElement locationElement = response.RootElement.GetProperty("results")[0].GetProperty("geometry").GetProperty("location");

            LatLong location = new LatLong()
            {
                Latitude = double.Parse(locationElement.GetProperty("lat").GetRawText()),
                Longitude = double.Parse(locationElement.GetProperty("lng").GetRawText())
            };

            return location;
        }
    }

    public class LatLong
    {
        private int easting, northing;
        private double latitude, longitude;

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; } //TODO Add conversion
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }  //TODO Add conversion
        }

        public int Easting
        {
            get { return easting; }
            set
            {
                easting = value;
                convertToLatLong();
            }
        }

        public int Northing
        {
            get { return northing; }
            set
            {
                northing = value;
                convertToLatLong();
            }
        }


        //From http://micronavigation.com/latlong-gridref.html
        private void convertToLatLong()
        {
            double a = 6377563.396, b = 6356256.910;              // Airy 1830 major & minor semi-axes
            double F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
            double lat0 = 49 * Math.PI / 180, lon0 = -2 * Math.PI / 180;  // NatGrid true origin
            double N0 = -100000, E0 = 400000;                     // northing & easting of true origin, metres
            double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
            double n = (a - b) / (a + b), n2 = n * n, n3 = n * n * n;

            double lat = lat0, M = 0;
            do
            {
                lat = (Northing - N0 - M) / (a * F0) + lat;

                var Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (lat - lat0);
                var Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(lat - lat0) * Math.Cos(lat + lat0);
                var Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (lat - lat0)) * Math.Cos(2 * (lat + lat0));
                var Md = (35 / 24) * n3 * Math.Sin(3 * (lat - lat0)) * Math.Cos(3 * (lat + lat0));
                M = b * F0 * (Ma - Mb + Mc - Md);                // meridional arc

            } while (Northing - N0 - M >= 0.00001);  // ie until < 0.01mm

            double cosLat = Math.Cos(lat), sinLat = Math.Sin(lat);
            var nu = a * F0 / Math.Sqrt(1 - e2 * sinLat * sinLat);              // transverse radius of curvature
            var rho = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinLat * sinLat, 1.5);  // meridional radius of curvature
            var eta2 = nu / rho - 1;

            var tanLat = Math.Tan(lat);
            double tan2lat = tanLat * tanLat, tan4lat = tan2lat * tan2lat, tan6lat = tan4lat * tan2lat;
            var secLat = 1 / cosLat;
            double nu3 = nu * nu * nu, nu5 = nu3 * nu * nu, nu7 = nu5 * nu * nu;
            var VII = tanLat / (2 * rho * nu);
            var VIII = tanLat / (24 * rho * nu3) * (5 + 3 * tan2lat + eta2 - 9 * tan2lat * eta2);
            var IX = tanLat / (720 * rho * nu5) * (61 + 90 * tan2lat + 45 * tan4lat);
            var X = secLat / nu;
            var XI = secLat / (6 * nu3) * (nu / rho + 2 * tan2lat);
            var XII = secLat / (120 * nu5) * (5 + 28 * tan2lat + 24 * tan4lat);
            var XIIA = secLat / (5040 * nu7) * (61 + 662 * tan2lat + 1320 * tan4lat + 720 * tan6lat);

            double dE = (Easting - E0), dE2 = dE * dE, dE3 = dE2 * dE, dE4 = dE2 * dE2, dE5 = dE3 * dE2, dE6 = dE4 * dE2, dE7 = dE5 * dE2;
            lat = lat - VII * dE2 + VIII * dE4 - IX * dE6;
            var lon = lon0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;

            latitude = lat * (180d / Math.PI);
            longitude = lon * (180d / Math.PI);
        }

    }

    public class BoundingBox
    {
        public float North { get; set; }
        public float South { get; set; }
        public float East { get; set; }
        public float West { get; set; }

        public override string ToString()
        {
            return $"{South},{West},{North},{East}";
        }
    }
}
