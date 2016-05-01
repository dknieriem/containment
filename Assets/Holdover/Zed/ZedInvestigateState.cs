using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InvestigateState : FSMState
{	

		public float DistanceTraveled;
		public float OriginalDistance;
		
		public InvestigateState (Zed zedRef)
		{ 
				stateID = StateID.InvestigateStateID;
				StateZedRef = zedRef;
		}
	
		public override void DoBeforeEntering ()
		{
				base.DoBeforeEntering ();
	
				DistanceTraveled = 0.0f;
				OriginalDistance = Vector3.Distance (StateZedRef.transform.position, StateZedRef.InterestLocation);
		}
	
		public override void Reason (Game game)
		{				
				//if interest in range, attack
		
				// If any "sound" is within hearing range
				int soundCount = StateZedRef.UpdateSoundsICanHear ();	
		
				if (soundCount > 0) {
						Sound target = StateZedRef.SoundsICanHear.First ();	
						
						if (target.Magnitude (StateZedRef.transform.position) > StateZedRef.InterestMagnitude * 1.2f) {
								StateZedRef.InterestLocation = target.transform.position;
								StateZedRef.InterestMagnitude = target.Magnitude (Vector3.Distance (target.transform.position, StateZedRef.transform.position));
								return;
						}
				}
		
				//if no longer interested, return to idle
				if (StateZedRef.InterestMagnitude < 0.1f) { //(npcZed.CountdownToRandomChange <= 0) {
						StateZedRef.SetTransition (Transition.InvestigationEndedTransition);
						return;
				}
				
				if (Vector2.Distance (StateZedRef.transform.position, StateZedRef.InterestLocation) < 0.5) {
						StateZedRef.SetTransition (Transition.InvestigationEndedTransition);
						//Debug.Log ("Investigate -> Idle (Distance < 0.5)");
						return;
				}
				
				if (DistanceTraveled > 1.2f * OriginalDistance) {
						StateZedRef.SetTransition (Transition.InvestigationEndedTransition);
						//Debug.Log ("Investigate -> Idle (Distance < 0.5)");
						return;
				}
				
		}
	
		public override void Act (Game game)
		{
				//Vector3 dirVector = StateZedRef.InterestLocation - StateZedRef.transform.position;
				//Debug.Log (dirVector.x + ", " + dirVector.y);
				//dirVector.Normalize ();
			
				StateZedRef.InterestMagnitude *= (1 - 0.01f * Time.deltaTime);
				StateZedRef.Velocity = Mathf.Clamp (StateZedRef.InterestMagnitude, Zed.MaxVelocity * 0.2f, Zed.MaxVelocity * 0.8f); //Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
			
				if (StateZedRef.Velocity > 0) {
						float rotationAngle = Mathf.Atan2 (StateZedRef.DirVector.y, StateZedRef.DirVector.x) * Mathf.Rad2Deg;
						StateZedRef.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
				}
			
				Vector3 deltaPosition = StateZedRef.DirVector * StateZedRef.Velocity * Time.deltaTime;
				StateZedRef.transform.position += deltaPosition;
				DistanceTraveled += Vector3.Magnitude (deltaPosition);
				// Apply the Velocity
				//npc.rigidbody2D.velocity = npcZed.DirVector * velocity * Time.deltaTime;
		}
			
} // InvestigateState
