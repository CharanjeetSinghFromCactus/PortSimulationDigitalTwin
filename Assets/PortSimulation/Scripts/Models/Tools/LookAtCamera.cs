using System;
using Games.CameraManager;
using UnityEngine;

namespace PortSimulation
{
    public class LookAtCamera : MonoBehaviour
    {
        public Transform cameraTransform;
        public float lookSpeed = 5f;
        
        public bool isMainCamera = true;

        private void Start()
        {
            if(isMainCamera)
                cameraTransform = ServiceLocatorFramework.ServiceLocator.Current.Get<ICameraManager>().GetCamera().transform;
        }

        void Update()
        {
            if (cameraTransform != null)
            {
                Vector3 direction = cameraTransform.position - transform.position;
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