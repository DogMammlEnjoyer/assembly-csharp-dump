using System;
using System.Runtime.InteropServices;

public class MothershipSetUserDataCallback : SetUserDataCompleteClientDelegateWrapper
{
	public MothershipSetUserDataCallback(MothershipClientApiClient clientApiClient) : base(clientApiClient)
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<SetUserDataResponse> callbackPair = gchandle.Target as CallbackPair<SetUserDataResponse>;
			if (wasSuccess)
			{
				SetUserDataResponse obj = SetUserDataResponse.FromMothershipResponse(response);
				callbackPair.successCallback(obj);
			}
			else
			{
				callbackPair.errorCallback(error, response.statusCode);
			}
			gchandle.Free();
		}
	}
}
