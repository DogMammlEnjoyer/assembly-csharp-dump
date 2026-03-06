using System;
using System.Collections;
using System.Collections.Generic;

namespace WebSocketSharp.Server
{
	public class WebSocketServiceManager
	{
		internal WebSocketServiceManager(Logger log)
		{
			this._log = log;
			this._hosts = new Dictionary<string, WebSocketServiceHost>();
			this._keepClean = true;
			this._state = ServerState.Ready;
			this._sync = ((ICollection)this._hosts).SyncRoot;
			this._waitTime = TimeSpan.FromSeconds(1.0);
		}

		public int Count
		{
			get
			{
				object sync = this._sync;
				int count;
				lock (sync)
				{
					count = this._hosts.Count;
				}
				return count;
			}
		}

		public IEnumerable<WebSocketServiceHost> Hosts
		{
			get
			{
				object sync = this._sync;
				IEnumerable<WebSocketServiceHost> result;
				lock (sync)
				{
					result = this._hosts.Values.ToList<WebSocketServiceHost>();
				}
				return result;
			}
		}

		public WebSocketServiceHost this[string path]
		{
			get
			{
				bool flag = path == null;
				if (flag)
				{
					throw new ArgumentNullException("path");
				}
				bool flag2 = path.Length == 0;
				if (flag2)
				{
					throw new ArgumentException("An empty string.", "path");
				}
				bool flag3 = path[0] != '/';
				if (flag3)
				{
					string message = "It is not an absolute path.";
					throw new ArgumentException(message, "path");
				}
				bool flag4 = path.IndexOfAny(new char[]
				{
					'?',
					'#'
				}) > -1;
				if (flag4)
				{
					string message2 = "It includes either or both query and fragment components.";
					throw new ArgumentException(message2, "path");
				}
				WebSocketServiceHost result;
				this.InternalTryGetServiceHost(path, out result);
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
						foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
						{
							webSocketServiceHost.KeepClean = value;
						}
						this._keepClean = value;
					}
				}
			}
		}

		public IEnumerable<string> Paths
		{
			get
			{
				object sync = this._sync;
				IEnumerable<string> result;
				lock (sync)
				{
					result = this._hosts.Keys.ToList<string>();
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
						foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
						{
							webSocketServiceHost.WaitTime = value;
						}
						this._waitTime = value;
					}
				}
			}
		}

		private bool canSet()
		{
			return this._state == ServerState.Ready || this._state == ServerState.Stop;
		}

		internal bool InternalTryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			path = path.TrimSlashFromEnd();
			object sync = this._sync;
			bool result;
			lock (sync)
			{
				result = this._hosts.TryGetValue(path, out host);
			}
			return result;
		}

		internal void Start()
		{
			object sync = this._sync;
			lock (sync)
			{
				foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
				{
					webSocketServiceHost.Start();
				}
				this._state = ServerState.Start;
			}
		}

		internal void Stop(ushort code, string reason)
		{
			object sync = this._sync;
			lock (sync)
			{
				this._state = ServerState.ShuttingDown;
				foreach (WebSocketServiceHost webSocketServiceHost in this._hosts.Values)
				{
					webSocketServiceHost.Stop(code, reason);
				}
				this._state = ServerState.Stop;
			}
		}

		public void AddService<TBehavior>(string path, Action<TBehavior> initializer) where TBehavior : WebSocketBehavior, new()
		{
			bool flag = path == null;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = path.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "path");
			}
			bool flag3 = path[0] != '/';
			if (flag3)
			{
				string message = "It is not an absolute path.";
				throw new ArgumentException(message, "path");
			}
			bool flag4 = path.IndexOfAny(new char[]
			{
				'?',
				'#'
			}) > -1;
			if (flag4)
			{
				string message2 = "It includes either or both query and fragment components.";
				throw new ArgumentException(message2, "path");
			}
			path = path.TrimSlashFromEnd();
			object sync = this._sync;
			lock (sync)
			{
				WebSocketServiceHost webSocketServiceHost;
				bool flag5 = this._hosts.TryGetValue(path, out webSocketServiceHost);
				if (flag5)
				{
					string message3 = "It is already in use.";
					throw new ArgumentException(message3, "path");
				}
				webSocketServiceHost = new WebSocketServiceHost<TBehavior>(path, initializer, this._log);
				bool flag6 = !this._keepClean;
				if (flag6)
				{
					webSocketServiceHost.KeepClean = false;
				}
				bool flag7 = this._waitTime != webSocketServiceHost.WaitTime;
				if (flag7)
				{
					webSocketServiceHost.WaitTime = this._waitTime;
				}
				bool flag8 = this._state == ServerState.Start;
				if (flag8)
				{
					webSocketServiceHost.Start();
				}
				this._hosts.Add(path, webSocketServiceHost);
			}
		}

		public void Clear()
		{
			List<WebSocketServiceHost> list = null;
			object sync = this._sync;
			lock (sync)
			{
				list = this._hosts.Values.ToList<WebSocketServiceHost>();
				this._hosts.Clear();
			}
			foreach (WebSocketServiceHost webSocketServiceHost in list)
			{
				bool flag = webSocketServiceHost.State == ServerState.Start;
				if (flag)
				{
					webSocketServiceHost.Stop(1001, string.Empty);
				}
			}
		}

		public bool RemoveService(string path)
		{
			bool flag = path == null;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = path.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "path");
			}
			bool flag3 = path[0] != '/';
			if (flag3)
			{
				string message = "It is not an absolute path.";
				throw new ArgumentException(message, "path");
			}
			bool flag4 = path.IndexOfAny(new char[]
			{
				'?',
				'#'
			}) > -1;
			if (flag4)
			{
				string message2 = "It includes either or both query and fragment components.";
				throw new ArgumentException(message2, "path");
			}
			path = path.TrimSlashFromEnd();
			object sync = this._sync;
			WebSocketServiceHost webSocketServiceHost;
			lock (sync)
			{
				bool flag5 = !this._hosts.TryGetValue(path, out webSocketServiceHost);
				if (flag5)
				{
					return false;
				}
				this._hosts.Remove(path);
			}
			bool flag6 = webSocketServiceHost.State == ServerState.Start;
			if (flag6)
			{
				webSocketServiceHost.Stop(1001, string.Empty);
			}
			return true;
		}

		public bool TryGetServiceHost(string path, out WebSocketServiceHost host)
		{
			bool flag = path == null;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = path.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "path");
			}
			bool flag3 = path[0] != '/';
			if (flag3)
			{
				string message = "It is not an absolute path.";
				throw new ArgumentException(message, "path");
			}
			bool flag4 = path.IndexOfAny(new char[]
			{
				'?',
				'#'
			}) > -1;
			if (flag4)
			{
				string message2 = "It includes either or both query and fragment components.";
				throw new ArgumentException(message2, "path");
			}
			return this.InternalTryGetServiceHost(path, out host);
		}

		private Dictionary<string, WebSocketServiceHost> _hosts;

		private volatile bool _keepClean;

		private Logger _log;

		private volatile ServerState _state;

		private object _sync;

		private TimeSpan _waitTime;
	}
}
