using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PersonButton : MonoBehaviour
{

	public Person person;
	public Button myButton;
    UIHandler UIHandlerInstance;

	void Start ()
	{
        UIHandlerInstance = UIHandler.Instance();
		myButton = gameObject.GetComponent<Button> ();
		if (myButton == null) {
			Debug.Log ("Button null...");
			return;
		}

		if (person == null) {
			Debug.Log ("No Person to link...");
			return;
		}

		myButton.onClick.RemoveAllListeners ();
		myButton.onClick.AddListener (delegate {
            //PersonInfoPanel newPersonPanel = 
            UIHandlerInstance.AddPersonInfoPanel(person);
            //newPersonPanel.ShowPerson (person);
		});
		
	}

	void Awake ()
	{
		myButton = gameObject.GetComponent<Button> ();
	}

	public void SetPerson (Person newPerson)
	{
		person = newPerson;
		if (myButton == null) {
			Debug.Log ("Button still null.");
			return;
		}

        Debug.Log("SetPerson to " + person.FirstName);
		Text buttonText = myButton.gameObject.GetComponentInChildren<Text> ();
		if (buttonText != null) {
			buttonText.text = person.FirstName + " " + person.LastName + "\t(" + person.LocationX + "," + person.LocationY + ")";
		}

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(delegate {
            UIHandlerInstance.AddPersonInfoPanel(person);
            //personPanel.ShowPerson (person);
        });
    }
}
