using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MoveToInterestState : FSMState
{	

		public MoveToInterestState ()
		{ 
				stateID = StateID.MoveToInterestStateID;
		}
	
		public override void Reason (GameObject game, GameObject npc)
		{
		
				Zed npcZed = npc.GetComponent<Zed> ();
				
				//if interest in range, attack
		
				// If any "sound" is within hearing range
				int soundCount = npcZed.UpdateSoundsICanHear ();	
		
				if (soundCount > 0 && npcZed.SoundsICanHear.First ().Magnitude (Vector3.Distance (npcZed.SoundsICanHear.First ().transform.position, npc.transform.position)) > npcZed.InterestMagnitude) {
			
						// find largest magnitude
						Sound target = npcZed.SoundsICanHear.First ();	
			
						// set npc target location to sound location
						npcZed.InterestLocation = target.transform.position;
						npcZed.InterestMagnitude = target.Magnitude (Vector3.Distance (target.transform.position, npc.transform.position));
			
						return;
				}
		
				
						
				//if no longer interested, return to idle
						
				if (npcZed.InterestMagnitude < 0.1f) { //(npcZed.CountdownToRandomChange <= 0) {
				
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);		
						npcZed.DirVector = new Vector3 (-1, -1, 0);	
						npcZed.velocity = 0;
						npcZed.CountdownToRandomChange = Random.Range (10, 25);
				
						npcZed.SetTransition (Transition.LoseInterestTransition);
				}
			
		}
	
		public override void Act (GameObject game, GameObject npc)
		{
				//move toward the interest point
				Zed npcZed = npc.GetComponent<Zed> ();
		
				/*if (Vector2.Distance (npc.transform.position, npcZed.InterestLocation) < 0.5) {
			
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);
			
						npcZed.DirVector = new Vector3 (-1, -1, 0);
			
						npc.rigidbody2D.velocity = Vector2.zero;
						
						npcZed.SetTransition (Transition.LoseInterestTransition);
			
						//Debug.Log ("MoveToInterest -> Idle (Distance < 0.5)");
			
						return;
			
				} */
				
				//npcZed.CountdownToRandomChange -= Time.deltaTime;
			
				//Vector3 dirVector = npcZed.InterestLocation - npc.transform.position;
			
				//Debug.Log (dirVector.x + ", " + dirVector.y);
				
				//dirVector.Normalize ();
			
				npcZed.InterestMagnitude *= (1 - 0.01f * Time.deltaTime);
			
				npcZed.velocity = Mathf.Clamp (npcZed.InterestMagnitude, Zed.MaxVelocity * 0.2f, Zed.MaxVelocity); //Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
			
	
			
				if (npcZed.velocity > 0) {
						float rotationAngle = Mathf.Atan2 (npcZed.DirVector.y, npcZed.DirVector.x) * Mathf.Rad2Deg;
				
						npcZed.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
				}
			
				npcZed.transform.position += npcZed.DirVector * npcZed.velocity * Time.deltaTime;
			
			
				//Debug.Log ("Moving. Vel = " + velocity + ". Dir = " + direction + ".");
			
				// Apply the Velocity
				//npc.rigidbody2D.velocity = npcZed.DirVector * velocity * Time.deltaTime;
			
		}
			
} // MoveToInterestState
