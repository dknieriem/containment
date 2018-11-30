using UnityEngine;
using System.Collections.Generic;
using System;

public class Sector : MonoBehaviour, IVector2Storable //
{

	private void OnDrawGizmos()
	{
		if (mapPoly == null || mapPoly.Count == 0) // || true -> just skip for now, kthx
		{
			return;
		}

		Gizmos.color = Color.black;
		for (int i = 0; i < mapPoly.Count; i ++)
		{
			Vector3 t0 = mapPoly[i];
			Vector3 t1;
			if (i < mapPoly.Count - 1)
			{
				t1 = mapPoly[i + 1];
			}
			else
			{
				t1 = mapPoly[0];
			}

			Gizmos.DrawLine(t0, t1);
		}

	}

	//attributes created by MapGenerator
	public uint Id;
	public Vector2 position { get; set; } //position on map
	public List<Vector2> mapPoly;
	public Mesh mesh;
	public float height = -1; //altitude
	public float[] NeighborSectorDistance;
	public float[] NeighborSectorTravelTime;
	public uint[] NeighborSectorIds;
	public int used = 0;
	public string type = "";
	public int featureNumber = -1;
	public int cType = -1;
	public int? harbor;
	public int? pit;
	public int? lake; //id of lake
	public float area; //area in square graph distance units (km?)
	public float flux = 0; //water flow
	public int? river; //id of river 
	public int? riverToSector; //id of sector river flows into
	public float? score; //city score
	public int? confluence;
	public int crossroad;
	public int? port;
	public int population;
	public int manor = -1;
	public float cost;
	public int path = 0;

	//old WorldBuilder attributes
	public int LocationX, LocationY;
	//public static int MaxPopulation = 1000;

	public World world;
	GameObject[] myRegions;

	public bool IsVisible;
	public bool IsVisited;
    public bool PlayerGroupPresent;
    public bool ZedsPresent;

	public int ZedCount;
	public int PlayerGroupCount;

	public int residentCapacity;
	public int defenseRating;
    public int maxBuildingCount;
    public int BuildingCount;

    public List<Building> Buildings;

	// Use this for initialization
	void Start ()
	{
		//TOO Many statements 
		//Debug.Log ("Starting: Sector " + gameObject.name);
		//Game = GameObject.Find ("Game").GetComponent<Gameworld> ();
		world = GameManager.Instance.world;
		//mapMask = gameObject.GetComponent<SpriteRenderer> ();
	}

	void Awake ()
	{
		//mapMask = gameObject.GetComponent<SpriteRenderer> ();
		if (PlayerGroupCount > 0) {
			IsVisible = true;
			IsVisited = true;
		} else {
			IsVisible = false;
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
				
	}
		
	//updated each game hour
	public void DoNextUpdate ()
	{
	}

    static public int Distance(int[] posOne, int[] posTwo)
    {
        return Mathf.FloorToInt( Mathf.Sqrt(Mathf.Pow(posOne[0] - posTwo[0], 2) + Mathf.Pow(posOne[1] - posTwo[1], 2)));
    }

	public void resetMesh()
	{
		mesh = GetComponent<MeshFilter>().mesh;

		if (mapPoly == null || mapPoly.Count == 0)
			return;

		mesh.Clear();

		Vector3[] vertices = new Vector3[mapPoly.Count];
		Color[] colors = new Color[mapPoly.Count];
		int[] tris = new int[(mapPoly.Count - 2) * 3];
		Vector2[] uvs = new Vector2[4];
		for(int i = 0; i < mapPoly.Count; i++)
		{
			vertices[i] = new Vector3(mapPoly[i].x, mapPoly[i].y);

			if (height < 20)
			{
				colors[i] = Color.blue;
			} else
			{
				colors[i] = Color.Lerp(Color.green, Color.red, height / 100.0f);
			}
			
		}

		for(int i = 0; i < mapPoly.Count - 2; i++)
		{
			int triIndex = i * 3;

			Vector3 a = vertices[0];
			Vector3 b = vertices[i + 1];
			Vector3 c = vertices[i + 2];

			Vector3 crossProd = Vector3.Cross(c - a, b - a);
			Vector3 cameraDir = Camera.main.transform.forward;

			float norm = Vector3.Dot(crossProd, cameraDir);

			if (norm > 0)
			{
				tris[triIndex] = 0;
				tris[triIndex + 1] = i + 1;
				tris[triIndex + 2] = i + 2;
			}
			else
			{
				//Debug.Log("Norm[" + i + "] < 0");
				tris[triIndex] = i + 2;
				tris[triIndex + 1] = i + 1;
				tris[triIndex + 2] = 0;
			}
			
		}

		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(0, 1);
		uvs[2] = new Vector2(1, 1);
		uvs[3] = new Vector2(1, 0);

		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.triangles = tris;

		mesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = mesh;
	}
}