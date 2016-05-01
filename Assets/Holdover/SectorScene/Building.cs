using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{

		public WallSegment[] BuildingWallSegments;
		public int[] BuildingWallSegmentsXPos;
		public int[] BuildingWallSegmentsYPos;
		public int[,] BuildingPlan;
		
		public int[] BuildingPosition;
		
		public bool BuildingIsSecure = true;

		// Use this for initialization
		void Start ()
		{
				BuildingPosition = new int[2];
				BuildingPosition [0] = Mathf.FloorToInt (transform.position.x);
				BuildingPosition [1] = Mathf.FloorToInt (transform.position.y);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		public void WallSegmentDestroyed (WallSegment wallSegmentDestroyed)
		{
				BuildingIsSecure = false;
		
				for (int i = 0; i < BuildingWallSegments.Length; i++) {
		
						if (BuildingWallSegments [i] == wallSegmentDestroyed) {
								BuildingWallSegments [i] = null;
						}
		
				}
		}
}
