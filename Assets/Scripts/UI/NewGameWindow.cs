using UnityEngine;
using System.Collections;

public class NewGameWindow : UIWindowScript
{
	public CharacterPropertySet charPropertySet;
	public GroupPropertySet groupPropertySet;
	public WorldPropertySet worldPropertySet;

    //void Start ()
    //{
    //	Debug.Log ("UIWindow.Start() for: " + windowObject.name);
    //	m_transform = GetComponent<RectTransform> ();
    //	if (CloseButton == null)
    //		return;

    //	CloseButton.onClick.RemoveAllListeners ();
    //	CloseButton.onClick.AddListener (this.ClosePanel);
    //}

    new void Start()
    {
        base.Start();
    }

    public override void Complete () //override and close
	{
		ClosePanel ();
		GameManager.Instance.NewGame (charPropertySet, groupPropertySet, worldPropertySet);
	}
}
