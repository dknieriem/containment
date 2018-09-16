using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Group : MonoBehaviour
{
    public static int MaxMembersPerSquadAllowed = 16;
    public static Group PlayerGroup;

    public GroupPropertySet GroupProperties;

    public bool IsPlayerGroup;
	public Sector HomeSector;
	//public int HomeSectorLocationX, HomeSectorLocationY;
	public List<Person> GroupMembers;
    public List<Party> Parties;
	public Person GroupLeader;

	public int TotalGroupMembers;

	public int[,] SectorGroupMembers;

    public bool groupInfoDirty; // = true;
		
	GameManager gameManager;
						
	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Starting: PlayerGroup");
		gameManager = GameManager.Instance;
	}

	public void NewGroup (GroupPropertySet groupProperties, CharacterPropertySet characterProperties, bool isPlayerGroup)
	{

        GroupProperties = groupProperties;
        IsPlayerGroup = isPlayerGroup;
        if (IsPlayerGroup)
        {
            Debug.Log("is player group");
            Group.PlayerGroup = this;
        }

        TotalGroupMembers = GroupProperties.GetIntValue("Group Members");
		Debug.Log ("PlayerGroup " + this.name + ".NewGroup()");
        Debug.Log("total group members " + TotalGroupMembers);
		World world = gameManager.world;
		Debug.Log (world.ToString ());
		Debug.Log ("World Size (pg.cs): " + world.DimensionsX + ", " + world.DimensionsY);
		SectorGroupMembers = new int[world.DimensionsX, world.DimensionsY];
		Debug.Log ("SGM: " + SectorGroupMembers.ToString ());
		//Debug.Log ("GroupMember Size: " + SectorGroupMembers.Length);			
		GroupMembers = new List<Person> (TotalGroupMembers);

        HomeSector = world.GetSectorFromCoords(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
        StartGroup(TotalGroupMembers, characterProperties);

        InitializeHomeSector();

        //gameManager.uiHandler.playerGroupInfoPanel.GetComponent<GroupInfoPanel>().ManualUpdate();
		groupInfoDirty = true;
	}

	void UpdateGroupMemberCounts ()
	{
		World world = gameManager.world;
		SectorGroupMembers = new int[world.DimensionsX, world.DimensionsY];
//		Debug.Log ("SGM: " + SectorGroupMembers.ToString ());
//		Debug.Log ("GM: " + GroupMembers.ToString ());
		foreach (Person p in GroupMembers) {
			int[] sectorToAdd = { p.LocationX, p.LocationY };
			SectorGroupMembers [sectorToAdd [0], sectorToAdd [1]]++;
		}	
	}

	void UpdateGroupMembers ()
	{
		foreach (Person p in GroupMembers) {
			p.DoNextUpdate ();
		}
	}
	//updated each game hour
	public void DoNextUpdate ()
	{
        groupInfoDirty = false;
		UpdateGroupMembers ();
		UpdateGroupMemberCounts ();
		UpdateGroupStats ();
	}

	void UpdateGroupStats ()
	{
		//	GroupStatTotals = new float[Enum.GetNames (typeof(Person.Stats)).Length];
		//	for (int i = 0; i < GroupMembers.Count; i++) {
		//		Person p = GroupMembers [i];
		//		for (int k = 0; k < GroupStatTotals.Length; k++) {
		//			GroupStatTotals [k] += p.CurrentStats [k];
		//		}
		//	}
	}

	public void StartGroup (int numMembers, CharacterPropertySet characterProperties)
	{
		Debug.Log ("Group Initializing... " + numMembers + " members");
		TotalGroupMembers = numMembers;

        Person leader = Person.CreateFromProperties(characterProperties, this);
        GroupMembers.Add(leader);
        GroupLeader = leader;

		for (int i = 1; i < numMembers; i++) {
			Person newPerson = Person.CreateRandomCharacter (this);
			GroupMembers.Add (newPerson);
		}
				
		GenerateRandomRelationships ();
		CopyRelationshipsToPeople ();
		CalculatePowerStructure ();
	}

	public void StartGroup (int numMembers, int homeSectorX, int homeSectorY)
	{
		World world = gameManager.world;
		TotalGroupMembers = numMembers;
		Debug.Log ("Adding " + numMembers + " to (" + homeSectorX + ", " + homeSectorY + ").");
		HomeSector = world.WorldSectors [homeSectorX, homeSectorY];
		InitializeHomeSector ();

		for (int i = 0; i < numMembers; i++) {
			Person newPerson = Person.CreateRandomCharacter (this);
			newPerson.LocationX = homeSectorX;
			newPerson.LocationY = homeSectorY;
			GroupMembers.Add (newPerson);
		}
				
		GenerateRandomRelationships ();
		CopyRelationshipsToPeople ();
	}

	public void InitializeHomeSector ()
    {

        Debug.Log("Initialize Home Sector, home is " + HomeSector.ToString() + ".");

        //SetHomeSector (newLocation);

        //foreach(Person member in GroupMembers)
        //{
        //    member.LocationX = HomeSector.LocationX;
        //    member.LocationY = HomeSector.LocationY;
        //}

        SectorGroupMembers [HomeSector.LocationX, HomeSector.LocationY] = TotalGroupMembers;
        HomeSector.PlayerGroupCount = TotalGroupMembers;
		HomeSector.IsVisible = true;
		HomeSector.IsVisited = true;
	}

	public void SetHomeSector (Sector newLocation)
	{
		HomeSector = newLocation;
        groupInfoDirty = true;
        //HomeSectorLocationX = HomeSector.LocationX;
		//HomeSectorLocationY = HomeSector.LocationY;
	}

	public float GetDoctorSkillInSector (Sector currentSector)
	{
		float doctorSkill = 0;
		
		foreach (Person member in GroupMembers) {
			if (member.CurrentSector == currentSector && member.CurrentRole == Person.Role.Doctor) {
				doctorSkill += member.Skills [(int)Person.Skill.Doctor];
			}
		
		}
		return doctorSkill;
	}

	public void GenerateRandomRelationships ()
	{
        for(int i = 0; i < TotalGroupMembers; i++)
        {
            Person one = GroupMembers.ElementAt<Person>(i);
            for(int j = i+1; j < TotalGroupMembers; j++)
            {
                Person two = GroupMembers.ElementAt<Person>(j);
                float baseScore = UnityEngine.Random.Range(10.0f, 20.0f);
                //Relationship newRelationship = 
                new Relationship(one, two, baseScore);
            }
        }
		//	RelationshipStrengths = new float[TotalGroupMembers][];
				
		//	for (int i = 0; i < TotalGroupMembers; i++) {
		//		RelationshipStrengths [i] = new float[TotalGroupMembers];				
		//		for (int j = 0; j <= i; j++) {
		//			if (i == j) {
		//				RelationshipStrengths [i] [j] = 100.0f;
		//			} else {
		//				float strengthItoJ = UnityEngine.Random.Range (10.0f, 90.0f);
		//				float strengthJtoI = UnityEngine.Random.Range (10.0f, 90.0f);
		//				float deltaStrength = strengthItoJ - strengthJtoI;
		//				strengthItoJ -= deltaStrength / 4;
		//				strengthJtoI += deltaStrength / 4;
		//				RelationshipStrengths [i] [j] = strengthItoJ;
		//				RelationshipStrengths [j] [i] = strengthJtoI;
		//			}
		//		}
		//	}
	}

	public void CopyRelationshipsToPeople ()
	{
		//	for (int i = 0; i < GroupMembers.Count; i++) {
		//		Person p = GroupMembers [i];
		//		p.SetRelationships (GroupMembers.ToArray (), RelationshipStrengths [i]);
		//	}
	}

	public void CalculatePowerStructure ()
	{
		//	LeadershipLinks = new float[TotalGroupMembers][];
		//	float[] leadershipPoints = new float[TotalGroupMembers]; 
		//Person[] PeopleSortedByLeadership = GroupMembers.OrderBy( p => p.Leadership ).ToArray();
				
		//	for (int i = 0; i < GroupMembers.Count; i++) {
		//		LeadershipLinks [i] = new float[TotalGroupMembers];				
		//		for (int j = 0; j < GroupMembers.Count; j++) {
		//			if (GroupMembers [i].Skills [(int)Person.Skill.Leadership] > GroupMembers [j].Skills [(int)Person.Skill.Leadership]) { 
		//				LeadershipLinks [i] [j] = (GroupMembers [j].Skills [(int)Person.Skill.Leadership] / 100.0f) * RelationshipStrengths [j] [i] * RelationshipStrengths [j] [i]; // i == j gives bonus equal to i's Leadership score
		//				leadershipPoints [i] += (GroupMembers [j].Skills [(int)Person.Skill.Leadership] / 100.0f) * RelationshipStrengths [j] [i] * RelationshipStrengths [j] [i];
		//			}
		//		}
		//	}
	}

	public Person[] GetMembersInSector (int locationX, int locationY)
	{
		Person[] members = GroupMembers.Where (p => p.LocationX == locationX && p.LocationY == locationY).ToArray ();
		
		return members;
	}

    public bool AddPersonToGroup(Person newPerson)
    {
    
        foreach(Person existingMember in GroupMembers)
        {
            if(newPerson.Equals(existingMember))
            {
                return false;
            }
        }

        GroupMembers.Add(newPerson);
        newPerson.MyGroup = this;
        return true;
    }
}
