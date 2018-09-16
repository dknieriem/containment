using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterPropertySet : PropertySet
{

    // Use this for initialization
    void Start()
    {
        //Debug.Log ("CharacterPropertySet.Start()");
        properties = new Dictionary<string, Property>();
        properties.Add("First Name", new StringProperty("First Name", "first") as Property);
        properties.Add("Last Name", new StringProperty("Last Name", "last") as Property);
        properties.Add("Age", new IntegerProperty("Age", 23, 18, 65) as Property);
        properties.Add("Gender", new EnumProperty(new string[] {"Female", "Male"}, 0) as Property);
		CreatePropertyObjects ();
	}

}
