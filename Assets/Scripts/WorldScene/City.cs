using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    string Name;
    List<Sector> Sectors;
    int[] Center;
    int zedPop;

    bool isCleared;
    bool isVisible;
    bool isVisited;

    public World world;
    City(int[] center, int radius)
	{

        Center = center;
		Sectors = new List<Sector>();
		addSectors(center, radius);
    }
    // Use this for initialization
    void Start () {
        world = GameManager.Instance.world;
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    void addSectors(int[] center, int radius)
    {

        for (int x = center[0] - radius; x < center[0] + radius; x++)
        {

            for (int y = center[1] - radius; y < center[1] + radius; y++)
            {
                int[] position = new int[] { x, y };
                if (x >= 0 && x < world.DimensionsX && y >= 0 && y < world.DimensionsY)
                {
                    int d = Sector.Distance(position, center);

                    if (d < radius)
                    {
                        Sector toAdd = world.GetSectorFromCoords(position);
                        Sectors.Add(toAdd);
                    }
                }
            }
		}
	}	
}
