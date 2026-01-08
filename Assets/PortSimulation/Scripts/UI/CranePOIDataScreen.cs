using TMPro;
using PortSimulation.POI;
using UISystem;

namespace PortSimulation.UI
{
	using UnityEngine;
	using System.Collections;
	
	public class CranePOIDataScreen : UISystem.Screen
	{
		public TMP_Text vehicleName;
		public TMP_Text containerID;
		public TMP_Text loadCapacity;

		// public UISystem.Popup poiIconPopup;
		private CraneData data;

		public void Init(CraneData data)
		{
			this.data = data;
		}
		public override void Show()
		{
			data = ServiceLocatorFramework.ServiceLocator.Current.Get<CraneData>();
			vehicleName.text = data.craneName;
			containerID.text = data.ContainerID;
			loadCapacity.text = data.containerWeight.ToString("F2") + " tons";
			CoroutineExtension.ExecuteAfterFrame(this,
				()=>ServiceLocatorFramework.ServiceLocator.Current.Unregister<CraneData>());
			base.Show();
		}

		public override void Hide()
		{
			base.Hide();
		}
		
		public void OnCloseButtonClick()
		{
			var poi = ServiceLocatorFramework.ServiceLocator.Current.Get<POIReference>().CurrentPOI;
			if (poi != null)
			{
				poi.OnHide();
			}
			ViewController.Instance.ChangeScreen(ScreenName.PlacementCategoryScreen);
		}
	}
}