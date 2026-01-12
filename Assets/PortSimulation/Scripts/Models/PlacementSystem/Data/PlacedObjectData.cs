using System;
using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    [Serializable]
    public class PlacedObjectData
    {
        public string Guid;
        public int CategoryIndex;
        public int ItemIndex;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }
}