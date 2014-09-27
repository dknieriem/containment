using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GroupInfoScript : MonoBehaviour
{
		PlayerGroup Group;
		Text HomeSectorLocationText;
		Text GroupMembersText;
		
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: GroupInfoScript");
				Group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
				HomeSectorLocationText = GameObject.Find ("PlayerGroupHomeLocation").GetComponent<Text> ();
				GroupMembersText = GameObject.Find ("PlayerGroupTotalCount").GetComponent<Text> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		
		void FixedUpdate ()
		{
				HomeSectorLocationText.text = "(" + Group.HomeSectorLocation.x + "," + Group.HomeSectorLocation.y + ")";
				GroupMembersText.text = "" + Group.TotalGroupMembers; 		
		}
}
