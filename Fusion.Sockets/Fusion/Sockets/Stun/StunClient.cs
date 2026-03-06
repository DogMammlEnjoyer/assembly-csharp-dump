using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fusion.Sockets.Stun
{
	internal static class StunClient
	{
		public static void Reset()
		{
			StunClient.PendingRequests.Clear();
		}

		[DebuggerStepThrough]
		public static Task<StunResult> QueryReflexiveInfo(NetAddress boundLocalAddress, Func<byte[], NetAddress, bool> sendDataViaSocket, NetAddress? customPublicAddress, string customStunServer = null, bool extendedAttempts = false, Func<bool> keepRunning = null)
		{
			StunClient.<QueryReflexiveInfo>d__3 <QueryReflexiveInfo>d__ = new StunClient.<QueryReflexiveInfo>d__3();
			<QueryReflexiveInfo>d__.<>t__builder = AsyncTaskMethodBuilder<StunResult>.Create();
			<QueryReflexiveInfo>d__.boundLocalAddress = boundLocalAddress;
			<QueryReflexiveInfo>d__.sendDataViaSocket = sendDataViaSocket;
			<QueryReflexiveInfo>d__.customPublicAddress = customPublicAddress;
			<QueryReflexiveInfo>d__.customStunServer = customStunServer;
			<QueryReflexiveInfo>d__.extendedAttempts = extendedAttempts;
			<QueryReflexiveInfo>d__.keepRunning = keepRunning;
			<QueryReflexiveInfo>d__.<>1__state = -1;
			<QueryReflexiveInfo>d__.<>t__builder.Start<StunClient.<QueryReflexiveInfo>d__3>(ref <QueryReflexiveInfo>d__);
			return <QueryReflexiveInfo>d__.<>t__builder.Task;
		}

		public unsafe static bool TryParseAndStoreStunMessage(NetAddress* origin, byte* buffer, int bufferLength)
		{
			StunMessage stunMessage = StunMessage.TryParse(buffer, bufferLength);
			bool flag = ((stunMessage != null) ? stunMessage.MappedAddress : null) == null;
			bool result;
			if (flag)
			{
				TraceLogStream logTraceStun = InternalLogStreams.LogTraceStun;
				if (logTraceStun != null)
				{
					logTraceStun.Log("Invalid STUN Message, no Mapped Address found.");
				}
				result = false;
			}
			else
			{
				ConcurrentDictionary<int, NetAddress> concurrentDictionary;
				bool flag2 = StunClient.PendingRequests.TryGetValue(stunMessage.ID, out concurrentDictionary);
				if (flag2)
				{
					int port = stunMessage.MappedAddress.Port;
					string ip = stunMessage.MappedAddress.Address.ToString();
					NetAddress netAddress = NetAddress.CreateFromIpPort(ip, (ushort)port);
					bool flag3 = netAddress.IsValid && concurrentDictionary.TryAdd(origin->GetHashCode(), netAddress);
					if (flag3)
					{
						TraceLogStream logTraceStun2 = InternalLogStreams.LogTraceStun;
						if (logTraceStun2 != null)
						{
							logTraceStun2.Log(string.Format("Reply received (ID={0}, STUN Server={1}): {2}", stunMessage.ID, origin->NativeAddress, netAddress.NativeAddress));
						}
						return true;
					}
				}
				TraceLogStream logTraceStun3 = InternalLogStreams.LogTraceStun;
				if (logTraceStun3 != null)
				{
					logTraceStun3.Log(string.Format("Capture STUN Message from {0}", origin->NativeAddress));
				}
				result = true;
			}
			return result;
		}

		private static bool QueryLocalAddress(NetAddress boundLocalAddress, out AddressFamily addressFamily, out NetAddress localAddress)
		{
			AddressFamily addressFamily2 = boundLocalAddress.IsIPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
			bool hasAddress = boundLocalAddress.HasAddress;
			bool result;
			if (hasAddress)
			{
				TraceLogStream logTraceStun = InternalLogStreams.LogTraceStun;
				if (logTraceStun != null)
				{
					logTraceStun.Log(string.Format("Using Local Address ({0}", boundLocalAddress));
				}
				localAddress = boundLocalAddress;
				addressFamily = addressFamily2;
				result = true;
			}
			else
			{
				localAddress = NetAddress.AnyIPv4Addr;
				addressFamily = addressFamily2;
				TraceLogStream logTraceStun2 = InternalLogStreams.LogTraceStun;
				if (logTraceStun2 != null)
				{
					logTraceStun2.Log(string.Format("Resolving Local Address ({0})", addressFamily2));
				}
				IPAddress ipaddress;
				bool localAddress2 = StunClient.GetLocalAddress(ref addressFamily, out ipaddress);
				if (localAddress2)
				{
					bool flag = addressFamily != addressFamily2;
					if (flag)
					{
						TraceLogStream logTraceStun3 = InternalLogStreams.LogTraceStun;
						if (logTraceStun3 != null)
						{
							logTraceStun3.Warn(string.Format("No Address of Family {0} found, changed to {1}", addressFamily2, addressFamily));
						}
					}
					try
					{
						NetAddress netAddress = NetAddress.CreateFromIpPort(ipaddress.ToString(), boundLocalAddress.NativeAddress.Port);
						bool isValid = netAddress.IsValid;
						if (isValid)
						{
							localAddress = netAddress;
							return true;
						}
					}
					catch
					{
					}
				}
				LogStream logWarn = InternalLogStreams.LogWarn;
				if (logWarn != null)
				{
					logWarn.Log("[STUN] Unable to resolve Local Address");
				}
				result = false;
			}
			return result;
		}

		private static bool QueryPublicAddress(Func<byte[], NetAddress, bool> sendAnyData, AddressFamily originalFamily, ref Guid requestID, out bool skipNATDiscovery)
		{
			skipNATDiscovery = false;
			bool flag = sendAnyData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = originalFamily == AddressFamily.InterNetworkV6;
				List<StunServers.StunServer> stunServer = StunServers.GetStunServer(flag2);
				bool flag3 = stunServer.Count == 0;
				if (flag3)
				{
					LogStream logWarn = InternalLogStreams.LogWarn;
					if (logWarn != null)
					{
						logWarn.Log("[STUN] Unable to find any valid STUN Server, aborting Reflexive Address query.");
					}
					result = false;
				}
				else
				{
					bool flag4 = stunServer.Count == 1;
					if (flag4)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Log("[STUN] Only one STUN Server found, skip NAT Type Discovery.");
						}
						skipNATDiscovery = true;
					}
					StunMessage stunMessage = new StunMessage(requestID, StunMessage.StunMessageType.BindingRequest);
					byte[] arg = stunMessage.Serialize();
					bool flag5 = false;
					foreach (StunServers.StunServer stunServer2 in stunServer)
					{
						try
						{
							NetAddress netAddress = flag2 ? stunServer2.IPv6Addr : stunServer2.IPv4Addr;
							bool flag6 = !netAddress.IsValid;
							if (!flag6)
							{
								bool flag7 = sendAnyData(arg, netAddress);
								if (flag7)
								{
									flag5 = true;
									TraceLogStream logTraceStun = InternalLogStreams.LogTraceStun;
									if (logTraceStun != null)
									{
										logTraceStun.Log(string.Format("Request sent to {0}", netAddress));
									}
								}
								else
								{
									TraceLogStream logTraceStun2 = InternalLogStreams.LogTraceStun;
									if (logTraceStun2 != null)
									{
										logTraceStun2.Warn(string.Format("Unable to send request to {0}", netAddress));
									}
								}
							}
						}
						catch (Exception message)
						{
							TraceLogStream logTraceStun3 = InternalLogStreams.LogTraceStun;
							if (logTraceStun3 != null)
							{
								logTraceStun3.Error(message);
							}
						}
					}
					bool flag8 = !flag5;
					if (flag8)
					{
						result = false;
					}
					else
					{
						requestID = stunMessage.ID;
						result = true;
					}
				}
			}
			return result;
		}

		private static bool GetLocalAddress(ref AddressFamily addressFamily, out IPAddress localIP)
		{
			localIP = IPAddress.None;
			try
			{
				using (Socket socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.IP))
				{
					socket.Connect((addressFamily == AddressFamily.InterNetwork) ? StunClient.TestIPs.TestNetIpv4 : StunClient.TestIPs.TestNetIpv6);
					localIP = ((IPEndPoint)socket.LocalEndPoint).Address;
				}
			}
			catch
			{
				bool flag = addressFamily != AddressFamily.InterNetworkV6;
				if (flag)
				{
					return false;
				}
				addressFamily = AddressFamily.InterNetwork;
				TraceLogStream logTraceStun = InternalLogStreams.LogTraceStun;
				if (logTraceStun != null)
				{
					logTraceStun.Warn("No Address of Family InterNetworkV6 found, changed to InterNetwork");
				}
				return StunClient.GetLocalAddress(ref addressFamily, out localIP);
			}
			return !localIP.Equals(IPAddress.None);
		}

		private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<int, NetAddress>> PendingRequests = new ConcurrentDictionary<Guid, ConcurrentDictionary<int, NetAddress>>();

		private static class TestIPs
		{
			public static readonly IPEndPoint TestNetIpv4 = new IPEndPoint(IPAddress.Parse("203.0.113.0"), 65530);

			public static readonly IPEndPoint TestNetIpv6 = new IPEndPoint(IPAddress.Parse("2001:db8::"), 65530);
		}
	}
}
