using System;

public class VerifyAgeData
{
	public VerifyAgeData(VerifyAgeResponse response)
	{
		if (response == null)
		{
			return;
		}
		this.Status = response.Status;
		if (response.Session == null && response.DefaultSession == null)
		{
			return;
		}
		this.Session = new TMPSession(response.Session, response.DefaultSession, this.Status);
	}

	public readonly SessionStatus Status;

	public readonly TMPSession Session;
}
