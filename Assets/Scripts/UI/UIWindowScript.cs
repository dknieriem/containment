using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIWindowScript : MonoBehaviour, IDragHandler
{
	RectTransform m_transform = null;
	public Button CloseButton;
	public GameObject windowObject;
	public bool isEnabled;
	public bool isDraggable;
	// Use this for initialization
	void Start ()
	{
		Debug.Log ("UIWindow.Start() for: " + windowObject.name);
		m_transform = GetComponent<RectTransform> ();
		if (CloseButton == null)
			return;

		CloseButton.onClick.RemoveAllListeners ();
		CloseButton.onClick.AddListener (this.ClosePanel);
	}

	public void OnDrag (PointerEventData eventData)
	{
		if (!isDraggable)
			return;

		m_transform.position += new Vector3 (eventData.delta.x, eventData.delta.y);
		m_transform.transform.SetAsLastSibling (); //put on top of windows

		// magic : add zone clamping if's here.
	}

	public void ClosePanel ()
	{
		Debug.Log ("ClosePanel() for: " + windowObject.name);
		isEnabled = false;
		windowObject.transform.localScale = new Vector3 (0, 0, 0);
		//windowObject.SetActive (false);
	}

	public void OpenPanel ()
	{
		isEnabled = true;
		windowObject.transform.localScale = new Vector3 (1, 1, 1);
		//windowObject.SetActive (true);
	}

	public void TogglePanel ()
	{
		isEnabled = !isEnabled;

		if (isEnabled)
			windowObject.transform.localScale = new Vector3 (1, 1, 1);
		else
			windowObject.transform.localScale = new Vector3 (0, 0, 0);

		//windowObject.SetActive (!windowObject.activeSelf);
	}
}