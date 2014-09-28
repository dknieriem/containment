using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroup : MonoBehaviour
{
		WorldInfo World;

		public static int MaxSquadsAllowed = 10;
		public static int MaxMembersAllowed = 100;
		public static int MaxMembersPerSquadAllowed = 16;

		public Sector HomeSector;
		public int[] HomeSectorLocation;
		
		public int TotalGroupMembers;
		public ArrayList GroupMemberLocations = new ArrayList ();
		public int[,] SectorGroupMembers;
		
		public ArrayList GroupMemberNames = new ArrayList ();
		
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: PlayerGroup");
				World = GameObject.Find ("World").GetComponent<WorldInfo> ();
				//Debug.Log (World.ToString ());
				SectorGroupMembers = new int[World.Dimensions [0], World.Dimensions [1]];
				//Debug.Log ("GroupMember Size: " + SectorGroupMembers.Length);
				HomeSectorLocation = new int[2];			
				StartGroup (Random.Range (5, 10), Random.Range (0, 10), Random.Range (0, 10));
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		void FixedUpdate ()
		{
				SectorGroupMembers = new int[World.Dimensions [0], World.Dimensions [1]];
		
				for (int i = 0; i < TotalGroupMembers; i++) {
						int[] sectorToAdd = (int[])GroupMemberLocations [i];
						SectorGroupMembers [sectorToAdd [0], sectorToAdd [1]] ++;
				}
		}
		
		public void StartGroup (int numMembers, Sector homeSector)
		{
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [(int)HomeSectorLocation [0], (int)HomeSectorLocation [1]] = numMembers;
				TotalGroupMembers = numMembers;
				
				for (int i = 0; i < numMembers; i++) {
						GroupMemberLocations.Add (HomeSectorLocation);				
						GroupMemberNames.Add ("Steve " + i);
				}
		}
		
		public void StartGroup (int numMembers, int homeSectorX, int homeSectorY)
		{
		
				Debug.Log ("Adding " + numMembers + " to (" + homeSectorX + ", " + homeSectorY + ").");
				Sector homeSector = World.WorldSectors [homeSectorX, homeSectorY];
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [homeSectorX, homeSectorY] = numMembers;
				TotalGroupMembers = numMembers;
				
				for (int i = 0; i < numMembers; i++) {
						GroupMemberLocations.Add (HomeSectorLocation);				
						GroupMemberNames.Add ("Steve " + i);
				}
		}
		
		public void SetHomeSector (Sector newLocation)
		{
				HomeSector = newLocation;
				HomeSectorLocation [0] = HomeSector.LocationX;
				HomeSectorLocation [1] = HomeSector.LocationY;
		}
}
