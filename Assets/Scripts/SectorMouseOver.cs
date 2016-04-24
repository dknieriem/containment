using UnityEngine;
using System.Collections;

public class SectorMouseOver : MonoBehaviour
{

	public UIHandler uiHandler;
	public Sector mySector;

	void OnAwake ()
	{
		uiHandler = UIHandler.Instance ();
		mySector = transform.GetComponentInParent<Sector> ();
	}

	void OnMouseOver ()
	{
		uiHandler.SectorInfoPanelEnabled = true;
		uiHandler.sectorInfoPanel.SetSector (mySector); 
	}
}
