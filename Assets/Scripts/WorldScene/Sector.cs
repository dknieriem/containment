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
	//public static int MaxPopulation = 1000;
	public int Population;
	public int PlayerGroupCount;
	public Sector[] NeighboringSectors;
	public SectorType[] NeighboringSectorTypes;
	public bool IsVisible;
	public bool IsVisited;
	//Gameworld Game;
	public World world;
	GameObject[] myRegions;
	SpriteRenderer[] mySprites;
	SpriteRenderer mapMask;
	//public static float SecondsPerUpdate = 10.0f;
	//public static float NextUpdateCountdown;
		
	// Use this for initialization
	void Start ()
	{
		//TOO Many statements 
		//Debug.Log ("Starting: Sector " + gameObject.name);
		//Game = GameObject.Find ("Game").GetComponent<Gameworld> ();
		world = GameManager.Instance.world;
		mapMask = gameObject.GetComponent<SpriteRenderer> ();
	}

	void Awake ()
	{
		mapMask = gameObject.GetComponent<SpriteRenderer> ();
		if (PlayerGroupCount > 0) {
			IsVisible = true;
			IsVisited = true;
		} else {
			IsVisible = false;
		}
				
		if (GameManager.Instance.IsDebug || IsVisible) {
			mapMask.enabled = false;
		} else if (IsVisited) {
			mapMask.enabled = true;
			Color maskColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
//			if (Population > 1000)
//				maskColor = new Color (1.0f, Mathf.Clamp01 (Population / 100000.0f), Mathf.Clamp01 (Population / 100000.0f), 0.5f);
//			else if (Population > 100)
//				maskColor = new Color (Mathf.Clamp01 (Population / 1000.0f), 1.0f, 1.0f, 0.5f);
//			else
//				maskColor = new Color (Mathf.Clamp01 (Population / 100.0f), 1.0f, Mathf.Clamp01 (Population / 100.0f), 0.5f);
			mapMask.color = maskColor;//new Color (0.7f, 0.7f, 1.0f, 0.25f);
		} else {
			mapMask.enabled = true;
			mapMask.color = new Color (0.7f, 0.7f, 0.7f, 1.0f);
		}
	}

	public void SetNeighboringSectors ()
	{
		NeighboringSectors = new Sector[4];
		NeighboringSectorTypes = new SectorType[8];
		if (LocationX > 0) {
			NeighboringSectors [West] = world.WorldSectors [LocationX - 1, LocationY];
			NeighboringSectorTypes [West] = world.WorldSectors [LocationX - 1, LocationY].SecType;
						
			if (LocationY > 0)
				NeighboringSectorTypes [SouthWest] = world.WorldSectors [LocationX - 1, LocationY - 1].SecType;
			if (LocationY < world.DimensionsY - 1)
				NeighboringSectorTypes [NorthWest] = world.WorldSectors [LocationX - 1, LocationY + 1].SecType;
		}
		if (LocationX < world.DimensionsX - 1) {
			NeighboringSectors [East] = world.WorldSectors [LocationX + 1, LocationY];
			NeighboringSectorTypes [East] = world.WorldSectors [LocationX + 1, LocationY].SecType;
						
			if (LocationY > 0)
				NeighboringSectorTypes [SouthEast] = world.WorldSectors [LocationX + 1, LocationY - 1].SecType;
			if (LocationY < world.DimensionsY - 1)
				NeighboringSectorTypes [NorthEast] = world.WorldSectors [LocationX + 1, LocationY + 1].SecType;
		}
		if (LocationY > 0) {
			NeighboringSectors [South] = world.WorldSectors [LocationX, LocationY - 1];
			NeighboringSectorTypes [South] = world.WorldSectors [LocationX, LocationY - 1].SecType;
		}
		if (LocationY < world.DimensionsY - 1) {
			NeighboringSectors [North] = world.WorldSectors [LocationX, LocationY + 1];
			NeighboringSectorTypes [North] = world.WorldSectors [LocationX, LocationY + 1].SecType;
		}
	}

	public void GetRegionSprites ()
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
				mySprites [i].GetComponent<SpriteRenderer> ().sortingLayerName = "Sectors";
				mySprites [i].GetComponent<SpriteRenderer> ().sortingOrder = 1;
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
			mySprites [i].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-center");
		
		if (LocationY < world.DimensionsY - 1 && NeighboringSectorTypes [North] != SectorType.Water) {
			mySprites [6].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-nc");
			mySprites [7].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-nc");
			mySprites [8].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-nc");
		}
		if (LocationY > 0 && NeighboringSectorTypes [South] != SectorType.Water) {
			mySprites [0].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-sc");
			mySprites [1].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-sc");
			mySprites [2].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-sc");
		}
		if (LocationX < world.DimensionsX - 1 && NeighboringSectorTypes [East] != SectorType.Water) {			
			mySprites [5].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-ec");
						
			if (mySprites [2].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-sc"))
				mySprites [2].sprite = GameManager.Instance.Sprites.ReturnSprite ("ground");
			else if (mySprites [2].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
				mySprites [2].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-ec");				
				
			if (mySprites [8].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-nc"))
				mySprites [8].sprite = GameManager.Instance.Sprites.ReturnSprite ("ground");
			else if (mySprites [8].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
				mySprites [8].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-ec");				
		}
		if (LocationX > 0 && NeighboringSectorTypes [West] != SectorType.Water) {
			mySprites [3].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-wc");
						
			if (mySprites [0].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-sc"))
				mySprites [0].sprite = GameManager.Instance.Sprites.ReturnSprite ("ground");
			else if (mySprites [0].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
				mySprites [0].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-wc");				
			
			if (mySprites [6].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-nc"))
				mySprites [6].sprite = GameManager.Instance.Sprites.ReturnSprite ("ground");
			else if (mySprites [6].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
				mySprites [6].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-wc");				
						
		}
				
		if (LocationX > 0 && LocationY > 0 && NeighboringSectorTypes [SouthWest] != SectorType.Water && mySprites [0].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
			mySprites [0].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-sw");
				
		if (LocationX > 0 && LocationY < world.DimensionsY - 1 && NeighboringSectorTypes [NorthWest] != SectorType.Water && mySprites [6].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
			mySprites [6].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-nw");
		
		if (LocationX < world.DimensionsX - 1 && LocationY > 0 && NeighboringSectorTypes [SouthEast] != SectorType.Water && mySprites [2].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
			mySprites [2].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-se");
		
		if (LocationX < world.DimensionsX - 1 && LocationY < world.DimensionsY - 1 && NeighboringSectorTypes [NorthEast] != SectorType.Water && mySprites [8].sprite == GameManager.Instance.Sprites.ReturnSprite ("water-center"))
			mySprites [8].sprite = GameManager.Instance.Sprites.ReturnSprite ("water-ne");

	}

	void SetNonWaterTiles ()
	{
		switch (SecType) {
		case SectorType.Residential:
			for (int x = 0; x < 3; x++) {
				for (int y = 0; y < 3; y++) {
					int spriteNum = x * 3 + y;
					mySprites [spriteNum].sprite = GameManager.Instance.Sprites.ReturnSprite ("res-zone-" + (x + 1) + "-" + (y + 1));
				}
			}
			break;
		case SectorType.Commercial:
			for (int x = 0; x < 3; x++) {
				for (int y = 0; y < 3; y++) {
					int spriteNum = x * 3 + y;
					mySprites [spriteNum].sprite = GameManager.Instance.Sprites.ReturnSprite ("comm-zone-" + (x + 1) + "-" + (y + 1));
				}
			}
			break;
		case SectorType.Grass:
			for (int i = 0; i < 9; i++) {
				//Debug.Log ("Grass i=" + i);
				mySprites [i].sprite = GameManager.Instance.Sprites.ReturnSprite ("grass-center");
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
				
		if (GameManager.Instance.IsDebug || IsVisible) {
			mapMask.enabled = false;
		} else if (IsVisited) {
			mapMask.enabled = true;
			Color maskColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
//			if (Population > 1000)
//				maskColor = new Color (1.0f, Mathf.Clamp01 (Population / 100000.0f), Mathf.Clamp01 (Population / 100000.0f), 0.5f);
//			else if (Population > 100)
//				maskColor = new Color (Mathf.Clamp01 (Population / 1000.0f), 1.0f, 1.0f, 0.5f);
//			else
//				maskColor = new Color (Mathf.Clamp01 (Population / 100.0f), 1.0f, Mathf.Clamp01 (Population / 100.0f), 0.5f);
			mapMask.color = maskColor;//new Color (0.7f, 0.7f, 1.0f, 0.25f);
		} else {
			mapMask.enabled = true;
			mapMask.color = new Color (0.7f, 0.7f, 0.7f, 1.0f);
		}
	}
		
	//updated each game hour
	public void DoNextUpdate ()
	{
	}

}