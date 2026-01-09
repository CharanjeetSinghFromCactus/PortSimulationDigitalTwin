namespace PortSimulation.UI
{
	using DataBindingFramework;
	using EnhancedUI.EnhancedScroller;
	using PlacementSystem;
	using TMPro;
	using UnityEngine.UI;
	using UnityEngine;

	public class PlacementItemCellView : EnhancedScrollerCellView
	{
		private PlacementItemData _placementItemItemData;
		[SerializeField] private Image categoryIcon;
		[SerializeField] private TMP_Text categoryNameText;
		private IObserverManager observerManager;
		IObserver<PlacementItemData> onPlacementItemClick;
		private void OnEnable()
		{
			observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
			onPlacementItemClick = observerManager.GetOrCreateObserver<PlacementItemData>(ObserverNameConstants.OnPlacementItemClick);
		}

		private void OnDisable()
		{
			observerManager.RemoveObserver(ObserverNameConstants.OnPlacementItemClick);
		}

		public void SetData(PlacementItemData item)
		{
			this._placementItemItemData = item;
			categoryIcon.sprite = item.PlaceableIcon;
			categoryNameText.text = item.PlaceableName;
		}

		public void OnClick()
		{
			if (_placementItemItemData != null)
			{
				onPlacementItemClick.Notify(_placementItemItemData);
			}
		}
	}
}