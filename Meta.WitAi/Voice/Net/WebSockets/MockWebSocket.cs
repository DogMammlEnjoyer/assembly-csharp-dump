using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.Voice.Net.Encoding.Wit;
using Meta.WitAi.Json;

namespace Meta.Voice.Net.WebSockets
{
	public class MockWebSocket : IWebSocket
	{
		public WitWebSocketConnectionState State { get; set; }

		public MockWebSocket(bool autoOpen = false)
		{
			if (autoOpen)
			{
				this.HandleConnect = new Action(this.SimulateOpen);
			}
		}

		public event Action OnOpen;

		public void SimulateOpen()
		{
			this.State = WitWebSocketConnectionState.Connected;
			Action onOpen = this.OnOpen;
			if (onOpen == null)
			{
				return;
			}
			onOpen();
		}

		public event Action<byte[], int, int> OnMessage;

		public void SimulateResponse(byte[] bytes, int offset, int length)
		{
			Action<byte[], int, int> onMessage = this.OnMessage;
			if (onMessage == null)
			{
				return;
			}
			onMessage(bytes, offset, length);
		}

		public void SimulateResponse(WitResponseNode jsonData, byte[] binaryData = null)
		{
			byte[] array = WitChunkConverter.Encode(jsonData, binaryData);
			this.SimulateResponse(array, 0, array.Length);
		}

		public event Action<string> OnError;

		public void SimulateError(string error)
		{
			Action<string> onError = this.OnError;
			if (onError != null)
			{
				onError(error);
			}
			this.SimulateClose(WebSocketCloseCode.Abnormal);
		}

		public event Action<WebSocketCloseCode> OnClose;

		private void SimulateClose(WebSocketCloseCode closeCode)
		{
			this.State = WitWebSocketConnectionState.Disconnected;
			Action<WebSocketCloseCode> onClose = this.OnClose;
			if (onClose == null)
			{
				return;
			}
			onClose(closeCode);
		}

		public event Action HandleConnect;

		public Task Connect()
		{
			MockWebSocket.<Connect>d__25 <Connect>d__;
			<Connect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Connect>d__.<>4__this = this;
			<Connect>d__.<>1__state = -1;
			<Connect>d__.<>t__builder.Start<MockWebSocket.<Connect>d__25>(ref <Connect>d__);
			return <Connect>d__.<>t__builder.Task;
		}

		public event Action<byte[]> HandleSend;

		public Task Send(byte[] data)
		{
			MockWebSocket.<Send>d__29 <Send>d__;
			<Send>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Send>d__.<>4__this = this;
			<Send>d__.data = data;
			<Send>d__.<>1__state = -1;
			<Send>d__.<>t__builder.Start<MockWebSocket.<Send>d__29>(ref <Send>d__);
			return <Send>d__.<>t__builder.Task;
		}

		public event Action HandleClose;

		public Task Close()
		{
			this.State = WitWebSocketConnectionState.Disconnecting;
			Action handleClose = this.HandleClose;
			if (handleClose != null)
			{
				handleClose();
			}
			this.SimulateClose(WebSocketCloseCode.Normal);
			return Task.CompletedTask;
		}
	}
}
