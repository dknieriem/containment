using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PassiveState : FSMState
{	
	
		public PassiveState ()
		{ 
				stateID = StateID.PassiveStateID;
		}
	
		public override void Reason (GameObject game, GameObject npc)
		{
				Zed npcZed = npc.GetComponent<Zed> ();
		
				// If any characters within instinct range
				//TODO: add detection of characters
		
				//Sound Detection
				if (npcZed.UpdateSoundsICanHear () > 0) {
						if (npcZed.SoundsICanHear [0].Magnitude > Zed.SoundMagThreshholdForPassiveState) {
								npcZed.SetTransition (Transition.PassiveToIdleTransition);
								return;
						}
				}
		}
	
		public override void Act (GameObject game, GameObject npc)
		{

		}
	
} // PassiveState
