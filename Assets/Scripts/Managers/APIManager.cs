using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Request;

/// <summary>Class making calls to itch.io API (requests init)</summary>
public class APIManager : MonoBehaviour
{
	const string KEY_API_KEY = "ApiKey";

	[Header("Settings")]
	public List<Request> apiRequests;

	Action<RequestType, string> ThrowError;
	string apiKey;

	public void Init(Action<RequestType, string> error)
	{
		ThrowError = error;
	}

	/// <summary>Checks if we have an API key to access itch.io API and if the key is valid</summary>
	public IEnumerator CheckAPIKey(Action Success, Action Fail)
	{
		// retrive local API Key
		if(PlayerPrefs.HasKey(KEY_API_KEY))
		{
			apiKey = PlayerPrefs.GetString(KEY_API_KEY);
			yield return CheckAPIKeyValidity(apiKey, Success, Fail);
		}
		else
			Fail();

		yield break;
	}

	/// <summary>Checks validity of provided API key</summary>
	public IEnumerator CheckAPIKeyValidity(string apiKey, Action Success, Action Fail)
	{
		this.apiKey = apiKey;

		yield return SendRequest(RequestType.CHECK, (response) =>
		{
			// if key is valid => saves key localy
			PlayerPrefs.SetString(KEY_API_KEY, apiKey);
			PlayerPrefs.Save();

			Success();
		}, true, () => Fail());
	}

	/// <summary>Sends a request to itch.io API/></summary>
	public void SendRequest(RequestType request, Action<string> RequestSuccess)
	{
		StartCoroutine(SendRequest(request, RequestSuccess, false, null));
	}

	/// <summary>Sends a request to itch.io API</summary>
	public IEnumerator SendRequest(RequestType request, Action<string> RequestSuccess, bool isSilent = false, Action Fail = null)
	{
		Request selectedRequest = apiRequests.Find(item => { return item.type == request; });

		if(selectedRequest == null)
		{
			ThrowError(request, "Couldn't find request of type " + request);
			yield break;
		}

		UnityWebRequest generatedRequest = selectedRequest.GetRequest(apiKey);

		if(string.IsNullOrEmpty(generatedRequest.url))
		{
			Debug.Log(generatedRequest.url + " / " + selectedRequest.urlFormat);
			yield break;
		}

		yield return generatedRequest.SendWebRequest();

		if(generatedRequest.isNetworkError || generatedRequest.isHttpError)
		{
			Debug.Log("format : " + selectedRequest.urlFormat + " / key : " + apiKey + " / full url : " + generatedRequest.url);
			Debug.Log(generatedRequest.error);

			if(isSilent)
			{
				Fail();
				yield break;
			}
			else
			{
				ThrowError(request, generatedRequest.error);
				yield break;
			}
		}

		// waits until download is done
		while (!generatedRequest.downloadHandler.isDone)
			yield return null;

		string data = !string.IsNullOrEmpty(generatedRequest.downloadHandler.text) ? generatedRequest.downloadHandler.text : "error";
		RequestSuccess(data);
		yield break;
	}

	/// <summary>Downloads a texture from a URL</summary>
	public IEnumerator DownloadTextureFromURL(string url, Action<Texture2D> Success, Action Fail)
	{
		if(string.IsNullOrEmpty(url))
		{
			Debug.Log(url);
			yield break;
		}

		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

		yield return request.SendWebRequest();

		if(request.isNetworkError || request.isHttpError)
		{
			Fail();
			yield break;
		}

		// wait for end of download
		while (!request.downloadHandler.isDone)
			yield return null;

		Texture2D downloaded = (request.downloadHandler as DownloadHandlerTexture).texture;
		Success(downloaded);
		yield break;
	}
}