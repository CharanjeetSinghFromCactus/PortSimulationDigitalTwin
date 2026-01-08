using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace PlacementSystem
{
    [CreateAssetMenu(fileName = "PlaceableContainer", menuName = "Data/PlaceableContainer")]
    public class PlaceableContainer : ScriptableObject
    {
        public List<PlaceableData> data;
    }

    [System.Serializable]
    public class PlaceableData
    {
        public string name;
        public int id;
        public Vector2Int size;
        public GameObject placeablePrefab;
    }
}