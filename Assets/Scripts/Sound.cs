using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{

		public float Amplitude = 1.0f;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		public float Magnitude (float distance)
		{
	
				return Amplitude / (distance * distance * 4);
	
	
		}
}
