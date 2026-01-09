using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    public interface IPlacementTool
    {
        void SetTarget(IPlaceable target);
        void UpdateTool(RaycastHit hit);
    }
}