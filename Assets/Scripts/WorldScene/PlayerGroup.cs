using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroup : MonoBehaviour
{
		int GroupStartingSize = 5;
		public static int MaxSquadsAllowed = 10;
		public static int MaxMembersAllowed = 100;
		public static int MaxMembersPerSquadAllowed = 16;
		public Sector HomeSector;
		public int[] HomeSectorLocation;
		public List<Person> GroupMembers;
		public Person GroupLeader;
		public float[][] RelationshipStrengths;
		public float[][] LeadershipLinks;
		public int TotalGroupMembers;
		//public ArrayList GroupMemberLocations = new ArrayList ();
		public int[,] SectorGroupMembers;
		
		public float[] GroupStatTotals;
		
		//public ArrayList GroupMemberNames = new ArrayList ();
		
//		GameWorld Game;
		WorldInfo World;
	
						
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: PlayerGroup");
				//		Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				World = GameObject.Find ("World").GetComponent<WorldInfo> ();
				//Debug.Log (World.ToString ());
				SectorGroupMembers = new int[World.Dimensions [0], World.Dimensions [1]];
				//Debug.Log ("GroupMember Size: " + SectorGroupMembers.Length);
				HomeSectorLocation = new int[2];			
				GroupMembers = new List<Person> (GroupStartingSize);
				StartGroup (GroupStartingSize, Random.Range (0, 10), Random.Range (0, 10));
		}
		
		public void GroupMemberMoved ()
		{
				SectorGroupMembers = new int[World.Dimensions [0], World.Dimensions [1]];
				foreach (Person p in GroupMembers) {
						int[] sectorToAdd = {p.LocationX, p.LocationY};
						SectorGroupMembers [sectorToAdd [0], sectorToAdd [1]] ++;
				}	
		}
		
		//updated each game hour
		public void DoNextUpdate ()
		{
				
				UpdateGroupStats ();
				DoAttacks ();
		}
				
		void UpdateGroupStats ()
		{
				GroupStatTotals = new float[Person.Stats.Length];
				for (int i = 0; i < GroupMembers.Count; i++) {
						Person p = GroupMembers [i];
						for(int k = 0; k < Person.Stats.Length; k++){
							GroupStatTotals[k] += p.Stats[k];
						}
				}
		}
				
		void DoAttacks ()
		{
		
				int[,] numZedsKilled = new int[World.Dimensions [0], World.Dimensions [1]];
				int[,] numCharsInSector = new int[World.Dimensions [0], World.Dimensions [1]];
				
				//for each group member in 
				for (int i = 0; i < GroupMembers.Count; i++) {
						Person p = GroupMembers [i];
						numZedsKilled [p.LocationX, p.LocationY] += (int)(p.CurrentAttackStrength * Random.Range (0.8f, 1.0f));
						numCharsInSector [p.LocationX, p.LocationY] ++;
						if (numZedsKilled [p.LocationX, p.LocationY] > World.WorldSectors [p.LocationX, p.LocationY].ZedCount) {
								numZedsKilled [p.LocationX, p.LocationY] = World.WorldSectors [p.LocationX, p.LocationY].ZedCount;
						}
						//TODO: EventHandler.NewMessage(p.FirstName + " " + p.LastName + " killed " + numZedsKilled + " zeds", p.LocationX, p.LocationY, World.CurrentDate);
				}		
				
				for (int i = 0; i < GroupMembers.Count; i++) {
						Person p = GroupMembers [i];
						int curCharZedKills = (int)(numZedsKilled [p.LocationX, p.LocationY] / p.CurrentAttackStrength);
						Debug.Log (string.Format ("{0} {1} killed {2} zeds at sector {3}, {4} on {5}", p.FirstName, p.LastName, curCharZedKills, p.LocationX, p.LocationY, World.CurrentDate));
						World.WorldSectors [p.LocationX, p.LocationY].ZedCount -= curCharZedKills;
						p.LifetimeZedKills += curCharZedKills;
				}	
				
		}
				
		public void StartGroup (int numMembers, Sector homeSector)
		{
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
				TotalGroupMembers = numMembers;
				//Debug.Log ("Adding " + numMembers + " to (" + homeSectorX + ", " + homeSectorY + ").");
				Sector homeSector = World.WorldSectors [homeSectorX, homeSectorY];
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
		}
		
		public void SetHomeSector (Sector newLocation)
		{
				HomeSector = newLocation;
				HomeSectorLocation [0] = HomeSector.LocationX;
				HomeSectorLocation [1] = HomeSector.LocationY;
		}
		
		public void GenerateRandomRelationships ()
		{
				RelationshipStrengths = new float[TotalGroupMembers][];
				
				for (int i = 0; i < TotalGroupMembers; i++) {
						RelationshipStrengths [i] = new float[TotalGroupMembers];				
						for (int j = 0; j <= i; j++) {
								if (i == j) {
										RelationshipStrengths [i] [j] = 100.0f;
								} else {
										float strengthItoJ = Random.Range (10.0f, 90.0f);
										float strengthJtoI = Random.Range (10.0f, 90.0f);
										float deltaStrength = strengthItoJ - strengthJtoI;
										strengthItoJ -= deltaStrength / 4;
										strengthJtoI += deltaStrength / 4;
										RelationshipStrengths [i] [j] = strengthItoJ;
										RelationshipStrengths [j] [i] = strengthJtoI;
								}
						}
				}
		}
		
		public void CopyRelationshipsToPeople ()
		{
				for (int i = 0; i < GroupMembers.Count; i++) {
						Person p = GroupMembers [i];
						p.SetRelationships (GroupMembers.ToArray (), RelationshipStrengths [i]);
				}
		}
		
		public void CalculatePowerStructure ()
		{
				LeadershipLinks = new float[TotalGroupMembers][];
				float[] leadershipPoints = new float[TotalGroupMembers]; 
				//Person[] PeopleSortedByLeadership = GroupMembers.OrderBy( p => p.Leadership ).ToArray();
				
				for (int i = 0; i < GroupMembers.Count; i++) {
						LeadershipLinks [i] = new float[TotalGroupMembers];				
						for (int j = 0; j < GroupMembers.Count; j++) {
								if (GroupMembers [i].Leadership > GroupMembers [j].Leadership) { 
										LeadershipLinks [i] [j] = (GroupMembers [j].Leadership / 100.0f) * RelationshipStrengths [j] [i] * RelationshipStrengths [j] [i]; // i == j gives bonus equal to i's Leadership score
										leadershipPoints [i] += (GroupMembers [j].Leadership / 100.0f) * RelationshipStrengths [j] [i] * RelationshipStrengths [j] [i];
								}
						}
				}
				
		
		}
}
