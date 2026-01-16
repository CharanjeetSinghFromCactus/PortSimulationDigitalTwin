using System.Collections.Generic;
using PampelGames.RoadConstructor;

namespace PortSimulation.RoadSystem.Data
{
    [System.Serializable]
    public class RoadSystemSaveData
    {
        public List<SavedRoad> roads = new List<SavedRoad>();
        public List<SerializedIntersection> intersections = new List<SerializedIntersection>();
        public List<SerializedRoundabout> roundabouts = new List<SerializedRoundabout>();
    }
}
