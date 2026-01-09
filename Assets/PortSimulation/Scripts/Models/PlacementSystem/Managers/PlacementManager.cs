using System;
using DataBindingFramework;
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
        
        private readonly PlacementContext _context = new PlacementContext();
        private PlacementToolHandler _toolHandler;
        
        IObserverManager _observerManager;
        private DataBindingFramework.IObserver<bool> canFinalizePlacement;
        private DataBindingFramework.IObserver<PlacementItemData> onPlacementItemClick;
        private DataBindingFramework.IObserver<IPlaceable> setPlacementTarget;
        
        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            _observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
            canFinalizePlacement = _observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.CanPlacePlaceableObject);
            canFinalizePlacement.Bind(this, OnCanFinalizePlacement);
            
            onPlacementItemClick = _observerManager.GetOrCreateObserver<PlacementItemData>(ObserverNameConstants.OnPlacementItemClick);
            onPlacementItemClick.Bind(this, OnPlacementItemClick);
            
            setPlacementTarget = _observerManager.GetOrCreateObserver<IPlaceable>(ObserverNameConstants.SetPlacementTarget);
        }

        private void Start()
        {
            
            // Initialize ToolHandler here to ensure ServiceLocator is ready
            if (_toolHandler == null)
            {
                _toolHandler = new PlacementToolHandler(this, mainCamera, placementLayer,  1000f, 250f, 0.5f, 0.1f, 20f);
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
            _observerManager.RemoveObserver(ObserverNameConstants.SetPlacementTarget);
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
                Ray ray = new Ray(mainCamera.transform.position,mainCamera.transform.forward);
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

                Spawn(itemData.ObjectToPlace, spawnPosition, spawnRotation);
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
                    _context.SetPlaceable(placeable);
                    if (previewPlacementContainer != null)
                    {
                        placeable.Transform.SetParent(previewPlacementContainer.transform);
                    }
                    setPlacementTarget.Notify(placeable);
                }
            }
        }

        // ===== PUBLIC API =====

        public void Spawn(GameObject prefab, Vector3? position = null, Quaternion? rotation = null)
        {
            Vector3 spawnPos = position ?? Vector3.zero;
            Quaternion spawnRot = rotation ?? Quaternion.identity;

            GameObject go = Instantiate(prefab, spawnPos, spawnRot);
            if (previewPlacementContainer != null)
            {
                go.transform.SetParent(previewPlacementContainer.transform);
            }
            IPlaceable placeable = go.GetComponent<IPlaceable>();
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementPropertiesScreen);
            _context.SetPlaceable(placeable);
            
            // Set the active tool target to the newly spawned object
            setPlacementTarget.Notify(placeable);
        }

        public void CancelSpawn()
        {
            Debug.Log("Cancel Placement");
            _context.CancelPlacement();
            setPlacementTarget.Notify(null);
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
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

            _context.Clear();
            setPlacementTarget.Notify(null);
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
        }
        
    }
}