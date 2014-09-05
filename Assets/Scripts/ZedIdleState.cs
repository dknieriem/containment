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
		
				// If any characters within instinct range
				//TODO: add detection of characters
		
				//Sound Detection
				if (npcZed.MoveTowardsSoundsICanHear ()) {
						npcZed.SetTransition (Transition.GetInterestTransition);
						return;
				}
		
				//if any other zeds are visible, move toward center of mass
				if (npcZed.MoveTowardsOtherZeds ()) {
						Debug.Log ("Transition: other Zeds nearby!");
						npcZed.SetTransition (Transition.IdleToWalkTransition);
						return;
				}
		
				//randomly decide to walk, with probability p
				
				if (npcZed.CountdownToRandomChange <= 0) {
				
						int f = UnityEngine.Random.Range (0, 100);
						
						if (f <= 1) {
						
								npcZed.InterestLocation.x = Random.Range (0, npcZed.Game.Terrain.Dimensions [0] - 1);
								npcZed.InterestLocation.y = Random.Range (0, npcZed.Game.Terrain.Dimensions [1] - 1);
						
								npcZed.DirVector = npcZed.InterestLocation - npc.transform.position;
								npcZed.DirVector.Normalize ();
								
								npcZed.CountdownToRandomChange = Random.Range (10, 25);
				
								npcZed.SetTransition (Transition.IdleToWalkTransition);

						}
			
				}
		
		}
	
		public override void Act (GameObject game, GameObject npc)
		{
				//idle - nothing
				
				Zed npcZed = npc.GetComponent<Zed> ();
				
				npcZed.CountdownToRandomChange -= Time.deltaTime;
				
		}
		
} // IdleState
