using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.Net.NativeWebSocket;

namespace Meta.Voice.Net.WebSockets
{
	public class NativeWebSocketWrapper : IWebSocket
	{
		public NativeWebSocketWrapper(string url, Dictionary<string, string> headers)
		{
			this._webSocket = new WebSocket(url, headers);
			this._webSocket.OnOpen += this.RaiseOpen;
			this._webSocket.OnMessage += this.RaiseMessage;
			this._webSocket.OnError += this.RaiseError;
			this._webSocket.OnClose += this.RaiseClose;
		}

		~NativeWebSocketWrapper()
		{
			this._webSocket.OnOpen -= this.RaiseOpen;
			this._webSocket.OnMessage -= this.RaiseMessage;
			this._webSocket.OnError -= this.RaiseError;
			this._webSocket.OnClose -= this.RaiseClose;
		}

		public WitWebSocketConnectionState State
		{
			get
			{
				switch (this._webSocket.State)
				{
				case WebSocketState.Connecting:
					return WitWebSocketConnectionState.Connecting;
				case WebSocketState.Open:
					return WitWebSocketConnectionState.Connected;
				case WebSocketState.Closing:
					return WitWebSocketConnectionState.Disconnecting;
				case WebSocketState.Closed:
					return WitWebSocketConnectionState.Disconnected;
				default:
					return WitWebSocketConnectionState.Disconnected;
				}
			}
		}

		public Task Connect()
		{
			NativeWebSocketWrapper.<Connect>d__5 <Connect>d__;
			<Connect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Connect>d__.<>4__this = this;
			<Connect>d__.<>1__state = -1;
			<Connect>d__.<>t__builder.Start<NativeWebSocketWrapper.<Connect>d__5>(ref <Connect>d__);
			return <Connect>d__.<>t__builder.Task;
		}

		public event Action OnOpen;

		private void RaiseOpen()
		{
			Action onOpen = this.OnOpen;
			if (onOpen == null)
			{
				return;
			}
			onOpen();
		}

		public Task Send(byte[] data)
		{
			NativeWebSocketWrapper.<Send>d__10 <Send>d__;
			<Send>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Send>d__.<>4__this = this;
			<Send>d__.data = data;
			<Send>d__.<>1__state = -1;
			<Send>d__.<>t__builder.Start<NativeWebSocketWrapper.<Send>d__10>(ref <Send>d__);
			return <Send>d__.<>t__builder.Task;
		}

		public event Action<byte[], int, int> OnMessage;

		private void RaiseMessage(byte[] data, int offset, int length)
		{
			Action<byte[], int, int> onMessage = this.OnMessage;
			if (onMessage == null)
			{
				return;
			}
			onMessage(data, offset, length);
		}

		public event Action<string> OnError;

		private void RaiseError(string error)
		{
			Action<string> onError = this.OnError;
			if (onError == null)
			{
				return;
			}
			onError(error);
		}

		public Task Close()
		{
			NativeWebSocketWrapper.<Close>d__19 <Close>d__;
			<Close>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Close>d__.<>4__this = this;
			<Close>d__.<>1__state = -1;
			<Close>d__.<>t__builder.Start<NativeWebSocketWrapper.<Close>d__19>(ref <Close>d__);
			return <Close>d__.<>t__builder.Task;
		}

		public event Action<WebSocketCloseCode> OnClose;

		private void RaiseClose(WebSocketCloseCode closeCode)
		{
			Action<WebSocketCloseCode> onClose = this.OnClose;
			if (onClose == null)
			{
				return;
			}
			onClose((WebSocketCloseCode)closeCode);
		}

		private readonly WebSocket _webSocket;
	}
}
