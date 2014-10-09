using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroup : MonoBehaviour
{

		public enum CharacterState
		{
				Dead = 0,
				Resting, //required sleep
				Recovering, //healing from wounds
				Idle, //not assigned a job
				Repairing, //repairing or building defenses
				Defending, //defending a safe house
				Hunting, //actively hunting zed in the sector
				Scouting, //scouting buildings in the sector, discovering safety values and lootable contents
				Looting }//looting buildings in the sector	
		;

		public enum Skill
		{
				MeleeAttack = 0,
				RangedAttack,
				Sneak,
				Sprint,
				Strength,
				Stamina }
		;

		public class Person
		{
				public string FirstName;
				public string LastName;
				public 	int LocationX; 
				public 	int LocationY;
				public 	CharacterState CurrentState;
				public 	float BaseAttackStrength; //average number of Zeds per hour killed, modified by melee weapon strength and stamina.
				public 	float CurrentAttackStrength; //modified attack strength
				public 	float BaseStamina; // maximum stamina fully-rested
				public 	float CurrentStamina; //current stamina. when < 10%, character should seek rest. at 0, will pass out.
				public 	float BaseHealth; //maximum health
				public 	float CurrentHealth; //current health
		}

		public static int MaxSquadsAllowed = 10;
		public static int MaxMembersAllowed = 100;
		public static int MaxMembersPerSquadAllowed = 16;

		public Sector HomeSector;
		public int[] HomeSectorLocation;
		
		public List<Person> GroupMembers;
		public int TotalGroupMembers;
		//public ArrayList GroupMemberLocations = new ArrayList ();
		public int[,] SectorGroupMembers;
		
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
				GroupMembers = new List<Person> (5);
				StartGroup (Random.Range (5, 10), Random.Range (0, 10), Random.Range (0, 10));
		}
	
		Person CreateRandomCharacter ()
		{
				Person x = new Person ();
				x.FirstName = "Steve";
				x.LastName = "Johnson";
				x.CurrentState = CharacterState.Idle;
				x.BaseAttackStrength = 15.0f;
				x.BaseHealth = 100.0f;
				x.BaseStamina = 100.0f;
				
				return x;
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
				UpdatePersonAttributes ();
				DoAttacks ();
		}
				
		void UpdatePersonAttributes ()
		{
				for (int i = 0; i < GroupMembers.Count; i++) {
						Person p = GroupMembers [i];
						p.CurrentHealth = p.BaseHealth;
						p.CurrentStamina = p.BaseStamina * (p.CurrentHealth / p.BaseHealth);
						p.CurrentAttackStrength = p.BaseAttackStrength * (p.CurrentStamina / p.BaseStamina);
						Debug.Log (string.Format ("{0} {1} stats H,S,A: {2} {3} {4}", p.FirstName, p.LastName, p.CurrentHealth, p.CurrentStamina, p.CurrentAttackStrength));
				}
		}
				
		void DoAttacks ()
		{
		
				//for each group member in 
				for (int i = 0; i < GroupMembers.Count; i++) {
						Person p = GroupMembers [i];
						int numZedsKilled = (int)(p.CurrentAttackStrength * Random.Range (0.8f, 1.0f));
						Debug.Log ("NZK 1: " + numZedsKilled);
						if (numZedsKilled > World.WorldSectors [p.LocationX, p.LocationY].ZedCount) {
								numZedsKilled = World.WorldSectors [p.LocationX, p.LocationY].ZedCount;
						}
						//TODO: EventHandler.NewMessage(p.FirstName + " " + p.LastName + " killed " + numZedsKilled + " zeds", p.LocationX, p.LocationY, World.CurrentDate);
						Debug.Log (string.Format ("{0} {1} killed {2} zeds at sector {3}, {4} on {5}", p.FirstName, p.LastName, numZedsKilled, p.LocationX, p.LocationY, World.CurrentDate));
						World.WorldSectors [p.LocationX, p.LocationY].ZedCount -= numZedsKilled;
				}		
				
		}
				
		public void StartGroup (int numMembers, Sector homeSector)
		{
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [(int)HomeSectorLocation [0], (int)HomeSectorLocation [1]] = numMembers;
				TotalGroupMembers = numMembers;
				
				for (int i = 0; i < numMembers; i++) {
						Person newPerson = CreateRandomCharacter ();
						newPerson.LocationX = homeSector.LocationX;
						newPerson.LocationY = homeSector.LocationY;
						GroupMembers.Add (newPerson);
				}
		}
		
		public void StartGroup (int numMembers, int homeSectorX, int homeSectorY)
		{
		
				Debug.Log ("Adding " + numMembers + " to (" + homeSectorX + ", " + homeSectorY + ").");
				Sector homeSector = World.WorldSectors [homeSectorX, homeSectorY];
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [homeSectorX, homeSectorY] = numMembers;
				TotalGroupMembers = numMembers;
				
				for (int i = 0; i < numMembers; i++) {
						Person newPerson = CreateRandomCharacter ();
						newPerson.LocationX = homeSectorX;
						newPerson.LocationY = homeSectorY;
						GroupMembers.Add (newPerson);
				}
		}
		
		public void SetHomeSector (Sector newLocation)
		{
				HomeSector = newLocation;
				HomeSectorLocation [0] = HomeSector.LocationX;
				HomeSectorLocation [1] = HomeSector.LocationY;
		}
}
