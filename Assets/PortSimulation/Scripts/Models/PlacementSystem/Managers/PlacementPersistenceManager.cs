using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    public class PlacementPersistenceManager : ISavable, ILoadable
    {
        private List<IPlaceable> _placedObjects = new List<IPlaceable>();
        private PlacementDataContainer _dataContainer;
        private string _savePath;
        private Transform parent;
        public PlacementPersistenceManager(PlacementDataContainer dataContainer,Transform parent)
        {
            _dataContainer = dataContainer;
            this.parent = parent;
            _savePath = Path.Combine(Application.persistentDataPath, "PlacementData.json");
        }

        public void RegisterObject(IPlaceable placeable)
        {
            if (!_placedObjects.Contains(placeable))
            {
                _placedObjects.Add(placeable);
            }
        }

        public void UnregisterObject(IPlaceable placeable)
        {
            if (_placedObjects.Contains(placeable))
            {
                _placedObjects.Remove(placeable);
            }
        }

        public void Save()
        {
            List<PlacedObjectData> saveDataList = new List<PlacedObjectData>();

            foreach (var placeable in _placedObjects)
            {
                if (placeable == null || placeable.Transform == null) continue;

                var identifier = placeable.Transform.GetComponent<PlacementIdentifier>();
                if (identifier != null)
                {
                    PlacedObjectData data = new PlacedObjectData
                    {
                        Guid = identifier.Guid,
                        CategoryIndex = identifier.CategoryId,
                        ItemIndex = identifier.ItemId,
                        Position = placeable.Transform.position,
                        Rotation = placeable.Transform.rotation,
                        Scale = placeable.Transform.localScale
                    };
                    saveDataList.Add(data);
                }
            }

            string json = JsonUtility.ToJson(new Wrapper { Items = saveDataList }, true);
            File.WriteAllText(_savePath, json);
            Debug.Log($"Saved placement data to {_savePath}");
        }

        public void Load()
        {
            if (!File.Exists(_savePath))
            {
                Debug.LogWarning($"No save file found at {_savePath}");
                return;
            }

            string json = File.ReadAllText(_savePath);
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            List<PlacedObjectData> data = wrapper.Items;

            foreach (var obj in _placedObjects)
            {
                if (obj != null)
                {
                    obj.DestroyPlaceable();
                }
            }
            _placedObjects.Clear();

            if (data == null) return;

            foreach (var saveItem in data)
            {
                if (_dataContainer == null) continue;
                
                if (saveItem.CategoryIndex < 0 || saveItem.CategoryIndex >= _dataContainer.PlacementCategories.Length) continue;
                var category = _dataContainer.PlacementCategories[saveItem.CategoryIndex];
                
                if (saveItem.ItemIndex < 0 || saveItem.ItemIndex >= category.Data.Length) continue;
                var itemData = category.Data[saveItem.ItemIndex];

                if (itemData.ObjectToPlace != null)
                {
                    GameObject newObj = Object.Instantiate(itemData.ObjectToPlace, saveItem.Position, saveItem.Rotation,parent);
                    newObj.transform.localScale = saveItem.Scale;

                    var identifier = newObj.GetComponent<PlacementIdentifier>();
                    if (identifier == null) identifier = newObj.AddComponent<PlacementIdentifier>();
                    
                    identifier.Guid = saveItem.Guid;
                    identifier.CategoryId = saveItem.CategoryIndex;
                    identifier.ItemId = saveItem.ItemIndex;

                    var placeable = newObj.GetComponent<IPlaceable>();
                    if (placeable != null)
                    {
                        RegisterObject(placeable);
                    }
                }
            }
            Debug.Log($"Loaded placement data from {_savePath}");
        }

        [System.Serializable]
        private class Wrapper
        {
            public List<PlacedObjectData> Items;
        }
    }
}