using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIWindowScript : MonoBehaviour, IDragHandler
{
	protected RectTransform m_transform = null;
	public Button CloseButton;
	public GameObject windowObject;
	public bool isEnabled;
	public bool isDraggable;
    public bool isDisposable;

	// Use this for initialization
	protected void Start ()
	{
		Debug.Log ("UIWindow.Start() for: " + windowObject.name);
		m_transform = GetComponent<RectTransform> ();
        m_transform.anchorMax = new Vector2(0.5f, 0.5f);
        m_transform.anchorMin = new Vector2(0.5f, 0.5f);
        m_transform.anchoredPosition = new Vector2(0.0f, 0.0f);



        if (CloseButton == null)
			return;

		CloseButton.onClick.RemoveAllListeners ();
		CloseButton.onClick.AddListener (this.ClosePanel);

        if (!this.isEnabled)
            this.ClosePanel();
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

    public void RemovePanel()
    {
        Debug.Log("RemovePanel() for: " + windowObject.name);
        Destroy(windowObject);
    }

	public void OpenPanel ()
	{
        Debug.Log("OpenPanel() for: " + windowObject.name);
        isEnabled = true;
        windowObject.transform.SetAsLastSibling();
        windowObject.transform.localScale = new Vector3 (1, 1, 1);
		//windowObject.SetActive (true);
	}

	public void TogglePanel ()
	{

        Debug.Log("TogglePanel() for: " + windowObject.name);
        isEnabled = !isEnabled;

		if (isEnabled)
			windowObject.transform.localScale = new Vector3 (1, 1, 1);
		else
			windowObject.transform.localScale = new Vector3 (0, 0, 0);

		//windowObject.SetActive (!windowObject.activeSelf);
	}

	public virtual void Complete () //override and close
	{
		ClosePanel ();
	}
}