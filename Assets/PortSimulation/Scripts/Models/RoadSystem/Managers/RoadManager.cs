using DataBindingFramework;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using PortSimulation.Managers;
using PortSimulation.RoadSystem.Data;

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

            LoadRoads();
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

            saveRoadObserver = _observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SaveRoad);
            saveRoadObserver.Bind(this, OnSaveRoad);
        }

        private DataBindingFramework.IObserver<string> saveRoadObserver;

        private void OnSaveRoad(string _)
        {
            SaveRoads();
        }

        private void OnDisable()
        {
            setRoadTypeObserver.Unbind(OnSetRoadType);
            canBuildRoadObserver.Unbind(OnCanBuildRoad);
            undoRoadObserver?.Unbind(OnUndoRoad);
            demolishRoadObserver?.Unbind(OnDemolishRoad);
            saveRoadObserver?.Unbind(OnSaveRoad);

            _observerManager.RemoveObserver(ObserverNameConstants.SetRoadType);
            _observerManager.RemoveObserver(ObserverNameConstants.CanBuildRoad);
            // We generally don't remove the observers themselves if they might be used elsewhere, 
            // but for consistency with existing code:
            _observerManager.RemoveObserver(ObserverNameConstants.UndoRoad);
            _observerManager.RemoveObserver(ObserverNameConstants.DemolishRoad);
            _observerManager.RemoveObserver(ObserverNameConstants.SaveRoad);

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

        [ContextMenu("Save Roads")]
        public void SaveRoads()
        {
            var saveData = new RoadSystemSaveData();
            var constructor = roadBuilder.roadConstructor;

            foreach (var road in constructor.GetRoads())
            {
                var serializedRoad = (PampelGames.RoadConstructor.SerializedRoad)road.Serialize();
                saveData.roads.Add(new SavedRoad(serializedRoad));
            }

            foreach (var intersection in constructor.GetIntersections())
            {
                if (intersection is PampelGames.RoadConstructor.RoundaboutObject roundabout)
                {
                    saveData.roundabouts.Add((PampelGames.RoadConstructor.SerializedRoundabout)roundabout.Serialize());
                }
                else
                {
                    saveData.intersections.Add((PampelGames.RoadConstructor.SerializedIntersection)intersection.Serialize());
                }
            }

            string json = JsonUtility.ToJson(saveData, true);
            var path = System.IO.Path.Combine(Application.persistentDataPath, "road_save.json");
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"Roads saved to {path}");
        }

        [ContextMenu("Load Roads")]
        public void LoadRoads()
        {
            var path = System.IO.Path.Combine(Application.persistentDataPath, "road_save.json");
            if (!System.IO.File.Exists(path))
            {
                Debug.LogWarning("No save file found.");
                return;
            }

            string json = System.IO.File.ReadAllText(path);
            var saveData = JsonUtility.FromJson<RoadSystemSaveData>(json);

            var sceneObjects = new System.Collections.Generic.List<PampelGames.RoadConstructor.SerializedSceneObject>();

            foreach (var savedRoad in saveData.roads)
            {
                sceneObjects.Add(savedRoad.ToSerializedRoad());
            }

            sceneObjects.AddRange(saveData.roundabouts);
            sceneObjects.AddRange(saveData.intersections);

            var constructor = roadBuilder.roadConstructor;
            try
            {
                constructor.SetSerializableRoadSystem(sceneObjects);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading roads: {e.Message}");
            }

            Debug.Log("Roads loaded.");
        }
    }
}
