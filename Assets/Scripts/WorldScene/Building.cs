using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{

    public int NumberOfRooms = 1;

    public int NumberOfZeds;

    public float defenseLevel = 1.0f;
		
	public bool BuildingIsSecure = true;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
        checkBuildingIsSecure();
	}

    public void updateDefenseLevel(float delta)
    {
        defenseLevel -= delta;

        if(defenseLevel < 0)
        {
            defenseLevel = 0;
        }
    }

    void checkBuildingIsSecure()
    {
        if(defenseLevel <= 0 || NumberOfZeds > 0)
        {
            BuildingIsSecure = false;
        } else
        {
            BuildingIsSecure = true;
        }
    }

}
