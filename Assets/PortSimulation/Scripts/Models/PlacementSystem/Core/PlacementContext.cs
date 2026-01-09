namespace PortSimulation.PlacementSystem
{
    public sealed class PlacementContext
    {
        public IPlaceable CurrentPlaceable { get; private set; }

        public void SetPlaceable(IPlaceable placeable)
        {
            CurrentPlaceable?.EndEdit();
            CurrentPlaceable = placeable;
            CurrentPlaceable?.BeginEdit();
        }

        public void Clear()
        {
            CurrentPlaceable?.EndEdit();
            CurrentPlaceable = null;
        }
    }
}