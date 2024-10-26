using System;
using UnityEngine;

/// <summary>In app window</summary>
[Serializable]
public class Window
{
	public enum WindowType
	{
		NONE,
		BUTLER_PATH,
		LOADING,
		LOGIN,
		UPLOAD
	}

	public WindowType type;
	public GameObject panel;
	public Vector2Int size;

	/// <summary>Pops window</summary>
	public void Show(bool state)
	{
		panel.SetActive(state);
		Screen.SetResolution(size.x, size.y, FullScreenMode.Windowed);
	}
}