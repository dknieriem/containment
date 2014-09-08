using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class IdleState : FSMState
{	
	
		public float GracePeriodCountdown;
		public float Deadtime;
	
		public IdleState (Zed zedRef)
		{ 
				stateID = StateID.IdleStateID;
				StateZedRef = zedRef;
		}
	
		public override void DoBeforeEntering ()
		{
				base.DoBeforeEntering ();
		
				StateZedRef.Velocity = 0;
				StateZedRef.InterestMagnitude = 0;
				//StateZedRef.DirVector.Normalize = new Vector3 (0, 0);
				StateZedRef.InterestLocation = new Vector3 (0, 0);
				
				//npc.rigidbody2D.velocity = Vector2.zero;
				//StateZedRef.CountdownToRandomChange = Random.Range (10, 25);
				GracePeriodCountdown = 10.0f;
				Deadtime = 60.0f;
		}
	
		public override void Reason (Game game)
		{
				// If any characters within instinct range
				//TODO: add detection of characters
					
				//Sound Detection
				if (StateZedRef.UpdateSoundsICanHear () > 0) {
						StateZedRef.MoveTowardsSoundsICanHear ();
						if (Vector3.Distance (StateZedRef.InterestLocation, StateZedRef.transform.position) > Zed.ConfrontMaximumDistance) {
								StateZedRef.SetTransition (Transition.InvestigateTransition);
						} else {
								StateZedRef.SetTransition (Transition.ConfrontTransition);
						}
						return;
				}
		
				//if any other zeds are visible, start herding
				if (StateZedRef.UpdateZedsICanSee () > 0) {
						Debug.Log ("I->H Transition: other Zeds nearby!");
						//StateZedRef.MoveTowardsOtherZeds ();
						StateZedRef.SetTransition (Transition.HerdingTransition);
						return;
				}
		
				//randomly decide to walk, with probability p = 0.01 / second
				if (GracePeriodCountdown <= 0) {						
						if (Random.Range (0.0f, 100.0f) <= 1 * Time.deltaTime) {
								StateZedRef.SetTransition (Transition.IdleToRoamTransition);
						}
				}
				
				if (Deadtime <= 0) {
						StateZedRef.SetTransition (Transition.IdleToPassiveTransition);
				}
		}
	
		public override void Act (Game game)
		{
				GracePeriodCountdown -= Time.deltaTime;
				Deadtime -= Time.deltaTime;
				//idle - nothing
		}
		
} // IdleState
