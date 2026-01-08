using TMPro;
using PortSimulation.POI;

namespace PortSimulation.UI
{
	using UnityEngine;
	using System.Collections;
	
	public class VehiclePOIDataScreen : UISystem.Screen
	{
		public TMP_Text vehicleName;
		public TMP_Text vehicleType;
		public TMP_Text speed;
		public TMP_Text loadCapacity;
		
		private VehicleData data;

		public override void Show()
		{
			data = ServiceLocatorFramework.ServiceLocator.Current.Get<VehicleData>();
			SetVehicleData(data);
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				ServiceLocatorFramework.ServiceLocator.Current.Unregister<VehicleData>();
			});
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
			UISystem.ViewController.Instance.ChangeScreen(UISystem.ScreenName.PlacementCategoryScreen);
		}

		public void SetVehicleData(VehicleData data)
		{
			if (data == null)
			{
				Debug.LogWarning("⚠️ Vehicle data is null — cannot populate popup.");
				return;
			}
			vehicleName.text = data.vehicleName;
			vehicleType.text = data.vehicleType.ToString();
			speed.text = data.speed.ToString("F2") + " km/h";
			loadCapacity.text = data.loadCapacity.ToString("F2") + " tons";
		}
	}
}