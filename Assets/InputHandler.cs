using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
{

		public float CameraPixelsPerSecond = 5.0f; 

		Camera mainCamera;

		Terrain terrain;
		
		// Use this for initialization
		void Start ()
		{
	
				mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
				Debug.Log ("Camera name: " + mainCamera.name);
				
				terrain = GameObject.Find ("Terrain").GetComponent<Terrain> ();
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
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
				
				if (newCameraPos.x > terrain.t_size [0]) {
						newCameraPos.x = (float)terrain.t_size [0];
				}
		
				if (newCameraPos.y > terrain.t_size [1]) {
						newCameraPos.y = (float)terrain.t_size [1];
				}
				
				mainCamera.transform.position = newCameraPos;
		}
	
}
