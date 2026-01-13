using PampelGames.RoadConstructor;
using PampelGames.Shared.Utility;
using UnityEngine;
using PortSimulation.RoadSystem.Data;
using DataBindingFramework;
using ServiceLocatorFramework;

namespace PortSimulation.RoadSystem
{
    public class RoadBuilderScript : RoadBuilderBase
    {
        private Vector3 pointerPosition;
        private Vector3 lastPointerPosition;

        private IObserver<bool> canBuildRoadObserver;

        private void Start()
        {
            var observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<DataBindingFramework.IObserverManager>();
            canBuildRoadObserver = observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.CanBuildRoad);

            if (roadConstructor != null && roadConstructor._RoadSet != null)
            {
                foreach (var road in roadConstructor._RoadSet.roads)
                {
                    Debug.Log($"Available Road: {road.roadName}, Category: {road.category}");
                }
            }
        }

        private void Awake()
        {
            InitializePointer();
            builderRoadType = BuilderRoadType.Road;

            // Fix: Register existing roads in the scene
            if (registerSceneObjects)
                roadConstructor.RegisterSceneObjects();
        }

        public void HandleUpdate(Ray ray, bool isMousePressed, bool isShiftPressed, bool isCtrlPressed)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopBuilding();
                canBuildRoadObserver?.Notify(false);
                return;
            }

            if (roadConstructor == null) return;

            // Destroy display objects from previous frames to prevent buildup
            roadConstructor.ClearAllDisplayObjects();

            if (!Physics.Raycast(ray, out var hit))
            {
                SetPointerActive(false);
                SetPointerDemolishActive(false);
                return;
            }

            // Demolish Logic
            SetPointerDemolishActive(isDemolishActive);
            if (isDemolishActive)
            {
                var radius = GetDefaultRadius();
                pointerDemolishPosition = SnapPointerDemolish(radius, hit.point, hit.normal);

                if (isMousePressed)
                {
                    roadConstructor.Demolish(pointerDemolishPosition, radius);
                }
                else
                {
                    // Visual representation
                    roadConstructor.DisplayDemolishObjects(pointerDemolishPosition, radius);
                }

                SetPointerActive(false); // Hide normal pointer when demolishing
                return; // consistent with demo logic, don't build when demolishing
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
                ActivateRoad(roadData.RoadId);
            }
        }

        public void Undo()
        {
            UndoLastConstruction();
        }

        private bool isDemolishActive = false;
        private Vector3 pointerDemolishPosition;

        public void ToggleDemolish(bool active)
        {
            isDemolishActive = active;
            if (!active)
            {
                SetPointerDemolishActive(false);
            }
        }

        public void StopBuilding()
        {
            DeactivateRoad();
            ResetValues();
            SetPointerActive(false);
            ToggleDemolish(false); // Ensure demolish is off
        }
    }
}
