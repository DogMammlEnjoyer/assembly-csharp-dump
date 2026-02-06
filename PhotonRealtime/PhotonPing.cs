using System;

namespace Photon.Realtime
{
	public abstract class PhotonPing : IDisposable
	{
		public virtual bool StartPing(string ip)
		{
			throw new NotImplementedException();
		}

		public virtual bool Done()
		{
			throw new NotImplementedException();
		}

		public virtual void Dispose()
		{
			throw new NotImplementedException();
		}

		protected internal void Init()
		{
			this.GotResult = false;
			this.Successful = false;
			this.PingId = (byte)PhotonPing.RandomIdProvider.Next(255);
		}

		public string DebugString = "";

		public bool Successful;

		protected internal bool GotResult;

		protected internal int PingLength = 13;

		protected internal byte[] PingBytes = new byte[]
		{
			125,
			125,
			125,
			125,
			125,
			125,
			125,
			125,
			125,
			125,
			125,
			125,
			0
		};

		protected internal byte PingId;

		private static readonly Random RandomIdProvider = new Random();
	}
}
