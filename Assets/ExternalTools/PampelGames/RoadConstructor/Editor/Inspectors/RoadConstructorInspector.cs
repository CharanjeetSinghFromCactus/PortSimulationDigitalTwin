// ----------------------------------------------------
// Road Constructor
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using PampelGames.Shared;
using PampelGames.Shared.Construction.Editor;
using PampelGames.Shared.Editor;
using PampelGames.Shared.Tools.PGInspector;
using PampelGames.Shared.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PampelGames.RoadConstructor.Editor
{
    [CustomEditor(typeof(RoadConstructor))]
    public class RoadConstructorInspector : UnityEditor.Editor
    {
        public VisualTreeAsset _visualTree;
        private VisualElement container;
        private RoadConstructor roadConstructor;

        /********************************************************************************************************************************/

        private ToolbarButton documentation;

        private ToolbarToggle componentSettingsToggle;
        private GroupBox ComponentSettingsGroup;
        private VisualElement ComponentSettings;
        private ToolbarToggle roadSetToggle;
        private GroupBox RoadSetSettingsGroup;

        // Integrations
        private ToolbarToggle integrationsToggle;
        private GroupBox IntegrationsGroup;
        private VisualElement IntegrationsParent;

        // Road Set
        private VisualElement Construction;
        private ObjectField roadSet;
        private SerializedObject roadSetSerializedObject;
        private Button createNewSet;

        // Road Visualization
        private FloatField roadPreviewLength;
        private Toggle roadPreviewSpawnObjects;
        private Toggle roadPreviewElevated;
        private Button createPreviewButton;
        private Button focusPreviewButton;
        private Button removePreviewButton;

        
        private ToolbarMenu addPartsMenu;
        private ToolbarMenu partsMenu;
        private ToolbarToggle roadsToggle;
        private VisualElement RoadsParent;
        private ToolbarToggle lanePresetsToggle;
        private VisualElement LanePresetsParent;
        private ToolbarToggle spawnObjectPresetsToggle;
        private VisualElement SpawnObjectPresetsParent;
        private ToolbarToggle trafficLanePresetsToggle;
        private VisualElement TrafficLanePresetsParent;

        private SerializedProperty componentSettingsProperty;

        /********************************************************************************************************************************/
        protected void OnEnable()
        {
            container = new VisualElement();
            _visualTree.CloneTree(container);
            roadConstructor = target as RoadConstructor;

            GetDefaultReferences();
            FindElements(container);
            BindElements();
            VisualizeElements();
        }

        /********************************************************************************************************************************/

        private void GetDefaultReferences()
        {
            if (roadConstructor._DefaultReferences == null)
            {
                roadConstructor._DefaultReferences = PGAssetUtility.LoadAsset<SO_DefaultReferences>(Constants.DefaultReferences);
                EditorUtility.SetDirty(roadConstructor);
            }
        }

        private void FindElements(VisualElement root)
        {
            documentation = root.Q<ToolbarButton>(nameof(documentation));

            componentSettingsToggle = root.Q<ToolbarToggle>(nameof(componentSettingsToggle));
            ComponentSettingsGroup = root.Q<GroupBox>(nameof(ComponentSettingsGroup));
            ComponentSettings = root.Q<VisualElement>(nameof(ComponentSettings));
            roadSetToggle = root.Q<ToolbarToggle>(nameof(roadSetToggle));
            RoadSetSettingsGroup = root.Q<GroupBox>(nameof(RoadSetSettingsGroup));
            createNewSet = root.Q<Button>(nameof(createNewSet));

            roadPreviewLength = root.Q<FloatField>(nameof(roadPreviewLength));
            roadPreviewSpawnObjects = root.Q<Toggle>(nameof(roadPreviewSpawnObjects));
            roadPreviewElevated = root.Q<Toggle>(nameof(roadPreviewElevated));
            createPreviewButton = root.Q<Button>(nameof(createPreviewButton));
            focusPreviewButton = root.Q<Button>(nameof(focusPreviewButton));
            removePreviewButton = root.Q<Button>(nameof(removePreviewButton));

            roadSet = root.Q<ObjectField>(nameof(roadSet));
            Construction = root.Q<VisualElement>(nameof(Construction));

            partsMenu = root.Q<ToolbarMenu>(nameof(partsMenu));
            RoadsParent = root.Q<VisualElement>(nameof(RoadsParent));
            roadsToggle = root.Q<ToolbarToggle>(nameof(roadsToggle));
            addPartsMenu = root.Q<ToolbarMenu>(nameof(addPartsMenu));
            lanePresetsToggle = root.Q<ToolbarToggle>(nameof(lanePresetsToggle));
            LanePresetsParent = root.Q<VisualElement>(nameof(LanePresetsParent));
            spawnObjectPresetsToggle = root.Q<ToolbarToggle>(nameof(spawnObjectPresetsToggle));
            SpawnObjectPresetsParent = root.Q<VisualElement>(nameof(SpawnObjectPresetsParent));
            trafficLanePresetsToggle = root.Q<ToolbarToggle>(nameof(trafficLanePresetsToggle));
            TrafficLanePresetsParent = root.Q<VisualElement>(nameof(TrafficLanePresetsParent));
        }

        private void BindElements()
        {
            roadPreviewLength.PGSetupBindProperty(serializedObject, nameof(roadPreviewLength));
            roadPreviewSpawnObjects.PGSetupBindProperty(serializedObject, nameof(roadPreviewSpawnObjects));
            roadPreviewElevated.PGSetupBindProperty(serializedObject, nameof(roadPreviewElevated));

            componentSettingsProperty = serializedObject.FindProperty(nameof(RoadConstructor.componentSettings));
        }

        private void VisualizeElements()
        {
            documentation.tooltip = "Open the documentation page.";
            componentSettingsToggle.tooltip = "Component Settings";
            roadSetToggle.tooltip = "Road Set";
            roadSet.tooltip = "Road sets contain the road data for construction.";
            createNewSet.tooltip = "Create a new road set.";

            roadPreviewLength.tooltip = "Length of a preview road.";
            roadPreviewLength.PGClampValue();
            roadPreviewElevated.tooltip = "Creates the preview roads as elevated.";
            createPreviewButton.tooltip = "For each expanded road in the Road Setup tab, this will create a corresponding road in the scene.\n\n" +
                                          "Objects are parented to the '" + Constants.RoadPreviewParent + "' GameObject.";
            focusPreviewButton.tooltip = "Focuses the scene camera on the '" + Constants.RoadPreviewParent + "' GameObject.";
            removePreviewButton.tooltip = "Removes the '" + Constants.RoadPreviewParent + "' GameObject from the scene.";

            roadSet.objectType = typeof(RoadSet);

            

            addPartsMenu.PGRemoveMenuArrow(true, false);
            addPartsMenu.tooltip = "Add a new part.";
            partsMenu.tooltip = "Show additional options.";
            roadsToggle.tooltip = "Road Setup\n" +
                                  "(click to expand/collapse)\n" + "\n" +
                                  "These are the roads that are built into the scene.";
            lanePresetsToggle.tooltip = "Construction Lane Presets\n" +
                                        "(click to expand/collapse)\n" + "\n" +
                                        "One preset can be added to multiple roads.\n\n" +
                                        "Construction lanes are used for the construction of the roads.";
            spawnObjectPresetsToggle.tooltip = "Spawn Object Presets\n" +
                                               "(click to expand/collapse)\n" + "\n" +
                                               "One preset can be added to multiple roads.\n\n" +
                                               "These are additional objects which can be spawned onto the roads.";
            trafficLanePresetsToggle.tooltip = "Traffic Lane Presets\n" +
                                               "(click to expand/collapse)\n" + "\n" +
                                               "One preset can be added to multiple roads.\n\n" +
                                               "Traffic lanes operate independently from construction and can be optionally utilized for custom traffic systems. " +
                                               "Each lane is associated with a Unity Spline.\n\n" +
                                               "Remember to toggle the 'Add Traffic Component' setting if you want to add them automatically to new roads.";
        }


        /********************************************************************************************************************************/
        /********************************************************************************************************************************/

        public override VisualElement CreateInspectorGUI()
        {
            DrawIntegrations();
            DrawTopToolbar();
            DrawRoadPreview();
            DrawComponentSettins();
            DrawColliderWarning();
            DrawTerrain();
            DrawRoadSet();

            return container;
        }

        /********************************************************************************************************************************/

        private void DrawIntegrations()
        {
            integrationsToggle = container.Q<ToolbarToggle>(nameof(integrationsToggle));
            IntegrationsGroup = container.Q<GroupBox>(nameof(IntegrationsGroup));
            IntegrationsParent = IntegrationsGroup.Q<VisualElement>(nameof(IntegrationsParent));

            integrationsToggle.tooltip = "Integrations";

            integrationsToggle.RegisterValueChangedCallback(evt =>
            {
                roadConstructor._editorDisplay = EditorDisplay.Integrations;
                EditorDisplayVisibility();
            });

            ConstructionEditorUtility.CreateIntegrationsEditor(serializedObject, IntegrationsParent, roadConstructor.integrations,
                componentSettingsProperty);
        }

        /********************************************************************************************************************************/

        private void DrawTopToolbar()
        {
            documentation.clicked += () => { Application.OpenURL(Constants.DocumentationURL); };

            EditorDisplayVisibility();

            componentSettingsToggle.RegisterValueChangedCallback(evt =>
            {
                roadConstructor._editorDisplay = EditorDisplay.ComponentSettings;
                EditorDisplayVisibility();
            });

            roadSetToggle.RegisterValueChangedCallback(evt =>
            {
                roadConstructor._editorDisplay = EditorDisplay.RoadSet;
                EditorDisplayVisibility();
                AddTrafficComponentHelpBox();
            });

            createNewSet.clicked += () =>
            {
                var newFile = CreateInstance<RoadSet>();

                var defaultDirectory = "Assets/";

                var defaultFileName = "New Road Set";
                var extension = "asset";

                var filePath = EditorUtility.SaveFilePanel("Create Road Set", defaultDirectory, defaultFileName, extension);
                if (string.IsNullOrEmpty(filePath)) return;

                var assetPath = "Assets" + filePath.Substring(Application.dataPath.Length);

                AssetDatabase.CreateAsset(newFile, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                roadConstructor._RoadSet = newFile;
                EditorUtility.SetDirty(roadConstructor);
            };
        }

        private void EditorDisplayVisibility()
        {
            if (roadConstructor._editorDisplay == EditorDisplay.ComponentSettings)
            {
                componentSettingsToggle.SetValueWithoutNotify(true);
                roadSetToggle.SetValueWithoutNotify(false);
                integrationsToggle.SetValueWithoutNotify(false);
            }

            if (roadConstructor._editorDisplay == EditorDisplay.RoadSet)
            {
                roadSetToggle.SetValueWithoutNotify(true);
                componentSettingsToggle.SetValueWithoutNotify(false);
                integrationsToggle.SetValueWithoutNotify(false);
            }

            if (roadConstructor._editorDisplay == EditorDisplay.Integrations)
            {
                roadSetToggle.SetValueWithoutNotify(false);
                componentSettingsToggle.SetValueWithoutNotify(false);
                integrationsToggle.SetValueWithoutNotify(true);
            }

            ComponentSettingsGroup.PGDisplayStyleFlex(roadConstructor._editorDisplay == EditorDisplay.ComponentSettings);
            RoadSetSettingsGroup.PGDisplayStyleFlex(roadConstructor._editorDisplay == EditorDisplay.RoadSet);
            IntegrationsGroup.PGDisplayStyleFlex(roadConstructor._editorDisplay == EditorDisplay.Integrations);
        }

        /********************************************************************************************************************************/

        private void DrawRoadPreview()
        {
            createPreviewButton.clicked += () =>
            {
                var position = Vector3.zero;
                var previewParent = FindFirstObjectByType<PreviewRoadParent>();
                if (previewParent != null)
                {
                    position = previewParent.transform.position;
                    DestroyImmediate(previewParent.gameObject);
                }

                var previewParentObj = new GameObject(Constants.RoadPreviewParent);
                previewParent = previewParentObj.AddComponent<PreviewRoadParent>();
                roadConstructor.ConstructPreviewRoads(previewParent.transform, roadConstructor.roadPreviewLength,
                    roadConstructor.roadPreviewSpawnObjects, roadConstructor.roadPreviewElevated);
                previewParentObj.transform.position = position;
            };
            focusPreviewButton.clicked += () =>
            {
                var previewParent = FindFirstObjectByType<PreviewRoadParent>();
                if (previewParent == null) return;
                Selection.activeGameObject = previewParent.gameObject;
                SceneView.lastActiveSceneView.FrameSelected();
                Selection.activeGameObject = roadConstructor.gameObject;
            };
            removePreviewButton.clicked += () =>
            {
                var previewParent = FindFirstObjectByType<PreviewRoadParent>();
                if (previewParent != null) DestroyImmediate(previewParent.gameObject);
            };
        }
        
        /********************************************************************************************************************************/

        private void DrawComponentSettins()
        {
            PGEditorAutoSetup.CreateAndBindClassElements<ComponentSettings>(componentSettingsProperty, ComponentSettings);
            
            
            // Layer Warning Box
            LayerField addColliderLayer = ComponentSettings.Q<LayerField>(nameof(addColliderLayer));
            LayerMaskField groundLayers = ComponentSettings.Q<LayerMaskField>(nameof(groundLayers));
            
            AddLayerWarningBox(addColliderLayer);
            AddLayerWarningBox(groundLayers);

            void AddLayerWarningBox(VisualElement element)
            {
                var layerWarningBox = new HelpBox(
                    "The road layer should not be included in the ground layers to avoid interference with object spawning and ground detection.",
                    HelpBoxMessageType.Warning);
                layerWarningBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                element.Add(layerWarningBox);
            }
        }

        /********************************************************************************************************************************/

        private void DrawColliderWarning()
        {
            EnumField addCollider = ComponentSettings.Q<EnumField>(nameof(addCollider));
            LayerField addColliderLayer = ComponentSettings.Q<LayerField>(nameof(addColliderLayer));
            LayerMaskField groundLayers = ComponentSettings.Q<LayerMaskField>(nameof(groundLayers));
            
            ColliderWarningDisplay();
            addCollider.RegisterValueChangedCallback(evt => { ColliderWarningDisplay(); });
            addColliderLayer.RegisterValueChangedCallback(evt => { ColliderWarningDisplay(); });
            groundLayers.RegisterValueChangedCallback(evt => { ColliderWarningDisplay(); });
        }

        private void ColliderWarningDisplay()
        {
            var displayBox = false;
            var settings = roadConstructor.componentSettings;

            if (settings.addCollider != AddCollider.None)
            {
                var singleLayerMask = 1 << settings.addColliderLayer;
                if ((settings.groundLayers.value & singleLayerMask) != 0) displayBox = true;
            }

            LayerField addColliderLayer = ComponentSettings.Q<LayerField>(nameof(addColliderLayer));
            LayerMaskField groundLayers = ComponentSettings.Q<LayerMaskField>(nameof(groundLayers));

            SetBoxDisplay(addColliderLayer);
            SetBoxDisplay(groundLayers);

            void SetBoxDisplay(VisualElement element)
            {
                var layerWarningBox = element.Q<HelpBox>();
                layerWarningBox.PGDisplayStyleFlex(displayBox);
            }
        }
        

        /********************************************************************************************************************************/

        private void DrawTerrain()
        {
            var settings = roadConstructor.componentSettings;

            IntegerField slopeTextureIndex = ComponentSettings.Q<IntegerField>(nameof(slopeTextureIndex));
            Slider slopeTextureStrength = ComponentSettings.Q<Slider>(nameof(slopeTextureStrength));
            SliderInt slopeSmooth = ComponentSettings.Q<SliderInt>(nameof(slopeSmooth));
            TextField slopeTextureName = ComponentSettings.Q<TextField>(nameof(slopeTextureName));
            slopeTextureName.isReadOnly = true;

            SetTextureName();
            slopeTextureIndex.RegisterValueChangedCallback(evt => SetTextureName());

            void SetTextureName()
            {
                slopeTextureStrength.PGDisplayStyleFlex(settings.slopeTextureIndex >= 0);
                slopeSmooth.PGDisplayStyleFlex(settings.slopeTextureIndex >= 0);

                if (settings.terrains.Count == 0 || settings.terrains[0] == null || settings.terrains[0].terrainData == null ||
                    slopeTextureIndex.value < 0
                    || slopeTextureIndex.value >= settings.terrains[0].terrainData.terrainLayers.Length)
                {
                    slopeTextureName.value = "n.a.";
                    return;
                }

                slopeTextureName.value = settings.terrains[0].terrainData.terrainLayers[slopeTextureIndex.value]?.name;
            }
        }

        /********************************************************************************************************************************/
        private void DrawRoadSet()
        {
            roadSet.RegisterValueChangedCallback(evt =>
            {
                if (roadConstructor._RoadSet == null)
                {
                    Construction.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                }
                else
                {
                    Construction.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    DrawRoadSetInternal();
                }
            });
        }

        private void DrawRoadSetInternal()
        {
            roadSetSerializedObject = new SerializedObject(roadConstructor._RoadSet);

            DrawPartsToolbar();
            DrawRoads();
            DrawLanePresets();
            DrawSpawnObjectPresets();
            DrawTrafficLanePresets();
        }

        /********************************************************************************************************************************/

        private void DrawPartsToolbar()
        {
            PartsToolbarVisibility();
            DrawAddRemoveParts();


            roadsToggle.RegisterValueChangedCallback(evt =>
            {
                if (roadConstructor._editorActivePartType != PartType.Roads)
                {
                    roadConstructor._editorActivePartType = PartType.Roads;
                    EditorUtility.SetDirty(roadConstructor);
                    roadsToggle.SetValueWithoutNotify(false);
                }
                else
                {
                    for (var i = 0; i < roadConstructor._RoadSet.roads.Count; i++)
                    {
                        var road = roadConstructor._RoadSet.roads[i];
                        road._editorVisible = roadsToggle.value;
                    }
                }

                EditorUtility.SetDirty(roadConstructor._RoadSet);

                DrawRoads();
                PartsToolbarVisibility();
                DrawAddRemoveParts();
            });

            lanePresetsToggle.RegisterValueChangedCallback(evt =>
            {
                if (roadConstructor._editorActivePartType != PartType.LanePreset)
                {
                    roadConstructor._editorActivePartType = PartType.LanePreset;
                    EditorUtility.SetDirty(roadConstructor);
                    lanePresetsToggle.SetValueWithoutNotify(false);
                }
                else
                {
                    for (var i = 0; i < roadConstructor._RoadSet.lanePresets.Count; i++)
                    {
                        var preset = roadConstructor._RoadSet.lanePresets[i];
                        preset._editorVisible = lanePresetsToggle.value;
                    }
                }

                EditorUtility.SetDirty(roadConstructor._RoadSet);

                DrawLanePresets();
                PartsToolbarVisibility();
                DrawAddRemoveParts();
            });

            spawnObjectPresetsToggle.RegisterValueChangedCallback(evt =>
            {
                if (roadConstructor._editorActivePartType != PartType.SpawnObjectPreset)
                {
                    roadConstructor._editorActivePartType = PartType.SpawnObjectPreset;
                    EditorUtility.SetDirty(roadConstructor);
                    spawnObjectPresetsToggle.SetValueWithoutNotify(false);
                }
                else
                {
                    for (var i = 0; i < roadConstructor._RoadSet.spawnObjectPresets.Count; i++)
                    {
                        var preset = roadConstructor._RoadSet.spawnObjectPresets[i];
                        preset._editorVisible = spawnObjectPresetsToggle.value;
                    }
                }

                EditorUtility.SetDirty(roadConstructor._RoadSet);

                DrawSpawnObjectPresets();
                PartsToolbarVisibility();
                DrawAddRemoveParts();
            });

            trafficLanePresetsToggle.RegisterValueChangedCallback(evt =>
            {
                if (roadConstructor._editorActivePartType != PartType.TrafficLanePreset)
                {
                    roadConstructor._editorActivePartType = PartType.TrafficLanePreset;
                    EditorUtility.SetDirty(roadConstructor);
                    trafficLanePresetsToggle.SetValueWithoutNotify(false);
                }
                else
                {
                    for (var i = 0; i < roadConstructor._RoadSet.trafficLanePresets.Count; i++)
                    {
                        var preset = roadConstructor._RoadSet.trafficLanePresets[i];
                        preset._editorVisible = trafficLanePresetsToggle.value;
                    }
                }

                EditorUtility.SetDirty(roadConstructor._RoadSet);

                DrawTrafficLanePresets();
                PartsToolbarVisibility();
                DrawAddRemoveParts();
            });
        }

        private void PartsToolbarVisibility()
        {
            if (roadConstructor._editorActivePartType != PartType.Roads) roadsToggle.SetValueWithoutNotify(false);
            if (roadConstructor._editorActivePartType != PartType.LanePreset) lanePresetsToggle.SetValueWithoutNotify(false);
            if (roadConstructor._editorActivePartType != PartType.SpawnObjectPreset) spawnObjectPresetsToggle.SetValueWithoutNotify(false);
            if (roadConstructor._editorActivePartType != PartType.TrafficLanePreset) trafficLanePresetsToggle.SetValueWithoutNotify(false);

            RoadsParent.PGDisplayStyleFlex(roadConstructor._editorActivePartType == PartType.Roads);
            LanePresetsParent.PGDisplayStyleFlex(roadConstructor._editorActivePartType == PartType.LanePreset);
            SpawnObjectPresetsParent.PGDisplayStyleFlex(roadConstructor._editorActivePartType == PartType.SpawnObjectPreset);
            TrafficLanePresetsParent.PGDisplayStyleFlex(roadConstructor._editorActivePartType == PartType.TrafficLanePreset);

            SetModuleStyle(roadsToggle, roadConstructor._editorActivePartType == PartType.Roads);
            SetModuleStyle(lanePresetsToggle, roadConstructor._editorActivePartType == PartType.LanePreset);
            SetModuleStyle(spawnObjectPresetsToggle, roadConstructor._editorActivePartType == PartType.SpawnObjectPreset);
            SetModuleStyle(trafficLanePresetsToggle, roadConstructor._editorActivePartType == PartType.TrafficLanePreset);

            void SetModuleStyle(ToolbarToggle toggle, bool active)
            {
                toggle.style.backgroundColor = !active ? PGColors.ToolbarButtonBackground() : PGColors.ButtonBackground();
            }
        }

        private void DrawAddRemoveParts()
        {
            addPartsMenu.menu.MenuItems().Clear();
            partsMenu.menu.MenuItems().Clear();

            if (roadConstructor._editorActivePartType == PartType.Roads)
            {
                addPartsMenu.menu.AppendAction("Add Road", _ =>
                {
                    roadConstructor._RoadSet.roads.Add(new Road());
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawRoads();
                });

                partsMenu.menu.AppendAction("Remove All Roads", _ =>
                {
                    if (EditorUtility.DisplayDialog("Remove All Roads", "Are you sure you want to remove all roads from the Road Set: "
                                                                        + roadConstructor._RoadSet.name + "?", "Ok", "Cancel"))
                    {
                        roadConstructor._RoadSet.roads.Clear();
                        EditorUtility.SetDirty(roadConstructor._RoadSet);
                        DrawRoads();
                    }
                });
            }

            else if (roadConstructor._editorActivePartType == PartType.LanePreset)
            {
                addPartsMenu.menu.AppendAction("Add Lane Preset", _ =>
                {
                    roadConstructor._RoadSet.lanePresets.Add(new LanePreset());
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawLanePresets();
                });

                partsMenu.menu.AppendAction("Remove All Lane Presets", _ =>
                {
                    if (EditorUtility.DisplayDialog("Remove All Lane Presets", "Are you sure you want to remove all lane presets from the Road Set: "
                                                                               + roadConstructor._RoadSet.name + "?", "Ok", "Cancel"))
                    {
                        roadConstructor._RoadSet.lanePresets.Clear();
                        EditorUtility.SetDirty(roadConstructor._RoadSet);
                        DrawLanePresets();
                    }
                });
            }

            else if (roadConstructor._editorActivePartType == PartType.SpawnObjectPreset)
            {
                addPartsMenu.menu.AppendAction("Add Object Preset", _ =>
                {
                    roadConstructor._RoadSet.spawnObjectPresets.Add(new SpawnObjectPreset());
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawSpawnObjectPresets();
                });

                partsMenu.menu.AppendAction("Remove All Object Preset", _ =>
                {
                    if (EditorUtility.DisplayDialog("Remove All Object Preset",
                            "Are you sure you want to remove all object presets from the Road Set: "
                            + roadConstructor._RoadSet.name + "?", "Ok", "Cancel"))
                    {
                        roadConstructor._RoadSet.spawnObjectPresets.Clear();
                        EditorUtility.SetDirty(roadConstructor._RoadSet);
                        DrawSpawnObjectPresets();
                    }
                });
            }
            else if (roadConstructor._editorActivePartType == PartType.TrafficLanePreset)
            {
                addPartsMenu.menu.AppendAction("Add Traffic Lane Preset", _ =>
                {
                    roadConstructor._RoadSet.trafficLanePresets.Add(new TrafficLanePreset());
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawTrafficLanePresets();
                });

                partsMenu.menu.AppendAction("Remove All Traffic Lane Presets", _ =>
                {
                    if (EditorUtility.DisplayDialog("Remove All Traffic Lane Presets",
                            "Are you sure you want to remove all traffic lane presets from the Road Set: " + roadConstructor._RoadSet.name + "?",
                            "Ok", "Cancel"))
                    {
                        roadConstructor._RoadSet.trafficLanePresets.Clear();
                        EditorUtility.SetDirty(roadConstructor._RoadSet);
                        DrawTrafficLanePresets();
                    }
                });
            }
        }

        /********************************************************************************************************************************/


        private void DrawRoads()
        {
            RoadsParent.Clear();
            if (roadConstructor._RoadSet == null) return;
            if (roadConstructor._editorActivePartType != PartType.Roads) return;

            var areAllVisible = roadConstructor._RoadSet.roads.All(road => road._editorVisible);
            if (areAllVisible) roadsToggle.value = true;

            var roadsProperty = roadSetSerializedObject.FindProperty(nameof(RoadSet.roads));
            roadsProperty.serializedObject.Update();

            for (var i = 0; i < roadConstructor._RoadSet.roads.Count; i++)
            {
                var road = roadConstructor._RoadSet.roads[i];
                var itemParent = PGModuleEditorUtility.CreateItemParentWithToggle(road, i);
                var toolbar = itemParent.Q<Toolbar>(PGModuleEditorUtility.Toolbar + i);
                var itemToggle = itemParent.Q<ToolbarToggle>(PGModuleEditorUtility.ItemToggle + i);
                var itemMenu = itemParent.Q<ToolbarMenu>(PGModuleEditorUtility.ItemMenu + i);
                var itemPropertyParent = itemParent.Q<GroupBox>(PGModuleEditorUtility.ItemPropertyParent + i);
                toolbar.style.height = 24f;

                var allLanes = road.GetAllLanes(roadConstructor._RoadSet.lanePresets);
                itemToggle.text += road.GetRoadDisplayText(allLanes);

                itemToggle.value = road._editorVisible;
                itemPropertyParent.PGDisplayStyleFlex(road._editorVisible);
                itemToggle.RegisterValueChangedCallback(evt =>
                {
                    road._editorVisible = !road._editorVisible;
                    itemPropertyParent.PGDisplayStyleFlex(road._editorVisible);
                    DrawRoads();
                });

                var i1 = i;

                itemMenu.PGAppendMoveItems(roadConstructor._RoadSet.roads, i1, () =>
                {
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawRoads();
                });
                itemMenu.menu.AppendAction("Duplicate", action =>
                {
                    var roadOld = roadConstructor._RoadSet.roads[i1];
                    var duplicate = PGClassUtility.CopyClass(roadOld) as Road;

                    duplicate!.roadName = roadOld.roadName + " (copy)";

                    duplicate.splineEdgesEditor = new List<SplineEdgeEditor>();
                    for (var j = 0; j < roadOld.splineEdgesEditor.Count; j++)
                    {
                        var splineEdgeCopy = PGClassUtility.CopyClass(roadOld.splineEdgesEditor[j]);
                        duplicate.splineEdgesEditor.Add(splineEdgeCopy as SplineEdgeEditor);
                    }

                    duplicate.spawnObjects = new List<SpawnObject>();
                    for (var j = 0; j < roadOld.spawnObjects.Count; j++)
                    {
                        var spawnObjectCopy = PGClassUtility.CopyClass(roadOld.spawnObjects[j]);
                        duplicate.spawnObjects.Add(spawnObjectCopy as SpawnObject);
                    }

                    duplicate.trafficLanes = new List<TrafficLaneEditor>();
                    for (var j = 0; j < roadOld.trafficLanes.Count; j++)
                    {
                        var spawnObjectCopy = PGClassUtility.CopyClass(roadOld.trafficLanes[j]);
                        duplicate.trafficLanes.Add(spawnObjectCopy as TrafficLaneEditor);
                    }

                    roadConstructor._RoadSet.roads.Insert(i1 + 1, duplicate);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawRoads();
                });
                itemMenu.menu.AppendAction("Remove", action =>
                {
                    roadConstructor._RoadSet.roads.RemoveAt(i1);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawRoads();
                });

                /********************************************************************************************************************************/
                var itemProperty = roadsProperty.GetArrayElementAtIndex(i);
                var roadItem = road.CreatePropertyGUI(road, itemProperty);
                /********************************************************************************************************************************/

                itemPropertyParent.Add(roadItem);

                RoadsParent.Add(itemParent);
            }
        }

        /********************************************************************************************************************************/

        private void DrawLanePresets()
        {
            LanePresetsParent.Clear();
            if (roadConstructor._RoadSet == null) return;
            if (roadConstructor._editorActivePartType != PartType.LanePreset) return;

            var areAllVisible = roadConstructor._RoadSet.lanePresets.All(template => template._editorVisible);
            if (areAllVisible) lanePresetsToggle.value = true;

            var templatesProperty = roadSetSerializedObject.FindProperty(nameof(RoadSet.lanePresets));
            templatesProperty.serializedObject.Update();

            for (var i = 0; i < roadConstructor._RoadSet.lanePresets.Count; i++)
            {
                var template = roadConstructor._RoadSet.lanePresets[i];
                var itemParent = PGModuleEditorUtility.CreateItemParentWithToggle(template, i);
                var toolbar = itemParent.Q<Toolbar>(PGModuleEditorUtility.Toolbar + i);
                var itemToggle = itemParent.Q<ToolbarToggle>(PGModuleEditorUtility.ItemToggle + i);
                var itemMenu = itemParent.Q<ToolbarMenu>(PGModuleEditorUtility.ItemMenu + i);
                var itemPropertyParent = itemParent.Q<GroupBox>(PGModuleEditorUtility.ItemPropertyParent + i);
                toolbar.style.height = 24f;

                var _category = template.category;
                if (!string.IsNullOrEmpty(_category)) _category += " | ";
                var _templateName = template.templateName;
                if (string.IsNullOrEmpty(_templateName)) _templateName = "Lane Set";
                itemToggle.text = _category + _templateName;

                itemToggle.value = template._editorVisible;
                itemPropertyParent.PGDisplayStyleFlex(template._editorVisible);
                itemToggle.RegisterValueChangedCallback(evt =>
                {
                    template._editorVisible = !template._editorVisible;
                    itemPropertyParent.PGDisplayStyleFlex(template._editorVisible);
                    DrawLanePresets();
                });

                var i1 = i;

                itemMenu.PGAppendMoveItems(roadConstructor._RoadSet.lanePresets, i1, () =>
                {
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawLanePresets();
                });
                itemMenu.menu.AppendAction("Duplicate", action =>
                {
                    var duplicate = PGClassUtility.CopyClass(roadConstructor._RoadSet.lanePresets[i1]) as LanePreset;
                    duplicate.lanes = new List<SplineEdgeEditor>();
                    for (var j = 0; j < roadConstructor._RoadSet.lanePresets[i1].lanes.Count; j++)
                    {
                        var duplicateItem = PGClassUtility.CopyClass(roadConstructor._RoadSet.lanePresets[i1].lanes[j]) as SplineEdgeEditor;
                        duplicate.lanes.Add(duplicateItem);
                    }

                    roadConstructor._RoadSet.lanePresets.Insert(i1, duplicate);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawLanePresets();
                });
                itemMenu.menu.AppendAction("Remove", action =>
                {
                    roadConstructor._RoadSet.lanePresets.RemoveAt(i1);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawLanePresets();
                });

                /********************************************************************************************************************************/
                var itemProperty = templatesProperty.GetArrayElementAtIndex(i);
                var templateItem = template.CreatePropertyGUI(template, itemProperty);
                /********************************************************************************************************************************/

                itemPropertyParent.Add(templateItem);

                LanePresetsParent.Add(itemParent);
            }
        }


        /********************************************************************************************************************************/

        private void DrawSpawnObjectPresets()
        {
            SpawnObjectPresetsParent.Clear();
            if (roadConstructor._RoadSet == null) return;
            if (roadConstructor._editorActivePartType != PartType.SpawnObjectPreset) return;

            var areAllVisible = roadConstructor._RoadSet.spawnObjectPresets.All(spawnObject => spawnObject._editorVisible);
            if (areAllVisible) spawnObjectPresetsToggle.value = true;

            var spawnObjectsProperty = roadSetSerializedObject.FindProperty(nameof(RoadSet.spawnObjectPresets));
            spawnObjectsProperty.serializedObject.Update();

            for (var i = 0; i < roadConstructor._RoadSet.spawnObjectPresets.Count; i++)
            {
                var spawnObject = roadConstructor._RoadSet.spawnObjectPresets[i];
                var itemParent = PGModuleEditorUtility.CreateItemParentWithToggle(spawnObject, i);
                var toolbar = itemParent.Q<Toolbar>(PGModuleEditorUtility.Toolbar + i);
                var itemToggle = itemParent.Q<ToolbarToggle>(PGModuleEditorUtility.ItemToggle + i);
                var itemMenu = itemParent.Q<ToolbarMenu>(PGModuleEditorUtility.ItemMenu + i);
                var itemPropertyParent = itemParent.Q<GroupBox>(PGModuleEditorUtility.ItemPropertyParent + i);
                toolbar.style.height = 24f;

                var _category = spawnObject.category;
                if (!string.IsNullOrEmpty(_category)) _category += " | ";
                var _templateName = spawnObject.spawnObjectName;
                if (string.IsNullOrEmpty(_templateName)) _templateName = "Object Set";
                itemToggle.text = _category + _templateName;

                itemToggle.value = spawnObject._editorVisible;
                itemPropertyParent.PGDisplayStyleFlex(spawnObject._editorVisible);
                itemToggle.RegisterValueChangedCallback(evt =>
                {
                    spawnObject._editorVisible = !spawnObject._editorVisible;
                    itemPropertyParent.PGDisplayStyleFlex(spawnObject._editorVisible);
                    DrawSpawnObjectPresets();
                });

                var i1 = i;

                itemMenu.PGAppendMoveItems(roadConstructor._RoadSet.spawnObjectPresets, i1, () =>
                {
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawSpawnObjectPresets();
                });
                itemMenu.menu.AppendAction("Duplicate", action =>
                {
                    var duplicate = PGClassUtility.CopyClass(roadConstructor._RoadSet.spawnObjectPresets[i1]) as SpawnObjectPreset;
                    duplicate.spawnObjects = new List<SpawnObject>();
                    for (var j = 0; j < roadConstructor._RoadSet.spawnObjectPresets[i1].spawnObjects.Count; j++)
                    {
                        var duplicateItem = PGClassUtility.CopyClass(roadConstructor._RoadSet.spawnObjectPresets[i1].spawnObjects[j]) as SpawnObject;
                        duplicate.spawnObjects.Add(duplicateItem);
                    }

                    roadConstructor._RoadSet.spawnObjectPresets.Insert(i1, duplicate);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawSpawnObjectPresets();
                });
                itemMenu.menu.AppendAction("Remove", action =>
                {
                    roadConstructor._RoadSet.spawnObjectPresets.RemoveAt(i1);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawSpawnObjectPresets();
                });

                /********************************************************************************************************************************/
                var itemProperty = spawnObjectsProperty.GetArrayElementAtIndex(i);
                var spawnObjectItem = spawnObject.CreatePropertyGUI(spawnObject, itemProperty);
                /********************************************************************************************************************************/

                itemPropertyParent.Add(spawnObjectItem);

                SpawnObjectPresetsParent.Add(itemParent);
            }
        }


        /********************************************************************************************************************************/

        private void DrawTrafficLanePresets()
        {
            TrafficLanePresetsParent.Clear();
            if (roadConstructor._RoadSet == null) return;
            if (roadConstructor._editorActivePartType != PartType.TrafficLanePreset) return;

            AddTrafficComponentHelpBox();

            var areAllVisible = roadConstructor._RoadSet.trafficLanePresets.All(trafficLane => trafficLane._editorVisible);
            if (areAllVisible) trafficLanePresetsToggle.value = true;

            var trafficLanesProperty = roadSetSerializedObject.FindProperty(nameof(RoadSet.trafficLanePresets));
            trafficLanesProperty.serializedObject.Update();

            for (var i = 0; i < roadConstructor._RoadSet.trafficLanePresets.Count; i++)
            {
                var trafficLane = roadConstructor._RoadSet.trafficLanePresets[i];
                var itemParent = PGModuleEditorUtility.CreateItemParentWithToggle(trafficLane, i);
                var toolbar = itemParent.Q<Toolbar>(PGModuleEditorUtility.Toolbar + i);
                var itemToggle = itemParent.Q<ToolbarToggle>(PGModuleEditorUtility.ItemToggle + i);
                var itemMenu = itemParent.Q<ToolbarMenu>(PGModuleEditorUtility.ItemMenu + i);
                var itemPropertyParent = itemParent.Q<GroupBox>(PGModuleEditorUtility.ItemPropertyParent + i);
                toolbar.style.height = 24f;

                var _category = trafficLane.category;
                if (!string.IsNullOrEmpty(_category)) _category += " | ";
                var _templateName = trafficLane.trafficLanePresetName;
                if (string.IsNullOrEmpty(_templateName)) _templateName = "Traffic Lane Set";
                itemToggle.text = _category + _templateName;

                itemToggle.value = trafficLane._editorVisible;
                itemPropertyParent.PGDisplayStyleFlex(trafficLane._editorVisible);
                itemToggle.RegisterValueChangedCallback(evt =>
                {
                    trafficLane._editorVisible = !trafficLane._editorVisible;
                    itemPropertyParent.PGDisplayStyleFlex(trafficLane._editorVisible);
                    DrawTrafficLanePresets();
                });

                var i1 = i;

                itemMenu.PGAppendMoveItems(roadConstructor._RoadSet.trafficLanePresets, i1, () =>
                {
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawTrafficLanePresets();
                });
                itemMenu.menu.AppendAction("Duplicate", action =>
                {
                    var duplicate = PGClassUtility.CopyClass(roadConstructor._RoadSet.trafficLanePresets[i1]) as TrafficLanePreset;
                    duplicate.trafficLanes = new List<TrafficLaneEditor>();
                    for (var j = 0; j < roadConstructor._RoadSet.trafficLanePresets[i1].trafficLanes.Count; j++)
                    {
                        var duplicateItem =
                            PGClassUtility.CopyClass(roadConstructor._RoadSet.trafficLanePresets[i1].trafficLanes[j]) as TrafficLaneEditor;
                        duplicate.trafficLanes.Add(duplicateItem);
                    }

                    roadConstructor._RoadSet.trafficLanePresets.Insert(i1, duplicate);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawTrafficLanePresets();
                });
                itemMenu.menu.AppendAction("Remove", action =>
                {
                    roadConstructor._RoadSet.trafficLanePresets.RemoveAt(i1);
                    EditorUtility.SetDirty(roadConstructor._RoadSet);
                    DrawTrafficLanePresets();
                });

                /********************************************************************************************************************************/
                var itemProperty = trafficLanesProperty.GetArrayElementAtIndex(i);
                var trafficLaneItem = trafficLane.CreatePropertyGUI(trafficLane, itemProperty);
                /********************************************************************************************************************************/

                itemPropertyParent.Add(trafficLaneItem);

                TrafficLanePresetsParent.Add(itemParent);
            }
        }

        private void AddTrafficComponentHelpBox()
        {
            if (roadConstructor._editorActivePartType != PartType.TrafficLanePreset) return;

            if (!roadConstructor.componentSettings.addTrafficComponent)
            {
                var existingHelpBox = TrafficLanePresetsParent.Q<HelpBox>("AddTrafficCompHelpBox");
                if (existingHelpBox != null) return;
                var helpBox = new HelpBox("To add the Traffic component to new roads, the 'Add Traffic Comp.' setting needs to be activated.",
                    HelpBoxMessageType.Warning);
                helpBox.style.marginTop = 6;
                helpBox.name = "AddTrafficCompHelpBox";
                TrafficLanePresetsParent.Insert(0, helpBox);
            }
            else
            {
                var existingHelpBox = TrafficLanePresetsParent.Q<HelpBox>("AddTrafficCompHelpBox");
                if (existingHelpBox != null) TrafficLanePresetsParent.Remove(existingHelpBox);
            }
        }
    }
}