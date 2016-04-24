using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PersonInfoPanel : MonoBehaviour
{
	public Faction group;
		
	Person person = null;
		
	Text PersonNameText;
	Text PersonLocationText;
	Text PersonStatsText;
	Text PersonRelationshipsText;
	//Text PersonRoleText;

	private static PersonInfoPanel panel;

	public static PersonInfoPanel Instance ()
	{
		if (!panel) {
			panel = FindObjectOfType (typeof(PersonInfoPanel)) as PersonInfoPanel;
			if (!panel)
				Debug.LogError ("There needs to be one active PersonInfoPanel script on a GameObject in your scene.");
		}
        
		return panel;
	}

	void Start ()
	{
		Debug.Log ("Starting: PersonInfoScript");
		PersonNameText = GameObject.Find ("PersonName").GetComponent<Text> ();
		PersonLocationText = GameObject.Find ("PersonLocation").GetComponent<Text> ();
		PersonStatsText = GameObject.Find ("PersonStats").GetComponent<Text> ();
		PersonRelationshipsText = GameObject.Find ("PersonRelationships").GetComponent<Text> ();
		//PersonRoleText = GameObject.Find ("PersonRole").GetComponent<Text> ();

		//Debug.Log (group.name);
		//Debug.Log (group.GroupMembers.ToArray ().Length);
		Debug.Log ("Group: " + group.ToString ());
		group = GameManager.Instance.world.PlayerGroup;
		Debug.Log ("Group: " + group.ToString ());
		//UpdatePerson (group.GroupMembers.ToArray () [0]);
	}

	public void ShowPerson (Person newPerson)
	{
		if (newPerson == null) {
			Debug.Log ("ShowPerson... no person!");
		} else {
			SetPerson (newPerson);
		}
		gameObject.transform.SetAsLastSibling ();
		gameObject.GetComponent<UIWindowScript> ().OpenPanel ();
	}

	public void SetPerson (Person newPerson)
	{
		if (person == newPerson) {
			return;
		}
		person = newPerson;
		PersonNameText.text = string.Format ("{0} {1}", person.FirstName, person.LastName);
		Debug.Log ("New Person! " + PersonNameText.text);
	}

	void FixedUpdate ()
	{
		if (person == null) {
			return;
		}
				
		PersonLocationText.text = "(" + person.LocationX + "," + person.LocationY + ")";
		//PersonRoleText.text = "Role: " + Person.RoleNames [(int)person.CurrentRole];
				
		PersonStatsText.text = "Stats: \n";
		for (int i = 0; i < Enum.GetNames (typeof(Person.Stats)).Length; i++) {
			PersonStatsText.text += Person.StatNames [i] + person.CurrentStats [i] + "/" + person.BaseStats [i] + "\n";
		}
	
		PersonStatsText.text += "Zeds Killed: " + person.LifetimeZedKills;
					
		PersonRelationshipsText.text = "Relationships: \n";
				
//		Person[] Relationships = person.Relationships;
				
//		for (int i = 0; i < Relationships.Length; i++) {
//			Person p = Relationships [i];
//			if (person != p) {
//				int myIntForP = 0;
//				for (int j = 0; j < p.Relationships.Length; j++) {
//					if (p.Relationships [j] == person) {
//						myIntForP = j;
//					}
//				}
//				PersonRelationshipsText.text += p.FirstName + ": " + person.RelationshipStrengths [i] + " / " + p.RelationshipStrengths [myIntForP] + "\n";
//			}
//		}
				
	}
}
