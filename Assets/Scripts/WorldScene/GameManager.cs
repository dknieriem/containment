using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{

	public static GameManager Instance = null;

	public bool IsDebug = false;
	public bool inGame = false;
	public bool isPaused = true;

	public WorldBuilder worldBuilder;
	public MapGenerator mapGenerator;
	public World world;
	public Group playerGroup;
	public AudioClip MapClickAudio;
	public SpriteManager Sprites;
	public LuaLoader luaLoader;
    public UIHandler uiHandler;
    public GameObject DebugPanel;

    public float SecondsPerHour = 10.0f;

	public float NextHourCountdown = 10.0f;
	


	void Awake ()
	{
		if (Instance == null) //Check if instance already exists
			Instance = this; //if not, set instance to this
		else if (Instance != this)  //If instance already exists and it's not this:
			Destroy (gameObject); //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.   

		if (isPaused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;

		DontDestroyOnLoad (gameObject); //Sets this to not be destroyed when reloading scene
	}
		
	// Use this for initialization
	void Start ()
	{
		NextHourCountdown = SecondsPerHour;
		if (isPaused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}

	void FixedUpdate ()
	{

		NextHourCountdown -= Time.fixedDeltaTime;
//		Debug.Log (Time.fixedDeltaTime);
		if (NextHourCountdown < 0) {
			NextHourCountdown = SecondsPerHour;		
			//Debug.Log ("Next hour! " + NextHourCountdown);
			world.DoNextUpdate ();
		}
	}

	public void TogglePause ()
	{
		isPaused = !isPaused;
		
		if (isPaused) {
			Debug.Log ("Paused");
			Time.timeScale = 0;
		} else {
			Debug.Log ("Unpaused");
			Time.timeScale = 1;
		}
	}

	public void ToggleDebug ()
	{
		IsDebug = !IsDebug;
	}

	public void NewGame (CharacterPropertySet charProps, GroupPropertySet groupProps, WorldPropertySet worldProps)
	{
		//NewGameWindow newGameWindow = GameObject.Find ("New Game Window").GetComponent<NewGameWindow> ();
		Debug.Log ("GameManager.NewGame(props)");
		worldBuilder.BuildWorld (charProps, groupProps, worldProps);
		inGame = true;
	}

	public void NewGame ()
	{
		NewGameWindow newGameWindow = GameObject.Find ("New Game Window").GetComponent<NewGameWindow> ();
		Debug.Log ("GameManager.NewGame()");
		worldBuilder.BuildWorld (newGameWindow.charPropertySet, newGameWindow.groupPropertySet, newGameWindow.worldPropertySet);
		inGame = true;
	}
}
