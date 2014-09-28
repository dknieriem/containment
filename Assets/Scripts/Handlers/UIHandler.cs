using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
		GameWorld Game;
		WorldInfo World;
		Text CurrentDate;
	
		GameObject PlayerGroupInfoPanel;
		bool PlayerGroupInfoPanelEnabled = false;
		GameObject SectorInfoPanel;
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
		
				SectorInfoPanel = GameObject.Find ("SectorInfoPanel");
		}
	
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
				CurrentDate.text = World.CurrentDate.ToString ();
				
				if ((Game.IsDebug || SectorInfoPanelEnabled) && World.GetSectorFromScreenPos (Input.mousePosition) != null) {
						SectorInfoPanel.SetActive (true);
				} else {
						SectorInfoPanel.SetActive (false);
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
		
		public void ToggleSectorInfoPanel ()
		{		
				SectorInfoPanelEnabled = !SectorInfoPanelEnabled;
		}
}
