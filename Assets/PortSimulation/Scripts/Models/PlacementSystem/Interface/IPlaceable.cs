namespace PortSimulation.PlacementSystem
{
    public interface IPlaceable : IPositionUpdatable
    {
        bool IsValidPlacement { get; }
        void BeginEdit();
        void EndEdit();
        void DestroyPlaceable();
        
        UnityEngine.Transform Transform { get; }
    }
}