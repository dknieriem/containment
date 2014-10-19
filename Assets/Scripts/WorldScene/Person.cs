using UnityEngine;
using System.Collections;
using System;

public partial class Person
{

		public string FirstName;
		public string LastName;
		public 	int LocationX; 
		public 	int LocationY;
		public Sector CurrentSector;
		public 	CharacterState CurrentState;
		public Role CurrentRole;
		
		public PlayerGroup MyGroup;
		public Person[] Relationships;
		public float[] RelationshipStrengths;
		
		public float[] BaseStats;
		public float[] CurrentStats;
		public float[] Skills;
		public int LifetimeZedKills;			
		
		//TODO: Queue / BehaviorTree of actions to perform
		
		public Person ()
		{
				FirstName = "Blankity";
				LastName = "Blank";
				CurrentState = Person.CharacterState.Idle;
				CurrentRole = Person.Role.None;
				BaseStats = new float[Enum.GetNames (typeof(Person.Stats)).Length];
				CurrentStats = new float[Enum.GetNames (typeof(Person.Stats)).Length];
				Skills = new float[Enum.GetNames (typeof(Person.Skill)).Length];
		}
		
		public static Person CreateRandomCharacter (PlayerGroup myGroup)
		{
				Person x = new Person ();
				x.MyGroup = myGroup;
				x.CurrentSector = myGroup.HomeSector;
				x.LocationX = x.CurrentSector.LocationX;
				x.LocationY = x.CurrentSector.LocationY;
				x.FirstName = maleFirstNames [UnityEngine.Random.Range (1, maleFirstNames.Length) - 1];
				x.LastName = lastNames [UnityEngine.Random.Range (1, lastNames.Length) - 1];
				
				x.BaseStats [(int)Stats.Health] = UnityEngine.Random.Range (8000, 10000) / 100.0f;
				x.BaseStats [(int)Stats.Stamina] = UnityEngine.Random.Range (7000, 9000) / 100.0f;
				x.BaseStats [(int)Stats.KillRate] = UnityEngine.Random.Range (1000, 2000) / 100.0f;
		
				x.CurrentStats [(int)Stats.Health] = x.BaseStats [(int)Stats.Health];
				x.CurrentStats [(int)Stats.Stamina] = x.BaseStats [(int)Stats.Stamina];
				x.CurrentStats [(int)Stats.KillRate] = x.BaseStats [(int)Stats.KillRate];
				
				x.Skills [(int)Skill.Leadership] = UnityEngine.Random.Range (0.0f, 60.0f);
				if (x.Skills [(int)Skill.Leadership] < 30.0f) {
						x.Skills [(int)Skill.Leadership] = 0.0f;
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
		
				UpdateAttributes ();
				UpdateSkills ();
				UpdateStats ();
		
		}
		
		public void UpdateAttributes ()
		{
		}
		
		public void UpdateSkills ()
		{
		
		}
		
		public void UpdateStats ()
		{
		
				BaseStats [(int)Stats.InjuryRate] = CurrentSector.ZedCount;
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				//if a patient, injury rate will be modified by the doctor's skill 
				CurrentStats [(int)Stats.InjuryRate] -= MyGroup.GetDoctorSkillInSector (CurrentSector);
				
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] = BaseStats [(int)Stats.Stamina] * (CurrentStats [(int)Stats.Health] / BaseStats [(int)Stats.Health]);
				//OR stamina -= killrate
				CurrentStats [(int)Stats.KillRate] = BaseStats [(int)Stats.KillRate] * (CurrentStats [(int)Stats.Stamina] / BaseStats [(int)Stats.Stamina]);
					
				CurrentStats [(int)Stats.InjuryRate] = 0; //reset Injury Rate 
		
		}
}
