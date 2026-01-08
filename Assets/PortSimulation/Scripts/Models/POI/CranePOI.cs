using PortSimulation.UI;
using ServiceLocatorFramework;
using UISystem;

namespace PortSimulation.POI
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class CranePOI : PointOfInterest
	{
		
		#region PRIVATE_VARS
		
		[SerializeField] private Popup poiPopup;
		[SerializeField] private CraneData craneData;
		[SerializeField] private Animator craneAnimator;
		#endregion

		#region PUBLIC_VARS

		#endregion

		#region UNITY_CALLBACKS
		
		public void Init(CraneData data)
		{
			craneData = data;
		}

		public override void Start()
		{
			base.Start();
			UnityEngine.Random.InitState(UnityEngine.Random.Range(1000,5000));
			int craneID  = UnityEngine.Random.Range(1, 20);
			string craneName = $"Crane_{craneID}";
			craneData.craneName = craneName;
			int containerID = UnityEngine.Random.Range(100, 500);
			string containerName = $"Container_{containerID}";
			craneData.ContainerID = containerName;
			int weight = UnityEngine.Random.Range(1000, 30000);
			craneData.containerWeight = weight;
		}

		#endregion

		#region PUBLIC_METHODS
		public override void OnClick()
		{
			
			// if(poiCanvasPopup is CranePOICanvas cranePoi)
			// {
			// 	cranePoi.Init(craneData);
			// }
			ServiceLocator.Current.Register(craneData);
			ServiceLocatorFramework.ServiceLocator.Current.Get<POIReference>().UpdatePOI(this);
			if(craneAnimator != null)
			{
				craneAnimator.SetTrigger("OperateCrane");
			}
			CoroutineExtension.ExecuteAfterFrame(this, () =>
			{
				ServiceLocator.Current.Get<POIManager>().HideOtherPOIs(this);
				ViewController.Instance.ChangeScreen(ScreenName.CranePOIDataScreen);
				// poiCanvasPopup.Show();
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
			poiPopup.Show();
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		
	}
}