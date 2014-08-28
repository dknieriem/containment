using UnityEngine;
using System.Collections;

public class MusicControllerInputHandler : MonoBehaviour
{

		public bool isPlaying = true;
		
		public AudioClip[] songs;
		
		public int currentClipNumber;
		
		private AudioSource musicPlayer;
	
		// Use this for initialization
		void Start ()
		{
	
				musicPlayer = GetComponent<AudioSource> ();
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
				if (Input.GetKeyDown (KeyCode.M)) {
						isPlaying = !isPlaying;
				}
				
				
				
	
				if (!isPlaying)
						musicPlayer.Stop ();
		
				if (isPlaying && (!musicPlayer.isPlaying || Input.GetButtonDown ("Next Song")))
						PlayNextSong ();
		}

		void PlayNextSong ()
		{
			
				currentClipNumber = (currentClipNumber + 1) % songs.Length;
			
				musicPlayer.clip = songs [currentClipNumber];
			
				musicPlayer.Play ();
			
		}

}
