using UnityEngine;
using System;

namespace PortSimulation.RoadSystem.Data
{
    [CreateAssetMenu(fileName = "RoadDataContainer", menuName = "PortSimulation/Containers/RoadDataContainer")]
    public class RoadDataContainer : ScriptableObject
    {
        [field: SerializeField] public RoadData[] Roads { get; private set; }

        public RoadData GetRoadData(string roadName)
        {
            return Array.Find(Roads, x => x.RoadName == roadName);
        }


        [ContextMenu("Add Default Data")]
        public void AddDefaultData()
        {
            Roads = new RoadData[]
            {
                new RoadData("One Way", "One-Way Asphalt"),
                new RoadData("Two Way", "Side Street Asphalt"),
                new RoadData("Four Way", "Boulevard Asphalt"),
                new RoadData("Six Way", "Main Street Asphalt")
            };
        }
    }

    [Serializable]
    public class RoadData
    {
        [field: SerializeField] public string RoadName { get; private set; }
        [field: SerializeField] public string RoadId { get; private set; } // The ID used by RoadConstructor
        [field: SerializeField] public Sprite RoadIcon { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        public RoadData(string name, string id)
        {
            RoadName = name;
            RoadId = id;
        }
    }
}
