using System;
using UnityEngine;

namespace PortSimulation.Data
{
    [Serializable]
    public class PanToolData
    {
        public float PanSpeed = 0.1f;
    }

    [Serializable]
    public class PlacementPanToolData
    {
        public float LerpSpeed = 10f;
        public float MaxRaycastDistance = 1000f;
        public LayerMask PlacementAreaMask;
    }

    [Serializable]
    public class PlacementRotateToolData
    {
        public float RotateSpeed = 120f;
    }

    [Serializable]
    public class PlacementScaleToolData
    {
        public float Speed = 1f;
        public float MinScale = 0.1f;
        public float MaxScale = 10f;
    }

    [CreateAssetMenu(fileName = "ToolConfig", menuName = "PortSimulation/ToolConfig")]
    public class ToolConfig : ScriptableObject
    {
        public PanToolData PanToolData;
        public PlacementPanToolData PlacementPanToolData;
        public PlacementRotateToolData PlacementRotateToolData;
        public PlacementScaleToolData PlacementScaleToolData;
    }
}