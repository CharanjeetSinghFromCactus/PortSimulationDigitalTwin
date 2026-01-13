using PortSimulation.Managers;
using PortSimulation.RoadSystem;
using PortSimulation.Tools;
using PortSimulation.RoadSystem.Data;
using UnityEngine;
using ServiceLocatorFramework;

namespace PortSimulation.Tools
{
    public class RoadConstructionTool : ITool
    {
        private RoadBuilderScript roadBuilder;
        private Camera mainCamera;
        private Transform cameraTarget;
        private float panSpeed = 20f;
        private BoxCollider bounds; // Optional bounds

        public RoadConstructionTool(RoadBuilderScript builder, Camera camera, Transform cameraTarget, float panSpeed = 20f, BoxCollider bounds = null)
        {
            this.roadBuilder = builder;
            this.mainCamera = camera;
            this.cameraTarget = cameraTarget;
            this.panSpeed = panSpeed;
            this.bounds = bounds;
        }

        public void UpdateTool()
        {
            // Camera Pan Movement (WASD)
            HandleCameraMovement();

            // Road Construction Logic
            if (roadBuilder != null && mainCamera != null)
            {
                // Prevent building if over UI
                if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    return;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                bool isMousePressed = Input.GetMouseButtonDown(0);
                // bool isShiftPressed = Input.GetKey(KeyCode.LeftShift); // Shift used for tangent, maybe config?
                // Shift is also commonly used for running, but let's keep it for tangents for now or removed if conflicts
                bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
                bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl);

                roadBuilder.HandleUpdate(ray, isMousePressed, isShiftPressed, isCtrlPressed);
            }
        }

        private void HandleCameraMovement()
        {
            if (mainCamera == null || cameraTarget == null) return;

            float h = Input.GetAxis("Horizontal"); // A/D
            float v = Input.GetAxis("Vertical");   // W/S

            if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
            {
                // We move relative to the camera's orientation, but we apply the position change to the target.
                Vector3 forward = mainCamera.transform.forward;
                Vector3 right = mainCamera.transform.right;

                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                Vector3 moveDir = (forward * v + right * h).normalized;
                Vector3 newPos = cameraTarget.position + moveDir * panSpeed * Time.deltaTime;

                // Optional clamping if bounds exist
                if (bounds != null)
                {
                    newPos.x = Mathf.Clamp(newPos.x, bounds.bounds.min.x, bounds.bounds.max.x);
                    newPos.z = Mathf.Clamp(newPos.z, bounds.bounds.min.z, bounds.bounds.max.z);
                }

                cameraTarget.position = newPos;
            }
        }
    }
}
