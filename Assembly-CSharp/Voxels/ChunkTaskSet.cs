using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace Voxels
{
	public class ChunkTaskSet
	{
		public Chunk Chunk
		{
			get
			{
				return this.Chunks[0];
			}
		}

		public bool HasChunks
		{
			get
			{
				if (this.Chunks == null || this.Chunks.Count == 0)
				{
					return false;
				}
				using (List<Chunk>.Enumerator enumerator = this.Chunks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == null)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !this.Current.IsCreated && this.Callback == null && this.Tasks.Count == 0;
			}
		}

		public ChunkTaskSet(GenerationParameters parameters)
		{
			this.Chunks = new List<Chunk>();
			this.Parameters = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>();
		}

		public ChunkTaskSet(Chunk chunk, GenerationParameters parameters, [TupleElementNames(new string[]
		{
			"task",
			"callback"
		})] params ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>[] tasks)
		{
			this.Chunks = new List<Chunk>
			{
				chunk
			};
			this.Parameters = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>(tasks);
		}

		public ChunkTaskSet(IList<Chunk> chunks, GenerationParameters parameters, [TupleElementNames(new string[]
		{
			"task",
			"callback"
		})] params ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>[] tasks)
		{
			this.Chunks = new List<Chunk>(chunks);
			this.Parameters = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>(tasks);
		}

		public void AddTask(ChunkTaskSet.ChunkTaskDelegate task, Action<Chunk> callback = null)
		{
			this.Tasks.Enqueue(new ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>(task, callback));
		}

		public void Start()
		{
			if (this.Current.IsCreated || this.Callback != null)
			{
				throw new InvalidOperationException("Cannot start a ChunkTaskSet that is already running.");
			}
			this.StartNext();
			this.UpdateDirty();
		}

		public void Complete()
		{
			this.CompleteCurrent();
			while (this.StartNext())
			{
				this.CompleteCurrent();
			}
			this.UpdateDirty();
		}

		public bool CompleteIfReady()
		{
			if (this.CompleteCurrentIfReady())
			{
				if (this.Tasks.Count == 0)
				{
					this.UpdateDirty();
					return true;
				}
				this.StartNext();
			}
			this.UpdateDirty();
			return false;
		}

		private bool CompleteCurrentIfReady()
		{
			if (this.Current.IsCompleted)
			{
				this.CompleteCurrent();
				return true;
			}
			return false;
		}

		private void CompleteCurrent()
		{
			this.Current.Complete();
			foreach (Chunk obj in this.Chunks)
			{
				Action<Chunk> callback = this.Callback;
				if (callback != null)
				{
					callback(obj);
				}
			}
			this.Current = default(ChunkTask);
			this.Callback = null;
		}

		private bool StartNext()
		{
			if (this.Tasks.Count == 0)
			{
				return false;
			}
			ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>> task = this.Tasks.Dequeue();
			ValueTuple<ChunkTask, Action<Chunk>> valueTuple = this.CreateTask(task);
			this.Current = valueTuple.Item1;
			this.Callback = valueTuple.Item2;
			if (this.Current.IsCompleted)
			{
				this.CompleteCurrent();
				return this.StartNext();
			}
			return true;
		}

		private void UpdateDirty()
		{
			if (this.Current.IsCreated || this.Callback != null || this.Tasks.Count > 0)
			{
				using (List<Chunk>.Enumerator enumerator = this.Chunks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Chunk chunk = enumerator.Current;
						chunk.IsDirty = false;
					}
					return;
				}
			}
			foreach (Chunk chunk2 in this.Chunks)
			{
				chunk2.IsDirty = (chunk2.State < ChunkState.MeshAssigned);
			}
		}

		private ValueTuple<ChunkTask, Action<Chunk>> CreateTask([TupleElementNames(new string[]
		{
			"task",
			"callback"
		})] ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>> task)
		{
			return this.CreateTask(task.Item1, task.Item2);
		}

		private ValueTuple<ChunkTask, Action<Chunk>> CreateTask(ChunkTaskSet.ChunkTaskDelegate task, Action<Chunk> callback = null)
		{
			if (this.Chunks.Count == 1)
			{
				return new ValueTuple<ChunkTask, Action<Chunk>>((task != null) ? task(this.Chunks[0], this.Parameters) : default(ChunkTask), callback);
			}
			NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(this.Chunks.Count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < this.Chunks.Count; i++)
			{
				jobs[i] = ((task != null) ? task(this.Chunks[i], this.Parameters).Handle : default(JobHandle));
			}
			JobHandle handle = JobHandle.CombineDependencies(jobs);
			jobs.Dispose();
			return new ValueTuple<ChunkTask, Action<Chunk>>(new ChunkTask(this.Chunks[0], handle, null), callback);
		}

		public List<Chunk> Chunks;

		public GenerationParameters Parameters;

		[TupleElementNames(new string[]
		{
			"task",
			"callback"
		})]
		public Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>> Tasks;

		public ChunkTask Current;

		public Action<Chunk> Callback;

		public delegate ChunkTask ChunkTaskDelegate(Chunk chunk, GenerationParameters parameters);
	}
}
