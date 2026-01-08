// ----------------------------------------------------
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using PampelGames.Shared.Construction;

namespace PampelGames.RoadConstructor
{
    public class ConstructionObjects
    {
        public readonly List<IntersectionObject> newIntersections = new();
        public readonly List<IntersectionObject> newReplacedIntersections = new();
        public readonly List<IntersectionObject> removableIntersections = new();
        public readonly List<RoadObject> newRoads = new();
        public readonly List<RoadObject> newReplacedRoads = new();
        public readonly List<RoadObject> removableRoads = new();
        public readonly List<RoadObject> updatableRoads = new(); // Updates connections and creates end objects (if applicable)

        public List<RoadObject> CombinedNewRoads
        {
            get
            {
                var combinedList = new List<RoadObject>(newRoads);
                combinedList.AddRange(newReplacedRoads);
                return combinedList;
            }
        }

        public List<IntersectionObject> CombinedNewIntersections
        {
            get
            {
                var combinedList = new List<IntersectionObject>(newIntersections);
                combinedList.AddRange(newReplacedIntersections);
                return combinedList;
            }
        }

        public List<SceneObject> CombinedNewObjects
        {
            get
            {
                var combinedList = new List<SceneObject>(newRoads);
                combinedList.AddRange(newReplacedRoads);
                combinedList.AddRange(newIntersections);
                combinedList.AddRange(newReplacedIntersections);
                return combinedList;
            }
        }

        public List<SceneObject> CombinedRemovableObjects
        {
            get
            {
                var combinedList = new List<SceneObject>(removableRoads);
                combinedList.AddRange(removableIntersections);
                return combinedList;
            }
        }

        public List<ConstructionFail> ValidateDuplicates()
        {
            var constructionFails = new List<ConstructionFail>();
            var duplicateRoads = new HashSet<RoadObject>();
            if (removableRoads.Any(road => !duplicateRoads.Add(road)))
            {
                constructionFails.Add(new ConstructionFail(FailCause.OverlapTrack));
                return constructionFails;
            }

            var duplicateIntersections = new HashSet<IntersectionObject>();
            if (removableIntersections.Any(intersection => !duplicateIntersections.Add(intersection)))
                constructionFails.Add(new ConstructionFail(FailCause.OverlapTrack));
            return constructionFails;
        }

        public ConstructionObjects IsolateElevated(ConstructionObjects constructionObjects, bool elevated)
        {
            var constructionObjectsIsolated = new ConstructionObjects();

            for (var i = 0; i < constructionObjects.newIntersections.Count; i++)
            {
                var _intersectionObject = constructionObjects.newIntersections[i];
                if ((elevated && _intersectionObject.elevated) || (!elevated && !_intersectionObject.elevated))
                    constructionObjectsIsolated.newIntersections.Add(_intersectionObject);
            }

            for (var i = 0; i < constructionObjects.newRoads.Count; i++)
            {
                var _roadObject = constructionObjects.newRoads[i];
                if ((elevated && _roadObject.elevated) || (!elevated && !_roadObject.elevated))
                    constructionObjectsIsolated.newRoads.Add(_roadObject);
            }

            for (var i = 0; i < constructionObjects.newReplacedIntersections.Count; i++)
            {
                var _intersectionObject = constructionObjects.newReplacedIntersections[i];
                if ((elevated && _intersectionObject.elevated) || (!elevated && !_intersectionObject.elevated))
                    constructionObjectsIsolated.newReplacedIntersections.Add(_intersectionObject);
            }

            for (var i = 0; i < constructionObjects.newReplacedRoads.Count; i++)
            {
                var _roadObject = constructionObjects.newReplacedRoads[i];
                if ((elevated && _roadObject.elevated) || (!elevated && !_roadObject.elevated))
                    constructionObjectsIsolated.newReplacedRoads.Add(_roadObject);
            }

            constructionObjectsIsolated.removableIntersections.AddRange(constructionObjects.removableIntersections);
            constructionObjectsIsolated.removableRoads.AddRange(constructionObjects.removableRoads);
            constructionObjectsIsolated.updatableRoads.AddRange(constructionObjects.updatableRoads);
            return constructionObjectsIsolated;
        }

        public void Merge(ConstructionObjects other)
        {
            newRoads.AddRange(other.newRoads);
            newReplacedRoads.AddRange(other.newReplacedRoads);
            newIntersections.AddRange(other.newIntersections);
            newReplacedIntersections.AddRange(other.newReplacedIntersections);
            removableRoads.AddRange(other.removableRoads);
            removableIntersections.AddRange(other.removableIntersections);
            updatableRoads.AddRange(other.updatableRoads);
        }

        public void DestroyNewObjects(ConstructionPool pool)
        {
            foreach (var newRoad in newRoads) ObjectUtility.DestroyObject(pool, newRoad.gameObject);
            foreach (var newRoad in newReplacedRoads) ObjectUtility.DestroyObject(pool, newRoad.gameObject);
            foreach (var newIntersection in newIntersections) ObjectUtility.DestroyObject(pool, newIntersection.gameObject);
            foreach (var newIntersection in newReplacedIntersections) ObjectUtility.DestroyObject(pool, newIntersection.gameObject);
            newRoads.Clear();
            newReplacedRoads.Clear();
            newIntersections.Clear();
            newReplacedIntersections.Clear();
        }

        public void DestroyRemovableObjects(ConstructionPool pool)
        {
            foreach (var removableRoad in removableRoads) ObjectUtility.DestroyObject(pool, removableRoad.gameObject);
            foreach (var removableIntersection in removableIntersections) ObjectUtility.DestroyObject(pool, removableIntersection.gameObject);
            removableRoads.Clear();
            removableIntersections.Clear();
        }
    }
}