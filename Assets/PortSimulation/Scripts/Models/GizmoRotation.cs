using System;
using DataBindingFramework;
using Unity.VisualScripting;
using UnityEngine;

namespace PortSimulation
{
    public class GizmoRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public float rotationSpeed = 100f;
        public float minX = -45f;
        public float maxX = 45f;
        public float minY = -90f;
        public float maxY = 90f;

        [Header("Interaction Settings")]
        [Tooltip("Only objects in these layers will respond to rotation.")]
        public LayerMask gizmoLayerMask;

        private Vector3 lastMousePosition;
        private float currentX;
        private float currentY;
        private bool isRotating = false;
        
        [SerializeField] private Transform objectToRotate;

        private IObserverManager observerManager;
        DataBindingFramework.IObserver<bool> canUsePanTool;

        private void OnEnable()
        {
            observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
            canUsePanTool = observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.canUsePanTool);
        }

        void Start()
        {
            
            Vector3 euler = transform.eulerAngles;
            currentX = euler.x;
            currentY = euler.y;
        }
        

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Raycast using the selected layer mask
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, gizmoLayerMask))
                {
                    if (hit.transform == transform)
                    {
                        isRotating = true;
                        canUsePanTool.Notify(false);
                        lastMousePosition = Input.mousePosition;
                    }
                }
            }
            
            
            if (Input.GetMouseButtonUp(0))
            {
                isRotating = false;
                canUsePanTool.Notify(true);
            }

            if (isRotating)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;

                currentX -= delta.y * rotationSpeed * Time.deltaTime;
                currentY -= delta.x * rotationSpeed * Time.deltaTime;

                currentX = Mathf.Clamp(currentX, minX, maxX);
                currentY = Mathf.Clamp(currentY, minY, maxY);

                transform.rotation = Quaternion.Euler(currentX, currentY, 0);
                if (objectToRotate != null)
                {
                    objectToRotate.rotation = Quaternion.Euler(currentX, currentY, 0);
                }

                lastMousePosition = Input.mousePosition;
            }
        }

        private void OnDisable()
        {
            observerManager.RemoveObserver(ObserverNameConstants.canUsePanTool);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw a visual cue only if the object’s layer is part of the selected mask
            if (((1 << gameObject.layer) & gizmoLayerMask) != 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 1.1f);
            }
        }
#endif
    }
}
