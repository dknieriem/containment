using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

public class PropertySet : MonoBehaviour
{

	protected Dictionary<String,Property> properties;

	public Transform InputParent;
	public GameObject textPrefab, boolPrefab, enumPrefab, sliderPrefab;

	void Start ()
	{
		//Debug.Log ("PropertySet.Start()");
	}

	protected void CreatePropertyObjects ()
	{

		//	while (InputParent.childCount > 0) {
		//		GameObject.Destroy (InputParent.GetChild (0));
		//	}

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
			Transform labelGameObj;
			Text labelObject; 
			//LayoutElement myLayout = newField.AddComponent<LayoutElement> ();
			switch (typeName) {
			case "System.Boolean":
				newField = Instantiate (boolPrefab);
				Toggle newToggle = newField.GetComponent<Toggle> ();
				newToggle.isOn = (bool)currentProp.GetValue ();
				newToggle.onValueChanged.RemoveAllListeners ();
				newToggle.onValueChanged.AddListener (delegate {
                    this.SetValue(localCopy, newToggle.isOn);
                    return;
				});
				break;

			case "System.String":
				newField = Instantiate (textPrefab);
				InputField newInput = newField.GetComponent<InputField> ();
				newInput.text = currentProp.GetValue ().ToString ();
				newInput.onEndEdit.RemoveAllListeners ();
				newInput.onEndEdit.AddListener (delegate {
					this.SetValue (localCopy, newInput.text);
                    return;

                });
				break;
			case "System.Enum":
				EnumProperty enumProp = currentProp as EnumProperty;
				newField = Instantiate (enumPrefab);
				Dropdown dropdown = newField.GetComponent<Dropdown> ();
				dropdown.value = (int)currentProp.GetValue ();
				dropdown.ClearOptions ();
				//for (int i = enumProp.min; i <= enumProp.max; i++) {
				List<string> options = new List<string> (enumProp.possibleValues);

				dropdown.AddOptions (options);
				dropdown.onValueChanged.RemoveAllListeners ();
				dropdown.onValueChanged.AddListener (delegate {
					this.SetValue (localCopy, dropdown.value);
                    return;

                });
				//}
				break;
            case "System.Int32":
                    IntegerProperty intProp = currentProp as IntegerProperty;
                    newField = Instantiate(sliderPrefab);
                    Slider newSlider = newField.GetComponent<Slider>();
                    newSlider.maxValue = intProp.maxValue;
                    newSlider.minValue = intProp.minValue;
                    newSlider.direction = Slider.Direction.LeftToRight;
                    newSlider.value = (int) currentProp.GetValue();

                    Text valueObj = newSlider.transform.Find("Text").GetComponent<Text>();
                    valueObj.text = ((int)newSlider.value).ToString();

                    newSlider.wholeNumbers = true;
                    newSlider.onValueChanged.RemoveAllListeners();
                    newSlider.onValueChanged.AddListener(delegate {
                       Text valObj = newSlider.transform.Find("Text").GetComponent<Text>();
                        valObj.text = ((int) newSlider.value).ToString();

                        this.SetValue(localCopy, (int) newSlider.value);
                        //Debug.Log(this.name + " set to " + this.GetIntValue());
                        return;

                    });
                    break;
                default:
				newField = new GameObject ();
				break;
			}
			labelGameObj = newField.transform.Find ("Property Label"); 
			if (labelGameObj != null) {
				labelObject = labelGameObj.GetComponent<Text> ();
				labelObject.text = currentName;
			}
			newField.name = currentName + " Field";
			newField.transform.SetParent (InputParent);
            newField.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		}
	}

	void SetValue (Property field, System.Object newValue)
	{
		field.SetValue (newValue);

	}

	public string GetStringValue (string propertyName)
	{
		Property received;
		bool found = properties.TryGetValue (propertyName, out received);
		if (!found)
			return "";

		string type = received.GetPropTypeString ();

		if (type == "System.String")
			return received.GetValue () as string;
		else
			return "";

	}

	public int GetIntValue (string propertyName)
	{
		Property received;
		bool found = properties.TryGetValue (propertyName, out received);
		if (!found)
			return -1;

		string type = received.GetPropTypeString ();

		if (type == "System.Int32")
			return (int)received.GetValue ();
		else
			return -1;

	}

	public int GetEnumValue (string propertyName)
	{
		Property received;
		bool found = properties.TryGetValue (propertyName, out received);
		if (!found)
			return -1;

		string type = received.GetPropTypeString ();

		if (type == "System.Enum")
			return (int)received.GetValue ();
		else
			return -1;

	}
}