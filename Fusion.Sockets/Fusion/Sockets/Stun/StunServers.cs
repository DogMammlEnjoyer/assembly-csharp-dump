using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fusion.Sockets.Stun
{
	internal static class StunServers
	{
		public static List<StunServers.StunServer> GetStunServer(bool IPv6Support)
		{
			List<StunServers.StunServer> list = new List<StunServers.StunServer>();
			bool flag = StunServers._stunServers == null;
			List<StunServers.StunServer> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				foreach (StunServers.StunServer stunServer in StunServers._stunServers)
				{
					bool flag2 = stunServer != null && stunServer.HasIPv4Support && (!IPv6Support || stunServer.HasIPv6Support);
					if (flag2)
					{
						list.Add(stunServer);
					}
				}
				result = list;
			}
			return result;
		}

		[DebuggerStepThrough]
		public static Task SetupStunServers(string customStunServer = null)
		{
			StunServers.<SetupStunServers>d__6 <SetupStunServers>d__ = new StunServers.<SetupStunServers>d__6();
			<SetupStunServers>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetupStunServers>d__.customStunServer = customStunServer;
			<SetupStunServers>d__.<>1__state = -1;
			<SetupStunServers>d__.<>t__builder.Start<StunServers.<SetupStunServers>d__6>(ref <SetupStunServers>d__);
			return <SetupStunServers>d__.<>t__builder.Task;
		}

		[DebuggerStepThrough]
		private static Task<StunServers.StunServer> ResolveStunServerInfo(string stunServerAddress)
		{
			StunServers.<ResolveStunServerInfo>d__7 <ResolveStunServerInfo>d__ = new StunServers.<ResolveStunServerInfo>d__7();
			<ResolveStunServerInfo>d__.<>t__builder = AsyncTaskMethodBuilder<StunServers.StunServer>.Create();
			<ResolveStunServerInfo>d__.stunServerAddress = stunServerAddress;
			<ResolveStunServerInfo>d__.<>1__state = -1;
			<ResolveStunServerInfo>d__.<>t__builder.Start<StunServers.<ResolveStunServerInfo>d__7>(ref <ResolveStunServerInfo>d__);
			return <ResolveStunServerInfo>d__.<>t__builder.Task;
		}

		private static readonly string[] DefaultStunServerList = new string[]
		{
			"stun1.l.google.com:19302",
			"stun2.l.google.com:19302",
			"stun3.l.google.com:19302",
			"stun4.l.google.com:19302"
		};

		private static volatile StunServers.StunServer[] _stunServers;

		private static volatile bool _runningResolution;

		public class StunServer
		{
			public bool HasIPv4Support
			{
				get
				{
					return this.IPv4Addr.IsValid;
				}
			}

			public bool HasIPv6Support
			{
				get
				{
					return this.IPv6Addr.IsValid;
				}
			}

			public override string ToString()
			{
				return string.Format("[{0}: {1}: {2}, {3}: {4}]", new object[]
				{
					"StunServer",
					"IPv4Addr",
					this.IPv4Addr,
					"IPv6Addr",
					this.IPv6Addr
				});
			}

			public static IEqualityComparer<StunServers.StunServer> StunServerEqualityComparer { get; } = new StunServers.StunServer.Pv4AddrEqualityComparer();

			public NetAddress IPv4Addr;

			public NetAddress IPv6Addr;

			private sealed class Pv4AddrEqualityComparer : IEqualityComparer<StunServers.StunServer>
			{
				public bool Equals(StunServers.StunServer x, StunServers.StunServer y)
				{
					bool flag = x == y;
					bool result;
					if (flag)
					{
						result = true;
					}
					else
					{
						bool flag2 = x == null;
						if (flag2)
						{
							result = false;
						}
						else
						{
							bool flag3 = y == null;
							if (flag3)
							{
								result = false;
							}
							else
							{
								bool flag4 = x.GetType() != y.GetType();
								result = (!flag4 && x.IPv4Addr.Equals(y.IPv4Addr));
							}
						}
					}
					return result;
				}

				public int GetHashCode(StunServers.StunServer obj)
				{
					return obj.IPv4Addr.GetHashCode();
				}
			}
		}
	}
}
