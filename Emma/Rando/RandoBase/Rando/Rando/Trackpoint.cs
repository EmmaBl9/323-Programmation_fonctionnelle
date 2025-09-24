using Gpx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rando
{
    public class Trackpoint
    {
        public double Latitude;
        public double Longitude;
        public double? Elevation;

        public double distance=0;

        private const double EARTH_RADIUS = 6371; // [km]
        private const double RADIAN = Math.PI / 180;

        public double GetDistanceFrom(Trackpoint other)
        {
            double thisLatitude = Latitude * RADIAN;
            double otherLatitude = other.Latitude * RADIAN;
            double deltaLongitude = Math.Abs(Longitude - other.Longitude) * RADIAN;

            double cos = Math.Cos(deltaLongitude) * Math.Cos(thisLatitude) * Math.Cos(otherLatitude) +
                Math.Sin(thisLatitude) * Math.Sin(otherLatitude);

            return EARTH_RADIUS * Math.Acos(Math.Max(Math.Min(cos, 1), -1));
        }
    }

}

