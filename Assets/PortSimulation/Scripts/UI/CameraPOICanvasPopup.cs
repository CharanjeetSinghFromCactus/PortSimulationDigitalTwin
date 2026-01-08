using TMPro;

namespace PortSimulation.UI
{
	using UnityEngine;
	using System;
	using System.Collections;
	
	public class CameraPOICanvasPopup : UISystem.Popup
	{

		public UISystem.Popup poiIconPopup;
		
		public override void Show()
		{
			base.Show();
		}

		public override void Hide()
		{
			base.Hide();
		}
		
		public void OnCloseButtonClick()
		{
			Hide();
			poiIconPopup.Show();
		}
	}
	
}