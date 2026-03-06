using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NativeWebSocket
{
	public class WebSocket : IWebSocket
	{
		public event WebSocketOpenEventHandler OnOpen;

		public event WebSocketMessageEventHandler OnMessage;

		public event WebSocketErrorEventHandler OnError;

		public event WebSocketCloseEventHandler OnClose;

		public WebSocket(string url, Dictionary<string, string> headers = null)
		{
			this.uri = new Uri(url);
			if (headers == null)
			{
				this.headers = new Dictionary<string, string>();
			}
			else
			{
				this.headers = headers;
			}
			this.subprotocols = new List<string>();
			string scheme = this.uri.Scheme;
			if (!scheme.Equals("ws") && !scheme.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + scheme);
			}
		}

		public WebSocket(string url, string subprotocol, Dictionary<string, string> headers = null)
		{
			this.uri = new Uri(url);
			if (headers == null)
			{
				this.headers = new Dictionary<string, string>();
			}
			else
			{
				this.headers = headers;
			}
			this.subprotocols = new List<string>
			{
				subprotocol
			};
			string scheme = this.uri.Scheme;
			if (!scheme.Equals("ws") && !scheme.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + scheme);
			}
		}

		public WebSocket(string url, List<string> subprotocols, Dictionary<string, string> headers = null)
		{
			this.uri = new Uri(url);
			if (headers == null)
			{
				this.headers = new Dictionary<string, string>();
			}
			else
			{
				this.headers = headers;
			}
			this.subprotocols = subprotocols;
			string scheme = this.uri.Scheme;
			if (!scheme.Equals("ws") && !scheme.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + scheme);
			}
		}

		public void SetRequestHeader(string name, string value)
		{
			this.headers[name] = value;
		}

		public void CancelConnection()
		{
			CancellationTokenSource tokenSource = this.m_TokenSource;
			if (tokenSource == null)
			{
				return;
			}
			tokenSource.Cancel();
		}

		public Task Connect()
		{
			WebSocket.<Connect>d__28 <Connect>d__;
			<Connect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Connect>d__.<>4__this = this;
			<Connect>d__.<>1__state = -1;
			<Connect>d__.<>t__builder.Start<WebSocket.<Connect>d__28>(ref <Connect>d__);
			return <Connect>d__.<>t__builder.Task;
		}

		public WebSocketState State
		{
			get
			{
				switch (this.m_Socket.State)
				{
				case WebSocketState.Connecting:
					return WebSocketState.Connecting;
				case WebSocketState.Open:
					return WebSocketState.Open;
				case WebSocketState.CloseSent:
				case WebSocketState.CloseReceived:
					return WebSocketState.Closing;
				case WebSocketState.Closed:
					return WebSocketState.Closed;
				default:
					return WebSocketState.Closed;
				}
			}
		}

		public Task Send(byte[] bytes)
		{
			return this.SendMessage(this.sendBytesQueue, WebSocketMessageType.Binary, new ArraySegment<byte>(bytes));
		}

		public Task SendText(string message)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			return this.SendMessage(this.sendTextQueue, WebSocketMessageType.Text, new ArraySegment<byte>(bytes, 0, bytes.Length));
		}

		private Task SendMessage(List<ArraySegment<byte>> queue, WebSocketMessageType messageType, ArraySegment<byte> buffer)
		{
			WebSocket.<SendMessage>d__33 <SendMessage>d__;
			<SendMessage>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendMessage>d__.<>4__this = this;
			<SendMessage>d__.queue = queue;
			<SendMessage>d__.messageType = messageType;
			<SendMessage>d__.buffer = buffer;
			<SendMessage>d__.<>1__state = -1;
			<SendMessage>d__.<>t__builder.Start<WebSocket.<SendMessage>d__33>(ref <SendMessage>d__);
			return <SendMessage>d__.<>t__builder.Task;
		}

		private Task HandleQueue(List<ArraySegment<byte>> queue, WebSocketMessageType messageType)
		{
			WebSocket.<HandleQueue>d__34 <HandleQueue>d__;
			<HandleQueue>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleQueue>d__.<>4__this = this;
			<HandleQueue>d__.queue = queue;
			<HandleQueue>d__.messageType = messageType;
			<HandleQueue>d__.<>1__state = -1;
			<HandleQueue>d__.<>t__builder.Start<WebSocket.<HandleQueue>d__34>(ref <HandleQueue>d__);
			return <HandleQueue>d__.<>t__builder.Task;
		}

		public void DispatchMessageQueue()
		{
			if (this.m_MessageList.Count == 0)
			{
				return;
			}
			object incomingMessageLock = this.IncomingMessageLock;
			List<byte[]> list;
			lock (incomingMessageLock)
			{
				list = new List<byte[]>(this.m_MessageList);
				this.m_MessageList.Clear();
			}
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				WebSocketMessageEventHandler onMessage = this.OnMessage;
				if (onMessage != null)
				{
					onMessage(list[i]);
				}
			}
		}

		public Task Receive()
		{
			WebSocket.<Receive>d__37 <Receive>d__;
			<Receive>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Receive>d__.<>4__this = this;
			<Receive>d__.<>1__state = -1;
			<Receive>d__.<>t__builder.Start<WebSocket.<Receive>d__37>(ref <Receive>d__);
			return <Receive>d__.<>t__builder.Task;
		}

		public Task Close()
		{
			WebSocket.<Close>d__38 <Close>d__;
			<Close>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Close>d__.<>4__this = this;
			<Close>d__.<>1__state = -1;
			<Close>d__.<>t__builder.Start<WebSocket.<Close>d__38>(ref <Close>d__);
			return <Close>d__.<>t__builder.Task;
		}

		private Uri uri;

		private Dictionary<string, string> headers;

		private List<string> subprotocols;

		private ClientWebSocket m_Socket = new ClientWebSocket();

		private CancellationTokenSource m_TokenSource;

		private CancellationToken m_CancellationToken;

		private readonly object OutgoingMessageLock = new object();

		private readonly object IncomingMessageLock = new object();

		private bool isSending;

		private List<ArraySegment<byte>> sendBytesQueue = new List<ArraySegment<byte>>();

		private List<ArraySegment<byte>> sendTextQueue = new List<ArraySegment<byte>>();

		private List<byte[]> m_MessageList = new List<byte[]>();
	}
}
