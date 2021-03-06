﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UITabbedWindow : MonoBehaviour
{

	public Button[] Tabs;
	public GameObject[] Panels;
	public Button previousTabButton;
	public Button nextTabButton;
	public UIWindowScript myWindow;
    public UIWindowScript previousWindow;
    public UIWindowScript nextWindow;
	public int currentTab = 0;
	public bool prevQuits = true;
	public bool nextQuits = false;
	public Text windowTitle;
	public string windowName;
	public string[] tabNames;

	void Start ()
	{
		Debug.Log ("UITabbedWindow.Start() for: " + gameObject.name);

		if (Tabs != null && Panels != null) {

			for (int i = 0; i < Tabs.Length; i++) {
				if (Panels.Length >= i && Panels [i] != null) {
					Tabs [i].onClick.RemoveAllListeners ();
					Tabs [i].onClick.AddListener (Panels [i].transform.SetAsLastSibling);
				}
			}
		}

		if (previousTabButton != null) {
			previousTabButton.onClick.RemoveAllListeners ();
			previousTabButton.onClick.AddListener (this.previousTab);
		}


		if (nextTabButton != null) {
			nextTabButton.onClick.RemoveAllListeners ();
			nextTabButton.onClick.AddListener (this.nextTab);
		}

		if (myWindow == null)
			myWindow = GetComponentInParent<UIWindowScript> ();

		updateWindowTitle ();
	}

	void previousTab ()
	{
		
		currentTab--;
		if (currentTab < 0) {
			currentTab = 0;
			if (prevQuits)
            {
                myWindow.ClosePanel();
            }
			
            if(previousWindow != null)
            {
                previousWindow.OpenPanel();
            }

		}
		Panels [currentTab].transform.SetAsLastSibling ();

		if (tabNames != null && tabNames [currentTab] != null)
			updateWindowTitle ();

	}


	void nextTab ()
	{
		
		currentTab++;
		if (currentTab >= Panels.Length) {
			currentTab = Panels.Length - 1;
			if (nextQuits)
            {
                myWindow.Complete();
            }
				
            if (nextWindow != null)
            {
                nextWindow.OpenPanel();
            }
        }

		Panels [currentTab].transform.SetAsLastSibling ();

		if (tabNames != null && tabNames [currentTab] != null)
			updateWindowTitle ();

	}

	void updateWindowTitle ()
	{
		windowTitle.text = windowName + ": " + tabNames [currentTab];
	}
}