using System;
using System.Collections.Generic;

namespace PortSimulation
{
    [Serializable]
    public class ShipDataCollection
    {
        public List<ShipDetails> Ships;
    }
    
    [Serializable]
    public class ShipDetails
    {
        public string From_To;
        public string ETA;
        public string BERTH;
        public string ETS;
        public string AgentOfArrival;
        public string AgentOfDeparture;
        public VesselInfo DetailsOfVessel;
    }
    
    [Serializable]
    public class VesselInfo
    {
        public string Name;
        public string Type;
        public float Length_m;
        public float Width_m;
        public string Nationality;
        public int Gross_t;
        public int Net_t;
        public float MaxDraught_m;
        public int DeadWt_t;
    }
}