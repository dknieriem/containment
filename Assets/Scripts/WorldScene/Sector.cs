using UnityEngine;
using System.Collections;

public class Sector : MonoBehaviour
{

		public enum SectorType
		{
				Water,
				Grass,
				Forest,
				Residential,
				Commercial,
				Industrial,
				Seaport,
				Airport}
		;

		public static int North = 0,
				East = 1,
				South = 2,
				West = 3;
				
		public static string[] SectorRegionNames = {
				"North West",
				"North Central",
				"North East",
				"Central West",
				"Central",
				"Central East",
				"South West",
				"South Central",
				"South East"
		};
		
		GameWorld Game;
		WorldInfo World;
		Sector[] NeighboringSectors;

		public SectorType SecType;
		public int LocationX, LocationY;
		public int ZedCount;
		public int[] GroupCount;
		public int PlayerGroupCount;
		public float[] ZedProbabilityMigrate;
		public bool IsVisited = false;
		public bool IsVisible = false;
		GameObject[] myRegions;
		SpriteRenderer[] mySprites;
		SpriteRenderer mapMask;
		
		
		public static float SecondsPerUpdate = 10.0f;
		public static float NextUpdateCountdown;
		
		// Use this for initialization
		void Start ()
		{
				Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				World = GameObject.Find ("World").GetComponent<WorldInfo> ();
						
				GroupCount = new int[WorldInfo.NumGroups];
				for (int i = 0; i < WorldInfo.NumGroups; i++) {
						GroupCount [i] = 0;
				}
				
				myRegions = new GameObject[SectorRegionNames.Length];
				mySprites = new SpriteRenderer[SectorRegionNames.Length];
				
				
				/*for (int i = 0; i < SectorRegionNames.Length; i++) {
						myRegions [i] = new GameObject (SectorRegionNames [i] + " Sprite");
						myRegions [i].AddComponent<SpriteRenderer> ();
						myRegions [i].transform.parent = transform;
						myRegions [i].transform.localPosition = new Vector3 ((float)((i % 3) - 1) / 3, (float)((i - 1) / 3) / 3, -1.0f);
						
						mySprites [i] = myRegions [i].GetComponent<SpriteRenderer> ();
						mySprites [i].sprite = Game.Sprites.RandomSprite ();
						//mySprites [i].sortingLayerID = 2;
				}*/
				
				for (int x = 0; x < 3; x++) {
						for (int y = 0; y < 3; y++) {
				
								int i = x + y * 3;
				
								myRegions [i] = new GameObject (SectorRegionNames [i] + " Sprite");
								myRegions [i].AddComponent<SpriteRenderer> ();
								myRegions [i].transform.parent = transform;
								myRegions [i].transform.localPosition = new Vector3 ((float)(x - 1) / 3, (float)(y - 1) / 3, 9.0f);
								mySprites [i] = myRegions [i].GetComponent<SpriteRenderer> ();
								mySprites [i].sprite = Game.Sprites.RandomSprite ();
						}
				}
				
				mapMask = gameObject.GetComponent<SpriteRenderer> ();
								
				ZedCount = Random.Range (0, 100);
				ZedProbabilityMigrate = new float[4];			
				
				NeighboringSectors = new Sector[4];
				
				if (LocationX > 0) {
						NeighboringSectors [West] = World.WorldSectors [LocationX - 1, LocationY];
						ZedProbabilityMigrate [West] = 0.015f;
				}
				if (LocationX < World.Dimensions [0] - 1) {
						NeighboringSectors [East] = World.WorldSectors [LocationX + 1, LocationY];
						ZedProbabilityMigrate [East] = 0.015f;
				}
				if (LocationY > 0) {
						NeighboringSectors [North] = World.WorldSectors [LocationX, LocationY - 1];
						ZedProbabilityMigrate [North] = 0.015f;
				}
				if (LocationY < World.Dimensions [1] - 1) {
						NeighboringSectors [South] = World.WorldSectors [LocationX, LocationY + 1];
						ZedProbabilityMigrate [South] = 0.015f;
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
		
				if (PlayerGroupCount > 0) {
						IsVisible = true;
						IsVisited = true;
				}
		
				if (PlayerGroupCount <= 0) {
						IsVisible = false;
				}
				if (IsVisible || Game.IsDebug) {
						mapMask.enabled = false;
						//mapMask.color = new Color (1.0f, 1.0f, 1.0f);
				} else if (IsVisited) {
						mapMask.enabled = true;
						mapMask.color = new Color (0.7f, 0.7f, 1.0f);
				} else {
						mapMask.enabled = true;
						mapMask.color = new Color (0.0f, 0.0f, 0.0f);
		
				}
				if (Game.IsPlaying) {
						NextUpdateCountdown -= Time.fixedDeltaTime;
						if (NextUpdateCountdown < 0) {
								DoNextUpdate ();
								NextUpdateCountdown = SecondsPerUpdate;		
						}
				}
		}
		
		//updated each game hour
		void DoNextUpdate ()
		{
		
				int[] numMigrating = {0,0,0,0};
		
				for (int i = 0; i < ZedCount; i++) {
		
						float p = Random.Range (0.0f, 1.0f);
		
						if (p < ZedProbabilityMigrate [North]) {
								numMigrating [North]++;
						} else if ((p -= ZedProbabilityMigrate [North]) < ZedProbabilityMigrate [East]) {
								numMigrating [East]++;
						} else if ((p -= ZedProbabilityMigrate [East]) < ZedProbabilityMigrate [South]) {
								numMigrating [South]++;
						} else if ((p -= ZedProbabilityMigrate [South]) < ZedProbabilityMigrate [West]) {
								numMigrating [West]++;
						}
				}
								
				if (NeighboringSectors [North] != null) {
						NeighboringSectors [North].ZedCount += numMigrating [North];
						ZedCount = ZedCount - numMigrating [North];
				}
				if (NeighboringSectors [East] != null) {
						NeighboringSectors [East].ZedCount += numMigrating [East];
						ZedCount = ZedCount - numMigrating [East]; 
				}
				if (NeighboringSectors [South] != null) {
						NeighboringSectors [South].ZedCount += numMigrating [South];
						ZedCount = ZedCount - numMigrating [South]; 
				}
				if (NeighboringSectors [West] != null) {
						NeighboringSectors [West].ZedCount += numMigrating [West];
						ZedCount = ZedCount - numMigrating [West]; 
				}
				
		}
}