using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.BuildingBlocks
{
	public class SharedSpatialAnchorCore : SpatialAnchorCoreBuildingBlock
	{
		public UnityEvent<List<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> OnSpatialAnchorsShareCompleted
		{
			get
			{
				return this._onSpatialAnchorsShareCompleted;
			}
			set
			{
				this._onSpatialAnchorsShareCompleted = value;
			}
		}

		public UnityEvent<List<OVRSpatialAnchor>, OVRAnchor.ShareResult> OnSpatialAnchorsShareToGroupCompleted
		{
			get
			{
				return this._onSpatialAnchorsShareToGroupCompleted;
			}
			set
			{
				this._onSpatialAnchorsShareToGroupCompleted = value;
			}
		}

		public UnityEvent<List<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> OnSharedSpatialAnchorsLoadCompleted
		{
			get
			{
				return this._onSharedSpatialAnchorsLoadCompleted;
			}
			set
			{
				this._onSharedSpatialAnchorsLoadCompleted = value;
			}
		}

		private void Start()
		{
			this._onShareCompleted = (Action<OVRSpatialAnchor.OperationResult, IEnumerable<OVRSpatialAnchor>>)Delegate.Combine(this._onShareCompleted, new Action<OVRSpatialAnchor.OperationResult, IEnumerable<OVRSpatialAnchor>>(this.OnShareCompleted));
			this._onShareToGroupCompleted = (Action<OVRResult<OVRAnchor.ShareResult>, IEnumerable<OVRSpatialAnchor>>)Delegate.Combine(this._onShareToGroupCompleted, new Action<OVRResult<OVRAnchor.ShareResult>, IEnumerable<OVRSpatialAnchor>>(this.OnShareToGroupCompleted));
		}

		public new void InstantiateSpatialAnchor(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			SharedSpatialAnchorCore.<InstantiateSpatialAnchor>d__15 <InstantiateSpatialAnchor>d__;
			<InstantiateSpatialAnchor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<InstantiateSpatialAnchor>d__.<>4__this = this;
			<InstantiateSpatialAnchor>d__.prefab = prefab;
			<InstantiateSpatialAnchor>d__.position = position;
			<InstantiateSpatialAnchor>d__.rotation = rotation;
			<InstantiateSpatialAnchor>d__.<>1__state = -1;
			<InstantiateSpatialAnchor>d__.<>t__builder.Start<SharedSpatialAnchorCore.<InstantiateSpatialAnchor>d__15>(ref <InstantiateSpatialAnchor>d__);
		}

		private Task InitSpatialAnchor(OVRSpatialAnchor anchor)
		{
			SharedSpatialAnchorCore.<InitSpatialAnchor>d__16 <InitSpatialAnchor>d__;
			<InitSpatialAnchor>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<InitSpatialAnchor>d__.<>4__this = this;
			<InitSpatialAnchor>d__.anchor = anchor;
			<InitSpatialAnchor>d__.<>1__state = -1;
			<InitSpatialAnchor>d__.<>t__builder.Start<SharedSpatialAnchorCore.<InitSpatialAnchor>d__16>(ref <InitSpatialAnchor>d__);
			return <InitSpatialAnchor>d__.<>t__builder.Task;
		}

		public override void LoadAndInstantiateAnchors(GameObject prefab, List<Guid> uuids)
		{
			SharedSpatialAnchorCore.<LoadAndInstantiateAnchors>d__17 <LoadAndInstantiateAnchors>d__;
			<LoadAndInstantiateAnchors>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadAndInstantiateAnchors>d__.<>4__this = this;
			<LoadAndInstantiateAnchors>d__.prefab = prefab;
			<LoadAndInstantiateAnchors>d__.uuids = uuids;
			<LoadAndInstantiateAnchors>d__.<>1__state = -1;
			<LoadAndInstantiateAnchors>d__.<>t__builder.Start<SharedSpatialAnchorCore.<LoadAndInstantiateAnchors>d__17>(ref <LoadAndInstantiateAnchors>d__);
		}

		public void LoadAndInstantiateAnchorsFromGroup(GameObject prefab, Guid groupUuid)
		{
			SharedSpatialAnchorCore.<LoadAndInstantiateAnchorsFromGroup>d__18 <LoadAndInstantiateAnchorsFromGroup>d__;
			<LoadAndInstantiateAnchorsFromGroup>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadAndInstantiateAnchorsFromGroup>d__.<>4__this = this;
			<LoadAndInstantiateAnchorsFromGroup>d__.prefab = prefab;
			<LoadAndInstantiateAnchorsFromGroup>d__.groupUuid = groupUuid;
			<LoadAndInstantiateAnchorsFromGroup>d__.<>1__state = -1;
			<LoadAndInstantiateAnchorsFromGroup>d__.<>t__builder.Start<SharedSpatialAnchorCore.<LoadAndInstantiateAnchorsFromGroup>d__18>(ref <LoadAndInstantiateAnchorsFromGroup>d__);
		}

		private void LoadSharedSpatialAnchorsRoutine(GameObject prefab, OVRResult<List<OVRSpatialAnchor.UnboundAnchor>, OVRSpatialAnchor.OperationResult> result)
		{
			SharedSpatialAnchorCore.<LoadSharedSpatialAnchorsRoutine>d__19 <LoadSharedSpatialAnchorsRoutine>d__;
			<LoadSharedSpatialAnchorsRoutine>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadSharedSpatialAnchorsRoutine>d__.<>4__this = this;
			<LoadSharedSpatialAnchorsRoutine>d__.prefab = prefab;
			<LoadSharedSpatialAnchorsRoutine>d__.result = result;
			<LoadSharedSpatialAnchorsRoutine>d__.<>1__state = -1;
			<LoadSharedSpatialAnchorsRoutine>d__.<>t__builder.Start<SharedSpatialAnchorCore.<LoadSharedSpatialAnchorsRoutine>d__19>(ref <LoadSharedSpatialAnchorsRoutine>d__);
		}

		public void ShareSpatialAnchors(List<OVRSpatialAnchor> anchors, List<OVRSpaceUser> users)
		{
			if (anchors == null || users == null)
			{
				throw new ArgumentNullException();
			}
			if (anchors.Count == 0 || users.Count == 0)
			{
				throw new ArgumentException("[SharedSpatialAnchorCore] Anchors or users cannot be zero.");
			}
			OVRSpatialAnchor.ShareAsync(anchors, users).ContinueWith<IEnumerable<OVRSpatialAnchor>>(this._onShareCompleted, anchors);
		}

		public void ShareSpatialAnchors(List<OVRSpatialAnchor> anchors, Guid groupUuid)
		{
			if (anchors == null)
			{
				throw new ArgumentNullException();
			}
			if (anchors.Count == 0)
			{
				throw new ArgumentException("[SharedSpatialAnchorCore] Anchors list cannot be zero.");
			}
			OVRSpatialAnchor.ShareAsync(anchors, groupUuid).ContinueWith<IEnumerable<OVRSpatialAnchor>>(this._onShareToGroupCompleted, anchors);
		}

		private void OnShareCompleted(OVRSpatialAnchor.OperationResult result, IEnumerable<OVRSpatialAnchor> anchors)
		{
			if (result == OVRSpatialAnchor.OperationResult.Success)
			{
				List<OVRSpatialAnchor> list;
				using (new OVRObjectPool.ListScope<OVRSpatialAnchor>(ref list))
				{
					list.AddRange(anchors);
					UnityEvent<List<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> onSpatialAnchorsShareCompleted = this.OnSpatialAnchorsShareCompleted;
					if (onSpatialAnchorsShareCompleted != null)
					{
						onSpatialAnchorsShareCompleted.Invoke(new List<OVRSpatialAnchor>(list), OVRSpatialAnchor.OperationResult.Success);
					}
				}
				return;
			}
			UnityEvent<List<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> onSpatialAnchorsShareCompleted2 = this.OnSpatialAnchorsShareCompleted;
			if (onSpatialAnchorsShareCompleted2 == null)
			{
				return;
			}
			onSpatialAnchorsShareCompleted2.Invoke(null, result);
		}

		private void OnShareToGroupCompleted(OVRResult<OVRAnchor.ShareResult> result, IEnumerable<OVRSpatialAnchor> anchors)
		{
			if (result.Success)
			{
				List<OVRSpatialAnchor> list;
				using (new OVRObjectPool.ListScope<OVRSpatialAnchor>(ref list))
				{
					list.AddRange(anchors);
					UnityEvent<List<OVRSpatialAnchor>, OVRAnchor.ShareResult> onSpatialAnchorsShareToGroupCompleted = this.OnSpatialAnchorsShareToGroupCompleted;
					if (onSpatialAnchorsShareToGroupCompleted != null)
					{
						onSpatialAnchorsShareToGroupCompleted.Invoke(new List<OVRSpatialAnchor>(list), result.Status);
					}
				}
				return;
			}
			UnityEvent<List<OVRSpatialAnchor>, OVRAnchor.ShareResult> onSpatialAnchorsShareToGroupCompleted2 = this.OnSpatialAnchorsShareToGroupCompleted;
			if (onSpatialAnchorsShareToGroupCompleted2 == null)
			{
				return;
			}
			onSpatialAnchorsShareToGroupCompleted2.Invoke(null, result.Status);
		}

		private void OnDestroy()
		{
			this._onShareCompleted = (Action<OVRSpatialAnchor.OperationResult, IEnumerable<OVRSpatialAnchor>>)Delegate.Remove(this._onShareCompleted, new Action<OVRSpatialAnchor.OperationResult, IEnumerable<OVRSpatialAnchor>>(this.OnShareCompleted));
			this._onShareToGroupCompleted = (Action<OVRResult<OVRAnchor.ShareResult>, IEnumerable<OVRSpatialAnchor>>)Delegate.Remove(this._onShareToGroupCompleted, new Action<OVRResult<OVRAnchor.ShareResult>, IEnumerable<OVRSpatialAnchor>>(this.OnShareToGroupCompleted));
		}

		[SerializeField]
		private UnityEvent<List<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> _onSpatialAnchorsShareCompleted;

		[SerializeField]
		private UnityEvent<List<OVRSpatialAnchor>, OVRAnchor.ShareResult> _onSpatialAnchorsShareToGroupCompleted;

		[SerializeField]
		private UnityEvent<List<OVRSpatialAnchor>, OVRSpatialAnchor.OperationResult> _onSharedSpatialAnchorsLoadCompleted;

		private Action<OVRSpatialAnchor.OperationResult, IEnumerable<OVRSpatialAnchor>> _onShareCompleted;

		private Action<OVRResult<OVRAnchor.ShareResult>, IEnumerable<OVRSpatialAnchor>> _onShareToGroupCompleted;
	}
}
