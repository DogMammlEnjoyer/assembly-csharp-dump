using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithCowatchingState : Message<CowatchingState>
	{
		public MessageWithCowatchingState(IntPtr c_message) : base(c_message)
		{
		}

		public override CowatchingState GetCowatchingState()
		{
			return base.Data;
		}

		protected override CowatchingState GetDataFromMessage(IntPtr c_message)
		{
			return new CowatchingState(CAPI.ovr_Message_GetCowatchingState(CAPI.ovr_Message_GetNativeMessage(c_message)));
		}
	}
}
