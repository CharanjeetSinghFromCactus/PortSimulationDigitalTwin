using System;

namespace PortSimulation.UI
{
    [Serializable]
    public class CraneData
    {
        public string craneName;
        public CraneType craneType; 
        public string ContainerID;
        public float containerWeight;
    }
}