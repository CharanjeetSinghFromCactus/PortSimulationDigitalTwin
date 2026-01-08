using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlacementSystem
{
    public class InputManager : ITick
    {
        private Camera sceneCamera;
        private Vector3 lastPosition;
        private LayerMask placementMask;

        public Action onClick, onExit;

        public InputManager(Camera camera, LayerMask placementMask)
        {
            this.sceneCamera = camera;
            this.placementMask = placementMask;
            TickManager.Instance.Add(this);
        }

        ~InputManager()
        {
            TickManager.Instance.Remove(this);
        }
        
        public bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        public Vector3 GetSelectedMapPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;

            Ray ray = sceneCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementMask))
            {
    //            Debug.Log($"Hit Object {hit.collider.name}");
                lastPosition = hit.point;
            }

            return lastPosition;
        }

        public void Tick()
        {
            // Debug.Log("Input manager Update");
            if (Input.GetMouseButtonDown(0))
            {
                onClick?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onExit?.Invoke();
            }
        }
    }
}