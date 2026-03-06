using System;
using System.ComponentModel;
using System.IO;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.OpenWriteCompleted" /> event.</summary>
	public class OpenWriteCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenWriteCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets a writable stream that is used to send data to a server.</summary>
		/// <returns>A <see cref="T:System.IO.Stream" /> where you can write data to be uploaded.</returns>
		public Stream Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal OpenWriteCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly Stream _result;
	}
}
