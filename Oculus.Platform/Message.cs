using System;

namespace Oculus.Platform
{
	public abstract class Message<T> : Message
	{
		public Message(IntPtr c_message) : base(c_message)
		{
			if (!base.IsError)
			{
				this.data = this.GetDataFromMessage(c_message);
			}
		}

		public T Data
		{
			get
			{
				return this.data;
			}
		}

		protected abstract T GetDataFromMessage(IntPtr c_message);

		private T data;

		public new delegate void Callback(Message<T> message);
	}
}
