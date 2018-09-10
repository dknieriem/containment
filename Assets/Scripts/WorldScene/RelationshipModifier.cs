using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipModifier : MonoBehaviour {

    //static enum constraintType { };

    public bool isUnique = false;
    
    public float delta = 0.0f;

    public string explanation = "explanation text";

    public DateTime startDate;

    public DateTime expiration;

    public RelationshipModifier(string explanation, float delta)
    {
        this.explanation = explanation;

        this.delta = delta;

        this.startDate = GameManager.Instance.world.CurrentDate;


    }

    public RelationshipModifier(DateTime expiration, string explanation = "explanation text", float delta = 0.0f, bool isUnique = false)
    {
        this.explanation = explanation;

        this.delta = delta;

        this.isUnique = isUnique;
        
        this.expiration = expiration;

    }


    public bool check(Relationship relationship)
    {

        if (expiration < GameManager.Instance.world.CurrentDate)
        {
            return false;
        }



        return true;
    }

}
