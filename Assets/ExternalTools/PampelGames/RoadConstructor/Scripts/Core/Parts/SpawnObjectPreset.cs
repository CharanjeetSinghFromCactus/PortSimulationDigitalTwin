// ----------------------------------------------------
// Road Constructor
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using System;
using System.Collections.Generic;
using PampelGames.Shared.Utility;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PampelGames.RoadConstructor
{
    [Serializable]
    public class SpawnObjectPreset : PGIModule
    {
        public bool _editorVisible = true;

        public string category;
        public string spawnObjectName;
        public List<SpawnObject> spawnObjects = new();

        public string ModuleName()
        {
            return string.Empty;
        }

        public string ModuleInfo()
        {
            return string.Empty;
        }

#if UNITY_EDITOR
        public VisualElement CreatePropertyGUI(SpawnObjectPreset spawnObjectPreset, SerializedProperty property)
        {
            return SpawnObjectPresetDrawerCreation.CreatePropertyGUI(spawnObjectPreset, property);
        }
#endif
    }

    [Serializable]
    public class SpawnObject
    {
        [PGLabel("Type")]
        [Tooltip("Type of object being spawned.\n\n" +
                 "Road: Objects spawned over the full road lenght, for example street lights, props etc.\n\n" +
                 "Intersection Approach: Approaching an intersection, for example traffic lights.\n\n" +
                 "Intersection Exit: Exiting an intersection, for example pedestrian crossings.\n\n" +
                 "Railing: Railings on the outisde of the roads.")]
        public SpawnObjectType objectType = SpawnObjectType.Road;

        [PGLabel("Object")] public GameObject obj;

        /********************************************************************************************************************************/
        // Railing

        [PGGroup("Railing")] [PGClamp] public float railingSpacing = 3f;

        [PGGroup("Railing")] [PGLabel("Offset")] [Tooltip("Local offset of the rails (width/height);")]
        public Vector2 railingOffset = Vector2.zero;

        [PGGroup("Railing")] [PGLabel("Auto Resize")] [Tooltip("Automatically adjusts the railing size.x to fill potential gaps between railings.")]
        public bool railingAutoSize = true;

        [PGGroup("Railing")] [PGLabel("Object Types")] [Tooltip("Allows to select which object types are used for the railing.")]
        public ObjectTypeSelection railingObjectType = ObjectTypeSelection.Any;

        [PGGroup("Railing")] [PGLabel("Elevation")] [Tooltip("Allows spawning only above or below the elevation height.")]
        public Elevation railingElevation = Elevation.Any;

        [PGGroup("Railing")] [PGLabel("Road Ends")] [Tooltip("Allows to spawn railings on the road ends.")]
        public bool railingRoadEnds = true;

        [PGLabel("Curvature Range")]
        [Tooltip("Allow spawning only within the curvature range (in degrees from start tangent to end tangent).")]
        [PGClamp]
        [PGGroup("Railing")]
        public Vector2 railingCurvature = new(0f, 180f);


        /********************************************************************************************************************************/
        // Custom

        [PGGroup("Custom")]
        [PGLabel("Intersection Type")]
        [Tooltip("Type of intersection:\n\n" +
                 "Intersection: Involves at least three roads meeting.\n\n" + "Lane Transition: Refers to a change of road type.\n\n" +
                 "Road End: End of a road.")]
        [PGDisplaySelection(new[] {nameof(objectType)},
            new[] {(int) SpawnObjectType.IntersectionApproach, (int) SpawnObjectType.IntersectionExit})]
        public IntersectionExitType intersectionExitType = IntersectionExitType.Intersection;

        [PGGroup("Custom")] [PGDisplaySelection(new[] {nameof(objectType)}, new[] {(int) SpawnObjectType.Road})]
        public SpacingType spacingType = SpacingType.WorldUnits;

        [PGGroup("Custom")]
        [PGDisplaySelection(new[] {nameof(objectType)}, new[] {(int) SpawnObjectType.Road})]
        [PGClamp]
        [Tooltip("Minimum spacing to the next object in world units.")]
        public float spacing = 10;

        [PGGroup("Custom")] [Tooltip("Position on the width of the road.")]
        public SpawnObjectPosition position = SpawnObjectPosition.Middle;

        [PGGroup("Custom")]
        [PGLabel("Pos. Offset Forward")]
        [Tooltip("Position offset local z-direction in world units, following the road curvature.")]
        [PGDisplaySelection(new[] {nameof(objectType)},
            new[] {(int) SpawnObjectType.IntersectionApproach, (int) SpawnObjectType.Road})]
        public float positionOffsetForward;

        [PGGroup("Custom")]
        [PGLabel("Pos. Offset Right")]
        [Tooltip("Position offset local x-direction in world units, where positive values move the object to the inside.")]
        [PGDisplaySelection(new[] {nameof(position)}, new[] {(int) SpawnObjectPosition.Side, (int) SpawnObjectPosition.BothSides})]
        public float positionOffsetRight;

        [PGGroup("Custom")] [Tooltip("Height offset from the base height")]
        public float heightOffset;

        [PGGroup("Custom")]
        [Tooltip("Y-rotation, with the object forward-axis looking towards the inside or outside the road.\n\n" +
                 "Random ranges from 0 to 360 degrees.")]
        public SpawnObjectRotation rotation = SpawnObjectRotation.Inside;

        [PGGroup("Custom")] [PGLabel("Align Normal")] [Tooltip("Aligns the objects to the road up vector.")]
        public bool alignToNormal;

        [PGGroup("Custom")] [PGMinMaxSlider(0f, 2f)] [Tooltip("Scale multiplier (random from-to).")]
        public Vector2 scale = Vector2.one;

        [PGGroup("Custom")]
        [PGDisplaySelection(new[] {nameof(objectType)}, new[] {(int) SpawnObjectType.IntersectionApproach})]
        [Tooltip("Allow only certain intersection directions for this object.")]
        public bool requiresDirection;

        [PGGroup("Custom")]
        [PGLabel("Forward")]
        [PGMargin]
        [Tooltip("Requires intersection forward direction.")]
        [PGDisplaySelection(new[] {nameof(objectType)}, new[] {(int) SpawnObjectType.IntersectionApproach})]
        public bool requiresDirectionForward;

        [PGGroup("Custom")]
        [PGLabel("Left")]
        [PGMargin]
        [Tooltip("Requires intersection left direction.")]
        [PGDisplaySelection(new[] {nameof(objectType)}, new[] {(int) SpawnObjectType.IntersectionApproach})]
        public bool requiresDirectionLeft;

        [PGGroup("Custom")]
        [PGLabel("Right")]
        [PGMargin]
        [Tooltip("Requires intersection right direction.")]
        [PGDisplaySelection(new[] {nameof(objectType)}, new[] {(int) SpawnObjectType.IntersectionApproach})]
        public bool requiresDirectionRight;

        [PGGroup("Custom")] [Tooltip("Allows spawning only above or below the elevation height.")]
        public Elevation elevation = Elevation.Any;

        [PGGroup("Custom")]
        [Tooltip("Allows spawning only within a specified range of height, measured as the distance from the ground to the road.")]
        public Vector2 heightRange = new(0f, 10f);

        [PGGroup("Custom")]
        [PGLabel("Curvature Range")]
        [Tooltip("Allows spawning only within the curvature range (in degrees from start tangent to end tangent).")]
        [PGClamp(0f, 180f)]
        public Vector2 curvature = new(0f, 180f);

        [PGGroup("Custom")]
        [Tooltip("Removes spawns if they overlap with other roads or intersections.\n\n" +
                 "Useful for elements such as pillars or objects which are larger than the elevation height.")]
        public bool removeOverlap;

        [PGGroup("Custom")] [PGSlider] [Tooltip("Chance to spawn this object.")]
        public float chance = 1f;
    }
}