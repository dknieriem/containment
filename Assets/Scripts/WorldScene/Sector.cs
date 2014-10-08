using UnityEngine;
using System.Collections;

public class Sector : MonoBehaviour
{

		public enum SectorType
		{
				
				Grass,
				Forest,
				Residential,
				Commercial,
				Industrial,
				Seaport,
				Airport,
				Water}
		;

		public static int North = 0,
				East = 1,
				South = 2,
				West = 3,
				NorthEast = 4,
				SouthEast = 5,
				NorthWest = 6,
				SouthWest = 7;
				
		public static string[] SectorRegionNames = {
		"South West",
		"South Central",
		"South East",
		"Central West",
		"Central",
		"Central East",
				"North West",
				"North Central",
				"North East",
						};
		
		public SectorType SecType;
		public int LocationX, LocationY;
		public int ZedCount;
		public int[] GroupCount;
		public int PlayerGroupCount;
		public float[] ZedProbabilityMigrate;
		public bool IsVisited = false;
		public bool IsVisible = false;
		public Sector[] NeighboringSectors;
		public SectorType[] NeighboringSectorTypes;
		
		GameWorld Game;
		WorldInfo World;
		GameObject[] myRegions;
		SpriteRenderer[] mySprites;
		SpriteRenderer mapMask;
		
		//public static float SecondsPerUpdate = 10.0f;
		//public static float NextUpdateCountdown;
		
		// Use this for initialization
		void Start ()
		{
				Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				World = GameObject.Find ("World").GetComponent<WorldInfo> ();
				mapMask = gameObject.GetComponent<SpriteRenderer> ();
		
				GroupCount = new int[World.NumGroups];
				for (int i = 0; i < World.NumGroups; i++) {
						GroupCount [i] = 0;
				}
						
				SetZeds ();				
				SetNeighboringSectors ();				
				GetRegionSprites ();
		}
	
		void SetZeds ()
		{
				ZedCount = Random.Range (0, 100);
				ZedProbabilityMigrate = new float[4];	
		}
	
		void SetNeighboringSectors ()
		{
				NeighboringSectors = new Sector[4];
				NeighboringSectorTypes = new SectorType[8];
				if (LocationX > 0) {
						NeighboringSectors [West] = World.WorldSectors [LocationX - 1, LocationY];
						NeighboringSectorTypes [West] = World.WorldSectors [LocationX - 1, LocationY].SecType;
						ZedProbabilityMigrate [West] = 0.015f;
			
						if (LocationY > 0)
								NeighboringSectorTypes [SouthWest] = World.WorldSectors [LocationX - 1, LocationY - 1].SecType;
						if (LocationY < World.Dimensions [1] - 1)
								NeighboringSectorTypes [NorthWest] = World.WorldSectors [LocationX - 1, LocationY + 1].SecType;
				}
				if (LocationX < World.Dimensions [0] - 1) {
						NeighboringSectors [East] = World.WorldSectors [LocationX + 1, LocationY];
						NeighboringSectorTypes [East] = World.WorldSectors [LocationX + 1, LocationY].SecType;
						ZedProbabilityMigrate [East] = 0.015f;
			
						if (LocationY > 0)
								NeighboringSectorTypes [SouthEast] = World.WorldSectors [LocationX + 1, LocationY - 1].SecType;
						if (LocationY < World.Dimensions [1] - 1)
								NeighboringSectorTypes [NorthEast] = World.WorldSectors [LocationX + 1, LocationY + 1].SecType;
				}
				if (LocationY > 0) {
						NeighboringSectors [South] = World.WorldSectors [LocationX, LocationY - 1];
						NeighboringSectorTypes [South] = World.WorldSectors [LocationX, LocationY - 1].SecType;
						ZedProbabilityMigrate [South] = 0.015f;
				}
				if (LocationY < World.Dimensions [1] - 1) {
						NeighboringSectors [North] = World.WorldSectors [LocationX, LocationY + 1];
						NeighboringSectorTypes [North] = World.WorldSectors [LocationX, LocationY + 1].SecType;
						ZedProbabilityMigrate [North] = 0.015f;
				}
		}
	
		void GetRegionSprites ()
		{
				myRegions = new GameObject[SectorRegionNames.Length];
				mySprites = new SpriteRenderer[SectorRegionNames.Length];
		
				for (int x = 0; x < 3; x++) {
						for (int y = 0; y < 3; y++) {
								int i = x + y * 3;
								myRegions [i] = new GameObject (SectorRegionNames [i] + " Sprite");
								myRegions [i].AddComponent<SpriteRenderer> ();
								myRegions [i].transform.parent = transform;
								myRegions [i].transform.localPosition = new Vector3 ((float)(x - 1) / 3, (float)(y - 1) / 3, 9.0f);
								mySprites [i] = myRegions [i].GetComponent<SpriteRenderer> ();
						}
				}
				
				switch (SecType) {
				case SectorType.Water:
						SetWaterTiles ();
						break;
				default:
						SetNonWaterTiles ();
						break;
				}
		}
						
