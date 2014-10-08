using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GroupInfoScript : MonoBehaviour
{
		PlayerGroup Group;
		Text HomeSectorLocationText;
		Text GroupMembersText;
		Text GroupMemberNamesText;
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: GroupInfoScript");
				Group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
				HomeSectorLocationText = GameObject.Find ("PlayerGroupHomeLocation").GetComponent<Text> ();
				GroupMembersText = GameObject.Find ("PlayerGroupTotalCount").GetComponent<Text> ();
				GroupMemberNamesText = GameObject.Find ("PlayerGroupMemberNames").GetComponent<Text> ();
		}
	
		void FixedUpdate ()
		{
				HomeSectorLocationText.text = "(" + Group.HomeSectorLocation [0] + "," + Group.HomeSectorLocation [1] + ")";
				GroupMembersText.text = "Members: " + Group.TotalGroupMembers; 		
				GroupMemberNamesText.text = "";
				foreach (PlayerGroup.Person p in Group.GroupMembers) {
						int[] location = {p.LocationX, p.LocationY};
						GroupMemberNamesText.text += p.FirstName + " " + p.LastName + "\t(" + location [0] + "," + location [1] + ")\n";
				
				}
		}
}
