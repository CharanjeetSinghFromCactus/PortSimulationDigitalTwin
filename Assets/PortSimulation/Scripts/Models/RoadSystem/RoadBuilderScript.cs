using PampelGames.RoadConstructor;
using PampelGames.Shared.Utility;
using UnityEngine;
using PortSimulation.RoadSystem.Data;

namespace PortSimulation.RoadSystem
{
    public class RoadBuilderScript : RoadBuilderBase
    {
        private Vector3 pointerPosition;
        private Vector3 lastPointerPosition;

        private void Awake()
        {
            InitializePointer();
            builderRoadType = BuilderRoadType.Road;
        }

        public void HandleUpdate(Ray ray, bool isMousePressed, bool isShiftPressed, bool isCtrlPressed)
        {
            if (roadConstructor == null) return;

            // Destroy display objects from previous frames to prevent buildup
            roadConstructor.ClearAllDisplayObjects();

            if (!Physics.Raycast(ray, out var hit))
            {
                SetPointerActive(false);
                return;
            }

            pointerPosition = SnapPointer(hit.point);
            SetPointerActive(true);

            // Handle optional tangent fixing
            var roadSettings = new RoadSettings();
            if (isShiftPressed)
            {
                roadSettings.setTangent01 = true;
                roadSettings.tangent01 = lastTangent01;
            }
            if (isCtrlPressed)
            {
                roadSettings.setTangent02 = true;
                roadSettings.tangent02 = lastTangent02;
            }

            ConstructionResult result = null;

            if (isMousePressed)
            {
                // Construct logic
                if (builderRoadType == BuilderRoadType.Road)
                {
                    result = ConstructRoad(pointerPosition, roadSettings);
                }
            }
            else
            {
                // Display logic
                if (builderRoadType == BuilderRoadType.Road)
                {
                    result = DisplayRoad(pointerPosition, roadSettings);
                }
            }

            // Save tangents for next segment
            if (result != null && result.isValid && result is ConstructionResultRoad roadResult)
            {
                if (!roadSettings.setTangent01) lastTangent01 = roadResult.roadData.tangent01;
                if (!roadSettings.setTangent02) lastTangent02 = roadResult.roadData.tangent02;
            }
        }

        public void SetRoadType(RoadData roadData)
        {
            ResetValues();
            if (roadData != null)
            {
                ActivateRoad(roadData.RoadName);
            }
        }

        public void StopBuilding()
        {
            DeactivateRoad();
            ResetValues();
            SetPointerActive(false);
        }
    }
}
