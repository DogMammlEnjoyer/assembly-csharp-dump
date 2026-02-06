using System;
using System.Runtime.InteropServices;

public class MothershipGetStorefrontCallback : GetStorefrontRequestCompleteDelegateWrapper
{
	public MothershipGetStorefrontCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<MothershipGetStorefrontResponse> callbackPair = gchandle.Target as CallbackPair<MothershipGetStorefrontResponse>;
			if (wasSuccess)
			{
				MothershipGetStorefrontResponse obj = MothershipGetStorefrontResponse.FromMothershipResponse(response);
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
