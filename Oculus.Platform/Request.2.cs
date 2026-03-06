using System;
using UnityEngine;

namespace Oculus.Platform
{
	public class Request
	{
		public Request(ulong requestID)
		{
			this.RequestID = requestID;
		}

		public ulong RequestID { get; set; }

		public Request OnComplete(Message.Callback callback)
		{
			this.callback_ = callback;
			Callback.AddRequest(this);
			return this;
		}

		public virtual void HandleMessage(Message msg)
		{
			if (this.callback_ != null)
			{
				this.callback_(msg);
				return;
			}
			throw new UnityException("Request with no handler.  This should never happen.");
		}

		public static void RunCallbacks(uint limit = 0U)
		{
			if (limit == 0U)
			{
				Callback.RunCallbacks();
				return;
			}
			Callback.RunLimitedCallbacks(limit);
		}

		private Message.Callback callback_;
	}
}
