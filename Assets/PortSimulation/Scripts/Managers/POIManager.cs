using System.Collections.Generic;

namespace PortSimulation
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class POIManager : MonoBehaviour
	{

		#region PRIVATE_VARS
		[SerializeField] private List<PointOfInterest> pois = new List<PointOfInterest>();
		PointOfInterest currentPOI;
		float timeSinceLastClick = 0f;
		#endregion

		#region PUBLIC_VARS

		#endregion

		#region UNITY_CALLBACKS

		private void OnEnable()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Register(this);
		}

		private void OnDisable()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Unregister<POIManager>();
		}

		#endregion

		#region PUBLIC_METHODS
		
		public void Register(PointOfInterest poi)
		{
			if(!pois.Contains(poi))
			{
				pois.Add(poi);
			}
		}
		
		public void Unregister(PointOfInterest poi)
		{
			if(pois.Contains(poi))
			{
				pois.Remove(poi);
			}
		}
		public void HideOtherPOIs(PointOfInterest poi)
		{
			currentPOI = poi;
			foreach(var p in pois)
			{
				if(p != poi)
				{
					p.OnHide();
				}
			}
		}

		public void MouseDown()
		{
			timeSinceLastClick = Time.time;
		}

		public void MouseUp()
		{
			if(Mathf.Abs(timeSinceLastClick - Time.time) < 0.1f)
			{
				currentPOI.OnHide();
			}
		}
		#endregion

		#region PRIVATE_METHODS
		
		#endregion

	}
}