using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.UploadStringCompleted" /> event.</summary>
	public class UploadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets the server reply to a string upload operation that is started by calling an <see cref="Overload:System.Net.WebClient.UploadStringAsync" /> method.</summary>
		/// <returns>A <see cref="T:System.Byte" /> array that contains the server reply.</returns>
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal UploadStringCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly string _result;
	}
}
