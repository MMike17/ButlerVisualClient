using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class used to parse data retrieved from server</summary>
public class DataParser
{
	// user keys
	const string KEY_USER_COVER_URL = "cover_url";
	const string KEY_USER_ID = "id";
	const string KEY_USER_URL = "url";
	const string KEY_USER_GAMER = "gamer";
	const string KEY_USER_USERNAME = "username";
	const string KEY_USER_DISPLAYNAME = "display_name";

	// games keys
	const string KEY_OSX = "p_osx";
	const string KEY_WINDOWS = "p_windows";
	const string KEY_LINUX = "p_linux";
	const string KEY_ANDROID = "p_android";
	const string KEY_PUBLISHED = "published";
	const string KEY_COVER_URL = "cover_url";
	const string KEY_TITLE = "title";
	const string KEY_URL_NAME = "url";
	const string KEY_CREATION_DATE = "created_at";

	/// <summary>Parses user data retrieved from server and returns UserData object</summary>
	public UserData ParseUser(string rawData)
	{
		UserData data = new UserData();

		// splits infos in fields
		string cleanContent = rawData.Substring(9, rawData.Length - 11).Replace("\\/", "/").Replace("\"", "");
		string[] fields = cleanContent.Split(new char[1] { ',' });

		foreach (string field in fields)
		{
			// scans keys
			string[] cells = field.Split(':');

			string key = cells[0];
			string value = cells.Length > 2 ? cells[1] + ":" + cells[2] : cells[1];

			switch (key)
			{
				case KEY_USER_COVER_URL:
					data.userCoverURL = value;
					break;
				case KEY_USER_ID:
					data.userID = int.Parse(value);
					break;
				case KEY_USER_URL:
					data.userURL = value;
					break;
				case KEY_USER_GAMER:
					data.isGamer = bool.Parse(value);
					break;
				case KEY_USER_USERNAME:
					data.userName = value;
					break;
				case KEY_USER_DISPLAYNAME:
					data.userDisplayName = value;
					break;
			}
		}

		return data;
	}

	/// <summary>Parses games list retrieved from server and returns GameInfos object</summary>
	public List<GameInfos> ParseGames(string rawData)
	{
		List<GameInfos> infos = new List<GameInfos>();

		string cleanContent = rawData.Substring(11, rawData.Length - 12).Replace("\\/", "/").Replace("\"", "");
		string[] games = cleanContent.Split(new string[1] { "},{" }, StringSplitOptions.None);

		// loops through games
		foreach (string gameContent in games)
		{
			GameInfos gameInfo = new GameInfos();

			string[] shard = gameContent.Split(new string[1] { "user:" }, StringSplitOptions.None);
			string separator = "},";
			bool error = false;

			if (shard.Length <= 1)
				error = true;
			else if (!shard[1].Contains(separator))
			{
				separator = "}";
				error = !shard[1].Contains(separator);
			}

			if (error)
			{
				Debug.LogError("Couldn't parse game with name \"" + shard[0].Split(':')[1].Split(',')[0] + "\"");
				continue;
			}

			string cleanGame = shard[0] + shard[1].Split(new string[1] { separator }, StringSplitOptions.None)[1];
			string[] fields = cleanGame.Split(new char[1] { ',' }, StringSplitOptions.None);

			foreach (string field in fields)
			{
				if (!field.Contains(":"))
					continue; // empty string

				// scans keys
				string[] cells = field.Split(new char[1] { ':' });

				string key = cells[0];
				string value = cells[1];

				// to manage content that includes ":"
				for (int i = 2; i < cells.Length; i++)
					value += ":" + cells[i];

				switch (key)
				{
					case KEY_OSX:
						gameInfo.isMac = bool.Parse(value);
						break;
					case KEY_WINDOWS:
						gameInfo.isWindows = bool.Parse(value);
						break;
					case KEY_LINUX:
						gameInfo.isLinux = bool.Parse(value);
						break;
					case KEY_ANDROID:
						gameInfo.isAndroid = bool.Parse(value);
						break;
					case KEY_PUBLISHED:
						gameInfo.isPublished = bool.Parse(value);
						break;
					case KEY_COVER_URL:
						gameInfo.coverURL = value;
						break;
					case KEY_TITLE:
						gameInfo.title = value;
						break;
					case KEY_URL_NAME:
						gameInfo.uploadName = value.Split('/')[3];
						break;
					case KEY_CREATION_DATE:
						if (DateTime.TryParse(value, out DateTime date))
							gameInfo.creationDate = date;
						else
							gameInfo.creationDate = DateTime.Now; // placeholder date
						break;
				}
			}

			infos.Add(gameInfo);
		}

		infos.Sort();
		return infos;
	}
}