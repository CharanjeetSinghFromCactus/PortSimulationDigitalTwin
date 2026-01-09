using UnityEngine;
using PortSimulation.Tools;

namespace PortSimulation.PlacementSystem
{
    public interface IPlacementTool : ITool
    {
        void SetTarget(IPlaceable target);
    }
}