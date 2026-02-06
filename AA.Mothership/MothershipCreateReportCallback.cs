using System;
using System.Runtime.InteropServices;

public class MothershipCreateReportCallback : CreateReportCompleteDelegateWrapper
{
	public MothershipCreateReportCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<CreateReportResponse> callbackPair = gchandle.Target as CallbackPair<CreateReportResponse>;
			if (wasSuccess)
			{
				CreateReportResponse obj = CreateReportResponse.FromMothershipResponse(response);
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
