using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SectorInfoPanel : MonoBehaviour
{

	//public GameManager gameManager;
	//World world;
	RectTransform sectorInfoPanel;
	Sector CursorSector;
	//Text SectorName;
	Text SectorCoordsText;
	Text SectorZedsText;
	Text SectorPlayerGroupCountText;
	Text SectorTypeText;
	Vector3 SectorInfoPanelDimensions;
	public Vector3 InfoPanelCursorOffset = new Vector3 (10, 0, 0);

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Starting: SectorInfoScript");
		//gameManager = GameManager.Instance ();
		//world = gameManager.world;
		sectorInfoPanel = gameObject.GetComponent<RectTransform> ();
		SectorInfoPanelDimensions = new Vector3 (sectorInfoPanel.rect.width, -sectorInfoPanel.rect.height, 0);
		//SectorName = GameObject.Find ("SectorName").GetComponent<Text> ();
		SectorCoordsText = GameObject.Find ("SectorLocation").GetComponent<Text> ();
		SectorZedsText = GameObject.Find ("SectorZeds").GetComponent<Text> ();
		SectorTypeText = GameObject.Find ("SectorType").GetComponent<Text> ();
		SectorPlayerGroupCountText = GameObject.Find ("SectorPlayerGroupCount").GetComponent<Text> ();
	}

	public void SetSector (Sector newSector)
	{

		if (newSector != null)
			CursorSector = newSector;
	}

	// Update is called once per frame
	void Update ()
	{
		//CursorSector = World.GetSectorFromScreenPos (Input.mousePosition);
		if (CursorSector != null) {
			sectorInfoPanel.position = Input.mousePosition + (SectorInfoPanelDimensions / 2) + InfoPanelCursorOffset;
			SectorCoordsText.text = "(" + CursorSector.LocationX + "," + CursorSector.LocationY + ")";		
			SectorZedsText.text = "Pop: " + CursorSector.Population;
			SectorPlayerGroupCountText.text = "Group: " + CursorSector.PlayerGroupCount;
			SectorTypeText.text = CursorSector.SecType.ToString ();
			//sectorInfoPanel.gameObject.SetActive (true);
		} else {
			//sectorInfoPanel.gameObject.SetActive (false);
		}
	}
}
