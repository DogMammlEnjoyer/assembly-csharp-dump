using System;
using System.Runtime.InteropServices;

public class MothershipPurchaseOfferCallback : PurchaseOfferRequestCompleteDelegateWrapper
{
	public MothershipPurchaseOfferCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<MothershipPurchaseOfferResponse> callbackPair = gchandle.Target as CallbackPair<MothershipPurchaseOfferResponse>;
			if (wasSuccess)
			{
				MothershipPurchaseOfferResponse obj = MothershipPurchaseOfferResponse.FromMothershipResponse(response);
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
