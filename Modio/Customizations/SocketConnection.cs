using System;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Modio.Customizations
{
	internal class SocketConnection : ISocketConnection
	{
		private Action<WssMessages> Receive { get; set; }

		private Action Disconnect { get; set; }

		public bool Connected()
		{
			ClientWebSocket clientWebSocket = this.webSocket;
			return clientWebSocket != null && clientWebSocket.State == WebSocketState.Open;
		}

		public Task<Error> SetupConnection(string url, Action<WssMessages> onReceive, Action onDisconnect)
		{
			SocketConnection.<SetupConnection>d__12 <SetupConnection>d__;
			<SetupConnection>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SetupConnection>d__.<>4__this = this;
			<SetupConnection>d__.url = url;
			<SetupConnection>d__.onReceive = onReceive;
			<SetupConnection>d__.onDisconnect = onDisconnect;
			<SetupConnection>d__.<>1__state = -1;
			<SetupConnection>d__.<>t__builder.Start<SocketConnection.<SetupConnection>d__12>(ref <SetupConnection>d__);
			return <SetupConnection>d__.<>t__builder.Task;
		}

		public Task CloseConnection()
		{
			SocketConnection.<CloseConnection>d__13 <CloseConnection>d__;
			<CloseConnection>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CloseConnection>d__.<>4__this = this;
			<CloseConnection>d__.<>1__state = -1;
			<CloseConnection>d__.<>t__builder.Start<SocketConnection.<CloseConnection>d__13>(ref <CloseConnection>d__);
			return <CloseConnection>d__.<>t__builder.Task;
		}

		private void ReceiveMessages()
		{
			SocketConnection.<ReceiveMessages>d__14 <ReceiveMessages>d__;
			<ReceiveMessages>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ReceiveMessages>d__.<>4__this = this;
			<ReceiveMessages>d__.<>1__state = -1;
			<ReceiveMessages>d__.<>t__builder.Start<SocketConnection.<ReceiveMessages>d__14>(ref <ReceiveMessages>d__);
		}

		public Task<Error> SendData(WssMessages message)
		{
			SocketConnection.<SendData>d__15 <SendData>d__;
			<SendData>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SendData>d__.<>4__this = this;
			<SendData>d__.message = message;
			<SendData>d__.<>1__state = -1;
			<SendData>d__.<>t__builder.Start<SocketConnection.<SendData>d__15>(ref <SendData>d__);
			return <SendData>d__.<>t__builder.Task;
		}

		private ClientWebSocket webSocket;

		private readonly Mutex _sending = new Mutex();

		private bool closingConnection;
	}
}
