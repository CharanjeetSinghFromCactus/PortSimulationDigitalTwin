// ----------------------------------------------------
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace PampelGames.Shared.Construction
{
    public static class ObjectUtility
    {
        public const string PrefixUndo = "UndoObject_";
        public static GameObject CreateObj(ConstructionPool pool, string prefix, ShadowCastingMode shadowCastingMode, 
            out MeshFilter meshFilter, out MeshRenderer meshRenderer)
        {
            var obj = pool.GetRenderGameObject(out meshFilter, out meshRenderer);
            obj.name = prefix + "_" + obj.GetInstanceID();
            meshRenderer.shadowCastingMode = shadowCastingMode;
            return obj;
        }

        public static GameObject CreateLODObj(ConstructionPool pool, ShadowCastingMode shadowCastingMode, GameObject parent, int lod,
            out MeshFilter meshFilter, out MeshRenderer meshRenderer)
        {
            var obj = pool.GetRenderGameObject(out meshFilter, out meshRenderer);
            obj.transform.SetParent(parent.transform);
            obj.name = parent.name + "_LOD" + lod;
            meshRenderer.shadowCastingMode = shadowCastingMode;
            return obj;
        }
        
        public static GameObject CreateUndoObj(ConstructionPool pool)
        {
            var obj = pool.GetGameObject();
            obj.name = PrefixUndo + obj.GetInstanceID();
            return obj;
        }
        
        public static void DestroyObject(ConstructionPool pool, Object obj)
        {
            if (obj is SpawnedConstructionObject spawnedConstructionObject)
            {
                pool.ReleaseSpawn(spawnedConstructionObject);
                return;
            }
            if (obj is GameObject gameObject)
            {
                pool.ReleaseGameObject(gameObject);
                return;
            }
            
            DestroyObjectDirect(obj);
        }

        public static void DestroyObjectDirect(Object obj)
        {
            if (Application.isPlaying) Object.Destroy(obj);
            else Object.DestroyImmediate(obj);       
        }

#if UNITY_EDITOR
        public static SplineContainer CreateTestObject(ISpline spline, string name = "Test Spline")
        {
            var test = new GameObject(name);
            var cont = test.AddComponent<SplineContainer>();
            var testSpline = new Spline(spline);
            testSpline.SetTangentMode(TangentMode.Broken);
            cont.Spline = testSpline;
            
            var parent = GameObject.Find("TestSplineParent");
            if (parent == null) parent = new GameObject("TestSplineParent");
            test.transform.SetParent(parent.transform);
            
            return cont;
        }
        
        public static void RemoveTestObjects()
        {
            if (Application.isPlaying) Object.Destroy(GameObject.Find("TestSplineParent"));
            else Object.DestroyImmediate(GameObject.Find("TestSplineParent"));
        }
#endif
    }
}