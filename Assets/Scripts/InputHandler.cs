using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
{

		public float CameraPixelsPerSecond = 5.0f; 

		public GameObject SoundPrefab;

		Camera mainCamera;

		TerrainInfo terrainInfo;
		
		Game game;
		
		
		
		string currentSetFollowed;
	
		int currentZedMemberFollowed;
	
		int currentGroupMemberFollowed;
		
		// Use this for initialization
		void Start ()
		{
	
				mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
				//Debug.Log ("Camera name: " + mainCamera.name);
				
				terrainInfo = gameObject.GetComponentInChildren<TerrainInfo> ();//GameObject.Find ("Terrain").GetComponent<Terrain> ();
				//Debug.Log ("Terrain name: " + terrainInfo.name);
				
				game = gameObject.GetComponent<Game> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
		
				if (Input.GetButtonDown ("Previous Zed")) {
						ChangeCamTarget ("Zeds", -1);
				}
	
				if (Input.GetButtonDown ("Next Zed")) {
						ChangeCamTarget ("Zeds", +1);
				}
	
				if (Input.GetButtonDown ("Action 1")) {
						getClick ("Action 1");
				}
				
				if (Input.GetButtonDown ("Action 2")) {
						getClick ("Action 2");
				}
	
				MoveCamera ();
	
		}
	
		void MoveCamera ()
		{
	
				Vector3 newCameraPos = new Vector3 (mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
	
				float xDelta = Input.GetAxis ("Horizontal") * Time.deltaTime * CameraPixelsPerSecond;
				float yDelta = Input.GetAxis ("Vertical") * Time.deltaTime * CameraPixelsPerSecond;

				newCameraPos.x += xDelta;
				newCameraPos.y += yDelta;
				
				//mainCamera.transform.Translate (Vector3.up * yDelta);
				
				if (newCameraPos.x < 0) {
						newCameraPos.x = 0.0f;
				}
				
				if (newCameraPos.y < 0) {
						newCameraPos.y = 0.0f;
				}
				
				//TODO: stop camera from showing past edge of terrain
				
				if (newCameraPos.x > terrainInfo.Dimensions [0] - 1) {
						newCameraPos.x = (float)terrainInfo.Dimensions [0];
				}
		
				if (newCameraPos.y > terrainInfo.Dimensions [1] - 1) {
						newCameraPos.y = (float)terrainInfo.Dimensions [1];
				}
				
				mainCamera.transform.position = newCameraPos;
		}
	
		public void ChangeCamTarget (string groupName, int delta)
		{
		
				currentSetFollowed = groupName;
		
				if (groupName == "Zeds") {
			
						currentZedMemberFollowed = (currentZedMemberFollowed + delta) % game.GameZeds.Length;
			
						if (currentZedMemberFollowed < 0) 
								currentZedMemberFollowed += game.GameZeds.Length;
			
				}
		
				/*		if (groupName == "Characters") {
			
						currentGroupMemberFollowed = (currentGroupMemberFollowed + delta) % gameZeds.Length;
			
						if (currentGroupMemberFollowed < 0) 
								currentGroupMemberFollowed += gameZeds.Length;
			
				}
		*/
		
				MoveCamToTarget ();
		
		}
	
		public void MoveCamToTarget ()
		{
		
				if (currentSetFollowed == "Zeds") {
			
						Zed target = game.GameZeds [currentZedMemberFollowed];
			
						Vector3 newCamPos = new Vector3 (target.transform.position.x, target.transform.position.y, mainCamera.transform.position.z);
			
						mainCamera.transform.position = newCamPos;
			
				}
		
		
		}
		
		void getClick (string action)
		{
		
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				//Debug.Log (ray.origin.ToString()+", "+((ray.direction - Camera.main.transform.position) * 10).ToString());
		
				Debug.Log (action + ", " + ray.origin.ToString ());
		
				if (ray.origin.x > 0 && ray.origin.y > 0 && ray.origin.x < terrainInfo.Dimensions [0] && ray.origin.y < terrainInfo.Dimensions [1]) {
		
						Vector3 newSoundPosition = new Vector3 (ray.origin.x, ray.origin.y, 0);
				
						GameObject newSound = (GameObject)Instantiate (SoundPrefab, newSoundPosition, Quaternion.identity);
								
						Sound newSoundObj = newSound.GetComponent<Sound> ();
							
						newSoundObj.Amplitude = Random.Range (10, 20);
						newSoundObj.Duration = Random.Range (1, 5);
						newSoundObj.Radius = Random.Range (1, 10);
							
						//Debug.Log (GameObject.Find ("Sounds"));
														
						newSound.GetComponent<AudioSource> ().clip = game.GetComponent<Game> ().MapClick;
						newSound.GetComponent<AudioSource> ().Play ();
						newSound.name = "Sound " + Time.frameCount;
				
						newSound.transform.parent = GameObject.Find ("Sounds").transform;
				}
		}
}
