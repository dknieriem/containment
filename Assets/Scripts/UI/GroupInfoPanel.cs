using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GroupInfoPanel : MonoBehaviour
{

	public Group Group;
	Text HomeSectorLocationText;
	Text GroupMembersText;
	//Text[] GroupMemberNamesText;
	GameObject GroupMemberList;
	//public PersonInfoPanel personInfoPanel;
	public GameObject PersonButtonPrefab;
    //UIHandler uiHandler;
	// Use this for initialization
	void Start ()
	{
        //uiHandler = UIHandler.Instance();
		Debug.Log ("GroupInfoPanel.Start()");
		//Group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
		HomeSectorLocationText = GameObject.Find ("PlayerGroupHomeLocation").GetComponent<Text> ();
		GroupMemberList = GameObject.Find ("GroupMemberList");
		GroupMembersText = GameObject.Find ("PlayerGroupTotalCount").GetComponent<Text> ();
		//GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
		//personInfoPanel = UIHandler.Instance ().personInfoPanel.GetComponent<PersonInfoPanel> ();
	}

	void FixedUpdate ()
	{
		
		if (Group.groupInfoDirty) {
			UpdateGroupList ();
			Group.groupInfoDirty = false;
		}
		

		//for (int i = 0; i < Group.GroupMembers.Count; i++) {
		//	Person p = Group.GroupMembers [i];
		//	GroupMemberNamesText [i].text = p.FirstName + " " + p.LastName + "\t(" + p.LocationX + "," + p.LocationY + ")\n";
		//}
	}

    public void ManualUpdate()
    {
        FixedUpdate();
    }
	void UpdateGroupList ()
	{
		
		for (int i = 0; i < GroupMemberList.transform.childCount; i++) {
			Destroy (GroupMemberList.transform.GetChild (0).gameObject);
		}
				
		//int memberCount = Group.GroupMembers.Count;
		foreach (Person member in Group.GroupMembers) {
            //for (int i = 0; i < memberCount; i++) {
            Debug.Log("Create button for: " + member.FirstName + " " + member.LastName);
              
			GameObject newButton = Instantiate (PersonButtonPrefab) as GameObject;
			newButton.name = "PlayerGroupMember " + member.FirstName + " Button";
			newButton.transform.SetParent (GroupMemberList.transform);
            newButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			PersonButton button = newButton.GetComponent<PersonButton> ();
			button.SetPerson (member);//(Group.GroupMembers [i]);
						
		}
		
		//GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
		HomeSectorLocationText.text = "(" + Group.HomeSector.LocationX + "," + Group.HomeSector.LocationY + ")";
		GroupMembersText.text = "Members: " + Group.GroupMembers.Count; 		

	}
}
