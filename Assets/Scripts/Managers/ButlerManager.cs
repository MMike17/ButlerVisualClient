using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary>Class executing butler commands through butler.exe</summary>
public class ButlerManager : MonoBehaviour
{
	const string KEY_BUTLER_PATH = "ButlerPath";

	[Header("Settings")]
	public string butlerCommandFormat;

	string butlerPath;

	/// <summary>Checks if we have a path for butler.exe and if it's valid</summary>
	public bool CheckButlerPath()
	{
		if(PlayerPrefs.HasKey(KEY_BUTLER_PATH))
		{
			butlerPath = PlayerPrefs.GetString(KEY_BUTLER_PATH);

			return File.Exists(butlerPath);
		}
		else
			return false;
	}

	/// <summary>Checks if butler.exe exists at provided path and saves path in player prefs</summary>
	public bool CheckPathValidity(string path)
	{
		if(File.Exists(path))
		{
			PlayerPrefs.SetString(KEY_BUTLER_PATH, path);
			PlayerPrefs.Save();
			return true;
		}
		else
			return false;
	}

	/// <summary>Launches build pushing through butler</summary>
	public void UploadBuild(string folder, string userName, string game, string channel)
	{
		// butler push <folder> <user>/<project>:<channel>
		string butlerCommand = string.Format(butlerCommandFormat, "\"" + folder + "\"", userName, game, channel).Replace("\\", "/");
		butlerCommand = "echo off &echo Starting build upload &echo. &" + butlerCommand;

		Process butler = new Process();
		butler.StartInfo = new ProcessStartInfo("cmd.exe", "/c" + butlerCommand);
		butler.StartInfo.WorkingDirectory = Path.GetDirectoryName(butlerPath);
		butler.Start();
	}
}