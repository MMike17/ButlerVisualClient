using System.Collections.Generic;
using UnityEngine;
using static Request;

/// <summary>Class managing popups</summary>
public class PopupManager : MonoBehaviour
{
	[Header("Assign in Inspector")]
	public List<Popup> popups;

	[Header("Test")]
	public bool shouldTest;
	public RequestType testPopup;

	RequestType currentType;

	void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if(shouldTest && currentType != testPopup)
			Pop(testPopup, "test");
#endif
	}

	void Awake()
	{
		shouldTest = false;

		currentType = RequestType.NONE;
	}

	/// <summary>Pops provided popup</summary>
	public void Pop(RequestType popup, string message = "")
	{
		if(popup == currentType)
			return;

		Popup targetPopup = popups.Find(item => { return item.type == popup; });

		if(targetPopup == null && popup != RequestType.NONE)
		{
			Debug.LogError("Couldn't find popup for type : " + popup);
			return;
		}

		popups.ForEach(item => item.Pop(false, "", () => Pop(RequestType.NONE)));

		if(popup != RequestType.NONE)
			targetPopup.Pop(true, message, () => Pop(RequestType.NONE));

		currentType = popup;
	}
}