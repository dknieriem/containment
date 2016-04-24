using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

public class PropertySet : MonoBehaviour
{

	Dictionary<String,Property> properties;

	public Transform InputParent;
	public GameObject textPrefab, boolPrefab, enumPrefab;

	//Old
	//	//Character Info
	//
	//	public string FirstName, LastName;
	//
	//
	//	//Group Info
	//
	//	//Government GovtType;
	//	public int GovtType;
	//
	//	//-1 defensive, 0 neutral, 1 aggressive
	//	public int defaultStance;
	//
	//	public bool AllLifeIsSacred;
	//	public bool Cannibal;
	//	public bool ZombiesArePeopleToo;
	//
	//	//eliminate Zeds, Repopulate the Earth, New World Order, Live and Let Live, Retreat from the World
	//	public int GroupMantra;
	//
	//
	//	//World Info
	//	public string WorldName;
	//	//0,S=20x20 1,M = 40x40 2,L=64x64 3,XL=128x128
	//	public int WorldSize;
	//
	//	//Rural, Suburban, Urban
	//	public int WorldType;

	void Start ()
	{
		properties = new Dictionary<string, Property> ();

		properties.Add ("First Name", new StringProperty ("First Name", "first") as Property);
		properties.Add ("Last Name", new StringProperty ("Last Name", "last") as Property);
		properties.Add ("Gov't Type", new EnumProperty (new string[] { "Council", "Dictatorship", "Fiefdom" }, 0) as Property);

		//Type type = this.GetType ();
		//FieldInfo[] members = type.GetFields ();

		Dictionary<string,Property>.Enumerator PropEnum = properties.GetEnumerator ();
		Debug.Log (properties.Count);
		//for (int i = 0; i < properties.Count; i++) {
		while (PropEnum.MoveNext ()) {
			KeyValuePair<string,Property> currentPair = PropEnum.Current;
			Property currentProp = currentPair.Value;
			string currentName = currentPair.Key;

			string typeName = currentProp.GetPropTypeString ();//members[i].ToString ();
			Debug.Log ("\"" + typeName + "\" " + currentName + ": " + currentProp.GetValue ().ToString ());
			Property localCopy = currentProp;

			GameObject newField;// = new GameObject ();
			Text labelObject; 
			//LayoutElement myLayout = newField.AddComponent<LayoutElement> ();
			switch (typeName) {
			case "System.Boolean":
				newField = Instantiate (boolPrefab);
				Toggle newToggle = newField.GetComponent<Toggle> ();
				newToggle.onValueChanged.RemoveAllListeners ();
				newToggle.onValueChanged.AddListener (delegate {
					return this.setValue (localCopy, newToggle.isOn);
				});
				labelObject = newField.transform.Find ("Label").GetComponent<Text> ();
				labelObject.text = currentName;
				break;

			case "System.String":
				newField = Instantiate (textPrefab);
				InputField newInput = newField.GetComponent<InputField> ();
				newInput.onEndEdit.RemoveAllListeners ();
				newInput.onEndEdit.AddListener (delegate {
					return this.setValue (localCopy, newInput.text);
				});
				labelObject = newField.transform.Find ("Label").GetComponent<Text> ();
				labelObject.text = currentName;
				break;
			case "System.Enum":
				EnumProperty enumProp = currentProp as EnumProperty;
				newField = Instantiate (enumPrefab);
				Dropdown dropdown = newField.GetComponent<Dropdown> ();
				dropdown.ClearOptions ();
				//for (int i = enumProp.min; i <= enumProp.max; i++) {
				List<string> options = new List<string> (enumProp.possibleValues);

				dropdown.AddOptions (options);
				dropdown.onValueChanged.RemoveAllListeners ();
				dropdown.onValueChanged.AddListener (delegate {
					return this.setValue (localCopy, dropdown.value);
				});
				//}
				break;
			default:
				newField = new GameObject ();
				break;
			}

			newField.name = currentName + " Field";
			newField.transform.parent = InputParent;


		}
	}

	void OldStart ()
	{
		

		Type type = this.GetType ();
		FieldInfo[] members = type.GetFields ();

		for (int i = 0; i < members.Length; i++) {
			string typeName = members [i].FieldType.ToString ();//members[i].ToString ();
			Debug.Log ("\"" + typeName + "\" " + members [i].Name + ": " + members [i].GetValue (this));
			FieldInfo localCopy = members [i];

			GameObject newField;// = new GameObject ();
			Text labelObject; 
			//LayoutElement myLayout = newField.AddComponent<LayoutElement> ();
			switch (typeName) {
			case "System.Boolean":
				newField = Instantiate (boolPrefab);
				Toggle newToggle = newField.GetComponent<Toggle> ();
				newToggle.onValueChanged.RemoveAllListeners ();
				newToggle.onValueChanged.AddListener (delegate {
					return this.setValue (localCopy, newToggle.isOn);
				});
				labelObject = newField.transform.Find ("Label").GetComponent<Text> ();
				labelObject.text = members [i].Name;
				break;

			case "System.String":
				newField = Instantiate (textPrefab);
				InputField newInput = newField.GetComponent<InputField> ();
				newInput.onEndEdit.RemoveAllListeners ();
				newInput.onEndEdit.AddListener (delegate {
					return this.setValue (localCopy, newInput.text);
				});
				labelObject = newField.transform.Find ("Property Label").GetComponent<Text> ();
				labelObject.text = members [i].Name;
				break;
			default:
				newField = new GameObject ();
				break;
			}

			newField.name = members [i].Name + " Field";
			newField.transform.parent = InputParent;

		}
	}

	void setValue (FieldInfo field, System.Object newValue)
	{
		field.SetValue (this, newValue);

	}

	void setValue (Property field, System.Object newValue)
	{
		field.SetValue (newValue);

	}
}