using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class IdleState : FSMState
{	
	
		public IdleState ()
		{ 
				stateID = StateID.IdleStateID;
		}
	
		public override void Reason (GameObject game, GameObject npc)
		{
	
				Zed npcZed = npc.GetComponent<Zed> ();
		
				// If any "sound" is within hearing range
				//1. linq find all sound objects within range X
				IEnumerable<Sound> sounds = game.GetComponentsInChildren<Sound> ();
		
				//		Debug.Log ("Total sounds: " + sounds.Count ());
		
				IEnumerable<Sound> npcSounds = npc.GetComponents<Sound> ();
		
				IEnumerable<Sound> targets = sounds.Where (s => Vector3.Distance (s.transform.position, npc.transform.position) < s.Radius).Except (npcSounds);
		
				//		Debug.Log ("Target sounds: " + targets.Count ());
				
				//2. keep only sounds with magnitude > threshhold T
				IEnumerable<Sound> targets2 = targets.Where (s => s.Magnitude (Vector3.Distance (s.transform.position, npc.transform.position)) > 0)
			.OrderByDescending (s => s.Magnitude (Vector3.Distance (s.transform.position, npc.transform.position)));
				//Debug.Log ("Target2 sounds: " + targets2.Count ());			
			
				if (targets2.Count () > 0) {
			
						//3. find largest magnitude
						Sound target = targets2.First ();
			
						//		Debug.Log ("Target sound: " + target.transform.position.ToString ());
			
						//4. set npc target location to sound location
						npcZed.InterestLocation = target.transform.position;
						npcZed.InterestMagnitude = target.Magnitude (Vector3.Distance (target.transform.position, npc.transform.position));
						
						//5. set transition to get interest
						npcZed.CountdownToForgettingInterest = Random.Range (10, 25);
						npcZed.SetTransition (Transition.GetInterestTransition);
			
						return;
				}
			
				// Or if any characters within instinct range
						
				//TODO: add detection of characters 
		
				//randomly decide to walk, with probability p
				
				if (npcZed.CountdownToForgettingInterest <= 0) {
				
						int f = UnityEngine.Random.Range (0, 100);
						
						if (f <= 3) {
						
								npcZed.InterestLocation.x = Random.Range (0, npcZed.Game.Terrain.Dimensions [0] - 1);
								npcZed.InterestLocation.y = Random.Range (0, npcZed.Game.Terrain.Dimensions [1] - 1);
						
								npcZed.CountdownToForgettingInterest = Random.Range (10, 25);
				
								npcZed.SetTransition (Transition.IdleToWalkTransition);

						}
			
				}
		
		}
	
		public override void Act (GameObject game, GameObject npc)
		{
				//idle - nothing
				
				Zed npcZed = npc.GetComponent<Zed> ();
				
				npcZed.CountdownToForgettingInterest -= Time.deltaTime;
				
		}
		
} // IdleState
