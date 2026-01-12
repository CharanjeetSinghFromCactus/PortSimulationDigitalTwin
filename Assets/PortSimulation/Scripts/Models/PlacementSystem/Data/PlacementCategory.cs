using System;
using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    [Serializable]
    public class PlacementCategory
    {
        [field:SerializeField] public int CategoryId { get; private set; }
        [field:SerializeField] public string CategoryName { get; private set; }
        [field:SerializeField] public PlacementItemData[] Data { get; private set; }
        [field:SerializeField] public Sprite CategoryIcon { get; private set; }
    }
}