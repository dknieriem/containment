using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIHandler : MonoBehaviour
{


		Text CurrentDate;
		WorldInfo World;
	
		// Use this for initialization
		void Start ()
		{
	
				World = gameObject.GetComponentInChildren<WorldInfo> ();
				CurrentDate = GameObject.Find ("CurrentDate").GetComponent<Text> ();
	
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
	
				CurrentDate.text = World.CurrentDate.ToString ();
	
		}
}
