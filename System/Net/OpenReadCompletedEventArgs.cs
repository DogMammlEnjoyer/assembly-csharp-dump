using System;
using System.ComponentModel;
using System.IO;
using Unity;

namespace System.Net
{
	/// <summary>Provides data for the <see cref="E:System.Net.WebClient.OpenReadCompleted" /> event.</summary>
	public class OpenReadCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenReadCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken) : base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		/// <summary>Gets a readable stream that contains data downloaded by a <see cref="Overload:System.Net.WebClient.DownloadDataAsync" /> method.</summary>
		/// <returns>A <see cref="T:System.IO.Stream" /> that contains the downloaded data.</returns>
		public Stream Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal OpenReadCompletedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly Stream _result;
	}
}
