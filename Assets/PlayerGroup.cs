using UnityEngine;
using System.Collections;

public class PlayerGroup : MonoBehaviour
{
		WorldInfo World;

		public Sector HomeSector;
		public Vector3 HomeSectorLocation;
		
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
								//	SectorGroupMembers [x, y] = World.WorldSectors [x, y].PlayerGroupCount;
								TotalGroupMembers += SectorGroupMembers [x, y];
						}
				}
		}
		
		public void SetHomeSector (Sector newLocation)
		{
				HomeSector = newLocation;
				HomeSectorLocation = HomeSector.transform.position;		
		}
}
