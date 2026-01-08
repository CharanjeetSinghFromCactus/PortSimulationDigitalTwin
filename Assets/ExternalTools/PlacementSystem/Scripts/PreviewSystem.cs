using UnityEngine;

namespace PlacementSystem
{
    public class PreviewSystem : MonoBehaviour
    {
        [SerializeField] private float previewYOffset = 0.06f;

        [SerializeField] private GameObject cellIndicator;
        private GameObject previewObject;

        [SerializeField] private Material previewMaterial;
        private Material previewMaterialInstance;

        private Renderer cellIndicatorRenderer;

        private void Start()
        {
            previewMaterialInstance = new Material(previewMaterial);
            previewMaterialInstance.renderQueue = 3010;
            cellIndicator.SetActive(false);
            cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        }

        public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
        {
            previewObject = Instantiate(prefab);
            PreparePreview(previewObject);
            PrepareCursor(size);
            cellIndicator.SetActive(true);
        }

        public void StopShowingPlacementPreview()
        {
            if (previewObject)
            {
                Destroy(previewObject);
            }

            cellIndicator.SetActive(false);
        }

        public void UpdatePlacementPreview(Vector3 position, bool isPlacementValid)
        {
            MovePreview(position);
            MoveCursor(position);
            ApplyFeedback(isPlacementValid);
        }

        private void ApplyFeedback(bool isPlacementValid)
        {
            Color previewColor = isPlacementValid ? Color.white : Color.red;
            cellIndicatorRenderer.material.color = previewColor;
            previewColor.a = 0.5f;
            previewMaterialInstance.color = previewColor;

        }

        private void MoveCursor(Vector3 position)
        {
            cellIndicator.transform.position = position;
        }

        private void MovePreview(Vector3 position)
        {
            previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
        }

        private void PrepareCursor(Vector2Int size)
        {
            if (size.magnitude > 0)
            {
                cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
                cellIndicator.GetComponentInChildren<Renderer>().material.mainTextureScale = size;
            }
        }

        private void PreparePreview(GameObject previewObject)
        {
            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterialInstance;
                }

                renderer.material = materials[0];
            }
        }
    }
}