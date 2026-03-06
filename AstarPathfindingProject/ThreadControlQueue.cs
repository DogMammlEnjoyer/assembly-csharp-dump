using System;
using System.Threading;

namespace Pathfinding
{
	internal class ThreadControlQueue
	{
		public ThreadControlQueue(int numReceivers)
		{
			this.numReceivers = numReceivers;
		}

		public bool IsEmpty
		{
			get
			{
				return this.head == null;
			}
		}

		public bool IsTerminating
		{
			get
			{
				return this.terminate;
			}
		}

		public void Block()
		{
			object obj = this.lockObj;
			lock (obj)
			{
				this.blocked = true;
				this.block.Reset();
			}
		}

		public void Unblock()
		{
			object obj = this.lockObj;
			lock (obj)
			{
				this.blocked = false;
				this.block.Set();
			}
		}

		public void Lock()
		{
			Monitor.Enter(this.lockObj);
		}

		public void Unlock()
		{
			Monitor.Exit(this.lockObj);
		}

		public bool AllReceiversBlocked
		{
			get
			{
				object obj = this.lockObj;
				bool result;
				lock (obj)
				{
					result = (this.blocked && this.blockedReceivers == this.numReceivers);
				}
				return result;
			}
		}

		public void PushFront(Path path)
		{
			object obj = this.lockObj;
			lock (obj)
			{
				if (!this.terminate)
				{
					if (this.tail == null)
					{
						this.head = path;
						this.tail = path;
						if (this.starving && !this.blocked)
						{
							this.starving = false;
							this.block.Set();
						}
						else
						{
							this.starving = false;
						}
					}
					else
					{
						path.next = this.head;
						this.head = path;
					}
				}
			}
		}

		public void Push(Path path)
		{
			object obj = this.lockObj;
			lock (obj)
			{
				if (!this.terminate)
				{
					if (this.tail == null)
					{
						this.head = path;
						this.tail = path;
						if (this.starving && !this.blocked)
						{
							this.starving = false;
							this.block.Set();
						}
						else
						{
							this.starving = false;
						}
					}
					else
					{
						this.tail.next = path;
						this.tail = path;
					}
				}
			}
		}

		private void Starving()
		{
			this.starving = true;
			this.block.Reset();
		}

		public void TerminateReceivers()
		{
			object obj = this.lockObj;
			lock (obj)
			{
				this.terminate = true;
				this.block.Set();
			}
		}

		public Path Pop()
		{
			Monitor.Enter(this.lockObj);
			Path result;
			try
			{
				if (this.terminate)
				{
					this.blockedReceivers++;
					throw new ThreadControlQueue.QueueTerminationException();
				}
				if (this.head == null)
				{
					this.Starving();
				}
				while (this.blocked || this.starving)
				{
					this.blockedReceivers++;
					if (this.blockedReceivers > this.numReceivers)
					{
						throw new InvalidOperationException(string.Concat(new string[]
						{
							"More receivers are blocked than specified in constructor (",
							this.blockedReceivers.ToString(),
							" > ",
							this.numReceivers.ToString(),
							")"
						}));
					}
					Monitor.Exit(this.lockObj);
					this.block.WaitOne();
					Monitor.Enter(this.lockObj);
					if (this.terminate)
					{
						throw new ThreadControlQueue.QueueTerminationException();
					}
					this.blockedReceivers--;
					if (this.head == null)
					{
						this.Starving();
					}
				}
				Path path = this.head;
				Path next = this.head.next;
				if (next == null)
				{
					this.tail = null;
				}
				this.head.next = null;
				this.head = next;
				result = path;
			}
			finally
			{
				if (Monitor.IsEntered(this.lockObj))
				{
					Monitor.Exit(this.lockObj);
				}
			}
			return result;
		}

		public void ReceiverTerminated()
		{
			Monitor.Enter(this.lockObj);
			this.blockedReceivers++;
			Monitor.Exit(this.lockObj);
		}

		public Path PopNoBlock(bool blockedBefore)
		{
			Path result;
			lock (this.lockObj)
			{
				if (this.terminate)
				{
					this.blockedReceivers++;
					throw new ThreadControlQueue.QueueTerminationException();
				}
				if (this.head == null)
				{
					this.Starving();
				}
				if (this.blocked || this.starving)
				{
					if (!blockedBefore)
					{
						this.blockedReceivers++;
						if (this.terminate)
						{
							throw new ThreadControlQueue.QueueTerminationException();
						}
						if (this.blockedReceivers != this.numReceivers && this.blockedReceivers > this.numReceivers)
						{
							throw new InvalidOperationException(string.Concat(new string[]
							{
								"More receivers are blocked than specified in constructor (",
								this.blockedReceivers.ToString(),
								" > ",
								this.numReceivers.ToString(),
								")"
							}));
						}
					}
					result = null;
				}
				else
				{
					if (blockedBefore)
					{
						this.blockedReceivers--;
					}
					Path path = this.head;
					Path next = this.head.next;
					if (next == null)
					{
						this.tail = null;
					}
					this.head.next = null;
					this.head = next;
					result = path;
				}
			}
			return result;
		}

		private Path head;

		private Path tail;

		private readonly object lockObj = new object();

		private readonly int numReceivers;

		private bool blocked;

		private int blockedReceivers;

		private bool starving;

		private bool terminate;

		private ManualResetEvent block = new ManualResetEvent(true);

		public class QueueTerminationException : Exception
		{
		}
	}
}
