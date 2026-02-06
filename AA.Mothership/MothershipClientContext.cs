using System;

public static class MothershipClientContext
{
	public static bool IsClientLoggedIn()
	{
		return !string.IsNullOrEmpty(MothershipClientContext.MothershipId) && !string.IsNullOrEmpty(MothershipClientContext.Token);
	}

	public static void ForgetAllCredentials()
	{
		MothershipClientContext.MothershipId = (MothershipClientContext.Token = "");
	}

	public static string MothershipId;

	public static string Token;
}
