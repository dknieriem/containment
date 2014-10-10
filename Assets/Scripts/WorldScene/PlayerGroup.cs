using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroup : MonoBehaviour
{

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
				SetHomeSector (homeSector);
				homeSector.PlayerGroupCount = numMembers;
				SectorGroupMembers [(int)HomeSectorLocation [0], (int)HomeSectorLocation [1]] = numMembers;
				TotalGroupMembers = numMembers;
				
				for (int i = 0; i < numMembers; i++) {
						Person newPerson = Person.CreateRandomCharacter ();
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
						Person newPerson = Person.CreateRandomCharacter ();
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
