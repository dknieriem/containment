using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Game : MonoBehaviour
{

		public bool IsDebug = true; //TODO on build, change to false

		public Zed[] GameZeds;

		public Sound[] GameSounds;
						
		public TerrainInfo Terrain;

		public AudioClip[] ZedSounds;
		
		public AudioClip MapClick;

		//private GameObject Button;

		// Use this for initialization
		void Start ()
		{
	
				Terrain = gameObject.GetComponentInChildren<TerrainInfo> ();
	
				//Button = GameObject.Find ("ToggleDebugButton");
				
				GameZeds = gameObject.GetComponentsInChildren<Zed> ();
				
				//Debug.Log ("Zeds: " + GameZeds.Length);
				
				foreach (Zed zed in GameZeds) {
				
						GameObject zedObj = zed.gameObject;
				
						zedObj.transform.position.Set ((float)Random.Range (0, Terrain.Dimensions [0]), (float)Random.Range (0, Terrain.Dimensions [1]), 0); 
					
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void FixedUpdate ()
		{
		
				GameZeds = gameObject.GetComponentsInChildren<Zed> ();
				
				GameSounds = gameObject.GetComponentsInChildren<Sound> ();
				
				//Debug.Log ("Sounds: " + GameSounds.Length);
		
		}
		
		void ToggleDebug ()
		{
		
				IsDebug = !IsDebug;
				
				Debug.Log ("Debug now: " + IsDebug.ToString ());
		
				//TODO: change toggle button text (add/remove * to 
				Text buttonText = GameObject.Find ("ToggleDebugButton").GetComponentInChildren<Text> ();
				
				if (IsDebug)
						buttonText.text = "Toggle Debug Off";
				else
						buttonText.text = "Toggle Debug On";
		
		}
		
		void Exit ()
		{
		
				Debug.Log ("Main Menu Exit");
		
				Application.Quit ();
		
		}
}
