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
		
		public static int X = 0, Y = 1;
		public GameObject SectorPrefab;
		public WorldInfo WorkingWith;
		public int[] Dimensions;
		public float[][] WaterNoise, CityNoise;
		public float[,,] MapImage;
		public Sector.SectorType[,] WorldSectorTypes;
		public bool[,] SectorIsWater;
		public bool[,] SectorIsCity;
		public float WaterLevel = 0.1f;
		public float CivLevel = 0.4f;
		public float CommCityFraction = 0.05f; //fraction of city sectors that are commercial centers

		void Start ()
		{
				WorkingWith = GameObject.Find ("World").GetComponent<WorldInfo> ();
		}

		public void BuildWorld ()
		{		

				Debug.Log ("Working With: " + WorkingWith.name);
				WorkingWith.Dimensions = Dimensions;
				
				WorldSectorTypes = new Sector.SectorType[Dimensions [0], Dimensions [1]];
				SectorIsWater = new bool[Dimensions [0], Dimensions [1]];
				SectorIsCity = new bool[Dimensions [0], Dimensions [1]];
				WaterNoise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (Dimensions [0], Dimensions [1], 4);
				CityNoise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (Dimensions [0], Dimensions [1], 3);
				GenerateWater ();
			
				GenerateCities ();
			
				SaveImages ();
				
				RemoveExistingSectors ();
				InstantiateSectors ();
				WorkingWith.NumGroups = 4;
				WorkingWith.CurrentDate = new DateTime (1999, 10, 5, 14, 0, 0);
				//TODO: MORE
						
						
		}
		
		void SaveImages ()
		{
				MapImage = new float[Dimensions [0], Dimensions [1], 3];		
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
				
								if (SectorIsWater [x, y]) {
										MapImage [x, y, 0] = WaterNoise [x] [y];
										MapImage [x, y, 1] = WaterNoise [x] [y];
										MapImage [x, y, 2] = 1.0f;
								} else if (SectorIsCity [x, y]) {
										MapImage [x, y, 0] = 1.0f;
										MapImage [x, y, 1] = CityNoise [x] [y];
										MapImage [x, y, 2] = CityNoise [x] [y];
								} else {
										MapImage [x, y, 0] = WaterNoise [x] [y];
										MapImage [x, y, 1] = WaterNoise [x] [y];
										MapImage [x, y, 2] = WaterNoise [x] [y];
								}
						}
				}
				
				SaveImageFromFloat3 (MapImage, "perlinnoise-" + System.DateTime.Now.ToFileTime () + ".png");
		}
		
		void SaveImageFromBool (bool[,] data, string filename)
		{
				float[][] floatData = new float[data.GetLength (0)] [];
				for (int x = 0; x < data.GetLength(0); x++) {
						floatData [x] = new float[data.GetLength (1)];
						for (int y = 0; y < data.GetLength(1); y++) {
								floatData [x] [y] = data [x, y] ? 1 : 0;
						}
				}
				SaveImageFromFloat (floatData, filename);
		}
		
		void SaveImageFromFloat (float[][] data, string filename)
		{
				UnityEngine.Texture2D tex = new Texture2D (Dimensions [0], Dimensions [1]);
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								tex.SetPixel (x, y, new Color (data [x] [y], data [x] [y], data [x] [y]));
						}
				}
				byte[] bytes = tex.EncodeToPNG ();
				System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + filename, bytes);		
		}
		void SaveImageFromFloat3 (float[,,] data, string filename)
		{
				UnityEngine.Texture2D tex = new Texture2D (Dimensions [0], Dimensions [1]);
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								tex.SetPixel (x, y, new Color (data [x, y, 0], data [x, y, 1], data [x, y, 2]));
						}
				}
				tex.Apply ();
				GameObject.Find ("DebugControlPanel").GetComponent<DebugInfoScript> ().GetNewMapImage (tex);
				
				byte[] bytes = tex.EncodeToPNG ();
				System.IO.File.WriteAllBytes (Application.persistentDataPath + "/" + filename, bytes);		
		}
		
		
		public void GenerateWater ()
		{
				if (WaterLevel == 0.0f) {
						return;
				}
				
				int numWaterSectorsToCreate = Mathf.FloorToInt (Dimensions [0] * Dimensions [1] * WaterLevel);
				int numWaterBodies = UnityEngine.Random.Range (1, 4);
		
				List<Float2D> Candidates = new List<Float2D> ();
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								Candidates.Add (new Float2D (x, y, WaterNoise [x] [y]));
						}
				}
			
				//first sorted array	
				Float2D[] SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
				
				for (int i = 0; i < numWaterBodies; i++) {
						Float2D firstSeed = SortedCandidates [0]; //pick a water sector
						SectorIsWater [firstSeed.X, firstSeed.Y] = true; //set it as water
						WorldSectorTypes [firstSeed.X, firstSeed.Y] = Sector.SectorType.Water;
						numWaterSectorsToCreate--; //record that a water sector was made
						//Collection<Float2D> Exceptions = Candidates.Where (c => Distance (c.x, firstSeed.x) < Dimensions [0] / 4 && 
						//		Distance (c.y, firstSeed.y) < Dimensions [1] / 4);
						Candidates = Candidates.Where (c => Distance (c.X, firstSeed.X) > Dimensions [0] / 4 && 
								Distance (c.Y, firstSeed.Y) > Dimensions [1] / 4).ToList<Float2D> ();//.Except (Exceptions); //remove nearby sectors from the seeding process
						SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
				}
				
				Candidates = new List<Float2D> ();
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								if (SectorIsWater [x, y]) {
										AddNeighbors (Candidates, x, y, WaterNoise, SectorIsWater);
								}
						}
				}
		
				SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
		
				while (numWaterSectorsToCreate > 0) {
						Float2D newSecWater = SortedCandidates [0];
						SectorIsWater [newSecWater.X, newSecWater.Y] = true;
						WorldSectorTypes [newSecWater.X, newSecWater.Y] = Sector.SectorType.Water;
						numWaterSectorsToCreate--;
						AddNeighbors (Candidates, newSecWater.X, newSecWater.Y, WaterNoise, SectorIsWater);
						Candidates.Remove (newSecWater);
						
						SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
						
						foreach (Float2D candidate in SortedCandidates) {
								int x = candidate.X;
								int y = candidate.Y;
								if (x > 0 && x < Dimensions [0] - 1 && y > 0 && y < Dimensions [1] - 1) {
										if (SectorIsWater [x - 1, y] && SectorIsWater [x + 1, y] && SectorIsWater [x, y - 1] && SectorIsWater [x, y + 1]) {
												SectorIsWater [x, y] = true;
												WorldSectorTypes [x, y] = Sector.SectorType.Water;
												numWaterSectorsToCreate--;
												Candidates.Remove (candidate);
										}
								}
						}//end foreach
						SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
				}//end while
		}
		
		public void GenerateCities ()
		{
				if (CivLevel == 0.0f) {
						return;
				}
			
				int numCitySectorsToCreate = Mathf.FloorToInt (Dimensions [0] * Dimensions [1] * CivLevel);
				int numCommSectorsToCreate = Mathf.FloorToInt (numCitySectorsToCreate * CommCityFraction);
				Debug.Log ("Creating " + numCitySectorsToCreate + " city Sectors");
				Debug.Log ("Of which " + numCommSectorsToCreate + " are commercial sectors");
				int numCities = UnityEngine.Random.Range (1, 4);
				Debug.Log ("Total of " + numCities + " Cities");
				List<Float2D> Candidates = new List<Float2D> ();
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								Candidates.Add (new Float2D (x, y, CityNoise [x] [y]));
						}
				}
		
				//first sorted array	
				Float2D[] SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
		
				for (int i = 0; i < numCities; i++) {
						Float2D firstSeed = SortedCandidates [0]; //pick a city sector
						SectorIsCity [firstSeed.X, firstSeed.Y] = true; //set it as city
						
						AddToNeighbors (CityNoise, firstSeed.X, firstSeed.Y, 0.2f);
						WorldSectorTypes [firstSeed.X, firstSeed.Y] = Sector.SectorType.Commercial;
						numCitySectorsToCreate--; //record that a water sector was made
						numCommSectorsToCreate--;
						Candidates = Candidates.Where (c => Distance (c.X, firstSeed.X) > Dimensions [0] / 4 && 
								Distance (c.Y, firstSeed.Y) > Dimensions [1] / 4).ToList<Float2D> ();//.Except (Exceptions); //remove nearby sectors from the seeding process
						SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
				}
		
				Candidates = new List<Float2D> ();
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								if (SectorIsCity [x, y]) {
										AddNeighbors (Candidates, x, y, CityNoise, SectorIsCity);
								}
						}
				}
		
				SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
		
				while (numCitySectorsToCreate > 0) {
						Float2D newSecCity = SortedCandidates [0];
						SectorIsCity [newSecCity.X, newSecCity.Y] = true;
						if (numCommSectorsToCreate > 0) {
								WorldSectorTypes [newSecCity.X, newSecCity.Y] = Sector.SectorType.Commercial;
								numCommSectorsToCreate--;
						} else {
								WorldSectorTypes [newSecCity.X, newSecCity.Y] = Sector.SectorType.Residential;
						}
						numCitySectorsToCreate--;
						AddNeighbors (Candidates, newSecCity.X, newSecCity.Y, CityNoise, SectorIsCity);
						Candidates.Remove (newSecCity);
			
						SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
			
						foreach (Float2D candidate in SortedCandidates) {
								int x = candidate.X;
								int y = candidate.Y;
								if (x > 0 && x < Dimensions [0] - 1 && y > 0 && y < Dimensions [1] - 1) {
										if (SectorIsCity [x - 1, y] && SectorIsCity [x + 1, y] && SectorIsCity [x, y - 1] && SectorIsCity [x, y + 1]) {
												SectorIsCity [x, y] = true;
												WorldSectorTypes [x, y] = Sector.SectorType.Residential;
												numCitySectorsToCreate--;
												Candidates.Remove (candidate);
										}
								}
						}//end foreach
						SortedCandidates = Candidates.OrderByDescending (c => c.Value).ToArray<Float2D> ();
				}//end while
		}

		void AddToNeighbors (float[][] floatArray2D, int x, int y, float s)
		{
				int maximumX = floatArray2D.Length;
				int maximumY = floatArray2D [0].Length;
				if (x > 0) {
						floatArray2D [x - 1] [y] += s;
				}
				if (x < maximumX) {
						floatArray2D [x + 1] [y] += s;
				}
				if (y > 0) {
						floatArray2D [x] [y - 1] += s;
				}
				if (y < maximumY) {
						floatArray2D [x] [y + 1] += s;
				}
		
		}		
		
		
		void AddNeighbors (List<Float2D> candidates, int x, int y, float[][] valueSource, bool[,] checkSource)
		{
		
				if (x > 0 && !SectorIsWater [x - 1, y] && !checkSource [x - 1, y]) {
						candidates.Add (new Float2D (x - 1, y, valueSource [x - 1] [y] + valueSource [x] [y]));
				}
				if (x < Dimensions [0] - 1 && !SectorIsWater [x + 1, y] && !checkSource [x + 1, y]) {
						candidates.Add (new Float2D (x + 1, y, valueSource [x + 1] [y] + valueSource [x] [y]));
				}
				if (y > 0 && !SectorIsWater [x, y - 1] && !checkSource [x, y - 1]) {
						candidates.Add (new Float2D (x, y - 1, valueSource [x] [y - 1] + valueSource [x] [y]));
				}
				if (y < Dimensions [1] - 1 && !SectorIsWater [x, y + 1] && !checkSource [x, y + 1]) {
						candidates.Add (new Float2D (x, y + 1, valueSource [x] [y + 1] + valueSource [x] [y]));
				}
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

				WorkingWith.WorldSectors = new Sector[Dimensions [0], Dimensions [1]];
				for (int i = 0; i < Dimensions[0]; i++) {
						for (int j = 0; j < Dimensions[1]; j++) {
								GameObject Sector = Instantiate (SectorPrefab) as GameObject;
								Sector.transform.parent = WorkingWith.transform;
								Sector.transform.position = new Vector3 (i + 0.5f, j + 0.5f, 2);
								Sector sec = Sector.GetComponent<Sector> ();
								
								sec.LocationX = i;
								sec.LocationY = j;
								sec.SecType = WorldSectorTypes [i, j];
								Sector.name = "Sector [" + i + ", " + j + "]";
								WorkingWith.WorldSectors [i, j] = sec;
						}
				}
		}
}

