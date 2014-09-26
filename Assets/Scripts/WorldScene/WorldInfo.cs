using UnityEngine;
using System;
using System.Collections;

public class WorldInfo : MonoBehaviour
{

		public GameObject SectorPrefab;
		public Sector[,] WorldSectors;
		public int[] Dimensions;
		public static int NumGroups;
		public DateTime CurrentDate;
		// Use this for initialization
		
		public static float SecondsPerHour = 10.0f;
		
		public static float NextHourCountdown;
		void Start ()
		{
				Dimensions = new int[2];
				Dimensions [0] = 64;
				Dimensions [1] = 64;
				InstantiateSectors ();
				
				NumGroups = 4;
				
				CurrentDate = new DateTime (1999, 10, 5, 14, 0, 0);
				
				NextHourCountdown = SecondsPerHour;
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		void FixedUpdate ()
		{
				NextHourCountdown -= Time.fixedDeltaTime;
				if (NextHourCountdown < 0) {
						DoNextUpdate ();
						NextHourCountdown = SecondsPerHour;		
				}
		}
	
		void InstantiateSectors ()
		{
				WorldSectors = new Sector[Dimensions [0], Dimensions [1]];
				for (int i = 0; i < Dimensions[0]; i++) {
						for (int j = 0; j < Dimensions[1]; j++) {
								GameObject Sector = Instantiate (SectorPrefab) as GameObject;
								Sector.transform.parent = transform;
								Sector.transform.position = new Vector3 (i, j, 2);
								WorldSectors [i, j] = Sector.GetComponent<Sector> ();
								WorldSectors [i, j].LocationX = i;
								WorldSectors [i, j].LocationY = j;
								Sector.name = "Sector [" + i + ", " + j + "]";
						}
				}
		}

		void DoNextUpdate ()
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
