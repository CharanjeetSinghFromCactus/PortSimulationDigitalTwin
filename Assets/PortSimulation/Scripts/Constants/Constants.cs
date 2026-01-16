namespace PortSimulation
{
    using UnityEngine;
    using System;
    using System.Collections;


    public static class ObserverNameConstants
    {
        public const string canUsePanTool = "canUsePanTool";
        public const string CanPlacePlaceableObject = "canPlacePlaceableObject";
        public const string OnPlacementItemClick = "OnPlacementItemClick";
        public const string SendSelectedToolName = "SendSelectedToolName";
        public const string SetPlacementTarget = "SetPlacementTarget";
        public const string DeleteObject = "DeleteObject";
        public const string SetRoadType = "SetRoadType";
        public const string CanBuildRoad = "CanBuildRoad";
        public const string UndoRoad = "UndoRoad";
        public const string DemolishRoad = "DemolishRoad";
        public const string SaveRoad = "SaveRoad";
    }

    public static class ToolNameConstants
    {
        public const string PanToolName = "PanTool";
        public const string PlacementPanToolName = "PlacementPanTool";
        public const string PlacementRotateToolName = "PlacementRotateTool";
        public const string PlacementScaleToolName = "PlacementScaleTool";
        public const string RoadConstructionToolName = "RoadConstructionTool";
    }

    public static class PropertyNameConstants
    {
        public const string PlacementCategoryNameProperty = "PlacementCategoryName";
    }

    public static class PlacementPersistantConstant
    {
        public const string placementDataFileName = "PlacementData.dat";
    }
}