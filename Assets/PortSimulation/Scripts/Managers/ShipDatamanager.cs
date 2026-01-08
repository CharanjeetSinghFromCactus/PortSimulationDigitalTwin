using LitJson;
using PortSimulation.POI;
using PortSimulation.UI;
using SimpleJSON;
using UnityEngine.Networking;

namespace PortSimulation
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class ShipDatamanager:MonoBehaviour
	{

		#region PRIVATE_VARS
		[SerializeField] private ShipPoi[] ships;
		[SerializeField] TextAsset data;
		[SerializeField] private ShipDataCollection shipDataCollection;
		#endregion

		#region PUBLIC_VARS
		
		#endregion

		#region UNITY_CALLBACKS
		private void OnEnable()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Register(this);
		}

		private void Start()
		{
			FetchData();
			InitializeShipPOIs();
		}
		
		
		private void OnDisable()
		{
			ServiceLocatorFramework.ServiceLocator.Current.Unregister<ShipDatamanager>();
		}

		#endregion

		#region PUBLIC_METHODS
		private void InitializeShipPOIs()
		{
			for (int i = 0; i < ships.Length; i++)
			{
				ShipDetails shipDetails = GetRandomShipDetails();
				ships[i].Init(shipDetails);
			}
		}
		void FetchData()
		{
			string jsonString = data.text;
			var jsonData = JSON.Parse(jsonString) as JSONArray;
			for (int i = 0; i < jsonData.Count; i++)
			{
				ShipDetails shipDetails = JsonUtility.FromJson<ShipDetails>(jsonData[i].ToString());
				shipDataCollection.Ships.Add(shipDetails);
			}
		}

		public ShipDetails GetRandomShipDetails()
		{
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			int randomIndex = UnityEngine.Random.Range(0, shipDataCollection.Ships.Count);		
			return shipDataCollection.Ships[randomIndex];
		}
		#endregion

		#region PRIVATE_METHODS
		
		#endregion

	}
}