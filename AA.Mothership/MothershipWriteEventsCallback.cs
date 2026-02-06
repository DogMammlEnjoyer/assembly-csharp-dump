using System;
using System.Runtime.InteropServices;

public class MothershipWriteEventsCallback : WriteEventsCompleteClientDelegateWrapper
{
	public MothershipWriteEventsCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<MothershipWriteEventsResponse> callbackPair = gchandle.Target as CallbackPair<MothershipWriteEventsResponse>;
			if (wasSuccess)
			{
				MothershipWriteEventsResponse obj = MothershipWriteEventsResponse.FromMothershipResponse(response);
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
