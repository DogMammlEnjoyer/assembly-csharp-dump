using System;

namespace Modio.Customizations
{
	[Serializable]
	internal struct WssMessages
	{
		public WssMessages(params WssMessage[] messages)
		{
			this.messages = messages;
		}

		public WssMessage[] messages;
	}
}
