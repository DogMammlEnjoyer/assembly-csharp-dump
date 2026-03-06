using System;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Composer.Interfaces;

namespace Meta.WitAi.Composer
{
	[Serializable]
	public class ComposerSessionData
	{
		public string sessionID
		{
			get
			{
				return this.session.SessionId;
			}
		}

		public ComposerContextMap contextMap
		{
			get
			{
				return this.session.ContextMap;
			}
		}

		public IComposerSession session;

		public ComposerService composer;

		public ComposerResponseData responseData;
	}
}
