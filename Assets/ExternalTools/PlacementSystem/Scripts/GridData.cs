using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    public class GridData
    {
        public Dictionary<Vector3, PlacementData> data = new();

        public void AddObjectAT(Vector3 gridPosition, Vector2Int objectSize, int id, int placementObjectIndex)
        {
            List<Vector3> positionToOccupy = CalculatePositions(gridPosition, objectSize);
            PlacementData placementData = new PlacementData(positionToOccupy, id, placementObjectIndex);

            foreach (var pos in positionToOccupy)
            {
                if (data.ContainsKey(pos))
                {
                    throw new Exception("Position already occupied");
                }

                data.Add(pos, placementData);
            }

        }


        private List<Vector3> CalculatePositions(Vector3 gridPosition, Vector2Int objectSize)
        {
            List<Vector3> positions = new();
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    Vector3 newPosition = gridPosition + new Vector3(x, 0, y);
                    positions.Add(newPosition);
                }
            }

            return positions;
        }

        public bool CanPlaceAtPosition(Vector3 gridPosition, Vector2Int objectSize)
        {
            List<Vector3> position = CalculatePositions(gridPosition, objectSize);
            foreach (var pos in position)
            {
                if (data.ContainsKey(pos))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class PlacementData
    {
        public List<Vector3> occupiedPositions;
        public int ID { get; private set; }
        public int placedObjectIndex { get; private set; }

        public PlacementData(List<Vector3> occupiedPositions, int id, int placedObjectIndex)
        {
            this.occupiedPositions = occupiedPositions;
            ID = id;
            this.placedObjectIndex = placedObjectIndex;
        }
    }
}