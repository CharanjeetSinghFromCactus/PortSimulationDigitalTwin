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

        private readonly PlacementContext _context = new PlacementContext();
        private IPlacementTool _activeTool;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
                TrySelectExisting();

            if (_context.CurrentPlaceable == null || _activeTool == null)
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementLayer))
            {
                _activeTool.UpdateTool(hit);
            }
        }

        private void TrySelectExisting()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placeableLayer))
            {
                IPlaceable placeable = hit.collider.GetComponentInParent<IPlaceable>();
                if (placeable != null)
                    _context.SetPlaceable(placeable);
            }
        }

        // ===== PUBLIC API =====

        public void Spawn(GameObject prefab)
        {
            GameObject go = Instantiate(prefab);
            IPlaceable placeable = go.GetComponent<IPlaceable>();
            _context.SetPlaceable(placeable);
        }

        public void ConfirmPlacement()
        {
            if (_context.CurrentPlaceable == null)
                return;

            if (!_context.CurrentPlaceable.IsValidPlacement)
            {
                Debug.Log("❌ Invalid placement");
                return;
            }

            _context.Clear();
        }

        public void SetTool(IPlacementTool tool)
        {
            _activeTool = tool;
            _activeTool?.SetTarget(_context.CurrentPlaceable);
        }
    }
}