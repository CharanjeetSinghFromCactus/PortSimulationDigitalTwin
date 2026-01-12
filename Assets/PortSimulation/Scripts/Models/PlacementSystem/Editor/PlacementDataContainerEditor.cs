using UnityEditor;
using UnityEngine;
using PortSimulation.PlacementSystem;

namespace PortSimulation.Editor
{
    [CustomEditor(typeof(PlacementDataContainer))]
    public class PlacementDataContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PlacementDataContainer container = (PlacementDataContainer)target;

            if (GUILayout.Button("Update IDs"))
            {
                UpdateIDs(container);
            }
        }

        private void UpdateIDs(PlacementDataContainer container)
        {
            if (container.PlacementCategories == null) return;

            for (int i = 0; i < container.PlacementCategories.Length; i++)
            {
                var category = container.PlacementCategories[i];
                if (category == null) continue;

                // Update Category ID
                var categoryIdProp = category.GetType().GetProperty("CategoryId");
                var categoryIdField = category.GetType().GetField("<CategoryId>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (categoryIdField != null)
                {
                    categoryIdField.SetValue(category, i);
                }

                if (category.Data == null) continue;

                for (int j = 0; j < category.Data.Length; j++)
                {
                    var item = category.Data[j];
                    if (item == null) continue;

                    // Update Item ID
                    var itemIdField = item.GetType().GetField("<ItemId>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (itemIdField != null)
                    {
                        itemIdField.SetValue(item, j);
                    }

                    // Update Category ID in Item
                    var itemCategoryIdField = item.GetType().GetField("<CategoryId>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (itemCategoryIdField != null)
                    {
                        itemCategoryIdField.SetValue(item, i);
                    }
                }
            }

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
        }
    }
}