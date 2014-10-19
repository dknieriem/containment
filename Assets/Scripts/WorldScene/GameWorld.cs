using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameWorld : MonoBehaviour
{

		public bool IsDebug = true; //TODO on build, change to false
		
		public bool IsPlaying = true; 
		
		public WorldBuilder Builder;
		public WorldInfo World;
		public AudioClip MapClick;
		public SpriteManager Sprites;
		
		public static float SecondsPerHour = 10.0f;
	
		public static float NextHourCountdown;
	
		public GameObject DebugPanel;
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: GameWorld");
				World = gameObject.GetComponentInChildren<WorldInfo> ();
				Debug.Log (World.name);
				Sprites = gameObject.GetComponent<SpriteManager> ();
				Builder = gameObject.GetComponent<WorldBuilder> ();
				NextHourCountdown = SecondsPerHour;
		}
	
		void FixedUpdate ()
		{
				NextHourCountdown -= Time.fixedDeltaTime;
				if (NextHourCountdown < 0) {
						World.DoNextUpdate ();
						NextHourCountdown = SecondsPerHour;		
				}
		}
	
		public void TogglePause ()
		{
		
				IsPlaying = !IsPlaying;
		
				if (IsPlaying) {
						Debug.Log ("Unpaused");
				} else {
						Debug.Log ("Paused");
				}
		
		}
	
		public void ToggleDebug ()
		{
				IsDebug = !IsDebug;
				Debug.Log ("Debug now: " + IsDebug);
				Text buttonText = GameObject.Find ("ToggleDebugButton").GetComponentInChildren<Text> ();
		
				if (IsDebug) {
						buttonText.text = "Debug On";
						DebugPanel.SetActive (true);
				} else {
						buttonText.text = "Debug Off";
						DebugPanel.SetActive (false);
				}
		}
	
		void Exit ()
		{
				Debug.Log ("Main Menu Exit");
				Application.Quit ();
		}
}
