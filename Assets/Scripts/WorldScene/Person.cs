using UnityEngine;
using System.Collections;
using System;

public partial class Person
{

		public string FirstName;
		public string LastName;
		public int LocationX; 
		public int LocationY;
		public Sector CurrentSector;
		public CharacterState CurrentState;
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
				x.CurrentStats [(int)Stats.KillRate] = 0;
				
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
		
		void UpdateAttributes ()
		{
		
		}
		
		void UpdateSkills ()
		{
		
		}
		
		void UpdateStats ()
		{
				BaseStats [(int)Stats.InjuryRate] = CurrentSector.ZedCount;
				
				switch (CurrentRole) {
				case Role.None:
						UpdateStatsNone ();
						break;
				case Role.Patient:
						UpdateStatsPatient ();
						break;
				case Role.Doctor:
						UpdateStatsDoctor ();
						break;
				case Role.Builder:
						UpdateStatBuilder ();
						break;
				case Role.Guard:
						UpdateStatsGuard ();
						break;
				case Role.Scout:
						UpdateStatsScout ();
						break;
				case Role.Looter:
						UpdateStatsLooter ();
						break;				
				}
				
				CurrentStats [(int)Stats.InjuryRate] = 0; //reset Injury Rate 
				for (int k = 0; k < CurrentStats.Length; k++) {
						if (CurrentStats [k] < 0) {
								CurrentStats [k] = 0;
						}
				}
		}
		
		void UpdateStatsNone ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 		
				CurrentStats [(int)Stats.Health] -= 0; //TODO: adjust based on current location in sector (safe or not?)
				CurrentStats [(int)Stats.Stamina] += 0; //TODO: adjust based on food level + rationing
				CurrentStats [(int)Stats.KillRate] = 0;
		}
		
		void UpdateStatsPatient ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				//if a patient, injury rate will be modified by the doctor's skill 
				CurrentStats [(int)Stats.InjuryRate] -= MyGroup.GetDoctorSkillInSector (CurrentSector);
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] -= CurrentStats [(int)Stats.KillRate];
				CurrentStats [(int)Stats.KillRate] = 0;
		}
		
		void UpdateStatsDoctor ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] -= CurrentStats [(int)Stats.KillRate];
				CurrentStats [(int)Stats.KillRate] = 0;
		}
		
		void UpdateStatBuilder ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				CurrentStats [(int)Stats.BuildRate] = BaseStats [(int)Stats.BuildRate] * (CurrentStats [(int)Stats.Stamina] / BaseStats [(int)Stats.Stamina]);
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] -= CurrentStats [(int)Stats.KillRate];
				CurrentStats [(int)Stats.KillRate] = 0;
		}
		
		void UpdateStatsGuard ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] -= CurrentStats [(int)Stats.KillRate];
				CurrentStats [(int)Stats.KillRate] = BaseStats [(int)Stats.KillRate] * (CurrentStats [(int)Stats.Stamina] / BaseStats [(int)Stats.Stamina]);
		}
		
		void UpdateStatsScout ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] -= CurrentStats [(int)Stats.KillRate];
				CurrentStats [(int)Stats.KillRate] = BaseStats [(int)Stats.KillRate] * (CurrentStats [(int)Stats.Stamina] / BaseStats [(int)Stats.Stamina]);
		}
		
		void UpdateStatsLooter ()
		{
				CurrentStats [(int)Stats.InjuryRate] = BaseStats [(int)Stats.InjuryRate] / (Skills [(int)Skill.MeleeStrength] + Skills [(int)Skill.FirearmStrength]); 
				CurrentStats [(int)Stats.Health] -= CurrentStats [(int)Stats.InjuryRate];
				CurrentStats [(int)Stats.Stamina] -= CurrentStats [(int)Stats.KillRate];
				CurrentStats [(int)Stats.KillRate] = BaseStats [(int)Stats.KillRate] * (CurrentStats [(int)Stats.Stamina] / BaseStats [(int)Stats.Stamina]);
		}
		
		public void AddZedKills (int numZedsKilled)
		{
				LifetimeZedKills += numZedsKilled;
				//TODO: add experience for each kill,
								
				Person[] sectorMates = MyGroup.GetMembersInSector (LocationX, LocationY);
			
				for (int j = 0; j < sectorMates.Length; j++) {
						if (sectorMates [j].CurrentRole == Role.Guard || sectorMates [j].CurrentRole == Role.Scout || sectorMates [j].CurrentRole == Role.Looter) { //we have a match!
								AdjustRelationship (sectorMates [j], 0.5f);
						}
				}
		}
		
		public void AdjustRelationship (Person otherPerson, float adjustment)
		{
				//otherPerson.AdjustRelationship(this, adjustment); From addzedkills, this will be done in other person's update.
				for (int i = 0; i < Relationships.Length; i++) {
						if (Relationships [i] == otherPerson) { //we have a match!
								RelationshipStrengths [i] += adjustment;
						}
				}
		}
}
