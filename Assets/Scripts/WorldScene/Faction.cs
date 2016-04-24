using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Faction : MonoBehaviour
{
	public int GroupStartingSize = 5;
	public static int MaxMembersPerSquadAllowed = 16;
	public Sector HomeSector;
	public int HomeSectorLocationX, HomeSectorLocationY;
	public List<Person> GroupMembers;
	public Person GroupLeader;
	//public float[][] RelationshipStrengths;
	//public float[][] LeadershipLinks;
	public int TotalGroupMembers;
	//public ArrayList GroupMemberLocations = new ArrayList ();
	public int[,] SectorGroupMembers;
		
	//public float[] GroupStatTotals;
	public bool groupInfoDirty = true;
	//public ArrayList GroupMemberNames = new ArrayList ();
		
	GameManager gameManager;
	//World world;
						
	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Starting: PlayerGroup");
		gameManager = GameManager.Instance;
	}

	public void NewGroup ()
	{
		Debug.Log ("PlayerGroup " + this.name + ".NewGroup()");
		World world = gameManager.world;
		Debug.Log (world.ToString ());
		Debug.Log ("World Size (pg.cs): " + world.DimensionsX + ", " + world.DimensionsY);
		SectorGroupMembers = new int[world.DimensionsX, world.DimensionsY];
		Debug.Log ("SGM: " + SectorGroupMembers.ToString ());
		//Debug.Log ("GroupMember Size: " + SectorGroupMembers.Length);			
		GroupMembers = new List<Person> (GroupStartingSize);
		StartGroup (GroupStartingSize, UnityEngine.Random.Range (0, 10), UnityEngine.Random.Range (0, 10));
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

	public void StartGroup (int numMembers, Sector homeSector)
	{
		Debug.Log ("Group Initializing... " + numMembers + " members, home is " + homeSector.ToString () + ".");
		TotalGroupMembers = numMembers;
		InitializeHomeSector (homeSector);
												
		for (int i = 0; i < numMembers; i++) {
			Person newPerson = Person.CreateRandomCharacter (this);
			newPerson.LocationX = homeSector.LocationX;
			newPerson.LocationY = homeSector.LocationY;
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
		Sector homeSector = world.WorldSectors [homeSectorX, homeSectorY];
		InitializeHomeSector (homeSector);

		for (int i = 0; i < numMembers; i++) {
			Person newPerson = Person.CreateRandomCharacter (this);
			newPerson.LocationX = homeSectorX;
			newPerson.LocationY = homeSectorY;
			GroupMembers.Add (newPerson);
		}
				
		GenerateRandomRelationships ();
		CopyRelationshipsToPeople ();
	}

	public void InitializeHomeSector (Sector newLocation)
	{
		SetHomeSector (newLocation);
		SectorGroupMembers [newLocation.LocationX, newLocation.LocationY] = TotalGroupMembers;
		HomeSector.PlayerGroupCount = TotalGroupMembers;
		HomeSector.IsVisible = true;
		HomeSector.IsVisited = true;
	}

	public void SetHomeSector (Sector newLocation)
	{
		HomeSector = newLocation;
		HomeSectorLocationX = HomeSector.LocationX;
		HomeSectorLocationY = HomeSector.LocationY;
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
}
