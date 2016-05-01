using UnityEngine;

public class ConfrontState : FSMState
{	
	
		public ConfrontState (Zed zedRef)
		{ 
				stateID = StateID.ConfrontStateID;
				StateZedRef = zedRef;
		}
		public override void DoBeforeEntering ()
		{
				base.DoBeforeEntering ();
		}
		public override void Reason (Game game)
		{		
				StateZedRef.DirVector = StateZedRef.InterestLocation - StateZedRef.transform.position;
				StateZedRef.DirVector.Normalize ();
		
				//if interest in range, attack
		
						
				//if interest ran out before we got there, return to idle
				if (StateZedRef.InterestMagnitude < 0.5f) {
						StateZedRef.SetTransition (Transition.ConfrontationEndedTransition);
						return;
				}
				
				//if we're there, return to idle
				if (Vector2.Distance (StateZedRef.transform.position, StateZedRef.InterestLocation) < 0.5) {
						StateZedRef.SetTransition (Transition.ConfrontationEndedTransition);
						//Debug.Log ("Investigate -> Idle (Distance < 0.5)");
						return;
				}
		}
	
		public override void Act (Game game)
		{
				StateZedRef.InterestMagnitude *= (1 - 0.01f * Time.deltaTime); //lose 1% interest per second, compounded by physics timestep frequency
				StateZedRef.Velocity = Mathf.Clamp (StateZedRef.InterestMagnitude, Zed.MaxVelocity * 0.2f, Zed.MaxVelocity * 0.8f); //Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
		
				//if (StateZedRef.Velocity > 0) {
				float rotationAngle = Mathf.Atan2 (StateZedRef.DirVector.y, StateZedRef.DirVector.x) * Mathf.Rad2Deg;
				StateZedRef.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
				//}
		
				StateZedRef.transform.position += StateZedRef.DirVector * StateZedRef.Velocity * Time.deltaTime;
		
				// Apply the Velocity
				//npc.rigidbody2D.velocity = npcZed.DirVector * velocity * Time.deltaTime;
		}
	
} // ConfrontState