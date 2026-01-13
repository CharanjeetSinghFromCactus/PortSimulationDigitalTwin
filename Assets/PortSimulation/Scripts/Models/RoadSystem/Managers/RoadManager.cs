using DataBindingFramework;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using PortSimulation.Managers;

namespace PortSimulation.RoadSystem
{
    public class RoadManager : MonoBehaviour
    {
        [SerializeField] private RoadBuilderScript roadBuilder;
        [SerializeField] private LayerMask roadLayer; // For future raycasting if needed separately
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private PortSimulation.RoadSystem.Data.RoadDataContainer roadDataContainer;

        private IObserverManager _observerManager;
        private DataBindingFramework.IObserver<string> setRoadTypeObserver;
        private DataBindingFramework.IObserver<bool> canBuildRoadObserver;
        private DataBindingFramework.IObserver<string> undoRoadObserver;
        private DataBindingFramework.IObserver<bool> demolishRoadObserver;

        private bool isBuilding = false;

        private void Start()
        {
            var tool = new Tools.RoadConstructionTool(roadBuilder, mainCamera, cameraTarget);
            var toolManager = ServiceLocatorFramework.ServiceLocator.Current.Get<Managers.ToolManager>();
            toolManager?.RegisterTool(ToolNameConstants.RoadConstructionToolName, tool);
        }

        private void Awake()
        {
            ServiceLocatorFramework.ServiceLocator.Current.Register<RoadManager>(this);
        }

        private void OnEnable()
        {
            _observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();

            setRoadTypeObserver = _observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SetRoadType);
            setRoadTypeObserver.Bind(this, OnSetRoadType);

            canBuildRoadObserver = _observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.CanBuildRoad);
            canBuildRoadObserver.Bind(this, OnCanBuildRoad);

            undoRoadObserver = _observerManager.GetOrCreateObserver<string>(ObserverNameConstants.UndoRoad);
            undoRoadObserver.Bind(this, OnUndoRoad);

            demolishRoadObserver = _observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.DemolishRoad);
            demolishRoadObserver.Bind(this, OnDemolishRoad);
        }

        private void OnDisable()
        {
            setRoadTypeObserver.Unbind(OnSetRoadType);
            canBuildRoadObserver.Unbind(OnCanBuildRoad);
            undoRoadObserver?.Unbind(OnUndoRoad);
            demolishRoadObserver?.Unbind(OnDemolishRoad);

            _observerManager.RemoveObserver(ObserverNameConstants.SetRoadType);
            _observerManager.RemoveObserver(ObserverNameConstants.CanBuildRoad);
            // We generally don't remove the observers themselves if they might be used elsewhere, 
            // but for consistency with existing code:
            _observerManager.RemoveObserver(ObserverNameConstants.UndoRoad);
            _observerManager.RemoveObserver(ObserverNameConstants.DemolishRoad);

            var toolManager = ServiceLocatorFramework.ServiceLocator.Current.Get<Managers.ToolManager>();
            toolManager?.UnregisterTool(ToolNameConstants.RoadConstructionToolName);

            ServiceLocatorFramework.ServiceLocator.Current.Unregister<RoadManager>();
        }



        private void OnUndoRoad(string _)
        {
            roadBuilder.Undo();
        }

        private void OnDemolishRoad(bool active)
        {
            roadBuilder.ToggleDemolish(active);
        }

        private void OnSetRoadType(string roadType)
        {
            var data = roadDataContainer.GetRoadData(roadType);
            if (data != null)
            {
                roadBuilder.SetRoadType(data);
            }
        }

        private void OnCanBuildRoad(bool canBuild)
        {
            isBuilding = canBuild;
            var toolManager = ServiceLocatorFramework.ServiceLocator.Current.Get<Managers.ToolManager>();

            if (isBuilding)
            {
                toolManager?.ActivateTool(ToolNameConstants.RoadConstructionToolName);
            }
            else
            {
                roadBuilder.StopBuilding();
                // Revert to PanTool or default
                toolManager?.ActivateTool(ToolNameConstants.PanToolName);
            }
        }
    }
}
