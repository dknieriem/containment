using UnityEngine;
using System.Collections;

public partial class Person
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
	
		public enum CharacterLocation
		{
				Outside = 0,
				UnclearedBuilding,
				ClearedBuilding,
				Safehouse }
		;
	
		public enum CharacterMission
		{
				None = 0,
				HealSelf,
				Doctor,
				Repair,
				ClearSector,
				ScoutSector,
				LootSector }
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
		
		public float Leadership;
		
		public PlayerGroup MyGroup;
		public Person[] Relationships;
		public float[] RelationshipStrengths;
		
		public int LifetimeZedKills;			
		
		//TODO: Queue / BehaviorTree of actions to perform
		
		public static Person CreateRandomCharacter (PlayerGroup myGroup)
		{
				Person x = new Person ();
				x.MyGroup = myGroup;
				x.FirstName = maleFirstNames [Random.Range (1, maleFirstNames.Length) - 1];
				x.LastName = lastNames [Random.Range (1, lastNames.Length) - 1];
				x.CurrentState = Person.CharacterState.Idle;
				x.BaseAttackStrength = Random.Range (1000, 2000) / 100.0f;
				x.BaseHealth = Random.Range (8000, 10000) / 100.0f;
				x.BaseStamina = Random.Range (7000, 9000) / 100.0f;
		
				x.CurrentAttackStrength = x.BaseAttackStrength;
				x.CurrentHealth = x.BaseHealth;
				x.CurrentStamina = x.BaseStamina;
		
				x.Leadership = Random.Range (0.0f, 100.0f);
				if (x.Leadership < 30.0f) {
						x.Leadership = 0.0f;
				}
		
				return x;
		}	

		public void SetRelationships (Person[] people)
		{
				Relationships = new Person[people.Length];
				RelationshipStrengths = new float[people.Length];
		
				for (int i = 0; i < people.Length; i++) {
						Relationships [i] = people [i];
						RelationshipStrengths [i] = 0.0f;
				}
		
		}
		
		public void SetRelationships (Person[] people, float[] strengths)
		{
				Relationships = new Person[people.Length];
				RelationshipStrengths = new float[people.Length];
		
				for (int i = 0; i < people.Length; i++) {
						Relationships [i] = people [i];
						RelationshipStrengths [i] = strengths [i];
				}
		
		}
		public void DoNextUpdate ()
		{
		
		
		
		}
		
}
