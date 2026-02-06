using System;
using Fusion.Sockets;

namespace Fusion
{
	public static class NetworkRunnerCallbackArgs
	{
		public class ConnectRequest
		{
			public NetAddress RemoteAddress { get; set; }

			public void Accept()
			{
				this.Result = new OnConnectionRequestReply?(OnConnectionRequestReply.Ok);
			}

			public void Refuse()
			{
				this.Result = new OnConnectionRequestReply?(OnConnectionRequestReply.Refuse);
			}

			public void Waiting()
			{
				this.Result = new OnConnectionRequestReply?(OnConnectionRequestReply.Waiting);
			}

			internal OnConnectionRequestReply? Result;
		}
	}
}
