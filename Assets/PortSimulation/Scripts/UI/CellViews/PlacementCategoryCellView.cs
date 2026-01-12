using DataBindingFramework;
using EnhancedUI.EnhancedScroller;
using PortSimulation.PlacementSystem;
using TMPro;
using UISystem;
using Unity.Properties;
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
		private IPropertyManager _propertyManager;
		private Property<int> SetCurrentCategoryName;
		private void OnEnable()
		{
			_propertyManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IPropertyManager>();
			SetCurrentCategoryName = _propertyManager.GetOrCreateProperty<int>(PropertyNameConstants.PlacementCategoryNameProperty);
		}

		private void OnDisable()
		{
			_propertyManager.RemoveProperty<string>(PropertyNameConstants.PlacementCategoryNameProperty);
		}

		public void SetData(PlacementCategory category)
		{
			this.category = category;
			categoryIcon.sprite = category.CategoryIcon;
			categoryNameText.text = category.CategoryName;
		}

		public void OnClick()
		{
			Debug.Log($"CategoryName in Cell View {category.CategoryName}");
			SetCurrentCategoryName.Value = category.CategoryId;
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
			});
		}
	}
}