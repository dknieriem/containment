 	using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteManager : MonoBehaviour
{


		protected Dictionary<string,Sprite> SpriteDatabase = new Dictionary<string, Sprite> ();

		public string[] SpriteNames;

		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: SpriteManager");
				Sprite[] allSprites = Resources.LoadAll<Sprite> ("Sprites/Environment/SectorTiles");
				SpriteNames = new string[allSprites.Length];
	
				for (int i = 0; i < SpriteNames.Length; i++) {
						SpriteNames [i] = allSprites [i].name;
						SpriteDatabase.Add (SpriteNames [i], allSprites [i]);
						//Debug.Log (i + ": " + SpriteNames [i]);
				}
		}
	
				
		public Sprite ReturnSprite (string spriteName)
		{
				if (SpriteDatabase.ContainsKey (spriteName)) {
						Sprite result;
						SpriteDatabase.TryGetValue (spriteName, out result);
						return result;
				} else {
						return null;
				}
		}
		
		public Sprite RandomSprite ()
		{
				int r = Random.Range (0, SpriteNames.Length);
				string name = SpriteNames [r];
				Sprite result;
				SpriteDatabase.TryGetValue (name, out result);
				return result;
		}
}
