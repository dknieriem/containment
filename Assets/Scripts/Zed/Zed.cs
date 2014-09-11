using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Zed : MonoBehaviour
{
		//Static Attributes
		public static float WalkVelocity = 0.6f; //units per second
		public static float MaxVelocity = 1.0f; //units per second
		public static float FieldOfView = 120.0f; //degrees
		public static float VisualRange = 10.0f; //units distance
		public static float ConfrontMaximumDistance = 10.0f; //units distance
		public static float SoundMagThreshholdForPassiveState = 2.0f;
		public static float HerdConfrontMagThreshhold = 10.0f;
		
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
		public float CountdownToNextSound = 10.0f;
		public string CurrentState;
		public string PreviousState;

		public string GUID;
		
		//Private Attributes
		private FSMSystem fsm;
		//private int stepsToNextLogicUpdate = 4; //10
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
				//UpdateSoundIMake ();
				
				//stepsToNextLogicUpdate--;
				//if (stepsToNextLogicUpdate == 0) {
				fsm.CurrentState.Reason (Game);
				//		stepsToNextLogicUpdate = 4; //10
				//}
				
				fsm.CurrentState.Act (Game);
				line.SetPosition (0, gameObject.transform.position);
				
				if (InterestLocation.x != 0 && InterestLocation.y != 0) {
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
				} 
				//else count <= 0
				return false;	
		}
		
		public int UpdateZedsICanSee ()
		{
		
				//Debug.Log ("Game Zeds: " + Game.GameZeds.Count);
				//Debug.Log ("angle: " + transform.rotation.z);
				//float myAngle = transform.eulerAngles.z;
				//float x = Mathf.Cos (myAngle);
				//float y = Mathf.Sin (myAngle);
				//Debug.Log ("vector: (" + x + ", " + y + ")");
				//Debug.Log ("Dir Vector: " + DirVector);
				
				IEnumerable<Zed> ZedsInRange = Game.GameZeds.Where (z => Vector3.Distance (z.transform.position, gameObject.transform.position) <= Zed.VisualRange).Except (gameObject.GetComponents<Zed> ());		
				
				/*if (ZedsInRange.Count () > 0) {
						Debug.Log ("Zed " + GUID + " has " + ZedsInRange.Count () + " Zeds in range");
						Debug.Log ("Zed rotation: " + transform.eulerAngles.z);
						float x = Mathf.Cos (transform.eulerAngles.z);
						float y = Mathf.Sin (transform.eulerAngles.z);
						Debug.Log ("Dir Vector is (" + x + ", " + y + ")");
				
						foreach (Zed z in ZedsInRange) {
								Vector3 dirVector = z.transform.position - transform.position;
								float angle = Vector3.Angle (dirVector, new Vector3 (x, y, 0));
								Debug.Log ("Zed " + z.GUID + " dirVector: " + dirVector);
								Debug.Log ("Angle calc: " + angle);
						
						}
				}*/
				IEnumerable<Zed> VisibleZeds = ZedsInRange.Where (z => IsVisible (z.gameObject));
				
				ZedsICanSee = VisibleZeds.Where (z => z.Velocity > 0)
				.ToArray ();
				//Debug.Log ("Visible Zeds: " + ZedsICanSee.Count ());
				return ZedsICanSee.Count ();
		}
		
		public bool IsVisible (GameObject other)
		{
				float myAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
				float x = Mathf.Cos (myAngle);
				float y = Mathf.Sin (myAngle);
		
				Vector3 vectorToOther = other.transform.position - transform.position;
				Vector3 myDirVector = new Vector3 (x, y, 0);
				float rawAngle = Vector3.Angle (myDirVector, vectorToOther);
		
				//Debug.Log ("Angle BW " + GUID + " + " + other.GetComponent<Zed> ().GUID + ": " + rawAngle);
				if (rawAngle < Zed.FieldOfView / 2.0f) {
						return true;
				}
		
				return false;
		}
		
		public bool MoveTowardsSoundsICanHear ()
		{
				// If any "sound" is within hearing range
				int soundCount = UpdateSoundsICanHear ();	
		
				if (soundCount > 0) {
			
						//find largest magnitude
						Sound target = SoundsICanHear.First ();
			
						//Debug.Log (GUID + " loc: " + transform.position + ", target: " + target.transform.position + ", dist: " + Vector3.Distance (target.transform.position, transform.position));
			                                                                                                                       
						// set npc target location to sound location
						InterestLocation = target.transform.position;
			
						DirVector = InterestLocation - transform.position;
						DirVector.Normalize ();
						InterestMagnitude = target.Magnitude (transform.position);
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
				.Where (s => s.Magnitude (transform.position) > 0.1f)
				.OrderByDescending (s => s.Magnitude (transform.position))
				.ToArray ();
	
				return SoundsICanHear.Count ();
		}
		
		public void UpdateSoundIMake ()
		{
				if (fsm.CurrentStateID == StateID.PassiveStateID) {
						return;
				}
				
				CountdownToNextSound -= Time.deltaTime;
				if (CountdownToNextSound <= 0) { //play another sound
						if (gameObject.GetComponent<Sound> ()) { //no sound playing currently, not sure why this would still be here!
								Object.Destroy (gameObject.GetComponent<Sound> ());
						}
			
						Sound newSound = gameObject.AddComponent<Sound> ();
			
						switch (fsm.CurrentStateID) {
						
						case StateID.PassiveStateID:
								break;
						
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
								
						case StateID.HerdStateID:
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
				
						case StateID.ConfrontStateID:
								newSound.Amplitude = 1.5f;
								newSound.Duration = 1.0f;
								newSound.Radius = 2.0f;
								CountdownToNextSound = Random.Range (3, 10);
								break;
				
						case StateID.AttackInterestStateID:
				
								break;
						} //end switch(currentstateID)
			
						int whichSound = Random.Range (0, Game.ZedSounds.Length - 1);
			
						gameObject.GetComponent<AudioSource> ().clip = Game.ZedSounds [whichSound];
						gameObject.GetComponent<AudioSource> ().Play ();	
				}//end if(countdown < 0)
		}
				
		private void MakeFSM ()
		{
		
				PassiveState passive = new PassiveState (this);
				passive.AddTransition (Transition.PassiveToIdleTransition, StateID.IdleStateID);
		
				IdleState idle = new IdleState (this);
				idle.AddTransition (Transition.IdleToPassiveTransition, StateID.PassiveStateID);
				idle.AddTransition (Transition.IdleToRoamTransition, StateID.RoamStateID);
				idle.AddTransition (Transition.HerdingTransition, StateID.HerdStateID);
				idle.AddTransition (Transition.InvestigateTransition, StateID.InvestigateStateID);
				idle.AddTransition (Transition.ConfrontTransition, StateID.ConfrontStateID);
				
				RoamState roam = new RoamState (this);
				roam.AddTransition (Transition.RoamToIdleTransition, StateID.IdleStateID);
				roam.AddTransition (Transition.HerdingTransition, StateID.HerdStateID);
				roam.AddTransition (Transition.InvestigateTransition, StateID.InvestigateStateID);
				roam.AddTransition (Transition.ConfrontTransition, StateID.ConfrontStateID);
		
				HerdState herd = new HerdState (this);
				herd.AddTransition (Transition.HerdingEndedTransition, StateID.IdleStateID);
				herd.AddTransition (Transition.ConfrontTransition, StateID.ConfrontStateID);
		
				InvestigateState investigate = new InvestigateState (this);
				investigate.AddTransition (Transition.InvestigationEndedTransition, StateID.IdleStateID);
				investigate.AddTransition (Transition.ConfrontTransition, StateID.ConfrontStateID);
		
				ConfrontState confront = new ConfrontState (this);
				confront.AddTransition (Transition.ConfrontationEndedTransition, StateID.IdleStateID);
	
				fsm = new FSMSystem ();
				fsm.AddState (passive);
				fsm.AddState (idle);
				fsm.AddState (roam);
				fsm.AddState (herd);
				fsm.AddState (investigate);
				fsm.AddState (confront);
		}	
		void OnCollisionStay2D (Collision2D collision)
		{
				countdownToInflictDamage -= Time.deltaTime;
				if (countdownToInflictDamage <= 0) {
						countdownToInflictDamage = 1.0f;
						GameObject other = collision.gameObject;
						WallSegment otherWall = other.GetComponent<WallSegment> ();
						if (otherWall != null) {
								float velocity = collision.relativeVelocity.magnitude;
								float damage = velocity * Mass;
								otherWall.Damage (damage);
						}
				}
		}
}
