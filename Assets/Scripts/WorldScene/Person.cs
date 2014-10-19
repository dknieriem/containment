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

		public string FirstName;
		public string LastName;
		public 	int LocationX; 
		public 	int LocationY;
		public Sector CurrentSector;
		public 	CharacterState CurrentState;
		
		
		public PlayerGroup MyGroup;
		public Person[] Relationships;
		public float[] RelationshipStrengths;
		
		public float[] BaseStats;
		public float[] CurrentStats;
		public float[] Skills;
		public int LifetimeZedKills;			
		
		//TODO: Queue / BehaviorTree of actions to perform
		
		public Person()
		{
		x.MyGroup = myGroup;
				x.FirstName = "Blankity";
				x.LastName = "Blank";
				x.CurrentState = Person.CharacterState.Idle;
				
				x.BaseStats = new float[Player.Stats.Length];
				x.CurrentStats = new float[Player.Stats.Length];
				x.Skills = new float[Player.Skills.Length];
		}
		
		public static Person CreateRandomCharacter (PlayerGroup myGroup)
		{
				Person x = new Person ();
				x.MyGroup = myGroup;
				x.mySector = myGroup.HomeSector;
				x.LocationX = x.mySector.LocationX;
				x.LocationY = x.mySector.LocationY;
				x.FirstName = maleFirstNames [Random.Range (1, maleFirstNames.Length) - 1];
				x.LastName = lastNames [Random.Range (1, lastNames.Length) - 1];
				
				x.BaseStats[Stats.Health] = Random.Range (8000, 10000) / 100.0f;
				x.BaseStats[Stats.Stamina] = Random.Range (7000, 9000) / 100.0f;
				x.BaseStats[Stats.KillRate] = Random.Range (1000, 2000) / 100.0f;
		
				x.CurrentStats[Stats.Health] = x.BaseStats[Stats.Health];
				x.CurrentStats[Stats.Stamina]= x.BaseStats[Stats.Stamina];
				x.CurrentStats[Stats.KillRate] = x.BaseStats[Stats.KillRate];
				
				x.Skills[Skills.Leadership] = Random.Range (0.0f, 60.0f);
				if (x.Skills[Skills.Leadership] < 30.0f) {
						x.Skills[Skills.Leadership] = 0.0f;
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
		
		UpdateAttributes();
		UpdateSkills();
		UpdateStats();
		
		}
		
		public void UpdateAttributes()
		{
				}
		
		public void UpdateSkills()
		{
		
		}
		
		public void UpdateStats()
		{
		
				BaseStats[Stats.InjuryRate] = mySector.ZedCount;
				CurrentStats[Stats.InjuryRate] = BaseStats[Stats.InjuryRate] / (Skills[Skill.MeleeStrength] + Skills[Skill.FirearmStrength] ); 
				//if a patient, injury rate will be modified by the doctor's skill 
				CurrentStats[Stats.InjuryRate] -= myGroup.GetDoctorSkillInSector(mySector);
				
				CurrentStats[Stats.Health] -= CurrentStats[Stats.InjuryRate];
				CurrentStats[Stats.Stamina] = BaseStats[Stats.Stamina] * (CurrentHealth / BaseHealth);
				//OR stamina -= killrate
				CurrentAttackStrength = BaseAttackStrength * (CurrentStamina / BaseStamina);
				Debug.Log (string.Format ("{0} {1} stats H,S,A: {2} {3} {4}", FirstName, LastName, CurrentHealth, CurrentStamina, CurrentAttackStrength));
				
		
		
				CurrentStats[Stats.InjuryRate] = 0; //reset Injury Rate 
		
		}
}
