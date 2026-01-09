using UnityEngine;
using PortSimulation.Tools;

namespace PortSimulation.PlacementSystem
{
    public sealed class RotateTool : IPlacementTool
    {
        private IPlaceable _target;
        private float speed = 120f;

        public RotateTool(float rotateSpeed)
        {
            this.speed = rotateSpeed;
        }
        public void SetTarget(IPlaceable target) => _target = target;

        public void UpdateTool()
        {
            if (_target == null || !Input.GetMouseButton(0)) return;

            float delta = -Input.GetAxis("Mouse X");
            _target.Transform.Rotate(Vector3.up, delta * speed * Time.deltaTime);
        }
    }
}