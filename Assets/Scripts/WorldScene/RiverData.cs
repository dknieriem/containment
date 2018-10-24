using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverData : MonoBehaviour {


	public int river; //id of the river this is a part of
	public int siteId; //id of the sector this part is in
	public int flowToSiteId; //id of the sector this part flows to
	public Vector2 position; //Vector2 position of the something for this river section

	public Vector2 positionFrom; //Vector2 position of the sector this part is in
	public Vector2 positionTo; //Vector2 position of the sector this part flows to

	public RiverData(int river, int siteId, Vector2 position)
	{
		this.river = river;
		this.siteId = siteId;
		this.position = position;
	}

	public RiverData()
	{
		this.river = -1;
		this.siteId = -1;
		this.position = Vector2.negativeInfinity;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnDrawGizmos()
	{
		if (position == Vector2.negativeInfinity) 
		{
			return;
		}

		Gizmos.color = Color.blue;
			Gizmos.DrawLine(positionFrom, positionTo);
		}

}
