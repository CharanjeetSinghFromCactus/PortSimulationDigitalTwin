namespace PortSimulation.PlacementSystem
{
	using UnityEngine;
	using System;
	using System.Collections;

	[Serializable]
	public class PlacementItemData
	{
		public PlacementItemData(GameObject objectToPlace, string placeableName, Sprite placeableIcon)
		{
			ObjectToPlace = objectToPlace;
			PlaceableName = placeableName;
			PlaceableIcon = placeableIcon;
		}

		[field:SerializeField] public int ItemId { get; private set; }
		[field:SerializeField] public int CategoryId { get; private set; }
		[field:SerializeField] public GameObject ObjectToPlace { get; private set; }
		[field:SerializeField] public string PlaceableName { get; private set; }
		[field:SerializeField] public Sprite PlaceableIcon { get; private set; }
		
	}
}