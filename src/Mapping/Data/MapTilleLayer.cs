using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapping.Data
{
    public class MapTilleLayer : MapLayer
    {
        public bool BaseLayer { get; set; }

        public string URL { get; set; }
        public float Opacity { get; set; }
        public string Attribution { get; set; }
    }
}
