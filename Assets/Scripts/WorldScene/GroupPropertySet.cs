using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupPropertySet : PropertySet
{

	void Start ()
	{
		//Debug.Log ("CharacterPropertySet.Start()");
		properties = new Dictionary<string, Property> ();
		properties.Add ("Gov't Type", new EnumProperty (new string[] { "Council", "Dictatorship", "Fiefdom" }, 0) as Property);
		properties.Add ("Stance", new EnumProperty (new string[] { "Hiding", "Avoidant", "Defensive", "Aggressive" }, 2) as Property);
		properties.Add ("All Life Is Sacred", new BoolProperty ("All Life Is Sacred") as Property);
		properties.Add ("Cannibal", new BoolProperty ("Cannibal") as Property);
		properties.Add ("Zombies are People, Too", new BoolProperty ("Zombies are People, Too") as Property);
		properties.Add ("Group Mantra", new EnumProperty (new string[] {
			"Eliminate Zeds",
			"Repopulate the Earth",
			"New World Order",
			"Live and Let Live",
			"Retreat from the World"
		}, 0) as Property);
		CreatePropertyObjects ();
	}
}