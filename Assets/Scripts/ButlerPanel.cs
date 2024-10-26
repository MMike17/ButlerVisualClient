using System;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Class used to manage the "butler path" panel</summary>
public class ButlerPanel : MonoBehaviour
{
	[Header("Assign in Inspector")]
	public TMP_InputField inputPath;
	public Button pickPath, validate;
	public TextMeshProUGUI errorMessage;

	Func<string, bool> SaveButlerPath;
	Action Next;

	public void Init(Func<string, bool> saveButlerPath, Action next)
	{
		SaveButlerPath = saveButlerPath;
		Next = next;

		errorMessage.enabled = false;

		pickPath.onClick.AddListener(() => StandaloneFileBrowser.OpenFilePanelAsync("Select \"butler.exe\" file", "", "exe", false, (path) => inputPath.SetTextWithoutNotify(path[0])));

		validate.onClick.AddListener(() =>
		{
			// stops if string empty
			if (string.IsNullOrEmpty(inputPath.text))
			{
				errorMessage.enabled = true;
				return;
			}

			// checks path
			if (SaveButlerPath(inputPath.text))
				Next();
			else
				errorMessage.enabled = true;
		});
	}
}