using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RelationshipButton : MonoBehaviour {
    
    public Relationship relationship;
    public Button myButton;
    UIHandler UIHandlerInstance;

    void Start()
    {
        UIHandlerInstance = UIHandler.Instance();
        myButton = gameObject.GetComponent<Button>();
        if (myButton == null)
        {
            Debug.Log("Button null...");
            return;
        }

        if (relationship == null)
        {
            Debug.Log("No Relationship to link...");
            return;
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(delegate {
            //PersonInfoPanel newPersonPanel = 
            UIHandlerInstance.AddRelationshipInfoPanel(relationship);
            //newPersonPanel.ShowPerson (person);
        });

    }

    void Awake()
    {

        myButton = gameObject.GetComponent<Button>();

    }

    public void SetRelationship(Relationship newRelationship, Person person)
    {
        relationship = newRelationship;
        if (myButton == null)
        {
            Debug.Log("Button still null.");
            return;
        }

        Debug.Log("SetRelationship");
        Text buttonText = myButton.gameObject.GetComponentInChildren<Text>();

        Person other;
        if (person.Equals(relationship.personOne))
        {
            other = relationship.personTwo;
        }
        else if (person.Equals(relationship.personTwo))
        {
            other = relationship.personOne;
        }
        else
        {
            Debug.Log(string.Format("{0} does not seem to be in the relationship between {1} and {2}! ", person.Id, relationship.personOne.Id, relationship.personTwo.Id));
            return;
        }

        if (buttonText != null)
        {
            buttonText.text = string.Format("-> {0} {1}: {2:0.0#}/{3:0.0#}",other.FirstName, other.LastName, relationship.CurrentScore, relationship.BaseScore);
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(delegate {
            UIHandlerInstance.AddRelationshipInfoPanel(relationship);
            //personPanel.ShowPerson (person);
        });
    }
}