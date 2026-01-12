namespace PortSimulation.PlacementSystem
{
    public interface IPlaceable
    {
        bool IsValidPlacement { get; }
        void BeginEdit();
        void EndEdit();
        void DestroyPlaceable();
        
        UnityEngine.Transform Transform { get; }
    }
}