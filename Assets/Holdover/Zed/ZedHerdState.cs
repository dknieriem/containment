using UnityEngine;
using System.Linq;

public class HerdState : FSMState
{	
	
		public float GracePeriodCountdown;
	
		public HerdState (Zed zedRef)
		{ 
				stateID = StateID.HerdStateID;
				StateZedRef = zedRef;
		}
	
		public override void DoBeforeEntering ()
		{
				base.DoBeforeEntering ();
		
				StateZedRef.InterestLocation = new Vector3 (0, 0);		
				StateZedRef.DirVector = new Vector3 (0, 0);	
				StateZedRef.Velocity = 0;
				//npc.rigidbody2D.velocity = Vector2.zero;
				//StateZedRef.CountdownToRandomChange = Random.Range (10, 25);
				GracePeriodCountdown = 20.0f;
		}
	
		public override void Reason (Game game)
		{
				// If any characters within instinct range
				//TODO: add detection of characters
		
				//Sound Detection
				if (StateZedRef.UpdateSoundsICanHear () > 0) {
						Sound target = StateZedRef.SoundsICanHear.First ();
			
						if (Vector3.Distance (target.transform.position, StateZedRef.transform.position) <= Zed.ConfrontMaximumDistance || target.Magnitude (StateZedRef.transform.position) > Zed.HerdConfrontMagThreshhold) {
								StateZedRef.SetTransition (Transition.ConfrontTransition);
								return;
						}
				}
		
				//if any other zeds are visible, move toward center of mass
				if (StateZedRef.MoveTowardsOtherZeds ()) {
						GracePeriodCountdown = 20.0f;
						return;
				} else if (GracePeriodCountdown <= 0) {
				
						StateZedRef.SetTransition (Transition.HerdingEndedTransition);
		
				}
		}
	
		public override void Act (Game game)
		{
				GracePeriodCountdown -= Time.deltaTime;
				float velocity = Random.Range (0.2f, 0.6f) * Zed.WalkVelocity; //Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
				if (velocity > 0) {
						float rotationAngle = Mathf.Atan2 (StateZedRef.DirVector.y, StateZedRef.DirVector.x) * Mathf.Rad2Deg;
						StateZedRef.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
				}
		
				StateZedRef.transform.position += StateZedRef.DirVector * velocity * Time.deltaTime;
		}
	
} // IdleState
