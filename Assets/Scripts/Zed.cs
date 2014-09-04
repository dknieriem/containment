using UnityEngine;
using System.Collections;

public class Zed : MonoBehaviour
{

		public int HitPoints = 100;

		public float MaxVelocity = 1.0f;

		public float Inertia = 8.0f; //dimensionless. 0 = a leaf on the wind. 1 = a kitten. 10 = a person. 

		private FSMSystem fsm;
		
		public Vector3 InterestLocation;
		
		public float InterestMagnitude;
		
		public float CountdownToForgettingInterest;
		
		
		
		public Game Game;
		
		public TerrainInfo Terrain;
		
		LineRenderer line;
		
		public float CountdownToNextSound;
		
		private int stepsToNextLogicUpdate = 10;
		
		private float countdownToInflictDamage = 0.0f;
		
		public void SetTransition (Transition t)
		{
				fsm.PerformTransition (t);
		}
	
		public void Start ()
		{
		
				//DEBUG: Debug.Log ("Zed start");
				
				Game = GameObject.Find ("Game").GetComponent<Game> ();
					
				Terrain = Game.GetComponentInChildren<TerrainInfo> ();//Ter.GetComponent<TerrainInfo> (); //GameObject.Find ("Terrain")
		
				line = gameObject.GetComponent<LineRenderer> ();
		
				InterestLocation = new Vector3 (-1, -1, 0);
				
				//DEBUG: Debug.Log ("Old Pos: " + transform.position);
				
				transform.position = new Vector3 (Random.Range (0, Terrain.Dimensions [0]), Random.Range (0, Terrain.Dimensions [1]), 0);
				
				//DEBUG: Debug.Log ("New Pos: " + transform.position);
				
				CountdownToNextSound = 10.0f;
		
				MakeFSM ();	
		
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		void FixedUpdate ()
		{
		
				UpdateSound ();
				
				stepsToNextLogicUpdate--;
		
				if (stepsToNextLogicUpdate == 0) {
		
						fsm.CurrentState.Reason (Game.gameObject, gameObject);
						stepsToNextLogicUpdate = 10;
				
				}
				
				fsm.CurrentState.Act (Game.gameObject, gameObject);
				
				line.SetPosition (0, gameObject.transform.position);
				
				if (InterestLocation.x != -1 && InterestLocation.y != -1)
						line.SetPosition (1, InterestLocation);
				else
						line.SetPosition (1, gameObject.transform.position);
		
		}
		
		public void UpdateSound ()
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
								newSound.Radius = 2.0f;
				
								CountdownToNextSound = Random.Range (25, 60);
				
								break;
						case StateID.RandomWalkStateID:
				
								newSound.Amplitude = 1.0f;
								newSound.Duration = 2.0f;
								newSound.Radius = 2.0f;
				
								CountdownToNextSound = Random.Range (10, 15);
				
								break;
						case StateID.MoveToInterestStateID:
			
								newSound.Amplitude = 1.0f;
								newSound.Duration = 1.0f;
								newSound.Radius = 2.0f;
				
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
		
		private void MakeFSM ()
		{
		
				IdleState idle = new IdleState ();
				idle.AddTransition (Transition.IdleToWalkTransition, StateID.RandomWalkStateID);
				idle.AddTransition (Transition.GetInterestTransition, StateID.MoveToInterestStateID);
		
		
				RandomWalkState walk = new RandomWalkState ();
				walk.AddTransition (Transition.WalkToIdleTransition, StateID.IdleStateID);
				walk.AddTransition (Transition.GetInterestTransition, StateID.MoveToInterestStateID);
		
				MoveToInterestState move = new MoveToInterestState ();
				move.AddTransition (Transition.LoseInterestTransition, StateID.IdleStateID);
				move.AddTransition (Transition.InterestInRangeTransition, StateID.AttackInterestStateID);
		
		
				fsm = new FSMSystem ();
				fsm.AddState (idle);
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
								float damage = velocity * Inertia;
								otherWall.SendMessage ("Damage", damage);
						}
				}
		}
}
