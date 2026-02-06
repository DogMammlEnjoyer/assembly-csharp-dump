using System;
using System.Runtime.InteropServices;

public class MothershipGetRoomPlayersSubscriptionsCallback : ClientGetBulkSubscriptionsCompleteDelegateWrapper
{
	public MothershipGetRoomPlayersSubscriptionsCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<BulkGetSubscriptionsResponse> callbackPair = gchandle.Target as CallbackPair<BulkGetSubscriptionsResponse>;
			if (wasSuccess)
			{
				BulkGetSubscriptionsResponse obj = BulkGetSubscriptionsResponse.FromMothershipResponse(response);
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
