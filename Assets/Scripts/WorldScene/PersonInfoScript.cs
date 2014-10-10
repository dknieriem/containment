using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PersonInfoScript : MonoBehaviour
{

		Person person;
		Text PersonNameText;
		Text PersonLocationText;
		//Text GroupMemberNamesText;

		void Start ()
		{
				Debug.Log ("Starting: PersonInfoScript");
				PersonNameText = GameObject.Find ("PersonName").GetComponent<Text> ();
				PersonLocationText = GameObject.Find ("PersonLocation").GetComponent<Text> ();
				
				PlayerGroup group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
				Debug.Log (group.name);
				Debug.Log (group.GroupMembers.ToArray ().Length);
				UpdatePerson (group.GroupMembers.ToArray () [0]);
		}

		public void UpdatePerson (Person newPerson)
		{
				if (person == newPerson) {
						return;
				}
				person = newPerson;
				PersonNameText.text = string.Format ("{0} {1}", person.FirstName, person.LastName);
				Debug.Log ("New Person! " + newPerson.BaseAttackStrength);
		}

		void FixedUpdate ()
		{
				if (person == null) {
						return;
				}
				
				PersonLocationText.text = "(" + person.LocationX + "," + person.LocationY + ")";			
		}
}
