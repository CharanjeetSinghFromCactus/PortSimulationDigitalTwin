using PortSimulation.UI;
using UISystem;

namespace PortSimulation.POI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class VehiclePoi : PointOfInterest
	{
		
		#region PRIVATE_VARS
		
		[SerializeField] private Popup poiPopup;
		[SerializeField] private VehicleData vehicleData;
		#endregion

		#region PUBLIC_VARS

		#endregion

		#region UNITY_CALLBACKS
		
		#endregion

		#region PUBLIC_METHODS
		
		public void Init(VehicleData data)
		{
			vehicleData = data;
		}
		public override void OnClick()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Register(vehicleData);
			ServiceLocatorFramework.ServiceLocator.Current.Get<POIReference>().UpdatePOI(this);
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				ServiceLocatorFramework.ServiceLocator.Current.Get<POIManager>().HideOtherPOIs(this);
				ViewController.Instance.ChangeScreen(ScreenName.VehiclePOIDataScreen);
				poiPopup.Hide();
				OnFocus();
			});
		}

		public override void OnFocus()
		{
			
		}

		public override void OnHide()
		{
			poiPopup.Show();
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		
	}
}