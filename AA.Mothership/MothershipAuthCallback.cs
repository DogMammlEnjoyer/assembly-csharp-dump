using System;
using System.Runtime.InteropServices;

public class MothershipAuthCallback : LoginCompleteDelegateWrapper
{
	public MothershipAuthCallback(MothershipClientApiClient clientApiClient) : base(clientApiClient)
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<LoginResponse> callbackPair = gchandle.Target as CallbackPair<LoginResponse>;
			if (wasSuccess)
			{
				LoginResponse loginResponse = LoginResponse.FromMothershipResponse(response);
				MothershipClientContext.MothershipId = loginResponse.MothershipPlayerId;
				MothershipClientContext.Token = loginResponse.Token;
				callbackPair.successCallback(loginResponse);
			}
			else
			{
				callbackPair.errorCallback(error, response.statusCode);
			}
			gchandle.Free();
		}
	}
}
