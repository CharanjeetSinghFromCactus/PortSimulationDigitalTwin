using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace PlacementSystem
{
    [CustomEditor(typeof(PlaceableContainer))]
    public class PlaceableContainerEditor : Editor
    {
        private PlaceableContainer owner;

        public override void OnInspectorGUI()
        {
            owner = target as PlaceableContainer;
            base.OnInspectorGUI();
            if (owner)
            {
                for (int i = 0; i < owner.data.Count; i++)
                {
                    if (owner.data[i].placeablePrefab)
                    {
                        owner.data[i].name = owner.data[i].placeablePrefab.name;
                    }

                    owner.data[i].id = i;
                    Vector3 bounds = owner.data[i].placeablePrefab.GetComponentInChildren<MeshRenderer>().bounds.size;
                    int x = (int)Mathf.Ceil(bounds.x);
                    int y = (int)Mathf.Ceil(bounds.z);
                    Vector2Int flooredBounds = new Vector2Int(x, y);
                    owner.data[i].size = flooredBounds;
                }
            }
        }
    }
}