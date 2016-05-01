using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPropertySet : PropertySet
{

	// Use this for initialization
	void Start ()
	{
		//Debug.Log ("CharacterPropertySet.Start()");
		properties = new Dictionary<string, Property> ();
		properties.Add ("First Name", new StringProperty ("First Name", "first") as Property);
		properties.Add ("Last Name", new StringProperty ("Last Name", "last") as Property);
		CreatePropertyObjects ();
	}

}
