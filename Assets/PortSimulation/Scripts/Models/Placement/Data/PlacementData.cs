namespace PortSimulation.PlacementSystem
{
	using UnityEngine;
	using System;
	using System.Collections;

	[Serializable]
	public class PlacementData
	{
		public PlacementData(GameObject objectToPlace, string placeableName, Sprite placeableIcon)
		{
			ObjectToPlace = objectToPlace;
			PlaceableName = placeableName;
			PlaceableIcon = placeableIcon;
		}

		[field:SerializeField] public GameObject ObjectToPlace { get; private set; }
		[field:SerializeField] public string PlaceableName { get; private set; }
		[field:SerializeField] public Sprite PlaceableIcon { get; private set; }
		
	}
}