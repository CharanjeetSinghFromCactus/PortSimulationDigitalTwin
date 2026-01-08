using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    public class PlacementSystem : MonoBehaviour
    {
        private InputManager _inputManager;
        [SerializeField] private GameObject mouseIndicator;
        [SerializeField] private Camera camera;
        [SerializeField] private LayerMask placementMask;

        [SerializeField] private PlaceableContainer _container;
        [SerializeField] private GameObject gridVisualization;
        [SerializeField] private Grid grid;

        [SerializeField] private AudioSource source;
        [SerializeField] private PreviewSystem previewSystem;

        private int selectedObjectIndex = -1;
        private GridData floorData, furnitureData;
        private List<GameObject> placedObjects = new();

        private Vector3Int lastDetectedPosition = Vector3Int.zero;

        private void Start()
        {
            _inputManager = new InputManager(camera, placementMask);
            placedObjects = new();
            StopPlacement();
            floorData = new();
            furnitureData = new();
        }

        private void Update()
        {
            Vector3 position = _inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(position);
            if (lastDetectedPosition == gridPosition) return;
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            mouseIndicator.transform.position = position;
            previewSystem.UpdatePlacementPreview(gridPosition, placementValidity);
            lastDetectedPosition = gridPosition;
        }


        public void StartPlacement(int id)
        {
            StopPlacement();
            Debug.Log("Start Placement");
            selectedObjectIndex = _container.data.FindIndex(x => x.id == id);
            if (selectedObjectIndex < 0)
            {
                Debug.LogError("No selected object found");
                return;
            }

            gridVisualization.SetActive(true);
            previewSystem.StartShowingPlacementPreview(_container.data[selectedObjectIndex].placeablePrefab,
                _container.data[selectedObjectIndex].size);
            _inputManager.onClick += PlaceObject;
            _inputManager.onExit += StopPlacement;
        }

        private void PlaceObject()
        {
            if (_inputManager.IsPointerOverUI())
            {
                return;
            }

            Vector3 position = _inputManager.GetSelectedMapPosition();
            Vector3Int cellPosition = grid.WorldToCell(position);
            previewSystem.UpdatePlacementPreview(cellPosition, true);
            bool placementValidity = CheckPlacementValidity(cellPosition, selectedObjectIndex);
            if (!placementValidity) return;
            source.Play();
            GameObject spawnedObject = Instantiate(_container.data[selectedObjectIndex].placeablePrefab,
                cellPosition, Quaternion.identity);
            placedObjects.Add(spawnedObject);
            GridData selectedData = _container.data[selectedObjectIndex].id == 0 ? floorData : furnitureData;
            selectedData.AddObjectAT(cellPosition, _container.data[selectedObjectIndex].size,
                _container.data[selectedObjectIndex].id, placedObjects.Count - 1);
        }

        private bool CheckPlacementValidity(Vector3Int position, int i)
        {
            if (_container.data == null && _container.data.Count <= 0)
            {
                return false;
            }

            GridData selectedData = _container.data[selectedObjectIndex].id == 0 ? floorData : furnitureData;

            return selectedData.CanPlaceAtPosition(position, _container.data[selectedObjectIndex].size);
        }

        private void StopPlacement()
        {
            selectedObjectIndex = -1;
            lastDetectedPosition = Vector3Int.zero;
            gridVisualization.SetActive(false);
            previewSystem.StopShowingPlacementPreview();
            _inputManager.onClick -= PlaceObject;
            _inputManager.onExit -= StopPlacement;
        }
    }
}