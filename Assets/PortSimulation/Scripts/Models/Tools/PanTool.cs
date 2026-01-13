using System;
using System.Collections;
using DataBindingFramework;
using PortSimulation.Managers;
using PortSimulation.Tools;
using ServiceLocatorFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PortSimulation
{
    public class PanTool : MonoBehaviour, ITool
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
            
            if (panAction != null && panAction.action != null)
            {
                panAction.action.Enable();
                panAction.action.performed += OnPan;
                panAction.action.canceled += OnPan;
            }
        }
        

        private void Start()
        {
            if (bounds != null)
                panBounds = bounds.bounds;
            // Register self with ToolManager
            var toolManager = ServiceLocator.Current.Get<ToolManager>();
            if (toolManager != null)
            {
                toolManager.RegisterTool(ToolNameConstants.PanToolName, this,true);
            }
        }
        
        
        public void UpdateTool()
        {
            if (canUpdatePan && canUsePanTool)
            {
                UpdatePanMovement();
            }
        }

        private void OnDisable()
        {
            if (canUsePanToolObserver != null)
                canUsePanToolObserver.Unbind(CanUpdatePan);
            if (observerManager != null)
                observerManager.RemoveObserver(ObserverNameConstants.canUsePanTool);
            
            if (panAction != null && panAction.action != null)
            {
                panAction.action.performed -= OnPan;
                panAction.action.canceled -= OnPan;
                panAction.action.Disable();
            }
            
            // Unregister self from ToolManager
            var toolManager = ServiceLocator.Current.Get<ToolManager>();
            if (toolManager != null)
            {
                toolManager.UnregisterTool(ToolNameConstants.PanToolName);
            }
        }
        void UpdatePanMovement()
        {
            if (camera == null) return;

            Vector3 cameraForward = camera.forward;
            cameraForward.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(cameraForward);
            Vector3 panMovement = lookRotation * new Vector3(panInput.x, 0, panInput.y) * panSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + panMovement;

            // Clamp the new position within the defined bounds
            if (bounds != null)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, panBounds.min.x, panBounds.max.x);
                newPosition.z = Mathf.Clamp(newPosition.z, panBounds.min.z, panBounds.max.z);
            }

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