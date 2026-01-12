using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    public sealed class Placeable : MonoBehaviour, IPlaceable
    {
        public bool IsValidPlacement => _overlapCount == 0;

        public Transform Transform => transform;

        private int _overlapCount;
        private Collider _collider;
        private Rigidbody _rigidbody;
        private PlaceableSelection _selection;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;

            _rigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            _selection = GetComponent<PlaceableSelection>();
        }

        #region Edit State

        public void BeginEdit()
        {
            _selection?.OnSelect();
        }

        public void EndEdit()
        {
            _selection?.OnDeselect();
        }

        public void DestroyPlaceable()
        {
            Destroy(gameObject);
        }

        #endregion

        #region Collision Handling (Unity Side)

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IPlaceable>() == null)
                return;

            _overlapCount++;

            // Forward collision state to visual/selection logic
            _selection?.OnCollidingWithOtherModels();

            Debug.Log($"[PlacementManager] Object {gameObject.name} is colliding with {other.gameObject.name}. Placement invalid.");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<IPlaceable>() == null)
                return;

            _overlapCount = Mathf.Max(0, _overlapCount - 1);

            // Forward collision cleared state
            _selection?.OnCollisionCleared();
        }

        #endregion
    }
}