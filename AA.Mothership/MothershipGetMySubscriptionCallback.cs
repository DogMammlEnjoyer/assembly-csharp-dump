using System;
using System.Runtime.InteropServices;

public class MothershipGetMySubscriptionCallback : ClientGetMySubscriptionCompleteDelegateWrapper
{
	public MothershipGetMySubscriptionCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<GetMySubscriptionsResponse> callbackPair = gchandle.Target as CallbackPair<GetMySubscriptionsResponse>;
			if (wasSuccess)
			{
				GetMySubscriptionsResponse obj = GetMySubscriptionsResponse.FromMothershipResponse(response);
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
