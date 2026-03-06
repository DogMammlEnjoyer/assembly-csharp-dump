using System;
using System.ComponentModel;
using Unity;

namespace System.Net.NetworkInformation
{
	/// <summary>Allows applications to receive notification when the Internet Protocol (IP) address of a network interface, also called a network card or adapter, changes.</summary>
	public sealed class NetworkChange
	{
		/// <summary>Occurs when the IP address of a network interface changes.</summary>
		public static event NetworkAddressChangedEventHandler NetworkAddressChanged
		{
			add
			{
				Type typeFromHandle = typeof(INetworkChange);
				lock (typeFromHandle)
				{
					NetworkChange.MaybeCreate();
					if (NetworkChange.networkChange != null)
					{
						NetworkChange.networkChange.NetworkAddressChanged += value;
					}
				}
			}
			remove
			{
				Type typeFromHandle = typeof(INetworkChange);
				lock (typeFromHandle)
				{
					if (NetworkChange.networkChange != null)
					{
						NetworkChange.networkChange.NetworkAddressChanged -= value;
						NetworkChange.MaybeDispose();
					}
				}
			}
		}

		/// <summary>Occurs when the availability of the network changes.</summary>
		public static event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged
		{
			add
			{
				Type typeFromHandle = typeof(INetworkChange);
				lock (typeFromHandle)
				{
					NetworkChange.MaybeCreate();
					if (NetworkChange.networkChange != null)
					{
						NetworkChange.networkChange.NetworkAvailabilityChanged += value;
					}
				}
			}
			remove
			{
				Type typeFromHandle = typeof(INetworkChange);
				lock (typeFromHandle)
				{
					if (NetworkChange.networkChange != null)
					{
						NetworkChange.networkChange.NetworkAvailabilityChanged -= value;
						NetworkChange.MaybeDispose();
					}
				}
			}
		}

		private static void MaybeCreate()
		{
			if (NetworkChange.networkChange != null)
			{
				return;
			}
			if (NetworkChange.IsWindows)
			{
				throw new PlatformNotSupportedException("NetworkInformation.NetworkChange is not supported on the current platform.");
			}
			try
			{
				NetworkChange.networkChange = new MacNetworkChange();
			}
			catch
			{
				NetworkChange.networkChange = new LinuxNetworkChange();
			}
		}

		private static bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				return platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE;
			}
		}

		private static void MaybeDispose()
		{
			if (NetworkChange.networkChange != null && NetworkChange.networkChange.HasRegisteredEvents)
			{
				NetworkChange.networkChange.Dispose();
				NetworkChange.networkChange = null;
			}
		}

		/// <summary>Registers a network change instance to receive network change events.</summary>
		/// <param name="nc">The instance to register.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public static void RegisterNetworkChange(NetworkChange nc)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private static INetworkChange networkChange;
	}
}
