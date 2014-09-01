using UnityEngine;
using System.Collections;

public class TerrainInfo : MonoBehaviour
{

		public int[] Dimensions;

		// Use this for initialization
		void Start ()
		{
		
				Dimensions = new int[2];
			
				Dimensions [0] = 32;
				Dimensions [1] = 32;
		
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
