using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Game : MonoBehaviour
{

		public bool IsDebug = true; //TODO on build, change to false
		public List<Zed> GameZeds;
		public Sound[] GameSounds;			
		public TerrainInfo Terrain;
		public AudioClip[] ZedSounds;
		public AudioClip MapClick;
		public GameObject ZedPrefab;

		// Use this for initialization
		void Start ()
		{
				Terrain = gameObject.GetComponentInChildren<TerrainInfo> ();
	
				GameZeds = new List<Zed> (10);
				GameObject ZedParentObj = GameObject.Find ("Zeds");
				
				for (int i = 0; i < 10; i++) {
						GameObject zedObj = Instantiate (ZedPrefab) as GameObject;
						zedObj.transform.position.Set ((float)Random.Range (0, Terrain.Dimensions [0]), (float)Random.Range (0, Terrain.Dimensions [1]), 0); 
						zedObj.transform.parent = ZedParentObj.transform;
						Zed zed = zedObj.GetComponent<Zed> ();
						zed.name = "Zed " + zed.GUID;
						GameZeds.Add (zed);
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void FixedUpdate ()
		{
				GameZeds = gameObject.GetComponentsInChildren<Zed> ().ToList ();
				GameSounds = gameObject.GetComponentsInChildren<Sound> ();
				
		}
		
		void ToggleDebug ()
		{
				IsDebug = !IsDebug;
				Debug.Log ("Debug now: " + IsDebug.ToString ());
				Text buttonText = GameObject.Find ("ToggleDebugButton").GetComponentInChildren<Text> ();
				
				if (IsDebug) {
						buttonText.text = "Toggle Debug Off";
				} else {
						buttonText.text = "Toggle Debug On";
				}
		}
		
		void Exit ()
		{
				Debug.Log ("Main Menu Exit");
				Application.Quit ();
		}
}
