﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	[Range(16, 128)]
	public int graphWidth = 32; //km

	[Range(16, 128)]
	public int graphHeight = 32; //km

	[Range(0, 150)]
	public int numCities = 15;
	public int minSectors = 32 * 32; //map needs to have enough sectors to keep it interesting

	[Range(0, 10)]
	public int numRegions = 5;

	[Range(0,25)]
	public int precInput = 5; 
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
	List<Feature> features;
	List<int> land;
	List<Manor> manors;
	// Common variables
	public int graphSize = 1;
	//var modules = { }, customization = 0, history = [], historyStage = 0, elSelected, autoResize = true, graphSize,
	//  cells = [], land = [], riversData = [], manors = [], states = [], features = [],
	//  queue = [],

	struct Param
	{

	}

	struct Manor
	{
		public int i;
		public int region;
		public int population;
		public int siteId;
	}

	struct RiverData
	{
		public int river;
		public int siteId;
		public Vector2 position;

		public RiverData(int river, int siteId, Vector2 position)
		{
			this.river = river;
			this.siteId = siteId;
			this.position = position;
		}
	}

	struct Feature
	{
		public int i;
		public bool border;
		public bool land;
		public int? river;

		//public Feature(int i, bool border, bool land)
		//{
		//	this.i = i;
		//	this.border = border;
		//	this.land = land;
		//}
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

		List<Delaunay.Site> vSites = voronoi.getSites();
		sites = new List<Delaunay.Site>(new Delaunay.Site[vSites.Count]);
		polygons = new Dictionary<uint, List<Vector2>>(vSites.Count);

		for(int i = 0; i < vSites.Count; i++)
		{
			Delaunay.Site site = vSites[i];
			uint index = vSites[i].getIndex();
			polygons.Add(index, sites[i].Region(voronoi.plotBounds));
			siteCoordsById.Add(index, sites[i].Coord);
			siteIdsByCoord.Add(sites[i].Coord, index);
			sites[(int)index] = site;
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

		sectors = new List<Sector>(new Sector[sites.Count]);

		for (int i = 0; i < sites.Count; i++)
		{
			Delaunay.Site site = sites[i];
			uint sectorId = site.getIndex();
			string type = "";
			int? ctype = null;
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
					ctype = 99;
				}
			}
			Sector newSector = new Sector();
			newSector.Id = sectorId;
			newSector.position = position;
			newSector.mapPoly = site.Region(new Rect(0, 0, graphWidth, graphHeight));
			//newSector.height = 0;
			newSector.NeighborSectorDistance = null;
			newSector.NeighborSectorTravelTime = null;
			newSector.type = type;
			if (ctype == 99) newSector.cType = (int) ctype; 
			newSector.NeighborSectorIds = neighborIds.ToArray();
			sectors[(int)sectorId] = newSector;
			//sectors.Add(newSector);
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
		modifyHeights("all", 10, 1.0f);
		addHill(5, 0.35f);
		addRange(3);
		addRange(-4);
	}

	// Heighmap Template: High Island
	void templateHighIsland()
	{
		addMountain();
		modifyHeights("all", 10, 1.0f);
		addRange(6);
		addHill(12, 0.25f);
		addRange(-3);
		modifyHeights("land", 0, 0.75f);
		addPit(1);
		addHill(3, 0.15f);
	}

	// Heighmap Template: Low Island
	void templateLowIsland()
	{
		addMountain();
		modifyHeights("all", 10, 1.0f);
		smoothHeights(2);
		addRange(2);
		addHill(4, 0.4f);
		addHill(12, 0.2f);
		addRange(-8);
		modifyHeights("land", 0, 0.35f);
	}

	// Heighmap Template: Continents
	void templateContinents()
	{
		addMountain();
		modifyHeights("all", 10, 1.0f);
		addHill(30, 0.25f);
		int count = Mathf.CeilToInt(UnityEngine.Random.value * 4 + 4);
		addStrait(count);
		addPit(10);
		addRange(-10);
		modifyHeights("land", 0, 0.6f);
		smoothHeights(2);
		addRange(3);
	}

	// Heighmap Template: Archipelago
	void templateArchipelago()
	{
		addMountain();
		modifyHeights("all", 10, 1.0f);
		addHill(12, 0.15f);
		addRange(8);
		int count = Mathf.CeilToInt(UnityEngine.Random.value * 2 + 2);
		addStrait(count);
		addRange(-15);
		addPit(10);
		modifyHeights("land", -5, 0.7f);
		smoothHeights(3);
	}

	// Heighmap Template: Atoll
	void templateAtoll()
	{
		addMountain();
		modifyHeights("all", 10, 1.0f);
		addHill(2, 0.35f);
		addRange(2);
		smoothHeights(1);
		modifyHeights("27-100", 0, 0.1f);
	}

	// Heighmap Template: Mainland
	void templateMainland()
	{
		addMountain();
		modifyHeights("all", 10, 1.0f);
		addHill(30, 0.2f);
		addRange(10);
		addPit(20);
		addHill(10, 0.15f);
		addRange(-10);
		modifyHeights("land", 0, 0.4f);
		addRange(10);
		smoothHeights(3);
	}

	// Heighmap Template: Peninsulas
	void templatePeninsulas()
	{
		addMountain();
		modifyHeights("all", 15, 1.0f);
		addHill(30, 0);
		addRange(5);
		addPit(15);
		int count = Mathf.CeilToInt(UnityEngine.Random.value * 5 + 15);
		addStrait(count);
	}

	private void addMountain()
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

	// place with shift 0-0.5
	private void addHill(int count, float shift)
	{
		for (int c = 0; c < count; c++)
		{
			int limit = 0;
			uint cell;
			float height;

			do
			{
				height = UnityEngine.Random.value * 40 + 10; // 10-50
				float x = Mathf.Floor(UnityEngine.Random.value * graphWidth * (1 - shift * 2) + graphWidth * shift);
				float y = Mathf.Floor(UnityEngine.Random.value * graphHeight * (1 - shift * 2) + graphHeight * shift);
				Vector2 siteCoord = (Vector2) voronoi.NearestSitePoint(x, y);
				cell = siteIdsByCoord[siteCoord]; 
				limit++;
			} while (heights[(int) cell] + height > 90 && limit < 100);
	
			add(cell, "hill", height);
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
		if (type == "mountain")
		{
			heights[(int)start] = height;
		}
		for (int i = 0; i < queue.Count && height >= 1; i++)
		{
			if (type == "mountain") {
				height = heights[(int)queue[i]] * radius - height / 100;
			}
			else { height *= radius; }

			uint[] neighbors = sectors[(int)queue[i]].NeighborSectorIds;
			for(int j = 0; j < neighbors.Length; j++){
				int e = (int) neighbors[j];
				if (sectors[e].used == session)
				{
					break;
				}

				float mod = UnityEngine.Random.value * 0.2f + 0.9f; // 0.9-1.1 random factor
				heights[e] += height * mod;
				if (heights[e] > 100)
				{
					heights[(int)e] = 100;
				}
				sectors[e].used = session;
				queue.Add((uint)e);
			}
		}
	}

	private List<int> addRange(int mod, float height = -1, int from = -1, int to = -1)
	{
		int session = Mathf.CeilToInt(UnityEngine.Random.value * 100000);
		int count = Math.Abs(mod);
		List<int> range = new List<int>();
		for (int c = 0; c < count; c++)
		{
			range.Clear();
			range = new List<int>();
			float diff = 0;
			int start = from;
			int end = to;

			if (start == -1 || end == -1)
			{
				do
				{
					float xf = Mathf.Floor(UnityEngine.Random.value * (graphWidth * 0.7f)) + graphWidth * 0.15f;
					float yf = Mathf.Floor(UnityEngine.Random.value * (graphHeight * 0.6f)) + graphHeight * 0.2f;
					Vector2 startCoord = (Vector2)voronoi.NearestSitePoint(xf, yf);
					start = (int)siteIdsByCoord[startCoord];
					float xt = Mathf.Floor(UnityEngine.Random.value * (graphWidth * 0.7f)) + graphWidth * 0.15f;
					float yt = Mathf.Floor(UnityEngine.Random.value * (graphHeight * 0.6f)) + graphHeight * 0.2f;
					Vector2 endCoord = (Vector2)voronoi.NearestSitePoint(xt, yt);
					end = (int)siteIdsByCoord[endCoord];
					diff = Vector2.Distance(startCoord, endCoord);

				} while (diff < 150 / graphSize || diff > 300 / graphSize);
	  
			}

			if (start!= -1 && end != -1)
			{
				for (int l = 0; start != end && l < 10000; l++)
				{
					float min = 10000;
					uint[] neighbors = sectors[start].NeighborSectorIds;
					for(int j = 0; j < neighbors.Length; j++)
					{
						Sector e = sectors[(int)neighbors[j]];
						diff = Vector2.Distance(sectors[end].position, e.position);
						if (UnityEngine.Random.value > 0.8f)
						{
							diff = diff / 2;
						}

						if( diff < min)
						{
							min = diff;
							start = (int) neighbors[j];
						}
					}

					range.Add(start);
				}
			}

			float change = height != -1 ? height : UnityEngine.Random.Range(10.0f, 20.0f);
			for (int ri = 0; ri < range.Count; ri++)
			{
				int r = range[ri];
				float rnd = UnityEngine.Random.Range(0.4f, 1.2f);
				if (mod > 0)
				{
					heights[r] += change * rnd;
				}
				else if (heights[r] >= 10)
				{
					heights[r] -= change * rnd;
				}

				uint[] neighbors = sectors[start].NeighborSectorIds;
				for (int j = 0; j < neighbors.Length; j++)
				{
					Sector e = sectors[(int)neighbors[j]];
					if (e.used == session)
					{
						break;
					}

					e.used = session;
					rnd = UnityEngine.Random.Range(0.4f, 1.2f);
					float ch = change / 2 * rnd;

					if (mod > 0)
					{
						heights[(int)neighbors[j]] += ch;
					}
					else if (heights[(int)neighbors[j]] >= 10)
					{
						heights[(int)neighbors[j]] -= ch;
					}

					if (heights[(int)neighbors[j]] > 100) heights[(int)neighbors[j]] = mod > 0 ? 100 : 5;
				}

				if (heights[r] > 100) heights[r] = mod > 0 ? 100 : 5;
			}
			
		}

		return range;
	}

	private void addStrait(int width)
	{
		int session = Mathf.CeilToInt(UnityEngine.Random.value * 100000);
		float top = Mathf.Floor(UnityEngine.Random.value * graphWidth * 0.35f + graphWidth * 0.3f);
		float bottom = Mathf.Floor((graphWidth - top) - (graphWidth * 0.1f) + (UnityEngine.Random.value * graphWidth * 0.2f));
		Vector2 startCoord = (Vector2) voronoi.NearestSitePoint(top, graphHeight * 0.1f);
		int start = (int) siteIdsByCoord[startCoord];

		Vector2 endCoord = (Vector2)voronoi.NearestSitePoint(bottom, graphHeight * 0.9f);
		int end = (int) siteIdsByCoord[endCoord];
		
		List<int> range = new List<int>();
		for (int l = 0; start != end && l < 1000; l++)
		{
			float min = 10000; // dummy value

			uint[] neighbors = sectors[start].NeighborSectorIds;
			for (int j = 0; j < neighbors.Length; j++)
			{
				Sector e = sectors[(int)neighbors[j]];
				float diff = Vector2.Distance(e.position, sectors[end].position);

				if (UnityEngine.Random.value > 0.8f)
				{
					diff = diff / 2;
				}

				if (diff < min)
				{
					min = diff;
					start = (int)neighbors[j];
				}
			}

			range.Add(start);
		}

		List<int> query = new List<int>();

		for(; width > 0; width--)
		{

			range.ForEach(r => 
			//for (int ri = 0; ri < range.Count; ri++)
			{
			//	int r = range[ri];
				uint[] neighbors = sectors[r].NeighborSectorIds;

				for(int ni = 0; ni < neighbors.Length; ni++)
				{
					uint n = neighbors[ni];
					Sector neighbor = sectors[(int)n];
					if(neighbor.used == session)
					{
						break;
					}

					neighbor.used = session;
					query.Add((int)n);
					heights[(int)n] *= 0.23f;

					if(heights[(int)n] > 100 || heights[(int)n] < 5)
					{
						heights[(int)n] = 5;
					}
				}

				range = query;
			}
			);
		}

	}

	private void addPit(int count, float height = -1, int cell = -1)
	{
		int session = Mathf.CeilToInt(UnityEngine.Random.value * 100000);

		for (int c = 0; c < count; c++)
		{
			float change = height != -1 ? height + 10 : UnityEngine.Random.Range(10.0f, 30.0f);
			int start = cell;
			if (start == - 1)
			{
				//get all index where heights[i] >= 20 const lowlands = $.grep(cells, function(e) { return (heights[e.index] >= 20);});
				List<int> lowlands = heights.ToArray().Select((x, index) => index).Where(x => x >= 20).ToList();
				if (lowlands.Count == 0) return;
				int rnd = Mathf.FloorToInt(UnityEngine.Random.value * lowlands.Count);
				start = lowlands[rnd];
			}

			List<int> query = new List<int>(new int[] { start });
			List<int> newQuery = new List<int>();

			// depress pit center
			heights[start] -= change;
			if (heights[start] < 5 || heights[start] > 100) {
				heights[start] = 5;
			}

			sectors[start].used = session;

			for (int i = 1; i< 10000; i++) {
				float rnd = UnityEngine.Random.Range(0.4f, 1.2f);
				change -= i / 0.6f * rnd;
				if (change < 1) {
					break;
				}
				foreach( int p in query) {
					uint[] neighbors = sectors[p].NeighborSectorIds;
					foreach( uint e in neighbors) {
						if (sectors[(int)e].used == session) break;
						sectors[(int)e].used = session;
						if (UnityEngine.Random.value > 0.8f) break;
						newQuery.Add((int)e);
						heights[(int)e] -= change;
						if (heights[(int)e] < 5 || heights[(int)e] > 100)
						{
							heights[(int)e] = 5;
						}
					}
				}
				query = newQuery;
				newQuery = new List<int>();
			}
		}
	}

	private void modifyHeights(string range, float add, float mult)
	{
		int limMin = range == "land" ? 20 : range == "all" ? 0 : Int32.Parse(range.Split('-')[0]);
		int limMax = range == "land" || range == "all" ? 100 : Int32.Parse(range.Split('-')[1]);

		for (int i = 0; i < heights.Count; i++)
		{
			if (heights[i] < limMin || heights[i] > limMax) continue;
			heights[i] = modify(range, add, mult, heights[i]);
		}
	}

	private float modify(string range, float add, float mult, float v)
	{
		if (add != 0) v += add;
		if (mult != 1)
		{
			//if (mult === "^2") mult = (v - 20) / 100;
			//if (mult === "^3") mult = ((v - 20) * (v - 20)) / 100;
			if (range == "land") { v = 20 + (v - 20) * mult; }
			else { v *= mult; }
		}
		if (v < 0) v = 0.0f;
		if (v > 100) v = 100.0f;
		return v;
	}

	// Smooth heights using mean of neighbors
	private void smoothHeights(int fraction = 2)
	{
		for (int i = 0; i < heights.Count; i++)
		{
			List<float> nHeights = new List<float>();
			nHeights.Add(heights[i]);
			for(int j = 0; j < sectors[i].NeighborSectorIds.Length; j++)
			{
				nHeights.Add(heights[j]);
			}

			//cells[i].neighbors.forEach(function(e) { nHeights.push(heights[e]); });
			heights[i] = (heights[i] * (fraction - 1) + nHeights.Average()) / fraction;
		}
	}

	// Randomize heights a bit
	private void disruptHeights()
	{
		for (int i = 0; i < heights.Count; i++)
		{
			if (heights[i] < 18) continue;
			if (UnityEngine.Random.value < 0.5) continue;
			heights[i] += UnityEngine.Random.Range(-2.0f, 2.0f);
		}
	}

	private void markFeatures()
	{
		Debug.Log("markFeatures");
		float time = Time.time;

		UnityEngine.Random.InitState(seed);

		features = new List<Feature>();

		Queue<int> queue = new Queue<int>();
		queue.Enqueue(0);

		for (int i = 0; queue.Count > 0; i++)
		{
			Sector sector = sectors[queue.ElementAt(0)];
			sector.featureNumber = i;
			bool isLand = (heights[queue.ElementAt(0)] >= 20);
			bool isBorder = (sector.type == "border");
			if(isBorder && isLand)
			{
				sector.cType = 2;
			}

			while(queue.Count > 0)
			{
				int q = queue.Dequeue();
				if(sectors[q].type == "border")
				{
					isBorder = true;
					if (isLand)
					{
						sectors[q].cType = 2;
					}
				}

				uint[] neighbors = sectors[q].NeighborSectorIds;
				foreach(uint e in neighbors)
				{
					bool eLand = (heights[(int)e] >= 20);
					if(isLand == eLand && sectors[(int)e].featureNumber == -1)
					{
						sectors[(int)e].featureNumber = i;
						queue.Enqueue((int)e);
					}

					if(isLand && !eLand)
					{
						sectors[q].cType = 2;
						sectors[(int)e].cType = -1;
						if( sectors[q].harbor != null)
						{
							sectors[q].harbor++;
						} else
						{
							sectors[q].harbor = 1;
						}
					}
				}
			}

			Feature f = new Feature();
			f.i = i;
			f.border = isBorder;
			f.land = isLand;
			features.Add(f);

			for (int c = 0; c < sectors.Count; c++)
			{
				if (sectors[c].featureNumber == -1)
				{
					queue.Enqueue(c);
					break;
				}
			}
		}

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/markFeatures");
	}

	private void drawOcean()
	{
		Debug.Log("drawOcean");
		float time = Time.time;

		List<int> limits = new List<int>();
		float odd = 0.8f; // initial odd for ocean layer is 80%

		// Define type of ocean cells based on cell distance form land
		//let frontier = $.grep(cells, function(e) { return e.ctype === -1; });
		List<Sector> frontier = (List<Sector>) sectors.Where(s => s.cType == -1);
		if (UnityEngine.Random.value < odd) { limits.Add(-1); odd = 0.2f; }
		for (int c = -2; frontier.Count > 0 && c > -10; c--)
		{
			if (UnityEngine.Random.value < odd) { limits.Insert(0, c); odd = 0.2f; } else { odd += 0.2f; }
			foreach(Sector i in frontier) {
				uint[] neighbors = i.NeighborSectorIds;
				foreach(uint e in neighbors){
					if (sectors[(int)e].cType == null) sectors[(int)e].cType = c;
				}
			}
			//frontier = $.grep(cells, function(e) { return e.ctype === c; });
			frontier = (List<Sector>)sectors.Where(s => s.cType == c);
		}
		//don't draw anything here. We'll set sprites later based on cType
			   
		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/drawOcean");
	}

	private void elevateLakes()
	{
		Debug.Log("elevateLakes");
		float time = Time.time;

		//const lakes = $.grep(cells, function(e, d) { return heights[d] < 20 && !features[e.fn].border; });
		List<Sector> lakes = sectors.Where(s => heights[(int)s.Id] < 20 && !features[s.featureNumber].border).ToList();

		//lakes.sort(function(a, b) { return heights[b.index] - heights[a.index]; });
		lakes = lakes.OrderByDescending(l => heights[(int)l.Id]).ToList();
		for (int i = 0; i < lakes.Count; i++)
		{
			List<float> hs = new List<float>();
			uint id = lakes[i].Id;
			sectors[(int)id].height = heights[(int)id]; // use height on object level
			uint[] neighbors = lakes[i].NeighborSectorIds;
			foreach(uint n in neighbors){
				float nHeight = sectors[(int)n].height != null ? sectors[(int)n].height : heights[(int)n];
				if (nHeight >= 20) hs.Add(nHeight);
			}
			if (hs.Count > 0) sectors[(int)id].height = hs.Max() - 1;
			if (sectors[(int)id].height < 20) sectors[(int)id].height = 20;
			lakes[i].lake = 1;
		}

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/elevateLakes");
	}

	private void resolveDepressionsPrimary()
	{

		Debug.Log("resolveDepressionsPrimary");
		float time = Time.time;


		land = sectors.Select( s => (int)s.Id ).Where(s => heights[s] >= 20).ToList();

	    land = land.OrderByDescending( s => heights[s]).ToList();

	    int limit = 10;
	    int depression = 1;
		for (int l = 0; depression > 0 && l < limit; l++) {
		    depression = 0;

		    for (int i = 0; i < land.Count; i++) {
		        int id = land[i];
		        if (sectors[id].type == "border") continue;
		        List<float> hs = neighborHeights(sectors[id]); //land[i].neighbors.map(function(n) {return cells[n].height;});
		        float minHigh = hs.Min();
		        if (sectors[id].height <= minHigh) {
			        depression++;
		    	    sectors[id].pit = sectors[id].pit != null? sectors[id].pit + 1 : 1;
		        	sectors[id].height = minHigh + 2;
		        }
		      }
		      if (l == 0) Debug.Log(" depressions init: " + depression);
		    }

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/resolveDepressionsPrimary");
	}

	private List<float> neighborHeights(Sector sector)
	{
		List<float> hs = new List<float>(sector.NeighborSectorIds.Length);
		uint[] neighbors = sector.NeighborSectorIds;

		foreach(uint n in neighbors)
		{
			hs.Add(sectors[(int)n].height);
		}

		return hs;
	}

	private void reGraph()
	{
		Debug.Log("reGraph");
		float time = Time.time;

		//List<Sector> tempCells = new List<Sector>();
		//List<Vector2> newPoints = new List<Vector2>(); // to store new data
		
		// get average precipitation based on graph size
		float avPrec = precInput / 5000;
		int smallLakesMax = 500;
		int smallLakes = 0;
		int evaporation = 2;

		foreach(Sector i in sectors){
			int d = (int)i.Id;
			float height = i.height != null ? i.height : heights[d];
			if (height > 100) height = 100;
			int? pit = i.pit;
			int ctype = i.cType;
			if (ctype != -1 && ctype != -2 && height < 20) break; // exclude all deep ocean points
			Vector2 coord = i.position;
			coord.x = rn(coord.x, 2);
			coord.y = rn(coord.y, 2);

			int featureNumber = i.featureNumber;
			int? harbor = i.harbor;
			int? lake = i.lake;
			// mark potential cells for small lakes to add additional point there
			if (smallLakes < smallLakesMax && lake == null && pit > evaporation && ctype != 2)
			{
				i.lake = 2;
				smallLakes++;
			}
			//const region = i.region; // handle value for edit heightmap mode only
			//const culture = i.culture; // handle value for edit heightmap mode only
			//let copy = $.grep(newPoints, function(e) { return (e[0] == x && e[1] == y); });
			
			//NOTE: skip creating new sectors along coasts. just update existing sector's coord

			i.position = coord;

			/*List<Vector2> copy = newPoints.Where(v => v.x == coord.x && v.y == coord.y).ToList();
			if (copy.Count == 0)
			{
				newPoints.Add(coord);

				//tempCells.push({ index: tempCells.length, data:[x, y], height, pit, ctype, fn, harbor, lake, region, culture});
				Sector newSector = new Sector();
				newSector.Id = Convert.ToUInt32(tempCells.Count);
				newSector.position = coord;
				newSector.height = height;
				newSector.pit = pit;
				newSector.cType = ctype;
				newSector.featureNumber = featureNumber;
				newSector.harbor = harbor;
				newSector.lake = lake;
				
				tempCells.Add(newSector);
			}*/

			// add additional points for cells along coast
			/*if (ctype == 2 || ctype == -1)
			{
				if (i.type == "border") break;
				if (!features[featureNumber].land && !features[featureNumber].border) break;

				uint[] neighbors = i.NeighborSectorIds;
				foreach(uint e in neighbors) { 

					if (sectors[(int)e].cType == ctype)
					{
						float x1 = (coord.x * 2 + sectors[(int)e].position.x) / 3;
						float y1 = (coord.y * 2 + sectors[(int)e].position.y) / 3;
						x1 = rn(x1, 1);
						y1 = rn(y1, 1);
						//copy = $.grep(newPoints, function(e) { return e[0] === x1 && e[1] === y1; });
						copy = newPoints.Where(v => v.x == x1 && v.y == y1).ToList();
						if (copy.Count > 0) break;
						newPoints.Add( new Vector2(x1, y1));

						//tempCells.push({ index: tempCells.length, data:[x1, y1], height, pit, ctype, fn, harbor, lake, region, culture});
						Sector newSector = new Sector();
						newSector.Id = Convert.ToUInt32(tempCells.Count);
						newSector.position = new Vector2(x1, y1);
						newSector.height = height;
						newSector.pit = pit;
						newSector.cType = ctype;
						newSector.featureNumber = featureNumber;
						newSector.harbor = harbor;
						newSector.lake = lake;

						tempCells.Add(newSector);

					}
				}

			}*/

			// add potential small lakes
			/*if (lake == 2)
			{
				List<Vector2> polys = polygons[i.Id];
				foreach(Vector2 e in polys){
					if (UnityEngine.Random.value > 0.8f) break;
					float rnd = UnityEngine.Random.Range(0.6f, 1.4f);
					float x1 = rn((e.x * rnd + i.position.x) / (1 + rnd), 2);
					rnd = UnityEngine.Random.Range(0.6f, 1.4f);
					float y1 = rn((e.y * rnd + i.position.y) / (1 + rnd), 2);

					//copy = $.grep(newPoints, function(c) { return x1 === c[0] && y1 === c[1]; });
					copy = newPoints.Where(v => v.x == x1 && v.y == y1).ToList();
					if (copy.Count > 0) break;

					newPoints.Add(new Vector2(x1, y1));
					//tempCells.push({ index: tempCells.length, data:[x1, y1], height, pit, ctype, fn, region, culture});

					Sector newSector = new Sector();
					newSector.Id = Convert.ToUInt32(tempCells.Count);
					newSector.position = new Vector2(x1, y1);
					newSector.height = height;
					newSector.pit = pit;
					newSector.cType = ctype;
					newSector.featureNumber = featureNumber;
					newSector.harbor = harbor;
					newSector.lake = lake;

					tempCells.Add(newSector);
				}
			}*/
		} // end of foreach(Sector i in sectors)

		Debug.Log("small lakes candidates: " + smallLakes);
		//sectors = tempCells; // use tempCells as the only cells array
		//calculateVoronoi(newPoints); // recalculate Voronoi diagram using new points
		
		//let gridPath = ""; this was for showing the grid outline 
		//cells.map(function(i, d) {
		foreach(Sector i in sectors) {
			
			uint siteId = siteIdsByCoord[i.position];
			Delaunay.Site site = sites[(int)siteId];

			i.mapPoly = polygons[siteId];

			uint ud = i.Id;
			int d = (int)i.Id;
			if (i.height >= 20)
			{
				// calc cell area
				i.area = rn(Mathf.Abs(GetArea(i.mapPoly)), 2);
				float prec = rn(avPrec * i.area, 2);
				i.flux = i.lake != null ? prec * 10 : prec;
			}
			//uint[] neighbors = []; // re-detect neighbors

			//List<Delaunay.Edge> edges = site.edges;
			//List<uint> neighborIds = new List<uint>(edges.Count);
			//List<float> neighborDists = new List<float>(edges.Count);
			////for(int j = 0; j < neighbors.Count; j++)
			////{
			////	neighborIds[j] = neighbors[j].getIndex();
			////}
			//for (int j = 0; j < edges.Count; j++)
			//{
			//	Delaunay.Edge edge = edges[j];
			//	if(edge.leftSite != null && edge.rightSite != null)
			//	{
			//		if (edge.leftSite.getIndex() == sectorId)
			//		{
			//			neighborIds.Add(edge.rightSite.getIndex());
			//			neighborDists.Add(Vector2.Distance(position, edge.rightSite.Coord));
			//		} else
			//		{
			//			neighborIds.Add(edge.leftSite.getIndex());
			//			neighborDists.Add(Vector2.Distance(position, edge.leftSite.Coord));
			//		}
			//	} else if (i.height >= 20)
			//	{
			//		i.cType = 99;
			//		break;
			//	}
			//}
				
			//	//if (d < ea && i.height >= 20 && i.lake !== 1 && cells[ea].height >= 20 && cells[ea].lake !== 1)
			//	//{
			//	//	gridPath += "M" + edge[0][0] + "," + edge[0][1] + "L" + edge[1][0] + "," + edge[1][1];
			//	//}
			
			//i.neighborIds = neighborId;
			//i.neighborDists = neighborDists;


		} //end foreach (Sector i in sectors)

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/reGraph");
	}

	//from https://stackoverflow.com/questions/17775832/c-sharp-gmap-net-calculate-surface-of-polygon#17775964
	float GetArea(List<Vector2> points)
	{
		float area2 = 0;
		for (int numPoint = 0; numPoint < points.Count - 1; numPoint++)
		{
			Vector2 point = points[numPoint];
			Vector2 nextPoint = points[numPoint + 1];
			area2 += point.x * nextPoint.y - point.y * nextPoint.x;
		}
		return area2 / 2f;
	}

	private void resolveDepressionsSecondary()
	{
		Debug.Log("resolveDepressionsSecondary");
		float time = Time.time;

		land = sectors.Select(s => (int)s.Id).Where(s => heights[s] >= 20).ToList();

		land = land.OrderByDescending(s => heights[s]).ToList();

		int limit = 100;
		int depression = 1;
		for (int l = 0; depression > 0 && l < limit; l++)
		{
			depression = 0;
			for (int index = 0; index < land.Count; index++)
			{
				Sector i = sectors[land[index]];

				if (i.cType == 99) continue;
				List<float> nHeights = neighborHeights(i);
				float minHigh = nHeights.Min();
				if (i.height <= minHigh)
				{
					depression++;
					i.pit = i.pit != null ? i.pit + 1 : 1;
					i.height = Mathf.Floor(minHigh + 2);
				}
			}

			if (l == 0) Debug.Log(" depressions reGraphed: " + depression);
			if (l == limit - 1) Debug.LogError("Error: resolveDepressions iteration limit");
		}

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/resolveDepressionsSecondary");
	}

	private void flux()
	{
		Debug.Log("flux");
		float time = Time.time;

		List<RiverData> riversData = new List<RiverData>();
		
		int riverNext = 0;

		land = land.OrderByDescending(s => heights[s]).ToList();

		for (int index = 0; index < land.Count; index++)
		{
			Sector i = sectors[land[index]];
			int id = (int)i.Id;
			Vector2 position = i.position; //sx and xy
			int featureNumber = i.featureNumber;
			if (i.cType == 99)
			{
				if (i.river != null)
				{
					Vector2 pos = new Vector2(0.0f, 0.0f);
					float minP = Mathf.Min(new float[] { position.y, graphHeight - position.y, position.x, graphWidth - position.x });
					if (minP == position.y) { pos.x = position.x; pos.y = 0; }
					if (minP == graphHeight - position.y) { pos.x = position.x; pos.y = graphHeight; }
					if (minP == position.x) { pos.x = 0; pos.y = position.y; }
					if (minP == graphWidth - position.x) { pos.x = graphWidth; pos.y = position.y; }
					riversData.Add(new RiverData((int)i.river, id, pos));
				}
				continue;
			}

			if (features[featureNumber].river != null)
			{
				if (i.river != features[featureNumber].river)
				{
					i.river = null;
					i.flux = 0;
				}
			}

			int min = -1;

			List<float> nHeights = neighborHeights(i);
			float minHigh = nHeights.Min();
			foreach (uint n in i.NeighborSectorIds)
			{
				if (sectors[(int)n].height == minHigh)
				{
					min = (int)n;
				}
			}

			// Define river number
			if (min != -1 && i.flux > 1)
			{
				if (i.river == null)
				{
					// State new River
					i.river = riverNext;
					RiverData r = new RiverData();
					r.river = riverNext;
					r.siteId = id;
					r.position = position;
					riversData.Add(r);
					riverNext++;
				}
				// Assing existing River to the downhill cell
				if (sectors[min].river == null)
				{
					sectors[min].river = i.river;
				}
				else
				{
					int riverTo = (int)sectors[min].river;
					//$.grep(riversData, function(e) { return (e.river == land[i].river); });
					List<RiverData> iRiver = (List<RiverData>)riversData.Where(r => r.river == i.river);

					//$.grep(riversData, function(e) { return (e.river == riverTo); });
					List<RiverData> minRiver = (List<RiverData>)riversData.Where(r => r.river == riverTo);
					int iRiverL = iRiver.Count;
					int minRiverL = minRiver.Count;
					// re-assing river nunber if new part is greater
					if (iRiverL >= minRiverL)
					{
						sectors[min].river = i.river;
						iRiverL += 1;
						minRiverL -= 1;
					}

					// mark confluences
					if (sectors[min].height >= 20 && iRiverL > 1 && minRiverL > 1)
					{
						if (sectors[min].confluence == null)
						{
							sectors[min].confluence = minRiverL - 1;
						}
						else
						{
							sectors[min].confluence += minRiverL - 1;
						}
					}
				}
			}

			//if (sectors[min].flux) 
			sectors[min].flux += i.flux;
			if (i.river != null)
			{
				//const px = cells[min].data[0];
				//const py = cells[min].data[1];
				Vector2 pPos = sectors[min].position;
				if (sectors[min].height < 20)
				{
					// pour water to the sea
					//const x = (px + sx) / 2 + (px - sx) / 10;
					//const y = (py + sy) / 2 + (py - sy) / 10;
					Vector2 nPos = new Vector2((position.x + pPos.x) / 2, (position.y + pPos.y) / 2);
					RiverData rNew = new RiverData();
					rNew.river = (int)i.river;
					rNew.siteId = id;
					rNew.position = nPos;
					riversData.Add(rNew); // {river: land[i].river, cell: id, x, y});
				}
				else
				{
					if (sectors[min].lake == 1)
					{
						featureNumber = sectors[min].featureNumber;
						Feature f = features[featureNumber];

						if (f.river == null)
						{
							f.river = i.river;
							features[featureNumber] = f;
						}

						// add next River segment
						RiverData rNew = new RiverData();
						rNew.river = (int)i.river;
						rNew.siteId = id;
						rNew.position = pPos;
						riversData.Add(rNew); //{ river: land[i].river, cell: min, x: px, y: py});
					}
				}
			}
		}

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/flux");
	}

	private void addLakes()
	{
		Debug.Log("addLakes");
		float time = Time.time;

		int smallLakes = 0;
		for (int index = 0; index < land.Count; index++)
		{
			Sector i = sectors[land[index]];
			// elavate all big lakes
			if (i.lake == 1)
			{
				i.height = 19;
				i.cType = -1;
			}
			// define eligible small lakes
			if (i.lake == 2 && smallLakes < 100)
			{
				if (i.river != null)
				{
					i.height = 19;
					i.cType = -1;
					i.featureNumber = -1;
					smallLakes++;
				}
				else
				{
					i.lake = null;
					uint[] neighbors = i.NeighborSectorIds;
					foreach(uint n in neighbors)
					{ //i.neighbors.forEach(function(n) 
						if (sectors[(int)n].lake != 1 && sectors[(int)n].river != null)
						{
							sectors[(int)n].lake = 2;
							sectors[(int)n].height = 19;
							sectors[(int)n].cType = -1;
							sectors[(int)n].featureNumber = -1;
							smallLakes++;
						}
						else if (sectors[(int)n].lake == 2)
						{
							sectors[(int)n].lake = null;
						}
					}
				}
			}
		}
    Debug.Log( "small lakes: " + smallLakes);

		// mark small lakes
		List<int> unmarked = land.Where(e => sectors[e].featureNumber == -1).ToList(); //, function(e) { return e.fn === -1});

		while (unmarked.Count > 0) {

			int fn = -1;
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(unmarked[0]);
			List<int> lakeCells = new List<int>();//[unmarked[0].index], lakeCells = [];
			sectors[unmarked[0]].used = 999999; //"addLakes";
			while (queue.Count > 0) {
				int q = queue.Dequeue();
				lakeCells.Add(q);
				if (sectors[q].featureNumber != -1) fn = sectors[q].featureNumber;

				foreach(uint e in sectors[q].NeighborSectorIds){ //.forEach(function(e) {
					if (sectors[(int)e].lake != null && sectors[(int)e].used != 999999) {
						sectors[(int)e].used = 999999;

						queue.Enqueue((int)e);
					}
				}
			}

			if (fn == -1) {
				fn = features.Count;
				Feature f = new Feature();
				f.i = fn;
				f.land = false;
				f.border = false;
				features.Add(f); // {i: fn, land: false, border: false});
			}

			foreach (int c in lakeCells) //.forEach(function(c) {cells[c].fn = fn;});
			{
				sectors[c].featureNumber = fn;			
			}

			unmarked = land.Where(e => sectors[e].featureNumber == -1).ToList(); //unmarked = $.grep(land, function(e) { return e.fn === -1});
		}

		land = sectors.Select(s => (int)s.Id).Where(s => heights[s] >= 20).ToList();

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/addLakes");
	}

	private void generateCultures()
	{
		Debug.Log("generateCultures");
		float time = Time.time;

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/generateCultures");
	}

	// calculate population for manors, cells and states
	private void calculatePopulation()
	{
		Debug.Log("calculatePopulation");
		float time = Time.time;

		// neutral population factors < 1 as neutral lands are usually pretty wild
		//float ruralFactor = 0.5f, 
		float urbanFactor = 0.9f;

		// calculate population for each burg (based on trade/people attractors)
		//foreach(Manor m in manors){ //.map(function(m) {
		for(int i = 0; i < manors.Count; i++) {
			Manor m = manors[i];
			Sector s = sectors[m.i];
			float score = s.score == null ? 0.0f : (float)s.score;
			if (score <= 0) { score = rn(UnityEngine.Random.value, 2); }
			if (s.crossroad != null) { score += (int)s.crossroad; } // crossroads
			if (s.confluence != null) { score += Mathf.Pow((int)s.confluence, 0.3f); } // confluences
			if (m.i != m.region && s.port != null) { score *= 1.5f; } // ports (not capital)
			if (m.i == m.region && s.port == null) { score *= 2; } // land-capitals
			if (m.i == m.region && s.port != null) { score *= 3; } // port-capitals
			if (m.region == -1) score *= urbanFactor;
			float rnd = UnityEngine.Random.Range(0.6f, 1.4f); // + Math.random() * 0.8; // random factor
			m.population = Mathf.FloorToInt(score * rnd); //rn(score * rnd, 0);
		}

		// calculate rural population for each cell based on area + elevation (elevation to be changed to biome)
		float graphSizeAdj = 90 / Mathf.Sqrt(sectors.Count); // adjust to different graphSize
		//land.map(function(l) {
		for(int i = 0; i < land.Count; i++){
			Sector l = sectors[land[i]];
			int population = 0;
			float elevationFactor = Mathf.Pow(1 - l.height / 100, 3);
			population = Mathf.FloorToInt(elevationFactor * l.area * graphSizeAdj);
			//if (l.region == "neutral") population *= ruralFactor;
			l.population = population;
		}

		// calculate population for each region
		//states.map(function(s, i) {
		//	// define region burgs count
		//	var burgs = $.grep(manors, function(e) { return e.region === i; });
		//	s.burgs = burgs.length;
		//	// define region total and burgs population
		//	var burgsPop = 0; // get summ of all burgs population
		//	burgs.map(function(b) { burgsPop += b.population; });
		//	s.urbanPopulation = rn(burgsPop, 2);
		//	var regionCells = $.grep(cells, function(e) { return e.region === i; });
		//	let cellsPop = 0;
		//	regionCells.map(function(c) { cellsPop += c.pop});
		//	s.cells = regionCells.length;
		//	s.ruralPopulation = rn(cellsPop, 1);
		//});

		// collect data for neutrals
		//const neutralCells = $.grep(cells, function(e) { return e.region === "neutral"; });
		//if (neutralCells.length)
		//{
		//	let burgs = 0, urbanPopulation = 0, ruralPopulation = 0, area = 0;
		//	manors.forEach(function(m) {
		//		if (m.region !== "neutral") return;
		//		urbanPopulation += m.population;
		//		burgs++;
		//	});
		//	neutralCells.forEach(function(c) {
		//		ruralPopulation += c.pop;
		//		area += cells[c.index].area;
		//	});
		//	states.push({
		//	i: states.length, color: "neutral", name: "Neutrals", capital: "neutral",
  //      cells: neutralCells.length, burgs, urbanPopulation: rn(urbanPopulation, 2),
  //      ruralPopulation: rn(ruralPopulation, 2), area: rn(area)});
		//}

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/generateCultures");
	}

	private void drawCoastline()
	{
		throw new NotImplementedException();

		Debug.Log("drawOcean");
		float time = Time.time;
		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/drawOcean");
	}

	private void cleanData()
	{
		throw new NotImplementedException();

		Debug.Log("drawOcean");
		float time = Time.time;
		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/drawOcean");
	}

	private void manorsAndRegions()
	{
		throw new NotImplementedException();

		Debug.Log("manorsAndRegions");
		float time = Time.time;

	    //calculateChains();
	    rankPlacesGeography();
	    locateCapitals();
	    generateMainRoads();
	    rankPlacesEconomy();
	    locateTowns();
	    getNames();
	    shiftSettlements();
	    checkAccessibility();
	    defineRegions("withCultures");
	    generatePortRoads();
	    generateSmallRoads();
	    generateOceanRoutes();
	    calculatePopulation();
	    drawManors();
	    drawRegions();

		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/manorsAndRegions");
	}

	// Assess cells geographycal suitability for settlement
  	private void rankPlacesGeography() {
		Debug.Log("rankPlacesGeography");
		float time = Time.time;
		foreach (int cId in land) {
			Sector c = sectors[cId];
			float score = 0;
			c.flux = rn(c.flux, 2);
			// get base score from height (will be biom)
			if (c.height <= 40) score = 2;
			else if (c.height <= 50) score = 1.8f;
			else if (c.height <= 60) score = 1.6f;
			else if (c.height <= 80) score = 1.4f;
			score += (1 - c.height / 100) / 3;
			if (c.cType != null && UnityEngine.Random.value < 0.8f && c.river == null) {
				c.score = 0; // ignore 80% of extended cells
			} else {
				if (c.harbor != null) {
					if (c.harbor == 1) { score += 1; } else { score -= 0.3f; } // good sea harbor is valued
				}

				if (c.river != null) score += 1; // coastline is valued
				if (c.river != null && c.cType == 1) score += 1; // estuary is valued
				if (c.flux > 1) score += Mathf.Pow(c.flux, 0.3f); // riverbank is valued
				if (c.confluence != null) score += Mathf.Pow((int)c.confluence, 0.7f); // confluence is valued;
																					   //const neighbEv = c.neighbors.map(function(n) {if (cells[n].height >= 20) return cells[n].height;})
																					   //const difEv = c.height - d3.mean(neighbEv);
																					   // if (!isNaN(difEv)) score += difEv * 10 * (1 - c.height / 100); // local height maximums are valued
			}

			c.score = rn((UnityEngine.Random.value + 1.0f) * score, 3); // add random factor
		}

		land = land.OrderByDescending(s => heights[s]).ToList();
		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/rankPlacesGeography");
	}

	private void locateCapitals()
	{
		Debug.Log("locateCapitals");
		float time = Time.time;
		time = Time.time - time;

		// min distance detween capitals
		int count = numRegions;
		float spacing = (graphWidth + graphHeight) / 2 / count;
		Debug.Log(" states: " + count);

		for (int l = 0; manors.Count < count; l++)
		{
			int region = manors.Count;
			//const x = land[l].data[0], y = land[l].data[1];
			Vector2 pos = sectors[land[l]].position;
			float minDist = 10000; // dummy value
			for (int c = 0; c < manors.Count; c++)
			{
				float dist = Vector2.Distance(sites[manors[c].siteId].Coord, pos);
				if (dist < minDist) minDist = dist;
				if (minDist < spacing) break;
			}
			if (minDist >= spacing)
			{
				//const cell = land[l].index;
				//const closest = cultureTree.find(x, y);
				//const culture = getCultureId(closest);
				Manor m = new Manor();
				m.i = region;
				m.region = region;
				m.siteId = land[l];
				manors.Add(m); //{ i: region, cell, x, y, region, culture});
			}
			if (l == land.Count - 1) {
				Debug.LogError("Cannot place capitals with current spacing. Trying again with reduced spacing");
				l = -1;
				manors = new List<Manor>();
				spacing /= 1.2f;
			}
		}

		// For each capital create a country
		//const scheme = count <= 8 ? colors8 : colors20;
		//const mod = +powerInput.value;
		//manors.forEach(function(m, i) {
		//const power = rn(Math.random() * mod / 2 + 1, 1);
		//const color = scheme(i / count);
		//states.push({i, color, power, capital: i});
		//const p = cells[m.cell];
		//p.manor = i;
		//p.region = i;
		//p.culture = m.culture;
		//});
		Debug.Log("Time: " + time);
		Debug.Log("/drawRelief");
	}

	private void generateMainRoads()
	{
		Debug.Log("generateMainRoads");
		float time = Time.time;
		time = Time.time - time;

		//lineGen.curve(d3.curveBasis);
		if (manors.Count < 2) return;

		for (int f = 0; f < features.Count; f++)
		{
			if (!features[f].land) continue;

			//const manorsOnIsland = $.grep(land, function(e) { return e.manor !== undefined && e.fn === f; });
			List<int> manorsOnIsland = land.Where(e => sectors[e].manor != null && sectors[e].featureNumber == f).ToList();
			//List<S> manorsOnIsland = manors.Select()
			if (manorsOnIsland.Count > 1)
			{
				for (int d = 1; d < manorsOnIsland.Count; d++)
				{
					for (int m = 0; m < d; m++)
					{
						List<int> path = findLandPath(manorsOnIsland[d], manorsOnIsland[m], "main");
						restorePath(manorsOnIsland[m].index, manorsOnIsland[d].index, "main", path);
					}
				}
			}
		}

		Debug.Log("Time: " + time);
		Debug.Log("/generateMainRoads");
	}

	private List<int> findLandPath(int start, int end, string type)
	{
		// A* algorithm
		PriorityQueue queue = new Queue();// PriorityQueue({ comparator: function(a, b) { return a.p - b.p} });
		List<int> cameFrom = new List<int>();
		List<float> costTotal = new List<float>();
		costTotal[start] = 0;
		queue.Enqueue(.queue({e: start, p: 0});
    while (queue.length > 0) {
      var next = queue.dequeue().e;
      if (next === end) {break;}
      var pol = cells[next];
pol.neighbors.forEach(function(e) {
        if (cells[e].height >= 20) {
          var cost = cells[e].height / 100 * 2;
          if (cells[e].path && type === "main") {
            cost = 0.15;
          } else {
            if (typeof e.manor === "undefined") {cost += 0.1;}
            if (typeof e.river !== "undefined") {cost -= 0.1;}
            if (cells[e].harbor) {cost *= 0.3;}
            if (cells[e].path) {cost *= 0.5;}
            cost += Math.hypot(cells[e].data[0] - pol.data[0], cells[e].data[1] - pol.data[1]) / 30;
          }
          var costNew = costTotal[next] + cost;
          if (!cameFrom[e] || costNew<costTotal[e]) { //
            costTotal[e] = costNew;
            cameFrom[e] = next;
            var dist = Math.hypot(cells[e].data[0] - cells[end].data[0], cells[e].data[1] - cells[end].data[1]) / 15;
var priority = costNew + dist;
queue.queue({e, p: priority});
          }
        }
      });
    }
    return cameFrom;
  }

	function restorePath(end, start, type, from)
	{
		var path = [], current = end, limit = 1000;
		var prev = cells[end];
		if (type === "ocean" || !prev.path) { path.push({ scX: prev.data[0], scY: prev.data[1], i: end}); }
		if (!prev.path) { prev.path = 1; }
		for (let i = 0; i < limit; i++)
		{
			current = from[current];
			var cur = cells[current];
			if (!cur) { break; }
			if (cur.path)
			{
				cur.path += 1;
				path.push({ scX: cur.data[0], scY: cur.data[1], i: current});
		prev = cur;
		drawPath();
	} else {
        cur.path = 1;
        if (prev) {path.push({scX: prev.data[0], scY: prev.data[1], i: prev.index
});}
        prev = undefined;
        path.push({scX: cur.data[0], scY: cur.data[1], i: current});
      }
      if (current === start || !from[current]) {break;}
    }
    
  }

private void drawRelief()
	{
		throw new NotImplementedException();

		Debug.Log("drawRelief");
		float time = Time.time;
		time = Time.time - time;
		Debug.Log("Time: " + time);
		Debug.Log("/drawRelief");
	}


}