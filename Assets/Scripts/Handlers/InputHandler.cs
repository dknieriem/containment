using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
	
	public float CameraPixelsPerSecond;

	public World world;
	public GameManager gameManager;
	public UIHandler uiHandler;
	public GameObject SoundPrefab;
	public Camera mainCamera;

	void Start ()
	{

	}

	void Update ()
	{
		
		if (Input.GetButtonDown ("Action 1")) {
			if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log ("Clicked an event system object...");
            }

            else
				getClick ("Action 1");
		}
		
		if (Input.GetButtonDown ("Action 2")) {
			if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log ("Clicked an event system object...");
            }

            else
				getClick ("Action 2");
		}
				
		if (Input.GetKeyDown ("i")) {
			uiHandler.ToggleSectorInfoPanel ();
		}

		MoveCamera ();
		
	}

	void MoveCamera ()
	{
		
		Vector3 newCameraPos = new Vector3 (mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);

		float xDelta; //= Input.GetAxis ("Horizontal") * Time.deltaTime * CameraPixelsPerSecond;
		float yDelta;// = Input.GetAxis ("Vertical") * Time.deltaTime * CameraPixelsPerSecond;
		float zoomDelta; //= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * CameraPixelsPerSecond;

		if (gameManager.isPaused)
		{
			//Debug.Log("paused");
			xDelta = Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * CameraPixelsPerSecond;
			yDelta = Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime * CameraPixelsPerSecond;
			zoomDelta = Input.GetAxisRaw("Mouse ScrollWheel") * Time.unscaledDeltaTime * CameraPixelsPerSecond;
		} else
		{
			xDelta = Input.GetAxis("Horizontal") * Time.deltaTime * CameraPixelsPerSecond;
			yDelta = Input.GetAxis("Vertical") * Time.deltaTime * CameraPixelsPerSecond;
			zoomDelta = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * CameraPixelsPerSecond;
		}

		newCameraPos.x += xDelta;
		newCameraPos.y += yDelta;
		
		
		//Debug.Log ("Mousewheel: " + zoomDelta);
		if (!Mathf.Approximately (zoomDelta, 0.0f)) {
			float newZoom = mainCamera.orthographicSize * (1.0f - zoomDelta); 
			if (newZoom < 32) {
				newZoom = 32;
			}
			if (newZoom > 64) {
				newZoom = 64;
			}
			mainCamera.orthographicSize = newZoom;
		}
		//mainCamera.transform.Translate (Vector3.up * yDelta);
		
		if (newCameraPos.x < 0) {
			newCameraPos.x = 0.0f;
		}
		
		if (newCameraPos.y < 0) {
			newCameraPos.y = 0.0f;
		}
		
		//TODO: stop camera from showing past edge of terrain
		
		if (newCameraPos.x > world.DimensionsX - 1) {
			newCameraPos.x = (float)world.DimensionsX - 1;
		}
		
		if (newCameraPos.y > world.DimensionsY - 1) {
			newCameraPos.y = (float)world.DimensionsY - 1;
		}
		
		mainCamera.transform.position = newCameraPos;
				
	}
	
	/*public void ChangeCamTarget (string groupName, int delta)
		{
				currentSetFollowed = groupName;
				if (groupName == "Zeds") {
						currentZedMemberFollowed = (currentZedMemberFollowed + delta) % game.GameZeds.Count;
						if (currentZedMemberFollowed < 0) 
								currentZedMemberFollowed += game.GameZeds.Count;
				}
				/*		if (groupName == "Characters") {
			
						currentGroupMemberFollowed = (currentGroupMemberFollowed + delta) % gameZeds.Length;
			
						if (currentGroupMemberFollowed < 0) 
								currentGroupMemberFollowed += gameZeds.Length;
				}
				MoveCamToTarget ();
		}*/
	
	/*public void MoveCamToTarget ()
		{
				if (currentSetFollowed == "Zeds") {
						Zed target = game.GameZeds [currentZedMemberFollowed];
						Vector3 newCamPos = new Vector3 (target.transform.position.x, target.transform.position.y, mainCamera.transform.position.z);
						mainCamera.transform.position = newCamPos;
				}		
		}*/
	
	void getClick (string action)
	{
		Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		//Debug.Log (action + ", " + ray.origin);
				
		if (ray.origin.x > 0 && ray.origin.y > 0 && ray.origin.x < world.DimensionsX && ray.origin.y < world.DimensionsY) {
			Sector sectorClicked = null; // world.GetSectorAtPosition (ray.origin);
			
            if(sectorClicked != null) {
                //Debug.Log("Clicked " + sectorClicked.LocationX + ", " + sectorClicked.LocationY);
                uiHandler.ShowSectorInfoForSector(sectorClicked);
            } else
            {
               // Debug.Log("Clicked but no sector");
            }
		}
	}

}