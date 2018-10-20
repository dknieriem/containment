using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    public string Name;
    public List<Sector> Sectors;
    public int[] Center;
    public int zedPop;

    public bool isCleared;
    public bool isVisible;
    public bool isVisited;

    public World world;
    City(int[] center, int radius)
	{

        Center = center;
		Sectors = new List<Sector>();

    }
    // Use this for initialization
    void Start () {
        world = GameManager.Instance.world;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
