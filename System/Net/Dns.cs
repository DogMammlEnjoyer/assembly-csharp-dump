using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Mono.Net.Dns;

namespace System.Net
{
	/// <summary>Provides simple domain name resolution functionality.</summary>
	public static class Dns
	{
		static Dns()
		{
			if (Environment.GetEnvironmentVariable("MONO_DNS") != null)
			{
				Dns.resolver = new SimpleResolver();
				Dns.use_mono_dns = true;
			}
		}

		internal static bool UseMonoDns
		{
			get
			{
				return Dns.use_mono_dns;
			}
		}

		private static void OnCompleted(object sender, SimpleResolverEventArgs e)
		{
			DnsAsyncResult dnsAsyncResult = (DnsAsyncResult)e.UserToken;
			IPHostEntry hostEntry = e.HostEntry;
			if (hostEntry == null || e.ResolverError != ResolverError.NoError)
			{
				dnsAsyncResult.SetCompleted(false, new Exception("Error: " + e.ResolverError.ToString()));
				return;
			}
			dnsAsyncResult.SetCompleted(false, hostEntry);
		}

		private static IAsyncResult BeginAsyncCallAddresses(string host, AsyncCallback callback, object state)
		{
			SimpleResolverEventArgs simpleResolverEventArgs = new SimpleResolverEventArgs();
			simpleResolverEventArgs.Completed += Dns.OnCompleted;
			simpleResolverEventArgs.HostName = host;
			DnsAsyncResult dnsAsyncResult = new DnsAsyncResult(callback, state);
			simpleResolverEventArgs.UserToken = dnsAsyncResult;
			if (!Dns.resolver.GetHostAddressesAsync(simpleResolverEventArgs))
			{
				dnsAsyncResult.SetCompleted(true, simpleResolverEventArgs.HostEntry);
			}
			return dnsAsyncResult;
		}

		private static IAsyncResult BeginAsyncCall(string host, AsyncCallback callback, object state)
		{
			SimpleResolverEventArgs simpleResolverEventArgs = new SimpleResolverEventArgs();
			simpleResolverEventArgs.Completed += Dns.OnCompleted;
			simpleResolverEventArgs.HostName = host;
			DnsAsyncResult dnsAsyncResult = new DnsAsyncResult(callback, state);
			simpleResolverEventArgs.UserToken = dnsAsyncResult;
			if (!Dns.resolver.GetHostEntryAsync(simpleResolverEventArgs))
			{
				dnsAsyncResult.SetCompleted(true, simpleResolverEventArgs.HostEntry);
			}
			return dnsAsyncResult;
		}

		private static IPHostEntry EndAsyncCall(DnsAsyncResult ares)
		{
			if (ares == null)
			{
				throw new ArgumentException("Invalid asyncResult");
			}
			if (!ares.IsCompleted)
			{
				ares.AsyncWaitHandle.WaitOne();
			}
			if (ares.Exception != null)
			{
				throw ares.Exception;
			}
			IPHostEntry hostEntry = ares.HostEntry;
			if (hostEntry == null || hostEntry.AddressList == null || hostEntry.AddressList.Length == 0)
			{
				Dns.Error_11001(hostEntry.HostName);
			}
			return hostEntry;
		}

		/// <summary>Begins an asynchronous request for <see cref="T:System.Net.IPHostEntry" /> information about the specified DNS host name.</summary>
		/// <param name="hostName">The DNS name of the host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">A user-defined object that contains information about the operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that references the asynchronous request.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error was encountered executing the DNS query.</exception>
		[Obsolete("Use BeginGetHostEntry instead")]
		public static IAsyncResult BeginGetHostByName(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.BeginAsyncCall(hostName, requestCallback, stateObject);
			}
			return new Dns.GetHostByNameCallback(Dns.GetHostByName).BeginInvoke(hostName, requestCallback, stateObject);
		}

