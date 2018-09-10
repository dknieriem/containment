using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugInfoScript : MonoBehaviour
{

	public RawImage MapImage;
	public Texture Tex;

	// Use this for initialization
	void Start ()
	{
	
		//	MapImage = GameObject.Find ("DebugMapImage").GetComponent<RawImage> ();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void GetNewMapImage (Texture newTexture)
	{
		Debug.Log ("New Image Map!");
		Tex = newTexture;
		MapImage.texture = Tex;
	}
}
