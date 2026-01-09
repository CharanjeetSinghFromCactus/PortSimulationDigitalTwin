using System;
using DataBindingFramework;
using UISystem;

namespace PortSimulation.UI
{
	using UnityEngine;
	using System.Collections;

	public class PlacementPropertiesScreen:UISystem.Screen
	{
		private IObserverManager _observerManager;
		private DataBindingFramework.IObserver<bool> canPlacePlaceableObject;
		private DataBindingFramework.IObserver<string> sendSelectedToolName;
		private void OnEnable()
		{
			_observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
			canPlacePlaceableObject = _observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.CanPlacePlaceableObject);
			sendSelectedToolName = _observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SendSelectedToolName);
		}

		private void OnDisable()
		{
			_observerManager.RemoveObserver(ObserverNameConstants.CanPlacePlaceableObject);
			_observerManager.RemoveObserver(ObserverNameConstants.SendSelectedToolName);
		}

		public override void Show()
		{
			base.Show();
		}

		public override void Hide()
		{
			base.Hide();
		}

		public void OnPanToolButtonClick()
		{
			sendSelectedToolName.Notify(ToolNameConstants.PlacementPanToolName);
		}
		
		public void OnRotateToolButtonClick()
		{
			sendSelectedToolName.Notify(ToolNameConstants.PlacementRotateToolName);
		}
		
		public void OnScaleToolButtonClick()
		{
			sendSelectedToolName.Notify(ToolNameConstants.PlacementScaleToolName);
		}


		public void FinalizePlacementClick()
		{
			canPlacePlaceableObject.Notify(true);
		}

		public void CancelPlacementClick()
		{
			canPlacePlaceableObject.Notify(false);
		}

		public void OnBackButtonClick()
		{
			UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementObjectSelectionScreen);
		}
	}
}