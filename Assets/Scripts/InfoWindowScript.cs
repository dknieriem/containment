using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoWindowScript : MonoBehavior
{

public Text[] TextElements;

//TODO: abstractify the infoscript classes to pull all child components of type ui.text, then have a method to implement called "update text"

void Start()
{
TextElements = gameObject.GetComponents<Text>();
}

void Update()
{
//update each text element somehow?
}


void GetMessage(string ElementName, string newText)
{

}

}
