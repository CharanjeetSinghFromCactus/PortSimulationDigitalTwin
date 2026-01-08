// ----------------------------------------------------
// Road Constructor
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

#if UNITY_EDITOR
using System.Collections.Generic;
using PampelGames.Shared.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PampelGames.RoadConstructor
{
    public static class SpawnObjectPresetDrawerCreation
    {
        public static VisualElement CreatePropertyGUI(SpawnObjectPreset spawnObjectPreset, SerializedProperty property)
        {
            var container = new VisualElement();

            TextField category = new("Category");
            category.BindProperty(property.FindPropertyRelative(nameof(SpawnObjectPreset.category)));
            category.tooltip = "Category to which this object set will be applied.\n\n" +
                               "Multiple categories can be set using commas, for example: Category1,Category2,Category3.";

            TextField spawnObjectName = new("Name");
            spawnObjectName.BindProperty(property.FindPropertyRelative(nameof(SpawnObjectPreset.spawnObjectName)));
            spawnObjectName.tooltip = "Name of this object set.";


            var objectClassesProperty = property.FindPropertyRelative(nameof(SpawnObjectPreset.spawnObjects));
            var objectClassesListView = CreateObjectClassesListView(spawnObjectPreset.spawnObjects, objectClassesProperty);


            container.Add(category);
            container.Add(spawnObjectName);
            container.Add(objectClassesListView);

            return container;
        }

        /********************************************************************************************************************************/

        public static ListView CreateObjectClassesListView(List<SpawnObject> spawnObjects, SerializedProperty property)
        {
            var lanePresetIndexesListView = new ListView();

            lanePresetIndexesListView.showBoundCollectionSize = false;
            lanePresetIndexesListView.itemsSource = spawnObjects;
            lanePresetIndexesListView.PGObjectListViewStyle();
            lanePresetIndexesListView.reorderable = true;
            lanePresetIndexesListView.reorderMode = ListViewReorderMode.Animated;
            lanePresetIndexesListView.headerTitle = "Objects";
            lanePresetIndexesListView.showFoldoutHeader = true;

            lanePresetIndexesListView.makeItem = MakeObjectClassItem;

            lanePresetIndexesListView.bindItem = (item, i) =>
            {
                item.Clear();

                property.serializedObject.Update();
                var objectClassItemProperty = property.GetArrayElementAtIndex(i);

                PGEditorAutoSetup.CreateAndBindClassElements<SpawnObject>(objectClassItemProperty, item);

                EnumField objectType = item.Q<EnumField>(nameof(objectType));
                VisualElement Railing = item.Q<VisualElement>(nameof(Railing));
                VisualElement Custom = item.Q<VisualElement>(nameof(Custom));

                Railing.PGDisplayStyleFlex(spawnObjects[i].objectType == SpawnObjectType.Railing);
                Custom.PGDisplayStyleFlex(spawnObjects[i].objectType is SpawnObjectType.Road or SpawnObjectType.IntersectionApproach
                    or SpawnObjectType.IntersectionExit);
                objectType.RegisterValueChangedCallback(evt =>
                {
                    Railing.PGDisplayStyleFlex(spawnObjects[i].objectType == SpawnObjectType.Railing);
                    Custom.PGDisplayStyleFlex(spawnObjects[i].objectType is SpawnObjectType.Road or SpawnObjectType.IntersectionApproach
                        or SpawnObjectType.IntersectionExit);
                });


                /********************************************************************************************************************************/
                // Railing

                var railingBoundsButton = new Button();
                railingBoundsButton.name = nameof(railingBoundsButton);
                railingBoundsButton.text = "Calculate Bounds";
                railingBoundsButton.tooltip = "Calculates the max. bounds from the object's mesh renderers.";
                railingBoundsButton.clicked += () =>
                {
                    var _obj = spawnObjects[i].obj;
                    if (!_obj) return;
                    var meshRenderers = _obj.GetComponentsInChildren<MeshRenderer>();
                    if (meshRenderers.Length == 0) return;
                    var railingBounds = new Bounds();
                    for (var i = 0; i < meshRenderers.Length; i++) railingBounds.Encapsulate(meshRenderers[i].bounds);
                    spawnObjects[i].railingSpacing = Mathf.Max(railingBounds.size.x, railingBounds.size.y, railingBounds.size.z);
                };
                Railing.Insert(0, railingBoundsButton);
            };

            return lanePresetIndexesListView;
        }

        private static VisualElement MakeObjectClassItem()
        {
            var item = new GroupBox();
            item.PGBorderWidth(1);
            item.PGBorderColor(PGColors.CustomBorder());
            item.style.paddingRight = 3f;
            item.style.marginLeft = 0f;
            item.style.marginTop = 3f;
            item.style.marginBottom = 3f;
            return item;
        }
    }
}
#endif