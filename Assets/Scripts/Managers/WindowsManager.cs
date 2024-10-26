using System.Collections.Generic;
using UnityEngine;
using static Window;

/// <summary>Class managing windows (requests init)</summary>
public class WindowsManager : MonoBehaviour
{
	[Header("Assign in Inspector")]
	public Transform loadingSpinner;
	public List<Window> panels;

	[Header("Test")]
	public bool shouldTest;
	public WindowType testType;

	WindowType currentType;
	float spinnerSpeed;

	void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if(shouldTest)
		{
			if(currentType != testType)
				SwitchToWindow(testType);

			Window targetWindow = panels.Find(item => { return item.type == currentType; });
			Rect rect = loadingSpinner.parent.GetComponent<RectTransform>().rect;

			if(targetWindow != null)
				Debug.DrawLine(new Vector2(0, rect.height - targetWindow.size.y), new Vector2(rect.width, rect.height - targetWindow.size.y), Color.black);
		}
#endif
	}

	public void Init(WindowType initialType, float spinnerSpeed)
	{
		SwitchToWindow(initialType);
		currentType = initialType;

		this.spinnerSpeed = spinnerSpeed;

		shouldTest = false;
	}

	void Update()
	{
		loadingSpinner.Rotate(0, 0, spinnerSpeed * Time.deltaTime);
	}

	/// <summary>Switches to provided window</summary>
	public void SwitchToWindow(WindowType window)
	{
		if(window == currentType)
			return;

		Window targetWindow = panels.Find(item => { return item.type == window; });

		if(targetWindow == null)
		{
			Debug.LogError("Couldn't find window for type : " + window);
			return;
		}

		panels.ForEach(item => item.Show(false));
		targetWindow.Show(true);

		currentType = window;
	}
}