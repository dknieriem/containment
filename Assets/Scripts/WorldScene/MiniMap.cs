using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour
{

	public Camera MainCamera;
	public RawImage MapImage;
	public GameManager gameManager;
		
	// Use this for initialization
	void Start ()
	{
		//MainCamera = Camera.main;
		MapImage = gameObject.GetComponent<RawImage> ();
		//gameManager = GameManager.Instance;
	
	}

	public void MoveCameraToClickPosition ()
	{
		RectTransform MapRect = MapImage.rectTransform;
		int worldDimensionsX = gameManager.world.DimensionsX;
		int worldDimensionsY = gameManager.world.DimensionsY;
				
		Vector3 MapBottomLeft = MapImage.rectTransform.position;
		MapBottomLeft.x -= MapImage.rectTransform.rect.width / 2;
		MapBottomLeft.y -= MapImage.rectTransform.rect.height / 2;
		Debug.Log ("Map Position: " + MapBottomLeft + ". Click at " + Input.mousePosition);
		Vector2 worldPos = Input.mousePosition - MapBottomLeft;
		worldPos.x *= worldDimensionsX / MapRect.rect.width;
		worldPos.y *= worldDimensionsY / MapRect.rect.height;
		Debug.Log ("That's " + worldPos);
		
		if (worldPos.x < 0 || worldPos.x > worldDimensionsX || worldPos.y < 0 || worldPos.y > worldDimensionsY) {
			throw new System.ArithmeticException ("Calculated World Position Invalid!");
		}
				
		MoveCameraToWorldPosition (worldPos);		
		
	}

	void MoveCameraToWorldPosition (Vector2 newPosition)
	{
		MainCamera.transform.position = newPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		if (Input.GetButtonDown ("Action 1")) {
			getClick ("Action 1");
		}
	
	}

	void getClick (string action)
	{
		//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		//Vector2 MapPosition = MapImage.rectTransform.position;
		//Debug.Log ("Map Position: " + MapPosition + ". Click at " + Input.mousePosition);
	}
}
