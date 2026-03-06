using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCowatchViewerUpdate : Message<CowatchViewerUpdate>
	{
		public MessageWithCowatchViewerUpdate(IntPtr c_message) : base(c_message)
		{
		}

		public override CowatchViewerUpdate GetCowatchViewerUpdate()
		{
			return base.Data;
		}

		protected override CowatchViewerUpdate GetDataFromMessage(IntPtr c_message)
		{
			return new CowatchViewerUpdate(CAPI.ovr_Message_GetCowatchViewerUpdate(CAPI.ovr_Message_GetNativeMessage(c_message)));
		}
	}
}
