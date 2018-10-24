using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupPropertySet : PropertySet
{

	void Start ()
	{
		//Debug.Log ("CharacterPropertySet.Start()");
		properties = new Dictionary<string, Property> ();
        properties.Add("Group Members", new IntegerProperty("Group Members", 3, 2, 5) as Property);
        properties.Add("Starting Location", new EnumProperty("Starting Location", new string[] { "Camp Site", "House", "Fortified House" }, 1) as Property);
        properties.Add("Starting Sector", new EnumProperty("Starting Sector",  new string[] { "Not Cleared", "A Few Stray Zeds", "Secured" }, 1) as Property);
        //properties.Add ("Gov't Type", new EnumProperty (new string[] { "Council", "Dictatorship", "Fiefdom" }, 0) as Property);
		//properties.Add ("All Life Is Sacred", new BoolProperty ("All Life Is Sacred") as Property);
		//properties.Add ("Cannibal", new BoolProperty ("Cannibal") as Property);
		//properties.Add ("Zombies are People, Too", new BoolProperty ("Zombies are People, Too") as Property);
		properties.Add ("Group Mantra", new EnumProperty ("Group Mantra", new string[] {
			"Eliminate Zeds",
			"Repopulate the Earth",
			"New World Order",
			"Live and Let Live",
			"Retreat from the World"
		}, 0) as Property);
		CreatePropertyObjects ();
	}
}