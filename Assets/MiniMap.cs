using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour
{

		public Camera MainCamera;
		public RawImage MapImage;
		public GameWorld Game;

		// Use this for initialization
		void Start ()
		{
				Game = GameObject.Find ("Game").GetComponent<GameWorld> ();
				MainCamera = Camera.main;
				MapImage = gameObject.GetComponent<RawImage> ();
	
		}
	
		public void MoveCameraToClickPosition ()
		{
				RectTransform MapRect = MapImage.rectTransform;
				int[] worldDimensions = Game.World.Dimensions;
				
				Vector3 MapBottomLeft = MapImage.rectTransform.position;
				MapBottomLeft.x -= MapImage.rectTransform.rect.width / 2;
				MapBottomLeft.y -= MapImage.rectTransform.rect.height / 2;
				Debug.Log ("Map Position: " + MapBottomLeft + ". Click at " + Input.mousePosition);
				Vector2 worldPos = Input.mousePosition - MapBottomLeft;
				worldPos.x *= worldDimensions [0] / MapRect.rect.width;
				worldPos.y *= worldDimensions [1] / MapRect.rect.height;
				Debug.Log ("That's " + worldPos);
		
				if (worldPos.x < 0 || worldPos.x > worldDimensions [0] || worldPos.y < 0 || worldPos.y > worldDimensions [1]) {
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
