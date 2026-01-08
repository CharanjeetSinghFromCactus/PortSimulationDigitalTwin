using UnityEngine;

namespace PortSimulation
{
    public class LookAt : MonoBehaviour
    {
        public Transform lookAtTransform;
        public float lookSpeed = 5f;

        void Update()
        {
            if (lookAtTransform != null)
            {
                Vector3 direction = lookAtTransform.position - transform.position;
                if (direction.sqrMagnitude > 0.001f) // Avoid zero-length direction
                {
                    Quaternion targetRotation = Quaternion.LookRotation(-direction);
                    transform.rotation =
                        Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookSpeed);
                }
            }
        }
    }
}