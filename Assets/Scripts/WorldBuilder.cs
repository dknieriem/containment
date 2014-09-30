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
				public int x, y;
				public float value;

				public Float2D (int x, int y, float value)
				{
						this.x = x;
						this.y = y;
						this.value = value;
				}
				
				public int CompareTo (Float2D other)
				{
						if (this.value > (other).value) {
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
		public float[][] Noise;
		public float[,,] MapImage;
		public Sector.SectorType[,] WorldSectorTypes;
		public bool[,] SectorIsWater;
		public float WaterLevel = 0.1f;
		public float CivLevel = 0.4f;

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
			
				Noise = PerlinNoise.PerlinNoise.GeneratePerlinNoise (Dimensions [0], Dimensions [1], 4);
				GenerateWater ();
			
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
										MapImage [x, y, 0] = Noise [x] [y];
										MapImage [x, y, 1] = Noise [x] [y];
										MapImage [x, y, 2] = 1.0f;
								} else {
										MapImage [x, y, 0] = Noise [x] [y];
										MapImage [x, y, 1] = Noise [x] [y];
										MapImage [x, y, 2] = Noise [x] [y];
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
								Candidates.Add (new Float2D (x, y, Noise [x] [y]));
						}
				}
			
				//first sorted array	
				Float2D[] SortedCandidates = Candidates.OrderByDescending (c => c.value).ToArray<Float2D> ();
				
				for (int i = 0; i < numWaterBodies; i++) {
						Float2D firstSeed = SortedCandidates [0]; //pick a water sector
						SectorIsWater [firstSeed.x, firstSeed.y] = true; //set it as water
						WorldSectorTypes [firstSeed.x, firstSeed.y] = Sector.SectorType.Water;
						numWaterSectorsToCreate--; //record that a water sector was made
						//Collection<Float2D> Exceptions = Candidates.Where (c => Distance (c.x, firstSeed.x) < Dimensions [0] / 4 && 
						//		Distance (c.y, firstSeed.y) < Dimensions [1] / 4);
						Candidates = Candidates.Where (c => Distance (c.x, firstSeed.x) > Dimensions [0] / 4 && 
								Distance (c.y, firstSeed.y) > Dimensions [1] / 4).ToList<Float2D> ();//.Except (Exceptions); //remove nearby sectors from the seeding process
						SortedCandidates = Candidates.OrderByDescending (c => c.value).ToArray<Float2D> ();
				}
				
				Candidates = new List<Float2D> ();
				for (int x = 0; x < Dimensions[0]; x++) {
						for (int y = 0; y < Dimensions[1]; y++) {
								if (SectorIsWater [x, y]) {
										AddNeighbors (Candidates, x, y);
								}
						}
				}
		
				SortedCandidates = Candidates.OrderByDescending (c => c.value).ToArray<Float2D> ();
		
				while (numWaterSectorsToCreate > 0) {
						Float2D newSecWater = SortedCandidates [0];
						SectorIsWater [newSecWater.x, newSecWater.y] = true;
						WorldSectorTypes [newSecWater.x, newSecWater.y] = Sector.SectorType.Water;
						numWaterSectorsToCreate--;
						AddNeighbors (Candidates, newSecWater.x, newSecWater.y);
						Candidates.Remove (newSecWater);
						
						SortedCandidates = Candidates.OrderByDescending (c => c.value).ToArray<Float2D> ();
						
						foreach (Float2D candidate in SortedCandidates) {
								int x = candidate.x;
								int y = candidate.y;
								if (x > 0 && x < Dimensions [0] - 1 && y > 0 && y < Dimensions [1] - 1) {
										if (SectorIsWater [x - 1, y] && SectorIsWater [x + 1, y] && SectorIsWater [x, y - 1] && SectorIsWater [x, y + 1]) {
												SectorIsWater [x, y] = true;
												WorldSectorTypes [x, y] = Sector.SectorType.Water;
												numWaterSectorsToCreate--;
												Candidates.Remove (candidate);
										}
								}
						}//end foreach
						SortedCandidates = Candidates.OrderByDescending (c => c.value).ToArray<Float2D> ();
				}//end while
		}
		
		void AddNeighbors (List<Float2D> candidates, int x, int y)
		{
		
				if (x > 0 && !SectorIsWater [x - 1, y])
						candidates.Add (new Float2D (x - 1, y, Noise [x - 1] [y]));
				if (x < Dimensions [0] - 1 && !SectorIsWater [x + 1, y])
						candidates.Add (new Float2D (x + 1, y, Noise [x + 1] [y]));
				if (y > 0 && !SectorIsWater [x, y - 1])
						candidates.Add (new Float2D (x, y - 1, Noise [x] [y - 1]));
				if (y < Dimensions [1] - 1 && !SectorIsWater [x, y + 1])
						candidates.Add (new Float2D (x, y + 1, Noise [x] [y + 1]));
		
		
		}
		
		void RemoveExistingSectors ()
		{
		
		}
		void InstantiateSectors ()
		{
		
				ICollection<Sector> ExistingSectors = WorkingWith.GetComponentsInChildren<Sector> ();
				Debug.Log ("About to destroy " + ExistingSectors.Count);
				foreach (Sector sector in ExistingSectors) {
						Destroy (sector.gameObject);
				}
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

