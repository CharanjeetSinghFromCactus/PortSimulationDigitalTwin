
namespace PortSimulation.PlacementSystem
{
	using UnityEngine;
	using System.Collections.Generic;
	using System;
	using System.Collections;

	[CreateAssetMenu(fileName = "PlacementDataContainer" , menuName = "PortSimulation/Containers/PlacementDataContainer")]
	public class PlacementDataContainer:ScriptableObject
	{
		[field:SerializeField] public PlacementCategory[] PlacementCategories { get; private set; }

		public PlacementCategory GetCategory(int id)
		{
			return Array.Find(PlacementCategories, x => x.CategoryId == id);
		}
	}
}