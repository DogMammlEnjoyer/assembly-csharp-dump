using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.UploadDataCompleted" /> event.</summary>
	public class UploadDataCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadDataCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets the server reply to a data upload operation started by calling an <see cref="Overload:System.Net.WebClient.UploadDataAsync" /> method.</summary>
		/// <returns>A <see cref="T:System.Byte" /> array containing the server reply.</returns>
		public byte[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal UploadDataCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly byte[] _result;
	}
}
