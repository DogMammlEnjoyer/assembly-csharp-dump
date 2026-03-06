using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meta.XR.Util;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[DisallowMultipleComponent]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-spatial-anchors-persist-content/#ovrspatialanchor-component")]
[Feature(Feature.Anchors)]
public class OVRSpatialAnchor : MonoBehaviour
{
	private event Action<OVRSpatialAnchor.OperationResult> _onLocalize;

	internal OVRAnchor _anchor { get; private set; }

	public event Action<OVRSpatialAnchor.OperationResult> OnLocalize
	{
		add
		{
			bool flag;
			bool flag2;
			if (this.Created && OVRPlugin.GetSpaceComponentStatus(this._anchor.Handle, OVRPlugin.SpaceComponentType.Locatable, out flag, out flag2) && !flag2)
			{
				value(flag ? OVRSpatialAnchor.OperationResult.Success : OVRSpatialAnchor.OperationResult.Failure);
				return;
			}
			this._onLocalize += value;
		}
		remove
		{
			this._onLocalize -= value;
		}
	}

	public Guid Uuid
	{
		get
		{
			return this._anchor.Uuid;
		}
	}

	public bool Created
	{
		get
		{
			return this && this._anchor != OVRAnchor.Null;
		}
	}

	public bool PendingCreation
	{
		get
		{
			return this && this._requestId > 0UL;
		}
	}

	public OVRTask<bool> WhenCreatedAsync()
	{
		OVRSpatialAnchor.<WhenCreatedAsync>d__19 <WhenCreatedAsync>d__;
		<WhenCreatedAsync>d__.<>t__builder = OVRTaskBuilder<bool>.Create();
		<WhenCreatedAsync>d__.<>4__this = this;
		<WhenCreatedAsync>d__.<>1__state = -1;
		<WhenCreatedAsync>d__.<>t__builder.Start<OVRSpatialAnchor.<WhenCreatedAsync>d__19>(ref <WhenCreatedAsync>d__);
		return <WhenCreatedAsync>d__.<>t__builder.Task;
	}

	public bool Localized
	{
		get
		{
			bool flag;
			bool flag2;
			return this.Created && OVRPlugin.GetSpaceComponentStatus(this._anchor.Handle, OVRPlugin.SpaceComponentType.Locatable, out flag, out flag2) && flag;
		}
	}

	public OVRTask<bool> WhenLocalizedAsync()
	{
		OVRSpatialAnchor.<WhenLocalizedAsync>d__22 <WhenLocalizedAsync>d__;
		<WhenLocalizedAsync>d__.<>t__builder = OVRTaskBuilder<bool>.Create();
		<WhenLocalizedAsync>d__.<>4__this = this;
		<WhenLocalizedAsync>d__.<>1__state = -1;
		<WhenLocalizedAsync>d__.<>t__builder.Start<OVRSpatialAnchor.<WhenLocalizedAsync>d__22>(ref <WhenLocalizedAsync>d__);
		return <WhenLocalizedAsync>d__.<>t__builder.Task;
	}

	public OVRTask<OVRSpatialAnchor.OperationResult> ShareAsync(OVRSpaceUser user)
	{
		List<OVRSpaceUser> list = OVRObjectPool.List<OVRSpaceUser>();
		list.Add(user);
		return this.ShareAsyncInternal(list);
	}

	public OVRTask<OVRSpatialAnchor.OperationResult> ShareAsync(OVRSpaceUser user1, OVRSpaceUser user2)
	{
		List<OVRSpaceUser> list = OVRObjectPool.List<OVRSpaceUser>();
		list.Add(user1);
		list.Add(user2);
		return this.ShareAsyncInternal(list);
	}

	public OVRTask<OVRSpatialAnchor.OperationResult> ShareAsync(OVRSpaceUser user1, OVRSpaceUser user2, OVRSpaceUser user3)
	{
		List<OVRSpaceUser> list = OVRObjectPool.List<OVRSpaceUser>();
		list.Add(user1);
		list.Add(user2);
		list.Add(user3);
		return this.ShareAsyncInternal(list);
	}

	public OVRTask<OVRSpatialAnchor.OperationResult> ShareAsync(OVRSpaceUser user1, OVRSpaceUser user2, OVRSpaceUser user3, OVRSpaceUser user4)
	{
		List<OVRSpaceUser> list = OVRObjectPool.List<OVRSpaceUser>();
		list.Add(user1);
		list.Add(user2);
		list.Add(user3);
		list.Add(user4);
		return this.ShareAsyncInternal(list);
	}

	public OVRTask<OVRSpatialAnchor.OperationResult> ShareAsync(IEnumerable<OVRSpaceUser> users)
	{
		List<OVRSpaceUser> list = OVRObjectPool.List<OVRSpaceUser>();
		list.AddRange(users);
		return this.ShareAsyncInternal(list);
	}

	public unsafe OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(Guid groupUuid)
	{
		if (groupUuid == Guid.Empty)
		{
			throw new ArgumentException("groupUuid must not be a 0 uuid", "groupUuid");
		}
		ulong handle = this._anchor.Handle;
		ReadOnlySpan<ulong> anchors = new ReadOnlySpan<ulong>((void*)(&handle), 1);
		ReadOnlySpan<Guid> groupUuids = new ReadOnlySpan<Guid>((void*)(&groupUuid), 1);
		return OVRAnchor.ShareAsyncInternal(anchors, groupUuids);
	}

