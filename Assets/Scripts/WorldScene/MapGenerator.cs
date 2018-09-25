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
			newSector.type = type;
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

					//cells[start].neighbors.forEach(function(e) {
					//	diff = Math.hypot(cells[end].data[0] - cells[e].data[0], cells[end].data[1] - cells[e].data[1]);
					//	if (Math.random() > 0.8) diff = diff / 2;
					//	if (diff < min) { min = diff, start = e; }
					//});
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
				//	range.map(function(r) {
				//	let rnd = Math.random() * 0.4 + 0.8;
				//	if (mod > 0) heights[r] += change* rnd;
				//	else if (heights[r] >= 10) {heights[r] -= change* rnd;}
				//	cells[r].neighbors.forEach(function(e) {
				//		if (cells[e].used === session) return;
				//		cells[e].used = session;
				//		rnd = Math.random() * 0.4 + 0.8;
				//		const ch = change / 2 * rnd;
				//		if (mod > 0) {heights[e] += ch;} else if (heights[e] >= 10) {heights[e] -= ch;}
				//		 if (heights[e] > 100) heights[e] = mod > 0 ? 100 : 5;
				//	});
				//	if (heights[r] > 100) heights[r] = mod > 0 ? 100 : 5;
				//});
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

			range.ForEach(r => );
			for (int ri = 0; ri < range.Count; ri++)
			{
				int r = range[ri];
				uint[] neighbors = sectors[r].NeighborSectorIds;

				for(int ni = 0; ni < neighbors.Length; ni++)
				{
					uint n = neighbors[ni];
					Sector neighbor = sectors[n];
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
				List<float> lowlands = heights.ToArray().Where(x => x >= 20).ToList();
				if (lowlands.Count == 0) return;
				int rnd = Mathf.FloorToInt(UnityEngine.Random.value * lowlands.Count);
				start = lowlands[rnd].index; //TODO: eh?
	}
			List<int> query = new List<int>(new int[] { start });
			List<int> newQuery = new List<int>();

	// depress pit center
	heights[start] -= change;
      if (heights[start] < 5 || heights[start] > 100) heights[start] = 5;
      cells[start].used = session;
      for (let i = 1; i< 10000; i++) {
        const rnd = Math.random() * 0.4 + 0.8;
	change -= i / 0.6 * rnd;
        if (change< 1) break;
        query.map(function(p) {
          cells[p].neighbors.forEach(function(e) {
            if (cells[e].used === session) return;
            cells[e].used = session;
            if (Math.random() > 0.8) return;
            newQuery.push(e);
            heights[e] -= change;
            if (heights[e] < 5 || heights[e] > 100) heights[e] = 5;
          });
        });
        query = newQuery.slice();
        newQuery = [];
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
			heights[i] = (heights[i] * (fraction - 1) + nHeights.Average()) / (float) fraction;
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
