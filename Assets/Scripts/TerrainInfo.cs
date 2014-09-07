using UnityEngine;
using System.Collections;

public class TerrainInfo : MonoBehaviour
{
		public GameObject TerrainPrefab;
		public int[] Dimensions;

		// Use this for initialization
		void Start ()
		{
				Dimensions = new int[2];
				Dimensions [0] = 32;
				Dimensions [1] = 32;
				InstantiateTerrainTiles ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void InstantiateTerrainTiles ()
		{
				for (int i = 0; i < Dimensions[0]; i++) {
						for (int j = 0; j < Dimensions[1]; j++) {
								GameObject terrainTile = Instantiate (TerrainPrefab) as GameObject;
								terrainTile.transform.parent = transform;
								terrainTile.transform.position = new Vector3 (i, j, 2);
								terrainTile.name = "Terrain [" + i + ", " + j + "]";
						}
				}
		}
}
