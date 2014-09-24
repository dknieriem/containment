using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameWorld : MonoBehaviour
{

		public bool IsDebug = true; //TODO on build, change to false
		public WorldInfo World;
		public AudioClip MapClick;
		
		// Use this for initialization
		void Start ()
		{
				World = gameObject.GetComponentInChildren<WorldInfo> ();
				
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
	
		void FixedUpdate ()
		{
		
		}
	
		void ToggleDebug ()
		{
				IsDebug = !IsDebug;
				Debug.Log ("Debug now: " + IsDebug);
				Text buttonText = GameObject.Find ("ToggleDebugButton").GetComponentInChildren<Text> ();
		
				if (IsDebug) {
						buttonText.text = "Debug On";
				} else {
						buttonText.text = "Debug Off";
				}
		}
	
		void Exit ()
		{
				Debug.Log ("Main Menu Exit");
				Application.Quit ();
		}
}
