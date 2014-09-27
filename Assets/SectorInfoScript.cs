using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SectorInfoScript : MonoBehaviour
{

		GameWorld Game;
		WorldInfo World;
		RectTransform SectorInfoPanel;
		Sector CursorSector;
		Text SectorName;
		Text SectorCoords;
		Text SectorZeds;
		Text SectorPlayerGroupCount;
		Vector3 SectorInfoPanelDimensions;
		public Vector3 InfoPanelCursorOffset = new Vector3 (10, 0, 0);

		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: SectorInfoScript");
				Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				World = GameObject.Find ("World").GetComponent<WorldInfo> ();
				SectorInfoPanel = GameObject.Find ("SectorInfoPanel").GetComponent<RectTransform> ();
				SectorInfoPanelDimensions = new Vector3 (SectorInfoPanel.rect.width, - SectorInfoPanel.rect.height, 0);
				SectorName = GameObject.Find ("SectorName").GetComponent<Text> ();
				SectorCoords = GameObject.Find ("SectorLocation").GetComponent<Text> ();
				SectorZeds = GameObject.Find ("SectorZeds").GetComponent<Text> ();
				SectorPlayerGroupCount = GameObject.Find ("SectorPlayerGroupCount").GetComponent<Text> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				CursorSector = World.GetSectorFromScreenPos (Input.mousePosition);
				if (CursorSector != null) {
						SectorInfoPanel.position = Input.mousePosition + (SectorInfoPanelDimensions / 2) + InfoPanelCursorOffset;
						SectorCoords.text = "(" + CursorSector.LocationX + "," + CursorSector.LocationY + ")";		
						SectorZeds.text = "Zeds: " + CursorSector.ZedCount;
						SectorPlayerGroupCount.text = "Group Members: " + CursorSector.PlayerGroupCount;
				}
		}
}
