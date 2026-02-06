using System;
using System.Runtime.InteropServices;

public class MothershipListTitleDataCallback : ListMothershipTitleDataCompleteDelegateWrapper
{
	public MothershipListTitleDataCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData == IntPtr.Zero)
		{
			return;
		}
		GCHandle gchandle = (GCHandle)userData;
		try
		{
			CallbackPair<ListClientMothershipTitleDataResponse> callbackPair = gchandle.Target as CallbackPair<ListClientMothershipTitleDataResponse>;
			if (wasSuccess)
			{
				ListClientMothershipTitleDataResponse obj = ListClientMothershipTitleDataResponse.FromMothershipResponse(response);
				Action<ListClientMothershipTitleDataResponse> successCallback = callbackPair.successCallback;
				if (successCallback != null)
				{
					successCallback(obj);
				}
			}
			else
			{
				Action<MothershipError, int> errorCallback = callbackPair.errorCallback;
				if (errorCallback != null)
				{
					errorCallback(error, response.statusCode);
				}
			}
		}
		finally
		{
			gchandle.Free();
		}
	}
}
