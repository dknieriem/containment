using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
		GameWorld Game;
		WorldInfo World;
		Text CurrentDate;
		Sector CursorSector;
		RectTransform SectorInfoPanel;
		Text SectorName;
		Text SectorCoords;
		Vector3 SectorInfoPanelDimensions;
		public Vector3 InfoPanelCursorOffset = new Vector3 (10, 0, 0);
		
		// Use this for initialization
		void Start ()
		{
				Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				World = gameObject.GetComponentInChildren<WorldInfo> ();
				CurrentDate = GameObject.Find ("CurrentDate").GetComponent<Text> ();
				SectorInfoPanel = GameObject.Find ("SectorInfoPanel").GetComponent<RectTransform> ();
				SectorInfoPanelDimensions = new Vector3 (SectorInfoPanel.rect.width, - SectorInfoPanel.rect.height, 0);
				SectorName = GameObject.Find ("SectorName").GetComponent<Text> ();
				SectorCoords = GameObject.Find ("SectorLocation").GetComponent<Text> ();
		}
	
		void Update ()
		{
				if (Game.IsDebug) {
						CursorSector = World.GetSectorFromScreenPos (Input.mousePosition);
						if (CursorSector == null) {
								SectorInfoPanel.gameObject.SetActive (false);
						} else {
								SectorInfoPanel.gameObject.SetActive (true);
								SectorInfoPanel.position = Input.mousePosition + (SectorInfoPanelDimensions / 2) + InfoPanelCursorOffset;
								SectorCoords.text = "(" + CursorSector.LocationX + "," + CursorSector.LocationY + ")";		
						}
				} else {
						SectorInfoPanel.gameObject.SetActive (false);
				}
				
				
		}
		
		void FixedUpdate ()
		{
				CurrentDate.text = World.CurrentDate.ToString ();
		}
}
