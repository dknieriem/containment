using UnityEngine;

using System;
using System.Collections;
using System.Linq;
using PerlinNoise;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class WorldBuilder : MonoBehaviour
{
	
	public int Distance (int one, int two)
	{
		int result = one - two;
		if (result < 0)
			result *= -1;

		return result;
	}

	public struct Float2D : IComparable , IComparable<Float2D>
	{
		public int X, Y;
		public float Value;

		public Float2D (int x, int y, float value)
		{
			this.X = x;
			this.Y = y;
			this.Value = value;
		}

		public int CompareTo (Float2D other)
		{
			if (this.Value > (other).Value) {
				return 1;
			} else {
				return -1;
			}
		}

		int IComparable.CompareTo (object other) // Nongeneric IComparable
		{
			if (!(other is Float2D))
				throw new InvalidOperationException ("CompareTo: Not a note");
			return CompareTo ((Float2D)other);
		}

		public static bool operator < (Float2D n1, Float2D n2)
		{
			return n1.CompareTo (n2) < 0;
		}

		public static bool operator > (Float2D n1, Float2D n2)
		{
			return n1.CompareTo (n2) > 0;
		}
	}
		
	//public static int X = 0, Y = 1;
	public GameObject SectorPrefab;
	public World WorkingWith;
	GameManager gameManager;
	public int[] Dimensions;
	public float[][] WaterNoise, PopulationNoise;
	public float[,] HumanDensity;
	public float[,,] MapImage;
	public Sector.SectorType[,] WorldSectorTypes;
	public bool[,] SectorIsWater;
	public bool[,] SectorIsCity;
	public bool[,] SectorHasRoad;
	public float WaterLevel = 0.1f;
	//public float CivLevel = 0.4f;
	public float StartingPopulation = 1000000.0f;
	public float StartingPopulationPerSector;
	//public float CommCityFraction = 0.05f; //fraction of city sectors that are commercial centers

	void Start ()
	{
		Debug.Log ("Starting: WorldBuilder");
		gameManager = GameManager.Instance;
		WorkingWith = gameManager.world;//GameObject.Find ("World").GetComponent<World> ();
	}

	public void BuildWorld ()
	{		
		Debug.Log ("Working With: " + WorkingWith.name);


		WorkingWith.DimensionsX = Dimensions [0];
		WorkingWith.DimensionsY = Dimensions [1];
				
		WorldSectorTypes = new Sector.SectorType[Dimensions [0], Dimensions [1]];
		SectorIsWater = new bool[Dimensions [0], Dimensions [1]];
		SectorIsCity = new bool[Dimensions [0], Dimensions [1]];
		SectorHasRoad = new bool[Dimensions [0], Dimensions [1]];
		WaterNoise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (Dimensions [0], Dimensions [1], 4);
		PopulationNoise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (Dimensions [0], Dimensions [1], 4);
		StartingPopulationPerSector = StartingPopulation / (float)(Dimensions [0] * Dimensions [1]);
		Debug.Log ("Pop per sector: " + StartingPopulationPerSector);
//				ZedDensity = new float[Dimensions [0], Dimensions [1]];
		HumanDensity = new float[Dimensions [0], Dimensions [1]];
		//	GenerateWater ();
			
		GenerateCities ();
			
		//RemoveExistingSectors ();
		InstantiateSectors ();
		SetSectorHumanCounts ();

		Faction playerGroup = gameManager.playerGroup;
		playerGroup.NewGroup ();

		SaveImages ();
		WorkingWith.NumGroups = 4;
		WorkingWith.CurrentDate = new DateTime (1999, 10, 5, 14, 0, 0);


		//TODO: MORE
						
						
	}

	void SaveImages ()
	{
		MapImage = new float[Dimensions [0], Dimensions [1], 3];		
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {
				
				if (SectorIsWater [x, y]) {
					MapImage [x, y, 0] = WaterNoise [x] [y];
					MapImage [x, y, 1] = WaterNoise [x] [y];
					MapImage [x, y, 2] = 0.8f;
				} else if (SectorIsCity [x, y]) {
					MapImage [x, y, 0] = 0.8f;
					MapImage [x, y, 1] = Mathf.Clamp01 (HumanDensity [x, y]);
					MapImage [x, y, 2] = PopulationNoise [x] [y];
				} else {
					MapImage [x, y, 0] = (WaterNoise [x] [y] + Mathf.Clamp01 (HumanDensity [x, y])) * 0.5f;
					MapImage [x, y, 1] = Mathf.Clamp01 (HumanDensity [x, y]);
					MapImage [x, y, 2] = Mathf.Clamp01 (HumanDensity [x, y]);
				}
			}
		}
				
		SaveImageFromFloat3 (MapImage, "perlinnoise-" + System.DateTime.Now.ToFileTime () + ".png");
	}

	void SaveImageFromBool (bool[,] data, string filename)
	{
		float[][] floatData = new float[data.GetLength (0)] [];
		for (int x = 0; x < data.GetLength (0); x++) {
			floatData [x] = new float[data.GetLength (1)];
			for (int y = 0; y < data.GetLength (1); y++) {
				floatData [x] [y] = data [x, y] ? 1 : 0;
			}
		}
		SaveImageFromFloat (floatData, filename);
	}

	void SaveImageFromFloat (float[][] data, string filename)
	{
		UnityEngine.Texture2D tex = new Texture2D (Dimensions [0], Dimensions [1]);
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {
				tex.SetPixel (x, y, new Color (data [x] [y], data [x] [y], data [x] [y]));
			}
		}
		byte[] bytes = tex.EncodeToPNG ();
		System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + filename, bytes);		
	}

	void SaveImageFromFloat3 (float[,,] data, string filename)
	{
		UnityEngine.Texture2D tex = new Texture2D (Dimensions [0], Dimensions [1]);
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {
				tex.SetPixel (x, y, new Color (data [x, y, 0], data [x, y, 1], data [x, y, 2]));
			}
		}
		tex.Apply ();

		WorkingWith.SetMinimapImage (tex);
		GameObject.Find ("DebugControlPanel").GetComponent<DebugInfoScript> ().GetNewMapImage (tex);
				
		byte[] bytes = tex.EncodeToPNG ();
		System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + filename, bytes);		
	}

	public void GenerateWater ()
	{
	}

	public void GenerateCities ()
	{
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {
				HumanDensity [x, y] = PopulationNoise [x] [y] * PopulationNoise [x] [y] * StartingPopulationPerSector * 4.0f;
//				Debug.Log (HumanDensity [x, y]);
			}
		}

		//for (int i = 0; i < 6; i++) {
		//		IteratePopulation ();
		//}
	}

	void IteratePopulation ()
	{
	
		//int[] centerOfMass = getCenterOfMass ();
		//Debug.Log ("COM: " + centerOfMass.ToString ());
				
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {
						
				int[] neighborMax = getNeighborMax (x, y);
				int[] neighborMin = getNeighborMin (x, y);
				float popToMove = 0.0f;
				for (int neighborX = x - 1; neighborX <= x + 1; neighborX++) {
					for (int neighborY = y - 1; neighborY <= y + 1; neighborY++) {
						if (neighborX == x && neighborY == y)
							break;
														
						if (neighborX < 0 || neighborX >= Dimensions [0] || neighborY < 0 || neighborY >= Dimensions [1])
							break;
						
						popToMove = 0;//HumanDensity [x, y] * 0.001f;
												
						//if (SameDirection (x, y, neighborX, neighborY, centerOfMass [0], centerOfMass [1]))
						//		popToMove = 1;//HumanDensity [x, y] * 0.001f;
						
						if (neighborX == neighborMax [0] && neighborY == neighborMax [1])
							popToMove = Mathf.Min (HumanDensity [x, y], 1);// * 0.1f;
														
						if (neighborX == neighborMin [0] && neighborY == neighborMin [1])
							popToMove = Mathf.Min (HumanDensity [x, y], 1);// * 0.1f;
						
						HumanDensity [x, y] -= popToMove;
						HumanDensity [neighborX, neighborY] += popToMove;
						//MovePop (x, y, neighborX, neighborY, popToMove);
					}
								
								
				}
				/*					float neighborPopulations = GetNeighborPopulations (x, y);

								//case 1:
								if (HumanDensity [x, y] * 8 < neighborPopulations) {//
										MovePopFromCell (x, y, 0.125f * HumanDensity [x, y]);
								}
								//case 2:
								else if (HumanDensity [x, y] * 4 > neighborPopulations) {
										MovePopFromCell (x, y, 0.0125f * HumanDensity [x, y]);
								}
								//case 3:
								else {
										MovePopToCell (x, y, 0.015625f * HumanDensity [x, y]);
								}
	*/
			}
		}
	}

	bool SameDirection (int startX, int startY, int x1, int y1, int x2, int y2)
	{
		int xDir1 = (startX - x1 > 0) ? -1 : 1;
		int yDir1 = (startY - y1 > 0) ? -1 : 1;
		int xDir2 = (startX - x2 > 0) ? -1 : 1;
		int yDir2 = (startY - y2 > 0) ? -1 : 1;
		
		if (xDir1 == xDir2 && yDir1 == yDir2)
			return true;
		else
			return false;
	}

		
	int[] getCenterOfMass ()
	{
		float runningMax = 0.0f;
		float[] locFloat = { 0.0f, 0.0f };
		int[] location = new int[2];
		
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {
				runningMax += HumanDensity [x, y];
				locFloat [0] += x * HumanDensity [x, y];
				locFloat [1] += y * x * HumanDensity [x, y];
			}
		}
		locFloat [0] += 0.5f;
		locFloat [1] += 0.5f;
		locFloat [0] /= runningMax;
		locFloat [1] /= runningMax;
		
		location [0] = Mathf.FloorToInt (locFloat [0]);
		location [1] = Mathf.FloorToInt (locFloat [1]);
		
		return location;
	}

	int[] getNeighborMax (int mapX, int mapY)
	{
		float runningMax = 0.0f;
		int[] location = new int[2];
				
		for (int x = mapX - 1; x <= mapX + 1; x++) {
			for (int y = mapY - 1; y <= mapY + 1; y++) {
				if (x > 0 && x < Dimensions [0] && y > 0 && y < Dimensions [1]) {
					if (HumanDensity [x, y] > runningMax) {
						runningMax = HumanDensity [x, y];
						location [0] = x;
						location [1] = y;
					}
				}
			}
		}
		return location;
	}

	int[] getNeighborMin (int mapX, int mapY)
	{
		
		int[] location = new int[2];
		
		return location;
	}

	void MovePopToCell (int mapX, int mapY, float propToMove)
	{
		
		float neighborPop = GetNeighborPopulations (mapX, mapY);
		if (neighborPop == 0.0f)
			return;
		
		for (int x = mapX - 1; x <= mapX + 1; x++) {
			for (int y = mapY - 1; y <= mapY + 1; y++) {
				if (x > 0 && x < Dimensions [0] && y > 0 && y < Dimensions [1]) {
					//1: calc prop of pop to move from this cell		
					float prop = (HumanDensity [x, y] / neighborPop) * propToMove;			
					HumanDensity [mapX, mapY] += prop;
					HumanDensity [x, y] -= prop;
				}
			}
		}
		
	}

	
	void MovePopFromCell (int mapX, int mapY, float propToMove)
	{
		
		//float neighborPop = GetNeighborPopulations (mapX, mapY);
		//if (neighborPop == 0.0f)
		//		return;
		
		for (int x = mapX - 1; x <= mapX + 1; x++) {
			for (int y = mapY - 1; y <= mapY + 1; y++) {
				if (x > 0 && x < Dimensions [0] && y > 0 && y < Dimensions [1]) {
					//1: calc prop of pop to move from this cell		
					float prop = propToMove / 8.0f;//(HumanDensity [x, y] / neighborPop) * propToMove;			
					HumanDensity [mapX, mapY] -= prop;
					HumanDensity [x, y] += prop;
				}
			}
		}
		
	}

	float GetNeighborPopulations (int mapX, int mapY)
	{
		float pop = 0;

		for (int x = mapX - 1; x <= mapX + 1; x++) {
			for (int y = mapY - 1; y <= mapY + 1; y++) {
				if (x > 0 && x < Dimensions [0] && y > 0 && y < Dimensions [1]) {

					pop += HumanDensity [x, y];			

				}
			}
		}

		return pop;
	}

	void RemoveExistingSectors ()
	{
		ICollection<Sector> ExistingSectors = WorkingWith.GetComponentsInChildren<Sector> ();
		Debug.Log ("About to destroy " + ExistingSectors.Count);
		foreach (Sector sector in ExistingSectors) {
			Destroy (sector.gameObject);
		}
	}

	void InstantiateSectors ()
	{
		RemoveExistingSectors ();
		WorkingWith.DimensionsX = Dimensions [0];
		WorkingWith.DimensionsY = Dimensions [1];

		WorkingWith.WorldSectors = new Sector[Dimensions [0], Dimensions [1]];
		for (int i = 0; i < Dimensions [0]; i++) {
			for (int j = 0; j < Dimensions [1]; j++) {
				GameObject Sector = Instantiate (SectorPrefab) as GameObject;

				Sector.transform.parent = WorkingWith.transform;
				Sector.transform.position = new Vector3 (i + 0.5f, j + 0.5f, 2);
				Sector sec = Sector.GetComponent<Sector> ();
								
				sec.LocationX = i;
				sec.LocationY = j;
				sec.world = WorkingWith;
				sec.SecType = WorldSectorTypes [i, j];
				Sector.name = "Sector [" + i + ", " + j + "]";
				WorkingWith.WorldSectors [i, j] = sec;
			}
		}
		for (int i = 0; i < Dimensions [0]; i++) {
			for (int j = 0; j < Dimensions [1]; j++) {
				//WorkingWith.WorldSectors [i, j].SetZedCountRandom (0, 100);
				WorkingWith.WorldSectors [i, j].SetNeighboringSectors ();				
				WorkingWith.WorldSectors [i, j].GetRegionSprites ();
			}
		}
	}

	void SetSectorHumanCounts ()
	{
		for (int x = 0; x < Dimensions [0]; x++) {
			for (int y = 0; y < Dimensions [1]; y++) {		
				Sector sec = WorkingWith.WorldSectors [x, y];
								
				sec.Population = Mathf.FloorToInt (HumanDensity [x, y]);
			}
		}
	}
		
}

