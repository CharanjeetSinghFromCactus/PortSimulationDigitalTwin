namespace PortSimulation.PlacementSystem
{
    public interface IPlaceable
    {
        bool IsValidPlacement { get; }
        void BeginEdit();
        void EndEdit();
        UnityEngine.Transform Transform { get; }
    }
}