	public static OVRTask<OVRSpatialAnchor.OperationResult> ShareAsync(IEnumerable<OVRSpatialAnchor> anchors, IEnumerable<OVRSpaceUser> users)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		if (users == null)
		{
			throw new ArgumentNullException("users");
		}
		OVRTask<OVRSpatialAnchor.OperationResult> result;
		using (OVRNativeList<ulong> list = new OVRNativeList<ulong>(anchors.ToNonAlloc<OVRSpatialAnchor>().Count, Allocator.Temp))
		{
			foreach (OVRSpatialAnchor ovrspatialAnchor in anchors.ToNonAlloc<OVRSpatialAnchor>())
			{
				list.Add(ovrspatialAnchor._anchor.Handle);
			}
			using (OVRNativeList<ulong> list2 = new OVRNativeList<ulong>(users.ToNonAlloc<OVRSpaceUser>().Count, Allocator.Temp))
			{
				foreach (OVRSpaceUser ovrspaceUser in users.ToNonAlloc<OVRSpaceUser>())
				{
					list2.Add(ovrspaceUser._handle);
				}
				ulong requestId;
				result = OVRTask.Build(OVRPlugin.ShareSpaces(list, (uint)list.Count, list2, (uint)list2.Count, out requestId), requestId).ToTask<OVRSpatialAnchor.OperationResult>();
			}
		}
		return result;
	}

	public unsafe static OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(IEnumerable<OVRSpatialAnchor> anchors, Guid groupUuid)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		if (groupUuid == Guid.Empty)
		{
			throw new ArgumentException("groupUuid must not be a 0 uuid", "groupUuid");
		}
		OVREnumerable<OVRSpatialAnchor> ovrenumerable = anchors.ToNonAlloc<OVRSpatialAnchor>();
		OVRTask<OVRResult<OVRAnchor.ShareResult>> result;
		using (OVRNativeList<ulong> list = new OVRNativeList<ulong>(ovrenumerable.Count, Allocator.Temp))
		{
			foreach (OVRSpatialAnchor ovrspatialAnchor in ovrenumerable)
			{
				list.Add(ovrspatialAnchor._anchor.Handle);
			}
			ReadOnlySpan<Guid> groupUuids = new ReadOnlySpan<Guid>((void*)(&groupUuid), 1);
			result = OVRAnchor.ShareAsyncInternal(list, groupUuids);
		}
		return result;
	}

	public static OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareAsync(IEnumerable<OVRSpatialAnchor> anchors, IEnumerable<Guid> groupUuids)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		if (groupUuids == null)
		{
			throw new ArgumentNullException("groupUuids");
		}
		OVREnumerable<OVRSpatialAnchor> ovrenumerable = anchors.ToNonAlloc<OVRSpatialAnchor>();
		OVRTask<OVRResult<OVRAnchor.ShareResult>> result;
		using (OVRNativeList<ulong> list = new OVRNativeList<ulong>(ovrenumerable.Count, Allocator.Temp))
		{
			foreach (OVRSpatialAnchor ovrspatialAnchor in ovrenumerable)
			{
				list.Add(ovrspatialAnchor._anchor.Handle);
			}
			using (OVRNativeList<Guid> list2 = groupUuids.ToNativeList(Allocator.Temp))
			{
				using (NativeArray<Guid>.Enumerator enumerator2 = list2.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == Guid.Empty)
						{
							throw new ArgumentException("groupUuids must not contain a 0 uuid", "groupUuids");
						}
					}
				}
				result = OVRAnchor.ShareAsyncInternal(list, list2);
			}
		}
		return result;
	}

	private OVRTask<OVRSpatialAnchor.OperationResult> ShareAsyncInternal(List<OVRSpaceUser> users)
	{
		this.GetListToStoreTheShareRequest(users).Add(this);
		Guid guid = Guid.NewGuid();
		OVRSpatialAnchor.AsyncRequestTaskIds[this] = guid;
		return OVRTask.FromGuid<OVRSpatialAnchor.OperationResult>(guid);
	}

	private List<OVRSpatialAnchor> GetListToStoreTheShareRequest(List<OVRSpaceUser> users)
	{
		users.Sort((OVRSpaceUser x, OVRSpaceUser y) => x.Id.CompareTo(y.Id));
		foreach (ValueTuple<List<OVRSpaceUser>, List<OVRSpatialAnchor>> valueTuple in OVRSpatialAnchor.ShareRequests)
		{
			List<OVRSpaceUser> item = valueTuple.Item1;
			List<OVRSpatialAnchor> item2 = valueTuple.Item2;
			if (OVRSpatialAnchor.AreSortedUserListsEqual(users, item))
			{
				return item2;
			}
		}
		List<OVRSpatialAnchor> list = OVRObjectPool.List<OVRSpatialAnchor>();
		OVRSpatialAnchor.ShareRequests.Add(new ValueTuple<List<OVRSpaceUser>, List<OVRSpatialAnchor>>(users, list));
		return list;
	}

	private static bool AreSortedUserListsEqual(IReadOnlyList<OVRSpaceUser> sortedList1, IReadOnlyList<OVRSpaceUser> sortedList2)
	{
		if (sortedList1.Count != sortedList2.Count)
		{
			return false;
		}
		for (int i = 0; i < sortedList1.Count; i++)
		{
			if (sortedList1[i].Id != sortedList2[i].Id)
			{
				return false;
			}
		}
		return true;
	}

	public static OVRTask<OVRResult<OVRAnchor.SaveResult>> SaveAnchorsAsync(IEnumerable<OVRSpatialAnchor> anchors)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		OVRTask<OVRResult<OVRAnchor.SaveResult>> result;
		using (OVRNativeList<ulong> list = new OVRNativeList<ulong>(Allocator.Temp))
		{
			foreach (OVRSpatialAnchor ovrspatialAnchor in anchors.ToNonAlloc<OVRSpatialAnchor>())
			{
				list.Add(ovrspatialAnchor._anchor.Handle);
			}
			result = OVRAnchor.SaveSpacesAsync(list);
		}
		return result;
	}

	public OVRTask<OVRResult<OVRAnchor.SaveResult>> SaveAnchorAsync()
	{
		return this._anchor.SaveAsync();
	}

	public OVRTask<OVRResult<OVRAnchor.EraseResult>> EraseAnchorAsync()
	{
		return this._anchor.EraseAsync();
	}

	public static OVRTask<OVRResult<OVRAnchor.EraseResult>> EraseAnchorsAsync(IEnumerable<OVRSpatialAnchor> anchors, IEnumerable<Guid> uuids)
	{
		if (anchors == null && uuids == null)
		{
			throw new ArgumentException("One of anchors or uuids must not be null.");
		}
		List<OVRAnchor> list;
		OVRTask<OVRResult<OVRAnchor.EraseResult>> result;
		using (new OVRObjectPool.ListScope<OVRAnchor>(ref list))
		{
			foreach (OVRSpatialAnchor ovrspatialAnchor in anchors.ToNonAlloc<OVRSpatialAnchor>())
			{
				list.Add(ovrspatialAnchor._anchor);
			}
			result = OVRAnchor.EraseAsync(list, uuids);
		}
		return result;
	}

	private static void ThrowIfBound(Guid uuid)
	{
		if (OVRSpatialAnchor.SpatialAnchors.ContainsKey(uuid))
		{
			throw new InvalidOperationException(string.Format("Spatial anchor with uuid {0} is already bound to an {1}.", uuid, "OVRSpatialAnchor"));
		}
	}

	private void InitializeUnchecked(OVRSpace space, Guid uuid)
	{
		OVRSpatialAnchor.SpatialAnchors.Add(uuid, this);
		this._requestId = 0UL;
		this._anchor = new OVRAnchor(space, uuid);
		OVRLocatable ovrlocatable;
		if (this._anchor.TryGetComponent<OVRLocatable>(out ovrlocatable))
		{
			ovrlocatable.SetEnabledAsync(true, 0.0);
		}
		OVRStorable ovrstorable;
		if (this._anchor.TryGetComponent<OVRStorable>(out ovrstorable))
		{
			ovrstorable.SetEnabledAsync(true, 0.0);
		}
		OVRSharable ovrsharable;
		if (this._anchor.TryGetComponent<OVRSharable>(out ovrsharable))
		{
			ovrsharable.SetEnabledAsync(true, 0.0);
		}
		this.UpdateTransform();
	}

	private void Start()
	{
		this._startCalled = true;
		if (!this.Created)
		{
			this.CreateSpatialAnchor();
		}
	}

	private void Update()
	{
		if (this.Created)
		{
			this.UpdateTransform();
		}
	}

	private void LateUpdate()
	{
		OVRSpatialAnchor.SaveBatchAnchors();
		OVRSpatialAnchor.ShareBatchAnchors();
	}

	private static void ShareBatchAnchors()
	{
		foreach (ValueTuple<List<OVRSpaceUser>, List<OVRSpatialAnchor>> valueTuple in OVRSpatialAnchor.ShareRequests)
		{
			List<OVRSpaceUser> item = valueTuple.Item1;
			List<OVRSpatialAnchor> item2 = valueTuple.Item2;
			if (item.Count > 0 && item2.Count > 0)
			{
				OVRSpatialAnchor.Share(item2, item, null);
			}
			OVRObjectPool.Return<List<OVRSpaceUser>>(item);
			OVRObjectPool.Return<List<OVRSpatialAnchor>>(item2);
		}
		OVRSpatialAnchor.ShareRequests.Clear();
	}

	private void OnDestroy()
	{
		if (this._anchor != OVRAnchor.Null)
		{
			this._anchor.Dispose();
		}
		OVRSpatialAnchor.SpatialAnchors.Remove(this.Uuid);
	}

	private OVRPose GetTrackingSpacePose()
	{
		Camera main = Camera.main;
		if (main)
		{
			return base.transform.ToTrackingSpacePose(main);
		}
		return base.transform.ToOVRPose(false);
	}

	private void CreateSpatialAnchor()
	{
		if (OVRPlugin.CreateSpatialAnchor(new OVRPlugin.SpatialAnchorCreateInfo
		{
			BaseTracking = OVRPlugin.GetTrackingOriginType(),
			PoseInSpace = this.GetTrackingSpacePose().ToPosef(),
			Time = OVRPlugin.GetTimeInSeconds()
		}, out this._requestId))
		{
			OVRSpatialAnchor.CreationRequests[this._requestId] = this;
			return;
		}
		Object.Destroy(this);
	}

	internal static bool TryGetPose(OVRSpace space, out OVRPose pose)
	{
		OVRPlugin.Posef p;
		OVRPlugin.SpaceLocationFlags value;
		if (!OVRPlugin.TryLocateSpace(space, OVRPlugin.GetTrackingOriginType(), out p, out value) || !value.IsOrientationValid() || !value.IsPositionValid())
		{
			pose = OVRPose.identity;
			return false;
		}
		pose = p.ToOVRPose();
		Camera main = Camera.main;
		if (main)
		{
			pose = pose.ToWorldSpacePose(main);
		}
		return true;
	}

	private void UpdateTransform()
	{
		OVRPose ovrpose;
		if (OVRSpatialAnchor.TryGetPose(this._anchor.Handle, out ovrpose))
		{
			base.transform.SetPositionAndRotation(ovrpose.position, ovrpose.orientation);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitializeOnLoad()
	{
		OVRSpatialAnchor.CreationRequests.Clear();
		OVRSpatialAnchor.MultiAnchorCompletionDelegates.Clear();
		OVRSpatialAnchor.SpatialAnchors.Clear();
	}

	static OVRSpatialAnchor()
	{
		OVRManager.SpatialAnchorCreateComplete += OVRSpatialAnchor.OnSpatialAnchorCreateComplete;
		OVRManager.SpaceSaveComplete += OVRSpatialAnchor.OnSpaceSaveComplete;
		OVRManager.SpaceListSaveComplete += OVRSpatialAnchor.OnSpaceListSaveComplete;
		OVRManager.ShareSpacesComplete += OVRSpatialAnchor.OnShareSpacesComplete;
		OVRManager.SpaceEraseComplete += OVRSpatialAnchor.OnSpaceEraseComplete;
		OVRManager.SpaceQueryComplete += OVRSpatialAnchor.OnSpaceQueryComplete;
		OVRManager.SpaceSetComponentStatusComplete += OVRSpatialAnchor.OnSpaceSetComponentStatusComplete;
	}

	private static void InvokeMultiAnchorDelegate(ulong requestId, OVRSpatialAnchor.OperationResult result, OVRSpatialAnchor.MultiAnchorActionType actionType)
	{
		OVRSpatialAnchor.MultiAnchorDelegatePair multiAnchorDelegatePair;
		if (!OVRSpatialAnchor.MultiAnchorCompletionDelegates.Remove(requestId, out multiAnchorDelegatePair))
		{
			return;
		}
		Action<ICollection<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> @delegate = multiAnchorDelegatePair.Delegate;
		if (@delegate != null)
		{
			@delegate(multiAnchorDelegatePair.Anchors, result);
		}
		try
		{
			foreach (OVRSpatialAnchor key in multiAnchorDelegatePair.Anchors)
			{
				Guid id2;
				if (actionType != OVRSpatialAnchor.MultiAnchorActionType.Save)
				{
					if (actionType != OVRSpatialAnchor.MultiAnchorActionType.Share)
					{
						throw new ArgumentOutOfRangeException("actionType", actionType, null);
					}
					Guid id;
					if (OVRSpatialAnchor.AsyncRequestTaskIds.Remove(key, out id))
					{
						OVRTask.SetResult<OVRSpatialAnchor.OperationResult>(id, result);
					}
				}
				else if (OVRSpatialAnchor.AsyncRequestTaskIds.Remove(key, out id2))
				{
					OVRTask.SetResult<bool>(id2, result == OVRSpatialAnchor.OperationResult.Success);
				}
			}
		}
		finally
		{
			OVRObjectPool.Return<List<OVRSpatialAnchor>>(multiAnchorDelegatePair.Anchors);
		}
	}

	private static void OnSpatialAnchorCreateComplete(ulong requestId, bool success, OVRSpace space, Guid uuid)
	{
		OVRSpatialAnchor ovrspatialAnchor;
		if (!OVRSpatialAnchor.CreationRequests.Remove(requestId, out ovrspatialAnchor))
		{
			return;
		}
		if (ovrspatialAnchor)
		{
			ovrspatialAnchor._creationFailed = !success;
		}
		if (success && ovrspatialAnchor)
		{
			ovrspatialAnchor.InitializeUnchecked(space, uuid);
			return;
		}
		if (success && !ovrspatialAnchor)
		{
			OVRPlugin.DestroySpace(space);
			return;
		}
		if (!success && ovrspatialAnchor)
		{
			Object.Destroy(ovrspatialAnchor);
		}
	}

	public static OVRTask<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRAnchor.FetchResult>> LoadUnboundAnchorsAsync(IEnumerable<Guid> uuids, List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors, Action<List<OVRSpatialAnchor.UnboundAnchor>, int> onIncrementalResultsAvailable = null)
	{
		if (uuids == null)
		{
			throw new ArgumentNullException("uuids");
		}
		if (unboundAnchors == null)
		{
			throw new ArgumentNullException("unboundAnchors");
		}
		return OVRSpatialAnchor.LoadUnboundAnchorsAsync(new OVRAnchor.FetchOptions
		{
			Uuids = uuids
		}, unboundAnchors, onIncrementalResultsAvailable);
	}

	public static OVRTask<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult>> LoadUnboundSharedAnchorsAsync(IEnumerable<Guid> uuids, List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors)
	{
		OVRSpatialAnchor.<LoadUnboundSharedAnchorsAsync>d__62 <LoadUnboundSharedAnchorsAsync>d__;
		<LoadUnboundSharedAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult>>.Create();
		<LoadUnboundSharedAnchorsAsync>d__.uuids = uuids;
		<LoadUnboundSharedAnchorsAsync>d__.unboundAnchors = unboundAnchors;
		<LoadUnboundSharedAnchorsAsync>d__.<>1__state = -1;
		<LoadUnboundSharedAnchorsAsync>d__.<>t__builder.Start<OVRSpatialAnchor.<LoadUnboundSharedAnchorsAsync>d__62>(ref <LoadUnboundSharedAnchorsAsync>d__);
		return <LoadUnboundSharedAnchorsAsync>d__.<>t__builder.Task;
	}

	public static OVRTask<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult>> LoadUnboundSharedAnchorsAsync(Guid groupUuid, List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors)
	{
		OVRSpatialAnchor.<LoadUnboundSharedAnchorsAsync>d__63 <LoadUnboundSharedAnchorsAsync>d__;
		<LoadUnboundSharedAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult>>.Create();
		<LoadUnboundSharedAnchorsAsync>d__.groupUuid = groupUuid;
		<LoadUnboundSharedAnchorsAsync>d__.unboundAnchors = unboundAnchors;
		<LoadUnboundSharedAnchorsAsync>d__.<>1__state = -1;
		<LoadUnboundSharedAnchorsAsync>d__.<>t__builder.Start<OVRSpatialAnchor.<LoadUnboundSharedAnchorsAsync>d__63>(ref <LoadUnboundSharedAnchorsAsync>d__);
		return <LoadUnboundSharedAnchorsAsync>d__.<>t__builder.Task;
	}

	public static OVRTask<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult>> LoadUnboundSharedAnchorsAsync(Guid groupUuid, IEnumerable<Guid> allowedAnchorUuids, List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors)
	{
		OVRSpatialAnchor.<LoadUnboundSharedAnchorsAsync>d__64 <LoadUnboundSharedAnchorsAsync>d__;
		<LoadUnboundSharedAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult>>.Create();
		<LoadUnboundSharedAnchorsAsync>d__.groupUuid = groupUuid;
		<LoadUnboundSharedAnchorsAsync>d__.allowedAnchorUuids = allowedAnchorUuids;
		<LoadUnboundSharedAnchorsAsync>d__.unboundAnchors = unboundAnchors;
		<LoadUnboundSharedAnchorsAsync>d__.<>1__state = -1;
		<LoadUnboundSharedAnchorsAsync>d__.<>t__builder.Start<OVRSpatialAnchor.<LoadUnboundSharedAnchorsAsync>d__64>(ref <LoadUnboundSharedAnchorsAsync>d__);
		return <LoadUnboundSharedAnchorsAsync>d__.<>t__builder.Task;
	}

	private static OVRTask<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRAnchor.FetchResult>> LoadUnboundAnchorsAsync(OVRAnchor.FetchOptions fetchOptions, List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors, Action<List<OVRSpatialAnchor.UnboundAnchor>, int> resultsHandler)
	{
		OVRSpatialAnchor.<LoadUnboundAnchorsAsync>d__65 <LoadUnboundAnchorsAsync>d__;
		<LoadUnboundAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRAnchor.FetchResult>>.Create();
		<LoadUnboundAnchorsAsync>d__.fetchOptions = fetchOptions;
		<LoadUnboundAnchorsAsync>d__.unboundAnchors = unboundAnchors;
		<LoadUnboundAnchorsAsync>d__.resultsHandler = resultsHandler;
		<LoadUnboundAnchorsAsync>d__.<>1__state = -1;
		<LoadUnboundAnchorsAsync>d__.<>t__builder.Start<OVRSpatialAnchor.<LoadUnboundAnchorsAsync>d__65>(ref <LoadUnboundAnchorsAsync>d__);
		return <LoadUnboundAnchorsAsync>d__.<>t__builder.Task;
	}

	public static bool FromOVRAnchor(OVRAnchor anchor, out OVRSpatialAnchor.UnboundAnchor unboundAnchor)
	{
		if (anchor == OVRAnchor.Null)
		{
			throw new ArgumentNullException("anchor");
		}
		return OVRSpatialAnchor.TryGetUnbound(anchor, out unboundAnchor);
	}

	private static bool TryGetUnbound(OVRAnchor anchor, out OVRSpatialAnchor.UnboundAnchor unboundAnchor)
	{
		unboundAnchor = new OVRSpatialAnchor.UnboundAnchor(anchor.Handle, anchor.Uuid);
		OVRSpatialAnchor ovrspatialAnchor;
		if (OVRSpatialAnchor.SpatialAnchors.TryGetValue(unboundAnchor.Uuid, out ovrspatialAnchor))
		{
			return false;
		}
		OVRLocatable ovrlocatable;
		bool result = anchor.TryGetComponent<OVRLocatable>(out ovrlocatable);
		string empty = string.Empty;
		return result;
	}

	private static void OnSpaceSetComponentStatusComplete(ulong requestId, bool result, OVRSpace space, Guid uuid, OVRPlugin.SpaceComponentType componentType, bool enabled)
	{
		OVRSpatialAnchor ovrspatialAnchor;
		if (componentType == OVRPlugin.SpaceComponentType.Locatable && OVRSpatialAnchor.SpatialAnchors.TryGetValue(uuid, out ovrspatialAnchor))
		{
			Action<OVRSpatialAnchor.OperationResult> onLocalize = ovrspatialAnchor._onLocalize;
			if (onLocalize == null)
			{
				return;
			}
			onLocalize(enabled ? OVRSpatialAnchor.OperationResult.Success : OVRSpatialAnchor.OperationResult.Failure);
		}
	}

	private static void OnShareSpacesComplete(ulong requestId, OVRSpatialAnchor.OperationResult result)
	{
		OVRTask.SetResult<OVRSpatialAnchor.OperationResult>(requestId, result);
		OVRSpatialAnchor.InvokeMultiAnchorDelegate(requestId, result, OVRSpatialAnchor.MultiAnchorActionType.Share);
	}

	[Obsolete("You should use LoadUnboundAnchorsAsync to load previously saved anchors and AddComponent<OVRSpatialAnchor>() to create a new anchor. You should no longer need to use an OVRSpace handle directly.")]
	public void InitializeFromExisting(OVRSpace space, Guid uuid)
	{
		if (this._startCalled)
		{
			throw new InvalidOperationException("Cannot call InitializeFromExisting after Start. This must be set once upon creation.");
		}
		try
		{
			if (!space.Valid)
			{
				throw new ArgumentException(string.Format("Invalid space {0}.", space), "space");
			}
			OVRSpatialAnchor.ThrowIfBound(uuid);
		}
		catch
		{
			Object.Destroy(this);
			throw;
		}
		this.InitializeUnchecked(space, uuid);
	}

	[Obsolete("Use SaveAsync instead.")]
	public void Save(Action<OVRSpatialAnchor, bool> onComplete = null)
	{
		this.Save(this._defaultSaveOptions, onComplete);
	}

	[Obsolete("Use SaveAsync instead.")]
	public void Save(OVRSpatialAnchor.SaveOptions saveOptions, Action<OVRSpatialAnchor, bool> onComplete = null)
	{
		OVRTask<bool> task = this.SaveAsync(saveOptions);
		if (onComplete != null)
		{
			OVRSpatialAnchor.InvertedCapture<bool, OVRSpatialAnchor>.ContinueTaskWith(task, onComplete, this);
		}
	}

	[Obsolete("This property exposes an internal handle that should no longer be necessary. You can Save, Erase, and Share anchors using the methods in this class.")]
	public OVRSpace Space
	{
		get
		{
			return this._anchor.Handle;
		}
	}

	[Obsolete("Use ShareAsync instead.")]
	public void Share(OVRSpaceUser user, Action<OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		OVRTask<OVRSpatialAnchor.OperationResult> ovrtask = this.ShareAsync(user);
		if (onComplete != null)
		{
			ovrtask.ContinueWith(onComplete);
		}
	}

	[Obsolete("Use ShareAsync instead.")]
	public void Share(OVRSpaceUser user1, OVRSpaceUser user2, Action<OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		OVRTask<OVRSpatialAnchor.OperationResult> ovrtask = this.ShareAsync(user1, user2);
		if (onComplete != null)
		{
			ovrtask.ContinueWith(onComplete);
		}
	}

	[Obsolete("Use ShareAsync instead.")]
	public void Share(OVRSpaceUser user1, OVRSpaceUser user2, OVRSpaceUser user3, Action<OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		OVRTask<OVRSpatialAnchor.OperationResult> ovrtask = this.ShareAsync(user1, user2, user3);
		if (onComplete != null)
		{
			ovrtask.ContinueWith(onComplete);
		}
	}

	[Obsolete("Use ShareAsync instead.")]
	public void Share(OVRSpaceUser user1, OVRSpaceUser user2, OVRSpaceUser user3, OVRSpaceUser user4, Action<OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		OVRTask<OVRSpatialAnchor.OperationResult> ovrtask = this.ShareAsync(user1, user2, user3, user4);
		if (onComplete != null)
		{
			ovrtask.ContinueWith(onComplete);
		}
	}

	[Obsolete("Use ShareAsync instead.")]
	public void Share(IEnumerable<OVRSpaceUser> users, Action<OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		OVRTask<OVRSpatialAnchor.OperationResult> ovrtask = this.ShareAsync(users);
		if (onComplete != null)
		{
			ovrtask.ContinueWith(onComplete);
		}
	}

	[Obsolete("Use EraseAsync instead.")]
	public void Erase(Action<OVRSpatialAnchor, bool> onComplete = null)
	{
		this.Erase(this._defaultEraseOptions, onComplete);
	}

	[Obsolete("Use EraseAsync instead.")]
	public void Erase(OVRSpatialAnchor.EraseOptions eraseOptions, Action<OVRSpatialAnchor, bool> onComplete = null)
	{
		OVRTask<bool> task = this.EraseAsync(eraseOptions);
		if (onComplete != null)
		{
			OVRSpatialAnchor.InvertedCapture<bool, OVRSpatialAnchor>.ContinueTaskWith(task, onComplete, this);
		}
	}

	[Obsolete("Use LoadUnboundAnchorsAsync instead.")]
	public static bool LoadUnboundAnchors(OVRSpatialAnchor.LoadOptions options, Action<OVRSpatialAnchor.UnboundAnchor[]> onComplete)
	{
		OVRTask<OVRSpatialAnchor.UnboundAnchor[]> ovrtask = OVRSpatialAnchor.LoadUnboundAnchorsAsync(options);
		ovrtask.ContinueWith(onComplete);
		return ovrtask.IsPending;
	}

	[Obsolete("Use ShareAsync instead.")]
	public static void Share(ICollection<OVRSpatialAnchor> anchors, ICollection<OVRSpaceUser> users, Action<ICollection<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		if (users == null)
		{
			throw new ArgumentNullException("users");
		}
		using (NativeArray<ulong> spaces = OVRSpatialAnchor.ToNativeArray(anchors))
		{
			NativeArray<ulong> nativeArray = new NativeArray<ulong>(users.Count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			using (nativeArray)
			{
				int num = 0;
				foreach (OVRSpaceUser ovrspaceUser in users)
				{
					nativeArray[num++] = ovrspaceUser._handle;
				}
				ulong key;
				OVRPlugin.Result result = OVRPlugin.ShareSpaces(spaces, nativeArray, out key);
				if (result.IsSuccess())
				{
					OVRSpatialAnchor.MultiAnchorCompletionDelegates[key] = new OVRSpatialAnchor.MultiAnchorDelegatePair
					{
						Anchors = OVRSpatialAnchor.CopyAnchorListIntoListFromPool(anchors),
						Delegate = onComplete
					};
				}
				else if (onComplete != null)
				{
					onComplete(anchors, (OVRSpatialAnchor.OperationResult)result);
				}
			}
		}
	}

	[Obsolete("Use SaveAsync instead.")]
	public unsafe static void Save(ICollection<OVRSpatialAnchor> anchors, OVRSpatialAnchor.SaveOptions saveOptions, Action<ICollection<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> onComplete = null)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		using (NativeArray<ulong> nativeArray = OVRSpatialAnchor.ToNativeArray(anchors))
		{
			ulong key;
			OVRPlugin.Result result = OVRAnchor.SaveSpaceList((ulong*)nativeArray.GetUnsafeReadOnlyPtr<ulong>(), (uint)nativeArray.Length, saveOptions.Storage.ToSpaceStorageLocation(), out key);
			if (result.IsSuccess())
			{
				OVRSpatialAnchor.MultiAnchorCompletionDelegates[key] = new OVRSpatialAnchor.MultiAnchorDelegatePair
				{
					Anchors = OVRSpatialAnchor.CopyAnchorListIntoListFromPool(anchors),
					Delegate = onComplete
				};
			}
			else if (onComplete != null)
			{
				onComplete(anchors, (OVRSpatialAnchor.OperationResult)result);
			}
		}
	}

	[Obsolete("Use EraseAnchorAsync instead.")]
	public OVRTask<bool> EraseAsync()
	{
		return this.EraseAsync(this._defaultEraseOptions);
	}

	[Obsolete("Use EraseAnchorAsync instead.")]
	public OVRTask<bool> EraseAsync(OVRSpatialAnchor.EraseOptions eraseOptions)
	{
		ulong requestId;
		return OVRTask.Build(OVRAnchor.EraseSpace(this._anchor.Handle, eraseOptions.Storage.ToSpaceStorageLocation(), out requestId), requestId).ToTask<bool>(false);
	}

	[Obsolete("Use SaveAnchorAsync instead.")]
	public OVRTask<bool> SaveAsync()
	{
		return this.SaveAsync(this._defaultSaveOptions);
	}

	[Obsolete("Use SaveAnchorAsync instead.")]
	public OVRTask<bool> SaveAsync(OVRSpatialAnchor.SaveOptions saveOptions)
	{
		Guid guid = Guid.NewGuid();
		OVRSpatialAnchor.SaveRequests[saveOptions.Storage].Add(this);
		OVRSpatialAnchor.AsyncRequestTaskIds[this] = guid;
		return OVRTask.FromGuid<bool>(guid);
	}

	[Obsolete("Use SaveAnchorsAsync instead.")]
	public static OVRTask<OVRSpatialAnchor.OperationResult> SaveAsync(IEnumerable<OVRSpatialAnchor> anchors, OVRSpatialAnchor.SaveOptions saveOptions)
	{
		if (anchors == null)
		{
			throw new ArgumentNullException("anchors");
		}
		OVRTask<OVRSpatialAnchor.OperationResult> result;
		using (OVRNativeList<ulong> list = new OVRNativeList<ulong>(anchors.ToNonAlloc<OVRSpatialAnchor>().Count, Allocator.Temp))
		{
			foreach (OVRSpatialAnchor ovrspatialAnchor in anchors.ToNonAlloc<OVRSpatialAnchor>())
			{
				list.Add(ovrspatialAnchor._anchor.Handle);
			}
			ulong requestId;
			result = OVRTask.Build(OVRAnchor.SaveSpaceList(list, (uint)list.Count, saveOptions.Storage.ToSpaceStorageLocation(), out requestId), requestId).ToTask<OVRSpatialAnchor.OperationResult>();
		}
		return result;
	}

	[Obsolete("Use the overload of LoadUnboundAnchorsAsync that accepts a collection of Guids instead.")]
	public static OVRTask<OVRSpatialAnchor.UnboundAnchor[]> LoadUnboundAnchorsAsync(OVRSpatialAnchor.LoadOptions options)
	{
		if (options.Uuids == null)
		{
			throw new InvalidOperationException("LoadOptions.Uuids must not be null.");
		}
		ulong requestId;
		return OVRTask.Build(options.ToQueryOptions().TryQuerySpaces(out requestId), requestId).ToTask<OVRSpatialAnchor.UnboundAnchor[]>(null);
	}

	private static NativeArray<ulong> ToNativeArray(ICollection<OVRSpatialAnchor> anchors)
	{
		int count = anchors.Count;
		NativeArray<ulong> result = new NativeArray<ulong>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
		int num = 0;
		foreach (OVRSpatialAnchor ovrspatialAnchor in anchors.ToNonAlloc<OVRSpatialAnchor>())
		{
			result[num++] = (ovrspatialAnchor ? ovrspatialAnchor._anchor.Handle : 0UL);
		}
		return result;
	}

	private static List<OVRSpatialAnchor> CopyAnchorListIntoListFromPool(IEnumerable<OVRSpatialAnchor> anchorList)
	{
		List<OVRSpatialAnchor> list = OVRObjectPool.List<OVRSpatialAnchor>();
		list.AddRange(anchorList);
		return list;
	}

	[Obsolete]
	private static void SaveBatchAnchors()
	{
		foreach (KeyValuePair<OVRSpace.StorageLocation, List<OVRSpatialAnchor>> keyValuePair in OVRSpatialAnchor.SaveRequests)
		{
			if (keyValuePair.Value.Count != 0)
			{
				OVRSpatialAnchor.Save(keyValuePair.Value, new OVRSpatialAnchor.SaveOptions
				{
					Storage = keyValuePair.Key
				}, null);
				keyValuePair.Value.Clear();
			}
		}
	}

	private static void OnSpaceSaveComplete(ulong requestId, OVRSpace space, bool result, Guid uuid)
	{
	}

	private static void OnSpaceEraseComplete(ulong requestId, bool result, Guid uuid, OVRPlugin.SpaceStorageLocation location)
	{
	}

	private static void OnSpaceQueryComplete(ulong requestId, bool queryResult)
	{
		OVRTask<OVRSpatialAnchor.UnboundAnchor[]> ovrtask;
		if (!OVRTask.TryGetPendingTask<OVRSpatialAnchor.UnboundAnchor[]>(requestId, out ovrtask))
		{
			return;
		}
		if (!queryResult)
		{
			ovrtask.SetResult(null);
			return;
		}
		NativeArray<OVRPlugin.SpaceQueryResult> nativeArray;
		if (!OVRPlugin.RetrieveSpaceQueryResults(requestId, out nativeArray, Allocator.Temp))
		{
			ovrtask.SetResult(null);
			return;
		}
		using (nativeArray)
		{
			List<OVRSpatialAnchor.UnboundAnchor> list;
			using (new OVRObjectPool.ListScope<OVRSpatialAnchor.UnboundAnchor>(ref list))
			{
				foreach (OVRPlugin.SpaceQueryResult spaceQueryResult in nativeArray)
				{
					OVRSpatialAnchor.UnboundAnchor item;
					if (OVRSpatialAnchor.TryGetUnbound(new OVRAnchor(spaceQueryResult.space, spaceQueryResult.uuid), out item))
					{
						list.Add(item);
					}
				}
				OVRSpatialAnchor.UnboundAnchor[] result = (list.Count == 0) ? Array.Empty<OVRSpatialAnchor.UnboundAnchor>() : list.ToArray();
				ovrtask.SetResult(result);
			}
		}
	}

	private static void OnSpaceListSaveComplete(ulong requestId, OVRSpatialAnchor.OperationResult result)
	{
		OVRTask.SetResult<OVRSpatialAnchor.OperationResult>(requestId, result);
		OVRSpatialAnchor.InvokeMultiAnchorDelegate(requestId, result, OVRSpatialAnchor.MultiAnchorActionType.Save);
	}

	private bool _startCalled;

	private ulong _requestId;

	private bool _creationFailed;

	internal static readonly Dictionary<Guid, OVRSpatialAnchor> SpatialAnchors = new Dictionary<Guid, OVRSpatialAnchor>();

	private static readonly Dictionary<ulong, OVRSpatialAnchor> CreationRequests = new Dictionary<ulong, OVRSpatialAnchor>();

	private static readonly Dictionary<OVRSpatialAnchor, Guid> AsyncRequestTaskIds = new Dictionary<OVRSpatialAnchor, Guid>();

	private static readonly List<ValueTuple<List<OVRSpaceUser>, List<OVRSpatialAnchor>>> ShareRequests = new List<ValueTuple<List<OVRSpaceUser>, List<OVRSpatialAnchor>>>();

	private static readonly Dictionary<ulong, OVRSpatialAnchor.MultiAnchorDelegatePair> MultiAnchorCompletionDelegates = new Dictionary<ulong, OVRSpatialAnchor.MultiAnchorDelegatePair>();

	[Obsolete("See SaveAnchorAsync overload without SaveOptions")]
	private readonly OVRSpatialAnchor.SaveOptions _defaultSaveOptions = new OVRSpatialAnchor.SaveOptions
	{
		Storage = OVRSpace.StorageLocation.Local
	};

	[Obsolete("See EraseAnchorAsync overload without EraseOptions")]
	private readonly OVRSpatialAnchor.EraseOptions _defaultEraseOptions = new OVRSpatialAnchor.EraseOptions
	{
		Storage = OVRSpace.StorageLocation.Local
	};

	[Obsolete]
	private static readonly Dictionary<OVRSpace.StorageLocation, List<OVRSpatialAnchor>> SaveRequests = new Dictionary<OVRSpace.StorageLocation, List<OVRSpatialAnchor>>
	{
		{
			OVRSpace.StorageLocation.Cloud,
			new List<OVRSpatialAnchor>()
		},
		{
			OVRSpace.StorageLocation.Local,
			new List<OVRSpatialAnchor>()
		}
	};

	private struct MultiAnchorDelegatePair
	{
		public List<OVRSpatialAnchor> Anchors;

		public Action<ICollection<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> Delegate;
	}

	public readonly struct UnboundAnchor
	{
		public Guid Uuid { get; }

		public bool Localized
		{
			get
			{
				bool flag;
				bool flag2;
				return OVRPlugin.GetSpaceComponentStatus(this._space, OVRPlugin.SpaceComponentType.Locatable, out flag, out flag2) && flag;
			}
		}

		public bool Localizing
		{
			get
			{
				bool flag;
				bool flag2;
				return OVRPlugin.GetSpaceComponentStatus(this._space, OVRPlugin.SpaceComponentType.Locatable, out flag, out flag2) && !flag && flag2;
			}
		}

		public bool TryGetPose(out Pose pose)
		{
			OVRAnchor lhs = new OVRAnchor(this._space, this.Uuid);
			if (lhs == OVRAnchor.Null)
			{
				throw new InvalidOperationException("The UnboundAnchor is not valid. Was it default (zero) initialized?");
			}
			OVRLocatable ovrlocatable;
			if (!lhs.TryGetComponent<OVRLocatable>(out ovrlocatable))
			{
				throw new InvalidOperationException(string.Format("Anchor {0} is not localizable.", this.Uuid));
			}
			if (!ovrlocatable.IsEnabled)
			{
				throw new InvalidOperationException(string.Format("The anchor {0} is not localized. An anchor must be localized before getting the pose.", this.Uuid));
			}
			OVRPose ovrpose;
			if (OVRSpatialAnchor.TryGetPose(this._space, out ovrpose))
			{
				pose = new Pose(ovrpose.position, ovrpose.orientation);
				return true;
			}
			pose = Pose.identity;
			return false;
		}

		public OVRTask<bool> LocalizeAsync(double timeout = 0.0)
		{
			OVRAnchor ovranchor = new OVRAnchor(this._space, this.Uuid);
			OVRStorable ovrstorable;
			if (ovranchor.TryGetComponent<OVRStorable>(out ovrstorable))
			{
				ovrstorable.SetEnabledAsync(true, 0.0);
			}
			OVRSharable ovrsharable;
			if (ovranchor.TryGetComponent<OVRSharable>(out ovrsharable))
			{
				ovrsharable.SetEnabledAsync(true, 0.0);
			}
			return ovranchor.GetComponent<OVRLocatable>().SetEnabledAsync(true, timeout);
		}

		public void BindTo(OVRSpatialAnchor spatialAnchor)
		{
			if (!this._space.Valid)
			{
				throw new InvalidOperationException("UnboundAnchor does not refer to a valid anchor.");
			}
			if (spatialAnchor == null)
			{
				throw new ArgumentNullException("spatialAnchor");
			}
			if (spatialAnchor.Created)
			{
				throw new ArgumentException(string.Format("Cannot bind {0} to {1} because {2} is already bound to {3}.", new object[]
				{
					this.Uuid,
					"spatialAnchor",
					"spatialAnchor",
					spatialAnchor.Uuid
				}), "spatialAnchor");
			}
			if (spatialAnchor.PendingCreation)
			{
				throw new ArgumentException(string.Format("Cannot bind {0} to {1} because {2} is being used to create a new spatial anchor.", this.Uuid, "spatialAnchor", "spatialAnchor"), "spatialAnchor");
			}
			OVRSpatialAnchor.ThrowIfBound(this.Uuid);
			spatialAnchor.InitializeUnchecked(this._space, this.Uuid);
		}

		internal UnboundAnchor(OVRSpace space, Guid uuid)
		{
			this._space = space;
			this.Uuid = uuid;
		}

		[Obsolete("Use LocalizeAsync instead.")]
		public void Localize(Action<OVRSpatialAnchor.UnboundAnchor, bool> onComplete = null, double timeout = 0.0)
		{
			OVRTask<bool> task = this.LocalizeAsync(timeout);
			if (onComplete != null)
			{
				OVRSpatialAnchor.InvertedCapture<bool, OVRSpatialAnchor.UnboundAnchor>.ContinueTaskWith(task, onComplete, this);
			}
		}

		[Obsolete("Use TryGetPose instead.")]
		public Pose Pose
		{
			get
			{
				Pose result;
				if (!this.TryGetPose(out result))
				{
					throw new InvalidOperationException(string.Format("[{0}] Anchor must be localized before obtaining its pose.", this.Uuid));
				}
				return result;
			}
		}

		internal readonly OVRSpace _space;
	}

	private enum MultiAnchorActionType
	{
		Save,
		Share
	}

	private static class Development
	{
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void Log(string message)
		{
			Debug.Log("[OVRSpatialAnchor] " + message);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void LogWarning(string message)
		{
			Debug.LogWarning("[OVRSpatialAnchor] " + message);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void LogError(string message)
		{
			Debug.LogError("[OVRSpatialAnchor] " + message);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void LogRequestOrError(ulong requestId, OVRPlugin.Result result, string successMessage, string failureMessage)
		{
			result.IsSuccess();
		}

		[Conditional("DEVELOPMENT_BUILD")]
		public static void LogRequest(ulong requestId, string message)
		{
		}

		[Conditional("DEVELOPMENT_BUILD")]
		public static void LogRequestResult(ulong requestId, bool result, string successMessage, string failureMessage)
		{
		}
	}

	[OVRResultStatus]
	public enum OperationResult
	{
		Success,
		Failure = -1000,
		Failure_DataIsInvalid = -1008,
		Failure_InvalidParameter = -1001,
		Failure_SpaceCloudStorageDisabled = -2000,
		Failure_SpaceMappingInsufficient = -2001,
		Failure_SpaceLocalizationFailed = -2002,
		Failure_SpaceNetworkTimeout = -2003,
		Failure_SpaceNetworkRequestFailed = -2004,
		Failure_GroupNotFound = -2009
	}

	private readonly struct InvertedCapture<TResult, TCapture>
	{
		private InvertedCapture(Action<TCapture, TResult> callback, TCapture capture)
		{
			this._callback = callback;
			this._capture = capture;
		}

		private static void Invoke(TResult result, OVRSpatialAnchor.InvertedCapture<TResult, TCapture> invertedCapture)
		{
			Action<TCapture, TResult> callback = invertedCapture._callback;
			if (callback == null)
			{
				return;
			}
			callback(invertedCapture._capture, result);
		}

		public static void ContinueTaskWith(OVRTask<TResult> task, Action<TCapture, TResult> onCompleted, TCapture state)
		{
			task.ContinueWith<OVRSpatialAnchor.InvertedCapture<TResult, TCapture>>(OVRSpatialAnchor.InvertedCapture<TResult, TCapture>.s_delegate, new OVRSpatialAnchor.InvertedCapture<TResult, TCapture>(onCompleted, state));
		}

		private static readonly Action<TResult, OVRSpatialAnchor.InvertedCapture<TResult, TCapture>> s_delegate = new Action<TResult, OVRSpatialAnchor.InvertedCapture<TResult, TCapture>>(OVRSpatialAnchor.InvertedCapture<TResult, TCapture>.Invoke);

		private readonly TCapture _capture;

		private readonly Action<TCapture, TResult> _callback;
	}

	[Obsolete("Use EraseAnchorAsync instead, which does not require you to provide EraseOptions.")]
	public struct EraseOptions
	{
		public OVRSpace.StorageLocation Storage;
	}

	[Obsolete("Use SaveAnchorAsync instead, which does not require you to provide SaveOptions.")]
	public struct SaveOptions
	{
		public OVRSpace.StorageLocation Storage;
	}

	[Obsolete("Only for use with the obsolete version of LoadUnboundAnchorsAsync. Use the overload of LoadUnboundAnchorsAsync that accepts a collection of Guids")]
	public struct LoadOptions
	{
		public OVRSpace.StorageLocation StorageLocation { readonly get; set; }

		[Obsolete("This property is no longer required. MaxAnchorCount will be automatically set to the number of uuids to load.")]
		public int MaxAnchorCount { readonly get; set; }

		public double Timeout { readonly get; set; }

		public IReadOnlyList<Guid> Uuids
		{
			get
			{
				return this._uuids;
			}
			set
			{
				if (value != null && value.Count > 1024)
				{
					throw new ArgumentException(string.Format("There must not be more than {0} UUIDs (new value contains {1} UUIDs).", 1024, value.Count), "value");
				}
				this._uuids = value;
			}
		}

		internal OVRSpaceQuery.Options ToQueryOptions()
		{
			return new OVRSpaceQuery.Options
			{
				Location = this.StorageLocation,
				MaxResults = 1024,
				Timeout = this.Timeout,
				UuidFilter = this.Uuids,
				QueryType = OVRPlugin.SpaceQueryType.Action,
				ActionType = OVRPlugin.SpaceQueryActionType.Load
			};
		}

		public const int MaxSupported = 1024;

		private IReadOnlyList<Guid> _uuids;
	}
}
