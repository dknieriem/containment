using UnityEngine;
using System.Collections;

public class Person
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
		
		public int LifetimeZedKills;			
		
		public static Person CreateRandomCharacter ()
		{
				Person x = new Person ();
				x.FirstName = "Steve";
				x.LastName = "Johnson";
				x.CurrentState = Person.CharacterState.Idle;
				x.BaseAttackStrength = Random.Range (10.0f, 20.0f);
				x.BaseHealth = Random.Range (80.0f, 100.0f);
				x.BaseStamina = Random.Range (70.0f, 90.0f);
		
				return x;
		}	

}
