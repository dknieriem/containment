using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
	public GameManager gameManager;
	public World world;
	public Text CurrentDate;
	public GameObject playerGroupInfoPanel;
	public UIWindowScript debugInfoPanel;
	//public bool PlayerGroupInfoPanelEnabled = false;
	public SectorInfoPanel sectorInfoPanel;
	public bool SectorInfoPanelEnabled = false;
	public List<GameObject> personInfoPanels;
    public List<GameObject> relationshipInfoPanels;
    public GameObject personInfoPanelPrefab;
    public GameObject relationshipInfoPanelPrefab;
	public Button toggleDebugButton;
	public Button togglePauseButton;
	public Sprite pauseSprite;
	public Sprite playSprite;

	private static UIHandler uiHandler;

	public static UIHandler Instance ()
	{
		if (!uiHandler) {
			uiHandler = FindObjectOfType (typeof(UIHandler)) as UIHandler;
			if (!uiHandler)
				Debug.LogError ("There needs to be one active UIHandler script on a GameObject in your scene.");
		}
        
		return uiHandler;
	}
	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Starting: UIHandler");
		gameManager = GameManager.Instance;
		world = gameManager.world;
		//CurrentDate = GameObject.Find ("CurrentDate").GetComponent<Text> ();
			
		//PlayerGroupInfoPanel.SetActive (false);
		//sectorInfoPanel.gameObject.SetActive (false);

	}

	void Update ()
	{
	
	}

	void FixedUpdate ()
	{
		CurrentDate.text = world.CurrentDate.ToString ();
		
		//if ((GameManager.Instance.IsDebug || SectorInfoPanelEnabled) && world.GetSectorFromScreenPos (Input.mousePosition) != null) {
		//	SectorInfoPanel.SetActive (true);
		//} else {
		//	SectorInfoPanel.SetActive (false);
		//}
	}

    public PersonInfoPanel AddPersonInfoPanel(Person personToShow)
    {
        Debug.Log("AddPersonInfoPanel( " + personToShow.FirstName + " )");

        //check if we already have a panel for this person;
        foreach(GameObject obj in personInfoPanels){

            PersonInfoPanel panel = obj.GetComponent<PersonInfoPanel>();
            Debug.Log("looking at " + panel.getPerson().Id);
            if (panel.getPerson().Id == personToShow.Id)
            {
                obj.transform.SetAsLastSibling();
                obj.GetComponent<UIWindowScript>().OpenPanel();
                return panel;
            }
        }

        //if we reach here, there was no existing panel. Instantiate the prefab, set it up, and return it.

        GameObject newObject = (GameObject)Instantiate(personInfoPanelPrefab);
        personInfoPanels.Add(newObject);

        newObject.transform.SetParent(gameObject.transform.Find("Window Layer"));
        newObject.GetComponent<UIWindowScript>().OpenPanel();

        PersonInfoPanel newPanel = newObject.GetComponent < PersonInfoPanel>();

        newPanel.ShowPerson(personToShow);
        return newPanel;
    }

    public RelationshipInfoPanel AddRelationshipInfoPanel(Relationship relationship)
    {
        Debug.Log("AddRelationshipInfoPanel()");

        //check if we already have a panel for this person;
        foreach (GameObject obj in relationshipInfoPanels)
        {

            RelationshipInfoPanel panel = obj.GetComponent<RelationshipInfoPanel>();
            Debug.Log("looking at " + panel.getRelationship().Id);
            if (panel.getRelationship().Id == relationship.Id)
            {
                obj.transform.SetAsLastSibling();
                obj.GetComponent<UIWindowScript>().OpenPanel();
                return panel;
            }
        }

        //if we reach here, there was no existing panel. Instantiate the prefab, set it up, and return it.

        GameObject newObject = (GameObject)Instantiate(relationshipInfoPanelPrefab);
        relationshipInfoPanels.Add(newObject);

        newObject.transform.SetParent(gameObject.transform.Find("Window Layer"));
        newObject.GetComponent<UIWindowScript>().OpenPanel();

        RelationshipInfoPanel newPanel = newObject.GetComponent<RelationshipInfoPanel>();

        newPanel.ShowRelationship(relationship);
        return newPanel;
    }

    public void TogglePlayerGroupInfoPanel ()
	{
		playerGroupInfoPanel.GetComponent<UIWindowScript> ().TogglePanel ();
		
		//PlayerGroupInfoPanel.SetActive (PlayerGroupInfoPanelEnabled);
	}

	public void ToggleSectorInfoPanel ()
	{		
		sectorInfoPanel.GetComponent<UIWindowScript> ().TogglePanel ();
	}

    public void SetSectorInfoPanelSector (Sector newSector)
    {
        sectorInfoPanel.SetSector(newSector);
    }

    public void ShowSectorInfoForSector (Sector newSector)
    {
        SetSectorInfoPanelSector(newSector);
        sectorInfoPanel.GetComponent<UIWindowScript> ().OpenPanel ();
    }

	public void TogglePause ()
	{
		if (!gameManager.inGame)
			return;

		//Text pauseText = togglePauseButton.GetComponentInChildren<Text> ();
		SVGImage pauseImage = togglePauseButton.GetComponentInChildren<SVGImage>();
		gameManager.TogglePause ();
		if (gameManager.isPaused)
			//pauseText.text = "Unpause";
			pauseImage.sprite = playSprite;
		else
			//pauseText.text = "<color='red'>Pause</color>";
			pauseImage.sprite = pauseSprite;

	}

	public void ToggleDebug ()
	{
		Text debugText = toggleDebugButton.GetComponentInChildren<Text> ();
		gameManager.ToggleDebug ();
		if (gameManager.IsDebug) {
			debugText.text = "Debug <color='green'>On</color>";
			debugInfoPanel.OpenPanel ();
		} else {
			debugText.text = "Debug <color='red'>Off</color>";
			debugInfoPanel.ClosePanel ();
		}
	}

    public void RemoveAllDisposableWindows()
    {
       UIWindowScript[] AllWindows = FindObjectsOfType<UIWindowScript>();

        foreach(UIWindowScript Window in AllWindows)
        {
            if (Window.isDisposable)
            {
                Window.RemovePanel();
            } else
            {
                Window.ClosePanel();
            }
        }

    }

	public void Exit ()
	{
		Debug.Log ("UIHandler.Exit()");
        //TODO:save game
 
        Application.Quit();

	}

}
