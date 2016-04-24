using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GroupInfoPanel : MonoBehaviour
{

	public Faction Group;
	Text HomeSectorLocationText;
	Text GroupMembersText;
	//Text[] GroupMemberNamesText;
	GameObject GroupMemberList;
	public PersonInfoPanel personInfoPanel;
	public GameObject PersonButtonPrefab;

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("GroupInfoPanel.Start()");
		//Group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
		HomeSectorLocationText = GameObject.Find ("PlayerGroupHomeLocation").GetComponent<Text> ();
		GroupMemberList = GameObject.Find ("GroupMemberList");
		GroupMembersText = GameObject.Find ("PlayerGroupTotalCount").GetComponent<Text> ();
		//GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
		personInfoPanel = UIHandler.Instance ().personInfoPanel.GetComponent<PersonInfoPanel> ();
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

	void UpdateGroupList ()
	{
		
		for (int i = 0; i < GroupMemberList.transform.childCount; i++) {
			Destroy (GroupMemberList.transform.GetChild (0).gameObject);
		}
				
		//int memberCount = Group.GroupMembers.Count;
		foreach (Person member in Group.GroupMembers) {
			//for (int i = 0; i < memberCount; i++) {
			GameObject newButton = Instantiate (PersonButtonPrefab) as GameObject;
			newButton.name = "PlayerGroupMember " + member.FirstName + " Button";
			newButton.transform.SetParent (GroupMemberList.transform);
						
			PersonButton button = newButton.GetComponent<PersonButton> ();
			button.SetPerson (member);//(Group.GroupMembers [i]);
						
		}
		
		//GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
		HomeSectorLocationText.text = "(" + Group.HomeSectorLocationX + "," + Group.HomeSectorLocationY + ")";
		GroupMembersText.text = "Members: " + Group.TotalGroupMembers; 		

	}
}
