using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class ParallelStream<V, T>
	{
		public void Run_NoThreads(IEnumerable<V> sourceIn)
		{
			foreach (V arg in sourceIn)
			{
				T obj = this.ProducerF(arg);
				this.ConsumerF(obj);
			}
		}

		public void Run(IEnumerable<V> sourceIn)
		{
			this.source = sourceIn;
			this.producer_done = false;
			this.consumer_done_event = new AutoResetEvent(false);
			new Thread(new ThreadStart(this.ProducerThreadFunc))
			{
				Name = "ParallelStream_producer"
			}.Start();
			new Thread(new ThreadStart(this.ConsumerThreadFunc))
			{
				Name = "ParallelStream_consumer"
			}.Start();
			this.consumer_done_event.WaitOne();
		}

		private void ProducerThreadFunc()
		{
			foreach (V arg in this.source)
			{
				T obj = this.ProducerF(arg);
				this.store0.Add(obj);
			}
			this.producer_done = true;
		}

		private void ConsumerThreadFunc()
		{
			T obj = default(T);
			while (!this.producer_done || this.store0.Count > 0)
			{
				if (this.store0.Remove(ref obj))
				{
					this.ConsumerF(obj);
				}
			}
			this.consumer_done_event.Set();
		}

		public Func<V, T> ProducerF;

		public Action<T> ConsumerF;

		private LockingQueue<T> store0 = new LockingQueue<T>();

		private IEnumerable<V> source;

		private bool producer_done;

		private AutoResetEvent consumer_done_event;
	}
}
