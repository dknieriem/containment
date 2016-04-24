using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PersonButton : MonoBehaviour
{

	public Person person;
	public Button myButton;
	PersonInfoPanel personPanel;

	void Start ()
	{
		personPanel = PersonInfoPanel.Instance ();
		myButton = gameObject.GetComponent<Button> ();
		if (myButton == null) {
			Debug.Log ("Button null...");
			return;
		}

		if (person == null) {
			Debug.Log ("No Person to link...");
			//return;
		}

		myButton.onClick.RemoveAllListeners ();
		myButton.onClick.AddListener (delegate {
			personPanel.ShowPerson (person);
		});
		
	}

	void Awake ()
	{
		personPanel = PersonInfoPanel.Instance ();
		myButton = gameObject.GetComponent<Button> ();
		if (myButton == null) {
			Debug.Log ("Button null...");
			return;
		}

		if (person == null) {
			Debug.Log ("No Person to link...");
			//return;
		}

		myButton.onClick.RemoveAllListeners ();
		myButton.onClick.AddListener (delegate {
			personPanel.ShowPerson (person);
		});
		
	}

	public void SetPerson (Person newPerson)
	{
		person = newPerson;
		if (myButton == null) {
			Debug.Log ("Button still null.");
			return;
		}

		Text buttonText = myButton.gameObject.GetComponentInChildren<Text> ();
		if (buttonText != null) {
			buttonText.text = person.FirstName + " " + person.LastName + "\t(" + person.LocationX + "," + person.LocationY + ")";
		}
	}
}
