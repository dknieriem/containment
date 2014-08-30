using UnityEngine;
using System.Collections;

public class Zed : MonoBehaviour
{

		public int HitPoints = 100;

		public float maxVelocity = 1.0f;

		private FSMSystem fsm;
		
		public Vector3 InterestLocation;
		
		public float countdownToForgettingInterest;
		
		public Game game;
		
		public void SetTransition (Transition t)
		{
				fsm.PerformTransition (t);
		}
	
		public void Start ()
		{
		
				Debug.Log ("Zed start");
		
				InterestLocation = new Vector3 (-1, -1, 0);
		
				game = GameObject.Find ("Game").GetComponent<Game> ();
		
				MakeFSM ();	
		
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
		
				fsm.CurrentState.Reason (game.gameObject, gameObject);
				fsm.CurrentState.Act (game.gameObject, gameObject);
		
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
	
}
