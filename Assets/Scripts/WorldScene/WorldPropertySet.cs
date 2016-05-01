using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldPropertySet  : PropertySet
{

	void Start ()
	{
		//Debug.Log ("WorldPropertySet.Start()");
		properties = new Dictionary<string, Property> ();
		properties.Add ("World Size", new EnumProperty (new string[] { "Tiny", "Small", "Medium", "Large", "Extra Large" }, 2) as Property);
		properties.Add ("World Type", new EnumProperty (new string[] { "Rural", "Suburban", "Urban" }, 2) as Property);

		CreatePropertyObjects ();
	}

}