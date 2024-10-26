using System;
using UnityEngine.Networking;

/// <summary>Itch.io API request</summary>
[Serializable]
public class Request
{
	public enum RequestType
	{
		NONE,
		CHECK,
		CREDENTIALS,
		GAMES
	}

	public RequestType type;
	public string urlFormat;

	/// <summary>Gets formatted UnityWebRequest ready to be sent</summary>
	public UnityWebRequest GetRequest(string apiKey)
	{
		UnityWebRequest request = UnityWebRequest.Get(string.Format(urlFormat, apiKey));
		request.downloadHandler = new DownloadHandlerBuffer();

		return request;
	}
}