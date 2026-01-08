using UnityEngine;

namespace PortSimulation.POI
{
    public class POIReference
    {
        public PointOfInterest CurrentPOI { get; private set; }

        public void UpdatePOI(PointOfInterest poi)
        {
            CurrentPOI = poi;
        }
    }
}