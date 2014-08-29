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
				//if interest in range, attack
		
				//if can't see interest, idle
			
			
		}
	
		public override void Act (GameObject game, GameObject npc)
		{
				//move toward the interest point
		}
	
} // MoveToInterestState
