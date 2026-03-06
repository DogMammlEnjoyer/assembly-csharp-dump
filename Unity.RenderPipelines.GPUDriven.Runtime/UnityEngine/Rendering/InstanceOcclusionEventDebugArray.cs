using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct InstanceOcclusionEventDebugArray : IDisposable
	{
		public GraphicsBuffer CounterBuffer
		{
			get
			{
				return this.m_CounterBuffer;
			}
		}

		public void Init()
		{
			this.m_CounterBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 256, 4);
			this.m_PendingInfo = new UnsafeList<InstanceOcclusionEventDebugArray.Info>(4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this.m_Requests = new NativeQueue<InstanceOcclusionEventDebugArray.Request>(Allocator.Persistent);
		}

		public void Dispose()
		{
			if (this.m_HasLatest)
			{
				this.m_LatestInfo.Dispose();
				this.m_LatestCounters.Dispose();
				this.m_HasLatest = false;
			}
			InstanceOcclusionEventDebugArray.Request request;
			while (this.m_Requests.TryDequeue(out request))
			{
				request.readback.WaitForCompletion();
				request.info.Dispose();
			}
			this.m_Requests.Dispose();
			this.m_PendingInfo.Dispose();
			this.m_CounterBuffer.Dispose();
		}

		public int TryAdd(int viewInstanceID, InstanceOcclusionEventType eventType, int occluderVersion, int subviewMask, OcclusionTest occlusionTest)
		{
			int length = this.m_PendingInfo.Length;
			if (length + 1 > 64)
			{
				return -1;
			}
			InstanceOcclusionEventDebugArray.Info info = default(InstanceOcclusionEventDebugArray.Info);
			info.viewInstanceID = viewInstanceID;
			info.eventType = eventType;
			info.occluderVersion = occluderVersion;
			info.subviewMask = subviewMask;
			info.occlusionTest = occlusionTest;
			this.m_PendingInfo.Add(info);
			return length;
		}

		public void MoveToDebugStatsAndClear(DebugRendererBatcherStats debugStats)
		{
			if (this.m_PendingInfo.Length > 0)
			{
				InstanceOcclusionEventDebugArray.Request value = new InstanceOcclusionEventDebugArray.Request
				{
					info = this.m_PendingInfo,
					readback = AsyncGPUReadback.Request(this.m_CounterBuffer, this.m_PendingInfo.Length * 4 * 4, 0, null)
				};
				this.m_Requests.Enqueue(value);
				this.m_PendingInfo = new UnsafeList<InstanceOcclusionEventDebugArray.Info>(4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			}
			while (!this.m_Requests.IsEmpty())
			{
				InstanceOcclusionEventDebugArray.Request value = this.m_Requests.Peek();
				if (!value.readback.done)
				{
					break;
				}
				InstanceOcclusionEventDebugArray.Request request = this.m_Requests.Dequeue();
				if (!request.readback.hasError)
				{
					NativeArray<int> data = request.readback.GetData<int>(0);
					if (data.Length == request.info.Length * 4)
					{
						if (this.m_HasLatest)
						{
							this.m_LatestInfo.Dispose();
							this.m_LatestCounters.Dispose();
							this.m_HasLatest = false;
						}
						this.m_LatestInfo = request.info;
						this.m_LatestCounters = new NativeArray<int>(data, Allocator.Persistent);
						this.m_HasLatest = true;
					}
				}
			}
			debugStats.instanceOcclusionEventStats.Clear();
			if (this.m_HasLatest)
			{
				for (int i = 0; i < this.m_LatestInfo.Length; i++)
				{
					InstanceOcclusionEventDebugArray.Info info = this.m_LatestInfo[i];
					int occluderVersion = -1;
					if (info.HasVersion())
					{
						occluderVersion = 0;
						for (int j = 0; j < i; j++)
						{
							InstanceOcclusionEventDebugArray.Info info2 = this.m_LatestInfo[j];
							if (info2.HasVersion() && info2.viewInstanceID == info.viewInstanceID)
							{
								occluderVersion = info.occluderVersion - info2.occluderVersion;
								break;
							}
						}
					}
					int num = i * 4;
					int culledInstances = this.m_LatestCounters[num];
					int visibleInstances = this.m_LatestCounters[num + 1];
					int culledPrimitives = this.m_LatestCounters[num + 2];
					int visiblePrimitives = this.m_LatestCounters[num + 3];
					InstanceOcclusionEventStats instanceOcclusionEventStats = default(InstanceOcclusionEventStats);
					instanceOcclusionEventStats.viewInstanceID = info.viewInstanceID;
					instanceOcclusionEventStats.eventType = info.eventType;
					instanceOcclusionEventStats.occluderVersion = occluderVersion;
					instanceOcclusionEventStats.subviewMask = info.subviewMask;
					instanceOcclusionEventStats.occlusionTest = info.occlusionTest;
					instanceOcclusionEventStats.visibleInstances = visibleInstances;
					instanceOcclusionEventStats.culledInstances = culledInstances;
					instanceOcclusionEventStats.visiblePrimitives = visiblePrimitives;
					instanceOcclusionEventStats.culledPrimitives = culledPrimitives;
					debugStats.instanceOcclusionEventStats.Add(instanceOcclusionEventStats);
				}
			}
			NativeArray<int> data2 = new NativeArray<int>(256, Allocator.Temp, NativeArrayOptions.ClearMemory);
			this.m_CounterBuffer.SetData<int>(data2);
			data2.Dispose();
		}

		private const int InitialPassCount = 4;

		private const int MaxPassCount = 64;

		private GraphicsBuffer m_CounterBuffer;

		private UnsafeList<InstanceOcclusionEventDebugArray.Info> m_PendingInfo;

		private NativeQueue<InstanceOcclusionEventDebugArray.Request> m_Requests;

		private UnsafeList<InstanceOcclusionEventDebugArray.Info> m_LatestInfo;

		private NativeArray<int> m_LatestCounters;

		private bool m_HasLatest;

		internal struct Info
		{
			public bool HasVersion()
			{
				return this.eventType == InstanceOcclusionEventType.OccluderUpdate || this.occlusionTest > OcclusionTest.None;
			}

			public int viewInstanceID;

			public InstanceOcclusionEventType eventType;

			public int occluderVersion;

			public int subviewMask;

			public OcclusionTest occlusionTest;
		}

		internal struct Request
		{
			public UnsafeList<InstanceOcclusionEventDebugArray.Info> info;

			public AsyncGPUReadbackRequest readback;
		}
	}
}
