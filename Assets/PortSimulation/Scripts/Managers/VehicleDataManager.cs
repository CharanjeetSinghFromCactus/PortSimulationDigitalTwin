using PortSimulation.POI;
using UnityEngine.Networking;

namespace PortSimulation
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class VehicleDataManager:MonoBehaviour
	{

		#region PRIVATE_VARS
		[SerializeField] private VehiclePoi[] vehicles;
		#endregion

		#region PUBLIC_VARS
		
		#endregion

		#region UNITY_CALLBACKS

		private void Start()
		{
			FetchVehicleDataFromGoogleSheets();
		}

		#endregion

		#region PUBLIC_METHODS
		void FetchVehicleDataFromGoogleSheets()
		{
			StartCoroutine(ReadSheet());
		}
		#endregion

		#region PRIVATE_METHODS
		IEnumerator ReadSheet()
		{
			string url = "https://docs.google.com/spreadsheets/d/1EEJHDwagewENrII2to9nmIvs8VgLvHH6qzk7ekCALSo/gviz/tq?tqx=out:csv&sheet=VehicleData\n";
			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				yield return www.SendWebRequest();
				if (www.result == UnityWebRequest.Result.Success)
				{
					string[] rows = www.downloadHandler.text.Split('\n');
					for (int i = 1; i < rows.Length; i++)
					{
						string[] cells = rows[i].Split(',');
						Debug.Log($"{cells[0]} | {cells[1]} | {cells[2]} | {cells[3]} ");
						if(i-1 < vehicles.Length)
						{
							string vehicleName = cells[0].Trim('"');
							Debug.Log("Vehicle Name: " + vehicleName);
							VehicleType vehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), cells[1].Trim('"'));
							Debug.Log("Vehicle Type: " + vehicleType);
							float speed = float.Parse(cells[2].Trim('"'));
							Debug.Log("Vehicle Speed: " + speed);
							float loadCapacity = float.Parse(cells[3].Trim('"'));
							Debug.Log("Vehicle Load Capacity: " + loadCapacity);
							VehicleData data = new VehicleData
							{
								vehicleName = vehicleName,
								vehicleType = vehicleType,
								speed = speed,
								loadCapacity = loadCapacity
							};
							vehicles[i-1].Init(data);
						}
					}
				}
			}
		}
		#endregion

	}
}