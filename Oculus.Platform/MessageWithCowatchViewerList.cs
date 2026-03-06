using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCowatchViewerList : Message<CowatchViewerList>
	{
		public MessageWithCowatchViewerList(IntPtr c_message) : base(c_message)
		{
		}

		public override CowatchViewerList GetCowatchViewerList()
		{
			return base.Data;
		}

		protected override CowatchViewerList GetDataFromMessage(IntPtr c_message)
		{
			return new CowatchViewerList(CAPI.ovr_Message_GetCowatchViewerArray(CAPI.ovr_Message_GetNativeMessage(c_message)));
		}
	}
}
