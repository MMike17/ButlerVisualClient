/// <summary>Class used to transfer user infos</summary>
public class UserData
{
	public string userName, userDisplayName, userURL, userCoverURL;
	public int userID;
	public bool isGamer;

	public string BuildWindowName(string longFormat, string shortFormat)
	{
		if(string.IsNullOrEmpty(userDisplayName) || string.IsNullOrWhiteSpace(userDisplayName))
			return string.Format(shortFormat, userName, userID);
		else
			return string.Format(longFormat, userDisplayName, userName, userID);
	}

	public string GetDebug()
	{
		string debug = string.Empty;

		debug += "userName : " + userName;
		debug += "\nuserDisplayName : " + userDisplayName;
		debug += "\nuserURL : " + userURL;
		debug += "\nuserCoverURL : " + userCoverURL;
		debug += "\nuserID : " + userID;
		debug += "\nisGamer : " + isGamer;

		return debug;
	}
}