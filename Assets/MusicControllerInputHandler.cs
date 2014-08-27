using UnityEngine;
using System.Collections;

public class MusicControllerInputHandler : MonoBehaviour
{

		public bool isPlaying = true;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
				if (Input.GetKeyDown (KeyCode.M)) {
						isPlaying = !isPlaying;
			
			
						AudioSource musicPlayer = GetComponent<AudioSource> ();
			
						if (isPlaying)
								musicPlayer.Play ();
						else
								musicPlayer.Pause ();
				}
	
		}
}
