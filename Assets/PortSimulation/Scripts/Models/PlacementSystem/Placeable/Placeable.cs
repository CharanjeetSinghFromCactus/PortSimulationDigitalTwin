using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    [RequireComponent(typeof(Collider))]
    public sealed class Placeable : MonoBehaviour, IPlaceable
    {
        public bool IsValidPlacement => _overlapCount == 0;
        public Transform Transform => transform;

        private int _overlapCount;

        private void Awake()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;

            Rigidbody rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void BeginEdit() { }
        public void EndEdit() { }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IPlaceable>() != null)
                _overlapCount++;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<IPlaceable>() != null)
                _overlapCount = Mathf.Max(0, _overlapCount - 1);
        }
    }
}