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

	public GameObject SectorPrefab;
	public World WorkingWith;
 
	GameManager gameManager;
	public int DimensionsX, DimensionsY;
	public float[][] WaterNoise, PopulationNoise;
    public float[,] SectorScore;
	public float[,] HumanDensity, ZedDensity;
	public float[,,] MapImage;
	//public Sector.SectorType[,] WorldSectorTypes;
	public bool[,] SectorIsWater;
	public bool[,] SectorIsCity;
	public bool[,] SectorHasRoad;
	public float WaterLevel = 0.1f;
	public float CivLevel = 0.4f;
    public float CityLevel = 0.9f;
	public float StartingPopulation = 1000.0f;
	public float StartingZedCount = 100000.0f;
	public float StartingPopulationPerSector;
	public float StartingZedsPerSector;
	//public float CommCityFraction = 0.05f; //fraction of city sectors that are commercial centers

	void Start ()
	{
		Debug.Log ("Starting: WorldBuilder");
		gameManager = GameManager.Instance;
		WorkingWith = gameManager.world;//GameObject.Find ("World").GetComponent<World> ();
	}

	public void BuildWorld (CharacterPropertySet charProps, GroupPropertySet groupProps, WorldPropertySet worldProps)
	{		
		Debug.Log ("Working With: " + WorkingWith.name);
		MapGenerator mapGenerator = gameManager.mapGenerator;



		int WorldSizeProp = worldProps.GetEnumValue ("World Size");
		Debug.Log("WorldSizeProp: " + WorldSizeProp);
		if (WorldSizeProp == -1) {
			WorldSizeProp = 2;
			Debug.Log ("WorldSizeProp was null!");
		}

		int dimension = 0; // 128;
		World.WorldSizes.TryGetValue (WorldSizeProp, out dimension);
		Debug.Log ("World Size: " + dimension);
		DimensionsX = dimension;
		DimensionsY = dimension;
		WorkingWith.DimensionsX = DimensionsX;
		WorkingWith.DimensionsY = DimensionsY;

		mapGenerator.graphWidth = DimensionsX;
		mapGenerator.graphHeight = DimensionsY;

		mapGenerator.Generate();

		Debug.Log ("Pop per sector: " + StartingPopulationPerSector);
		ZedDensity = new float[DimensionsX, DimensionsY];
		HumanDensity = new float[DimensionsX, DimensionsY];


		Group playerGroup = gameManager.playerGroup;
		playerGroup.NewGroup (groupProps, charProps, true);

		//SaveImages ();
		WorkingWith.NumGroups = 4;
		WorkingWith.CurrentDate = new DateTime (1999, 10, 5, 14, 0, 0);


		//TODO: MORE
						
						
	}

	void SaveImages ()
	{
		MapImage = new float[DimensionsX, DimensionsY, 3];		
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {
				
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
				
		//SaveImageFromFloat3 (MapImage, "perlinnoise-" + System.DateTime.Now.ToFileTime () + ".png");
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
		UnityEngine.Texture2D tex = new Texture2D (DimensionsX, DimensionsY);
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {
				tex.SetPixel (x, y, new Color (data [x] [y], data [x] [y], data [x] [y]));
			}
		}
		byte[] bytes = tex.EncodeToPNG ();
		System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + filename, bytes);		
	}

	void SaveImageFromFloat3 (float[,,] data, string filename)
	{
		UnityEngine.Texture2D tex = new Texture2D (DimensionsX, DimensionsY);
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {
				tex.SetPixel (x, y, new Color (data [x, y, 0], data [x, y, 1], data [x, y, 2]));
			}
		}
		tex.Apply ();

		WorkingWith.SetMinimapImage (tex);
		GameObject.Find ("DebugControlPanel").GetComponent<DebugInfoScript> ().GetNewMapImage (tex);
				
		byte[] bytes = tex.EncodeToPNG ();
		System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + filename, bytes);		
	}



}

