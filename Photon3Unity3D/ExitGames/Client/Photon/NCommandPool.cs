using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	internal class NCommandPool
	{
		public NCommand Acquire(EnetPeer peer, byte[] inBuff, ref int readingOffset)
		{
			Stack<NCommand> obj = this.pool;
			NCommand ncommand;
			lock (obj)
			{
				bool flag2 = this.pool.Count == 0;
				if (flag2)
				{
					ncommand = new NCommand(peer, inBuff, ref readingOffset);
					ncommand.returnPool = this;
				}
				else
				{
					ncommand = this.pool.Pop();
					ncommand.Initialize(peer, inBuff, ref readingOffset);
				}
			}
			return ncommand;
		}

		public NCommand Acquire(EnetPeer peer, byte commandType, StreamBuffer payload, byte channel)
		{
			Stack<NCommand> obj = this.pool;
			NCommand ncommand;
			lock (obj)
			{
				bool flag2 = this.pool.Count == 0;
				if (flag2)
				{
					ncommand = new NCommand(peer, commandType, payload, channel);
					ncommand.returnPool = this;
				}
				else
				{
					ncommand = this.pool.Pop();
					ncommand.Initialize(peer, commandType, payload, channel);
				}
			}
			return ncommand;
		}

		public void Release(NCommand nCommand)
		{
			nCommand.Reset();
			Stack<NCommand> obj = this.pool;
			lock (obj)
			{
				this.pool.Push(nCommand);
			}
		}

		private readonly Stack<NCommand> pool = new Stack<NCommand>();
	}
}
