using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int graphWidth = 32; //km
	public int graphHeight = 32; //km
	public int numCities = 15;
	public int minSectors = 32 * 32; //map needs to have enough sectors to keep it interesting

	public World world;

	public enum MapTemplates
	{
		Volcano,
		HighIsland,
		LowIsland,
		Continents,
		Archipelago,
		Mainland,
		Peninsulas,
		Atoll,
		NONE
	};

	public MapTemplates mapTemplate;

	// main data variables
	public int seed;
	Param parameters;
	Delaunay.Voronoi voronoi;
	Dictionary<Vector2, uint> siteIdsByCoord;
	Dictionary<uint, Vector2> siteCoordsById;
	//diagram, -> was d3 svg
	List<Delaunay.Site> sites;
	Dictionary<uint, List<Vector2>> polygons;
	List<Vector2> points;
	List<Sector> sectors; //was cells in Azgaar's
	List<float> heights;

	// Common variables
	public int graphSize = 1;
	//var modules = { }, customization = 0, history = [], historyStage = 0, elSelected, autoResize = true, graphSize,
	//  cells = [], land = [], riversData = [], manors = [], states = [], features = [],
	//  queue = [],

	struct Param
	{

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float rn(float input, int decimals = 0)
	{
		float m = Mathf.Pow(10, decimals);
		return Mathf.Round(input * m) / m;

	}

	public void Generate() //translated from Azgaar's fantasy map generator
	{
		Debug.Log("Random map");
		float total = Time.time;
		//applyMapSize(); -> use if we wind up creating an image of the map, or to resize a mesh or something
		//randomizeOptions();
		if(world == null)
		{
			world = GameObject.FindObjectOfType<World>();
		}
		
		placePoints();
		calculateVoronoi(points);
		detectNeighbors();
		//drawScaleBar();
		defineHeightmap();
		markFeatures();
		drawOcean();
		elevateLakes();
		resolveDepressionsPrimary();
		reGraph();
		resolveDepressionsSecondary();
		flux();
		addLakes();
		drawCoastline();
		drawRelief();
		generateCultures();
		manorsAndRegions();
		cleanData();
		total = Time.time - total;
		Debug.Log("Total: " + total);
		Debug.Log("/Random map");
	}

	// Locate points to calculate Voronoi diagram
	private void placePoints()
	{
		Debug.Log("placePoints");
		float time = Time.time;
		points = new List<Vector2>();
		float mod = rn((graphWidth + graphHeight) / 1500, 2); // screen size modifier
		float spacing = rn(7.5f * mod / graphSize, 2); // space between points before jittering
		points = getJitteredGrid(spacing);
		heights = new List<float>( new float[points.Count]);
		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/placePoints");
	}

	private List<Vector2> getJitteredGrid(float spacing)
	{
		Debug.Log("getJitteredGrid");
		float time = Time.time;

		float radius = spacing / 2;
		float jittering = radius * 0.9f;
		Debug.Log(string.Format("Spacing: {0}, Radius: {1}, Jittering: {2}", spacing, radius, jittering));

		List<Vector2> pts = new List<Vector2>(); //[n][];

		for (float x = radius - graphWidth * 0.5f; x < graphWidth * 0.5f; x += spacing)
		{
			for (float y = radius - graphHeight * 0.5f; y < graphHeight * 0.5f; y += spacing)
			{
				float xj = (x + jitter(jittering));
				float yj = (y + jitter(jittering));

				pts.Add(new Vector2(xj, yj));

			}
		}

		Debug.Log("pts: " + pts.Count);

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/placePoints");

		return pts;
	}

	private float jitter(float jittering)
	{
		return UnityEngine.Random.Range(-jittering, jittering);
	}

	private void calculateVoronoi(List<Vector2> pts)
	{
		Debug.Log("calculateVoronoi");
		float time = Time.time;

		voronoi = new Delaunay.Voronoi(pts, colors(pts.Count), new Rect(0, 0, graphWidth, graphHeight));

		sites = voronoi.getSites();
		polygons = new Dictionary<uint, List<Vector2>>(sites.Count);

		for(int i = 0; i < sites.Count; i++)
		{
			uint index = sites[i].getIndex();
			polygons.Add(index, sites[i].Region(voronoi.plotBounds));
			siteCoordsById.Add(index, sites[i].Coord);
			siteIdsByCoord.Add(sites[i].Coord, index);
		}

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/calculateVoronoi");
	}

	private List<uint> colors(int count)
	{
		List<uint> colors = new List<uint>();
		for (int i = 0; i < count; i++)
		{
			colors.Add(0);
		}

		return colors;
	}

	private void detectNeighbors()
	{
		Debug.Log("detectNeighbors");
		float time = Time.time;

		sectors = new List<Sector>(sites.Count);

		for (int i = 0; i < sites.Count; i++)
		{
			Delaunay.Site site = sites[i];
			uint sectorId = site.getIndex();
			string type = "";
			//List<Delaunay.Site> neighbors = site.NeighborSites();

			Vector2 position = site.Coord;
			List<Delaunay.Edge> edges = site.edges;
			List<uint> neighborIds = new List<uint>(edges.Count);
			List<float> neighborDists = new List<float>(edges.Count);
			//for(int j = 0; j < neighbors.Count; j++)
			//{
			//	neighborIds[j] = neighbors[j].getIndex();
			//}
			for (int j = 0; j < edges.Count; j++)
			{
				Delaunay.Edge edge = edges[j];
				if(edge.leftSite != null && edge.rightSite != null)
				{
					if (edge.leftSite.getIndex() == sectorId)
					{
						neighborIds.Add(edge.rightSite.getIndex());
						neighborDists.Add(Vector2.Distance(position, edge.rightSite.Coord));
					} else
					{
						neighborIds.Add(edge.leftSite.getIndex());
						neighborDists.Add(Vector2.Distance(position, edge.leftSite.Coord));
					}
				} else
				{
					type = "border";
				}
			}
			Sector newSector = new Sector();
			newSector.position = position;
			newSector.mapPoly = site.Region(new Rect(0, 0, graphWidth, graphHeight));
			newSector.height = 0;
			newSector.NeighborSectorDistance = null;
			newSector.NeighborSectorTravelTime = null;
			newSector.NeighborSectorIds = neighborIds.ToArray();
			sectors.Add(newSector);
		}



		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/detectNeighbors");
	}

	private void defineHeightmap()
	{
		Debug.Log("defineHeightmap");
		float time = Time.time;

		if (mapTemplate == MapTemplates.NONE)
		{
			float rnd = UnityEngine.Random.value;
			if (rnd > 0.95) { mapTemplate = MapTemplates.Volcano; }
			else if (rnd > 0.75) { mapTemplate = MapTemplates.HighIsland; }
			else if (rnd > 0.55) { mapTemplate = MapTemplates.LowIsland; }
			else if (rnd > 0.35) { mapTemplate = MapTemplates.Continents; }
			else if (rnd > 0.15) { mapTemplate = MapTemplates.Archipelago; }
			else if (rnd > 0.10) { mapTemplate = MapTemplates.Mainland; }
			else if (rnd > 0.01) { mapTemplate = MapTemplates.Peninsulas; }
			else { mapTemplate = MapTemplates.Atoll; }
		}

		if (mapTemplate == MapTemplates.Volcano) templateVolcano();
		if (mapTemplate == MapTemplates.HighIsland) templateHighIsland();
		if (mapTemplate == MapTemplates.LowIsland) templateLowIsland();
		if (mapTemplate == MapTemplates.Continents) templateContinents();
		if (mapTemplate == MapTemplates.Archipelago) templateArchipelago();
		if (mapTemplate == MapTemplates.Atoll) templateAtoll();
		if (mapTemplate == MapTemplates.Mainland) templateMainland();
		if (mapTemplate == MapTemplates.Peninsulas) templatePeninsulas();

		Debug.Log(" template: " + mapTemplate);

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/defineHeightmap");
	}

	// Heighmap Template: Volcano
	void templateVolcano()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		addHill(5, 0.35);
		addRange(3);
		addRange(-4);
	}

	// Heighmap Template: High Island
	void templateHighIsland()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		addRange(6);
		addHill(12, 0.25);
		addRange(-3);
		modifyHeights("land", 0, 0.75);
		addPit(1);
		addHill(3, 0.15);
	}

	// Heighmap Template: Low Island
	void templateLowIsland()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		smoothHeights(2);
		addRange(2);
		addHill(4, 0.4);
		addHill(12, 0.2);
		addRange(-8);
		modifyHeights("land", 0, 0.35);
	}

	// Heighmap Template: Continents
	void templateContinents()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		addHill(30, 0.25);
		int count = Mathf.CeilToInt(UnityEngine.Random.value * 4 + 4);
		addStrait(count);
		addPit(10);
		addRange(-10);
		modifyHeights("land", 0, 0.6);
		smoothHeights(2);
		addRange(3);
	}

	// Heighmap Template: Archipelago
	void templateArchipelago()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		addHill(12, 0.15);
		addRange(8);
		int count = Mathf.CeilToInt(UnityEngine.Random.value * 2 + 2);
		addStrait(count);
		addRange(-15);
		addPit(10);
		modifyHeights("land", -5, 0.7);
		smoothHeights(3);
	}

	// Heighmap Template: Atoll
	void templateAtoll()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		addHill(2, 0.35);
		addRange(2);
		smoothHeights(1);
		modifyHeights("27-100", 0, 0.1);
	}

	// Heighmap Template: Mainland
	void templateMainland()
	{
		addMountain();
		modifyHeights("all", 10, 1);
		addHill(30, 0.2);
		addRange(10);
		addPit(20);
		addHill(10, 0.15);
		addRange(-10);
		modifyHeights("land", 0, 0.4);
		addRange(10);
		smoothHeights(3);
	}

	// Heighmap Template: Peninsulas
	void templatePeninsulas()
	{
		addMountain();
		modifyHeights("all", 15, 1);
		addHill(30, 0);
		addRange(5);
		addPit(15);
		int count = Mathf.CeilToInt(UnityEngine.Random.value * 5 + 15);
		addStrait(count);
	}

	void addMountain()
	{
		float x = UnityEngine.Random.value * graphWidth / 3 + graphWidth / 3;
		float y = UnityEngine.Random.value * graphHeight * 0.2f + graphHeight * 0.4f;
		Vector2 site = (Vector2) voronoi.NearestSitePoint(x, y);
		uint cellId;
		if (siteIdsByCoord.ContainsKey(site))
		{
			siteIdsByCoord.TryGetValue(site, out cellId);
			float height = UnityEngine.Random.value * 10 + 90; // 90-99
			add(cellId, "mountain", height);
		} else
		{
			Debug.Log("CellId for " + site.ToString() + " not found!");
		}

	}

	void add(uint start, string type, float height)
	{
		int session = Mathf.CeilToInt(UnityEngine.Random.value * (float)1e5);
		float radius = 0, hRadius = 0, mRadius = 0;
		switch (graphSize)
		{
			case 1: hRadius = 0.991f; mRadius = 0.91f; break;
			case 2: hRadius = 0.9967f; mRadius = 0.951f; break;
			case 3: hRadius = 0.999f; mRadius = 0.975f; break;
			case 4: hRadius = 0.9994f; mRadius = 0.98f; break;
		}
		radius = type == "mountain" ? mRadius : hRadius;
		List<uint> queue = new List<uint>(new uint[] { start });
		if (type == "mountain") heights[(int)start] = height;
		for (int i = 0; i < queue.Count && height >= 1; i++)
		{
			if (type == "mountain") { height = heights[(int)queue[i]] * radius - height / 100; }
			else { height *= radius; }

			uint[] neighbors = sectors[(int)queue[i]].NeighborSectorIds;
			for(int j = 0; j < neighbors.Length; j++){
				int e = (int) neighbors[j];
				if (sectors[e].used == session) return;
				float mod = UnityEngine.Random.value * 0.2f + 0.9f; // 0.9-1.1 random factor
				heights[e] += height * mod;
				if (heights[e] > 100) heights[(int)e] = 100;
				sectors[e].used = session;
				queue.Add((uint)e);
			}
		}
	}

	private void cleanData()
	{
		throw new NotImplementedException();
	}

	private void manorsAndRegions()
	{
		throw new NotImplementedException();
	}

	private void generateCultures()
	{
		throw new NotImplementedException();
	}

	private void drawRelief()
	{
		throw new NotImplementedException();
	}

	private void drawCoastline()
	{
		throw new NotImplementedException();
	}

	private void addLakes()
	{
		throw new NotImplementedException();
	}

	private void flux()
	{
		throw new NotImplementedException();
	}

	private void resolveDepressionsSecondary()
	{
		throw new NotImplementedException();
	}

	private void reGraph()
	{
		throw new NotImplementedException();
	}

	private void resolveDepressionsPrimary()
	{
		throw new NotImplementedException();
	}

	private void elevateLakes()
	{
		throw new NotImplementedException();
	}

	private void drawOcean()
	{
		throw new NotImplementedException();
	}

	private void markFeatures()
	{
		throw new NotImplementedException();
	}


}
