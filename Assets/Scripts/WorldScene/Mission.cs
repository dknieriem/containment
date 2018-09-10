using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour {

    public Group group;
    public Party party;
    public Sector targetSector;

    public enum MissionType
    {
        scavenge,
        clear,
        rebuild
    };

    public bool check()
    {
        //If the mission is done

        return false;
    }
}
