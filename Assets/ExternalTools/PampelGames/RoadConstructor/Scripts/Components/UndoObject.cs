// ----------------------------------------------------
// Road Constructor
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using System.Collections.Generic;
using PampelGames.Shared.Construction;
using UnityEngine;

namespace PampelGames.RoadConstructor
{
    public class UndoObject : MonoBehaviour, IPoolableConstruction
    {
        public ConstructionObjects constructionObjects;
    }

    internal static class UndoObjectUtility
    {
        public static void RegisterUndo(ComponentSettings settings, Transform undoParent, LinkedList<UndoObject> undoObjects,
            ConstructionObjects constructionObjects, ConstructionPool pool)
        {
            if (settings.undoStorageSize <= 0)
            {
                constructionObjects.DestroyRemovableObjects(pool);
                return;
            }

            var undoObj = ObjectUtility.CreateUndoObj(pool);

            var removableObjects = constructionObjects.CombinedRemovableObjects;

            foreach (var removableObject in removableObjects)
            {
                removableObject.transform.SetParent(undoObj.transform);
                removableObject.gameObject.SetActive(false);
            }

            undoObj.transform.SetParent(undoParent);
            var undoObject = undoObj.AddComponent<UndoObject>();

            undoObject.constructionObjects = constructionObjects;

            undoObjects.AddLast(undoObject);
            if (undoObjects.Count > settings.undoStorageSize)
            {
                var dequeuedUndo = undoObjects.First.Value;
                ObjectUtility.DestroyObject(pool, dequeuedUndo.gameObject);
                undoObjects.RemoveFirst();
            }
        }
    }
}