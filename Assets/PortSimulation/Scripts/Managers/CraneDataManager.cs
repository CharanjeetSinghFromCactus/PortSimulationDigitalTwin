using PortSimulation.POI;
using PortSimulation.UI;
using UnityEngine.Networking;

namespace PortSimulation
{
	using UnityEngine;
	using System;
	using System.Collections;

	public class CraneDataManager:MonoBehaviour
	{

		#region PRIVATE_VARS
		[SerializeField] private CranePOI[] cranes;
		#endregion

		#region PUBLIC_VARS
		
		#endregion

		#region UNITY_CALLBACKS

		private void Start()
		{
			FetchDataFromGoogleSheets();
		}

		#endregion

		#region PUBLIC_METHODS
		void FetchDataFromGoogleSheets()
		{
			StartCoroutine(ReadSheet());
		}
		#endregion

		#region PRIVATE_METHODS
		IEnumerator ReadSheet()
		{
			string url = "https://docs.google.com/spreadsheets/d/1EEJHDwagewENrII2to9nmIvs8VgLvHH6qzk7ekCALSo/gviz/tq?tqx=out:csv&sheet=CraneData\n";
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
						if(i-1 < cranes.Length)
						{
							string name = cells[0].Trim('"');
							CraneType type = (CraneType)int.Parse(cells[1].Trim('"'));
							string containerID = cells[2].Trim('"');
							int weight = int.Parse(cells[3].Trim('"'));
							
							CraneData data = new CraneData
							{
								craneName = name,
								craneType = type,
								ContainerID = containerID,
								containerWeight = weight
							};
							cranes[i-1].Init(data);
						}
					}
				}
			}
		}
		#endregion

	}
}