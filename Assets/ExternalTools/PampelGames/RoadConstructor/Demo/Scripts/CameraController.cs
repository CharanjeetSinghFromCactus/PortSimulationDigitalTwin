using PampelGames.Shared.Utility;
using UnityEngine;

namespace PampelGames.RoadConstructor.Demo
{
    public class CameraController : MonoBehaviour
    {
        public float speed = 10f;
        public float increaseSpeed = 1.5f;
        public float panningSpeed = 20f;

        public KeyCode forward = KeyCode.W;
        public KeyCode backwards = KeyCode.S;
        public KeyCode right = KeyCode.D;
        public KeyCode left = KeyCode.A;
        public KeyCode doubleSpeed = KeyCode.LeftShift;

        public float sensitivity = 0.002f;

        public float minAngle = 90f;
        public float maxAngle = 270f;

        private float currentSpeed;
        private bool moving;
        private Vector3 cameraPosition;

        private void Update()
        {
            var lastMoving = moving;
            var deltaPosition = Vector3.zero;

            if (moving) currentSpeed += increaseSpeed * Time.deltaTime;
            moving = false;

            Move(ref deltaPosition, forward, transform.forward, false, false);
            Move(ref deltaPosition, backwards, -transform.forward, false, false);
            Move(ref deltaPosition, right, transform.right, false, false);
            Move(ref deltaPosition, left, -transform.right, false, false);

            if (moving)
            {
                if (moving != lastMoving) currentSpeed = speed;
                var shiftMultiplier = PGHybridInput.GetKey(doubleSpeed) ? 2.0f : 1.0f;
                transform.position += deltaPosition * currentSpeed * Time.deltaTime * shiftMultiplier;
            }
            else
            {
                currentSpeed = 0f;
            }

            if (PGHybridInput.RightClick)
            {
                var eulerAngles = transform.eulerAngles;
                var axis = PGHybridInput.MouseDelta;
                eulerAngles.x += -axis.y * 359f * sensitivity;
                eulerAngles.y += axis.x * 359f * sensitivity;
                if (eulerAngles.x < minAngle || eulerAngles.x > maxAngle) transform.eulerAngles = eulerAngles;
            }

            if (PGHybridInput.MiddleClickDown) cameraPosition = gameObject.transform.position;

            if (PGHybridInput.MiddleClick)
            {
                var axis = PGHybridInput.MouseDelta;
                var newX = cameraPosition.x - axis.x * panningSpeed * sensitivity;
                var newY = cameraPosition.y + axis.y * panningSpeed * sensitivity;
                var newZ = cameraPosition.z;

                gameObject.transform.position = new Vector3(newX, newY, newZ);
                cameraPosition = gameObject.transform.position;
            }
        }

        private void Move(ref Vector3 deltaPosition, KeyCode keyCode, Vector3 directionVector, bool forceForward, bool forceBackward)
        {
            if (!PGHybridInput.GetKey(keyCode) && !forceBackward && !forceForward) return;
            deltaPosition += directionVector;
            moving = true;
        }
    }
}