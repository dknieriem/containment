using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class RandomWalkState : FSMState
{
	
		public RandomWalkState ()
		{ 
				stateID = StateID.RandomWalkStateID;
		}
	
		public override void Reason (GameObject game, GameObject npc)
		{
		
				Zed npcZed = npc.GetComponent<Zed> ();
		
				//Sound Detection
				if (npcZed.MoveTowardsSoundsICanHear ()) {
						npcZed.SetTransition (Transition.GetInterestTransition);
						return;
				}
		
				//if any other zeds are visible, move toward center of mass
				if (npcZed.MoveTowardsOtherZeds ()) {
						//Debug.Log ("Transition: other Zeds nearby!");
						//npcZed.SetTransition (Transition.IdleToWalkTransition);
						return;
				}
		
				if (npcZed.CountdownToRandomChange <= 0) {
		
						//randomly decide to idle, with probability p
						int f = UnityEngine.Random.Range (0, 100);
						if (f <= 2) {
						
								npcZed.InterestLocation = new Vector3 (-1, -1, 0);
								npcZed.DirVector = new Vector3 (-1, -1, 0);
								npcZed.velocity = 0;
								npcZed.CountdownToRandomChange = Random.Range (10, 25);
				
								npcZed.SetTransition (Transition.WalkToIdleTransition);
						

						}
		
				}
		
		}
	
		public override void Act (GameObject game, GameObject npc)
		{

				Zed npcZed = npc.GetComponent<Zed> ();
		
				/*if (Vector2.Distance (npc.transform.position, npcZed.InterestLocation) < 0.5) {
			
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);
						
						npc.rigidbody2D.velocity = Vector2.zero;
			
						npcZed.SetTransition (Transition.WalkToIdleTransition);
			
						//Debug.Log ("Moving -> Idle (Distance < 0.5)");
						
						return;
			
				}*/ 
				
				npcZed.CountdownToRandomChange -= Time.deltaTime;
				
				//Vector3 dirVector = npcZed.InterestLocation - npc.transform.position;
				//dirVector.Normalize ();
				//Debug.Log (dirVector.x + ", " + dirVector.y);
			
				float velocity = Random.Range (0.6f, 1.0f) * Zed.WalkVelocity; //Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
			
				if (velocity > 0) {
						float rotationAngle = Mathf.Atan2 (npcZed.DirVector.y, npcZed.DirVector.x) * Mathf.Rad2Deg;
				
						npcZed.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
				}
			
				npcZed.transform.position += npcZed.DirVector * velocity * Time.deltaTime;
			
				//Debug.Log ("Moving. Vel = " + velocity + ". Dir = " + direction + ".");
						
				// Apply the Velocity
				//npc.rigidbody2D.velocity = npcZed.DirVector * velocity * Time.deltaTime;
					
		}
		
} // RandomWalkState
