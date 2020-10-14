using System;
using System.Collections.Generic;
using System.Linq;
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
                Latitude = locationElement.GetProperty("lat").GetRawText(),
                Longitude = locationElement.GetProperty("lng").GetRawText()
            };

            return location;
        }
    }

    public struct LatLong
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
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
