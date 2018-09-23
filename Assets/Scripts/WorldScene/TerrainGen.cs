
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class TerrainGen : MonoBehaviour {

    private const float defaultExtentX = 1.0f;
    private const float defaultExtentY = 1.0f;
    //private const float[] defaultExtent = { defaultExtentX, defaultExtentY };

    public MeshFilter filter;
    public UnityEngine.Mesh mesh;

    public float extentX = defaultExtentX;
    public float extentY = defaultExtentY;
    public string generator = "generateCoast";
    public int npts = 10; //16384;
    public int ncities = 15;
    public int nterrs = 5;

	public Render render;

    public static Params defaultParams = new Params("generateCoast", 10, 15, 5, defaultExtentX, defaultExtentY); //16384

    public struct MapAndMesh
    {
        public Mesh mesh;
        public float[] h;
        public int[] downhill;

        public override string ToString()
        {
            string n = " | "; //System.Environment.NewLine;
            string output = "MapAndMesh: " + n;

            if(h != null)
            {
                output += "h: " + h.Length + n;
            } else
            {
                output += "h: null" + n;
            }
            
            if( downhill != null)
            {
                output += "downhill: " + downhill.Length + n;
            } else
            {
                output += "downhill: null" + n;
            }

            output += mesh.ToString();

            return output;
        }
    }

    public struct Mesh
    {
        public float[][] pts; //each pt is a cell, surrounded by a polygon from voronoi,
        public Delaunay.Voronoi voronoi;
        public List<Vector2> vxs; //each vx is a vertex in >= 1 voronoi polygon
        public Dictionary<Vector2, int> vxids;
        //public List<List<int>> adj;
        public Dictionary<Vector2, List<Vector2>> adj;
        //public List<List<int>> adjId;
        public Dictionary<int, List<int>> adjId;
        public List<int[]> edges;
        public List<List<int>> tris;
        public float extentX;
        public float extentY;

        public override string ToString()
        {
            string n = " | "; //System.Environment.NewLine;
            string output = "Mesh: " + n;

            output += "pts: " + pts.Length + n;
            output += "voronoi: " + voronoi.ToString() + n;
            output += "vxs: " + vxs.Count + n;
            output += "vxids: " + vxids.Count + n;
            output += "adj: " + adj.Count + n;
            output += "adjId: " + adjId.Count + n;
            output += "edges: " + edges.Count + n;
            output += "tris: " + tris.Count + n;
            
            return output;
        }

        public Mesh(float[][] inpts, Delaunay.Voronoi invor, List<Vector2> invxs, Dictionary<Vector2, int> invxids, Dictionary<Vector2, List<Vector2>> inadj, Dictionary<int, List<int>> inadjIds, List<int[]> inedges, List<List<int>> intris, float inextentX, float inextentY) //
		{
            pts = inpts;
            voronoi = invor;
            vxs = invxs;
            vxids = invxids;
            adj = inadj;
            adjId = inadjIds;
            tris = intris;
            edges = inedges;
            extentX = inextentX;
            extentY = inextentY;
        }

    }

    public struct Render
    {
        public Params p;
        public MapAndMesh h;
        //public List<List<int>> terr;
        public List<int> cities;
		public List<int[]> coast;
		public List<int[]> rivers;
		public List<int> regions;
		public Dictionary<int, List<int>> regionNeighbors;

		//

		public override string ToString()
		{
			string n = " | ";
			string output = "Render: ";

			output += h.ToString() + n;
			output += "Cities: ";
			if(cities != null)
			{
				for (int i = 0; i < cities.Count; i++)
				{
					output += i + ": " + cities[i] + ", ";
				}
			} else
			{
				output += "null";
			}
			if (coast != null)
			{
				output += n + "Coast.Count:" + coast.Count + n;
			} else
			{
				output += n + "Coast = null" + n;
			}

			if (rivers != null)
			{
				output += "Rivers.Count: " + rivers.Count + n;
			} else
			{
				output += "Rivers = null: " + n;
			}
			return output;
		}
	}

    public struct Params
    {
        public float extentX;
        public float extentY;
        public string generator;
        public int npts; // = 16384;
        public int ncities;
        public int nterrs;

        public Params(string generatorIn, int nptsIn, int ncitiesIn, int nterrsIn, float extentInX = defaultExtentX, float extentInY = defaultExtentY)
        {
            extentX = extentInX;
            extentY = extentInY;
            generator = generatorIn;
            npts = nptsIn;
            ncities = ncitiesIn;
            nterrs = nterrsIn;
        }
    }

    public void Start()
    {
        //filter = gameObject.GetComponent<MeshFilter>();
        mesh = filter.mesh;
        if(mesh == null)
        {
            mesh = new UnityEngine.Mesh();
            mesh.name = "Terrain Mesh";
        }
    }

	private void OnDrawGizmos()
	{
		if (mesh == null || mesh.vertexCount == 0) // || true -> just skip for now, kthx
		{
			return;
		} 
			
		List<Vector3> vertexBuffer = new List<Vector3>();
		List<int> triangleBuffer = new List<int>();
		List<int> cities = render.cities;
		List<int[]> rivers = render.rivers;
		// call this if the mesh has been modified and you want to get the vertices.
		vertexBuffer.Clear();
		filter.sharedMesh.GetVertices(vertexBuffer);
		filter.sharedMesh.GetTriangles(triangleBuffer, 0);
		Gizmos.color = Color.black;
		for (int i = 0; i < vertexBuffer.Count; i++)
		{
			Gizmos.DrawSphere(vertexBuffer[i], 0.1f);
		}

		Gizmos.color = Color.black;
		for (int i = 0; i < triangleBuffer.Count; i+= 3)
		{
			Vector3 t0 = vertexBuffer[triangleBuffer[i]];
			Vector3 t1 = vertexBuffer[triangleBuffer[i+1]];
			Vector3 t2 = vertexBuffer[triangleBuffer[i+2]];

			Gizmos.DrawLine(t0, t1);
			Gizmos.DrawLine(t1, t2);
			Gizmos.DrawLine(t2, t0);
		}

		Gizmos.color = Color.red;
		if (cities != null && cities.Count > 0)
		{
			for (int i = 0; i < cities.Count; i++)
			{
				Gizmos.DrawSphere(vertexBuffer[cities[i]], 0.25f);
			}
		}

		Gizmos.color = Color.blue;
		if (rivers != null && rivers.Count > 0)
		{
			for (int i = 0; i < rivers.Count; i++)
			{
				Vector3 r0 = vertexBuffer[rivers[i][0]];
				Vector3 r1 = vertexBuffer[rivers[i][1]];

				Gizmos.DrawLine(r0, r1);
			}
		}
	}
	public void GenerateTerrain()
    {
        Params p = defaultParams;

		p.extentX = extentX;
		p.extentY = extentY;
		p.generator = generator;
		p.npts = npts;
		p.ncities = ncities;
		p.nterrs = nterrs;

		render = new Render();
        render = generateCoast(p);
		//render.h = h;

		Debug.Log(render.ToString());

		ConvertMesh(render.h, mesh);
    }
    public TerrainGen(Params p)
    {
        Type t = this.GetType();
        MethodInfo method = t.GetMethod(p.generator);
        object[] o = { p };
        MapAndMesh h = (MapAndMesh) method.Invoke(this, o);
        ConvertMesh(h, mesh);
    }

    private void ConvertMesh(MapAndMesh h, UnityEngine.Mesh mesh)
    {
        int numVertices = h.h.Length;
        int numVx = h.mesh.vxs.Count;
        int numPts = h.mesh.pts.Length;
		//int numEdges = h.mesh.edges.Count;
		int numTris = h.mesh.tris.Count;

		Debug.Log(string.Format("h: {0}, v: {1}, p: {2}, f: {3}", numVertices, numVx, numPts, numTris));

        Vector3[] vertices = new Vector3[h.h.Length];
		Vector2[] uvs = new Vector2[h.h.Length];
        int p = 0;
        while (p < vertices.Length)
        {
            Vector2 pt2 = h.mesh.vxs[p];
            float height = h.h[p];
            //Debug.Log(string.Format("New Vertex p[{3} = ({0},{1}), h: {2}", pt2.x, pt2.y, height, p));

            vertices[p] = new Vector3(pt2.x * 16f , pt2.y * 16f, height * -16f);
			uvs[p] = new Vector2(pt2.x, pt2.y);
            p++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
		mesh.uv = uvs;


        List<int> triangles = new List<int>();

		Debug.Log("Camera Dir: " + Camera.main.transform.forward.ToString());

		for (int i = 0; i < h.mesh.tris.Count; i++)
        {
			//Debug.Log(i);
            List<int> tri = h.mesh.tris[i];
            //Debug.Log(string.Format("got a tri length {0}", tri.Count));
            if(tri.Count == 3)
            {
                //for (int j = 0; j < 3; j++)
                //{

				Vector3 a = mesh.vertices[tri[0]];
				Vector3 b = mesh.vertices[tri[1]];
				Vector3 c = mesh.vertices[tri[2]];

				Vector3 crossProd = Vector3.Cross(c - a, b - a);
				Vector3 cameraDir = Camera.main.transform.forward;

				float norm = Vector3.Dot(crossProd, cameraDir);
				
				if(norm > 0)
				{
					triangles.Add(tri[0]);
					triangles.Add(tri[1]);
					triangles.Add(tri[2]);
				} else
				{
					//Debug.Log("Norm[" + i + "] < 0");
					triangles.Add(tri[2]);
					triangles.Add(tri[1]);
					triangles.Add(tri[0]);
				}


				//triangles.Add(tri[j]);
     //           }
            } else
			{
				Debug.Log(string.Format("tris[{0}].Count = {1}", i, tri.Count));
			}
            
        }

        mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}

    private float runif(float lo, float hi) {
        return UnityEngine.Random.Range(lo, hi); // * (hi - lo);
    }

    private float[] rnorm() {
       double x1 = 0;
       double x2 = 0;
       double w = 2.0f;
       while (w >= 1) {
           x1 = runif(-1, 1);
           x2 = runif(-1, 1);
           w = x1 * x1 + x2 * x2;
       }

        w = Math.Sqrt(-2 * Math.Log(w) / w);
        float[] vec = { (float) x1, (float) x2 };
        return vec;
    }
    
    private float[] randomVector(float scale) {

        float[] vec = rnorm();
        vec[0] *= scale;
        vec[1] *= scale;
        return vec;
    }

	//public float[][] generatePoints(int n, float extentX = defaultExtentX, float extentY = defaultExtentY)
	//{

	//    float[][] pts = new float[n][];
	//    for (int i = 0; i < n; i++) {
	//        float[] pt = { UnityEngine.Random.Range(-0.5f, 0.5f) * extentX, UnityEngine.Random.Range(-0.5f, 0.5f) * extentY };
	//        pts[i] = pt;
	//    }

	//    return pts;
	//}

	public float[][] generatePoints(int n, float extentX = defaultExtentX, float extentY = defaultExtentY)
	{

		float spacing = Mathf.Sqrt(defaultExtentX * defaultExtentY) / Mathf.Sqrt((float) n);
		float radius = spacing / 2;
		float jittering = radius * 0.9f;
		Debug.Log(string.Format("Spacing: {0}, Radius: {1}, Jittering: {2}", spacing, radius, jittering));
		List<float[]> pts = new List<float[]>(); //[n][];

		for (float x = radius - defaultExtentX * 0.5f; x < defaultExtentX * 0.5f; x += spacing)
		{
			for (float y = radius - defaultExtentY * 0.5f; y < defaultExtentY * 0.5f; y += spacing)
			{
				float xj = (x + jitter(jittering));
				float yj = (y + jitter(jittering));

				pts.Add(new float[] { xj, yj });

			}
		}

		Debug.Log("pts: " + pts.Count);

		return pts.ToArray();
	}

	private float jitter(float amt)
	{
		return UnityEngine.Random.Range(-amt, amt);
	}

	private float[][] improvePoints(float[][] pts, int n = 1, float extentX  = defaultExtentX, float extentY = defaultExtentY) {
        float epsilon = 1.0e-6f;
	    for (int k = 0; k < n; k++) {
            //pts.Select(x => new float[] { Mathf.Round(x[0] / epsilon) * epsilon, Mathf.Round(x[1] / epsilon) * epsilon });
            Debug.Log("improvePoints round #" + k);
            //printPts(pts);


            int numPts = pts.Length;
            List<Vector2> Vecpts = new List<Vector2>(numPts);
            Vecpts = pts.Select(x => new Vector2(x[0], x[1])).ToList();
            List<uint> colors = new List<uint>(new uint[numPts]);
            //Debug.Log("improvePoints(). Colors: " + colors.Count());
            Delaunay.Voronoi v = new Delaunay.Voronoi(Vecpts, colors, new Rect(-extentX * 0.5f, -extentY * 0.5f, extentX, extentY));
            List<List<Vector2>> regions = v.Regions();
            //printVecs(regions);
            List<Vector2> centroids = regions.Select(r => centroid(r)).ToList();
            //printVec(centroids);
            pts = ConvertVecToFloat(centroids);

        }
        return pts;
    }

    float[][] ConvertVecToFloat(List<Vector2> vectors)
    {
        int numPts = vectors.Count;
        float[][] pts = new float[numPts][];

        for(int i =0; i < numPts; i++)
        {
            pts[i] = new float[] { vectors[i].x, vectors[i].y};
        }

        return pts;
    }

    List<Vector2> ConvertFloatToVec(float[][] floats)
    {
        int numPts = floats.Length;
        List<Vector2> vecs = new List<Vector2>(numPts);
        for(int i = 0; i < numPts; i++)
        {
            vecs[i] = new Vector2(floats[i][0], floats[i][1]);
        }

        return vecs;
    }

    void printVecs(List<List<Vector2>> vecArray)
    {
        for(int i = 0; i < vecArray.Count; i++)
        {
            Debug.Log(string.Format("vecArray[{0}]", i));
            printVec(vecArray[i]);
            Debug.Log("**********");
        }
    }

    void printVec(List<Vector2> vec)
    {
        for (int i = 0; i < vec.Count; i++)
        {
            Debug.Log(string.Format("( {0}, {1} )", vec[i].x, vec[i].y)); // vec.ToString());
        }
    }

    float[] centroid(List<float[]> polygon)
    {
        int i = -1;
        int n = polygon.Count;
        float x = 0;
        float y = 0;
        float[] a;
        float[] b = polygon[n - 1];
        float c;
        float k = 0;

        while (++i < n)
        {
            a = b;
            b = polygon[i];
            k += c = a[0] * b[1] - b[0] * a[1];
            x += (a[0] + b[0]) * c;
            y += (a[1] + b[1]) * c;
        }
        k *= 3;
        return new float[] { x / k, y / k };
    }

    Vector2 centroid(List<Vector2> polygon)
    {
        Vector2 result = new Vector2();
        int count = polygon.Count;
        foreach(Vector2 vector in polygon)
        {
            result += vector;
        }

        result /= count;

        return result;
    }
    public float[][] generateGoodPoints(int n, float extentX = defaultExtentX, float extentY = defaultExtentY) {

        float[][] pts = generatePoints(n, extentX, extentY);

        
        pts.OrderBy( x=> x[0]);
        Debug.Log("generateGoodPoints: ");
        //printPts(pts);

        return improvePoints(pts, 1, extentX, extentY);
    }

    public Mesh MakeMesh(float[][] pts, float extentX = defaultExtentX, float extentY = defaultExtentY) {

        int numPts = pts.Length;
        List<Vector2> Vecpts = new List<Vector2>(numPts);
        Vecpts = pts.Select(x => new Vector2(x[0], x[1])).ToList();
        List<uint> colors = new List<uint>(new uint[numPts]);

        Rect bounds = new Rect(-extentX * 0.5f, -extentY * 0.5f, extentX, extentY);

        Delaunay.Voronoi voronoi = new Delaunay.Voronoi(Vecpts, colors, bounds);

        //data from Voronoi:
        List<Delaunay.Site> sites = voronoi.getSites(); // List<Delaunay.Site>(numPts);
		List<Delaunay.Triangle> vTris = voronoi.Triangles();
		
		//List<Delaunay.Geo.LineSegment> m_delaunayTriangulation = voronoi.DelaunayTriangulation();
		//if (m_delaunayTriangulation != null)
		//{
		//	for (int i = 0; i < m_delaunayTriangulation.Count; i++)
		//	{
		//		Vector2 left = (Vector2)m_delaunayTriangulation[i].p0;
		//		Vector2 right = (Vector2)m_delaunayTriangulation[i].p1;
		//	}
		//}

		//helper counting variables

		int totalVerts = sites.Count;
		int totalFaces = vTris.Count;
		int totalEdges = totalVerts + totalFaces - 2; // v - e + f = 2

		//data to give to makeMesh():
		List<Vector2> siteCoords = new List<Vector2>(new Vector2[totalFaces]); //not from sites, but calculated as the centroid of each triangle
		List<Vector2> vxs = new List<Vector2>(new Vector2[totalVerts]);
        Dictionary<Vector2, int> vxids = new Dictionary<Vector2, int>(totalVerts);
        Dictionary<Vector2, List<Vector2>> adj = new Dictionary<Vector2, List<Vector2>>(totalEdges);
        Dictionary<int, List<int>> adjId = new Dictionary<int, List<int>>(totalEdges);
		//List<Vector2[]> edges = new List<Vector2[]>(new Vector2[totalEdges][]);
		List<int[]> edges = new List<int[]>();
        List<List<int>> tris = new List<List<int>>(new List<int>[totalFaces]);
		Dictionary<int, List<int>> sitePolyIds = new Dictionary<int, List<int>>(sites.Count); //for each site, a list of vert IDs to build a poly to highlight it
		//Dictionary<List<Vector2>, List<Vector2>> edgeSites = new Dictionary<List<Vector2>, List<Vector2>>();
		Dictionary<int, List<int>> siteTri = new Dictionary<int, List<int>>();

        //first, assign int IDs to all verts (including site coord)
        for (int i = 0; i < sites.Count; i++)
        {
            Vector2 coord = sites[i].Coord;
            
			int vertId = (int) sites[i].getIndex();
			//vxs[i] = coord;
			vxs[vertId] = coord;
			vxids.Add(coord, vertId);
            //sitePolyIds.Add(coord, new List<int>());

            adj.Add(coord, new List<Vector2>(3));
            adjId.Add(vertId, new List<int>(3));

        }

        for (int i = 0; i < vTris.Count; i++)
        {
			List<Delaunay.Site> tri = vTris[i].sites;
			List<Vector2> triCoord = new List<Vector2>(new Vector2[3]);
			List<int> triIndices = new List<int>(new int[3]);

			//assign triangle indices

			triCoord[0] = tri[0].Coord;
			triIndices[0] = (int)tri[0].getIndex();
			triCoord[1] = tri[1].Coord;
			triIndices[1] = (int)tri[1].getIndex();
			triCoord[2] = tri[2].Coord;
			triIndices[2] = (int)tri[2].getIndex();

			tris[i] = triIndices;
			//Debug.Log(string.Format("{0}: {1}, {2}, {3} ( {4} - {5} - {6} )", i, triIndices[0], triIndices[1], triIndices[2], triCoord[0], triCoord[1], triCoord[2]));

			Vector2 siteCoord = new Vector2((triCoord[0].x + triCoord[1].x + triCoord[2].x) / 3.0f, (triCoord[0].y + triCoord[1].y + triCoord[2].y) / 3.0f);
			siteCoords[i] = siteCoord;
			siteTri.Add(i, triIndices);

			//add adjacency by coord and index

			if (!adj[triCoord[0]].Contains(triCoord[1]))
				adj[triCoord[0]].Add(triCoord[1]);

			if (!adj[triCoord[0]].Contains(triCoord[2]))
				adj[triCoord[0]].Add(triCoord[2]);

			if (!adj[triCoord[1]].Contains(triCoord[0]))
				adj[triCoord[1]].Add(triCoord[0]);

			if (!adj[triCoord[1]].Contains(triCoord[2]))
				adj[triCoord[1]].Add(triCoord[2]);

			if (!adj[triCoord[2]].Contains(triCoord[1]))
				adj[triCoord[2]].Add(triCoord[1]);

			if (!adj[triCoord[2]].Contains(triCoord[0]))
				adj[triCoord[2]].Add(triCoord[0]);

			if (!adjId[triIndices[0]].Contains(triIndices[1]))
				adjId[triIndices[0]].Add(triIndices[1]);

			if (!adjId[triIndices[0]].Contains(triIndices[2]))
				adjId[triIndices[0]].Add(triIndices[2]);

			if (!adjId[triIndices[1]].Contains(triIndices[0]))
				adjId[triIndices[1]].Add(triIndices[0]);

			if (!adjId[triIndices[1]].Contains(triIndices[2]))
				adjId[triIndices[1]].Add(triIndices[2]);

			if (!adjId[triIndices[2]].Contains(triIndices[1]))
				adjId[triIndices[2]].Add(triIndices[1]);

			if (!adjId[triIndices[2]].Contains(triIndices[0]))
				adjId[triIndices[2]].Add(triIndices[0]);

			

			//assign edges

			int[] edge0 = new int[] { triIndices[0], triIndices[1] };
			int[] edge0alt = new int[] { triIndices[1], triIndices[0] };
			int[] edge1 = new int[] { triIndices[1], triIndices[2] };
			int[] edge1alt = new int[] { triIndices[2], triIndices[1] };
			int[] edge2 = new int[] { triIndices[2], triIndices[0] };
			int[] edge2alt = new int[] { triIndices[0], triIndices[2] };

			if (!edges.Contains(edge0) && !edges.Contains(edge0alt)){
				edges.Add(edge0);
			}

			if (!edges.Contains(edge1) && !edges.Contains(edge1alt)){
				edges.Add(edge1);
			}

			if (!edges.Contains(edge2) && !edges.Contains(edge2alt)){
				edges.Add(edge2);
			}

        } //end for each vTris

		//for(int i = 0; i < adjId.Count; i++)
		//{
			//Debug.Log(i + ": " + adjId[i].Count);
			//for(int j = 0; j < adjId[i].Count; j++)
			//{
			//	//Debug.Log(adjId[i][j]);
			//}
		//}

		//todo: add original site + poly to map as sector

        //todo: calc site adjacency by:
        // for each site, get edges
        // for each edge in edges, find edgeSites
        // for each site in edgeSites that isn't the current site, add to siteAdjacency

        // now we have a list of edges, we know which site(s) (1 or 2) they correspond to, 
        // and we have Vector2 coordinates for all of these. let's make some triangles.

        //we'll have to add the site coords to the vert list, and make edges to complete the tris

        //f faces = # of tris, = total length of polys
        //e edges = length of edge dictionary ( # of poly edges + # of sites)

        //totalVerts = totalEdges - totalFaces + 2; // or just # of sites + 2

        Debug.Log(string.Format("{0} sites, {1} edges, {2} faces, {3} verts", sites.Count, edges.Count, tris.Count, totalVerts));

        Mesh mesh = new Mesh(pts, voronoi, vxs, vxids, adj, adjId, edges, tris, extentX, extentY);

        return mesh;
    } //end MakeMesh()

    public Mesh generateGoodMesh(int n, float extentX = defaultExtentX, float extentY = defaultExtentY) {

        float[][] pts = generateGoodPoints(n, extentX, extentY);
        Debug.Log("generateGoodMesh: ");
        //printPts(pts);
        return MakeMesh(pts, extentX, extentY);
    }

    public void printPts(float[][] pts)
    {
        if (pts == null)
            return;

        Debug.Log("Pts:");
        for(int i = 0; i < pts.Length; i++)
        {
            Debug.Log(string.Format("{0}: {1}, {2}", i, pts[i][0], pts[i][1]));
        }
    }

    void printH(float[] h, bool showAll = false)
    {
        if (h == null)
            return;
		float Sum = h.Sum();

        Debug.Log("H:");
        for (int i = 0; i < h.Length; i++)
        {
            float h1 = h[i];
            if (showAll || h1 == Mathf.Infinity || h1 == Mathf.NegativeInfinity || float.IsNaN(h1) || Math.Abs(h1) > 900000 || Sum == 0)  
                Debug.Log(string.Format("h[{0}]: {1}", i, h[i]));
        }
    }

    public bool isedge(Mesh mesh, int i) {
        List<int> vxAdj;// = new List<int>();
        if(mesh.adjId.TryGetValue(i, out vxAdj))
        {
            return (vxAdj.Count < 3);
        } else
        {
            return false; //vx has 0 adj
        }
            //.ElementAt(i).Count < 3);
    }

    private bool isnearedge(Mesh mesh, int i) {
        float x = mesh.vxs[i].x;
        float y = mesh.vxs[i].y;
        float w = mesh.extentX;
        float h = mesh.extentY;
        return x < -0.45f * w || x > 0.45f * w || y < -0.45f * h || y > 0.45f * h;
    }

    private List<int> neighbours(Mesh mesh, int index) {

        List<int> onbs;
        bool found = mesh.adjId.TryGetValue(index, out onbs);
        List<int> nbs = new List<int>();
        if (found)
        {
            for (int i = 0; i < onbs.Count; i++)
            {
                nbs.Add(onbs[i]);
            }
        }
		//else
        //{
            //Debug.Log("neighbours(): " + mesh.ToString());
            //Debug.Log("Looking for adjId[" + index + "]");
            //Debug.Log("Not Found!");
        //}
        
        return nbs;
    }

    float distance(Mesh mesh, int i, int j) {
        Vector2 p = mesh.vxs[i];
        Vector2 q = mesh.vxs[j];
        return Vector2.Distance(p, q); // (float) Math.Sqrt((p[0] - q[0]) * (p[0] - q[0]) + (p[1] - q[1]) * (p[1] - q[1]));
    }

    int quantile(MapAndMesh h, float q) {
        float[] sortedh = new float[h.h.Length];
        for (int i = 0; i < h.h.Length; i++) {
            sortedh[i] = h.h[i];
        }
        Array.Sort(sortedh);
        h.h = sortedh;
        return bisectRight(sortedh, q, 0, 0);
    }

    private int bisectRight(float[] a, float x, int lo, int hi)
    {
        if (lo < 0)
        {
            lo = 0;
        }
        if (hi <= lo)
        {
            hi = a.Length - 1;
        }

        while (lo < hi)
        {
            int mid = lo + hi >> 1;
            if (a[mid] < x)
            {
                hi = mid;
            }
            else
            {
                lo = mid + 1;
            }
        }
        return lo;
    }

    private MapAndMesh zero(Mesh mesh) {
        //Debug.Log("zero()");
        MapAndMesh m = new MapAndMesh();
        m.mesh = mesh;
        float[] z = new float[mesh.vxs.Count];
        for (int i = 0; i < mesh.vxs.Count; i++) {
            z[i] = 0;
        }
        m.h = z;
        return m;
    }

    private MapAndMesh slope(Mesh mesh, float[] direction) {
        Debug.Log("slope()");
        MapAndMesh h = zero(mesh);
        //h.mesh = mesh;

        List<Vector2> vxs = mesh.vxs;
        float[] height = h.h;
        height = vxs.Select(v => {
            float h1 = v.x * direction[0] + v.y * direction[1];


            if (Math.Abs(h1) > 1 || h1 == Mathf.Infinity || h1 == Mathf.NegativeInfinity || float.IsNaN(h1))
            {
                //Debug.Log(string.Format("slope->dir {0}, {1} h[{2}] = {3}", direction[0], direction[1], mesh.vxs.IndexOf(v), h1));
            }
            return h1; // v.x * direction[0] + v.y * direction[1];
        }).ToArray();

        h.h = height;
        return h;
    }

    private MapAndMesh slope(Mesh mesh, Vector2 direction)
    {
        Debug.Log("slope()");

        MapAndMesh h = zero(mesh);
        //h.mesh = mesh;

        List<Vector2> vxs = mesh.vxs;
        float[] height = h.h;
        height = vxs.Select(v => {
            float h1 = v.x * direction.x + v.y * direction.y;
            if ( h1 == Mathf.Infinity || h1 == Mathf.NegativeInfinity || float.IsNaN(h1)) //Math.Abs(h1) > 1 ||
            {
                //Debug.Log(string.Format("slope->dir {0}, {1} h[{2}] ({3},{4}) = {5}", direction.x, direction.y, mesh.vxs.IndexOf(v), v.x, v.y, h1));
                //Debug.Log("v = " +v.ToString());
            }
            return h1; // v.x * direction.x + v.y * direction.y;
        }).ToArray();

        h.h = height;
        return h;
    }

    private MapAndMesh cone(Mesh mesh, float slope) {
        Debug.Log("cone()");
        MapAndMesh h = zero(mesh);
        //h.mesh = mesh;

        List<Vector2> vxs = mesh.vxs;
        float[] height = h.h;
        height = vxs.Select(v => {
            return Mathf.Pow(v.x * v.x + v.y * v.y, 0.5f) * slope;
        }).ToArray();

        h.h = height;
        return h;
    }

    private float[] normalize(float[] h) {
        //Debug.Log("normalize() before:");
        //printH(h, true);
        float lo = h.Min();
        float hi = h.Max() - lo;

        //Debug.Log(string.Format("lo: {0}, hi: {1}", lo, hi));

        float[] newH = new float[h.Length];
        for(int i = 0; i < h.Length; i++)
        {
            newH[i] = (h[i] - lo) / hi;
        }

        //Debug.Log("normalize() after:");
        //printH(newH, true);
        return newH; //h.Select( k => { return (k - lo) / hi ; }).ToArray();
    }

    private MapAndMesh peaky(MapAndMesh h) {
        Debug.Log("peaky()");
        h.h = normalize(h.h);
        h.h.Select( k => Math.Sqrt(k));

        return h;
    }

    private MapAndMesh add(MapAndMesh[] maps) {
        Debug.Log("add()");
        Debug.Log("Maps: " + maps.Count());
        

        for (int j = 0; j < maps.Length; j++)
        {
            //Debug.Log(j + ": " + maps[j].h.Length);
            //printH(maps[j].h);
        }
            int n = maps[0].h.Count();
        MapAndMesh newvals = zero(maps[0].mesh);
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < maps.Length; j++) {
                newvals.h[i] += maps[j].h[i];
            }
        }
        return newvals;
    }

    private MapAndMesh mountains(Mesh mesh, int n, float r = 0.05f) {

        MapAndMesh m = zero(mesh);

        Debug.Log("mtns. mesh.vxs " + mesh.vxs.Count);

        List<float[]> mounts = new List<float[]>();

        for (int i = 0; i < n; i++) {
            mounts.Add( new float[] { mesh.extentX * UnityEngine.Random.Range(-0.5f, 0.5f), mesh.extentY * UnityEngine.Random.Range(-0.5f, 0.5f)  });
        }

        for (int i = 0; i < mesh.vxs.Count; i++) {
            Vector2 p = mesh.vxs[i];
            for (int j = 0; j < n; j++) {
                float[] mount = mounts[j];
                m.h[i] += Mathf.Pow(Mathf.Exp(-((p.x - mount[0]) * (p.x - mount[0]) + (p.y - mount[1]) * (p.y - mount[1])) / (2 * r * r)), 2);
            }
        }
        return m;
    }

    private MapAndMesh relax(MapAndMesh old) {

        MapAndMesh m = zero(old.mesh);

        for (int i = 0; i < m.h.Length; i++) {
            List<int> nbs = neighbours(old.mesh, i);
            //Debug.Log(string.Format("neighbours({0} #{1}", i, nbs.Count));
            //Debug.Log(string.Format("and h.mesh.adjId[{0}]", h.mesh.adjId[i].Count));
            if (nbs.Count < 3) {
				Debug.Log("skipped "+i);
                m.h[i] = 0;
                continue;
            }
            m.h[i] = nbs.Select( j => { return old.h[j]; }).Average();
			//Debug.Log(string.Format("New h[{0}] = {1}", i, m.h[i]);
        }
        return m;
    }

    private int[] downhill(MapAndMesh map) {

		Debug.Log("downhill(), h = " + (map.h == null ? "null" : map.h.Length + ""));
		//Debug.Log();
        if (map.downhill != null) return map.downhill;

        int[] downs = new int[map.h.Length];
        for (int i = 0; i < map.h.Length; i++) {
            downs[i] = downfrom(i, map);
        }
		map.downhill = downs;
        return downs;
    }

    private int downfrom(int i, MapAndMesh h)
    {
        if (isedge(h.mesh, i)) return -2;
        int best = -1;
        float besth = h.h[i];
        List<int> nbs = neighbours(h.mesh, i);
        for (int j = 0; j < nbs.Count(); j++)
        {
            if (h.h[nbs[j]] < besth)
            {
                besth = h.h[nbs[j]];
                best = nbs[j];
            }
        }
        return best;
    }

    private void findSinks(MapAndMesh h) {
        int[] dh = downhill(h);
        int[] sinks = new int[dh.Length];
        for (int i = 0; i < dh.Length; i++) {
            int node = i;
            while (true) {
                if (isedge(h.mesh, node)) {
                    sinks[i] = -2;
                    break;
                }
                if (dh[node] == -1) {
                    sinks[i] = node;
                    break;
                }
                node = dh[node];
            }
        }
    }

    private MapAndMesh fillSinks(MapAndMesh h, float epsilon = 1e-5f) {

        Debug.Log("fillSinks()");
		float infinity = 999999; // Mathf.Infinity;
        MapAndMesh newh = zero(h.mesh);
        for (int i = 0; i < h.h.Length; i++) {
            if (isnearedge(h.mesh, i)) {
                //Debug.Log(string.Format("h[{0}] = {1} near edge", i, h.h[i]));
                newh.h[i] = h.h[i];
            } else {
                //Debug.Log(string.Format("h[{0}] = {1} not near edge", i, h.h[i]));
                newh.h[i] = infinity;
            }
        }
        while (true) { //repeat until no vx heights change
            bool changed = false;
            for (int i = 0; i < h.h.Length; i++) {
                if (newh.h[i] == h.h[i])
                {
                    continue; //it didn't change, skip it
                }
                List<int> nbs = neighbours(h.mesh, i);
                //Debug.Log(string.Format("h[{0}].nbs = {1}", i,  nbs.Count));
                for (int j = 0; j < nbs.Count; j++) {
                    if (h.h[i] >= newh.h[nbs[j]] + epsilon) {
						//Debug.Log(string.Format("(h.h[{0}] = {1} >= newh.h[nbs[{2}] = {3}] = {4} + eps", i, h.h[i], j, nbs[j], newh.h[nbs[j]]));
                        newh.h[i] = h.h[i];
                        changed = true;
                        break;
                    }
                    float oh = newh.h[nbs[j]] + epsilon;
                    if ((newh.h[i] > oh) && (oh > h.h[i])) {
						//Debug.Log(string.Format("(newh.h[{0}] = {1}> oh = {2}) && ( oh = {2} > h.h[{0}] = {3})", i, newh.h[i], oh, h.h[i]));
						newh.h[i] = oh;
                        changed = true;
                    }
                }
            }
            if (!changed) return newh; //when no vx heights change, return the result
        }
    }

    private MapAndMesh getFlux(MapAndMesh h) {

       // Debug.Log("getFlux()");

        int[] dh = downhill(h);

        int[] idxs = new int[h.h.Length];

        MapAndMesh flux = zero(h.mesh);
		float[] fh = new float[h.h.Length];

        for (int i = 0; i < h.h.Length; i++) {
			//Debug.Log(string.Format("dh[{0}] = {1}", i, dh[i]));
			idxs[i] = i;
            fh[i] = 1.0f/h.h.Length;
        }

        idxs = idxs.OrderByDescending(x => h.h[x]).ToArray();

        for (int i = 0; i < h.h.Length; i++) {
			//Debug.Log(string.Format("idxs[{0}] = {1}", i, idxs[i]));
            int j = idxs[i];
            if (dh[j] >= 0) {
				//Debug.Log(string.Format("dh[{0}] = {1} >= 0, flux.h += flux.h[{0}] = {2}", j, dh[j], fh[j]));
				fh[dh[j]] += fh[j];

            }
        }
		flux.h = fh;
        return flux;
    }

    private MapAndMesh getSlope(MapAndMesh h) {

        Debug.Log("getSlope()");
        //int[] dh = 
        downhill(h);
        MapAndMesh slope = zero(h.mesh);
        for (int i = 0; i < h.h.Length; i++) {
            float[] s = trislope(h, i);
            slope.h[i] = Mathf.Sqrt(s[0] * s[0] + s[1] * s[1]);
        }

        return slope;
    }

    private MapAndMesh erosionRate(MapAndMesh h) {

        Debug.Log("erosionRate()");

        MapAndMesh flux = getFlux(h);
		printH(flux.h);
        MapAndMesh slope = getSlope(h);
		printH(slope.h);
        MapAndMesh newh = zero(h.mesh);
        for (int i = 0; i < h.h.Length; i++) {
            float river = Mathf.Sqrt(flux.h[i]) * slope.h[i];
            float creep = slope.h[i] * slope.h[i];
            float total = 1000 * river + creep;
            total = total > 200 ? 200 : total;
            newh.h[i] = total;
        }
        return newh;
    }

    private MapAndMesh erode(MapAndMesh h, float amount) {
        Debug.Log("erode()");
        MapAndMesh er = erosionRate(h);
        MapAndMesh newh = zero(h.mesh);
        float maxr = er.h.Max();
		Debug.Log(string.Format("maxr = {0}, amount = {1}", maxr, amount));
        for (int i = 0; i < h.h.Length; i++) {
            newh.h[i] = h.h[i] - amount * (er.h[i] / maxr);
        }
        return newh;
    }

    MapAndMesh doErosion(MapAndMesh h, float amount, int n = 1) {
        Debug.Log("doErosion()");
        h = fillSinks(h);
		Debug.Log("doErosion.fillSinks()");
		printH(h.h);
        for (int i = 0; i < n; i++) {
            h = erode(h, amount);
			Debug.Log(string.Format("doErosion.erode({0}) #{1}", amount, i));
			printH(h.h);
			h = fillSinks(h);
			Debug.Log(string.Format("doErosion.fillSinks() #{0}", i));
			printH(h.h);
		}
        return h;
    }

    MapAndMesh setSeaLevel(MapAndMesh h, float q) {
        Debug.Log("setSeaLevel()");
        MapAndMesh newh = zero(h.mesh);
        int deltaId = quantile(h, q);
		float delta = h.h[deltaId];
		Debug.Log(String.Format("Set sea Level to {0}, delta: {1}", q, delta));
        for (int i = 0; i < h.h.Length; i++) {
			newh.h[i] = h.h[i] - delta;
        }
        return newh;
    }

    MapAndMesh cleanCoast(MapAndMesh h, int iters) {
        Debug.Log("cleanCoast()");
        for (int iter = 0; iter < iters; iter++) {
            int changed = 0;
            MapAndMesh newh = zero(h.mesh);
            for (int i = 0; i < h.h.Length; i++) {
                newh.h[i] = h.h[i];
                List<int> nbs = neighbours(h.mesh, i);
                if (h.h[i] <= 0 || nbs.Count() < 3) continue;
                int count = 0;
                float best = -999999;
                for (int j = 0; j < nbs.Count(); j++) {
                    if (h.h[nbs[j]] > 0) {
                        count++;
                    } else if (h.h[nbs[j]] > best) {
                        best = h.h[nbs[j]];    
                    }
                }
                if (count > 1) continue;
                newh.h[i] = best / 2;
                changed++;
            }

            h = newh;
            newh = zero(h.mesh);
            for (int i = 0; i < h.h.Length; i++) {
                newh.h[i] = h.h[i];
                List<int> nbs = neighbours(h.mesh, i);
                if (h.h[i] > 0 || nbs.Count() < 3) continue;
                int count = 0;
                float best = 999999;
                for (int j = 0; j < nbs.Count(); j++) {
                    if (h.h[nbs[j]] <= 0) {
                        count++;
                    } else if (h.h[nbs[j]] < best) {
                        best = h.h[nbs[j]];
                    }
                }
                if (count > 1) continue;
                newh.h[i] = best / 2;
                changed++;
            }

            h = newh;
        }
        return h;
    }

    private float[] trislope(MapAndMesh h, int i) {

        List<int> nbs = neighbours(h.mesh, i);
		//Debug.Log(string.Format("nbs[{0}].Count = {1}", i, nbs.Count));
        if (nbs.Count < 3) {
            float[] retu = { 0.0f, 0.0f };
            return retu;
        }

		//Debug.Log(string.Format("nbs[{0}] = {1}", i, nbs.ToString()));

        Vector2 p0 = h.mesh.vxs[nbs[0]];
        Vector2 p1 = h.mesh.vxs[nbs[1]];
        Vector2 p2 = h.mesh.vxs[nbs[2]];

        float x1 = p1.x - p0.x;
        float x2 = p2.x - p0.x;
        float y1 = p1.y - p0.y;
        float y2 = p2.y - p0.y;

        float det = x1 * y2 - x2 * y1;
        float h1 = h.h[nbs[1]] - h.h[nbs[0]];
        float h2 = h.h[nbs[2]] - h.h[nbs[0]];

        float[] ret = {(y2 * h1 - y1 * h2) / det,
                (-x2 * h1 + x1 * h2) / det };
        //if(Mathf.Abs(ret[0] + ret[1]) > 10)
        //{
        //    Debug.Log(string.Format("vxs[{0}].ret = ({1}, {2})", i, ret[0], ret[1]));
        //}

        return ret; 
    }

    private MapAndMesh cityScore(MapAndMesh h, List<int> cities) {

        //Debug.Log("cityScore()");
        MapAndMesh score = getFlux(h);
        score.h.Select(x => Mathf.Sqrt(x));

        for (int i = 0; i < h.h.Length; i++) {
            if (h.h[i] <= 0 || isnearedge(h.mesh, i)) {
                score.h[i] = -999999;
                continue;
            }
        score.h[i] += 0.01f / (1e-9f + Math.Abs(h.mesh.vxs[i][0]) - h.mesh.extentX / 2);
        score.h[i] += 0.01f / (1e-9f + Math.Abs(h.mesh.vxs[i][1]) - h.mesh.extentY / 2);
            for (int j = 0; j < cities.Count(); j++) {
                score.h[i] -= 0.02f / (distance(h.mesh, cities[j], i) + 1e-9f);
            }
        }
        return score;
    }

    private int placeCity(Render render) {

		Debug.Log("placeCity()");

        if (render.cities == null)
        {
            render.cities = new List<int>();
        }
            
        MapAndMesh score = cityScore(render.h, render.cities);
        float max = score.h.Max();
        int newcity = Array.IndexOf(score.h, max); //d3.scan(score.h, d3.descending); //get index of max(score.h[])
		Debug.Log(string.Format("New city: {0}, score: {1}", newcity, max));

		return newcity;
    }

    private List<int> placeCities(Render render, int n) {

		Debug.Log("placeCities " + n);
		//Params p = render.p;
		//int n = p.ncities;

		List<int> cities = new List<int>(n);

        for (int i = 0; i < n; i++) {
            int newCity = placeCity(render);
			cities.Add(newCity);
			render.cities = cities;
		}

		for (int i = 0; i < n; i++)
		{
			Debug.Log(i + ": " + cities[i]);
		}

		return cities;
    }

    List<int[]> contour(MapAndMesh h, float level = 0.0f) {

		Debug.Log("countour() level = " + level);
        List<int[]> edges = new List<int[]>();

		for (int i = 0; i < h.mesh.edges.Count; i++)
		{
			int[] e = h.mesh.edges[i];
			
			if (isnearedge(h.mesh, (int)e[0]) || isnearedge(h.mesh, (int)e[1])) continue;
			if ((h.h[(int)e[0]] > level && h.h[(int)e[1]] <= level) ||
				(h.h[(int)e[1]] > level && h.h[(int)e[0]] <= level))
			{
				int[] newEdge = { e[0], e[1] };
				edges.Add(newEdge);
			}
		}
		return edges; //mergeSegments(edges);
	}

	List<int[]> getRivers(MapAndMesh h, float limit) {

		Debug.Log("getRivers() limit = " + limit);

        int[] dh = downhill(h);
        MapAndMesh flux = getFlux(h);
        List<int[]> links = new List<int[]>();
        int above = 0;
        for (int i = 0; i < h.h.Length; i++) {
            if (h.h[i] > 0) above++;
        }
        limit *= above / h.h.Length;
        for (int i = 0; i < dh.Length; i++) {
            if (isnearedge(h.mesh, i)) continue;
            if (flux.h[i] > limit && h.h[i] > 0 && dh[i] >= 0) {
				int up = i; // h.mesh.vxids[i];
				int down = dh[i]; // h.mesh.vxids[dh[i]];
								  //if (h.h[dh[i]] > 0) { //we don't care anymore. if dh[i] < 0, i is on the coast, it's ok to assign the river to end in the ocean
				int[] newLink = new int[2] { up, down };
                links.Add(newLink);
                //} else { //this simply places the downslope half of the segment halfway between the vertices, so that the river is not drawn flowing past the coast
                //    Vector2 down2 = new Vector2( (up[0] + down[0])/2, (up[1] + down[1])/2 );
                //    List<Vector2> newLink = new List<Vector2>(2);
                //    newLink.Add(up);
                //    newLink.Add(down2);
                //    links.Add(newLink);
                //}
            }
        }

        return links; // mergeSegments(links).Select(x => relaxPath(x)); //we don't care about merging segments or smoothing the paths
    }

   

	//List<float[]> relaxPath(List<float[]> path) {
 //       List<float[]> newpath = new List<float[]>(path.Count()); // R[path[0]];
 //       newpath[0] = path[0];
 //       for (int i = 1; i < path.Count() - 1; i++) {
 //           float[] newpt = { 0.25f * path[i-1][0] + 0.5f * path[i][0] + 0.25f * path[i+1][0],
 //                        0.25f * path[i-1][1] + 0.5f * path[i][1] + 0.25f * path[i+1][1] };
 //           newpath[i] = newpt;
 //       }
 //       newpath[path.Count() - 1] = path[path.Count() - 1];

 //       return newpath;
 //   }

    MapAndMesh dropEdge(MapAndMesh h, int p = 4) {
        Debug.Log("dropEdge");
        MapAndMesh newh = zero(h.mesh);
        for (int i = 0; i < h.h.Length; i++) {
            Vector2 v = h.mesh.vxs[i];
            float x = 2.4f*v[0] / h.mesh.extentX;
            float y = 2.4f*v[1] / h.mesh.extentY;
            newh.h[i] = h.h[i] - Mathf.Exp(10*(Mathf.Pow(Mathf.Pow(x, p) + Mathf.Pow(y, p), 1/p) - 1));
        }
        return newh;
    }

    Render generateCoast(Params p) {
		Render render = new Render();
        Debug.Log("generateCoast()");
        Mesh mesh = generateGoodMesh(p.npts, p.extentX, p.extentY);
        MapAndMesh step1 = slope(mesh, UnityEngine.Random.insideUnitCircle.normalized * 4); // randomVector(4));
        Debug.Log("Step 1:");
        //printH(step1.h);
        
        MapAndMesh step2 = cone(mesh, runif(-1, -1));
        Debug.Log("Step 2:");
        //printH(step2.h);
        MapAndMesh step3 = mountains(mesh, 50);
        Debug.Log("Step 3:");
        //printH(step3.h);
        MapAndMesh[] steps = { step1, step2, step3 };

        MapAndMesh h = add(steps);
        Debug.Log("Added:");
        //printH(h.h); //, true);
        Debug.ClearDeveloperConsole();
        for (int i = 0; i < 10; i++) {
            h = relax(h);
            Debug.Log("Relax # " + i);
            printH(h.h);
        }
        
        h = peaky(h);
        Debug.Log("Peaky:");
        printH(h.h);
        h = doErosion(h, runif(0, 0.1f), 5);
        Debug.Log("doErosion:");
        printH(h.h);
		h.h = normalize(h.h);
		printH(h.h);
		h = setSeaLevel(h, runif(0.2f, 0.6f));
        Debug.Log("setSeaLevel:");
        printH(h.h);
        h = fillSinks(h);
        Debug.Log("fillSinks:");
        printH(h.h);
        h = cleanCoast(h, 3);
        Debug.Log("cleanCoast:");
        printH(h.h);

		

		render.h = h;
		Debug.Log("render.h:");
		printH(render.h.h);
		render.coast = contour(render.h);
		render.rivers = getRivers(render.h, 0.01f);

		render.cities = placeCities(render, p.ncities);


		Debug.Log(render.ToString());

        return render;
    }

/*    float[] terrCenter(MapAndMesh h, terr, city, bool landOnly) {
        var x = 0;
        var y = 0;
        var n = 0;
        for (var i = 0; i < terr.length; i++) {
            if (terr[i] != city) continue;
            if (landOnly && h[i] <= 0) continue;
            x += terr.mesh.vxs[i][0];
            y += terr.mesh.vxs[i][1];
            n++;
        }
        return [x/n, y/n];
    }*/
}