using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    [System.Serializable]
    public class PlacementTransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public PlacementTransformData() { }

        public PlacementTransformData(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            Scale = transform.localScale;
        }

        public void ApplyTo(Transform transform)
        {
            transform.position = Position;
            transform.rotation = Rotation;
            transform.localScale = Scale;
        }
    }

    public sealed class PlacementContext
    {
        public IPlaceable CurrentPlaceable { get; private set; }
        public bool IsNewPlacement { get; private set; }

        public PlacementTransformData EditableTransform = new PlacementTransformData();
        public PlacementTransformData InitialTransform = new PlacementTransformData();

        public void ApplyPlaceableSettings()
        {
            if (CurrentPlaceable != null && CurrentPlaceable.Transform != null)
            {
                EditableTransform.ApplyTo(CurrentPlaceable.Transform);
            }
        }

        public void SetPlaceable(IPlaceable placeable, bool isNewPlacement)
        {
            CurrentPlaceable?.EndEdit();

            CurrentPlaceable = placeable;
            IsNewPlacement = isNewPlacement;

            if (CurrentPlaceable != null)
            {
                InitialTransform = new PlacementTransformData(CurrentPlaceable.Transform);
                EditableTransform = new PlacementTransformData(CurrentPlaceable.Transform);
                CurrentPlaceable.BeginEdit();
            }
        }

        public void Clear()
        {
            CurrentPlaceable?.EndEdit();
            CurrentPlaceable = null;
        }

        public void CancelPlacement()
        {
            if (CurrentPlaceable == null) return;

            CurrentPlaceable.EndEdit();

            if (IsNewPlacement)
            {
                // Logic: If it's a new placement, we destroy the object on cancel
                CurrentPlaceable.DestroyPlaceable();
            }
            else
            {
                // Logic: If it's an existing object, we revert to the initial state
                InitialTransform.ApplyTo(CurrentPlaceable.Transform);
            }

            CurrentPlaceable = null;
        }

        public void DeletePlacement()
        {
            if (CurrentPlaceable == null) return;
            CurrentPlaceable.EndEdit();
            CurrentPlaceable.DestroyPlaceable();
            CurrentPlaceable = null;
        }
    }
}