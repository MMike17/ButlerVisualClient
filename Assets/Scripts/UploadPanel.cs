using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Class used to manage the "upload" panel</summary>
public class UploadPanel : MonoBehaviour
{
	[Header("Settings")]
	public Color validGamePanelColor;
	public string longUserNameFormat, shortUserNameFormat, invalidPathError;

	[Header("User identity")]
	public RawImage userCoverImage;
	public GameObject gamer, coverNotFound, coverError;
	public TextMeshProUGUI userName, link;
	public Transform loadingSpinner;
	public Button linkButton;

	[Header("Game path")]
	public TMP_InputField gamePath;
	public Button gamePathFolder;

	[Header("Game list")]
	public Transform gamesHolder;
	public GamePanel gamePrefab;

	[Header("Support pick")]
	public TMP_Dropdown buildSelector;
	public Button upload;

	Action<string, Action<Texture2D>, Action> DownloadPicture;
	Action<string, string, string, string> SendBuild;
	Action<Action<string>> GetUserGames, GetUserInfos;
	List<GamePanel> spawnedGames;
	DataParser dataParser;
	float spinnerSpeed;
	bool hasExtractedGames;

	GameInfos selectedGame;
	UserData userData;

	public void Init(float spinnerSpeed, Action<Action<string>> getUserInfos, Action<Action<string>> getUserGames, Action<string, Action<Texture2D>, Action> downloadUserCover, Action<string, string, string, string> sendBuild)
	{
		this.spinnerSpeed = spinnerSpeed;
		GetUserGames = getUserGames;
		DownloadPicture = downloadUserCover;
		GetUserInfos = getUserInfos;
		SendBuild = sendBuild;

		dataParser = new DataParser();

		gamePathFolder.onClick.AddListener(() => StandaloneFileBrowser.OpenFolderPanelAsync("Select game folder to upload", "", false, (path) => gamePath.SetTextWithoutNotify(path[0])));

		upload.onClick.AddListener(UploadGameBuild);

		hasExtractedGames = false;
		coverError.SetActive(false);
		coverNotFound.SetActive(false);
		userCoverImage.enabled = false;
	}

	void Update()
	{
		if(userCoverImage.texture == null && !coverError.activeSelf && !coverNotFound.activeSelf)
			loadingSpinner.Rotate(0, 0, spinnerSpeed * Time.deltaTime);
		else
			loadingSpinner.gameObject.SetActive(false);

		// extracts games when ready
		if(!hasExtractedGames && userData != null && !string.IsNullOrEmpty(userData.userCoverURL))
		{
			GetUserGames(ParseUserGames);
			hasExtractedGames = true;
		}
	}

	/// <summary>Called when the app goes to this window</summary>
	public void StartPanel()
	{
		GetUserInfos(ParseUserData);
	}

	void ParseUserData(string rawData)
	{
		userData = dataParser.ParseUser(rawData);

		userName.text = userData.BuildWindowName(longUserNameFormat, shortUserNameFormat);
		link.text = userData.userURL;

		linkButton.onClick.RemoveAllListeners();
		linkButton.onClick.AddListener(() => Application.OpenURL(userData.userURL));

		LayoutRebuilder.ForceRebuildLayoutImmediate(userName.transform.parent.parent.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(userName.transform.parent.parent.GetComponent<RectTransform>());

		gamer.SetActive(userData.isGamer);

		if(string.IsNullOrEmpty(userData.userCoverURL))
			coverNotFound.SetActive(true);
		else
			DownloadPicture(userData.userCoverURL, SetUserCoverPicture, () => coverError.SetActive(true));
	}

	void SetUserCoverPicture(Texture2D texture)
	{
		userCoverImage.enabled = true;
		userCoverImage.texture = texture;
	}

	void ParseUserGames(string rawData)
	{
		spawnedGames = new List<GamePanel>();
		List<GameInfos> extractedGames = dataParser.ParseGames(rawData);

		foreach (GameInfos info in extractedGames)
		{
			GamePanel panel = Instantiate(gamePrefab, gamesHolder);
			panel.Create(spinnerSpeed, validGamePanelColor, DownloadPicture, SelectGame, info);

			spawnedGames.Add(panel);
		}
	}

	void SelectGame(GameInfos infos)
	{
		selectedGame = infos;

		spawnedGames.ForEach(item =>
		{
			if(item.title.text != infos.title)
				item.selectionToggle.isOn = false;
		});
	}

	void UploadGameBuild()
	{
		// manages errors
		if(selectedGame == null)
			return;

		if(!Directory.Exists(gamePath.text))
		{
			gamePath.SetTextWithoutNotify("");
			gamePath.placeholder.GetComponent<TextMeshProUGUI>().text = invalidPathError;
			return;
		}

		// select build target
		string buildTarget = "none";

		switch(buildSelector.value)
		{
			case 0: // windows
				buildTarget = "windows-64";
				break;
			case 1: // Mac
				buildTarget = "mac-universal";
				break;
			case 2: // Linux
				buildTarget = "linux-universal";
				break;
			case 3: // Android
				buildTarget = "android";
				break;
		}

		SendBuild(gamePath.text, userData.userName, selectedGame.uploadName, buildTarget);
	}
}