		void SetWaterTiles ()
		{			
				for (int i = 0; i < 9; i++)
						mySprites [i].sprite = Game.Sprites.ReturnSprite ("water-center");
		
				if (LocationY < World.Dimensions [1] - 1 && NeighboringSectorTypes [North] != SectorType.Water) {
						mySprites [6].sprite = Game.Sprites.ReturnSprite ("water-nc");
						mySprites [7].sprite = Game.Sprites.ReturnSprite ("water-nc");
						mySprites [8].sprite = Game.Sprites.ReturnSprite ("water-nc");
				}
				if (LocationY > 0 && NeighboringSectorTypes [South] != SectorType.Water) {
						mySprites [0].sprite = Game.Sprites.ReturnSprite ("water-sc");
						mySprites [1].sprite = Game.Sprites.ReturnSprite ("water-sc");
						mySprites [2].sprite = Game.Sprites.ReturnSprite ("water-sc");
				}
				if (LocationX < World.Dimensions [0] - 1 && NeighboringSectorTypes [East] != SectorType.Water) {			
						mySprites [5].sprite = Game.Sprites.ReturnSprite ("water-ec");
						
						if (mySprites [2].sprite == Game.Sprites.ReturnSprite ("water-sc"))
								mySprites [2].sprite = Game.Sprites.ReturnSprite ("ground");
						else if (mySprites [2].sprite == Game.Sprites.ReturnSprite ("water-center"))
								mySprites [2].sprite = Game.Sprites.ReturnSprite ("water-ec");				
				
						if (mySprites [8].sprite == Game.Sprites.ReturnSprite ("water-nc"))
								mySprites [8].sprite = Game.Sprites.ReturnSprite ("ground");
						else if (mySprites [8].sprite == Game.Sprites.ReturnSprite ("water-center"))
								mySprites [8].sprite = Game.Sprites.ReturnSprite ("water-ec");				
				}
				if (LocationX > 0 && NeighboringSectorTypes [West] != SectorType.Water) {
						mySprites [3].sprite = Game.Sprites.ReturnSprite ("water-wc");
						
						if (mySprites [0].sprite == Game.Sprites.ReturnSprite ("water-sc"))
								mySprites [0].sprite = Game.Sprites.ReturnSprite ("ground");
						else if (mySprites [0].sprite == Game.Sprites.ReturnSprite ("water-center"))
								mySprites [0].sprite = Game.Sprites.ReturnSprite ("water-wc");				
			
						if (mySprites [6].sprite == Game.Sprites.ReturnSprite ("water-nc"))
								mySprites [6].sprite = Game.Sprites.ReturnSprite ("ground");
						else if (mySprites [6].sprite == Game.Sprites.ReturnSprite ("water-center"))
								mySprites [6].sprite = Game.Sprites.ReturnSprite ("water-wc");				
						
				}
				
				if (LocationX > 0 && LocationY > 0 && NeighboringSectorTypes [SouthWest] != SectorType.Water && mySprites [0].sprite == Game.Sprites.ReturnSprite ("water-center"))
						mySprites [0].sprite = Game.Sprites.ReturnSprite ("water-sw");
				
				if (LocationX > 0 && LocationY < World.Dimensions [1] - 1 && NeighboringSectorTypes [NorthWest] != SectorType.Water && mySprites [6].sprite == Game.Sprites.ReturnSprite ("water-center"))
						mySprites [6].sprite = Game.Sprites.ReturnSprite ("water-nw");
		
				if (LocationX < World.Dimensions [0] - 1 && LocationY > 0 && NeighboringSectorTypes [SouthEast] != SectorType.Water && mySprites [2].sprite == Game.Sprites.ReturnSprite ("water-center"))
						mySprites [2].sprite = Game.Sprites.ReturnSprite ("water-se");
		
				if (LocationX < World.Dimensions [0] - 1 && LocationY < World.Dimensions [1] - 1 && NeighboringSectorTypes [NorthEast] != SectorType.Water && mySprites [8].sprite == Game.Sprites.ReturnSprite ("water-center"))
						mySprites [8].sprite = Game.Sprites.ReturnSprite ("water-ne");
		}
		
		void SetNonWaterTiles ()
		{
		
				switch (SecType) {
				case SectorType.Residential:
						for (int x = 0; x < 3; x++) {
								for (int y = 0; y < 3; y++) {
										int spriteNum = x * 3 + y;
										mySprites [spriteNum].sprite = Game.Sprites.ReturnSprite ("res-zone-" + (x + 1) + "-" + (y + 1));
								}
						}
						break;
				case SectorType.Commercial:
						for (int x = 0; x < 3; x++) {
								for (int y = 0; y < 3; y++) {
										int spriteNum = x * 3 + y;
										mySprites [spriteNum].sprite = Game.Sprites.ReturnSprite ("comm-zone-" + (x + 1) + "-" + (y + 1));
								}
						}
						break;
				case SectorType.Grass:
						for (int i = 0; i < 9; i++) {
								mySprites [i].sprite = Game.Sprites.ReturnSprite ("grass-center");
						}
						break;
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
				} else {
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
		}
		
		//updated each game hour
		public void DoNextUpdate ()
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