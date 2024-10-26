using System;

/// <summary>Class used to transfer game infos</summary>
public class GameInfos : IComparable
{
	public string title, coverURL, uploadName;
	public bool isAndroid, isLinux, isMac, isWindows, isPublished;
	public DateTime creationDate;

	public int CompareTo(object obj)
	{
		GameInfos otherInfo = obj as GameInfos;
		return creationDate.CompareTo(otherInfo.creationDate);
	}

	public string GetDebug()
	{
		string debug = string.Empty;

		debug += "title : " + title;
		debug += "\ncoverURL : " + coverURL;
		debug += "\nuploadName : " + uploadName;
		debug += "\nisAndroid : " + isAndroid;
		debug += "\nisLinux : " + isLinux;
		debug += "\nisMac : " + isMac;
		debug += "\nisWindows : " + isWindows;
		debug += "\nisPublished : " + isPublished;
		debug += "\ncreationDate : " + creationDate.ToString("yyyy-MM-dd hh:mm:ss");

		return debug;
	}
}