using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapping.Data
{
    public class TravelAnalysisOptions
    {
        public List<TravelEntry> Entries { get; set; }

        public TravelAnalysisOptions()
        {
            Entries = new List<TravelEntry>();
        }
    }

    public class TravelEntry
    {
        public string LayerName { get; set; }
        public TravelType Type {
            get
            {
                return _type;
            }
            set
            {
                Stale = true;
                _type = value;
            }
        }

        private TravelType _type;
        public bool Stale { get; set; }
        public bool New { get; set; }
        public int Interval {
            get
            {
                return _interval;
            }
            set
            {
                Stale = true;
                _interval = value;
            }
        }

        private int _interval;

        public int Range
        {
            get
            {
                return _range;
            }
            set
            {
                Stale = true;
                _range = value;
            }
        }

        private int _range;
        public bool Removed { get; set; }
    }

    public enum TravelType
    {
        Car, 
        Foot,
        Bike
    }
}
