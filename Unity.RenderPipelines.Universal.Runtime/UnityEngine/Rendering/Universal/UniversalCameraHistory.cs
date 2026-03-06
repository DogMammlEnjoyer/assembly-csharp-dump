using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalCameraHistory : ICameraHistoryReadAccess, ICameraHistoryWriteAccess, IPerFrameHistoryAccessTracker, IDisposable
	{
		public void RequestAccess<Type>() where Type : ContextItem
		{
			uint value = UniversalCameraHistory.TypeId<Type>.value;
			if ((ulong)value >= (ulong)((long)this.m_Items.Length))
			{
				UniversalCameraHistory.Item[] array = new UniversalCameraHistory.Item[math.max((long)((ulong)math.ceilpow2(UniversalCameraHistory.s_TypeCount)), (long)(this.m_Items.Length * 2))];
				for (int i = 0; i < this.m_Items.Length; i++)
				{
					array[i] = this.m_Items[i];
				}
				this.m_Items = array;
			}
			this.m_Items[(int)value].requestVersion = this.m_Version;
		}

		public Type GetHistoryForRead<Type>() where Type : ContextItem
		{
			uint value = UniversalCameraHistory.TypeId<Type>.value;
			if ((ulong)value >= (ulong)((long)this.m_Items.Length))
			{
				return default(Type);
			}
			if (!this.IsValid((int)value))
			{
				return default(Type);
			}
			return (Type)((object)this.m_Items[(int)value].storage);
		}

		public bool IsAccessRequested<Type>() where Type : ContextItem
		{
			uint value = UniversalCameraHistory.TypeId<Type>.value;
			return (ulong)value < (ulong)((long)this.m_Items.Length) && this.IsValidRequest((int)value);
		}

		public Type GetHistoryForWrite<Type>() where Type : ContextItem, new()
		{
			uint value = UniversalCameraHistory.TypeId<Type>.value;
			if ((ulong)value >= (ulong)((long)this.m_Items.Length))
			{
				return default(Type);
			}
			if (!this.IsValidRequest((int)value))
			{
				return default(Type);
			}
			if (this.m_Items[(int)value].storage == null)
			{
				UniversalCameraHistory.Item[] items = this.m_Items;
				uint num = value;
				items[(int)num].storage = Activator.CreateInstance<Type>();
				CameraHistoryItem cameraHistoryItem = items[(int)num].storage as CameraHistoryItem;
				if (cameraHistoryItem != null)
				{
					cameraHistoryItem.OnCreate(this.m_HistoryTextures, value);
				}
			}
			this.m_Items[(int)value].writeVersion = this.m_Version;
			return (Type)((object)this.m_Items[(int)value].storage);
		}

		public bool IsWritten<Type>() where Type : ContextItem
		{
			uint value = UniversalCameraHistory.TypeId<Type>.value;
			return (ulong)value < (ulong)((long)this.m_Items.Length) && this.m_Items[(int)value].writeVersion == this.m_Version;
		}

		public event ICameraHistoryReadAccess.HistoryRequestDelegate OnGatherHistoryRequests;

		internal UniversalCameraHistory()
		{
			for (int i = 0; i < this.m_Items.Length; i++)
			{
				this.m_Items[i].Reset();
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < this.m_Items.Length; i++)
			{
				this.m_Items[i].Reset();
			}
			this.m_HistoryTextures.ReleaseAll();
		}

		internal void GatherHistoryRequests()
		{
			ICameraHistoryReadAccess.HistoryRequestDelegate onGatherHistoryRequests = this.OnGatherHistoryRequests;
			if (onGatherHistoryRequests == null)
			{
				return;
			}
			onGatherHistoryRequests(this);
		}

		private bool IsValidRequest(int i)
		{
			return this.m_Version - this.m_Items[i].requestVersion < 2;
		}

		private bool IsValid(int i)
		{
			return this.m_Version - this.m_Items[i].writeVersion < 2;
		}

		internal void ReleaseUnusedHistory()
		{
			for (int i = 0; i < this.m_Items.Length; i++)
			{
				if (!this.IsValidRequest(i) && !this.IsValid(i))
				{
					this.m_Items[i].Reset();
				}
			}
			this.m_Version++;
		}

		internal void SwapAndSetReferenceSize(int cameraWidth, int cameraHeight)
		{
			this.m_HistoryTextures.SwapAndSetReferenceSize(cameraWidth, cameraHeight);
		}

		private const int k_ValidVersionCount = 2;

		private static uint s_TypeCount;

		private UniversalCameraHistory.Item[] m_Items = new UniversalCameraHistory.Item[32];

		private int m_Version;

		private BufferedRTHandleSystem m_HistoryTextures = new BufferedRTHandleSystem();

		private static class TypeId<T>
		{
			public static uint value = UniversalCameraHistory.s_TypeCount++;
		}

		private struct Item
		{
			public void Reset()
			{
				ContextItem contextItem = this.storage;
				if (contextItem != null)
				{
					contextItem.Reset();
				}
				this.requestVersion = -2;
				this.writeVersion = -2;
			}

			public ContextItem storage;

			public int requestVersion;

			public int writeVersion;
		}
	}
}
