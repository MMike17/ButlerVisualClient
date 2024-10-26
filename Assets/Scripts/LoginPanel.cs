using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Class used to manage the "login" panel</summary>
public class LoginPanel : MonoBehaviour
{
	[Header("Assign in Inspector")]
	public TMP_InputField inputPath;
	public GameObject errorMessage;
	public Button validate, keyLink;

	public void Init(Action<string, Action> CheckKey)
	{
		errorMessage.SetActive(false);

		validate.onClick.AddListener(() =>
		{
			if(inputPath.text != String.Empty)
				CheckKey(inputPath.text, ShowErrorMessage);
			else
				ShowErrorMessage();
		});

		keyLink.onClick.AddListener(() => Application.OpenURL("https://itch.io/user/settings/api-keys"));
	}

	void ShowErrorMessage()
	{
		errorMessage.SetActive(true);
	}
}