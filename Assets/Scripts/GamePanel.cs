using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Class used to show user games</summary>
public class GamePanel : MonoBehaviour
{
	[Header("Assign in Inpector")]
	public Toggle selectionToggle;
	public TextMeshProUGUI title;
	public Transform loadingSpinner;
	public RawImage coverImage;
	public Image android, linux, osx, windows;
	public GameObject coverNotFound, coverError;

	Action<string, Action<Texture2D>, Action> DownloadTexture;
	Color validColor;
	float spinnerSpeed;

	/// <summary>Assigns all the needed informations to the game panel</summary>
	public void Create(float spinnerSpeed, Color validColor, Action<string, Action<Texture2D>, Action> downloadTexture, Action<GameInfos> selectGame, GameInfos infos)
	{

		this.spinnerSpeed = spinnerSpeed;
		this.validColor = validColor;

		selectionToggle.onValueChanged.AddListener((state) =>
		{
			if(state)
				selectGame(infos);
		});

		title.text = infos.title;

		android.color = infos.isAndroid ? validColor : android.color;
		linux.color = infos.isLinux ? validColor : linux.color;
		osx.color = infos.isMac ? validColor : osx.color;
		windows.color = infos.isWindows ? validColor : windows.color;

		Material instance = new Material(coverImage.material);
		instance.SetFloat("_EffectAmount", infos.isPublished ? 0 : 1);
		coverImage.material = instance;

		coverImage.enabled = false;
		coverError.SetActive(false);
		coverNotFound.SetActive(false);

		selectionToggle.isOn = false;

		if(string.IsNullOrEmpty(infos.coverURL))
			coverNotFound.SetActive(true);
		else
			downloadTexture(infos.coverURL, texture2D => { coverImage.texture = texture2D; coverImage.enabled = true; }, () => coverError.SetActive(true));
	}

	void Update()
	{
		if(coverImage.texture == null && !coverError.activeSelf && !coverNotFound.activeSelf)
			loadingSpinner.Rotate(0, 0, spinnerSpeed * Time.deltaTime);
		else
			loadingSpinner.gameObject.SetActive(false);
	}
}