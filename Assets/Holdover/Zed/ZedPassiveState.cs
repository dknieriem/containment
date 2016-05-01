using UnityEngine;

public class PassiveState : FSMState
{	
	
		public PassiveState (Zed zedRef)
		{ 
				stateID = StateID.PassiveStateID;
				StateZedRef = zedRef;
		}
	
		public override void DoBeforeEntering ()
		{
				base.DoBeforeEntering ();
		
				StateZedRef.Velocity = 0;
				StateZedRef.InterestMagnitude = 0;
				StateZedRef.DirVector = new Vector3 (0, 0);
				StateZedRef.InterestLocation = new Vector3 (0, 0);
		}
	
		public override void Reason (Game game)
		{
				// If any characters within instinct range
				//TODO: add detection of characters
		
				//Sound Detection
				if (StateZedRef.UpdateSoundsICanHear () > 0) {
						if (StateZedRef.SoundsICanHear [0].Magnitude (StateZedRef.transform.position) > Zed.SoundMagThreshholdForPassiveState) {
								StateZedRef.SetTransition (Transition.PassiveToIdleTransition);
								return;
						}
				}
		}
	
		public override void Act (Game game)
		{

		}
	
} // PassiveState

