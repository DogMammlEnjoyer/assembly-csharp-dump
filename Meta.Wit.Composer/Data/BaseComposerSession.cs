using System;
using Meta.WitAi.Composer.Interfaces;

namespace Meta.WitAi.Composer.Data
{
	public class BaseComposerSession : IComposerSession
	{
		public string SessionId { get; }

		public ComposerContextMap ContextMap { get; }

		public bool HasStarted { get; private set; }

		public DateTime SessionStart { get; private set; }

		public BaseComposerSession(string sessionId, ComposerContextMap contextMap)
		{
			this.HasStarted = false;
			this.SessionId = sessionId;
			this.ContextMap = contextMap;
			this.SessionStart = DateTime.UtcNow;
		}

		public void StartSession()
		{
			if (this.HasStarted)
			{
				return;
			}
			this.HasStarted = true;
			this.SessionStart = DateTime.UtcNow;
		}

		public void EndSession()
		{
			if (!this.HasStarted)
			{
				return;
			}
			this.HasStarted = false;
		}
	}
}
