using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{

		public float Amplitude = 1.0f;
		public float Radius = 5.0f;
		public float Duration = 2.0f;


		// Use this for initialization
		void Start ()
		{
	
		}
	
		// FixedUpdate is called once per physics step!
		void FixedUpdate ()
		{
				Duration -= Time.deltaTime;
			
				if (Duration <= 0) {
						
						if (gameObject.name.Contains ("Sound")) {
						
								Destroy (gameObject);
						
						} else {
						
								Destroy (this);
								
						}
						
				}
		}
	
		public float Magnitude (float distance)
		{
				if (distance > Radius || Radius <= 0) {
						return 0;
				} else {
						return Amplitude * Radius * Radius / distance / distance;
				}
	
		}
}
