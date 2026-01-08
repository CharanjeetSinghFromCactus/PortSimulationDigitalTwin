using System;
using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    [Serializable]
    public class PlacementCategory
    {
        [field:SerializeField] public string CategoryName { get; private set; }
        [field:SerializeField] public PlacementData Data { get; private set; }
        [field:SerializeField] public Sprite CategoryIcon { get; private set; }
    }
}