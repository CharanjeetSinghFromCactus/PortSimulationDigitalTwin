using System;

namespace PortSimulation
{
    [Serializable]
    public class VehicleData
    {
        public string vehicleName;
        public VehicleType vehicleType;
        public float speed;
        public float loadCapacity;
    }
}