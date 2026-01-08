using System;
using System.Collections;
using DataBindingFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PortSimulation
{
    public class PanTool : MonoBehaviour
    {
        private Bounds panBounds;
        public BoxCollider bounds;
        public float panSpeed = 0.1f;
        public Transform camera;
        bool canUpdatePan = false;
        public InputActionReference panAction;
        private Vector2 panInput;
        [SerializeField] private bool canUsePanTool = true;
        private IObserverManager observerManager;
        DataBindingFramework.IObserver<bool> canUsePanToolObserver;
        private void OnEnable()
        {
            observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
            canUsePanToolObserver = observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.canUsePanTool);
            canUsePanToolObserver.Bind(this, CanUpdatePan, "Can Update Pan Observer");
            
            panAction.action.Enable();
            panAction.action.performed += OnPan;
            panAction.action.canceled += OnPan;
        }
        

        private void Start()
        {
            panBounds = bounds.bounds;
        }

        // private IEnumerator UpdatePan()
        // {
        //     while (canUpdatePan)
        //     {
        //         UpdatePanMovement();
        //         yield return null;
        //     }
        //     yield return null;
        // }

        private void Update()
        {
            if (canUpdatePan && canUsePanTool)
            {
                UpdatePanMovement();
            }
        }
        private void OnDisable()
        {
            canUsePanToolObserver.Unbind(CanUpdatePan);
            observerManager.RemoveObserver(ObserverNameConstants.canUsePanTool);
            panAction.action.performed -= OnPan;
            panAction.action.canceled -= OnPan;
            panAction.action.Disable();
        }
        void UpdatePanMovement()
        {
            Vector3 cameraForward = camera.forward;
            cameraForward.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(cameraForward);
            Vector3 panMovement = lookRotation * new Vector3(panInput.x, 0, panInput.y) * panSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + panMovement;

            // Clamp the new position within the defined bounds
            newPosition.x = Mathf.Clamp(newPosition.x, panBounds.min.x, panBounds.max.x);
            newPosition.z = Mathf.Clamp(newPosition.z, panBounds.min.z, panBounds.max.z);

            transform.position = newPosition;
        }
        
        
        private void CanUpdatePan(bool obj)
        {
            canUsePanTool = obj;
        }

        private void OnPan(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                panInput = context.ReadValue<Vector2>();
                // StartCoroutine(UpdatePan());
                canUpdatePan = true;
            }
            
            if (context.canceled)
            {
                panInput = Vector2.zero;
                // StopCoroutine(UpdatePan());
                canUpdatePan = false;
            }
            
        }
    }
}