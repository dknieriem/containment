using UnityEngine;
using System.Collections;

public class Sector : MonoBehaviour
{

		public static enum CardinalDirections { North = 0, East, South, West }; 

		public int LocationX, LocationY;
		
		public int ZedCount;
		
		public float ZedProbabilityMigrate[];
		
		public int[] GroupCount;

		public bool IsVisited = false;
		
		public bool IsVisible = false;
		
		SpriteRenderer mySprite;
		
		public static float SecondsPerUpdate = 10.0f;
		
		public static float NextUpdateCountdown;
		
		// Use this for initialization
		void Start ()
		{
				GroupCount = new int[WorldInfo.NumGroups];
				for (int i = 0; i < WorldInfo.NumGroups; i++) {
						GroupCount [i] = 0;
				}
				
				mySprite = gameObject.GetComponent<SpriteRenderer> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
				if (IsVisible) {
						mySprite.color = new Color (1.0f, 1.0f, 1.0f);
				} else if (IsVisited) {
						mySprite.color = new Color (0.7f, 0.7f, 1.0f);
				} else {
						mySprite.color = new Color (0.0f, 0.0f, 0.0f);
		
				}
				
				NextUpdateCountdown -= Time.fixedDeltaTime;
				if (NextUpdateCountdown < 0) {
						DoNextUpdate ();
						NextUpdateCountdown = SecondsPerUpdate;		
				}
		}
		
		//updated each game hour
		void DoNextUpdate ()
		{
		
		
		
		}
}
