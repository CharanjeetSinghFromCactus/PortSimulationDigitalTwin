// ----------------------------------------------------
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PampelGames.Shared.Construction
{
    /// <summary>
    ///     Represents a pool for managing reusable construction-related objects.
    /// </summary>
    public class ConstructionPool : MonoBehaviour
    {
        public bool poolEnabled = true;
        public int poolSizeObjects;
        public int poolSizeSpawns;
        public int poolSizeMeshes;

        private readonly HashSet<PooledGameObject> pooledGameObjects = new();
        private readonly HashSet<PooledGameObject> pooledRenderGameObjects = new();
        private readonly HashSet<Mesh> pooledMeshes = new();
        private static readonly Dictionary<int, HashSet<GameObject>> pooledSpawns = new();

        private Transform objectParent;
        private Transform spawnParent;

        private class PooledGameObject
        {
            public int frame;
            public GameObject obj;
            public MeshFilter meshFilter;
            public MeshRenderer meshRenderer;
        }

        public void Initialize(bool _poolEnabled, int _poolSizeObjects, int _poolSizeSpawns, int _poolSizeMeshes)
        {
            if(!objectParent) objectParent = new GameObject("Objects").transform;
            if(!spawnParent) spawnParent = new GameObject("Spawns").transform;
            objectParent.SetParent(transform);
            spawnParent.SetParent(transform);
            
            poolEnabled = _poolEnabled;
            poolSizeObjects = _poolSizeObjects;
            poolSizeSpawns = _poolSizeSpawns;
            poolSizeMeshes = _poolSizeMeshes;
        }

        /********************************************************************************************************************************/

        public GameObject GetGameObject()
        {
            if (!poolEnabled || poolSizeObjects <= 0 || pooledGameObjects.Count == 0) return new GameObject();
            PooledGameObject pooledObj;
            if (Application.isPlaying) pooledObj = pooledGameObjects.FirstOrDefault(x => x.frame != Time.frameCount);
            else pooledObj = pooledGameObjects.FirstOrDefault();
            if (pooledObj == null)
            {
                pooledGameObjects.RemoveWhere(x => x == null);
                return new GameObject();
            }

            pooledGameObjects.Remove(pooledObj);
            pooledObj.obj.SetActive(true);
            return pooledObj.obj;
        }

        public GameObject GetRenderGameObject(out MeshFilter meshFilter, out MeshRenderer meshRenderer)
        {
            if (!poolEnabled || poolSizeObjects <= 0 || pooledRenderGameObjects.Count == 0)
            {
                var obj = new GameObject();
                meshFilter = obj.AddComponent<MeshFilter>();
                meshRenderer = obj.AddComponent<MeshRenderer>();
                return obj;
            }

            PooledGameObject pooledObj;
            if (Application.isPlaying) pooledObj = pooledRenderGameObjects.FirstOrDefault(x => x.frame != Time.frameCount);
            else pooledObj = pooledRenderGameObjects.FirstOrDefault();
            if (pooledObj == null)
            {
                pooledRenderGameObjects.RemoveWhere(x => x == null);
                var obj = new GameObject();
                meshFilter = obj.AddComponent<MeshFilter>();
                meshRenderer = obj.AddComponent<MeshRenderer>();
                return obj;
            }

            pooledRenderGameObjects.Remove(pooledObj);
            pooledObj.obj.SetActive(true);
            meshFilter = pooledObj.meshFilter;
            meshRenderer = pooledObj.meshRenderer;
            return pooledObj.obj;
        }

        public void ReleaseGameObject(GameObject obj)
        {
            if (!obj.TryGetComponent<IPoolableConstruction>(out var poolableConstruction))
            {
                ObjectUtility.DestroyObjectDirect(obj);
                return;   
            }
            
            if (obj.TryGetComponent<SceneObjectBase>(out var sceneObjectBase))
            {
                var spawnedObjects = sceneObjectBase.spawnedConstructionObjects;
                for (var i = 0; i < spawnedObjects.Count; i++) ReleaseSpawn(spawnedObjects[i]);
            }

            if (!poolEnabled || poolSizeObjects <= 0)
            {
                ObjectUtility.DestroyObjectDirect(obj);
                return;
            }

            for (var i = obj.transform.childCount - 1; i >= 0; i--)
            {
                var child = obj.transform.GetChild(i);
                child.SetParent(null);
                ReleaseGameObject(child.gameObject);
            }

            var components = obj.GetComponents<Component>();
            var isRenderGameObject = components.Any(c => c is MeshFilter) && components.Any(c => c is MeshRenderer);

            if (!isRenderGameObject && pooledGameObjects.Count > poolSizeObjects)
            {
                ObjectUtility.DestroyObjectDirect(obj);
                return;
            }

            if (isRenderGameObject && pooledRenderGameObjects.Count > poolSizeObjects)
            {
                ObjectUtility.DestroyObjectDirect(obj);
                return;
            }

            var pooledObj = new PooledGameObject {obj = obj, frame = Time.frameCount};

            foreach (var component in components)
            {
                if (component is Transform) continue;

                if (isRenderGameObject && component is MeshRenderer meshRenderer)
                {
                    meshRenderer.sharedMaterials = Array.Empty<Material>();
                    pooledObj.meshRenderer = meshRenderer;
                    continue;
                }

                if (isRenderGameObject && component is MeshFilter meshFilter)
                {
                    ReleaseMesh(meshFilter.sharedMesh);
                    meshFilter.sharedMesh = null;
                    pooledObj.meshFilter = meshFilter;
                    continue;
                }

                ObjectUtility.DestroyObjectDirect(component);
            }

            obj.transform.SetParent(objectParent);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(false);
            if (isRenderGameObject) pooledRenderGameObjects.Add(pooledObj);
            else pooledGameObjects.Add(pooledObj);
        }

        /********************************************************************************************************************************/

        public Mesh GetMesh()
        {
            if (!poolEnabled || poolSizeMeshes <= 0 || pooledMeshes.Count == 0) return new Mesh();

            var mesh = pooledMeshes.FirstOrDefault();
            if (!mesh)
            {
                pooledMeshes.RemoveWhere(x => !x);
                return new Mesh();
            }

            pooledMeshes.Remove(mesh);
            return mesh;
        }

        public void ReleaseMesh(Mesh mesh)
        {
            if (!poolEnabled || poolSizeMeshes <= 0 || pooledMeshes.Count > poolSizeMeshes)
            {
                ObjectUtility.DestroyObjectDirect(mesh);
                return;
            }

            if (!mesh.isReadable) return;

            mesh.Clear();
            pooledMeshes.Add(mesh);
        }

        /********************************************************************************************************************************/

        public GameObject GetSpawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!poolEnabled || poolSizeSpawns <= 0) return InstantiateSpawn(prefab, position, rotation);
            
            var prefabInstanceID = prefab.GetInstanceID();
            if (!pooledSpawns.TryGetValue(prefabInstanceID, out var objList)) return InstantiateSpawn(prefab, position, rotation);
            if (objList.Count == 0) return InstantiateSpawn(prefab, position, rotation);
            var obj = objList.FirstOrDefault();
            if (!obj)
            {
                objList.RemoveWhere(x => !x);
                return InstantiateSpawn(prefab, position, rotation);
            }

            objList.Remove(obj);
            obj.SetActive(true);
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        private GameObject InstantiateSpawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedObj = default; // Needed for build!
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                spawnedObj = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
                spawnedObj.transform.SetPositionAndRotation(position, rotation);
#endif
            }
            else
            {
                spawnedObj = Instantiate(prefab, position, rotation);
            }

            return spawnedObj;
        }

        public void ReleaseSpawn(SpawnedConstructionObject spawnedConstructionObject)
        {
            if (!spawnedConstructionObject) return;
            var prefabInstanceID = spawnedConstructionObject.prefabInstanceID;
            var obj = spawnedConstructionObject.gameObject;

            obj.transform.SetParent(null);
            
            if (!poolEnabled || poolSizeSpawns <= 0 || prefabInstanceID == 0)
            {
                ObjectUtility.DestroyObjectDirect(obj);
                return;
            }

            if (pooledSpawns.TryGetValue(prefabInstanceID, out var objList))
            {
                if (objList.Count > poolSizeSpawns)
                {
                    ObjectUtility.DestroyObjectDirect(obj);
                    return;
                }

                pooledSpawns[prefabInstanceID].Add(obj);
            }
            else
            {
                pooledSpawns.Add(prefabInstanceID, new HashSet<GameObject> {obj});
            }

            obj.transform.SetParent(spawnParent);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(false);
        }
    }
}