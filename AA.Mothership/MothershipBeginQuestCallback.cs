using System;
using System.Runtime.InteropServices;

public class MothershipBeginQuestCallback : QuestBeginLoginV2RequestCompleteDelegateWrapper
{
	public MothershipBeginQuestCallback()
	{
		this.swigCMemOwn = false;
	}

	public override void OnCompleteCallback(MothershipResponse response, bool wasSuccess, MothershipError error, IntPtr userData)
	{
		if (userData != IntPtr.Zero)
		{
			GCHandle gchandle = (GCHandle)userData;
			CallbackPair<PlayerQuestBeginLoginV2Response> callbackPair = gchandle.Target as CallbackPair<PlayerQuestBeginLoginV2Response>;
			if (wasSuccess)
			{
				PlayerQuestBeginLoginV2Response obj = PlayerQuestBeginLoginV2Response.FromMothershipResponse(response);
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
