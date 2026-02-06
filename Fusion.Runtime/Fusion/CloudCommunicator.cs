using System;
using Fusion.Photon.Realtime;
using Fusion.Protocol;

namespace Fusion
{
	internal class CloudCommunicator : CommunicatorBase, IDisposable
	{
		public FusionRelayClient Client { get; private set; }

		public override int CommunicatorID
		{
			get
			{
				return (this.Client != null) ? this.Client.LocalPlayer.ActorNumber : -1;
			}
		}

		public bool WasExtracted { get; set; }

		public CloudCommunicator(FusionAppSettings clientConfig)
		{
			this.Client = new FusionRelayClient(clientConfig);
			this.Client.OnEventCallback += this.PushPackage;
		}

		public override void Service()
		{
			bool flag = this.Client == null;
			if (!flag)
			{
				this.Client.Update();
				base.Service();
			}
		}

		public unsafe override bool SendPackage(byte code, int targetActor, bool reliable, byte* buffer, int bufferLength)
		{
			return this.Client != null && this.Client.SendEvent(targetActor, code, buffer, bufferLength, reliable);
		}

		protected override void ConvertData(object data, out byte[] dataBuffer, out int maxLength)
		{
			dataBuffer = null;
			maxLength = this._buffer.Length;
			this.Client.ExtractData(data, this._buffer, ref maxLength);
			bool flag = maxLength > 0;
			if (flag)
			{
				dataBuffer = this._buffer;
			}
		}

		public void Reset()
		{
			this.Client.Reset();
			this.MessageSendQueue.Clear();
			this.RecvQueue.Clear();
			this.Callbacks.Clear();
		}

		public void Dispose()
		{
			bool flag = !this.WasExtracted;
			if (flag)
			{
				this.Client = null;
			}
		}

		private readonly byte[] _buffer = new byte[65536];
	}
}
