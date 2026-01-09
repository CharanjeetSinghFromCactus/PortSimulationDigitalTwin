using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    public sealed class Placeable : MonoBehaviour, IPlaceable
    {
        public bool IsValidPlacement => _overlapCount == 0;

        public Transform Transform => transform;

        private int _overlapCount;
        private Collider col;
        private Rigidbody rb;
        private void Awake()
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;

            rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void BeginEdit() { }
        public void EndEdit() { }
        
        public void DestroyPlaceable()
        {
            Destroy(gameObject);   
        }

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

        public void UpdatePosition(Vector3 position)
        {
            transform.position = position;
        }
        
    }
}