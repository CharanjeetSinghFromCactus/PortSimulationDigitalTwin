using PortSimulation.UI;
using UISystem;

namespace PortSimulation.POI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class CCTVCameraPOI : PointOfInterest
	{
		
		#region PRIVATE_VARS

		[SerializeField] private Popup poiPopup;
		[SerializeField] private string rtspUrl;
		private CCTVData cctvData;
		#endregion

		#region PUBLIC_VARS

		#endregion

		#region UNITY_CALLBACKS

		public override void Start()
		{
			base.Start();
			cctvData = new CCTVData { rtspUrl = this.rtspUrl };
		}

		#endregion

		#region PUBLIC_METHODS
		public override void OnClick()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Register(cctvData);
			ServiceLocatorFramework.ServiceLocator.Current.Get<POIReference>().UpdatePOI(this);
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				ServiceLocatorFramework.ServiceLocator.Current.Get<POIManager>().HideOtherPOIs(this);
				ViewController.Instance.ChangeScreen(ScreenName.RTSPPlayerScreen);
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