using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.UploadValuesCompleted" /> event.</summary>
	public class UploadValuesCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadValuesCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets the server reply to a data upload operation started by calling an <see cref="Overload:System.Net.WebClient.UploadValuesAsync" /> method.</summary>
		/// <returns>A <see cref="T:System.Byte" /> array containing the server reply.</returns>
		public byte[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal UploadValuesCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly byte[] _result;
	}
}
