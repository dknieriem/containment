using UnityEngine;
using System.Collections;

public class Sector : MonoBehaviour
{

		public int LocationX, LocationY;
		
		public int ZedCount;
		
		public int[] GroupCount;

		public bool IsVisited = false;
		
		public bool IsVisible = false;
		
		SpriteRenderer mySprite;
		// Use this for initialization
		void Start ()
		{
				GroupCount = new int[WorldInfo.NumGroups];
				for (int i = 0; i < WorldInfo.NumGroups; i++) {
						GroupCount [i] = 0;
				}
				
				mySprite = gameObject.GetComponent<SpriteRenderer> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
				if (IsVisible) {
						mySprite.color = new Color (1.0f, 1.0f, 1.0f);
				} else if (IsVisited) {
						mySprite.color = new Color (0.7f, 0.7f, 1.0f);
				} else {
						mySprite.color = new Color (0.0f, 0.0f, 0.0f);
		
				}
		}
}
