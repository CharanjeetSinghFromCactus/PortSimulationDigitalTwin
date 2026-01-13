using System;
using DataBindingFramework;
using PortSimulation.Data;
using PortSimulation.Managers;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PortSimulation.PlacementSystem
{
    public sealed class PlacementManager : MonoBehaviour
    {
        public static PlacementManager Instance;

        [Header("Raycasting")]
        public Camera mainCamera;
        public LayerMask placementLayer;
        public LayerMask placeableLayer;
        [SerializeField] private PlacementDataContainer _container;
        [SerializeField] private GameObject previewPlacementContainer;
        [SerializeField] private GameObject finalPlacementContainer;
        [SerializeField] private ToolConfig toolConfig;

        private readonly PlacementContext _context = new PlacementContext();
        private PlacementToolHandler _toolHandler;
        private PlacementPersistenceManager _persistenceManager;
        IObserverManager _observerManager;
        private IPropertyManager _iPropertyManager;
        private DataBindingFramework.IObserver<bool> canFinalizePlacement;
        private DataBindingFramework.IObserver<PlacementItemData> onPlacementItemClick;
        private DataBindingFramework.IObserver<IPlaceable> setPlacementTarget;
        private DataBindingFramework.IObserver<bool> deleteObject;

        private int currentCategoryID = -1;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            _observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
            _iPropertyManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IPropertyManager>();
            canFinalizePlacement = _observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.CanPlacePlaceableObject);
            canFinalizePlacement.Bind(this, OnCanFinalizePlacement);

            onPlacementItemClick = _observerManager.GetOrCreateObserver<PlacementItemData>(ObserverNameConstants.OnPlacementItemClick);
            onPlacementItemClick.Bind(this, OnPlacementItemClick);

            setPlacementTarget = _observerManager.GetOrCreateObserver<IPlaceable>(ObserverNameConstants.SetPlacementTarget);

            deleteObject = _observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.DeleteObject);
            deleteObject.Bind(this, OnDeleteObject);
        }

        private void Start()
        {
            if (_persistenceManager == null)
            {
                _persistenceManager = new PlacementPersistenceManager(_container, finalPlacementContainer.transform);
            }

            _persistenceManager.Load();
            // Initialize ToolHandler here to ensure ServiceLocator is ready
            if (_toolHandler == null)
            {
                _toolHandler = new PlacementToolHandler(this, mainCamera, toolConfig);
            }
        }

        private void Update()
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0) && _context.CurrentPlaceable == null)
                TrySelectExisting();
        }

        private void OnDisable()
        {
            canFinalizePlacement.Unbind(OnCanFinalizePlacement);
            onPlacementItemClick.Unbind(OnPlacementItemClick);
            _observerManager.RemoveObserver(ObserverNameConstants.CanPlacePlaceableObject);
            _observerManager.RemoveObserver(ObserverNameConstants.OnPlacementItemClick);
            _observerManager.RemoveObserver(ObserverNameConstants.OnPlacementItemClick);
            _observerManager.RemoveObserver(ObserverNameConstants.SetPlacementTarget);
            _observerManager.RemoveObserver(ObserverNameConstants.DeleteObject);
            deleteObject.Unbind(OnDeleteObject);
        }

        private void OnCanFinalizePlacement(bool obj)
        {
            if (obj)
            {
                ConfirmPlacement();
            }
            else
            {
                CancelSpawn();
            }
        }

        private void OnPlacementItemClick(PlacementItemData itemData)
        {
            if (itemData != null && itemData.ObjectToPlace != null)
            {
                Vector3 spawnPosition = Vector3.zero;
                Quaternion spawnRotation = Quaternion.identity;

                // Raycast from camera center to find spawn point
                Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementLayer))
                {
                    spawnPosition = hit.point;
                }
                else
                {
                    // Fallback if raycast hits nothing (e.g. spawn in front of camera)
                    spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * 10f;
                    spawnPosition.y = 0; // Assuming ground is at y=0
                }

                currentCategoryID = _iPropertyManager.GetOrCreateProperty<int>(PropertyNameConstants.PlacementCategoryNameProperty).Value;
                Spawn(itemData, spawnPosition, spawnRotation);
            }
        }

        private void TrySelectExisting()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placeableLayer))
            {
                IPlaceable placeable = hit.collider.GetComponent<IPlaceable>();
                if (placeable != null)
                {
                    ViewController.Instance.ChangeScreen(ScreenName.PlacementPropertiesScreen);
                    _context.SetPlaceable(placeable, false);
                    if (previewPlacementContainer != null)
                    {
                        placeable.Transform.SetParent(previewPlacementContainer.transform);
                    }
                    setPlacementTarget.Notify(placeable);
                }
            }
        }

        // ===== PUBLIC API =====

        public void Spawn(PlacementItemData data, Vector3? position = null, Quaternion? rotation = null)
        {
            Vector3 spawnPos = position ?? Vector3.zero;
            Quaternion spawnRot = rotation ?? Quaternion.identity;
            GameObject go = Instantiate(data.ObjectToPlace, spawnPos, spawnRot);
            if (previewPlacementContainer != null)
            {
                go.transform.SetParent(previewPlacementContainer.transform);
            }
            IPlaceable placeable = go.GetComponent<IPlaceable>();
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementPropertiesScreen);
            PlacementIdentifier identifier = go.GetComponent<PlacementIdentifier>();
            if (identifier != null)
            {
                identifier.CategoryId = currentCategoryID;
                identifier.Guid = Guid.NewGuid().ToString();
                identifier.ItemId = data.ItemId;
            }
            _context.SetPlaceable(placeable, true);
            _persistenceManager.RegisterObject(placeable);
            // Set the active tool target to the newly spawned object
            setPlacementTarget.Notify(placeable);
        }

        public void CancelSpawn()
        {
            Debug.Log("Cancel Placement");
            ServiceLocatorFramework.ServiceLocator.Current.Get<ToolManager>().ActivateTool(ToolNameConstants.PanToolName);
            if (_context.IsNewPlacement)
            {
                _persistenceManager.UnregisterObject(_context.CurrentPlaceable);
            }
            _context.CancelPlacement();
            setPlacementTarget.Notify(null);
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
            _persistenceManager.Save();
        }

        public void ConfirmPlacement()
        {
            Debug.Log("Confirm Placement");
            ServiceLocatorFramework.ServiceLocator.Current.Get<ToolManager>().ActivateTool(ToolNameConstants.PanToolName);
            if (_context.CurrentPlaceable == null)
                return;

            if (!_context.CurrentPlaceable.IsValidPlacement)
            {
                Debug.Log("❌ Invalid placement");
                return;
            }

            if (finalPlacementContainer != null)
            {
                _context.CurrentPlaceable.Transform.SetParent(finalPlacementContainer.transform);
            }
            _persistenceManager.Save();
            _context.Clear();
            setPlacementTarget.Notify(null);
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
        }

        public void ReportCollision(IPlaceable placeable, Collider other)
        {
            Debug.Log($"[PlacementManager] Object {placeable.Transform.name} is colliding with {other.gameObject.name}. Placement invalid.");
        }

        private void OnDeleteObject(bool obj)
        {
            if (_context.CurrentPlaceable != null)
            {
                _persistenceManager.UnregisterObject(_context.CurrentPlaceable);
                _context.DeletePlacement();
                setPlacementTarget.Notify(null);
                ServiceLocatorFramework.ServiceLocator.Current.Get<ToolManager>().ActivateTool(ToolNameConstants.PanToolName);
                UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
                _persistenceManager.Save();
            }
        }

    }
}