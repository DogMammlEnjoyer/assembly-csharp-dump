using System;

namespace Fusion
{
	public struct RpcInfo
	{
		public static RpcInfo FromLocal(NetworkRunner runner, RpcChannel channel, RpcHostMode hostMode)
		{
			RpcInfo rpcInfo = new RpcInfo
			{
				Tick = runner.Simulation.Tick,
				Source = runner.Simulation.LocalPlayer,
				IsInvokeLocal = true,
				Channel = channel
			};
			bool flag = hostMode == RpcHostMode.SourceIsServer;
			if (flag)
			{
				bool flag2 = runner.Simulation.IsHostPlayer(rpcInfo.Source) && !runner.IsSinglePlayer;
				if (flag2)
				{
					rpcInfo.Source = default(PlayerRef);
				}
			}
			return rpcInfo;
		}

		public unsafe static RpcInfo FromMessage(NetworkRunner runner, SimulationMessage* message, RpcHostMode hostMode)
		{
			RpcInfo result = new RpcInfo
			{
				Tick = message->Tick,
				Source = message->Source,
				IsInvokeLocal = false,
				Channel = (message->GetFlag(8) ? RpcChannel.Unreliable : RpcChannel.Reliable)
			};
			bool flag = message->Source.IsNone && hostMode == RpcHostMode.SourceIsHostPlayer;
			if (flag)
			{
				PlayerRef source;
				bool flag2 = runner.Simulation.TryGetHostPlayer(out source);
				if (flag2)
				{
					result.Source = source;
				}
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("[RpcInfo: {0}={1}, {2}={3}, {4}={5}, {6}={7}]", new object[]
			{
				"Tick",
				this.Tick,
				"Source",
				this.Source,
				"IsInvokeLocal",
				this.IsInvokeLocal,
				"Channel",
				this.Channel
			});
		}

		public Tick Tick;

		public PlayerRef Source;

		public RpcChannel Channel;

		public bool IsInvokeLocal;
	}
}