		/// <summary>Begins an asynchronous request to resolve a DNS host name or IP address to an <see cref="T:System.Net.IPAddress" /> instance.</summary>
		/// <param name="hostName">The DNS name of the host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">A user-defined object that contains information about the operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that references the asynchronous request.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">The caller does not have permission to access DNS information.</exception>
		[Obsolete("Use BeginGetHostEntry instead")]
		public static IAsyncResult BeginResolve(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.BeginAsyncCall(hostName, requestCallback, stateObject);
			}
			return new Dns.ResolveCallback(Dns.Resolve).BeginInvoke(hostName, requestCallback, stateObject);
		}

		/// <summary>Asynchronously returns the Internet Protocol (IP) addresses for the specified host.</summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that references the asynchronous request.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostNameOrAddress" /> is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="hostNameOrAddress" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hostNameOrAddress" /> is an invalid IP address.</exception>
		public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object state)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.BeginAsyncCallAddresses(hostNameOrAddress, requestCallback, state);
			}
			return new Dns.GetHostAddressesCallback(Dns.GetHostAddresses).BeginInvoke(hostNameOrAddress, requestCallback, state);
		}

		/// <summary>Asynchronously resolves a host name or IP address to an <see cref="T:System.Net.IPHostEntry" /> instance.</summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">A user-defined object that contains information about the operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that references the asynchronous request.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostNameOrAddress" /> is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="hostNameOrAddress" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hostNameOrAddress" /> is an invalid IP address.</exception>
		public static IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.BeginAsyncCall(hostNameOrAddress, requestCallback, stateObject);
			}
			return new Dns.GetHostEntryNameCallback(Dns.GetHostEntry).BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		/// <summary>Asynchronously resolves an IP address to an <see cref="T:System.Net.IPHostEntry" /> instance.</summary>
		/// <param name="address">The IP address to resolve.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="stateObject">A user-defined object that contains information about the operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that references the asynchronous request.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="address" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="address" /> is an invalid IP address.</exception>
		public static IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback requestCallback, object stateObject)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.BeginAsyncCall(address.ToString(), requestCallback, stateObject);
			}
			return new Dns.GetHostEntryIPCallback(Dns.GetHostEntry).BeginInvoke(address, requestCallback, stateObject);
		}

		/// <summary>Ends an asynchronous request for DNS information.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance that is returned by a call to the <see cref="M:System.Net.Dns.BeginGetHostByName(System.String,System.AsyncCallback,System.Object)" /> method.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> object that contains DNS information about a host.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		[Obsolete("Use EndGetHostEntry instead")]
		public static IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.EndAsyncCall(asyncResult as DnsAsyncResult);
			}
			return ((Dns.GetHostByNameCallback)((AsyncResult)asyncResult).AsyncDelegate).EndInvoke(asyncResult);
		}

		/// <summary>Ends an asynchronous request for DNS information.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance that is returned by a call to the <see cref="M:System.Net.Dns.BeginResolve(System.String,System.AsyncCallback,System.Object)" /> method.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> object that contains DNS information about a host.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		[Obsolete("Use EndGetHostEntry instead")]
		public static IPHostEntry EndResolve(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.EndAsyncCall(asyncResult as DnsAsyncResult);
			}
			return ((Dns.ResolveCallback)((AsyncResult)asyncResult).AsyncDelegate).EndInvoke(asyncResult);
		}

		/// <summary>Ends an asynchronous request for DNS information.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance returned by a call to the <see cref="M:System.Net.Dns.BeginGetHostAddresses(System.String,System.AsyncCallback,System.Object)" /> method.</param>
		/// <returns>An array of type <see cref="T:System.Net.IPAddress" /> that holds the IP addresses for the host specified by the <paramref name="hostNameOrAddress" /> parameter of <see cref="M:System.Net.Dns.BeginGetHostAddresses(System.String,System.AsyncCallback,System.Object)" />.</returns>
		public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!Dns.use_mono_dns)
			{
				return ((Dns.GetHostAddressesCallback)((AsyncResult)asyncResult).AsyncDelegate).EndInvoke(asyncResult);
			}
			IPHostEntry iphostEntry = Dns.EndAsyncCall(asyncResult as DnsAsyncResult);
			if (iphostEntry == null)
			{
				return null;
			}
			return iphostEntry.AddressList;
		}

		/// <summary>Ends an asynchronous request for DNS information.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance returned by a call to an <see cref="Overload:System.Net.Dns.BeginGetHostEntry" /> method.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> instance that contains address information about the host.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		public static IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (Dns.use_mono_dns)
			{
				return Dns.EndAsyncCall(asyncResult as DnsAsyncResult);
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			if (asyncResult2.AsyncDelegate is Dns.GetHostEntryIPCallback)
			{
				return ((Dns.GetHostEntryIPCallback)asyncResult2.AsyncDelegate).EndInvoke(asyncResult);
			}
			return ((Dns.GetHostEntryNameCallback)asyncResult2.AsyncDelegate).EndInvoke(asyncResult);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostByName_icall(string host, out string h_name, out string[] h_aliases, out string[] h_addr_list, int hint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostByAddr_icall(string addr, out string h_name, out string[] h_aliases, out string[] h_addr_list, int hint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostName_icall(out string h_name);

		private static void Error_11001(string hostName)
		{
			throw new SocketException(11001, string.Format("Could not resolve host '{0}'", hostName));
		}

		private static IPHostEntry hostent_to_IPHostEntry(string originalHostName, string h_name, string[] h_aliases, string[] h_addrlist)
		{
			IPHostEntry iphostEntry = new IPHostEntry();
			ArrayList arrayList = new ArrayList();
			iphostEntry.HostName = h_name;
			iphostEntry.Aliases = h_aliases;
			for (int i = 0; i < h_addrlist.Length; i++)
			{
				try
				{
					IPAddress ipaddress = IPAddress.Parse(h_addrlist[i]);
					if ((Socket.OSSupportsIPv6 && ipaddress.AddressFamily == AddressFamily.InterNetworkV6) || (Socket.OSSupportsIPv4 && ipaddress.AddressFamily == AddressFamily.InterNetwork))
					{
						arrayList.Add(ipaddress);
					}
				}
				catch (ArgumentNullException)
				{
				}
			}
			if (arrayList.Count == 0)
			{
				Dns.Error_11001(originalHostName);
			}
			iphostEntry.AddressList = (arrayList.ToArray(typeof(IPAddress)) as IPAddress[]);
			return iphostEntry;
		}

		/// <summary>Creates an <see cref="T:System.Net.IPHostEntry" /> instance from the specified <see cref="T:System.Net.IPAddress" />.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" />.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="address" />.</exception>
		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByAddress(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return Dns.GetHostByAddressFromString(address.ToString(), false);
		}

		/// <summary>Creates an <see cref="T:System.Net.IPHostEntry" /> instance from an IP address.</summary>
		/// <param name="address">An IP address.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="address" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByAddress(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return Dns.GetHostByAddressFromString(address, true);
		}

		private static IPHostEntry GetHostByAddressFromString(string address, bool parse)
		{
			if (address.Equals("0.0.0.0"))
			{
				address = "127.0.0.1";
				parse = false;
			}
			if (parse)
			{
				IPAddress.Parse(address);
			}
			string h_name;
			string[] h_aliases;
			string[] h_addrlist;
			if (!Dns.GetHostByAddr_icall(address, out h_name, out h_aliases, out h_addrlist, Socket.FamilyHint))
			{
				Dns.Error_11001(address);
			}
			return Dns.hostent_to_IPHostEntry(address, h_name, h_aliases, h_addrlist);
		}

		/// <summary>Resolves a host name or IP address to an <see cref="T:System.Net.IPHostEntry" /> instance.</summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> instance that contains address information about the host specified in <paramref name="hostNameOrAddress" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="hostNameOrAddress" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostNameOrAddress" /> parameter is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error was encountered when resolving the <paramref name="hostNameOrAddress" /> parameter.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="hostNameOrAddress" /> parameter is an invalid IP address.</exception>
		public static IPHostEntry GetHostEntry(string hostNameOrAddress)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostNameOrAddress");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			IPAddress address;
			if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out address))
			{
				return Dns.GetHostEntry(address);
			}
			return Dns.GetHostByName(hostNameOrAddress);
		}

		/// <summary>Resolves an IP address to an <see cref="T:System.Net.IPHostEntry" /> instance.</summary>
		/// <param name="address">An IP address.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> instance that contains address information about the host specified in <paramref name="address" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="address" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="address" /> is an invalid IP address.</exception>
		public static IPHostEntry GetHostEntry(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return Dns.GetHostByAddressFromString(address.ToString(), false);
		}

		/// <summary>Returns the Internet Protocol (IP) addresses for the specified host.</summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <returns>An array of type <see cref="T:System.Net.IPAddress" /> that holds the IP addresses for the host that is specified by the <paramref name="hostNameOrAddress" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostNameOrAddress" /> is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="hostNameOrAddress" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hostNameOrAddress" /> is an invalid IP address.</exception>
		public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostNameOrAddress");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			IPAddress ipaddress;
			if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out ipaddress))
			{
				return new IPAddress[]
				{
					ipaddress
				};
			}
			return Dns.GetHostEntry(hostNameOrAddress).AddressList;
		}

		/// <summary>Gets the DNS information for the specified DNS host name.</summary>
		/// <param name="hostName">The DNS name of the host.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> object that contains host information for the address specified in <paramref name="hostName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostName" /> is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="hostName" />.</exception>
		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByName(string hostName)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			string h_name;
			string[] h_aliases;
			string[] h_addrlist;
			if (!Dns.GetHostByName_icall(hostName, out h_name, out h_aliases, out h_addrlist, Socket.FamilyHint))
			{
				Dns.Error_11001(hostName);
			}
			return Dns.hostent_to_IPHostEntry(hostName, h_name, h_aliases, h_addrlist);
		}

		/// <summary>Gets the host name of the local computer.</summary>
		/// <returns>A string that contains the DNS host name of the local computer.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving the local host name.</exception>
		public static string GetHostName()
		{
			string text;
			if (!Dns.GetHostName_icall(out text))
			{
				Dns.Error_11001(text);
			}
			return text;
		}

		/// <summary>Resolves a DNS host name or IP address to an <see cref="T:System.Net.IPHostEntry" /> instance.</summary>
		/// <param name="hostName">A DNS-style host name or IP address.</param>
		/// <returns>An <see cref="T:System.Net.IPHostEntry" /> instance that contains address information about the host specified in <paramref name="hostName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostName" /> is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="hostName" />.</exception>
		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry Resolve(string hostName)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			IPHostEntry iphostEntry = null;
			try
			{
				iphostEntry = Dns.GetHostByAddress(hostName);
			}
			catch
			{
			}
			if (iphostEntry == null)
			{
				iphostEntry = Dns.GetHostByName(hostName);
			}
			return iphostEntry;
		}

		/// <summary>Returns the Internet Protocol (IP) addresses for the specified host as an asynchronous operation.</summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an array of type <see cref="T:System.Net.IPAddress" /> that holds the IP addresses for the host that is specified by the <paramref name="hostNameOrAddress" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostNameOrAddress" /> is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="hostNameOrAddress" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hostNameOrAddress" /> is an invalid IP address.</exception>
		public static Task<IPAddress[]> GetHostAddressesAsync(string hostNameOrAddress)
		{
			return Task<IPAddress[]>.Factory.FromAsync<string>(new Func<string, AsyncCallback, object, IAsyncResult>(Dns.BeginGetHostAddresses), new Func<IAsyncResult, IPAddress[]>(Dns.EndGetHostAddresses), hostNameOrAddress, null);
		}

		/// <summary>Resolves an IP address to an <see cref="T:System.Net.IPHostEntry" /> instance as an asynchronous operation.</summary>
		/// <param name="address">An IP address.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.IPHostEntry" /> instance that contains address information about the host specified in <paramref name="address" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error is encountered when resolving <paramref name="address" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="address" /> is an invalid IP address.</exception>
		public static Task<IPHostEntry> GetHostEntryAsync(IPAddress address)
		{
			return Task<IPHostEntry>.Factory.FromAsync<IPAddress>(new Func<IPAddress, AsyncCallback, object, IAsyncResult>(Dns.BeginGetHostEntry), new Func<IAsyncResult, IPHostEntry>(Dns.EndGetHostEntry), address, null);
		}

		/// <summary>Resolves a host name or IP address to an <see cref="T:System.Net.IPHostEntry" /> instance as an asynchronous operation.</summary>
		/// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.IPHostEntry" /> instance that contains address information about the host specified in <paramref name="hostNameOrAddress" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="hostNameOrAddress" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of <paramref name="hostNameOrAddress" /> parameter is greater than 255 characters.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error was encountered when resolving the <paramref name="hostNameOrAddress" /> parameter.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="hostNameOrAddress" /> parameter is an invalid IP address.</exception>
		public static Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress)
		{
			return Task<IPHostEntry>.Factory.FromAsync<string>(new Func<string, AsyncCallback, object, IAsyncResult>(Dns.BeginGetHostEntry), new Func<IAsyncResult, IPHostEntry>(Dns.EndGetHostEntry), hostNameOrAddress, null);
		}

		private static bool use_mono_dns;

		private static SimpleResolver resolver;

		private delegate IPHostEntry GetHostByNameCallback(string hostName);

		private delegate IPHostEntry ResolveCallback(string hostName);

		private delegate IPHostEntry GetHostEntryNameCallback(string hostName);

		private delegate IPHostEntry GetHostEntryIPCallback(IPAddress hostAddress);

		private delegate IPAddress[] GetHostAddressesCallback(string hostName);
	}
}
