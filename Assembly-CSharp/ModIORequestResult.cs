using System;
using Modio;

public struct ModIORequestResult
{
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult result;
		result.success = false;
		result.message = inMessage;
		return result;
	}

	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult result;
		result.success = true;
		result.message = "";
		return result;
	}

	public static ModIORequestResult CreateFromError(Error error)
	{
		ModIORequestResult result;
		if (error)
		{
			result.success = false;
			result.message = error.GetMessage();
		}
		else
		{
			result.success = true;
			result.message = "";
		}
		return result;
	}

	public bool success;

	public string message;
}
