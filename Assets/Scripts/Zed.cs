using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Zed : MonoBehaviour
{
		//Static Attributes
		public static float WalkVelocity = 0.6f; //units per second
		public static float MaxVelocity = 1.0f; //units per second
		public static float FieldOfView = 60.0f; //degrees
		public static float VisualRange = 10.0f; //units distance
		public static float ConfrontMaximumDistance = 10.0f; //units distance
		public static float SoundMagThreshholdForPassiveState = 2.0f;
		
		//References to other GameObjects
		public Zed[] ZedsICanSee;
		public Sound[] SoundsICanHear;
		public Game Game;
		public TerrainInfo Terrain;
	
		//Public Attributes
		public int HitPoints = 100;
		public float Mass = 22.0f; //kg
		public Vector3 InterestLocation = new Vector3 (-1, -1, 0);
		public Vector3 DirVector;
		public float Velocity;
		public float InterestMagnitude;
		public float CountdownToRandomChange;
		public float CountdownToNextSound = 10.0f;
		public string CurrentState;
		public string GUID;
		
		//Private Attributes
		private FSMSystem fsm;
		private int stepsToNextLogicUpdate = 4; //10
		private float countdownToInflictDamage = 0.0f;
		
		//Sibling GameObjects	
		LineRenderer line;
			
		public void SetTransition (Transition t)
		{
				fsm.PerformTransition (t);
		}
	
		public void Start ()
		{
				//DEBUG: Debug.Log ("Zed start");
				
				Game = GameObject.Find ("Game").GetComponent<Game> ();
				Terrain = Game.GetComponentInChildren<TerrainInfo> ();
				line = gameObject.GetComponent<LineRenderer> ();
				//InterestLocation = new Vector3 (-1, -1, 0);
				transform.position = new Vector3 (Random.Range (0, Terrain.Dimensions [0]), Random.Range (0, Terrain.Dimensions [1]), 0);
				GUID = System.Guid.NewGuid ().ToString ();
				MakeFSM ();	
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		void FixedUpdate ()
		{
				UpdateSoundIMake ();
				
				stepsToNextLogicUpdate--;
				if (stepsToNextLogicUpdate == 0) {
						fsm.CurrentState.Reason (Game.gameObject, gameObject);
						UpdateStateString ();
						stepsToNextLogicUpdate = 4; //10
				}
				
				fsm.CurrentState.Act (Game.gameObject, gameObject);
				line.SetPosition (0, gameObject.transform.position);
				
				if (DirVector.x != -1 && DirVector.y != -1) {
						line.SetPosition (1, gameObject.transform.position + DirVector);
				} else {
						line.SetPosition (1, gameObject.transform.position);
				}
		}
		
		public bool MoveTowardsOtherZeds ()
		{
				int count = UpdateZedsICanSee ();
				if (count > 0) {
						InterestLocation = Vector3.zero;
			
						foreach (Zed z in ZedsICanSee) {
								InterestLocation += z.transform.position;
						}
			
						InterestLocation /= count;
			
						DirVector = InterestLocation - transform.position;
						DirVector.Normalize ();
						InterestMagnitude = 0.1f * count;

						return true;
				} else { //count <= 0
						return false;
				}
		}
		
		public int UpdateZedsICanSee ()
		{
				IEnumerable<Zed> ZedsInRange = Game.GameZeds.Where (z => Vector3.Distance (z.transform.position, gameObject.transform.position) <= Zed.VisualRange);		
		
				IEnumerable<Zed> VisibleZeds = ZedsInRange.Where (z => Mathf.Abs 
				(Vector3.Angle (z.gameObject.transform.position - transform.position, 
				new Vector3 (Mathf.Cos (transform.eulerAngles.z), 
				Mathf.Sin (transform.eulerAngles.z), 0))) < Zed.FieldOfView / 2.0f);
				
				ZedsICanSee = VisibleZeds.Where (z => z.Velocity > 0)
				.ToArray ();
				
				return ZedsICanSee.Count ();
		}
		
		public bool MoveTowardsSoundsICanHear ()
		{
				// If any "sound" is within hearing range
				int soundCount = UpdateSoundsICanHear ();	
		
				if (soundCount > 0) {
			
						//find largest magnitude
						Sound target = SoundsICanHear.First ();
			
						// set npc target location to sound location
						InterestLocation = target.transform.position;
			
						DirVector = InterestLocation - transform.position;
						DirVector.Normalize ();
						InterestMagnitude = target.Magnitude (Vector3.Distance (target.transform.position, transform.position));
						return true;	
				} else {
						return false;
				}
		}
		
		public int UpdateSoundsICanHear ()
		{
				IEnumerable<Sound> sounds = Game.GetComponentsInChildren<Sound> ();
				IEnumerable<Sound> mySounds = gameObject.GetComponents<Sound> ();	
				IEnumerable<Sound> targets = sounds.Where (s => Vector3.Distance (s.transform.position, transform.position) < s.Radius).Except (mySounds);
		
				//2. keep only sounds with magnitude > threshhold T
				SoundsICanHear = targets
				.Where (s => s.Magnitude (Vector3.Distance (s.transform.position, transform.position)) > 0.1f)
				.OrderByDescending (s => s.Magnitude (Vector3.Distance (s.transform.position, transform.position)))
				.ToArray ();
	
				return SoundsICanHear.Count ();
		}
		
		public void UpdateSoundIMake ()
		{
				CountdownToNextSound -= Time.deltaTime;
				if (CountdownToNextSound <= 0) { //play another sound
						if (gameObject.GetComponent<Sound> ()) { //no sound playing currently, not sure why this would still be here!
								Object.Destroy (gameObject.GetComponent<Sound> ());
						}
			
						Sound newSound = gameObject.AddComponent<Sound> ();
			
						switch (fsm.CurrentStateID) {
						
						case StateID.IdleStateID:
								newSound.Amplitude = 0.5f;
								newSound.Duration = 1.0f;
								newSound.Radius = 1.0f;
								CountdownToNextSound = Random.Range (25, 60);
								break;
								
						case StateID.RoamStateID:
								newSound.Amplitude = 1.0f;
								newSound.Duration = 1.0f;
								newSound.Radius = 1.0f;
								CountdownToNextSound = Random.Range (10, 15);
								break;
								
						case StateID.InvestigateStateID:
								newSound.Amplitude = 1.0f;
								newSound.Duration = 1.0f;
								newSound.Radius = 1.5f;
								CountdownToNextSound = Random.Range (5, 10);
								break;
								
						case StateID.AttackInterestStateID:
				
								break;
						} //end switch(currentstateID)
			
						int whichSound = Random.Range (0, Game.ZedSounds.Length - 1);
			
						gameObject.GetComponent<AudioSource> ().clip = Game.ZedSounds [whichSound];
						gameObject.GetComponent<AudioSource> ().Play ();	
				}//end if(countdown < 0)
		}
		
		public void UpdateStateString ()
		{
				switch (fsm.CurrentStateID) {
				case StateID.IdleStateID:
						CurrentState = "IdleState";				
						break;
						
				case StateID.PassiveStateID:
						CurrentState = "PassiveState";
						break;
						
				case StateID.RoamStateID:
						CurrentState = "RandomWalkState";	
						break;
										
				case StateID.InvestigateStateID:
						CurrentState = "InvestigateState";	
						break;
						
				case StateID.AttackInterestStateID:
						CurrentState = "AttackInterestState";	
						break;
				} //end switch(currentstateID)
		}
		
		private void MakeFSM ()
		{
				IdleState idle = new IdleState ();
				idle.AddTransition (Transition.IdleToPassiveTransition, StateID.PassiveStateID);
				idle.AddTransition (Transition.IdleToRoamTransition, StateID.RoamStateID);
				idle.AddTransition (Transition.HerdingTransition, StateID.HerdStateID);
				idle.AddTransition (Transition.InvestigateTransition, StateID.InvestigateStateID);
				idle.AddTransition (Transition.ConfrontTransition, StateID.ConfrontStateID);
				
				PassiveState passive = new PassiveState ();
				passive.AddTransition (Transition.PassiveToIdleTransition, StateID.IdleStateID);
				
				RandomWalkState walk = new RandomWalkState ();
				walk.AddTransition (Transition.RoamToIdleTransition, StateID.IdleStateID);
				walk.AddTransition (Transition.InvestigateTransition, StateID.InvestigateStateID);
		
				InvestigateState move = new InvestigateState ();
				move.AddTransition (Transition.InvestigationEndedTransition, StateID.IdleStateID);
	
				fsm = new FSMSystem ();
				fsm.AddState (idle);
				fsm.AddState (passive);
				fsm.AddState (walk);
				fsm.AddState (move);
		}	
		void OnCollisionStay2D (Collision2D collision)
		{
				countdownToInflictDamage -= Time.deltaTime;
				if (countdownToInflictDamage <= 0) {
						countdownToInflictDamage = 1.0f;
						GameObject other = collision.gameObject;
						Wall otherWall = other.GetComponent<Wall> ();
						if (otherWall != null) {
								float velocity = collision.relativeVelocity.magnitude;
								float damage = velocity * Mass;
								otherWall.Damage (damage);
						}
				}
		}
}
