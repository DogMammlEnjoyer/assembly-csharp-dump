using System;

namespace System.Net.NetworkInformation
{
	internal static class SystemNetworkInterface
	{
		public static NetworkInterface[] GetNetworkInterfaces()
		{
			NetworkInterface[] result;
			try
			{
				result = SystemNetworkInterface.nif.GetAllNetworkInterfaces();
			}
			catch
			{
				result = new NetworkInterface[0];
			}
			return result;
		}

		public static bool InternalGetIsNetworkAvailable()
		{
			return true;
		}

		public static int InternalLoopbackInterfaceIndex
		{
			get
			{
				return SystemNetworkInterface.nif.GetLoopbackInterfaceIndex();
			}
		}

		public static int InternalIPv6LoopbackInterfaceIndex
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static IPAddress GetNetMask(IPAddress address)
		{
			return SystemNetworkInterface.nif.GetNetMask(address);
		}

		private static readonly NetworkInterfaceFactory nif = NetworkInterfaceFactory.Create();
	}
}
