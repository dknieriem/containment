using UnityEngine;
using System;
using System.Collections;

public class WorldInfo : MonoBehaviour
{

		public GameObject SectorPrefab;
		public Sector[,] WorldSectors;
		public int[] Dimensions;
		public int NumGroups = 0;
		public DateTime CurrentDate;
		// Use this for initialization
				

	 
		void Start ()
		{
				Debug.Log ("Starting: WorldInfo");
								
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		void FixedUpdate ()
		{
		
		}

		public void DoNextUpdate ()
		{
				CurrentDate = CurrentDate.AddHours (1);
				Debug.Log (CurrentDate);
		}

		public Sector GetSectorAtPosition (Vector3 origin)
		{
				if (origin.x < 0 || origin.y < 0 || origin.x > Dimensions [0] || origin.y > Dimensions [1]) {
						throw new System.ArgumentOutOfRangeException ("Vector3 origin", "Not in valid range: x = [0," + Dimensions [0] + "], y = [0," + Dimensions [1] + "]");
				}
				
				int x = Mathf.FloorToInt (origin.x);
				int y = Mathf.FloorToInt (origin.y);
				
				return WorldSectors [x, y];	
		}
		
		public Sector GetSectorFromScreenPos (Vector3 screenPos)
		{
				Ray ray = Camera.main.ScreenPointToRay (screenPos);
				if (ray.origin.x > 0 && ray.origin.y > 0 && ray.origin.x < Dimensions [0] && ray.origin.y < Dimensions [1]) {			
						return GetSectorAtPosition (ray.origin);
				} 
				
				return null;
		}
}
