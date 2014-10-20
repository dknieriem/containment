﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class PersonInfoScript : MonoBehaviour
{

		Person person;
		
		Text PersonNameText;
		Text PersonLocationText;
		Text PersonStatsText;
		Text PersonRelationshipsText;

		void Start ()
		{
				Debug.Log ("Starting: PersonInfoScript");
				PersonNameText = GameObject.Find ("PersonName").GetComponent<Text> ();
				PersonLocationText = GameObject.Find ("PersonLocation").GetComponent<Text> ();
				PersonStatsText = GameObject.Find ("PersonStats").GetComponent<Text> ();
				PersonRelationshipsText = GameObject.Find ("PersonRelationships").GetComponent<Text> ();
				PlayerGroup group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
				//Debug.Log (group.name);
				//Debug.Log (group.GroupMembers.ToArray ().Length);
				UpdatePerson (group.GroupMembers.ToArray () [0]);
		}

		public void UpdatePerson (Person newPerson)
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
				PersonRoleText.text = "Role: " + person.RoleNames[(int) Person.CurrentRole];
				
				PersonStatsText.text = "Stats: \n";
				for (int i = 0; i < Enum.GetNames (typeof(Person.Stats)).Length; i++) {
						PersonStatsText.text += Person.StatNames [i] + person.CurrentStats [i] + "/" + person.BaseStats [i] + "\n";
				}
	
				PersonStatsText.text += "Zeds Killed: " + person.LifetimeZedKills;
					
				PersonRelationshipsText.text = "Relationships: \n";
				
				Person[] Relationships = person.Relationships;
				
				for (int i = 0; i < Relationships.Length; i++) {
						Person p = Relationships [i];
						if (person != p) {
								int myIntForP = 0;
								for (int j = 0; j < p.Relationships.Length; j++) {
										if (p.Relationships [j] == person) {
												myIntForP = j;
										}
								}
								PersonRelationshipsText.text += p.FirstName + ": " + person.RelationshipStrengths [i] + " / " + p.RelationshipStrengths [myIntForP] + "\n";
						}
				}
				
		}
}
