using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship
{ // : MonoBehaviour 

    const float minScore = -100.0f;
    const float maxScore = 100.0f;
	const float epsilon = 0.001f;

    public Person personOne;
    public Person personTwo;
    public int Id;

    float baseScore = 0;
    float currentScore = 0;
    DateTime beginning;

    public static int nextId = 0;
    public static List<RelationshipModifier> modifierPrototypes;
    public List<RelationshipModifier> modifiers;

    public bool ModsChangedLastTick = false;
    public bool ScoreChangedLastTick = false;
    public DateTime lastUpdated;

    public float BaseScore
    {
        get
        {
            return baseScore;
        }

        set
        {
            baseScore = value;
        }
    }

    public float CurrentScore
    {
        get
        {
            return currentScore;
        }

        set
        {
            currentScore = value;
        }
    }

    public Relationship(Person one, Person two, float baseSc)
    {
        personOne = one;
        personTwo = two;
        Id = nextId++;

        baseScore = currentScore = baseSc;

        modifiers = new List<RelationshipModifier>();
        beginning = GameManager.Instance.world.CurrentDate;
        lastUpdated = beginning;

        one.AddRelationship(this);
        two.AddRelationship(this);
    }

    public static void AddModifierPrototype( RelationshipModifier modifier)
    {
        modifierPrototypes.Add(modifier);
    }
    public void ModifyBaseScore( float delta)
    {
        if(delta == 0)
        {
            return;
        }

        baseScore += delta;
        calculateScore();
        ScoreChangedLastTick = true;
    }

    void calculateScore()
    {
        currentScore = baseScore;

        foreach(RelationshipModifier modifier in modifiers)
        {
            currentScore += modifier.delta;
        }

        clampScores();
    }


    void updateModifiers()
    {

        //first update existing modifiers
        foreach(RelationshipModifier modifier in modifiers)
        {
           if(! modifier.check(this))
            {
                modifiers.Remove(modifier);
                ModsChangedLastTick = true;
            }
        }

        //second, check if any new modifiers apply
    }
    public void Update()
    {
        if(lastUpdated == GameManager.Instance.world.CurrentDate)
        {
            Debug.Log("Already updated");
            return;
        }
        ModsChangedLastTick = false;
        ScoreChangedLastTick = false;
        updateModifiers();
        UpdateBaseScore();
        calculateScore();
        lastUpdated = GameManager.Instance.world.CurrentDate;
    }

    void clampScores()
    {
        if (baseScore < minScore)
            baseScore = minScore;

        if (currentScore < minScore)
            currentScore = minScore;

        if (baseScore > maxScore)
            baseScore = maxScore;

        if (currentScore > maxScore)
            currentScore = maxScore;
    }

    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }

    void UpdateBaseScore()
    {
        DateTime currentDate = GameManager.Instance.world.CurrentDate;

        //add to base score based on current score / total length of relationship

        float length = (float) (currentDate - beginning).TotalHours;

        if (length > 0)
        {
            float delta = currentScore / length;
            Debug.Log(string.Format("Rel {0}-{1} delta: {2}", personOne.FirstName, personTwo.FirstName, delta));
            ModifyBaseScore(delta);
        }

    }

    public bool Has(Person person)
    {
        if(personOne == person || personTwo == person)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void AddModifier(RelationshipModifier modifier)
    {
        modifiers.Add(modifier);
        ModsChangedLastTick = true;
    }

    public bool Equals(Relationship other)
    {
        if (System.Object.ReferenceEquals(this, other))
            return true;

        if (other == null)
            return false;

        return this.personOne.Equals(other.personOne) &&
            this.personTwo.Equals(other.personTwo) &&
            this.baseScore == other.baseScore &&
            this.currentScore == other.currentScore;
    }
}
