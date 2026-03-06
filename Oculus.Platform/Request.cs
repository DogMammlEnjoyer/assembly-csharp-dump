using System;
using UnityEngine;

namespace Oculus.Platform
{
	public sealed class Request<T> : Request
	{
		public Request(ulong requestID) : base(requestID)
		{
		}

		public Request<T> OnComplete(Message<T>.Callback callback)
		{
			if (this.callback_ != null)
			{
				throw new UnityException("Attempted to attach multiple handlers to a Request.  This is not allowed.");
			}
			this.callback_ = callback;
			Callback.AddRequest(this);
			return this;
		}

		public override void HandleMessage(Message msg)
		{
			if (!(msg is Message<T>))
			{
				string str = "Unable to handle message: ";
				Type type = msg.GetType();
				Debug.LogError(str + ((type != null) ? type.ToString() : null));
				return;
			}
			if (this.callback_ != null)
			{
				this.callback_((Message<T>)msg);
				return;
			}
			throw new UnityException("Request with no handler.  This should never happen.");
		}

		private Message<T>.Callback callback_;
	}
}
