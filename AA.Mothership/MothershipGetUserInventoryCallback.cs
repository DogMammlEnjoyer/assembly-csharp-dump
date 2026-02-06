using System;
using System.Runtime.InteropServices;

public class MothershipGetUserInventoryCallback : GetUserInventoryCompleteClientDelegateWrapper
{
	public MothershipGetUserInventoryCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<MothershipGetInventoryResponse> callbackPair = gchandle.Target as CallbackPair<MothershipGetInventoryResponse>;
			if (wasSuccess)
			{
				MothershipGetInventoryResponse obj = MothershipGetInventoryResponse.FromMothershipResponse(response);
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
