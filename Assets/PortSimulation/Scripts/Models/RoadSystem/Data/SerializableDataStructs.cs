using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace PortSimulation.RoadSystem.Data
{
    [Serializable]
    public struct SerializableKnot
    {
        public Vector3 Position;
        public Vector3 TangentIn;
        public Vector3 TangentOut;
        public Quaternion Rotation;

        public SerializableKnot(BezierKnot knot)
        {
            Position = knot.Position;
            TangentIn = knot.TangentIn;
            TangentOut = knot.TangentOut;
            Rotation = knot.Rotation;
        }

        public BezierKnot ToBezierKnot()
        {
            return new BezierKnot(Position, TangentIn, TangentOut, Rotation);
        }
    }

    [Serializable]
    public class SerializableSpline
    {
        public List<SerializableKnot> Knots = new List<SerializableKnot>();
        public bool Closed;

        public SerializableSpline() { }

        public SerializableSpline(Spline spline)
        {
            if (spline == null) return;
            Closed = spline.Closed;
            foreach (var knot in spline.Knots)
            {
                Knots.Add(new SerializableKnot(knot));
            }
        }

        public Spline ToSpline()
        {
            var spline = new Spline();
            spline.Closed = Closed;

            if (Knots == null) Knots = new List<SerializableKnot>();

            foreach (var k in Knots)
            {
                spline.Add(k.ToBezierKnot());
            }
            return spline;
        }
    }

    [Serializable]
    public class SavedRoad
    {
        // Fields from SerializedRoad that we need
        public string roadName;
        public bool elevated;
        public bool rampRoad;
        public string splitOriginalID;
        public SerializableSpline serializableSpline;
        // We can ignore spawnedObjects for now if they are re-generated, 
        // or we need to map them too if we want them saved. 
        // SerializedRoad has: List<SerializedSpawnedObject> spawnedObjects
        // Let's include them to be safe.
        public List<PampelGames.RoadConstructor.SerializedSpawnedObject> spawnedObjects;

        public SavedRoad() { }

        public SavedRoad(PampelGames.RoadConstructor.SerializedRoad serializedRoad)
        {
            this.roadName = serializedRoad.roadName;
            this.elevated = serializedRoad.elevated;
            this.rampRoad = serializedRoad.rampRoad;
            this.splitOriginalID = serializedRoad.splitOriginalID;
            this.spawnedObjects = serializedRoad.spawnedObjects;
            this.serializableSpline = new SerializableSpline(serializedRoad.spline);
        }

        public PampelGames.RoadConstructor.SerializedRoad ToSerializedRoad()
        {
            // We need to construct SerializedRoad.
            // Constructor: SerializedRoad(string roadName, bool elevated, List<SerializedSpawnedObject> spawnedObjects, bool rampRoad, Spline spline, string splitOriginalID, Spline splitOriginalSpline)

            // Note: splitOriginalSpline is also a Spline! We might need to serialize that too if it's important.
            // For now, let's assume it's null or not critical, or handle it if needed. 
            // Looking at RoadObject.cs, Serialize() passes splitOriginalSpline.

            // Let's keep it simple first. Use null for splitOriginalSpline if we didn't save it.

            Spline restoredSpline;
            if (serializableSpline != null)
            {
                restoredSpline = serializableSpline.ToSpline();
            }
            else
            {
                // Fallback if spline data is missing (should not happen with new saves)
                restoredSpline = new Spline(); // Empty spline
            }

            return new PampelGames.RoadConstructor.SerializedRoad(
                roadName,
                elevated,
                spawnedObjects,
                rampRoad,
                restoredSpline,
                splitOriginalID,
                null // We are not saving splitOriginalSpline yet. usually only needed if we want to unsplit?
            );
        }
    }
}
