using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace PortSimulation
{
    public class DollyCartUpdater : MonoBehaviour
    {
        public CinemachineSplineCart dollyCart;
        public bool canUpdate = true;
        public float duration;

        private void Start()
        {
            StartCoroutine(UpdateDollyCardPosition());
        }

        private IEnumerator UpdateDollyCardPosition()
        {
            float timeElapsed = 0f;
            while (canUpdate)
            {
                if (timeElapsed < duration)
                {
                    dollyCart.SplinePosition = Mathf.Clamp01(timeElapsed / duration);
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    timeElapsed = 0f; // Reset the timer to loop the movement
                }

                yield return null; // Wait for the next frame
            }
        }
    }
}