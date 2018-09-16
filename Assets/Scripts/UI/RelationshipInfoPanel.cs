using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RelationshipInfoPanel : MonoBehaviour {

    //public Group group;

    Relationship relationship = null;

    public PersonButton RelationshipNameOneButton;
    public PersonButton RelationshipNameTwoButton;
    public Text RelationshipScoreText;
    public Text RelationshipModsText;

    void Start()
    {
       // RelationshipScoreText = transform.Find("Score").GetComponent<Text>();

    }

    private void Awake()
    {

    }

    public void ShowRelationship(Relationship newRelationship)
    {
        if (newRelationship == null)
        {
            Debug.Log("ShowRelationship... no relationship!");
        }
        else
        {
            SetRelationship(newRelationship);
        }
        gameObject.GetComponent<UIWindowScript>().OpenPanel();
        gameObject.GetComponent<UIWindowScript>().isDisposable = true;
    }

    void SetRelationship(Relationship newRelationship)
    {
        Debug.Log( string.Format("SetRelationship({0}<->{1} )",newRelationship.personOne.FirstName, newRelationship.personTwo.FirstName));

        if (relationship != null && relationship.Equals(newRelationship))
        {
            Debug.Log("Already equal");
            return;
        }

        relationship = newRelationship;

        FixedUpdate();

        Debug.Log("SetRelationship() New Relationship! ");
    }



    public Relationship getRelationship ()
    {
        return relationship;
    }

    void FixedUpdate()
    {
        if (relationship == null)
        {
            return;
        }

        SetRelationshipNameOneButton();
        SetRelationshipNameTwoButton();
        SetRelationshipScoreText();
        SetRelationshipModsText();
            
    }

    private void SetRelationshipModsText()
    {

        if (relationship.ModsChangedLastTick)
        {
            string modText = "";

            foreach (RelationshipModifier mod in relationship.modifiers)
            {
                modText += string.Format("{0}: {1}, {2}", mod.delta, mod.explanation, mod.expiration) + System.Environment.NewLine;
            }

            RelationshipModsText.text = modText;
        }

    }

    private void SetRelationshipScoreText()
    {
        if (relationship.ScoreChangedLastTick)
        {
            RelationshipScoreText.text = string.Format("{0:0.0#}/{1:0.0#}", relationship.CurrentScore, relationship.BaseScore);
        }
        
    }

    private void SetRelationshipNameTwoButton()
    {
        if (relationship.personTwo.infoChangedLastTick)
        {
            RelationshipNameTwoButton.SetPerson(relationship.personTwo);
        }
        
    }

    private void SetRelationshipNameOneButton()
    {
        if (relationship.personTwo.infoChangedLastTick)
        {
            RelationshipNameOneButton.SetPerson(relationship.personOne);
        }
            
    }
}
