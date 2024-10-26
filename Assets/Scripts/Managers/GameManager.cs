using UnityEngine;
using static Request;
using static Window;

public class GameManager : MonoBehaviour
{
	[Header("Settings")]
	public float spinnersSpeed;

	[Header("Assign in Inspector")]
	public WindowsManager windowsManager;
	public PopupManager popupManager;
	public ButlerManager butlerManager;
	public APIManager apiManager;
	public LoginPanel loginPanel;
	public ButlerPanel butlerPanel;
	public UploadPanel uploadPanel;

	void Awake()
	{
		apiManager.Init(popupManager.Pop);

		loginPanel.Init((apiKey, Fail) =>
		{
			windowsManager.SwitchToWindow(WindowType.LOADING);

			// wait for online checking
			StartCoroutine(apiManager.CheckAPIKeyValidity(apiKey, () =>
			{
				windowsManager.SwitchToWindow(WindowType.UPLOAD);
				uploadPanel.StartPanel();
			}, () =>
			{
				Fail();
				windowsManager.SwitchToWindow(WindowType.LOGIN);
			}));
		});
		butlerPanel.Init(butlerManager.CheckPathValidity, () =>
		{
			windowsManager.SwitchToWindow(WindowType.LOADING);

			// wait for online checking
			StartCoroutine(apiManager.CheckAPIKey(() =>
			{
				windowsManager.SwitchToWindow(WindowType.UPLOAD);
				uploadPanel.StartPanel();
			}, () => windowsManager.SwitchToWindow(WindowType.LOGIN)));

		});
		uploadPanel.Init(spinnersSpeed, (Success) => apiManager.SendRequest(RequestType.CREDENTIALS, Success), (Success) => apiManager.SendRequest(RequestType.GAMES, Success), (url, Success, Fail) => StartCoroutine(apiManager.DownloadTextureFromURL(url, Success, Fail)), butlerManager.UploadBuild);

		InitWindowManager();
	}

	void InitWindowManager()
	{
		WindowType initialWindow = WindowType.LOADING;

		// check if we have a valid path for butler.exe
		if(!butlerManager.CheckButlerPath())
			initialWindow = WindowType.BUTLER_PATH;
		else
		{
			// checks if the API key is valid
			StartCoroutine(apiManager.CheckAPIKey(() =>
			{
				windowsManager.SwitchToWindow(WindowType.UPLOAD);
				uploadPanel.StartPanel();
			}, () => windowsManager.SwitchToWindow(WindowType.LOGIN)));
		}

		windowsManager.Init(initialWindow, spinnersSpeed);
	}
}