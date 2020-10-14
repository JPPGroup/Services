using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
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
    }
}
