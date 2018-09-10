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
	public Sector.SectorType[,] WorldSectorTypes;
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

		int WorldSizeProp = worldProps.GetEnumValue ("World Size");
		if (WorldSizeProp == -1) {
			WorldSizeProp = 2;
			Debug.Log ("WorldSizeProp was null!");
		}

		int dimension = 128;
		World.WorldSizes.TryGetValue (WorldSizeProp, out dimension);
		Debug.Log ("World Size: " + dimension);
		DimensionsX = dimension;
		DimensionsY = dimension;
		WorkingWith.DimensionsX = DimensionsX;
		WorkingWith.DimensionsY = DimensionsY;
				
		WorldSectorTypes = new Sector.SectorType[DimensionsX, DimensionsY];
		SectorIsWater = new bool[DimensionsX, DimensionsY];
		SectorIsCity = new bool[DimensionsX, DimensionsY];
		SectorHasRoad = new bool[DimensionsX, DimensionsY];
        SectorScore = new float[DimensionsX, DimensionsY];
		WaterNoise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (DimensionsX, DimensionsY, 4);
		PopulationNoise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (DimensionsX, DimensionsY, 4);
		StartingPopulationPerSector = StartingPopulation / (float)(DimensionsX * DimensionsY);
		StartingZedsPerSector = StartingZedCount / (float)(DimensionsX * DimensionsY);
		Debug.Log ("Pop per sector: " + StartingPopulationPerSector);
		ZedDensity = new float[DimensionsX, DimensionsY];
		HumanDensity = new float[DimensionsX, DimensionsY];
        GenerateHeightMap();
		GenerateWater ();
			
		GenerateCities ();
			
		//RemoveExistingSectors ();
		InstantiateSectors ();
		SetSectorHumanCounts ();
		SetSectorZedCounts ();

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

    public void GenerateHeightMap()
    {
        for (int x = 0; x < DimensionsX; x++)
        {
            for (int y = 0; y < DimensionsY; y++)
            {
                //Debug.Log(WaterNoise[x][y]); 
            }
        }
    }

    public void GenerateWater()
    {
        for (int x = 0; x < DimensionsX; x++)
        {
            for (int y = 0; y < DimensionsY; y++)
            {
                if(WaterNoise[x][y] < WaterLevel)
                {
                    SectorIsWater[x, y] = true;
                    WorldSectorTypes[x, y] = Sector.SectorType.Water;

                } 
            }
        }
    }

    public void GenerateCities ()
	{
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {
                if (!SectorIsWater[x, y])
                {
                    if (PopulationNoise[x][y] > CityLevel)
                    {
                        SectorIsCity[x, y] = true;
                        WorldSectorTypes[x, y] = Sector.SectorType.Commercial;
                        HumanDensity[x, y] = PopulationNoise[x][y] * PopulationNoise[x][y] * StartingPopulationPerSector * 100.0f;
                    } else {

                        if (PopulationNoise[x][y] > CityLevel * 0.75f)
                        {
                            WorldSectorTypes[x, y] = Sector.SectorType.Residential;
                            HumanDensity[x, y] = PopulationNoise[x][y] * PopulationNoise[x][y] * StartingPopulationPerSector * 4.0f;
                        } else
                        {
                            HumanDensity[x, y] = PopulationNoise[x][y] * PopulationNoise[x][y] * StartingPopulationPerSector;
                            WorldSectorTypes[x, y] = Sector.SectorType.Forest;
                        }
                        
                    }

                    //				
                } else
                {
                    HumanDensity[x, y] = 0;
                }
                //Debug.Log (HumanDensity [x, y]);
			}
		}
	}

    void adjustScores(int[] cityPos, int cityRadius)
    {
        for (int x = cityPos[0] - cityRadius; x < cityPos[0] + cityRadius; x++){
            for (int y = cityPos[1] - cityRadius; y < cityPos[1] + cityRadius; y++){

                if (x >= 0 && x < WorkingWith.DimensionsX && y >= 0 && y < WorkingWith.DimensionsY){
                    int d = Sector.Distance(new int[] {x,y}, cityPos);

                    if (d < cityRadius){
                       SectorScore[x,y] -= 50.0f / (d * d);
                    }
                }
            }
		}
	}

	int[] getHighScore()
    {
        int[] newPos = { 0, 0 };
        float maxScore = 0;

        for (int x = 0; x < WorkingWith.DimensionsX; x++){
            for (int y = 0; y < WorkingWith.DimensionsY; y++){
                if (SectorScore[x,y] > maxScore){
                    maxScore = SectorScore[x, y];
                    newPos[0] = x;
                    newPos[1] = y;
                }
            }
        }

        return newPos;
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
		
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {
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
		WorkingWith.DimensionsX = DimensionsX;
		WorkingWith.DimensionsY = DimensionsY;

		WorkingWith.WorldSectors = new Sector[DimensionsX, DimensionsY];
		for (int i = 0; i < DimensionsX; i++) {
			for (int j = 0; j < DimensionsY; j++) {
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
		for (int i = 0; i < DimensionsX; i++) {
			for (int j = 0; j < DimensionsY; j++) {
				//WorkingWith.WorldSectors [i, j].SetZedCountRandom (0, 100);
				WorkingWith.WorldSectors [i, j].SetNeighboringSectors ();				
				WorkingWith.WorldSectors [i, j].GetRegionSprites ();
			}
		}
	}

	void SetSectorHumanCounts ()
	{
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {		
				//Sector sec = WorkingWith.WorldSectors [x, y];
								
				//sec.Population = Mathf.FloorToInt (HumanDensity [x, y]);
			}
		}
	}

	void SetSectorZedCounts ()
	{
		for (int x = 0; x < DimensionsX; x++) {
			for (int y = 0; y < DimensionsY; y++) {		
		        Sector sec = WorkingWith.WorldSectors [x, y];

                sec.ZedCount = Mathf.FloorToInt(HumanDensity[x, y] * StartingZedsPerSector / StartingPopulationPerSector);
              			
			}
		}
	}
}

