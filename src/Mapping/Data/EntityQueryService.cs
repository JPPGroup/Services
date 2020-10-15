using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mapping.Data
{
    public class EntityQueryService
    {
        public async Task<string> GetQueryResponse(string query)
        {
            string inputString = Uri.EscapeUriString(query);

            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.GetAsync(
                $"https://lz4.overpass-api.de/api/interpreter?data={inputString}");

            return await resp.Content.ReadAsStringAsync();
        }

        public async Task<string> GetQueryResponse(EntityQueryOptions options, BoundingBox bounds)
        {
            StringBuilder query = new StringBuilder();
            query.Append($"[out:json];(");

            //TODO: Consider reflection
            if (options.Hospital)
            {
                query.Append($"way[amenity=hospital]({bounds});");
            }
            if (options.School)
            {
                query.Append($"way[amenity=school]({bounds});");
            }

            query.Append(");(._;>;);out;");

            return await GetQueryResponse(query.ToString());
        }
    }
}
