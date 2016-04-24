using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
	public GameManager gameManager;
	public World world;
	public Text CurrentDate;
	public GameObject playerGroupInfoPanel;
	public UIWindowScript debugInfoPanel;
	public bool PlayerGroupInfoPanelEnabled = false;
	public SectorInfoPanel sectorInfoPanel;
	public bool SectorInfoPanelEnabled = false;
	public GameObject personInfoPanel;
	public Button toggleDebugButton;
	public Button togglePauseButton;

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

	public void TogglePlayerGroupInfoPanel ()
	{
		playerGroupInfoPanel.GetComponent<UIWindowScript> ().TogglePanel ();
		
		//PlayerGroupInfoPanel.SetActive (PlayerGroupInfoPanelEnabled);
	}

	public void ToggleSectorInfoPanel ()
	{		
		sectorInfoPanel.GetComponent<UIWindowScript> ().TogglePanel ();
	}

	public void TogglePause ()
	{
		if (!gameManager.inGame)
			return;

		Text pauseText = togglePauseButton.GetComponentInChildren<Text> ();
		gameManager.TogglePause ();
		if (gameManager.isPaused)
			pauseText.text = "<color='red'>Pause</color>";
		else
			pauseText.text = "Unpause";
		
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

	public void Exit ()
	{
		Debug.Log ("UIHandler.Exit()");
		//save game
		Application.Quit ();
	}

}
