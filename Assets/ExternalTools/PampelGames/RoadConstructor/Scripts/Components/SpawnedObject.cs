// ----------------------------------------------------
// Road Constructor
// Copyright (c) Pampel Games e.K. All Rights Reserved.
// https://www.pampelgames.com
// ----------------------------------------------------

using PampelGames.Shared.Construction;
using PampelGames.Shared.Utility;

namespace PampelGames.RoadConstructor
{
    public class SpawnedObject : SpawnedConstructionObject
    {
        public SpawnObject spawnObject;
        public bool otherSide;

        public void Initialize(SpawnObject _spawnObject)
        {
            prefabInstanceID = _spawnObject.obj.GetInstanceID();
            spawnObject = PGClassUtility.CopyClass(_spawnObject) as SpawnObject;
        }
    }
}