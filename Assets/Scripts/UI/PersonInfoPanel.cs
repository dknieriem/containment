using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PersonInfoPanel : MonoBehaviour
{
	//public Group group;
		
	Person person = null;
		
	public Text PersonNameText;
	public Text PersonLocationText;
	public Text PersonStatsText;
	public Text PersonRelationshipsText;

    public GameObject RelationshipList;
    public GameObject RelationshipButtonPrefab;

	//Text PersonRoleText;

	//private static PersonInfoPanel panel;

	//public static PersonInfoPanel Instance ()
	//{
	//	if (!panel) {
	//		panel = FindObjectOfType (typeof(PersonInfoPanel)) as PersonInfoPanel;
	//		if (!panel)
	//			Debug.LogError ("There needs to be one active PersonInfoPanel script on a GameObject in your scene.");
	//	}
        
	//	return panel;
	//}

	void Start ()
	{

        //Debug.Log ("PersonInfoScript.Start()");

        //PersonRoleText = GameObject.Find ("PersonRole").GetComponent<Text> ();

        //Debug.Log (group.name);
        //Debug.Log (group.GroupMembers.ToArray ().Length);
        //Debug.Log ("Group: " + group.ToString ());
        //group = GameManager.Instance.world.PlayerGroup;
        //Debug.Log ("Group: " + group.ToString ());
        //UpdatePerson (group.GroupMembers.ToArray () [0]);

        RelationshipList = GameObject.Find("RelationshipList");
    }

    private void Awake()
    {

    }

    public void ShowPerson (Person newPerson)
	{
		if (newPerson == null) {
			Debug.Log ("ShowPerson... no person!");
		} else {
			SetPerson (newPerson);
		}
        gameObject.GetComponent<UIWindowScript>().OpenPanel();
        gameObject.GetComponent<UIWindowScript>().isDisposable = true;
    }

	void SetPerson (Person newPerson)
	{
        Debug.Log("SetPerson( " + newPerson.FirstName + " )");

        //PersonNameText = transform.Find("PersonName").GetComponent<Text>();
        //PersonLocationText = transform.Find("PersonLocation").GetComponent<Text>();
        //PersonStatsText = transform.Find("PersonStats").GetComponent<Text>();
        //PersonRelationshipsText = transform.Find("PersonRelationships").GetComponent<Text>();

        
        if (person != null && person.Id == newPerson.Id) {
            Debug.Log("Already equal");
			return;
		}

		person = newPerson;

        FixedUpdate();

		Debug.Log ("New Person! " + PersonNameText.text);
	}

    private void setStatsText()
    {
        string newText = "";

        for(int i = 0; i < Person.StatNames.Length; i++)
        {
            string statName = Person.StatNames[i];
            float baseStat = person.BaseStats[i];
            float currentStat = person.CurrentStats[i];
            newText += string.Format("{0}: {1}/{2}",statName,currentStat,baseStat) + System.Environment.NewLine;
        }

        PersonStatsText.text = newText;
    }

    private void setRelationshipsButtons()
    {
        for (int i = 0; i < RelationshipList.transform.childCount; i++)
        {
            Destroy(RelationshipList.transform.GetChild(0).gameObject);
        }

        //int memberCount = Group.GroupMembers.Count;
        foreach (Relationship relationship in person.Relationships)
        {
            //for (int i = 0; i < memberCount; i++) {
            Person personOne = relationship.personOne;
            Person personTwo = relationship.personTwo;
            Person other;
            if (person.Equals(personOne))
            {
                other = personTwo;
            }
            else if (person.Equals(personTwo))
            {
                other = personOne;
            }
            else
            {
                Debug.Log(string.Format("{0} does not seem to be in the relationship between {1} and {2}! ", person.Id, relationship.personOne.Id, relationship.personTwo.Id));
                return;
            }

            //newText += string.Format("{0}: {1}/{2}", other.FirstName, relationship.CurrentScore, relationship.BaseScore) + System.Environment.NewLine;
        

        //PersonRelationshipsText.text = newText;
        Debug.Log("Create button for: " + other.FirstName + " " + other.LastName);

            GameObject newButton = Instantiate(RelationshipButtonPrefab) as GameObject;
            newButton.name = "Relationship " + other.FirstName + " Button";
            newButton.transform.SetParent(RelationshipList.transform);
            newButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            RelationshipButton button = newButton.GetComponent<RelationshipButton>();
            button.SetRelationship(relationship, person);//(Group.GroupMembers [i]);

        }

        //GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
        //HomeSectorLocationText.text = "(" + Group.HomeSectorLocationX + "," + Group.HomeSectorLocationY + ")";
        //GroupMembersText.text = "Members: " + Group.TotalGroupMembers;

        //string newText = "";

        //Debug.Log(string.Format("{0} has {1} relationships", person.FirstName, person.Relationships.Count));

  
    }

    private void setRelationshipsText()
    {
        string newText = "";

        Debug.Log(string.Format("{0} has {1} relationships", person.FirstName, person.Relationships.Count));

        foreach(Relationship relationship in person.Relationships)
        {

            Person personOne = relationship.personOne;
            Person personTwo = relationship.personTwo;
            Person other;
            if (person.Equals(personOne)){
                other = personTwo;
            } else if (person.Equals(personTwo))
            {
                other = personOne;
            } else
            {
                Debug.Log(string.Format("{0} does not seem to be in the relationship between {1} and {2}! ", person.Id, relationship.personOne.Id, relationship.personTwo.Id));
                return;            
            }

            newText += string.Format("{0}: {1:0.0#}/{2:0.0#}", other.FirstName, relationship.CurrentScore, relationship.BaseScore) + System.Environment.NewLine;
        }

        PersonRelationshipsText.text = newText;
    }

    public Person getPerson()
    {
        return person;
    }

    void FixedUpdate ()
	{
		if (person == null) {
			return;
		}

        PersonNameText.text = string.Format("{0} {1}", person.FirstName, person.LastName);
        PersonLocationText.text = string.Format("({0}, {1})", person.LocationX, person.LocationY);

        setStatsText();
        setRelationshipsButtons();

    }
}
