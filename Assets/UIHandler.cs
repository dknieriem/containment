using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
		GameWorld Game;
		WorldInfo World;
		Text CurrentDate;
	
		GameObject PlayerGroupInfoPanel;
	
		bool PlayerGroupInfoPanelEnabled = false;
		
		bool SectorInfoPanelEnabled = true;
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: UIHandler");
				Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				World = gameObject.GetComponentInChildren<WorldInfo> ();
				CurrentDate = GameObject.Find ("CurrentDate").GetComponent<Text> ();
			
				PlayerGroupInfoPanel = GameObject.Find ("PlayerGroupInfoPanel");
				PlayerGroupInfoPanel.SetActive (false);
		}
	
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
				CurrentDate.text = World.CurrentDate.ToString ();
				
				if (Game.IsDebug || (World.GetSectorFromScreenPos (Input.mousePosition) == null)) {
						SectorInfoPanelEnabled = false;
				} else {
						SectorInfoPanelEnabled = false;
				}
		}
		
		public void TogglePlayerGroupInfoPanel ()
		{
				PlayerGroupInfoPanelEnabled = !PlayerGroupInfoPanelEnabled;
		
				if (PlayerGroupInfoPanelEnabled) {
						PlayerGroupInfoPanel.SetActive (true);
				} else {
						PlayerGroupInfoPanel.SetActive (false);
				}
		}
}
