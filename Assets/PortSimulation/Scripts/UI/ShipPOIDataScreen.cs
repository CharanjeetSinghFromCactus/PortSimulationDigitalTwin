using TMPro;
using UnityEngine;
using System;
using PortSimulation.POI;

namespace PortSimulation.UI
{
	public class ShipPOIDataScreen : UISystem.Screen
	{
		// [Header("References")]
		// public UISystem.Popup poiIconPopup;

		// UI Fields (TMP)
		[Header("Route Info")]
		public TMP_Text fromToText;
		public TMP_Text etaText;
		public TMP_Text berthText;
		public TMP_Text etsText;
		public TMP_Text agentArrivalText;

		[Header("Vessel Details")]
		public TMP_Text vesselNameText;
		public TMP_Text vesselTypeText;
		public TMP_Text vesselLengthText;
		public TMP_Text vesselWidthText;
		public TMP_Text nationalityText;
		public TMP_Text grossText;
		public TMP_Text netText;
		public TMP_Text maxDraughtText;
		public TMP_Text deadWeightText;

		[SerializeField] private ShipDetails currentShipData;
		
		public override void Show()
		{
			currentShipData = ServiceLocatorFramework.ServiceLocator.Current.Get<ShipDetails>();
			SetShipData(currentShipData);
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				ServiceLocatorFramework.ServiceLocator.Current.Unregister<ShipDetails>();
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
		
		public void SetShipData(ShipDetails ship)
		{
			if (ship == null)
			{
				Debug.LogWarning("⚠️ Ship data is null — cannot populate popup.");
				return;
			}
			
			// Route Info
			fromToText.text = ship.From_To;
			etaText.text = ship.ETA;
			berthText.text = ship.BERTH;
			etsText.text = ship.ETS;
			agentArrivalText.text = $"{ship.AgentOfArrival}/n{ship.AgentOfDeparture}";

			// Vessel Info
			vesselNameText.text = ship.DetailsOfVessel.Name;
			vesselTypeText.text = ship.DetailsOfVessel.Type;
			vesselLengthText.text = $"{ship.DetailsOfVessel.Length_m:0.0} m";
			vesselWidthText.text = $"{ship.DetailsOfVessel.Width_m:0.0} m";
			nationalityText.text = ship.DetailsOfVessel.Nationality;
			grossText.text = ship.DetailsOfVessel.Gross_t.ToString();
			netText.text = ship.DetailsOfVessel.Net_t.ToString();
			maxDraughtText.text = $"{ship.DetailsOfVessel.MaxDraught_m:0.0} m";
			deadWeightText.text = ship.DetailsOfVessel.DeadWt_t.ToString();
		}
	}
}
