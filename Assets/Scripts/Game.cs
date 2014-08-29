using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{

		public bool isDebug = true; //TODO on build, change to false

		private GameObject Button;

		// Use this for initialization
		void Start ()
		{
	
				Button = GameObject.Find ("ToggleDebugButton");
				
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void ToggleDebug ()
		{
		
				isDebug = !isDebug;
				
				Debug.Log ("Debug now: " + isDebug.ToString ());
		
				//TODO: change toggle button text (add/remove * to 
		
		}
		
		void Exit ()
		{
		
				Debug.Log ("Main Menu Exit");
		
				Application.Quit ();
		
		}
}
