using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GroupInfoScript : MonoBehaviour
{

		public Font DefaultFont;
		public int DefaultFontSize = 8;
		PlayerGroup Group;
		Text HomeSectorLocationText;
		Text GroupMembersText;
		Text[] GroupMemberNamesText;
		GameObject GroupMemberList;
		PersonInfoScript PersonScript;
		
		// Use this for initialization
		void Start ()
		{
				Debug.Log ("Starting: GroupInfoScript");
				Group = GameObject.Find ("PlayerGroup").GetComponent<PlayerGroup> ();
				HomeSectorLocationText = GameObject.Find ("PlayerGroupHomeLocation").GetComponent<Text> ();
				GroupMemberList = GameObject.Find ("GroupMemberList");
				GroupMembersText = GameObject.Find ("PlayerGroupTotalCount").GetComponent<Text> ();
				GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
				PersonScript = GameObject.Find ("PersonInfoPanel").GetComponent<PersonInfoScript> ();
		}
	
		void FixedUpdate ()
		{
				if (Group.GroupMembers.Count != GroupMemberNamesText.Length) {
						UpdateGroupList ();
				}
		
				HomeSectorLocationText.text = "(" + Group.HomeSectorLocation [0] + "," + Group.HomeSectorLocation [1] + ")";
				GroupMembersText.text = "Members: " + Group.TotalGroupMembers; 		

				for (int i = 0; i < Group.GroupMembers.Count; i++) {
						Person p = Group.GroupMembers [i];
						GroupMemberNamesText [i].text = p.FirstName + " " + p.LastName + "\t(" + p.LocationX + "," + p.LocationY + ")\n";
				}
		}
		
		void UpdateGroupList ()
		{
		
				for (int i = 0; i < GroupMemberList.transform.childCount; i++) {
						Destroy (GroupMemberList.transform.GetChild (0).gameObject);
				}
				
				int memberCount = Group.GroupMembers.Count;
				
				for (int i = 0; i < memberCount; i++) {
						GameObject newButton = new GameObject ();
						newButton.transform.SetParent (GroupMemberList.transform);
						
						RectTransform buttTransform = newButton.AddComponent<RectTransform> ();
						Button button = newButton.AddComponent<Button> ();
						buttTransform.sizeDelta = new Vector2 (200.0f, 16.0f);
						buttTransform.anchoredPosition = new Vector2 (50.0f, -16.0f * i + 45.0f);
						
						int p = i;
						button.onClick.AddListener (() => {
								PersonScript.UpdatePerson (Group.GroupMembers [p]); 
						});
						
						GameObject newText = new GameObject ();
						newText.transform.SetParent (newButton.transform);
						
						newText.AddComponent<Text> ();
						Text t = newText.GetComponent<Text> ();
						t.text = Group.GroupMembers [i].FirstName + " " + Group.GroupMembers [i].LastName;
						t.font = DefaultFont;
						t.fontSize = DefaultFontSize;
						t.rectTransform.sizeDelta = new Vector2 (200.0f, 16f);
						
				}
		
				GroupMemberNamesText = GroupMemberList.GetComponentsInChildren<Text> ();
		}
}
