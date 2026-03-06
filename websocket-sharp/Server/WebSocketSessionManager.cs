using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace WebSocketSharp.Server
{
	public class WebSocketSessionManager
	{
		internal WebSocketSessionManager(Logger log)
		{
			this._log = log;
			this._forSweep = new object();
			this._keepClean = true;
			this._sessions = new Dictionary<string, IWebSocketSession>();
			this._state = ServerState.Ready;
			this._sync = ((ICollection)this._sessions).SyncRoot;
			this._waitTime = TimeSpan.FromSeconds(1.0);
			this.setSweepTimer(60000.0);
		}

		internal ServerState State
		{
			get
			{
				return this._state;
			}
		}

		public IEnumerable<string> ActiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> res in this.broadping(WebSocketSessionManager._emptyPingFrameAsBytes))
				{
					bool value = res.Value;
					if (value)
					{
						yield return res.Key;
					}
					res = default(KeyValuePair<string, bool>);
				}
				Dictionary<string, bool>.Enumerator enumerator = default(Dictionary<string, bool>.Enumerator);
				yield break;
				yield break;
			}
		}

		public int Count
		{
			get
			{
				object sync = this._sync;
				int count;
				lock (sync)
				{
					count = this._sessions.Count;
				}
				return count;
			}
		}

		public IEnumerable<string> IDs
		{
			get
			{
				bool flag = this._state != ServerState.Start;
				IEnumerable<string> result;
				if (flag)
				{
					result = Enumerable.Empty<string>();
				}
				else
				{
					object sync = this._sync;
					lock (sync)
					{
						bool flag2 = this._state != ServerState.Start;
						if (flag2)
						{
							result = Enumerable.Empty<string>();
						}
						else
						{
							result = this._sessions.Keys.ToList<string>();
						}
					}
				}
				return result;
			}
		}

		public IEnumerable<string> InactiveIDs
		{
			get
			{
				foreach (KeyValuePair<string, bool> res in this.broadping(WebSocketSessionManager._emptyPingFrameAsBytes))
				{
					bool flag = !res.Value;
					if (flag)
					{
						yield return res.Key;
					}
					res = default(KeyValuePair<string, bool>);
				}
				Dictionary<string, bool>.Enumerator enumerator = default(Dictionary<string, bool>.Enumerator);
				yield break;
				yield break;
			}
		}

		public IWebSocketSession this[string id]
		{
			get
			{
				bool flag = id == null;
				if (flag)
				{
					throw new ArgumentNullException("id");
				}
				bool flag2 = id.Length == 0;
				if (flag2)
				{
					throw new ArgumentException("An empty string.", "id");
				}
				IWebSocketSession result;
				this.tryGetSession(id, out result);
				return result;
			}
		}

		public bool KeepClean
		{
			get
			{
				return this._keepClean;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._keepClean = value;
					}
				}
			}
		}

		public IEnumerable<IWebSocketSession> Sessions
		{
			get
			{
				bool flag = this._state != ServerState.Start;
				IEnumerable<IWebSocketSession> result;
				if (flag)
				{
					result = Enumerable.Empty<IWebSocketSession>();
				}
				else
				{
					object sync = this._sync;
					lock (sync)
					{
						bool flag2 = this._state != ServerState.Start;
						if (flag2)
						{
							result = Enumerable.Empty<IWebSocketSession>();
						}
						else
						{
							result = this._sessions.Values.ToList<IWebSocketSession>();
						}
					}
				}
				return result;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._waitTime;
			}
			set
			{
				bool flag = value <= TimeSpan.Zero;
				if (flag)
				{
					string message = "It is zero or less.";
					throw new ArgumentOutOfRangeException("value", message);
				}
				object sync = this._sync;
				lock (sync)
				{
					bool flag2 = !this.canSet();
					if (!flag2)
					{
						this._waitTime = value;
					}
				}
			}
		}

		private void broadcast(Opcode opcode, byte[] data, Action completed)
		{
			Dictionary<CompressionMethod, byte[]> dictionary = new Dictionary<CompressionMethod, byte[]>();
			try
			{
				foreach (IWebSocketSession webSocketSession in this.Sessions)
				{
					bool flag = this._state != ServerState.Start;
					if (flag)
					{
						this._log.Error("The service is shutting down.");
						break;
					}
					webSocketSession.Context.WebSocket.Send(opcode, data, dictionary);
				}
				bool flag2 = completed != null;
				if (flag2)
				{
					completed();
				}
			}
			catch (Exception ex)
			{
				this._log.Error(ex.Message);
				this._log.Debug(ex.ToString());
			}
			finally
			{
				dictionary.Clear();
			}
		}

		private void broadcast(Opcode opcode, Stream stream, Action completed)
		{
			Dictionary<CompressionMethod, Stream> dictionary = new Dictionary<CompressionMethod, Stream>();
			try
			{
				foreach (IWebSocketSession webSocketSession in this.Sessions)
				{
					bool flag = this._state != ServerState.Start;
					if (flag)
					{
						this._log.Error("The service is shutting down.");
						break;
					}
					webSocketSession.Context.WebSocket.Send(opcode, stream, dictionary);
				}
				bool flag2 = completed != null;
				if (flag2)
				{
					completed();
				}
			}
			catch (Exception ex)
			{
				this._log.Error(ex.Message);
				this._log.Debug(ex.ToString());
			}
			finally
			{
				foreach (Stream stream2 in dictionary.Values)
				{
					stream2.Dispose();
				}
				dictionary.Clear();
			}
		}

		private void broadcastAsync(Opcode opcode, byte[] data, Action completed)
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				this.broadcast(opcode, data, completed);
			});
		}

		private void broadcastAsync(Opcode opcode, Stream stream, Action completed)
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				this.broadcast(opcode, stream, completed);
			});
		}

		private Dictionary<string, bool> broadping(byte[] frameAsBytes)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (IWebSocketSession webSocketSession in this.Sessions)
			{
				bool flag = this._state != ServerState.Start;
				if (flag)
				{
					this._log.Error("The service is shutting down.");
					break;
				}
				bool value = webSocketSession.Context.WebSocket.Ping(frameAsBytes, this._waitTime);
				dictionary.Add(webSocketSession.ID, value);
			}
			return dictionary;
		}

		private bool canSet()
		{
			return this._state == ServerState.Ready || this._state == ServerState.Stop;
		}

		private static string createID()
		{
			return Guid.NewGuid().ToString("N");
		}

		private void setSweepTimer(double interval)
		{
			this._sweepTimer = new System.Timers.Timer(interval);
			this._sweepTimer.Elapsed += delegate(object sender, ElapsedEventArgs e)
			{
				this.Sweep();
			};
		}

		private void stop(PayloadData payloadData, bool send)
		{
			byte[] frameAsBytes = send ? WebSocketFrame.CreateCloseFrame(payloadData, false).ToArray() : null;
			object sync = this._sync;
			lock (sync)
			{
				this._state = ServerState.ShuttingDown;
				this._sweepTimer.Enabled = false;
				foreach (IWebSocketSession webSocketSession in this._sessions.Values.ToList<IWebSocketSession>())
				{
					webSocketSession.Context.WebSocket.Close(payloadData, frameAsBytes);
				}
				this._state = ServerState.Stop;
			}
		}

		private bool tryGetSession(string id, out IWebSocketSession session)
		{
			session = null;
			bool flag = this._state != ServerState.Start;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag2 = this._state != ServerState.Start;
					if (flag2)
					{
						result = false;
					}
					else
					{
						result = this._sessions.TryGetValue(id, out session);
					}
				}
			}
			return result;
		}

		internal string Add(IWebSocketSession session)
		{
			object sync = this._sync;
			string result;
			lock (sync)
			{
				bool flag = this._state != ServerState.Start;
				if (flag)
				{
					result = null;
				}
				else
				{
					string text = WebSocketSessionManager.createID();
					this._sessions.Add(text, session);
					result = text;
				}
			}
			return result;
		}

		internal bool Remove(string id)
		{
			object sync = this._sync;
			bool result;
			lock (sync)
			{
				result = this._sessions.Remove(id);
			}
			return result;
		}

		internal void Start()
		{
			object sync = this._sync;
			lock (sync)
			{
				this._sweepTimer.Enabled = this._keepClean;
				this._state = ServerState.Start;
			}
		}

		internal void Stop(ushort code, string reason)
		{
			bool flag = code == 1005;
			if (flag)
			{
				this.stop(PayloadData.Empty, true);
			}
			else
			{
				PayloadData payloadData = new PayloadData(code, reason);
				bool send = !code.IsReserved();
				this.stop(payloadData, send);
			}
		}

		public void Broadcast(byte[] data)
		{
			bool flag = this._state != ServerState.Start;
			if (flag)
			{
				string message = "The current state of the service is not Start.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			bool flag3 = (long)data.Length <= (long)WebSocket.FragmentLength;
			if (flag3)
			{
				this.broadcast(Opcode.Binary, data, null);
			}
			else
			{
				this.broadcast(Opcode.Binary, new MemoryStream(data), null);
			}
		}

		public void Broadcast(string data)
		{
			bool flag = this._state != ServerState.Start;
			if (flag)
			{
				string message = "The current state of the service is not Start.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array;
			bool flag3 = !data.TryGetUTF8EncodedBytes(out array);
			if (flag3)
			{
				string message2 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message2, "data");
			}
			bool flag4 = (long)array.Length <= (long)WebSocket.FragmentLength;
			if (flag4)
			{
				this.broadcast(Opcode.Text, array, null);
			}
			else
			{
				this.broadcast(Opcode.Text, new MemoryStream(array), null);
			}
		}

		public void Broadcast(Stream stream, int length)
		{
			bool flag = this._state != ServerState.Start;
			if (flag)
			{
				string message = "The current state of the service is not Start.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = !stream.CanRead;
			if (flag3)
			{
				string message2 = "It cannot be read.";
				throw new ArgumentException(message2, "stream");
			}
			bool flag4 = length < 1;
			if (flag4)
			{
				string message3 = "It is less than 1.";
				throw new ArgumentException(message3, "length");
			}
			byte[] array = stream.ReadBytes(length);
			int num = array.Length;
			bool flag5 = num == 0;
			if (flag5)
			{
				string message4 = "No data could be read from it.";
				throw new ArgumentException(message4, "stream");
			}
			bool flag6 = num < length;
			if (flag6)
			{
				string format = "Only {0} byte(s) of data could be read from the stream.";
				string message5 = string.Format(format, num);
				this._log.Warn(message5);
			}
			bool flag7 = num <= WebSocket.FragmentLength;
			if (flag7)
			{
				this.broadcast(Opcode.Binary, array, null);
			}
			else
			{
				this.broadcast(Opcode.Binary, new MemoryStream(array), null);
			}
		}

		public void BroadcastAsync(byte[] data, Action completed)
		{
			bool flag = this._state != ServerState.Start;
			if (flag)
			{
				string message = "The current state of the service is not Start.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			bool flag3 = (long)data.Length <= (long)WebSocket.FragmentLength;
			if (flag3)
			{
				this.broadcastAsync(Opcode.Binary, data, completed);
			}
			else
			{
				this.broadcastAsync(Opcode.Binary, new MemoryStream(data), completed);
			}
		}

		public void BroadcastAsync(string data, Action completed)
		{
			bool flag = this._state != ServerState.Start;
			if (flag)
			{
				string message = "The current state of the service is not Start.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array;
			bool flag3 = !data.TryGetUTF8EncodedBytes(out array);
			if (flag3)
			{
				string message2 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message2, "data");
			}
			bool flag4 = (long)array.Length <= (long)WebSocket.FragmentLength;
			if (flag4)
			{
				this.broadcastAsync(Opcode.Text, array, completed);
			}
			else
			{
				this.broadcastAsync(Opcode.Text, new MemoryStream(array), completed);
			}
		}

		public void BroadcastAsync(Stream stream, int length, Action completed)
		{
			bool flag = this._state != ServerState.Start;
			if (flag)
			{
				string message = "The current state of the service is not Start.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = !stream.CanRead;
			if (flag3)
			{
				string message2 = "It cannot be read.";
				throw new ArgumentException(message2, "stream");
			}
			bool flag4 = length < 1;
			if (flag4)
			{
				string message3 = "It is less than 1.";
				throw new ArgumentException(message3, "length");
			}
			byte[] array = stream.ReadBytes(length);
			int num = array.Length;
			bool flag5 = num == 0;
			if (flag5)
			{
				string message4 = "No data could be read from it.";
				throw new ArgumentException(message4, "stream");
			}
			bool flag6 = num < length;
			if (flag6)
			{
				string format = "Only {0} byte(s) of data could be read from the stream.";
				string message5 = string.Format(format, num);
				this._log.Warn(message5);
			}
			bool flag7 = num <= WebSocket.FragmentLength;
			if (flag7)
			{
				this.broadcastAsync(Opcode.Binary, array, completed);
			}
			else
			{
				this.broadcastAsync(Opcode.Binary, new MemoryStream(array), completed);
			}
		}

		public void CloseSession(string id)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.Close();
		}

		public void CloseSession(string id, ushort code, string reason)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.Close(code, reason);
		}

		public void CloseSession(string id, CloseStatusCode code, string reason)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.Close(code, reason);
		}

		public bool PingTo(string id)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			return webSocketSession.Context.WebSocket.Ping();
		}

		public bool PingTo(string message, string id)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message2 = "The session could not be found.";
				throw new InvalidOperationException(message2);
			}
			return webSocketSession.Context.WebSocket.Ping(message);
		}

		public void SendTo(byte[] data, string id)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.Send(data);
		}

		public void SendTo(string data, string id)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.Send(data);
		}

		public void SendTo(Stream stream, int length, string id)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.Send(stream, length);
		}

		public void SendToAsync(byte[] data, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.SendAsync(data, completed);
		}

		public void SendToAsync(string data, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.SendAsync(data, completed);
		}

		public void SendToAsync(Stream stream, int length, string id, Action<bool> completed)
		{
			IWebSocketSession webSocketSession;
			bool flag = !this.TryGetSession(id, out webSocketSession);
			if (flag)
			{
				string message = "The session could not be found.";
				throw new InvalidOperationException(message);
			}
			webSocketSession.Context.WebSocket.SendAsync(stream, length, completed);
		}

		public void Sweep()
		{
			bool sweeping = this._sweeping;
			if (sweeping)
			{
				this._log.Info("The sweeping is already in progress.");
			}
			else
			{
				object forSweep = this._forSweep;
				lock (forSweep)
				{
					bool sweeping2 = this._sweeping;
					if (sweeping2)
					{
						this._log.Info("The sweeping is already in progress.");
						return;
					}
					this._sweeping = true;
				}
				foreach (string key in this.InactiveIDs)
				{
					bool flag = this._state != ServerState.Start;
					if (flag)
					{
						break;
					}
					object sync = this._sync;
					lock (sync)
					{
						bool flag2 = this._state != ServerState.Start;
						if (flag2)
						{
							break;
						}
						IWebSocketSession webSocketSession;
						bool flag3 = !this._sessions.TryGetValue(key, out webSocketSession);
						if (!flag3)
						{
							WebSocketState connectionState = webSocketSession.ConnectionState;
							bool flag4 = connectionState == WebSocketState.Open;
							if (flag4)
							{
								webSocketSession.Context.WebSocket.Close(CloseStatusCode.Abnormal);
							}
							else
							{
								bool flag5 = connectionState == WebSocketState.Closing;
								if (!flag5)
								{
									this._sessions.Remove(key);
								}
							}
						}
					}
				}
				this._sweeping = false;
			}
		}

		public bool TryGetSession(string id, out IWebSocketSession session)
		{
			bool flag = id == null;
			if (flag)
			{
				throw new ArgumentNullException("id");
			}
			bool flag2 = id.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "id");
			}
			return this.tryGetSession(id, out session);
		}

		private static readonly byte[] _emptyPingFrameAsBytes = WebSocketFrame.CreatePingFrame(false).ToArray();

		private object _forSweep;

		private volatile bool _keepClean;

		private Logger _log;

		private Dictionary<string, IWebSocketSession> _sessions;

		private volatile ServerState _state;

		private volatile bool _sweeping;

		private System.Timers.Timer _sweepTimer;

		private object _sync;

		private TimeSpan _waitTime;
	}
}
