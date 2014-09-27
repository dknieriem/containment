using UnityEngine;
using System.Collections;

public class PlayerGroup : MonoBehaviour
{
		WorldInfo World;

		public static int MaxSquadsAllowed = 10;
		public static int MaxMembersAllowed = 100;
		public static int MaxMembersPerSquadAllowed = 16;

		public Sector HomeSector;
		public int[] HomeSectorLocation;
		
		public int TotalGroupMembers;
		public int[,] SectorGroupMembers;
		
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: PlayerGroup");
				World = GameObject.Find ("World").GetComponent<WorldInfo> ();
				Debug.Log (World.ToString ());
				
				SectorGroupMembers = new int[World.Dimensions [0], World.Dimensions [1]];
				Debug.Log ("GroupMember Size: " + SectorGroupMembers.Length);
				
				HomeSectorLocation = new int[2];
				
				StartGroup (Random.Range (5, 10), Random.Range (0, 10), Random.Range (0, 10));
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		void FixedUpdate ()
		{
				TotalGroupMembers = 0;
	
				for (int x = 0; x < World.Dimensions[0]; x++) {
						for (int y = 0; y < World.Dimensions[1]; y++) {
								SectorGroupMembers [x, y] = World.WorldSectors [x, y].PlayerGroupCount;
								TotalGroupMembers += SectorGroupMembers [x, y];
						}
				}
		}
		
		public void StartGroup (int numMembers, Sector homeSector)
		{
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [(int)HomeSectorLocation [0], (int)HomeSectorLocation [1]] = numMembers;
				TotalGroupMembers = numMembers;
		}
		
		public void StartGroup (int numMembers, int homeSectorX, int homeSectorY)
		{
		
				Debug.Log ("Adding " + numMembers + " to (" + homeSectorX + ", " + homeSectorY + ").");
				Sector homeSector = World.GetSectorAtPosition (new Vector3 ((float)homeSectorX, (float)homeSectorY));
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [homeSectorX, homeSectorY] = numMembers;
				TotalGroupMembers = numMembers;
		}
		
		public void SetHomeSector (Sector newLocation)
		{
				HomeSector = newLocation;
				HomeSectorLocation [0] = HomeSector.LocationX;
				HomeSectorLocation [1] = HomeSector.LocationY;
		}
}
