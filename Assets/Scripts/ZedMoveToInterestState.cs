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
		
				IEnumerable<Sound> sounds = game.GetComponentsInChildren<Sound> ();
		
				//		Debug.Log ("Total sounds: " + sounds.Count ());
		
				IEnumerable<Sound> npcSounds = npc.GetComponents<Sound> ();
		
				IEnumerable<Sound> targets = sounds.Where (s => Vector3.Distance (s.transform.position, npc.transform.position) < s.Radius).Except (npcSounds);
		
				//		Debug.Log ("Target sounds: " + targets.Count ());
		
				//2. keep only sounds with magnitude > threshhold T
				IEnumerable<Sound> targets2 = targets.Where (s => s.Magnitude (Vector3.Distance (s.transform.position, npc.transform.position)) > 0)
			.OrderByDescending (s => s.Magnitude (Vector3.Distance (s.transform.position, npc.transform.position)));
				//Debug.Log ("Target2 sounds: " + targets2.Count ());			
		
				if (targets2.Count () > 0 && targets2.First ().Magnitude (Vector3.Distance (targets2.First ().transform.position, npc.transform.position)) > npcZed.InterestMagnitude) {
			
						//3. find largest magnitude
						Sound target = targets2.First ();
			
						//Debug.Log ("New Target sound: " + target.transform.position + " with magnitude " + target.Magnitude (Vector3.Distance (target.transform.position, npc.transform.position)) + " over old mag " + npcZed.InterestMagnitude);
			
						//4. set npc target location to sound location
						npcZed.InterestLocation = target.transform.position;
						npcZed.InterestMagnitude = target.Magnitude (Vector3.Distance (target.transform.position, npc.transform.position));
			
						//5. set transition to get interest
						npcZed.CountdownToForgettingInterest = Random.Range (10, 25);
			
						return;
				}
		
				
						
				//if can't see interest, idle
			
				npcZed.CountdownToForgettingInterest -= Time.deltaTime;
			
				if (npcZed.CountdownToForgettingInterest <= 0) {
				
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);		
			
						npcZed.CountdownToForgettingInterest = Random.Range (10, 25);
				
						npcZed.SetTransition (Transition.LoseInterestTransition);
				}
			
		}
	
		public override void Act (GameObject game, GameObject npc)
		{
				//move toward the interest point
				Zed npcZed = npc.GetComponent<Zed> ();
		
				if (Vector2.Distance (npc.transform.position, npcZed.InterestLocation) < 0.5) {
			
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);
			
						npc.rigidbody2D.velocity = Vector2.zero;
						
						npcZed.SetTransition (Transition.LoseInterestTransition);
			
						//Debug.Log ("MoveToInterest -> Idle (Distance < 0.5)");
			
						return;
			
				} 
				
				npcZed.CountdownToForgettingInterest -= Time.deltaTime;
			
				Vector3 dirVector = npcZed.InterestLocation - npc.transform.position;
			
				//Debug.Log (dirVector.x + ", " + dirVector.y);
			
				float velocity = Mathf.Min (npcZed.MaxVelocity, dirVector.magnitude);
			
				dirVector.Normalize ();
			
				if (velocity > 0) {
						float rotationAngle = Mathf.Atan2 (dirVector.y, dirVector.x) * Mathf.Rad2Deg;
				
						npcZed.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, rotationAngle));
				}
			
				npcZed.transform.position += dirVector * velocity * Time.deltaTime;
			
			
				//Debug.Log ("Moving. Vel = " + velocity + ". Dir = " + direction + ".");
			
				// Apply the Velocity
				npc.rigidbody2D.velocity = dirVector * velocity * Time.deltaTime;
			
		}
			
} // MoveToInterestState
