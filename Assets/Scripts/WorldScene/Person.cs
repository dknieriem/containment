using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public partial class Person : IEquatable<Person>
{

    public int Id;
	public string FirstName;
	public string LastName;
    public int age;
	public int LocationX;
	public int LocationY;
	public Sector CurrentSector;
	public CharacterState CurrentState;
	public Role CurrentRole;
		
	public Group MyGroup;
    public List<Relationship> Relationships; //Person[] Relationships;
	//public float[] RelationshipStrengths;
		
	public float[] BaseStats;
	public float[] CurrentStats;
	public float[] Skills;
	public int LifetimeZedKills;
	public int LifetimeHumanKills;

    public bool infoChangedLastTick = false;
    public bool isHome;

    public static string[] DataNamesFirstMale;
	public static string[] DataNamesFirstFemale;
	public static string[] DataNamesLast;
    public static int nextId = 0;

	public enum Stance
	{
		Hiding,
		Avoidant,
		Defensive,
		Aggressive}

	;

	public enum LocationInSector
	{
		Indoors,
		Outdoors,
		InVehicle}

	;


	//TODO: Queue / BehaviorTree of actions to perform
		
	public Person ()
	{
        Id = Person.nextId++;
        FirstName = "Blankity";
		LastName = "Blank";
		CurrentState = Person.CharacterState.Idle;
		CurrentRole = Person.Role.None;
		BaseStats = new float[Enum.GetNames (typeof(Person.Stats)).Length];
		CurrentStats = new float[Enum.GetNames (typeof(Person.Stats)).Length];
		Skills = new float[Enum.GetNames (typeof(Person.Skill)).Length];
        
        Relationships = new List<Relationship>();
	}

   	public static Person CreateRandomCharacter (Group myGroup)
	{
        Person x = new Person ();
		x.MyGroup = myGroup;
		x.CurrentSector = myGroup.HomeSector;
		x.LocationX = x.CurrentSector.LocationX;
		x.LocationY = x.CurrentSector.LocationY;
        x.isHome = true;
		x.FirstName = DataNamesFirstMale [UnityEngine.Random.Range (1, DataNamesFirstMale.Length) - 1];
		x.LastName = DataNamesLast [UnityEngine.Random.Range (1, DataNamesLast.Length) - 1];

        x.age = UnityEngine.Random.Range(18, 65);
		x.BaseStats [(int)Stats.Health] = UnityEngine.Random.Range (8000, 10000) / 100.0f;
		x.BaseStats [(int)Stats.Stamina] = UnityEngine.Random.Range (7000, 9000) / 100.0f;
		x.BaseStats [(int)Stats.KillRate] = UnityEngine.Random.Range (1000, 2000) / 100.0f;
		
		x.CurrentStats [(int)Stats.Health] = x.BaseStats [(int)Stats.Health];
		x.CurrentStats [(int)Stats.Stamina] = x.BaseStats [(int)Stats.Stamina];
		x.CurrentStats [(int)Stats.KillRate] = 0;
				
		x.Skills [(int)Skill.Leadership] = UnityEngine.Random.Range (0.0f, 60.0f);
		if (x.Skills [(int)Skill.Leadership] < 30.0f) {
			x.Skills [(int)Skill.Leadership] = 0.0f;
		}
		
		return x;
	}

    public static Person CreateFromProperties(CharacterPropertySet characterProperties, Group myGroup)
    {
        Person x = new Person();
        x.MyGroup = myGroup;
        if(myGroup.HomeSector != null)
        {
            x.CurrentSector = myGroup.HomeSector;
            x.LocationX = x.CurrentSector.LocationX;
            x.LocationY = x.CurrentSector.LocationY;
            x.isHome = true;
        }

        x.FirstName = characterProperties.GetStringValue("First Name");
        x.LastName = characterProperties.GetStringValue("Last Name");
        x.age = characterProperties.GetIntValue("Age");
        x.BaseStats[(int)Stats.Health] = UnityEngine.Random.Range(8000, 10000) / 100.0f;
        x.BaseStats[(int)Stats.Stamina] = UnityEngine.Random.Range(7000, 9000) / 100.0f;
        x.BaseStats[(int)Stats.KillRate] = UnityEngine.Random.Range(1000, 2000) / 100.0f;

        x.CurrentStats[(int)Stats.Health] = x.BaseStats[(int)Stats.Health];
        x.CurrentStats[(int)Stats.Stamina] = x.BaseStats[(int)Stats.Stamina];
        x.CurrentStats[(int)Stats.KillRate] = 0;

        x.Skills[(int)Skill.Leadership] = UnityEngine.Random.Range(50.0f, 75.0f);

        if (x.Skills[(int)Skill.Leadership] < 30.0f)
        {
            x.Skills[(int)Skill.Leadership] = 0.0f;
        }

        return x;
    }

    public void SetRelationships(List<Relationship> newRels)
    {
        Relationships = newRels;
    }

    public void SetRelationship(Person person, float baseStrength)
    {

        Relationship existing = FindRelationshipWithPerson(person);

        if(existing == null)
        {
            Relationships.Add(new Relationship(this, person, baseStrength));
        } else
        {
            Debug.Log("Resetting relationship!");
            existing.ModifyBaseScore(baseStrength);
            existing.RemoveAllModifiers();
        }
            
    }

	public void SetRelationships (Person[] people, float[] strengths)
	{
		Relationships = new List<Relationship>();
		
		for (int i = 0; i < people.Length; i++) {
            SetRelationship(people[i], strengths[i]);
		}
		
	}

    public void AddRelationship(Relationship relationship)
    {
        if(!relationship.personOne.Equals(this) && !relationship.personTwo.Equals(this))
        {
            Debug.Log(string.Format("{0} is not in the relationship between {1} and {2}", this.Id, relationship.personOne.Id, relationship.personTwo.Id));
            return;
        } else
        {
            Relationships.Add(relationship);
        }

    }

    public void AddRelationship(Person person, float baseStrength)
    {
        Relationship existing = FindRelationshipWithPerson(person);

        if(existing == null)
        {
            Relationships.Add(new Relationship(this, person, baseStrength));
        } else
        {
            Debug.Log("Overwriting existing relationship!");
            existing.ModifyBaseScore(baseStrength);
            existing.RemoveAllModifiers();
        }
    }

	public void DoNextUpdate ()
	{
        infoChangedLastTick = false;
        UpdateLocation();
		UpdateAttributes ();
		UpdateSkills ();
		UpdateStats ();
        UpdateRelationships();
		
	}

    void UpdateLocation()
    {
        
        if(CurrentSector == MyGroup.HomeSector)
        {
            isHome = true;
        } else
        {
            isHome = false;
        }
    }

	void UpdateAttributes ()
	{
		
	}

	void UpdateSkills ()
	{
		
	}

	void UpdateStats ()
	{
		for (int k = 0; k < CurrentStats.Length; k++) {
			if (CurrentStats [k] < 0) {
				CurrentStats [k] = 0;
			}
		}
	}

    void UpdateRelationships ()
    {
        foreach(Relationship relationship in Relationships)
        {
            relationship.Update();
        }
    }

	public void AddZedKills (int numZedsKilled)
	{
		LifetimeZedKills += numZedsKilled;
		//TODO: add experience for each kill,
								
		Person[] sectorMates = MyGroup.GetMembersInSector (LocationX, LocationY);

        DateTime expiration = GameManager.Instance.world.CurrentDate;

        expiration += new TimeSpan(7, 0, 0, 0);

        for (int j = 0; j < sectorMates.Length; j++) {
			if (sectorMates [j].CurrentRole == Role.Guard || sectorMates [j].CurrentRole == Role.Scout || sectorMates [j].CurrentRole == Role.Looter) { //we have a match!

                //AdjustRelationship (sectorMates [j], 0.5f);
                AddRelationshipModifier(sectorMates[j], new RelationshipModifier(expiration, "Killed some zeds together", 10.0f, true));
			}
		}
	}

    private Relationship FindRelationshipWithPerson(Person person)
    {
        foreach(Relationship relationship in Relationships)
        {
            if (relationship.Has(person))
            {
                return relationship;
            }
        }

        return null;
    }

	public void AddRelationshipModifier (Person otherPerson, RelationshipModifier modifier)
	{
        //otherPerson.AdjustRelationship(this, adjustment); From addzedkills, this will be done in other person's update.

        Relationship existing = FindRelationshipWithPerson(otherPerson);

        if(existing != null)
        {
            existing.AddModifier(modifier);
        }

	}

    public bool Equals(Person other)
    {
        if (other == null)
            return false;

        return this.Id.Equals(other.Id) &&
            (
                object.ReferenceEquals(this.FirstName, other.FirstName) ||
                this.FirstName != null &&
                this.FirstName.Equals(other.FirstName)
            ) &&
            (
                object.ReferenceEquals(this.LastName, other.LastName) ||
                this.LastName != null &&
                this.LastName.Equals(other.LastName)
            );
    }

    public bool SetLocation(int sectorX, int sectorY)
    {
        if(sectorX == LocationX && sectorY == LocationY)
        {
            return false;
        }

        try
        {
            Sector newSector = GameManager.Instance.world.GetSectorFromCoords(sectorX, sectorY);
            CurrentSector = newSector;
            LocationX = sectorX;
            LocationY = sectorY;
            infoChangedLastTick = true;
            return true;
            
        } catch(System.ArgumentOutOfRangeException e)
        {
            Debug.Log(e.ToString());
            return false;
        }

    }
}
