using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class ServicePointScheduler
	{
		private ServicePoint ServicePoint { get; set; }

		public int MaxIdleTime
		{
			get
			{
				return this.maxIdleTime;
			}
			set
			{
				if (value < -1 || value > 2147483647)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value == this.maxIdleTime)
				{
					return;
				}
				this.maxIdleTime = value;
				this.Run();
			}
		}

		public int ConnectionLimit
		{
			get
			{
				return this.connectionLimit;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value == this.connectionLimit)
				{
					return;
				}
				this.connectionLimit = value;
				this.Run();
			}
		}

		public ServicePointScheduler(ServicePoint servicePoint, int connectionLimit, int maxIdleTime)
		{
			this.ServicePoint = servicePoint;
			this.connectionLimit = connectionLimit;
			this.maxIdleTime = maxIdleTime;
			this.schedulerEvent = new ServicePointScheduler.AsyncManualResetEvent(false);
			this.defaultGroup = new ServicePointScheduler.ConnectionGroup(this, string.Empty);
			this.operations = new LinkedList<ValueTuple<ServicePointScheduler.ConnectionGroup, WebOperation>>();
			this.idleConnections = new LinkedList<ValueTuple<ServicePointScheduler.ConnectionGroup, WebConnection, Task>>();
			this.idleSince = DateTime.UtcNow;
		}

		[Conditional("MONO_WEB_DEBUG")]
		private void Debug(string message)
		{
		}

		public int CurrentConnections
		{
			get
			{
				return this.currentConnections;
			}
		}

		public DateTime IdleSince
		{
			get
			{
				return this.idleSince;
			}
		}

		internal string ME { get; }

		public void Run()
		{
			if (Interlocked.CompareExchange(ref this.running, 1, 0) == 0)
			{
				Task.Run(() => this.RunScheduler());
			}
			this.schedulerEvent.Set();
		}

		private Task RunScheduler()
		{
			ServicePointScheduler.<RunScheduler>d__32 <RunScheduler>d__;
			<RunScheduler>d__.<>4__this = this;
			<RunScheduler>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RunScheduler>d__.<>1__state = -1;
			<RunScheduler>d__.<>t__builder.Start<ServicePointScheduler.<RunScheduler>d__32>(ref <RunScheduler>d__);
			return <RunScheduler>d__.<>t__builder.Task;
		}

		private void Cleanup()
		{
			if (this.groups != null)
			{
				string[] array = new string[this.groups.Count];
				this.groups.Keys.CopyTo(array, 0);
				foreach (string key in array)
				{
					if (this.groups.ContainsKey(key) && this.groups[key].IsEmpty())
					{
						this.groups.Remove(key);
					}
				}
				if (this.groups.Count == 0)
				{
					this.groups = null;
				}
			}
		}

		private void RunSchedulerIteration()
		{
			this.schedulerEvent.Reset();
			bool flag;
			do
			{
				flag = this.SchedulerIteration(this.defaultGroup);
				if (this.groups != null)
				{
					foreach (KeyValuePair<string, ServicePointScheduler.ConnectionGroup> keyValuePair in this.groups)
					{
						flag |= this.SchedulerIteration(keyValuePair.Value);
					}
				}
			}
			while (flag);
		}

		private bool OperationCompleted(ServicePointScheduler.ConnectionGroup group, WebOperation operation)
		{
			WebCompletionSource<ValueTuple<bool, WebOperation>>.Result currentResult = operation.Finished.CurrentResult;
			bool flag;
			WebOperation webOperation;
			if (!currentResult.Success)
			{
				flag = false;
				webOperation = null;
			}
			else
			{
				ValueTuple<bool, WebOperation> argument = currentResult.Argument;
				flag = argument.Item1;
				webOperation = argument.Item2;
			}
			if (!flag || !operation.Connection.Continue(webOperation))
			{
				group.RemoveConnection(operation.Connection);
				if (webOperation == null)
				{
					return true;
				}
				flag = false;
			}
			if (webOperation == null)
			{
				if (flag)
				{
					Task item = Task.Delay(this.MaxIdleTime);
					this.idleConnections.AddLast(new ValueTuple<ServicePointScheduler.ConnectionGroup, WebConnection, Task>(group, operation.Connection, item));
				}
				return true;
			}
			this.operations.AddLast(new ValueTuple<ServicePointScheduler.ConnectionGroup, WebOperation>(group, webOperation));
			if (flag)
			{
				this.RemoveIdleConnection(operation.Connection);
				return false;
			}
			group.Cleanup();
			group.CreateOrReuseConnection(webOperation, true);
			return false;
		}

		private void CloseIdleConnection(ServicePointScheduler.ConnectionGroup group, WebConnection connection)
		{
			group.RemoveConnection(connection);
			this.RemoveIdleConnection(connection);
		}

		private bool SchedulerIteration(ServicePointScheduler.ConnectionGroup group)
		{
			group.Cleanup();
			WebOperation nextOperation = group.GetNextOperation();
			if (nextOperation == null)
			{
				return false;
			}
			WebConnection item = group.CreateOrReuseConnection(nextOperation, false).Item1;
			if (item == null)
			{
				return false;
			}
			this.operations.AddLast(new ValueTuple<ServicePointScheduler.ConnectionGroup, WebOperation>(group, nextOperation));
			this.RemoveIdleConnection(item);
			return true;
		}

		private void RemoveOperation(WebOperation operation)
		{
			LinkedListNode<ValueTuple<ServicePointScheduler.ConnectionGroup, WebOperation>> linkedListNode = this.operations.First;
			while (linkedListNode != null)
			{
				LinkedListNode<ValueTuple<ServicePointScheduler.ConnectionGroup, WebOperation>> linkedListNode2 = linkedListNode;
				linkedListNode = linkedListNode.Next;
				if (linkedListNode2.Value.Item2 == operation)
				{
					this.operations.Remove(linkedListNode2);
				}
			}
		}

		private void RemoveIdleConnection(WebConnection connection)
		{
			LinkedListNode<ValueTuple<ServicePointScheduler.ConnectionGroup, WebConnection, Task>> linkedListNode = this.idleConnections.First;
			while (linkedListNode != null)
			{
				LinkedListNode<ValueTuple<ServicePointScheduler.ConnectionGroup, WebConnection, Task>> linkedListNode2 = linkedListNode;
				linkedListNode = linkedListNode.Next;
				if (linkedListNode2.Value.Item2 == connection)
				{
					this.idleConnections.Remove(linkedListNode2);
				}
			}
		}

		private void FinalCleanup()
		{
			this.groups = null;
			this.operations = null;
			this.idleConnections = null;
			this.defaultGroup = null;
			this.ServicePoint.FreeServicePoint();
			ServicePointManager.RemoveServicePoint(this.ServicePoint);
			this.ServicePoint = null;
		}

		public void SendRequest(WebOperation operation, string groupName)
		{
			ServicePoint servicePoint = this.ServicePoint;
			lock (servicePoint)
			{
				this.GetConnectionGroup(groupName).EnqueueOperation(operation);
				this.Run();
			}
		}

		public bool CloseConnectionGroup(string groupName)
		{
			ServicePointScheduler.ConnectionGroup connectionGroup;
			if (string.IsNullOrEmpty(groupName))
			{
				connectionGroup = this.defaultGroup;
			}
			else if (this.groups == null || !this.groups.TryGetValue(groupName, out connectionGroup))
			{
				return false;
			}
			if (connectionGroup != this.defaultGroup)
			{
				this.groups.Remove(groupName);
				if (this.groups.Count == 0)
				{
					this.groups = null;
				}
			}
			connectionGroup.Close();
			this.Run();
			return true;
		}

		private ServicePointScheduler.ConnectionGroup GetConnectionGroup(string name)
		{
			ServicePoint servicePoint = this.ServicePoint;
			ServicePointScheduler.ConnectionGroup result;
			lock (servicePoint)
			{
				if (string.IsNullOrEmpty(name))
				{
					result = this.defaultGroup;
				}
				else
				{
					if (this.groups == null)
					{
						this.groups = new Dictionary<string, ServicePointScheduler.ConnectionGroup>();
					}
					ServicePointScheduler.ConnectionGroup connectionGroup;
					if (this.groups.TryGetValue(name, out connectionGroup))
					{
						result = connectionGroup;
					}
					else
					{
						connectionGroup = new ServicePointScheduler.ConnectionGroup(this, name);
						this.groups.Add(name, connectionGroup);
						result = connectionGroup;
					}
				}
			}
			return result;
		}

		private void OnConnectionCreated(WebConnection connection)
		{
			Interlocked.Increment(ref this.currentConnections);
		}

		private void OnConnectionClosed(WebConnection connection)
		{
			this.RemoveIdleConnection(connection);
			Interlocked.Decrement(ref this.currentConnections);
		}

		public static Task<bool> WaitAsync(Task workerTask, int millisecondTimeout)
		{
			ServicePointScheduler.<WaitAsync>d__46 <WaitAsync>d__;
			<WaitAsync>d__.workerTask = workerTask;
			<WaitAsync>d__.millisecondTimeout = millisecondTimeout;
			<WaitAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<WaitAsync>d__.<>1__state = -1;
			<WaitAsync>d__.<>t__builder.Start<ServicePointScheduler.<WaitAsync>d__46>(ref <WaitAsync>d__);
			return <WaitAsync>d__.<>t__builder.Task;
		}

		private int running;

		private int maxIdleTime = 100000;

		private ServicePointScheduler.AsyncManualResetEvent schedulerEvent;

		private ServicePointScheduler.ConnectionGroup defaultGroup;

		private Dictionary<string, ServicePointScheduler.ConnectionGroup> groups;

		private LinkedList<ValueTuple<ServicePointScheduler.ConnectionGroup, WebOperation>> operations;

		private LinkedList<ValueTuple<ServicePointScheduler.ConnectionGroup, WebConnection, Task>> idleConnections;

		private int currentConnections;

		private int connectionLimit;

		private DateTime idleSince;

		private static int nextId;

		public readonly int ID = ++ServicePointScheduler.nextId;

		private class ConnectionGroup
		{
			public ServicePointScheduler Scheduler { get; }

			public string Name { get; }

			public bool IsDefault
			{
				get
				{
					return string.IsNullOrEmpty(this.Name);
				}
			}

			public ConnectionGroup(ServicePointScheduler scheduler, string name)
			{
				this.Scheduler = scheduler;
				this.Name = name;
				this.connections = new LinkedList<WebConnection>();
				this.queue = new LinkedList<WebOperation>();
			}

			public bool IsEmpty()
			{
				return this.connections.Count == 0 && this.queue.Count == 0;
			}

			public void RemoveConnection(WebConnection connection)
			{
				this.connections.Remove(connection);
				connection.Dispose();
				this.Scheduler.OnConnectionClosed(connection);
			}

			public void Cleanup()
			{
				LinkedListNode<WebConnection> linkedListNode = this.connections.First;
				while (linkedListNode != null)
				{
					WebConnection value = linkedListNode.Value;
					LinkedListNode<WebConnection> node = linkedListNode;
					linkedListNode = linkedListNode.Next;
					if (value.Closed)
					{
						this.connections.Remove(node);
						this.Scheduler.OnConnectionClosed(value);
					}
				}
			}

			public void Close()
			{
				foreach (WebOperation webOperation in this.queue)
				{
					webOperation.Abort();
					this.Scheduler.RemoveOperation(webOperation);
				}
				this.queue.Clear();
				foreach (WebConnection webConnection in this.connections)
				{
					webConnection.Dispose();
					this.Scheduler.OnConnectionClosed(webConnection);
				}
				this.connections.Clear();
			}

			public void EnqueueOperation(WebOperation operation)
			{
				this.queue.AddLast(operation);
			}

			public WebOperation GetNextOperation()
			{
				LinkedListNode<WebOperation> linkedListNode = this.queue.First;
				while (linkedListNode != null)
				{
					WebOperation value = linkedListNode.Value;
					LinkedListNode<WebOperation> node = linkedListNode;
					linkedListNode = linkedListNode.Next;
					if (!value.Aborted)
					{
						return value;
					}
					this.queue.Remove(node);
					this.Scheduler.RemoveOperation(value);
				}
				return null;
			}

			public WebConnection FindIdleConnection(WebOperation operation)
			{
				WebConnection webConnection = null;
				foreach (WebConnection webConnection2 in this.connections)
				{
					if (webConnection2.CanReuseConnection(operation) && (webConnection == null || webConnection2.IdleSince > webConnection.IdleSince))
					{
						webConnection = webConnection2;
					}
				}
				if (webConnection != null && webConnection.StartOperation(operation, true))
				{
					this.queue.Remove(operation);
					return webConnection;
				}
				foreach (WebConnection webConnection3 in this.connections)
				{
					if (webConnection3.StartOperation(operation, true))
					{
						this.queue.Remove(operation);
						return webConnection3;
					}
				}
				return null;
			}

			[return: TupleElementNames(new string[]
			{
				"connection",
				"created"
			})]
			public ValueTuple<WebConnection, bool> CreateOrReuseConnection(WebOperation operation, bool force)
			{
				WebConnection webConnection = this.FindIdleConnection(operation);
				if (webConnection != null)
				{
					return new ValueTuple<WebConnection, bool>(webConnection, false);
				}
				if (force || this.Scheduler.ServicePoint.ConnectionLimit > this.connections.Count || this.connections.Count == 0)
				{
					webConnection = new WebConnection(this.Scheduler.ServicePoint);
					webConnection.StartOperation(operation, false);
					this.connections.AddFirst(webConnection);
					this.Scheduler.OnConnectionCreated(webConnection);
					this.queue.Remove(operation);
					return new ValueTuple<WebConnection, bool>(webConnection, true);
				}
				return new ValueTuple<WebConnection, bool>(null, false);
			}

			private static int nextId;

			public readonly int ID = ++ServicePointScheduler.ConnectionGroup.nextId;

			private LinkedList<WebConnection> connections;

			private LinkedList<WebOperation> queue;
		}

		private class AsyncManualResetEvent
		{
			public Task WaitAsync()
			{
				return this.m_tcs.Task;
			}

			public bool WaitOne(int millisecondTimeout)
			{
				return this.m_tcs.Task.Wait(millisecondTimeout);
			}

			public Task<bool> WaitAsync(int millisecondTimeout)
			{
				return ServicePointScheduler.WaitAsync(this.m_tcs.Task, millisecondTimeout);
			}

			public void Set()
			{
				TaskCompletionSource<bool> tcs = this.m_tcs;
				Task.Factory.StartNew<bool>((object s) => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
				tcs.Task.Wait();
			}

			public void Reset()
			{
				TaskCompletionSource<bool> tcs;
				do
				{
					tcs = this.m_tcs;
				}
				while (tcs.Task.IsCompleted && Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this.m_tcs, new TaskCompletionSource<bool>(), tcs) != tcs);
			}

			public AsyncManualResetEvent(bool state)
			{
				if (state)
				{
					this.Set();
				}
			}

			private volatile TaskCompletionSource<bool> m_tcs = new TaskCompletionSource<bool>();
		}
	}
}
