using System;
using System.Runtime.InteropServices;

public class MothershipGetPlayerProgressionCallback : GetProgressionTrackValuesForPlayerCompleteClientDelegateWrapper
{
	public MothershipGetPlayerProgressionCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<GetProgressionTrackValuesForPlayerResponse> callbackPair = gchandle.Target as CallbackPair<GetProgressionTrackValuesForPlayerResponse>;
			if (wasSuccess)
			{
				GetProgressionTrackValuesForPlayerResponse obj = GetProgressionTrackValuesForPlayerResponse.FromMothershipResponse(response);
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
