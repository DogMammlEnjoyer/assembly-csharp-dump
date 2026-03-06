using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public readonly struct OVRAnchor : IEquatable<OVRAnchor>, IDisposable
{
	internal static void OnSpaceDiscoveryComplete(OVRDeserialize.SpaceDiscoveryCompleteData data)
	{
		OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> ovrtask;
		if (!OVRTask.TryGetPendingTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>(data.RequestId, out ovrtask))
		{
			return;
		}
		OVRAnchor.FetchTaskData fetchTaskData;
		OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult> result;
		if (ovrtask.TryGetInternalData<OVRAnchor.FetchTaskData>(out fetchTaskData))
		{
			if (OVRAnchor.Telemetry.GetMarker(OVRAnchor.Telemetry.MarkerId.DiscoverSpaces, data.RequestId) != null)
			{
				OVRTelemetryMarker? ovrtelemetryMarker;
				OVRTelemetryMarker valueOrDefault = ovrtelemetryMarker.GetValueOrDefault();
				string annotationKey = "results_count";
				List<OVRAnchor> anchors = fetchTaskData.Anchors;
				valueOrDefault.AddAnnotation(annotationKey, (long)((anchors != null) ? anchors.Count : 0));
			}
			result = OVRResult.From<List<OVRAnchor>, OVRAnchor.FetchResult>(fetchTaskData.Anchors, (OVRAnchor.FetchResult)data.Result);
		}
		else
		{
			Debug.LogError("SpaceDiscovery completed but its task does not have an associated anchor List. " + string.Format("RequestId={0}, Result={1}", data.RequestId, data.Result));
			result = OVRResult.From<List<OVRAnchor>, OVRAnchor.FetchResult>(null, (OVRAnchor.FetchResult)data.Result);
		}
		OVRAnchor.Telemetry.SetAsyncResultAndSend(OVRAnchor.Telemetry.MarkerId.DiscoverSpaces, data.RequestId, (long)data.Result);
		ovrtask.SetResult(result);
	}

	internal unsafe static void OnSpaceDiscoveryResultsAvailable(OVRDeserialize.SpaceDiscoveryResultsData data)
	{
		ulong requestId = data.RequestId;
		OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> ovrtask;
		if (!OVRTask.TryGetPendingTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>(requestId, out ovrtask))
		{
			return;
		}
		OVRAnchor.FetchTaskData fetchTaskData;
		if (!ovrtask.TryGetInternalData<OVRAnchor.FetchTaskData>(out fetchTaskData))
		{
			return;
		}
		NativeArray<OVRPlugin.SpaceDiscoveryResult> nativeArray = default(NativeArray<OVRPlugin.SpaceDiscoveryResult>);
		int num;
		OVRPlugin.Result result = OVRPlugin.RetrieveSpaceDiscoveryResults(requestId, null, 0, out num);
		if (!result.IsSuccess())
		{
			return;
		}
		do
		{
			if (nativeArray.IsCreated)
			{
				nativeArray.Dispose();
			}
			nativeArray = new NativeArray<OVRPlugin.SpaceDiscoveryResult>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
			result = OVRPlugin.RetrieveSpaceDiscoveryResults(requestId, (OVRPlugin.SpaceDiscoveryResult*)nativeArray.GetUnsafePtr<OVRPlugin.SpaceDiscoveryResult>(), nativeArray.Length, out num);
		}
		while (result == OVRPlugin.Result.Failure_InsufficientSize);
		int count = fetchTaskData.Anchors.Count;
		using (nativeArray)
		{
			if (!result.IsSuccess() || num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				OVRPlugin.SpaceDiscoveryResult spaceDiscoveryResult = nativeArray[i];
				fetchTaskData.Anchors.Add(new OVRAnchor(spaceDiscoveryResult.Space, spaceDiscoveryResult.Uuid));
			}
		}
		Action<List<OVRAnchor>, int> incrementalResultsCallback = fetchTaskData.IncrementalResultsCallback;
		if (incrementalResultsCallback == null)
		{
			return;
		}
		incrementalResultsCallback(fetchTaskData.Anchors, count);
	}

	public static OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchAnchorsAsync(List<OVRAnchor> anchors, OVRAnchor.FetchOptions options, Action<List<OVRAnchor>, int> incrementalResultsCallback = null)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		anchors.Clear();
		ulong requestId;
		return OVRTask.Build(options.DiscoverSpaces(out requestId), requestId).ToTask<List<OVRAnchor>, OVRAnchor.FetchResult>().WithInternalData<OVRAnchor.FetchTaskData>(new OVRAnchor.FetchTaskData
		{
			Anchors = anchors,
			IncrementalResultsCallback = incrementalResultsCallback
		});
	}

	public static OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchSharedAnchorsAsync(Guid groupUuid, List<OVRAnchor> anchors)
	{
		OVRAnchor.<FetchSharedAnchorsAsync>d__9 <FetchSharedAnchorsAsync>d__;
		<FetchSharedAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>.Create();
		<FetchSharedAnchorsAsync>d__.groupUuid = groupUuid;
		<FetchSharedAnchorsAsync>d__.anchors = anchors;
		<FetchSharedAnchorsAsync>d__.<>1__state = -1;
		<FetchSharedAnchorsAsync>d__.<>t__builder.Start<OVRAnchor.<FetchSharedAnchorsAsync>d__9>(ref <FetchSharedAnchorsAsync>d__);
		return <FetchSharedAnchorsAsync>d__.<>t__builder.Task;
	}

	public static OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchSharedAnchorsAsync(Guid groupUuid, IEnumerable<Guid> allowedAnchorUuids, List<OVRAnchor> anchors)
	{
		OVRAnchor.<FetchSharedAnchorsAsync>d__10 <FetchSharedAnchorsAsync>d__;
		<FetchSharedAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>.Create();
		<FetchSharedAnchorsAsync>d__.groupUuid = groupUuid;
		<FetchSharedAnchorsAsync>d__.allowedAnchorUuids = allowedAnchorUuids;
		<FetchSharedAnchorsAsync>d__.anchors = anchors;
		<FetchSharedAnchorsAsync>d__.<>1__state = -1;
		<FetchSharedAnchorsAsync>d__.<>t__builder.Start<OVRAnchor.<FetchSharedAnchorsAsync>d__10>(ref <FetchSharedAnchorsAsync>d__);
		return <FetchSharedAnchorsAsync>d__.<>t__builder.Task;
	}

	public static OVRTask<OVRAnchor> CreateSpatialAnchorAsync(Pose trackingSpacePose)
	{
		ulong requestId;
		return OVRTask.Build(OVRPlugin.CreateSpatialAnchor(new OVRPlugin.SpatialAnchorCreateInfo
		{
			BaseTracking = OVRPlugin.GetTrackingOriginType(),
			PoseInSpace = new OVRPlugin.Posef
			{
				Orientation = trackingSpacePose.rotation.ToFlippedZQuatf(),
				Position = trackingSpacePose.position.ToFlippedZVector3f()
			},
			Time = OVRPlugin.GetTimeInSeconds()
		}, out requestId), requestId).ToTask<OVRAnchor>(OVRAnchor.Null);
	}

	public static OVRTask<OVRAnchor> CreateSpatialAnchorAsync(Transform transform, Camera centerEyeCamera)
	{
		if (transform == null)
		{
			throw new ArgumentNullException("transform");
		}
		if (centerEyeCamera == null)
		{
			throw new ArgumentNullException("centerEyeCamera");
		}
		OVRPose ovrpose = transform.ToTrackingSpacePose(centerEyeCamera);
		return OVRAnchor.CreateSpatialAnchorAsync(new Pose
		{
			position = ovrpose.position,
			rotation = ovrpose.orientation
		});
	}

	public unsafe OVRTask<OVRResult<OVRAnchor.SaveResult>> SaveAsync()
	{
		ulong handle = this.Handle;
		return OVRAnchor.SaveSpacesAsync(new ReadOnlySpan<ulong>((void*)(&handle), 1));
	}

	public static OVRTask<OVRResult<OVRAnchor.SaveResult>> SaveAsync(IEnumerable<OVRAnchor> anchors)
	{
		OVRTask<OVRResult<OVRAnchor.SaveResult>> result;
		using (OVRNativeList<ulong> list = OVRNativeList.WithSuggestedCapacityFrom<OVRAnchor>(anchors).AllocateEmpty<ulong>(Allocator.Temp))
		{
			foreach (OVRAnchor ovranchor in anchors.ToNonAlloc<OVRAnchor>())
			{
				list.Add(ovranchor.Handle);
			}
			if (list.Count == 0)
			{
				result = OVRTask.FromResult<OVRResult<OVRAnchor.SaveResult>>(OVRResult.From<OVRAnchor.SaveResult>(OVRAnchor.SaveResult.Success));
			}
			else
			{
				result = OVRAnchor.SaveSpacesAsync(list);
			}
		}
		return result;
	}

	internal unsafe static OVRTask<OVRResult<OVRAnchor.SaveResult>> SaveSpacesAsync(ReadOnlySpan<ulong> spaces)
	{
		OVRTelemetryMarker marker = OVRTelemetry.Start(163056974, 0, -1L).AddAnnotation("space_count", (long)spaces.Length);
		fixed (ulong* pinnableReference = spaces.GetPinnableReference())
		{
			ulong requestId;
			OVRPlugin.Result result = OVRPlugin.SaveSpaces(pinnableReference, spaces.Length, out requestId);
			OVRAnchor.Telemetry.SetSyncResult(marker, requestId, result);
			return OVRTask.Build(result, requestId).ToResultTask<OVRAnchor.SaveResult>();
		}
	}

	internal static void OnSaveSpacesResult(OVRDeserialize.SpacesSaveResultData eventData)
	{
		OVRAnchor.Telemetry.SetAsyncResultAndSend(OVRAnchor.Telemetry.MarkerId.SaveSpaces, eventData.RequestId, (long)eventData.Result);
	}

	public unsafe OVRTask<OVRResult<OVRAnchor.EraseResult>> EraseAsync()
	{
		Guid uuid = this.Uuid;
		return OVRAnchor.EraseSpacesAsync(default(ReadOnlySpan<ulong>), new ReadOnlySpan<Guid>((void*)(&uuid), 1));
	}

	public static OVRTask<OVRResult<OVRAnchor.EraseResult>> EraseAsync(IEnumerable<OVRAnchor> anchors, IEnumerable<Guid> uuids)
	{
		if (anchors == null && uuids == null)
		{
			throw new ArgumentException("One of anchors or uuids must not be null.");
		}
		OVRTask<OVRResult<OVRAnchor.EraseResult>> result;
		using (OVRNativeList<ulong> list = OVRNativeList.WithSuggestedCapacityFrom<OVRAnchor>(anchors).AllocateEmpty<ulong>(Allocator.Temp))
		{
			foreach (OVRAnchor ovranchor in anchors.ToNonAlloc<OVRAnchor>())
			{
				list.Add(ovranchor.Handle);
			}
			using (OVRNativeList<Guid> list2 = uuids.ToNativeList(Allocator.Temp))
			{
				if (list.Count == 0 && list2.Count == 0)
				{
					result = OVRTask.FromResult<OVRResult<OVRAnchor.EraseResult>>(OVRResult.From<OVRAnchor.EraseResult>(OVRAnchor.EraseResult.Success));
				}
				else
				{
					result = OVRAnchor.EraseSpacesAsync(list, list2);
				}
			}
		}
		return result;
	}

	private unsafe static OVRTask<OVRResult<OVRAnchor.EraseResult>> EraseSpacesAsync(ReadOnlySpan<ulong> spaces, ReadOnlySpan<Guid> uuids)
	{
		OVRTelemetryMarker marker = OVRTelemetry.Start(163061838, 0, -1L).AddAnnotation("space_count", (long)spaces.Length).AddAnnotation("uuid_count", (long)uuids.Length);
		fixed (ulong* pinnableReference = spaces.GetPinnableReference())
		{
			ulong* spaces2 = pinnableReference;
			fixed (Guid* pinnableReference2 = uuids.GetPinnableReference())
			{
				Guid* uuids2 = pinnableReference2;
				ulong requestId;
				OVRPlugin.Result result = OVRPlugin.EraseSpaces((uint)spaces.Length, spaces2, (uint)uuids.Length, uuids2, out requestId);
				OVRAnchor.Telemetry.SetSyncResult(marker, requestId, result);
				return OVRTask.Build(result, requestId).ToResultTask<OVRAnchor.EraseResult>();
			}
		}
	}

	internal static void OnEraseSpacesResult(OVRDeserialize.SpacesEraseResultData eventData)
	{
		OVRAnchor.Telemetry.SetAsyncResultAndSend(OVRAnchor.Telemetry.MarkerId.EraseSpaces, eventData.RequestId, (long)eventData.Result);
	}

	public unsafe OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(IEnumerable<OVRSpaceUser> users)
	{
		if (users == null)
		{
			throw new ArgumentNullException("users");
		}
		OVRTask<OVRResult<OVRAnchor.ShareResult>> result;
		using (OVRNativeList<ulong> list = OVRNativeList.WithSuggestedCapacityFrom<OVRSpaceUser>(users).AllocateEmpty<ulong>(Allocator.Temp))
		{
			foreach (OVRSpaceUser ovrspaceUser in users.ToNonAlloc<OVRSpaceUser>())
			{
				list.Add(ovrspaceUser._handle);
			}
			if (list.Count < 1)
			{
				throw new ArgumentException("users must contain at least one user.");
			}
			ulong handle = this.Handle;
			result = OVRAnchor.ShareSpacesAsync(new ReadOnlySpan<ulong>((void*)(&handle), 1), list);
		}
		return result;
	}

	public static OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(IEnumerable<OVRAnchor> anchors, IEnumerable<OVRSpaceUser> users)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		if (users == null)
		{
			throw new ArgumentNullException("users");
		}
		OVRTask<OVRResult<OVRAnchor.ShareResult>> result;
		using (OVRNativeList<ulong> list = OVRNativeList.WithSuggestedCapacityFrom<OVRAnchor>(anchors).AllocateEmpty<ulong>(Allocator.Temp))
		{
			foreach (OVRAnchor ovranchor in anchors.ToNonAlloc<OVRAnchor>())
			{
				list.Add(ovranchor.Handle);
			}
			using (OVRNativeList<ulong> list2 = OVRNativeList.WithSuggestedCapacityFrom<OVRSpaceUser>(users).AllocateEmpty<ulong>(Allocator.Temp))
			{
				foreach (OVRSpaceUser ovrspaceUser in users.ToNonAlloc<OVRSpaceUser>())
				{
					list2.Add(ovrspaceUser._handle);
				}
				if (list2.Count < 1)
				{
					throw new ArgumentException("users must contain at least one user.");
				}
				if (list.Count == 0)
				{
					result = OVRTask.FromResult<OVRResult<OVRAnchor.ShareResult>>(OVRResult.From<OVRAnchor.ShareResult>(OVRAnchor.ShareResult.Success));
				}
				else
				{
					result = OVRAnchor.ShareSpacesAsync(list, list2);
				}
			}
		}
		return result;
	}

	private unsafe static OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareSpacesAsync(ReadOnlySpan<ulong> spaces, ReadOnlySpan<ulong> users)
	{
		fixed (ulong* pinnableReference = spaces.GetPinnableReference())
		{
			ulong* spaces2 = pinnableReference;
			fixed (ulong* pinnableReference2 = users.GetPinnableReference())
			{
				ulong* userHandles = pinnableReference2;
				ulong requestId;
				return OVRTask.Build(OVRPlugin.ShareSpaces(spaces2, (uint)spaces.Length, userHandles, (uint)users.Length, out requestId), requestId).ToResultTask<OVRAnchor.ShareResult>();
			}
		}
	}

	public unsafe OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(Guid groupUuid)
	{
		ulong handle = this.Handle;
		ReadOnlySpan<ulong> anchors = new ReadOnlySpan<ulong>((void*)(&handle), 1);
		ReadOnlySpan<Guid> groupUuids = new ReadOnlySpan<Guid>((void*)(&groupUuid), 1);
		return OVRAnchor.ShareAsyncInternal(anchors, groupUuids);
	}

	public unsafe static OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(IEnumerable<OVRAnchor> anchors, Guid groupUuid)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		OVREnumerable<OVRAnchor> ovrenumerable = anchors.ToNonAlloc<OVRAnchor>();
		OVRTask<OVRResult<OVRAnchor.ShareResult>> result;
		using (OVRNativeList<ulong> list = new OVRNativeList<ulong>(ovrenumerable.Count, Allocator.Temp))
		{
			foreach (OVRAnchor ovranchor in ovrenumerable)
			{
				list.Add(ovranchor.Handle);
			}
			ReadOnlySpan<Guid> groupUuids = new ReadOnlySpan<Guid>((void*)(&groupUuid), 1);
			result = OVRAnchor.ShareAsyncInternal(list, groupUuids);
		}
		return result;
	}

	internal unsafe static OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsyncInternal(ReadOnlySpan<ulong> anchors, ReadOnlySpan<Guid> groupUuids)
	{
		OVRPlugin.ShareSpacesInfo shareSpacesInfo = default(OVRPlugin.ShareSpacesInfo);
		shareSpacesInfo.RecipientType = OVRPlugin.ShareSpacesRecipientType.Group;
		fixed (ulong* pinnableReference = anchors.GetPinnableReference())
		{
			ulong* spaces = pinnableReference;
			fixed (Guid* pinnableReference2 = groupUuids.GetPinnableReference())
			{
				Guid* groupUuids2 = pinnableReference2;
				shareSpacesInfo.Spaces = spaces;
				shareSpacesInfo.SpaceCount = (uint)anchors.Length;
				OVRPlugin.ShareSpacesGroupRecipientInfo shareSpacesGroupRecipientInfo = new OVRPlugin.ShareSpacesGroupRecipientInfo
				{
					GroupCount = (uint)groupUuids.Length,
					GroupUuids = groupUuids2
				};
				shareSpacesInfo.RecipientInfo = (OVRPlugin.ShareSpacesRecipientInfoBase*)(&shareSpacesGroupRecipientInfo);
				ulong requestId;
				return OVRTask.Build(OVRPlugin.ShareSpaces(shareSpacesInfo, out requestId), requestId).ToResultTask<OVRAnchor.ShareResult>();
			}
		}
	}

	internal static void OnShareAnchorsToGroupsComplete(ulong requestId, OVRPlugin.Result result)
	{
		OVRTask.SetResult<OVRResult<OVRAnchor.ShareResult>>(requestId, OVRResult.From<OVRAnchor.ShareResult>((OVRAnchor.ShareResult)result));
	}

	internal ulong Handle { get; }

	public Guid Uuid { get; }

	internal OVRAnchor(ulong handle, Guid uuid)
	{
		this.Handle = handle;
		this.Uuid = uuid;
	}

	public T GetComponent<T>() where T : struct, IOVRAnchorComponent<T>
	{
		T result;
		if (!this.TryGetComponent<T>(out result))
		{
			throw new InvalidOperationException(string.Format("Anchor {0} does not have component {1}", this.Uuid, typeof(T).Name));
		}
		return result;
	}

	public bool TryGetComponent<T>(out T component) where T : struct, IOVRAnchorComponent<T>
	{
		component = default(T);
		bool flag;
		bool flag2;
		if (!OVRPlugin.GetSpaceComponentStatusInternal(this.Handle, component.Type, out flag, out flag2).IsSuccess())
		{
			return false;
		}
		component = component.FromAnchor(this);
		return true;
	}

	public bool SupportsComponent<T>() where T : struct, IOVRAnchorComponent<T>
	{
		ulong handle = this.Handle;
		T t = default(T);
		bool flag;
		bool flag2;
		return OVRPlugin.GetSpaceComponentStatusInternal(handle, t.Type, out flag, out flag2).IsSuccess();
	}

	public unsafe bool GetSupportedComponents(List<OVRPlugin.SpaceComponentType> components)
	{
		components.Clear();
		uint num;
		if (!OVRPlugin.EnumerateSpaceSupportedComponents(this.Handle, 0U, out num, null).IsSuccess())
		{
			return false;
		}
		OVRPlugin.SpaceComponentType* ptr = stackalloc OVRPlugin.SpaceComponentType[checked(unchecked((UIntPtr)num) * 4)];
		if (!OVRPlugin.EnumerateSpaceSupportedComponents(this.Handle, num, out num, ptr).IsSuccess())
		{
			return false;
		}
		for (uint num2 = 0U; num2 < num; num2 += 1U)
		{
			components.Add(ptr[(ulong)num2 * 4UL / 4UL]);
		}
		return true;
	}

	public bool Equals(OVRAnchor other)
	{
		return this.Handle.Equals(other.Handle) && this.Uuid.Equals(other.Uuid);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRAnchor)
		{
			OVRAnchor other = (OVRAnchor)obj;
			return this.Equals(other);
		}
		return false;
	}

	public static bool operator ==(OVRAnchor lhs, OVRAnchor rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRAnchor lhs, OVRAnchor rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override int GetHashCode()
	{
		return this.Handle.GetHashCode() * 486187739 + this.Uuid.GetHashCode();
	}

	public override string ToString()
	{
		return this.Uuid.ToString();
	}

	public void Dispose()
	{
		OVRPlugin.DestroySpace(this.Handle);
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void Init()
	{
		OVRAnchor._deferredTasks.Clear();
		OVRAnchor.Telemetry.OnInit();
	}

	internal unsafe static OVRTask<OVRPlugin.Result> FetchAnchors(IList<OVRAnchor> anchors, OVRPlugin.SpaceQueryInfo2 queryInfo)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		anchors.Clear();
		OVRTelemetryMarker marker = OVRTelemetry.Start(163069062, 0, -1L).AddAnnotation("timeout", queryInfo.Timeout).AddAnnotation("max_results", (long)queryInfo.MaxQuerySpaces).AddAnnotation("storage_location", (long)queryInfo.Location);
		switch (queryInfo.FilterType)
		{
		case OVRPlugin.SpaceQueryFilterType.Ids:
			marker.AddAnnotation("uuid_count", (long)queryInfo.IdInfo.NumIds);
			break;
		case OVRPlugin.SpaceQueryFilterType.Components:
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)8];
			*intPtr = (long)queryInfo.ComponentsInfo.Components[0];
			long* annotationValues = intPtr;
			marker.AddAnnotation("component_types", annotationValues, queryInfo.ComponentsInfo.NumComponents);
			break;
		}
		case OVRPlugin.SpaceQueryFilterType.Group:
			marker.AddAnnotation("group_count", 1L);
			break;
		}
		ulong requestId;
		OVRPlugin.Result result = OVRPlugin.QuerySpaces2(queryInfo, out requestId);
		OVRAnchor.Telemetry.SetSyncResult(marker, requestId, result);
		return OVRTask.Build(result, requestId).ToTask().WithInternalData<IList<OVRAnchor>>(anchors);
	}

	internal static OVRTask<bool> CreateDeferredSpaceComponentStatusTask(ulong space, OVRPlugin.SpaceComponentType componentType, bool enabledDesired, double timeout)
	{
		OVRAnchor.DeferredKey key = new OVRAnchor.DeferredKey
		{
			Space = space,
			ComponentType = componentType
		};
		List<OVRAnchor.DeferredValue> list;
		if (!OVRAnchor._deferredTasks.TryGetValue(key, out list))
		{
			list = OVRObjectPool.List<OVRAnchor.DeferredValue>();
			OVRAnchor._deferredTasks.Add(key, list);
		}
		OVRTask<bool> ovrtask = OVRTask.FromGuid<bool>(Guid.NewGuid());
		list.Add(new OVRAnchor.DeferredValue
		{
			EnabledDesired = enabledDesired,
			Task = ovrtask,
			Timeout = timeout,
			StartTime = Time.realtimeSinceStartup
		});
		return ovrtask;
	}

	internal static void OnSpaceSetComponentStatusComplete(OVRDeserialize.SpaceSetComponentStatusCompleteData eventData)
	{
		OVRAnchor.DeferredKey key = OVRAnchor.DeferredKey.FromEvent(eventData);
		List<OVRAnchor.DeferredValue> list;
		if (!OVRAnchor._deferredTasks.TryGetValue(key, out list))
		{
			return;
		}
		try
		{
			bool flag = eventData.Enabled != 0;
			for (int i = 0; i < list.Count; i++)
			{
				OVRAnchor.DeferredValue deferredValue = list[i];
				OVRTask<bool> task = deferredValue.Task;
				bool? flag2 = null;
				bool flag3;
				bool flag4;
				if (eventData.RequestId == deferredValue.RequestId)
				{
					flag2 = new bool?(eventData.Result >= 0);
				}
				else if (flag == deferredValue.EnabledDesired)
				{
					flag2 = new bool?(true);
				}
				else if (!OVRPlugin.GetSpaceComponentStatus(eventData.Space, eventData.ComponentType, out flag3, out flag4))
				{
					flag2 = new bool?(false);
				}
				else if (!flag4)
				{
					double num = deferredValue.Timeout;
					if (num > 0.0)
					{
						num -= (double)(Time.realtimeSinceStartup - deferredValue.StartTime);
						if (num <= 0.0)
						{
							flag2 = new bool?(false);
						}
					}
					if (flag2 == null)
					{
						ulong requestId;
						if (OVRPlugin.SetSpaceComponentStatus(eventData.Space, eventData.ComponentType, deferredValue.EnabledDesired, num, out requestId))
						{
							deferredValue.RequestId = requestId;
							list[i] = deferredValue;
						}
						else
						{
							flag2 = new bool?(false);
						}
					}
				}
				if (flag2 != null)
				{
					list.RemoveAt(i--);
					task.SetResult(flag2.Value);
				}
			}
		}
		finally
		{
			if (list.Count == 0)
			{
				OVRObjectPool.Return<List<OVRAnchor.DeferredValue>>(list);
				OVRAnchor._deferredTasks.Remove(key);
			}
		}
	}

	[Obsolete("Use the overload of FetchAnchorsAsync that accepts a FetchOptions parameter")]
	public static OVRTask<bool> FetchAnchorsAsync<T>(IList<OVRAnchor> anchors, OVRSpace.StorageLocation location = OVRSpace.StorageLocation.Local, int maxResults = 1024, double timeout = 0.0) where T : struct, IOVRAnchorComponent<T>
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		T t = default(T);
		return OVRAnchor.FetchAnchorsAsync(t.Type, anchors, location, maxResults, timeout);
	}

	[Obsolete("Use the overload of FetchAnchorsAsync that accepts a FetchOptions parameter")]
	public static OVRTask<bool> FetchAnchorsAsync(IEnumerable<Guid> uuids, IList<OVRAnchor> anchors, OVRSpace.StorageLocation location = OVRSpace.StorageLocation.Local, double timeout = 0.0)
	{
		OVRAnchor.<>c__DisplayClass54_0 CS$<>8__locals1 = new OVRAnchor.<>c__DisplayClass54_0();
		CS$<>8__locals1.uuids = uuids;
		CS$<>8__locals1.location = location;
		CS$<>8__locals1.timeout = timeout;
		CS$<>8__locals1.anchors = anchors;
		if (CS$<>8__locals1.uuids == null)
		{
			throw new ArgumentNullException("uuids");
		}
		if (CS$<>8__locals1.anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		return CS$<>8__locals1.<FetchAnchorsAsync>g__execute|0();
	}

	internal static void OnSpaceQueryComplete(OVRDeserialize.SpaceQueryCompleteData data)
	{
		OVRTelemetryMarker? ovrtelemetryMarker = null;
		OVRPlugin.Result? result = null;
		try
		{
			ovrtelemetryMarker = OVRAnchor.Telemetry.SetAsyncResult(OVRAnchor.Telemetry.MarkerId.QuerySpaces, data.RequestId, (long)data.Result);
			ulong requestId = data.RequestId;
			OVRTask<OVRPlugin.Result> ovrtask;
			if (OVRTask.TryGetPendingTask<OVRPlugin.Result>(data.RequestId, out ovrtask))
			{
				result = new OVRPlugin.Result?((OVRPlugin.Result)data.Result);
				if (data.Result >= 0)
				{
					IList<OVRAnchor> list;
					NativeArray<OVRPlugin.SpaceQueryResult> nativeArray;
					if (!ovrtask.TryGetInternalData<IList<OVRAnchor>>(out list) || list == null)
					{
						result = new OVRPlugin.Result?(OVRPlugin.Result.Failure_DataIsInvalid);
					}
					else if (!OVRPlugin.RetrieveSpaceQueryResults(requestId, out nativeArray, Allocator.Temp))
					{
						result = new OVRPlugin.Result?(OVRPlugin.Result.Failure_OperationFailed);
					}
					else
					{
						using (nativeArray)
						{
							if (ovrtelemetryMarker != null)
							{
								ovrtelemetryMarker.GetValueOrDefault().AddAnnotation("results_count", (long)nativeArray.Length);
							}
							foreach (OVRPlugin.SpaceQueryResult spaceQueryResult in nativeArray)
							{
								list.Add(new OVRAnchor(spaceQueryResult.space, spaceQueryResult.uuid));
							}
							result = new OVRPlugin.Result?((OVRPlugin.Result)data.Result);
						}
					}
				}
			}
		}
		finally
		{
			if (ovrtelemetryMarker != null)
			{
				ovrtelemetryMarker.GetValueOrDefault().Send();
			}
			if (result != null)
			{
				OVRTask.SetResult<OVRPlugin.Result>(data.RequestId, result.Value);
			}
		}
	}

	[Obsolete]
	internal static OVRTask<bool> FetchAnchorsAsync(OVRPlugin.SpaceComponentType type, IList<OVRAnchor> anchors, OVRSpace.StorageLocation location = OVRSpace.StorageLocation.Local, int maxResults = 1024, double timeout = 0.0)
	{
		OVRAnchor.<FetchAnchorsAsync>d__56 <FetchAnchorsAsync>d__;
		<FetchAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<bool>.Create();
		<FetchAnchorsAsync>d__.type = type;
		<FetchAnchorsAsync>d__.anchors = anchors;
		<FetchAnchorsAsync>d__.location = location;
		<FetchAnchorsAsync>d__.maxResults = maxResults;
		<FetchAnchorsAsync>d__.timeout = timeout;
		<FetchAnchorsAsync>d__.<>1__state = -1;
		<FetchAnchorsAsync>d__.<>t__builder.Start<OVRAnchor.<FetchAnchorsAsync>d__56>(ref <FetchAnchorsAsync>d__);
		return <FetchAnchorsAsync>d__.<>t__builder.Task;
	}

	[Obsolete]
	internal unsafe static OVRPlugin.Result SaveSpaceList(ulong* spaces, uint numSpaces, OVRPlugin.SpaceStorageLocation location, out ulong requestId)
	{
		OVRTelemetryMarker marker = OVRTelemetry.Start(163065048, 0, -1L).AddAnnotation("space_count", (long)((ulong)numSpaces)).AddAnnotation("storage_location", (long)location);
		OVRPlugin.Result result = OVRPlugin.SaveSpaceList(spaces, numSpaces, location, out requestId);
		OVRAnchor.Telemetry.SetSyncResult(marker, requestId, result);
		return result;
	}

	internal static void OnSpaceListSaveResult(OVRDeserialize.SpaceListSaveResultData eventData)
	{
		OVRAnchor.Telemetry.SetAsyncResultAndSend(OVRAnchor.Telemetry.MarkerId.SaveSpaceList, eventData.RequestId, (long)eventData.Result);
	}

	[Obsolete]
	internal static OVRPlugin.Result EraseSpace(ulong space, OVRPlugin.SpaceStorageLocation location, out ulong requestId)
	{
		OVRTelemetryMarker marker = OVRTelemetry.Start(163062284, 0, -1L).AddAnnotation("storage_location", (long)location);
		OVRPlugin.Result result = OVRPlugin.EraseSpaceWithResult(space, location, out requestId);
		OVRAnchor.Telemetry.SetSyncResult(marker, requestId, result);
		return result;
	}

	internal static void OnSpaceEraseComplete(OVRDeserialize.SpaceEraseCompleteData eventData)
	{
		OVRAnchor.Telemetry.SetAsyncResultAndSend(OVRAnchor.Telemetry.MarkerId.EraseSingleSpace, eventData.RequestId, (long)eventData.Result);
	}

	public OVRAnchor.TrackableType GetTrackableType()
	{
		List<OVRPlugin.SpaceComponentType> list;
		using (new OVRObjectPool.ListScope<OVRPlugin.SpaceComponentType>(ref list))
		{
			if (!this.GetSupportedComponents(list))
			{
				return OVRAnchor.TrackableType.None;
			}
			foreach (OVRPlugin.SpaceComponentType spaceComponentType in list)
			{
				if (spaceComponentType != OVRPlugin.SpaceComponentType.DynamicObject)
				{
					if (spaceComponentType == OVRPlugin.SpaceComponentType.MarkerPayload)
					{
						OVRMarkerPayload component = this.GetComponent<OVRMarkerPayload>();
						if (component.IsEnabled && component.PayloadType.IsQRCode())
						{
							return OVRAnchor.TrackableType.QRCode;
						}
					}
				}
				else
				{
					OVRDynamicObject component2 = this.GetComponent<OVRDynamicObject>();
					if (component2.IsEnabled)
					{
						return component2.TrackableType;
					}
				}
			}
		}
		return OVRAnchor.TrackableType.None;
	}

	private static void GetRequiredComponents(IEnumerable<OVRAnchor.TrackableType> trackableTypes, HashSet<OVRAnchor.TrackableType> trackableTypesOut, HashSet<OVRPlugin.SpaceComponentType> requiredComponentsOut)
	{
		foreach (OVRAnchor.TrackableType trackableType in trackableTypes.ToNonAlloc<OVRAnchor.TrackableType>())
		{
			trackableTypesOut.Add(trackableType);
			if (trackableType != OVRAnchor.TrackableType.Keyboard)
			{
				if (trackableType == OVRAnchor.TrackableType.QRCode)
				{
					requiredComponentsOut.Add(OVRPlugin.SpaceComponentType.MarkerPayload);
				}
			}
			else
			{
				requiredComponentsOut.Add(OVRPlugin.SpaceComponentType.DynamicObject);
			}
		}
	}

	public static OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchTrackablesAsync(List<OVRAnchor> anchors, IEnumerable<OVRAnchor.TrackableType> trackableTypes, Action<List<OVRAnchor>, int> incrementalResultsCallback = null)
	{
		OVRAnchor.<FetchTrackablesAsync>d__66 <FetchTrackablesAsync>d__;
		<FetchTrackablesAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>.Create();
		<FetchTrackablesAsync>d__.anchors = anchors;
		<FetchTrackablesAsync>d__.trackableTypes = trackableTypes;
		<FetchTrackablesAsync>d__.incrementalResultsCallback = incrementalResultsCallback;
		<FetchTrackablesAsync>d__.<>1__state = -1;
		<FetchTrackablesAsync>d__.<>t__builder.Start<OVRAnchor.<FetchTrackablesAsync>d__66>(ref <FetchTrackablesAsync>d__);
		return <FetchTrackablesAsync>d__.<>t__builder.Task;
	}

	[CompilerGenerated]
	internal static OVRTask<OVRPlugin.Result> <FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0(List<OVRAnchor> anchors, HashSet<OVRAnchor.TrackableType> trackableTypes, OVRPlugin.SpaceComponentType componentType, Action<List<OVRAnchor>, int> incrementalResultsCallback)
	{
		OVRAnchor.<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d <<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d;
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.<>t__builder = OVRTaskBuilder<OVRPlugin.Result>.Create();
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.anchors = anchors;
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.trackableTypes = trackableTypes;
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.componentType = componentType;
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.incrementalResultsCallback = incrementalResultsCallback;
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.<>1__state = -1;
		<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.<>t__builder.Start<OVRAnchor.<<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d>(ref <<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d);
		return <<FetchTrackablesAsync>g__QuerySingleComponentAsync|66_0>d.<>t__builder.Task;
	}

	[CompilerGenerated]
	internal static bool <FetchTrackablesAsync>g__DoesComponentMatchTrackableType|66_1(HashSet<OVRAnchor.TrackableType> trackableTypes, OVRAnchor anchor, OVRPlugin.SpaceComponentType componentType)
	{
		if (componentType != OVRPlugin.SpaceComponentType.DynamicObject)
		{
			if (componentType == OVRPlugin.SpaceComponentType.MarkerPayload)
			{
				if (anchor.GetComponent<OVRMarkerPayload>().PayloadType.IsQRCode())
				{
					return trackableTypes.Contains(OVRAnchor.TrackableType.QRCode);
				}
			}
			return false;
		}
		return trackableTypes.Contains(anchor.GetComponent<OVRDynamicObject>().TrackableType);
	}

	public static readonly OVRAnchor Null = new OVRAnchor(0UL, Guid.Empty);

	private static readonly Dictionary<OVRAnchor.DeferredKey, List<OVRAnchor.DeferredValue>> _deferredTasks = new Dictionary<OVRAnchor.DeferredKey, List<OVRAnchor.DeferredValue>>();

	internal static readonly Dictionary<Type, OVRPlugin.SpaceComponentType> _typeMap = new Dictionary<Type, OVRPlugin.SpaceComponentType>
	{
		{
			typeof(OVRLocatable),
			OVRPlugin.SpaceComponentType.Locatable
		},
		{
			typeof(OVRStorable),
			OVRPlugin.SpaceComponentType.Storable
		},
		{
			typeof(OVRSharable),
			OVRPlugin.SpaceComponentType.Sharable
		},
		{
			typeof(OVRBounded2D),
			OVRPlugin.SpaceComponentType.Bounded2D
		},
		{
			typeof(OVRBounded3D),
			OVRPlugin.SpaceComponentType.Bounded3D
		},
		{
			typeof(OVRSemanticLabels),
			OVRPlugin.SpaceComponentType.SemanticLabels
		},
		{
			typeof(OVRRoomLayout),
			OVRPlugin.SpaceComponentType.RoomLayout
		},
		{
			typeof(OVRAnchorContainer),
			OVRPlugin.SpaceComponentType.SpaceContainer
		},
		{
			typeof(OVRTriangleMesh),
			OVRPlugin.SpaceComponentType.TriangleMesh
		},
		{
			typeof(OVRDynamicObject),
			OVRPlugin.SpaceComponentType.DynamicObject
		},
		{
			typeof(OVRMarkerPayload),
			OVRPlugin.SpaceComponentType.MarkerPayload
		}
	};

	[OVRResultStatus]
	public enum SaveResult
	{
		Success,
		Failure = -1000,
		FailureInvalidAnchor = -1013,
		FailureDataIsInvalid = -1008,
		FailureInsufficientResources = -9000,
		FailureStorageAtCapacity = -9001,
		FailureInsufficientView = -9002,
		FailurePermissionInsufficient = -9003,
		FailureRateLimited = -9004,
		FailureTooDark = -9005,
		FailureTooBright = -9006,
		FailureUnsupported = -1004,
		FailurePersistenceNotEnabled = -2006
	}

	[OVRResultStatus]
	public enum EraseResult
	{
		Success,
		Failure = -1000,
		FailureInvalidAnchor = -1013,
		FailureDataIsInvalid = -1008,
		FailureInsufficientResources = -9000,
		FailurePermissionInsufficient = -9003,
		FailureRateLimited = -9004,
		FailureUnsupported = -1004,
		FailurePersistenceNotEnabled = -2006
	}

	[OVRResultStatus]
	public enum FetchResult
	{
		Success,
		Failure = -1000,
		FailureDataIsInvalid = -1008,
		FailureInvalidOption = -1001,
		FailureInsufficientResources = -9000,
		FailureInsufficientView = -9002,
		FailurePermissionInsufficient = -9003,
		FailureRateLimited = -9004,
		FailureTooDark = -9005,
		FailureTooBright = -9006,
		FailureUnsupported = -1004
	}

	[OVRResultStatus]
	public enum ShareResult
	{
		Success,
		Failure = -1000,
		FailureOperationFailed = -1006,
		FailureInvalidParameter = -1001,
		FailureHandleInvalid = -1013,
		FailureDataIsInvalid = -1008,
		FailureNetworkTimeout = -2003,
		FailureNetworkRequestFailed = -2004,
		FailureMappingInsufficient = -2001,
		FailureLocalizationFailed = -2002,
		FailureSharableComponentNotEnabled = -2006,
		FailureCloudStorageDisabled = -2000,
		FailurePermissionInsufficient = -9003,
		FailureUnsupported = -1004
	}

	private struct FetchTaskData
	{
		public List<OVRAnchor> Anchors;

		public Action<List<OVRAnchor>, int> IncrementalResultsCallback;
	}

	private struct DeferredValue
	{
		public OVRTask<bool> Task;

		public bool EnabledDesired;

		public ulong RequestId;

		public double Timeout;

		public float StartTime;
	}

	private struct DeferredKey : IEquatable<OVRAnchor.DeferredKey>
	{
		public static OVRAnchor.DeferredKey FromEvent(OVRDeserialize.SpaceSetComponentStatusCompleteData eventData)
		{
			return new OVRAnchor.DeferredKey
			{
				Space = eventData.Space,
				ComponentType = eventData.ComponentType
			};
		}

		public bool Equals(OVRAnchor.DeferredKey other)
		{
			return this.Space == other.Space && this.ComponentType == other.ComponentType;
		}

		public override bool Equals(object obj)
		{
			if (obj is OVRAnchor.DeferredKey)
			{
				OVRAnchor.DeferredKey other = (OVRAnchor.DeferredKey)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = this.Space.GetHashCode() * 486187739;
			int componentType = (int)this.ComponentType;
			return num + componentType.GetHashCode();
		}

		public ulong Space;

		public OVRPlugin.SpaceComponentType ComponentType;
	}

	public struct FetchOptions
	{
		internal unsafe OVRPlugin.Result DiscoverSpaces(out ulong requestId)
		{
			OVRTelemetryMarker marker = OVRTelemetry.Start(163054959, 0, -1L);
			OVRPlugin.Result result2;
			using (OVRNativeList<OVRAnchor.FilterUnion> ovrnativeList = new OVRNativeList<OVRAnchor.FilterUnion>(Allocator.Temp))
			{
				using (OVRNativeList<IntPtr> ovrnativeList2 = new OVRNativeList<IntPtr>(Allocator.Temp))
				{
					using (OVRNativeList<long> ovrnativeList3 = OVRNativeList.WithSuggestedCapacityFrom<Type>(this.ComponentTypes).AllocateEmpty<long>(Allocator.Temp))
					{
						if (this.SingleComponentType != null)
						{
							OVRPlugin.SpaceComponentType spaceComponentType = OVRAnchor.FetchOptions.GetSpaceComponentType(this.SingleComponentType);
							ovrnativeList3.Add((long)spaceComponentType);
							ovrnativeList.Add(new OVRAnchor.FilterUnion
							{
								ComponentFilter = new OVRPlugin.SpaceDiscoveryFilterInfoComponents
								{
									Type = OVRPlugin.SpaceDiscoveryFilterType.Component,
									Component = spaceComponentType
								}
							});
						}
						foreach (Type type in this.ComponentTypes.ToNonAlloc<Type>())
						{
							OVRPlugin.SpaceComponentType spaceComponentType2 = OVRAnchor.FetchOptions.GetSpaceComponentType(type);
							ovrnativeList3.Add((long)spaceComponentType2);
							ovrnativeList.Add(new OVRAnchor.FilterUnion
							{
								ComponentFilter = new OVRPlugin.SpaceDiscoveryFilterInfoComponents
								{
									Type = OVRPlugin.SpaceDiscoveryFilterType.Component,
									Component = spaceComponentType2
								}
							});
						}
						marker.AddAnnotation("component_types", ovrnativeList3.Data, ovrnativeList3.Count);
						using (OVRNativeList<Guid> ovrnativeList4 = this.Uuids.ToNativeList(Allocator.Temp))
						{
							if (this.SingleUuid != null)
							{
								ovrnativeList4.Add(this.SingleUuid.Value);
							}
							if (this.SingleUuid != null || this.Uuids != null)
							{
								ovrnativeList.Add(new OVRAnchor.FilterUnion
								{
									IdFilter = new OVRPlugin.SpaceDiscoveryFilterInfoIds
									{
										Type = OVRPlugin.SpaceDiscoveryFilterType.Ids,
										Ids = ovrnativeList4.Data,
										NumIds = ovrnativeList4.Count
									}
								});
							}
							marker.AddAnnotation("uuid_count", (long)ovrnativeList4.Count);
							for (int i = 0; i < ovrnativeList.Count; i++)
							{
								ovrnativeList2.Add(new IntPtr((void*)ovrnativeList.PtrToElementAt(i)));
							}
							marker.AddAnnotation("total_filter_count", (long)ovrnativeList2.Count);
							OVRPlugin.SpaceDiscoveryInfo spaceDiscoveryInfo = default(OVRPlugin.SpaceDiscoveryInfo);
							spaceDiscoveryInfo.NumFilters = (uint)ovrnativeList2.Count;
							spaceDiscoveryInfo.Filters = (OVRPlugin.SpaceDiscoveryFilterInfoHeader**)ovrnativeList2.Data;
							OVRPlugin.Result result = OVRPlugin.DiscoverSpaces(spaceDiscoveryInfo, out requestId);
							OVRAnchor.Telemetry.SetSyncResult(marker, requestId, result);
							result2 = result;
						}
					}
				}
			}
			return result2;
		}

		private static OVRPlugin.SpaceComponentType GetSpaceComponentType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			OVRPlugin.SpaceComponentType result;
			if (!OVRAnchor._typeMap.TryGetValue(type, out result))
			{
				throw new ArgumentException(type.FullName + " is not a supported anchor component type (IOVRAnchorComponent).", "type");
			}
			return result;
		}

		public Guid? SingleUuid;

		public IEnumerable<Guid> Uuids;

		public Type SingleComponentType;

		public IEnumerable<Type> ComponentTypes;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct FilterUnion
	{
		[FieldOffset(0)]
		public OVRPlugin.SpaceDiscoveryFilterType Type;

		[FieldOffset(0)]
		public OVRPlugin.SpaceDiscoveryFilterInfoComponents ComponentFilter;

		[FieldOffset(0)]
		public OVRPlugin.SpaceDiscoveryFilterInfoIds IdFilter;
	}

	public enum TrackableType
	{
		None,
		Keyboard,
		QRCode
	}

	private static class Telemetry
	{
		public static void OnInit()
		{
			OVRAnchor.Telemetry.s_markers.Clear();
		}

		public static void AddMarker(ulong requestId, OVRTelemetryMarker marker)
		{
			OVRAnchor.Telemetry.s_markers.Add(new OVRAnchor.Telemetry.Key(marker, requestId), marker);
		}

		public static OVRTelemetryMarker Start(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId, OVRPlugin.Result result)
		{
			OVRTelemetryMarker ovrtelemetryMarker = OVRTelemetry.Start((int)markerId, 0, -1L);
			OVRAnchor.Telemetry.SetSyncResult(ovrtelemetryMarker, requestId, result);
			return ovrtelemetryMarker;
		}

		public static void SetSyncResult(OVRTelemetryMarker marker, ulong requestId, OVRPlugin.Result result)
		{
			marker.AddAnnotation("sync_result", (long)result);
			if (!result.IsSuccess())
			{
				marker.SetResult(OVRPlugin.Qpl.ResultType.Fail).Send();
				return;
			}
			if (requestId == 0UL)
			{
				throw new ArgumentException("requestId must not be zero if the OVRPlugin method returns a successful result.", "requestId");
			}
			OVRAnchor.Telemetry.s_markers.Add(new OVRAnchor.Telemetry.Key(marker, requestId), marker);
		}

		public static void SetAsyncResultAndSend(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId, long result)
		{
			if (OVRAnchor.Telemetry.SetAsyncResult(markerId, requestId, result) == null)
			{
				return;
			}
			OVRTelemetryMarker? ovrtelemetryMarker;
			ovrtelemetryMarker.GetValueOrDefault().Send();
		}

		public static OVRTelemetryMarker? SetAsyncResult(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId, long result)
		{
			OVRTelemetryMarker ovrtelemetryMarker;
			if (!OVRAnchor.Telemetry.s_markers.Remove(new OVRAnchor.Telemetry.Key(markerId, requestId), out ovrtelemetryMarker))
			{
				return null;
			}
			return new OVRTelemetryMarker?(ovrtelemetryMarker.AddAnnotation("async_result", result).SetResult((result >= 0L) ? OVRPlugin.Qpl.ResultType.Success : OVRPlugin.Qpl.ResultType.Fail));
		}

		public static OVRTelemetryMarker? GetMarker(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId)
		{
			OVRTelemetryMarker value;
			if (!OVRAnchor.Telemetry.TryGetMarker(markerId, requestId, out value))
			{
				return null;
			}
			return new OVRTelemetryMarker?(value);
		}

		public static bool TryGetMarker(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId, out OVRTelemetryMarker marker)
		{
			return OVRAnchor.Telemetry.s_markers.TryGetValue(new OVRAnchor.Telemetry.Key(markerId, requestId), out marker);
		}

		public static bool Remove(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId, out OVRTelemetryMarker marker)
		{
			return OVRAnchor.Telemetry.s_markers.Remove(new OVRAnchor.Telemetry.Key(markerId, requestId), out marker);
		}

		public static OVRTelemetryMarker? GetRemove(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId)
		{
			OVRTelemetryMarker value;
			if (!OVRAnchor.Telemetry.Remove(markerId, requestId, out value))
			{
				return null;
			}
			return new OVRTelemetryMarker?(value);
		}

		private static Dictionary<OVRAnchor.Telemetry.Key, OVRTelemetryMarker> s_markers = new Dictionary<OVRAnchor.Telemetry.Key, OVRTelemetryMarker>();

		private readonly struct Key : IEquatable<OVRAnchor.Telemetry.Key>
		{
			public Key(OVRAnchor.Telemetry.MarkerId markerId, ulong requestId)
			{
				this._markerId = (int)markerId;
				this._requestId = requestId;
			}

			public Key(OVRTelemetryMarker marker, ulong requestId)
			{
				int markerId = marker.MarkerId;
				this._markerId = markerId;
				this._requestId = requestId;
			}

			public bool Equals(OVRAnchor.Telemetry.Key other)
			{
				return this._markerId == other._markerId && this._requestId == other._requestId;
			}

			public override bool Equals(object obj)
			{
				if (obj is OVRAnchor.Telemetry.Key)
				{
					OVRAnchor.Telemetry.Key other = (OVRAnchor.Telemetry.Key)obj;
					return this.Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return this._markerId.GetHashCode() * 486187739 + this._requestId.GetHashCode();
			}

			private readonly int _markerId;

			private readonly ulong _requestId;
		}

		internal enum MarkerId
		{
			DiscoverSpaces = 163054959,
			SaveSpaces = 163056974,
			EraseSpaces = 163061838,
			QuerySpaces = 163069062,
			SaveSpaceList = 163065048,
			EraseSingleSpace = 163062284,
			ConfigureTracker = 163068237
		}

		internal static class Annotation
		{
			public const string ComponentTypes = "component_types";

			public const string UuidCount = "uuid_count";

			public const string SpaceCount = "space_count";

			public const string TotalFilterCount = "total_filter_count";

			public const string ResultsCount = "results_count";

			public const string SynchronousResult = "sync_result";

			public const string AsynchronousResult = "async_result";

			public const string StorageLocation = "storage_location";

			public const string Timeout = "timeout";

			public const string MaxResults = "max_results";

			public const string GroupCount = "group_count";

			public const string DynamicObjectClasses = "dynamic_object_classes";

			public const string MarkerTypes = "marker_types";
		}
	}

	[Serializable]
	public struct TrackerConfiguration : IEquatable<OVRAnchor.TrackerConfiguration>
	{
		public bool KeyboardTrackingEnabled { readonly get; set; }

		public static bool KeyboardTrackingSupported
		{
			get
			{
				bool flag;
				return OVRPlugin.GetDynamicObjectTrackerSupported(out flag).IsSuccess() && flag && OVRPlugin.GetDynamicObjectKeyboardSupported(out flag).IsSuccess() && flag;
			}
		}

		internal bool RequiresDynamicObjectTracker
		{
			get
			{
				return this.KeyboardTrackingEnabled;
			}
		}

		internal OVRNativeList<OVRPlugin.DynamicObjectClass> ToDynamicObjectClasses(Allocator allocator)
		{
			OVRNativeList<OVRPlugin.DynamicObjectClass> result = new OVRNativeList<OVRPlugin.DynamicObjectClass>(allocator);
			if (this.KeyboardTrackingEnabled)
			{
				result.Add(OVRPlugin.DynamicObjectClass.Keyboard);
			}
			return result;
		}

		internal void ResetDynamicObjects()
		{
			OVRAnchor.TrackerConfiguration trackerConfiguration = default(OVRAnchor.TrackerConfiguration);
			this.SetDynamicObjectState(trackerConfiguration);
		}

		internal void SetDynamicObjectState(in OVRAnchor.TrackerConfiguration other)
		{
			this.KeyboardTrackingEnabled = other.KeyboardTrackingEnabled;
		}

		public bool QRCodeTrackingEnabled { readonly get; set; }

		public static bool QRCodeTrackingSupported
		{
			get
			{
				bool flag;
				return OVRPlugin.GetMarkerTrackingSupported(out flag).IsSuccess() && flag;
			}
		}

		internal OVRNativeList<OVRPlugin.MarkerType> ToMarkerTypes(Allocator allocator)
		{
			OVRNativeList<OVRPlugin.MarkerType> result = new OVRNativeList<OVRPlugin.MarkerType>(allocator);
			if (this.QRCodeTrackingEnabled)
			{
				result.Add(OVRPlugin.MarkerType.QRCode);
			}
			return result;
		}

		internal bool RequiresMarkerTracker
		{
			get
			{
				return this.QRCodeTrackingEnabled;
			}
		}

		internal void ResetMarkers()
		{
			OVRAnchor.TrackerConfiguration trackerConfiguration = default(OVRAnchor.TrackerConfiguration);
			this.SetMarkerState(trackerConfiguration);
		}

		internal void SetMarkerState(in OVRAnchor.TrackerConfiguration other)
		{
			this.QRCodeTrackingEnabled = other.QRCodeTrackingEnabled;
		}

		public void GetTrackableTypes(List<OVRAnchor.TrackableType> trackableTypes)
		{
			if (trackableTypes == null)
			{
				throw new ArgumentNullException("trackableTypes");
			}
			trackableTypes.Clear();
			if (this.KeyboardTrackingEnabled)
			{
				trackableTypes.Add(OVRAnchor.TrackableType.Keyboard);
			}
			if (this.QRCodeTrackingEnabled)
			{
				trackableTypes.Add(OVRAnchor.TrackableType.QRCode);
			}
		}

		public override string ToString()
		{
			List<string> list;
			string result;
			using (new OVRObjectPool.ListScope<string>(ref list))
			{
				list.Add(string.Format("{0}={1}", "KeyboardTrackingEnabled", this.KeyboardTrackingEnabled));
				list.Add(string.Format("{0}={1}", "QRCodeTrackingEnabled", this.QRCodeTrackingEnabled));
				result = "TrackerConfiguration<" + string.Join(", ", list) + ">";
			}
			return result;
		}

		public bool Equals(OVRAnchor.TrackerConfiguration other)
		{
			return this.KeyboardTrackingEnabled == other.KeyboardTrackingEnabled && this.QRCodeTrackingEnabled == other.QRCodeTrackingEnabled;
		}

		public override bool Equals(object obj)
		{
			if (obj is OVRAnchor.TrackerConfiguration)
			{
				OVRAnchor.TrackerConfiguration other = (OVRAnchor.TrackerConfiguration)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, bool>(HashCode.Combine<int, bool>(0, this.KeyboardTrackingEnabled), this.QRCodeTrackingEnabled);
		}

		public static bool operator ==(OVRAnchor.TrackerConfiguration lhs, OVRAnchor.TrackerConfiguration rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OVRAnchor.TrackerConfiguration lhs, OVRAnchor.TrackerConfiguration rhs)
		{
			return !lhs.Equals(rhs);
		}
	}

	[OVRResultStatus]
	public enum ConfigureTrackerResult
	{
		Success,
		Failure = -1000,
		Invalid = -1008,
		NotSupported = -1004
	}

	public sealed class Tracker : IDisposable
	{
		public OVRAnchor.TrackerConfiguration Configuration
		{
			get
			{
				return this._configuration;
			}
		}

		private OVRTask<OVRPlugin.Result> SetupMarkerTracker(OVRAnchor.TrackerConfiguration config)
		{
			OVRAnchor.Tracker.<SetupMarkerTracker>d__5 <SetupMarkerTracker>d__;
			<SetupMarkerTracker>d__.<>t__builder = OVRTaskBuilder<OVRPlugin.Result>.Create();
			<SetupMarkerTracker>d__.<>4__this = this;
			<SetupMarkerTracker>d__.config = config;
			<SetupMarkerTracker>d__.<>1__state = -1;
			<SetupMarkerTracker>d__.<>t__builder.Start<OVRAnchor.Tracker.<SetupMarkerTracker>d__5>(ref <SetupMarkerTracker>d__);
			return <SetupMarkerTracker>d__.<>t__builder.Task;
		}

		private OVRTask<OVRPlugin.Result> SetupDynamicObjectTracker(OVRAnchor.TrackerConfiguration config)
		{
			OVRAnchor.Tracker.<SetupDynamicObjectTracker>d__7 <SetupDynamicObjectTracker>d__;
			<SetupDynamicObjectTracker>d__.<>t__builder = OVRTaskBuilder<OVRPlugin.Result>.Create();
			<SetupDynamicObjectTracker>d__.<>4__this = this;
			<SetupDynamicObjectTracker>d__.config = config;
			<SetupDynamicObjectTracker>d__.<>1__state = -1;
			<SetupDynamicObjectTracker>d__.<>t__builder.Start<OVRAnchor.Tracker.<SetupDynamicObjectTracker>d__7>(ref <SetupDynamicObjectTracker>d__);
			return <SetupDynamicObjectTracker>d__.<>t__builder.Task;
		}

		public OVRTask<OVRResult<OVRAnchor.ConfigureTrackerResult>> ConfigureAsync(OVRAnchor.TrackerConfiguration configuration)
		{
			OVRAnchor.Tracker.<ConfigureAsync>d__9 <ConfigureAsync>d__;
			<ConfigureAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<OVRAnchor.ConfigureTrackerResult>>.Create();
			<ConfigureAsync>d__.<>4__this = this;
			<ConfigureAsync>d__.configuration = configuration;
			<ConfigureAsync>d__.<>1__state = -1;
			<ConfigureAsync>d__.<>t__builder.Start<OVRAnchor.Tracker.<ConfigureAsync>d__9>(ref <ConfigureAsync>d__);
			return <ConfigureAsync>d__.<>t__builder.Task;
		}

		public OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> FetchTrackablesAsync(List<OVRAnchor> anchors, Action<List<OVRAnchor>, int> incrementalResultsCallback = null)
		{
			if (anchors == null)
			{
				throw new ArgumentNullException("anchors");
			}
			List<OVRAnchor.TrackableType> trackableTypes;
			OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> result;
			using (new OVRObjectPool.ListScope<OVRAnchor.TrackableType>(ref trackableTypes))
			{
				this._configuration.GetTrackableTypes(trackableTypes);
				result = OVRAnchor.FetchTrackablesAsync(anchors, trackableTypes, incrementalResultsCallback);
			}
			return result;
		}

		~Tracker()
		{
			if (this._markerTracker != 0UL || this._dynamicObjectTracker != 0UL)
			{
				Debug.LogError("Tracker was not disposed of while one or more trackers were active, which leaks resources. Call Dispose() when no longer needed.");
			}
		}

		public void Dispose()
		{
			OVRAnchor.Tracker.<Dispose>d__12 <Dispose>d__;
			<Dispose>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Dispose>d__.<>4__this = this;
			<Dispose>d__.<>1__state = -1;
			<Dispose>d__.<>t__builder.Start<OVRAnchor.Tracker.<Dispose>d__12>(ref <Dispose>d__);
		}

		[CompilerGenerated]
		internal static OVRTask<OVRResult<ulong, OVRPlugin.Result>> <SetupMarkerTracker>g__CreateTrackerAsync|5_0(OVRAnchor.TrackerConfiguration config)
		{
			OVRAnchor.Tracker.<<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d <<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d;
			<<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d.<>t__builder = OVRTaskBuilder<OVRResult<ulong, OVRPlugin.Result>>.Create();
			<<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d.config = config;
			<<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d.<>1__state = -1;
			<<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d.<>t__builder.Start<OVRAnchor.Tracker.<<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d>(ref <<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d);
			return <<SetupMarkerTracker>g__CreateTrackerAsync|5_0>d.<>t__builder.Task;
		}

		[CompilerGenerated]
		internal static OVRTask<OVRResult<OVRPlugin.Result>> <SetupDynamicObjectTracker>g__SetClassesAsync|7_0(ulong tracker, OVRAnchor.TrackerConfiguration config)
		{
			OVRTask<OVRResult<OVRPlugin.Result>> result;
			using (OVRNativeList<OVRPlugin.DynamicObjectClass> list = config.ToDynamicObjectClasses(Allocator.Temp))
			{
				result = OVRPlugin.SetDynamicObjectTrackedClassesAsync(tracker, list);
			}
			return result;
		}

		[CompilerGenerated]
		internal static OVRTask<OVRResult<ulong, OVRPlugin.Result>> <SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1(ulong tracker, OVRAnchor.TrackerConfiguration config)
		{
			OVRAnchor.Tracker.<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d <<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d;
			<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d.<>t__builder = OVRTaskBuilder<OVRResult<ulong, OVRPlugin.Result>>.Create();
			<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d.tracker = tracker;
			<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d.config = config;
			<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d.<>1__state = -1;
			<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d.<>t__builder.Start<OVRAnchor.Tracker.<<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d>(ref <<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d);
			return <<SetupDynamicObjectTracker>g__CreateAndConfigureTrackerAsync|7_1>d.<>t__builder.Task;
		}

		private OVRAnchor.TrackerConfiguration _configuration;

		private int _asyncOperationCount;

		private ulong _markerTracker;

		private ulong _dynamicObjectTracker;

		private struct AsyncLock : IDisposable
		{
			private AsyncLock(OVRAnchor.Tracker tracker)
			{
				this._tracker = tracker;
				this._tracker._asyncOperationCount++;
			}

			public void Dispose()
			{
				this._tracker._asyncOperationCount--;
			}

			public static OVRTask<OVRAnchor.Tracker.AsyncLock> AcquireAsync(OVRAnchor.Tracker tracker)
			{
				OVRAnchor.Tracker.AsyncLock.<AcquireAsync>d__3 <AcquireAsync>d__;
				<AcquireAsync>d__.<>t__builder = OVRTaskBuilder<OVRAnchor.Tracker.AsyncLock>.Create();
				<AcquireAsync>d__.tracker = tracker;
				<AcquireAsync>d__.<>1__state = -1;
				<AcquireAsync>d__.<>t__builder.Start<OVRAnchor.Tracker.AsyncLock.<AcquireAsync>d__3>(ref <AcquireAsync>d__);
				return <AcquireAsync>d__.<>t__builder.Task;
			}

			private OVRAnchor.Tracker _tracker;
		}
	}
}
