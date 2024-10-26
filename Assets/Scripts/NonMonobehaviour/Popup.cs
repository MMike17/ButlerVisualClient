using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Request;

/// <summary>In app popup</summary>
[Serializable]
public class Popup
{
	public RequestType type;
	public GameObject panel;
	public Button closeButton;
	public TextMeshProUGUI messageText;
	[TextArea]
	public string messageFormat;

	/// <summary>Shows and hides the popup</summary>
	public void Pop(bool state, string message, Action ClosePopup)
	{
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(() => ClosePopup());

		panel.SetActive(state);
		messageText.text = string.Format(messageFormat, message);
	}
}