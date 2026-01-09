using UnityEngine;
using PortSimulation.Tools;

namespace PortSimulation.PlacementSystem
{
    public sealed class ScaleTool : IPlacementTool
    {
        private IPlaceable _target;

        private float _speed;
        private float _minScale;
        private float _maxScale;

        private Vector3 _startScale;
        private Vector2 _startMousePos;
        private bool _isScaling;

        public ScaleTool(float speed, float maxScale, float minScale)
        {
            _speed = speed;
            _maxScale = maxScale;
            _minScale = minScale;
        }

        public void SetTarget(IPlaceable target)
        {
            _target = target;
        }

        public void UpdateTool()
        {
            if (_target == null)
                return;

            // ============ START ============
            if (Input.GetMouseButtonDown(0))
            {
                _startScale = _target.Transform.localScale;
                _startMousePos = Input.mousePosition;
                _isScaling = true;
            }

            // ============ DRAG ============
            if (Input.GetMouseButton(0) && _isScaling)
            {
                Vector2 currentMousePos = Input.mousePosition;
                Vector2 dragVector = currentMousePos - _startMousePos;

                float dragDistance = dragVector.magnitude;

                // Decide scale direction (up/right = increase, down/left = decrease)
                float direction = Mathf.Sign(Vector2.Dot(dragVector, Vector2.up + Vector2.right));

                float scaleFactor = 1f + (dragDistance * direction * _speed * 0.001f);

                Vector3 newScale = _startScale * scaleFactor;
                newScale = ClampVector(newScale, _minScale, _maxScale);

                _target.Transform.localScale = newScale;
            }

            // ============ END ============
            if (Input.GetMouseButtonUp(0))
            {
                _isScaling = false;
            }
        }

        private Vector3 ClampVector(Vector3 value, float min, float max)
        {
            return new Vector3(
                Mathf.Clamp(value.x, min, max),
                Mathf.Clamp(value.y, min, max),
                Mathf.Clamp(value.z, min, max)
            );
        }
    }
}
