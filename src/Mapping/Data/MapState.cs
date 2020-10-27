using Jpp.Common;

namespace Mapping.Data
{
    public class MapState : BaseNotify
    {
        public LatLong Location
        {
            get { return _location; }
            set
            {
                SetField(ref _location, value, nameof(Location));
            }
        }
        private LatLong _location = new LatLong()
        {
            Latitude = 52.332510,
            Longitude = -0.897930
        };
    }
}
