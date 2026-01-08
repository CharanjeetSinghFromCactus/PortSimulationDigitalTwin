using PortSimulation.UI;
using UISystem;

namespace PortSimulation.POI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class CameraPOI : PointOfInterest
	{
		
		#region PRIVATE_VARS
		
		[SerializeField] private Popup poiCanvasPopup;
		[SerializeField] private Popup poiPopup;
		
		[SerializeField] private GameObject gizmos;
		#endregion

		#region PUBLIC_VARS

		#endregion

		#region UNITY_CALLBACKS
		
		#endregion

		#region PUBLIC_METHODS
		public override void OnClick()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Get<POIManager>().HideOtherPOIs(this);
			gizmos.SetActive(true);
			poiCanvasPopup.Show();
			poiPopup.Hide();
			OnFocus();
		}

		public override void OnFocus()
		{
			
		}

		public override void OnHide()
		{
			poiCanvasPopup.Hide();
			gizmos.SetActive(false);
			poiPopup.Show();
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		
	}
}