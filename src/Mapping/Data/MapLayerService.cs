using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapping.Data
{
    public class MapLayerService
    {
        List<MapLayer> layers = new List<MapLayer>();

        public MapLayerService()
        {
            layers.Add(new MapTilleLayer()
            {
                LayerName = "Open Street Maps",
                URL = "http://10.10.1.27/mapping/osm/tile/{z}/{x}/{y}.png",
                BaseLayer = true,
                Opacity = 1,
                Attribution = "OSM Street Maps Data"
            });
        }

        public IEnumerable<MapTilleLayer> GetTileLayers()
        {
            return layers.Where(t => t is MapTilleLayer).Cast<MapTilleLayer>();
        }
    }
}
