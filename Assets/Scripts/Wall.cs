using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wall : MonoBehaviour
{

		public float Health = 100.0f;
	
		// Use this for initialization
		void Start ()
		{

		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		void FixedUpdate ()
		{
				if (Health <= 0.0f) {
						Destroy (gameObject);	
				}
		}
		
		public void Damage (float damage)
		{
				Health -= damage;
				//damageText.text = damage.ToString ().Substring (0, Mathf.Min (damage.ToString ().Length, 3));
		}
	
}
