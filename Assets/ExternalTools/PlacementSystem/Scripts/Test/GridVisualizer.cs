using UnityEngine;

namespace PlacementSystem
{
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private Grid unityGrid; // Variable to hold the Unity Grid component
        public int gridWidth, gridHeight;

        void OnDrawGizmos()
        {
            // If we haven't found the grid yet, try to find it
            if (unityGrid == null)
                return;

            // If there is still no grid, stop here
            if (unityGrid == null) return;

            // Set the drawing color to Green
            Gizmos.color = Color.green;

            // Get the size you set (10, 10, 10)
            Vector3 cellSize = unityGrid.cellSize;

            // Draw the lines
            // We draw a 10x10 grid area as an example
            for (int x = -gridWidth; x <= gridWidth; x++)
            {
                for (int z = -gridHeight; z <= gridHeight; z++)
                {
                    // Calculate the starting point of this square
                    Vector3 cellCenter = unityGrid.GetCellCenterWorld(new Vector3Int(x, 0, z));

                    // Draw a wireframe cube at that spot with the correct size
                    Gizmos.DrawWireCube(cellCenter, cellSize);
                }
            }
        }
    }
}