using EnhancedUI.EnhancedScroller;
using PortSimulation.PlacementSystem;
using TMPro;
using UnityEngine.UI;

namespace PortSimulation.UI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class PlacementCategoryCellView : EnhancedScrollerCellView
	{
		private PlacementCategory category;
		[SerializeField] private Image categoryIcon;
		[SerializeField] private TMP_Text categoryNameText;
		
		public void SetData(PlacementCategory category)
		{
			this.category = category;
			categoryIcon.sprite = category.CategoryIcon;
			categoryNameText.text = category.CategoryName;
		}

		public void OnClick()
		{
			Debug.Log($"CategoryName {category.CategoryName}");
		}
	}
}