using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

internal static class OVRDeserialize
{
	public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
	{
		GCHandle gchandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		T result;
		try
		{
			result = (T)((object)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T)));
		}
		finally
		{
			gchandle.Free();
		}
		return result;
	}

	public unsafe static T MarshalEntireStructAs<T>(this OVRPlugin.EventDataBuffer eventDataBuffer, Allocator allocator = Allocator.Temp)
	{
		T result;
		using (NativeArray<byte> nativeArray = new NativeArray<byte>(eventDataBuffer.EventData.Length + 4, allocator, NativeArrayOptions.ClearMemory))
		{
			byte* unsafePtr = (byte*)nativeArray.GetUnsafePtr<byte>();
			byte[] eventData;
			byte* source;
			if ((eventData = eventDataBuffer.EventData) == null || eventData.Length == 0)
			{
				source = null;
			}
			else
			{
				source = &eventData[0];
			}
			*(int*)unsafePtr = (int)eventDataBuffer.EventType;
			UnsafeUtility.MemCpy((void*)(unsafePtr + 4), (void*)source, (long)eventDataBuffer.EventData.Length);
			result = Marshal.PtrToStructure<T>(new IntPtr((void*)unsafePtr));
		}
		return result;
	}

	public struct DisplayRefreshRateChangedData
	{
		public float FromRefreshRate;

		public float ToRefreshRate;
	}

	public struct SpaceQueryResultsData
	{
		public ulong RequestId;
	}

	public struct SpaceQueryCompleteData
	{
		public ulong RequestId;

		public int Result;
	}

	public struct SceneCaptureCompleteData
	{
		public ulong RequestId;

		public int Result;
	}

	public struct SpatialAnchorCreateCompleteData
	{
		public ulong RequestId;

		public int Result;

		public ulong Space;

		public Guid Uuid;
	}

	public struct SpaceSetComponentStatusCompleteData
	{
		public ulong RequestId;

		public int Result;

		public ulong Space;

		public Guid Uuid;

		public OVRPlugin.SpaceComponentType ComponentType;

		public int Enabled;
	}

	public struct SpaceSaveCompleteData
	{
		public ulong RequestId;

		public ulong Space;

		public int Result;

		public Guid Uuid;
	}

	public struct SpaceEraseCompleteData
	{
		public ulong RequestId;

		public int Result;

		public Guid Uuid;

		public OVRPlugin.SpaceStorageLocation Location;
	}

	public struct SpaceShareResultData
	{
		public ulong RequestId;

		public int Result;
	}

	public struct SpaceListSaveResultData
	{
		public ulong RequestId;

		public int Result;
	}

	public struct StartColocationSessionAdvertisementCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;

		public Guid AdvertisementUuid;
	}

	public struct StopColocationSessionAdvertisementCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;
	}

	public struct StartColocationSessionDiscoveryCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;
	}

	public struct StopColocationSessionDiscoveryCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;
	}

	public struct ColocationSessionDiscoveryResultData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public Guid AdvertisementUuid;

		public uint AdvertisementMetadataCount;

		[FixedBuffer(typeof(byte), 1024)]
		public OVRDeserialize.ColocationSessionDiscoveryResultData.<AdvertisementMetadata>e__FixedBuffer AdvertisementMetadata;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1024)]
		public struct <AdvertisementMetadata>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}

	public struct ColocationSessionAdvertisementCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;
	}

	public struct ColocationSessionDiscoveryCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;
	}

	public struct ShareSpacesToGroupsCompleteData
	{
		public OVRPlugin.EventType EventType;

		public ulong RequestId;

		public OVRPlugin.Result Result;
	}

	public struct SpaceDiscoveryCompleteData
	{
		public ulong RequestId;

		public int Result;
	}

	public struct SpaceDiscoveryResultsData
	{
		public ulong RequestId;
	}

	public struct SpacesSaveResultData
	{
		public ulong RequestId;

		public OVRAnchor.SaveResult Result;
	}

	public struct SpacesEraseResultData
	{
		public ulong RequestId;

		public OVRAnchor.EraseResult Result;
	}

	public struct PassthroughLayerResumedData
	{
		public int LayerId;
	}

	public struct BoundaryVisibilityChangedData
	{
		public OVRPlugin.BoundaryVisibility BoundaryVisibility;
	}

	public struct CreateDynamicObjectTrackerResultData
	{
		public OVRPlugin.EventType EventType;

		public ulong Tracker;

		public OVRPlugin.Result Result;
	}

	public struct SetDynamicObjectTrackedClassesResultData
	{
		public OVRPlugin.EventType EventType;

		public ulong Tracker;

		public OVRPlugin.Result Result;
	}

	public struct EventDataReferenceSpaceChangePending
	{
		public OVRPlugin.EventType EventType;

		public OVRPlugin.TrackingOrigin ReferenceSpaceType;

		public double ChangeTime;

		public OVRPlugin.Bool PoseValid;

		public OVRPlugin.Posef PoseInPreviousSpace;
	}
}
