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
		
				//if can't see interest, idle
			
			
				npcZed.countdownToForgettingInterest -= Time.deltaTime;
			
				if (npcZed.countdownToForgettingInterest <= 0) {
				
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);		
			
						npcZed.countdownToForgettingInterest = Random.Range (10, 25);
				
						npcZed.SetTransition (Transition.LoseInterestTransition);
				}
			
		}
	
		public override void Act (GameObject game, GameObject npc)
		{
				//move toward the interest point
				Zed npcZed = npc.GetComponent<Zed> ();
		
				if (Vector2.Distance (npc.transform.position, npcZed.InterestLocation) < 0.5) {
			
						npcZed.SetTransition (Transition.LoseInterestTransition);
			
						npcZed.InterestLocation = new Vector3 (-1, -1, 0);
			
						npc.rigidbody2D.velocity = Vector2.zero;
			
						//		Debug.Log ("MoveToInterest -> Idle (Distance < 0.5)");
			
						return;
			
				} 
				
				npcZed.countdownToForgettingInterest -= Time.deltaTime;
			
				UpdateSound (game, npc);
				
			
				Vector3 dirVector = npcZed.InterestLocation - npc.transform.position;
			
				//Debug.Log (dirVector.x + ", " + dirVector.y);
			
				float velocity = Mathf.Min (npcZed.maxVelocity, dirVector.magnitude);
			
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
		
		public void UpdateSound (GameObject game, GameObject npc)
		{
		
				Zed npcZed = npc.GetComponent<Zed> ();
		
				npcZed.countdownToNextSound -= Time.deltaTime;
		
				if (npcZed.countdownToNextSound <= 0) { //play another sound
			
						if (npc.GetComponent<Sound> ()) { //no sound playing currently, not sure why this would still be here!
								UnityEngine.Object.Destroy (npc.GetComponent<Sound> ());
						}
			
						Sound newSound = npc.AddComponent<Sound> ();
						
						newSound.Amplitude = 1.0f;
						newSound.Duration = Random.Range (5, 20);
										
						int whichSound = Random.Range (0, game.GetComponent<Game> ().zedSounds.Length);
						
						npcZed.GetComponent<AudioSource> ().clip = game.GetComponent<Game> ().zedSounds [whichSound];
						npcZed.GetComponent<AudioSource> ().Play ();
						npcZed.countdownToNextSound = newSound.Duration;
			
				}
		
		}
	
} // MoveToInterestState
