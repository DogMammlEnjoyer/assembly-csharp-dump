using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.DownloadStringCompleted" /> event.</summary>
	public class DownloadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DownloadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets the data that is downloaded by a <see cref="Overload:System.Net.WebClient.DownloadStringAsync" /> method.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the downloaded data.</returns>
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal DownloadStringCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly string _result;
	}
}
