using System;
using Unity;

namespace System.Net.NetworkInformation
{
	/// <summary>Provides data for the <see cref="E:System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged" /> event.</summary>
	public class NetworkAvailabilityEventArgs : EventArgs
	{
		internal NetworkAvailabilityEventArgs(bool isAvailable)
		{
			this.isAvailable = isAvailable;
		}

		/// <summary>Gets the current status of the network connection.</summary>
		/// <returns>
		///   <see langword="true" /> if the network is available; otherwise, <see langword="false" />.</returns>
		public bool IsAvailable
		{
			get
			{
				return this.isAvailable;
			}
		}

		internal NetworkAvailabilityEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool isAvailable;
	}
}
