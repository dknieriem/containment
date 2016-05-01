using UnityEngine;

public class RoamState : FSMState
{
		public float GracePeriodCountdown;
	
		public RoamState (Zed zedRef)
		{ 
				stateID = StateID.RoamStateID;
				StateZedRef = zedRef;
				
		}
	
		public override void DoBeforeEntering ()
		{
				base.DoBeforeEntering ();
		
				GracePeriodCountdown = 10.0f;
				StateZedRef.Velocity = 0;
				StateZedRef.InterestMagnitude = 0;
				StateZedRef.DirVector = Random.insideUnitSphere;
				StateZedRef.DirVector.z = 0;
				StateZedRef.DirVector.Normalize ();
		}
	
		public override void Reason (Game game)
		{	
				//Sound Detection
				if (StateZedRef.UpdateSoundsICanHear () > 0) {
						StateZedRef.SetTransition (Transition.ConfrontTransition);
						//StateZedRef.SetTransition (Transition.InvestigateTransition);
						return;
				}
		
				//if any other zeds are visible, move toward center of mass
				if (StateZedRef.UpdateZedsICanSee () > 0) {
						Debug.Log ("R->H Transition: other Zeds nearby!");
						StateZedRef.SetTransition (Transition.HerdingTransition);
						return;
				}
		
				//randomly decide to walk, with probability p = 0.01 / sec
				if (GracePeriodCountdown <= 0) {						
						if (Random.Range (0.0f, 100.0f) <= 1 * Time.deltaTime) {
								StateZedRef.SetTransition (Transition.RoamToIdleTransition);
						}
				}
		}
	
		public override void Act (Game game)
		{
				GracePeriodCountdown -= Time.deltaTime;
				float velocity = Random.Range (0.2f, 0.6f) * Zed.WalkVelocity; //Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
				if (velocity > 0) {
						float rotationAngle = Mathf.Atan2 (StateZedRef.DirVector.y, StateZedRef.DirVector.x) * Mathf.Rad2Deg;
						//StateZedRef.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
						StateZedRef.transform.eulerAngles = new Vector3 (0, 0, rotationAngle);
				}
			
				StateZedRef.transform.position += StateZedRef.DirVector * velocity * Time.deltaTime;
		}
} // RandomWalkState
