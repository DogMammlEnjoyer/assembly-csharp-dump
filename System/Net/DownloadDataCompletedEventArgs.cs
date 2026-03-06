using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.DownloadDataCompleted" /> event.</summary>
	public class DownloadDataCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DownloadDataCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets the data that is downloaded by a <see cref="Overload:System.Net.WebClient.DownloadDataAsync" /> method.</summary>
		/// <returns>A <see cref="T:System.Byte" /> array that contains the downloaded data.</returns>
		public byte[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal DownloadDataCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly byte[] _result;
	}
}
