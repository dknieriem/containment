﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameWorld : MonoBehaviour
{

		public bool IsDebug = true; //TODO on build, change to false
		
		public bool IsPlaying = true; 
		
		public WorldInfo World;
		public AudioClip MapClick;
		public SpriteManager Sprites;
		
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: GameWorld");
				World = gameObject.GetComponentInChildren<WorldInfo> ();
				Sprites = gameObject.GetComponent<SpriteManager> ();
				
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
	
		void FixedUpdate ()
		{
		
		}
	
		void TogglePause ()
		{
		
				IsPlaying = !IsPlaying;
		
				if (IsPlaying) {
						Debug.Log ("Unpaused");
				} else {
						Debug.Log ("Paused");
				}
		
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
