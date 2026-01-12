using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    [RequireComponent(typeof(Outline))]
    public class PlaceableSelection : MonoBehaviour
    {
        private Outline _outline;
        

        private void Awake()
        {
            _outline = GetComponent<Outline>();
            if (_outline == null)
            {
                _outline = gameObject.AddComponent<Outline>();
            }

            _outline.enabled = false;
        }

        #region Selection

        public void OnSelect()
        {
            UpdateOutline(true);
        }

        public void OnDeselect()
        {
            UpdateOutline(false);
        }

        #endregion

        #region Collision State (Called Externally)

        /// <summary>
        /// Call this when this object starts colliding with another placeable model.
        /// </summary>
        public void OnCollidingWithOtherModels()
        {
            UpdateCollisionVisual(true);
        }

        /// <summary>
        /// Call this when collision with another placeable model ends.
        /// </summary>
        public void OnCollisionCleared()
        {
            UpdateCollisionVisual(false);
        }

        #endregion

        #region Visual Handling

        private void UpdateOutline(bool state)
        {
            if (_outline == null) return;

            _outline.enabled = state;
            UpdateCollisionVisual(false);
        }

        private void UpdateCollisionVisual(bool IsColliding)
        {
            if (_outline == null || !_outline.enabled) return;

            // Example: Red when colliding, Green when valid
            _outline.OutlineColor = IsColliding ? Color.red : Color.green;
        }

        #endregion
    }
}