using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapImager : MonoBehaviour {

	//TODO: working on https://www.youtube.com/watch?time_continue=37&v=tijYJcJnl0I
	public float SnapDelay = 1.0f;
	public string SegmentName = "segment";
	public float Aspect = 1.0f;
	public int SuperSize = 2;
	public Camera otherCam;
	// Use this for initialization
	public void Capture () {
		Camera camera = GetComponent<Camera>();
		camera.enabled = true;

		Camera otherCam = GameObject.Find("Main Camera").GetComponent<Camera>();

		otherCam.enabled = false;

		camera.aspect = Aspect;

		//var xHalfUnit = camera.orthographicSize * Aspect;
		//var yHalfUnit = camera.orthographicSize;

		Helper.CreateAssetFolderIfNotExists("Minimap/Textures");

		ScreenCapture.CaptureScreenshot("Assets/Minimap/Textures/map.png");

		camera.enabled = false;
		otherCam.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
