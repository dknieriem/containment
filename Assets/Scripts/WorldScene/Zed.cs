using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zed : MonoBehaviour {

    public Sector location;

    public const float maxHealth = 100.0f;

    public float health;

    public void doUpdate()
    {
        checkHealth();
    }

    void checkHealth()
    {
        if(health < 0)
        {
            die();
        }

    }

    void die()
    {
        //TODO: something
    }
}
