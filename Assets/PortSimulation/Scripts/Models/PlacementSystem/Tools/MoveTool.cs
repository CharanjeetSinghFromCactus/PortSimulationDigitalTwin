using UnityEngine;
using UnityEngine.EventSystems;

namespace PortSimulation.PlacementSystem
{
    public sealed class MoveTool : IPlacementTool
    {
        private IPlaceable _target;
        private Camera _camera;
        private LayerMask _mask;
        private float _maxRaycastDistance;

        private Plane _dragPlane;
        private Vector3 _dragOffset;
        private bool _isDragging;

        private float _lerpSpeed;

        public MoveTool(
            Camera camera,
            LayerMask placementAreaMask,
            float maxRaycastDistance,
            float lerpSpeed = 10f)
        {
            _camera = camera;
            _mask = placementAreaMask;
            _maxRaycastDistance = maxRaycastDistance;
            _lerpSpeed = lerpSpeed;
        }

        public void SetTarget(IPlaceable target)
        {
            _target = target;
        }

        public void UpdateTool()
        {
            if (_target == null)
                return;

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            // ===================== MOUSE DOWN =====================
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, _maxRaycastDistance, _mask))
                {
                    // Create drag plane using surface normal
                    _dragPlane = new Plane(hit.normal, hit.point);

                    // Calculate offset between object and hit point
                    _dragOffset = _target.Transform.position - hit.point;

                    _isDragging = true;
                }
            }

            // ===================== DRAG =====================
            if (Input.GetMouseButton(0) && _isDragging)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (_dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPointOnPlane = ray.GetPoint(enter);
                    Vector3 desiredPos = hitPointOnPlane + _dragOffset;

                    // Smoothly move toward the desired position using Lerp
                    float t = Mathf.Clamp01(Time.deltaTime * _lerpSpeed);
                    _target.Transform.position = Vector3.Lerp(_target.Transform.position, desiredPos, t);
                }
            }

            // ===================== RELEASE =====================
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }
        }
    }
}