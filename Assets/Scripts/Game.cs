using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{

		public bool isDebug = true; //TODO on build, change to false

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void ToggleDebug ()
		{
		
				isDebug = !isDebug;
				
				Debug.Log ("Debug now: " + isDebug.ToString ());
		
		}
		
		void Exit ()
		{
		
				Debug.Log ("Main Menu Exit");
		
				Application.Quit ();
		
		}
}
