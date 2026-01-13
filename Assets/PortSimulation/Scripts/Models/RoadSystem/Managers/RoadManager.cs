using DataBindingFramework;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PortSimulation.RoadSystem
{
    public class RoadManager : MonoBehaviour
    {
        [SerializeField] private RoadBuilderScript roadBuilder;
        [SerializeField] private LayerMask roadLayer; // For future raycasting if needed separately
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PortSimulation.RoadSystem.Data.RoadDataContainer roadDataContainer;

        private IObserverManager _observerManager;
        private DataBindingFramework.IObserver<string> setRoadTypeObserver;
        private DataBindingFramework.IObserver<bool> canBuildRoadObserver;

        private bool isBuilding = false;

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
        }

        private void OnDisable()
        {
            setRoadTypeObserver.Unbind(OnSetRoadType);
            canBuildRoadObserver.Unbind(OnCanBuildRoad);

            _observerManager.RemoveObserver(ObserverNameConstants.SetRoadType);
            _observerManager.RemoveObserver(ObserverNameConstants.CanBuildRoad);

            ServiceLocatorFramework.ServiceLocator.Current.Unregister<RoadManager>();
        }

        private void Update()
        {
            if (!isBuilding) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            bool isMousePressed = Input.GetMouseButtonDown(0);
            bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
            bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl);

            roadBuilder.HandleUpdate(ray, isMousePressed, isShiftPressed, isCtrlPressed);
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
            if (!isBuilding)
            {
                roadBuilder.StopBuilding();
            }
        }
    }
}
