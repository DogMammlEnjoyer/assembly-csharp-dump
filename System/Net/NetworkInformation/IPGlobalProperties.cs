using System;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace System.Net.NetworkInformation
{
	/// <summary>Provides information about the network connectivity of the local computer.</summary>
	public abstract class IPGlobalProperties
	{
		/// <summary>Gets an object that provides information about the local computer's network connectivity and traffic statistics.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.IPGlobalProperties" /> object that contains information about the local computer.</returns>
		public static IPGlobalProperties GetIPGlobalProperties()
		{
			return IPGlobalPropertiesFactoryPal.Create();
		}

		internal static IPGlobalProperties InternalGetIPGlobalProperties()
		{
			return IPGlobalProperties.GetIPGlobalProperties();
		}

		/// <summary>Returns information about the Internet Protocol version 4 (IPv4) and IPv6 User Datagram Protocol (UDP) listeners on the local computer.</summary>
		/// <returns>An <see cref="T:System.Net.IPEndPoint" /> array that contains objects that describe the UDP listeners, or an empty array if no UDP listeners are detected.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function <see langword="GetUdpTable" /> failed.</exception>
		public abstract IPEndPoint[] GetActiveUdpListeners();

		/// <summary>Returns endpoint information about the Internet Protocol version 4 (IPv4) and IPv6 Transmission Control Protocol (TCP) listeners on the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.IPEndPoint" /> array that contains objects that describe the active TCP listeners, or an empty array, if no active TCP listeners are detected.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The Win32 function <see langword="GetTcpTable" /> failed.</exception>
		public abstract IPEndPoint[] GetActiveTcpListeners();

		/// <summary>Returns information about the Internet Protocol version 4 (IPv4) and IPv6 Transmission Control Protocol (TCP) connections on the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.TcpConnectionInformation" /> array that contains objects that describe the active TCP connections, or an empty array if no active TCP connections are detected.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The Win32 function <see langword="GetTcpTable" /> failed.</exception>
		public abstract TcpConnectionInformation[] GetActiveTcpConnections();

		/// <summary>Gets the Dynamic Host Configuration Protocol (DHCP) scope name.</summary>
		/// <returns>A <see cref="T:System.String" /> instance that contains the computer's DHCP scope name.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">A Win32 function call failed.</exception>
		public abstract string DhcpScopeName { get; }

		/// <summary>Gets the domain in which the local computer is registered.</summary>
		/// <returns>A <see cref="T:System.String" /> instance that contains the computer's domain name. If the computer does not belong to a domain, returns <see cref="F:System.String.Empty" />.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">A Win32 function call failed.</exception>
		public abstract string DomainName { get; }

		/// <summary>Gets the host name for the local computer.</summary>
		/// <returns>A <see cref="T:System.String" /> instance that contains the computer's NetBIOS name.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">A Win32 function call failed.</exception>
		public abstract string HostName { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that specifies whether the local computer is acting as a Windows Internet Name Service (WINS) proxy.</summary>
		/// <returns>
		///   <see langword="true" /> if the local computer is a WINS proxy; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">A Win32 function call failed.</exception>
		public abstract bool IsWinsProxy { get; }

		/// <summary>Gets the Network Basic Input/Output System (NetBIOS) node type of the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.NetBiosNodeType" /> value.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">A Win32 function call failed.</exception>
		public abstract NetBiosNodeType NodeType { get; }

		/// <summary>Provides Transmission Control Protocol/Internet Protocol version 4 (TCP/IPv4) statistical data for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.TcpStatistics" /> object that provides TCP/IPv4 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function <see langword="GetTcpStatistics" /> failed.</exception>
		public abstract TcpStatistics GetTcpIPv4Statistics();

		/// <summary>Provides Transmission Control Protocol/Internet Protocol version 6 (TCP/IPv6) statistical data for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.TcpStatistics" /> object that provides TCP/IPv6 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function <see langword="GetTcpStatistics" /> failed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The local computer is not running an operating system that supports IPv6.</exception>
		public abstract TcpStatistics GetTcpIPv6Statistics();

		/// <summary>Provides User Datagram Protocol/Internet Protocol version 4 (UDP/IPv4) statistical data for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.UdpStatistics" /> object that provides UDP/IPv4 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function GetUdpStatistics failed.</exception>
		public abstract UdpStatistics GetUdpIPv4Statistics();

		/// <summary>Provides User Datagram Protocol/Internet Protocol version 6 (UDP/IPv6) statistical data for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.UdpStatistics" /> object that provides UDP/IPv6 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function <see langword="GetUdpStatistics" /> failed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The local computer is not running an operating system that supports IPv6.</exception>
		public abstract UdpStatistics GetUdpIPv6Statistics();

		/// <summary>Provides Internet Control Message Protocol (ICMP) version 4 statistical data for the local computer.</summary>
		/// <returns>An <see cref="T:System.Net.NetworkInformation.IcmpV4Statistics" /> object that provides ICMP version 4 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The Win32 function <see langword="GetIcmpStatistics" /> failed.</exception>
		public abstract IcmpV4Statistics GetIcmpV4Statistics();

		/// <summary>Provides Internet Control Message Protocol (ICMP) version 6 statistical data for the local computer.</summary>
		/// <returns>An <see cref="T:System.Net.NetworkInformation.IcmpV6Statistics" /> object that provides ICMP version 6 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The Win32 function <see langword="GetIcmpStatisticsEx" /> failed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The local computer's operating system is not Windows XP or later.</exception>
		public abstract IcmpV6Statistics GetIcmpV6Statistics();

		/// <summary>Provides Internet Protocol version 4 (IPv4) statistical data for the local computer.</summary>
		/// <returns>An <see cref="T:System.Net.NetworkInformation.IPGlobalStatistics" /> object that provides IPv4 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function <see langword="GetIpStatistics" /> failed.</exception>
		public abstract IPGlobalStatistics GetIPv4GlobalStatistics();

		/// <summary>Provides Internet Protocol version 6 (IPv6) statistical data for the local computer.</summary>
		/// <returns>An <see cref="T:System.Net.NetworkInformation.IPGlobalStatistics" /> object that provides IPv6 traffic statistics for the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the Win32 function <see langword="GetIpStatistics" /> failed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The local computer is not running an operating system that supports IPv6.</exception>
		public abstract IPGlobalStatistics GetIPv6GlobalStatistics();

		/// <summary>Retrieves the stable unicast IP address table on the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.UnicastIPAddressInformationCollection" /> that contains a list of stable unicast IP addresses on the local computer.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the native <see langword="GetAdaptersAddresses" /> function failed.</exception>
		/// <exception cref="T:System.NotImplementedException">This method is not implemented on the platform. This method uses the native <see langword="NotifyStableUnicastIpAddressTable" /> function that is supported on Windows Vista and later.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have necessary <see cref="F:System.Net.NetworkInformation.NetworkInformationAccess.Read" /> permission.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The call to the native <see langword="NotifyStableUnicastIpAddressTable" /> function failed.</exception>
		public virtual UnicastIPAddressInformationCollection GetUnicastAddresses()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>Begins an asynchronous request to retrieve the stable unicast IP address table on the local computer.</summary>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous request.</returns>
		/// <exception cref="T:System.NotImplementedException">This method is not implemented on the platform. This method uses the native <see langword="NotifyStableUnicastIpAddressTable" /> function that is supported on Windows Vista and later.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The call to the native <see langword="NotifyStableUnicastIpAddressTable" /> function failed.</exception>
		public virtual IAsyncResult BeginGetUnicastAddresses(AsyncCallback callback, object state)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>Ends a pending asynchronous request to retrieve the stable unicast IP address table on the local computer.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that references the asynchronous request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the native <see langword="GetAdaptersAddresses" /> function failed.</exception>
		/// <exception cref="T:System.NotImplementedException">This method is not implemented on the platform. This method uses the native <see langword="NotifyStableUnicastIpAddressTable" /> function that is supported on Windows Vista and later.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have necessary <see cref="F:System.Net.NetworkInformation.NetworkInformationAccess.Read" /> permission.</exception>
		public virtual UnicastIPAddressInformationCollection EndGetUnicastAddresses(IAsyncResult asyncResult)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>Retrieves the stable unicast IP address table on the local computer as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.Net.NetworkInformation.NetworkInformationException">The call to the native <see langword="GetAdaptersAddresses" /> function failed.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have necessary <see cref="F:System.Net.NetworkInformation.NetworkInformationAccess.Read" /> permission.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The call to the native <see langword="NotifyStableUnicastIpAddressTable" /> function failed.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<UnicastIPAddressInformationCollection> GetUnicastAddressesAsync()
		{
			return Task<UnicastIPAddressInformationCollection>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetUnicastAddresses), new Func<IAsyncResult, UnicastIPAddressInformationCollection>(this.EndGetUnicastAddresses), null);
		}
	}
}
