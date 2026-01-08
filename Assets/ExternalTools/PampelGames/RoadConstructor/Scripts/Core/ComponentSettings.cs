// ----------------------------------------------------
// Road Constructor
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using System;
using System.Collections.Generic;
using PampelGames.Shared.Construction;
using PampelGames.Shared.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace PampelGames.RoadConstructor
{
    [Serializable]
    public class ComponentSettings
    {
        /********************************************************************************************************************************/
        // Integrations
        [PGHide] public bool integrationActive = true;
        [PGHide] public bool integrationDetectOverlap = true;


        /********************************************************************************************************************************/
        // Quality
        [PGHeader("Quality", HeaderType.Big)]
        [Tooltip("Number of subdivisions for a 10-unit length of road. The subdivisions will be adjusted for roads of differing lengths.")]
        [PGClamp(1)]
        public int resolution = 5;

        [Tooltip("Determines the level of detail in creating end-parts, intersections, etc.")] [PGClamp(1)]
        public int detailResolution = 15;

        [Tooltip("Automatically adjusts resolution based on the curvature and slope. Recommended for most use cases.")]
        public bool smartReduce = true;

        [PGLabel("Collider")]
        [Tooltip(
            "Adds mesh colliders to the roads and intersections.\n\nNote that if you add colliders, make sure the road layer is different than the ground layer specified below!")]
        public AddCollider addCollider = AddCollider.None;

        [PGLayerField]
        [PGLabel("Road Layer")]
        [Tooltip("Layer for the roads/intersection. Assigned only to LOD0 objects equipped with mesh filters.")]
        public int addColliderLayer;

        [PGTagField] [Tooltip("Tag for the roads/intersections. Assigned only to the main road/intersection object, not its LOD children.")]
        public string roadTag = Constants.DefaultTag;

        [PGListViewLOD] [Tooltip("Add additional Level of Detail for intersections, where LOD0 is the original.")]
        public List<float> lodList = new();


        /********************************************************************************************************************************/
        // Construction
        [PGHeader("Construction", HeaderType.Big)] [PGLabel("Base Height")] [Tooltip("Default height offset for road and intersection lanes.")]
        public float baseRoadHeight = 0.1f;

        [PGClamp] [Tooltip("Snap position on the x/z plane and y height. Useful for grid-style placement.")]
        public Vector3 grid;

        [PGClamp] [Tooltip("Offset to the position.")]
        public Vector3 gridOffset;

        [PGTopLine] [PGClamp] [Tooltip("Snap distance to existing roads/intersections, relative to the nearest road width.")]
        public float snapDistance = 1f;

        [PGClamp] [Tooltip("Snap distance to existing roads/intersections in y world units.")]
        public float snapHeight = 2f;

        [PGLabel("Snap Angle")] [PGClamp] [Tooltip("Snap angle in degrees to existing roads/intersections.")]
        public float snapAngleIntersection;

        [PGTopLine] [Tooltip("If a new track connects to the same type, its position snaps to the center of the existing track.")]
        public bool snapTrack = true;

        [PGLabel("Max. Length")]
        [PGMargin]
        [PGDisplaySelection(new[] {nameof(snapTrack)})]
        [PGClamp]
        [Tooltip("Snap Track: The maximum length of the existing track (multiplier to the width).")]
        public float snapTrackMaxLength = 2f;

        [PGLabel("Min. Angle")]
        [PGMargin]
        [PGDisplaySelection(new[] {nameof(snapTrack)})]
        [PGClamp]
        [Tooltip("Snap Track: The minimum curvature of the existing track (in degrees).")]
        public float snapTrackMinAngle = 30f;

        [PGTopLine]
        [PGLabel("Min. Angle Int.")]
        [PGClamp]
        [Tooltip(
            "The minimum angle required for roads to exit an intersection towards the nearest existing road.\nWhen this angle is reached, the road will curve.")]
        public float minAngleIntersection = 45f;

        [PGLabel("Dist. / Angle Road")]
        [Tooltip(
            "Determines how new roads are curved when connected to intersections.\n\nX-Axis: Road Length Forward / Road Length Right\n\nY-Axis: Angle in degrees.")]
        public AnimationCurve distanceRatioAngleCurve = AnimationCurve.Linear(1.5f, 90f, 4f, 0f);

        [PGTopLine]
        [PGLabel("Road UV")]
        [Tooltip(
            "Determines how UVs are handled over the length of the road.\n\nStretch: The UVs are stretched to fit the nearest full size of the road.\n\nCut: The UVs maintain their original size and are cut when the road ends.")]
        public SplineLengthUV splineLengthUV = SplineLengthUV.Cut;

        [PGSlider] [Tooltip("Lengths of the road curve tangents.\n" + "Higher values result in smaller corners.")]
        public float tangentLength = 0.5f;

        [PGLabel("Intersection Dist.")]
        [PGClamp]
        [Tooltip("Distance to intersections.\nSpace is used for example for crosswalks and mesh connections.")]
        public float intersectionDistance = 2;

        [Tooltip("The road curves can be aligned if both the start and end are connected to existing roads or intersections.\n\n" +
                 "Otherwise the connection tangents are free.")]
        public bool alignConnections = true;

        [Tooltip("If a new road connects to the same type, it connects directly to the existing one, omitting a connection track.")]
        public bool directConnections;

        [Tooltip("How road ends are constructed when not connected to intersections.")]
        public RoadEnd roadEnd = RoadEnd.Rounded;


        /********************************************************************************************************************************/
        // Random
        [Tooltip("Use a fixed random seed for randomized values.")] [PGTopLine]
        public bool randomSeed;

        [Tooltip("Seed value.")] [PGMargin] [PGDisplaySelection(new[] {nameof(randomSeed)})]
        public int seed;


        /********************************************************************************************************************************/
        // Validation
        [PGHeader("Validation", HeaderType.Big)] [Tooltip("Validates the road heights against the ground.")]
        public bool validateHeight = true;

        [Tooltip("Validates the road overlaps against other road/intersections.")]
        public bool validateOverlap = true;

        [PGClamp] [Tooltip("Min. to Max. length of a road.")] [PGVectorComponentLabel("Min", "Max")]
        public Vector2 roadLength = new(5f, 10000f);

        [PGLabel("Min. Overlap Dist.")] [PGClamp] [Tooltip("Minimum required distance between the road and overlapping roads/intersections.")]
        public float minOverlapDistance = 1f;

        [PGLabel("Max. Curvature")] [PGClamp(0f, 180f)] [Tooltip("The maximum curvature of the road.")]
        public float maxCurvature = 110f;


        /********************************************************************************************************************************/
        // Undo / Demolish
        [PGHeader("Undo / Demolish", HeaderType.Big)]
        [Tooltip(
            "The number of undoable actions that can be stored in the scene, and also the maximum number of successive undo operations.\n\nIf undo operations are not required, you can set this value to 0 to save on memory usage.")]
        [PGClamp]
        public int undoStorageSize = 5;

        [PGLabel("Undo Terrain")] [Tooltip("Resets the terrain when undoing construction.")]
        public bool undoResetsTerrain = true;

        [PGLabel("Demolish Terrain")] [Tooltip("Resets the terrain when demolishing roads.")]
        public bool demolishResetsTerrain = true;


        /********************************************************************************************************************************/
        // Elevation
        [PGHeader("Elevation", HeaderType.Big)] [Tooltip("Layers of the ground which are used to verify the elevation height.")]
        public LayerMask groundLayers;

        [Tooltip("Construction is only allowed within this distance to the ground.\n\n" +
                 "If you have a Min. value below 0, the road could be underneath the ground.\n\n" +
                 "Tip: The Terrain options could be helpful.")]
        [PGVectorComponentLabel("Min", "Max")]
        public Vector2 heightRange = new(-4, 12);

        [Tooltip("Distance from the road to the ground above this level is considered elevated.")] [PGLabel("Elevation Height")]
        public float elevationStartHeight = 1f;

        public float minOverlapHeight = 3f;

        [Tooltip("Allow elevated intersections.")] [PGLabel("Elev. Intersections")]
        public bool elevatedIntersections = true;

        [PGClamp(0f, 90f)] [Tooltip("The maximum slope of the road in degrees.")]
        public float maxSlope = 30f;

        public bool smoothSlope;


        /********************************************************************************************************************************/
        // Terrain
        [PGHeader("Terrain", HeaderType.Big)] [Tooltip("Apply terrain modifications.")]
        public bool terrainSettings;

        [PGDisplaySelection(new[] {nameof(terrainSettings)})] [Tooltip("Terrains")]
        public List<Terrain> terrains = new();

        [PGDisplaySelection(new[] {nameof(terrainSettings)})] [Tooltip("Removes terrain details along the road.")]
        public bool removeDetails;

        [PGDisplaySelection(new[] {nameof(terrainSettings)})] [Tooltip("Removes terrain trees along the road.")]
        public bool removeTrees;

        [PGDisplaySelection(new[] {nameof(terrainSettings)})]
        [Tooltip("Adjusts the terrain height to match the road height, up to the elevation height.")]
        public bool levelHeight;

        [PGGroup("SlopeTexture", FlexDirection.Row)]
        [PGDisplaySelection(new[] {nameof(terrainSettings), nameof(levelHeight)}, true, false)]
        [Tooltip("Terrain texture to be used for slopes resulting from terrain fitting. Set to -1 to disable.")]
        [PGClamp(-1, int.MaxValue)]
        [PGLabel("Slope Texture")]
        public int slopeTextureIndex = -1;

        [PGGroup("SlopeTexture", FlexDirection.Row)]
        [PGDisplaySelection(new[] {nameof(terrainSettings), nameof(levelHeight)}, true, false)]
        [PGLabel("")]
        [Tooltip("Name of the texture layer on the terrain (read-only).")]
        public string slopeTextureName;

        [PGDisplaySelection(new[] {nameof(terrainSettings), nameof(levelHeight)}, true, false)]
        [Tooltip("Strenght of the texture layer on the terrain.")]
        [PGSlider]
        [PGLabel("Texture Strength")]
        public float slopeTextureStrength = 0.75f;

        [PGDisplaySelection(new[] {nameof(terrainSettings), nameof(levelHeight)}, true, false)]
        [Tooltip("The width of the slopes resulting from terrain fitting, measured in heightmap pixels.")]
        [PGSlider(0, 5)]
        public int slopeSmooth = 1;


        /********************************************************************************************************************************/
        // Traffic System
        [PGHeader("Traffic System", HeaderType.Big)]
        [PGLabel("Add Traffic Comp.")]
        [Tooltip(
            "Adds the 'Traffic' component to roads/intersections.\n\nThis component can be useful for custom traffic systems as it adds additional splines for each lane, which can be added in the 'Traffic Lanes' on the roads.")]
        public bool addTrafficComponent;

        [PGDisplaySelection(new[] {nameof(addTrafficComponent)})]
        [Tooltip(
            "Updates the waypoints automatically for each road/intersection after construction, undo and demolish.\n\nWaypoints can be retrieved from each road/intersection by calling GetTrafficLanes() first and then calling GetWaypoints() on each lane.\n\nOr to get all waypoints from the system, simply call roadConstructor.GetWaypoints()")]
        public bool updateWaypoints = true;

        [PGDisplaySelection(new[] {nameof(addTrafficComponent), nameof(updateWaypoints)}, true, false)]
        [PGVectorComponentLabel("Min", "Max")]
        [PGClamp]
        [Tooltip("Maximum distance between each waypoint, based on curvature.\nIntersections always use the minimum value.")]
        public Vector2 waypointDistance = new(2f, 4f);

        [PGDisplaySelection(new[] {nameof(addTrafficComponent), nameof(updateWaypoints)}, true, false)]
        [Tooltip("Whether to draw gizmos for waypoints in the scene.")]
        public DrawGizmos waypointGizmos = DrawGizmos.None;

        [PGDisplaySelection(new[] {nameof(addTrafficComponent), nameof(updateWaypoints)}, true, false)]
        [PGLabel("Color")]
        [PGMargin]
        [Tooltip("Based on scene objects or a unique color for each traffic lane.")]
        public DrawGizmosColor waypointGizmosColor = DrawGizmosColor.Object;

        [PGDisplaySelection(new[] {nameof(addTrafficComponent), nameof(updateWaypoints)}, true, false)] [PGLabel("Size")] [PGMargin] [PGClamp]
        public float waypointGizmoSize = 1f;

        [PGDisplaySelection(new[] {nameof(addTrafficComponent), nameof(updateWaypoints)}, true, false)]
        [PGLabel("Start/End Only")]
        [PGMargin]
        [Tooltip("Draw only the start and end waypoints. Helpful for improving editor performance.")]
        public bool waypointConnectionsOnly;


        /********************************************************************************************************************************/
        // Pooling
        [PGHeader("Pooling", HeaderType.Big)]
        [Tooltip("Enables pooling.\n" +
                 "To disable pooling for a specific type, set the pool size to 0.")]
        public bool poolEnabled = true;

        [PGDisplaySelection(new[] {nameof(poolEnabled)})]
        [PGClamp]
        [Tooltip("Maximum number of pooled GameObjects for each pooled object type. \n\nSet to 0 to disable pooling.")]
        public int poolSizeObjects = 10;

        [PGDisplaySelection(new[] {nameof(poolEnabled)})]
        [PGClamp]
        [Tooltip("Maximum number of pooled spawns for each spawned object type. \n\nSet to 0 to disable pooling.")]
        public int poolSizeSpawns = 10;

        [PGDisplaySelection(new[] {nameof(poolEnabled)})] [PGClamp] [Tooltip("Maximum number of pooled meshes. \n\nSet to 0 to disable pooling.")]
        public int poolSizeMeshes = 10;


        /********************************************************************************************************************************/
        /********************************************************************************************************************************/

        public TerrainUpdateSettings CreateTerrainUpdateSettings()
        {
            var terrainUpdateSettings = new TerrainUpdateSettings(elevationStartHeight, groundLayers, Constants.RaycastOffset(this),
                levelHeight, slopeTextureIndex, slopeTextureStrength, slopeSmooth, removeDetails, removeTrees);

            return terrainUpdateSettings;
        }
    }
}