using PortSimulation.UI;
using UISystem;

namespace PortSimulation.POI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class ShipPoi : PointOfInterest
	{
		
		#region PRIVATE_VARS
		
		[SerializeField] private Popup poiPopup;
		[SerializeField] private MeshRenderer shipPath;
		private ShipDetails _details;
		#endregion

		#region PUBLIC_VARS

		#endregion

		#region UNITY_CALLBACKS
		
		#endregion

		#region PUBLIC_METHODS

		public void Init(ShipDetails data)
		{
			this._details = data;
		}
		public override void OnClick()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Register(_details);
			ServiceLocatorFramework.ServiceLocator.Current.Get<POIReference>().UpdatePOI(this);
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				ServiceLocatorFramework.ServiceLocator.Current.Get<POIManager>().HideOtherPOIs(this);
				ViewController.Instance.ChangeScreen(ScreenName.ShipPOIDataScreen);
				shipPath.enabled = true;
				poiPopup.Hide();
				OnFocus();
			});

		}

		public override void OnFocus()
		{
			
		}

		public override void OnHide()
		{
			// poiCanvasPopup.Hide();
			shipPath.enabled = false;
			poiPopup.Show();
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		
	}
}