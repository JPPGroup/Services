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
                Group = "Base Layers",
                URL = "http://10.10.1.27/mapping/osm/tile/{z}/{x}/{y}.png",
                BaseLayer = true,
                Opacity = 1,
                Attribution = "OSM Street Maps Data"
            });

            layers.Add(new MapTilleLayer()
            {
                LayerName = "Open Topo Maps",
                Group = "Base Layers",
                URL = "http://{s}.tile.opentopomap.org/{z}/{x}/{y}.png",
                BaseLayer = true,
                Opacity = 1,
                Attribution = "optentopomap.org"
            });

            layers.Add(new MapTilleLayer()
            {
                LayerName = "1919-1947",
                Group = "Historic Maps",
                URL = "http://nls.tileserver.com/nls/{z}/{x}/{y}.jpg",
                BaseLayer = false,
                Opacity = 0.25f,
                Attribution = "Historical Maps Layer, 1919-1947 from the <a href=\"http://maps.nls.uk/projects/api/\">NLS Maps API</a>"
            });

            layers.Add(new MapTilleLayer()
            {
                LayerName = "Radon",
                Group = "Geotechnical",
                URL = "http://10.10.1.27/mapping/radon/tile/{z}/{x}/{y}.png",
                BaseLayer = false,
                Opacity = 0.4f,
                Attribution = "BGS Radon"
            });

            layers.Add(new MapTilleLayer()
            {
                LayerName = "Flood Zone 2",
                Group = "Environmental",
                URL = "http://10.10.1.27/mapping/fz2/tile/{z}/{x}/{y}.png",
                BaseLayer = false,
                Opacity = 0.8f,
                Attribution = "EA Flood Map for Planning - Flood Zone 2 (September 2020)"
            });

            layers.Add(new MapTilleLayer()
            {
                LayerName = "Flood Zone 3",
                Group = "Environmental",
                URL = "http://10.10.1.27/mapping/fz3/tile/{z}/{x}/{y}.png",
                BaseLayer = false,
                Opacity = 0.8f,
                Attribution = "EA Flood Map for Planning - Flood Zone 3 (September 2020)"
            });
        }

        public IEnumerable<MapTilleLayer> GetTileLayers()
        {
            return layers.Where(t => t is MapTilleLayer).Cast<MapTilleLayer>();
        }

        public IEnumerable<MapTilleLayer> GetBaseTileLayers()
        {
            return layers.Where(t => t is MapTilleLayer && ((MapTilleLayer)t).BaseLayer).Cast<MapTilleLayer>();
        }

        public IEnumerable<IGrouping<string, MapLayer>> GetGroupedLayers()
        {
            return layers.GroupBy(l => l.Group);
        }
    }
}
