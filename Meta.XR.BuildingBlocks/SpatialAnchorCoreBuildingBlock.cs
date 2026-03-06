using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.BuildingBlocks
{
	public class SpatialAnchorCoreBuildingBlock : MonoBehaviour
	{
		public UnityEvent<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult> OnAnchorCreateCompleted
		{
			get
			{
				return this._onAnchorCreateCompleted;
			}
			set
			{
				this._onAnchorCreateCompleted = value;
			}
		}

		public UnityEvent<List<OVRSpatialAnchor>> OnAnchorsLoadCompleted
		{
			get
			{
				return this._onAnchorsLoadCompleted;
			}
			set
			{
				this._onAnchorsLoadCompleted = value;
			}
		}

		public UnityEvent<OVRSpatialAnchor.OperationResult> OnAnchorsEraseAllCompleted
		{
			get
			{
				return this._onAnchorsEraseAllCompleted;
			}
			set
			{
				this._onAnchorsEraseAllCompleted = value;
			}
		}

		public UnityEvent<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult> OnAnchorEraseCompleted
		{
			get
			{
				return this._onAnchorEraseCompleted;
			}
			set
			{
				this._onAnchorEraseCompleted = value;
			}
		}

		protected OVRSpatialAnchor.OperationResult Result { get; set; }

		public void InstantiateSpatialAnchor(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			if (prefab == null)
			{
				prefab = new GameObject("Spatial Anchor");
			}
			OVRSpatialAnchor anchor = Object.Instantiate<GameObject>(prefab, position, rotation).AddComponent<OVRSpatialAnchor>();
			this.InitSpatialAnchorAsync(anchor);
		}

		private void InitSpatialAnchorAsync(OVRSpatialAnchor anchor)
		{
			SpatialAnchorCoreBuildingBlock.<InitSpatialAnchorAsync>d__21 <InitSpatialAnchorAsync>d__;
			<InitSpatialAnchorAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<InitSpatialAnchorAsync>d__.<>4__this = this;
			<InitSpatialAnchorAsync>d__.anchor = anchor;
			<InitSpatialAnchorAsync>d__.<>1__state = -1;
			<InitSpatialAnchorAsync>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<InitSpatialAnchorAsync>d__21>(ref <InitSpatialAnchorAsync>d__);
		}

		protected Task WaitForInit(OVRSpatialAnchor anchor)
		{
			SpatialAnchorCoreBuildingBlock.<WaitForInit>d__22 <WaitForInit>d__;
			<WaitForInit>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForInit>d__.<>4__this = this;
			<WaitForInit>d__.anchor = anchor;
			<WaitForInit>d__.<>1__state = -1;
			<WaitForInit>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<WaitForInit>d__22>(ref <WaitForInit>d__);
			return <WaitForInit>d__.<>t__builder.Task;
		}

		protected Task SaveAsync(OVRSpatialAnchor anchor)
		{
			SpatialAnchorCoreBuildingBlock.<SaveAsync>d__23 <SaveAsync>d__;
			<SaveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAsync>d__.<>4__this = this;
			<SaveAsync>d__.anchor = anchor;
			<SaveAsync>d__.<>1__state = -1;
			<SaveAsync>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<SaveAsync>d__23>(ref <SaveAsync>d__);
			return <SaveAsync>d__.<>t__builder.Task;
		}

		public virtual void LoadAndInstantiateAnchors(GameObject prefab, List<Guid> uuids)
		{
			if (uuids == null)
			{
				throw new ArgumentNullException();
			}
			if (uuids.Count == 0)
			{
				Debug.Log("[SpatialAnchorCoreBuildingBlock] Uuid list is empty.");
				return;
			}
			this.LoadAnchorsAsync(prefab, uuids);
		}

		public void EraseAllAnchors()
		{
			if (OVRSpatialAnchor.SpatialAnchors.Count == 0)
			{
				return;
			}
			this.EraseAnchorsAsync();
		}

		public void EraseAnchorByUuid(Guid uuid)
		{
			SpatialAnchorCoreBuildingBlock.<EraseAnchorByUuid>d__26 <EraseAnchorByUuid>d__;
			<EraseAnchorByUuid>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<EraseAnchorByUuid>d__.<>4__this = this;
			<EraseAnchorByUuid>d__.uuid = uuid;
			<EraseAnchorByUuid>d__.<>1__state = -1;
			<EraseAnchorByUuid>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<EraseAnchorByUuid>d__26>(ref <EraseAnchorByUuid>d__);
		}

		protected void LoadAnchorsAsync(GameObject prefab, IEnumerable<Guid> uuids)
		{
			SpatialAnchorCoreBuildingBlock.<LoadAnchorsAsync>d__27 <LoadAnchorsAsync>d__;
			<LoadAnchorsAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadAnchorsAsync>d__.<>4__this = this;
			<LoadAnchorsAsync>d__.prefab = prefab;
			<LoadAnchorsAsync>d__.uuids = uuids;
			<LoadAnchorsAsync>d__.<>1__state = -1;
			<LoadAnchorsAsync>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<LoadAnchorsAsync>d__27>(ref <LoadAnchorsAsync>d__);
		}

		private void EraseAnchorsAsync()
		{
			SpatialAnchorCoreBuildingBlock.<EraseAnchorsAsync>d__28 <EraseAnchorsAsync>d__;
			<EraseAnchorsAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<EraseAnchorsAsync>d__.<>4__this = this;
			<EraseAnchorsAsync>d__.<>1__state = -1;
			<EraseAnchorsAsync>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<EraseAnchorsAsync>d__28>(ref <EraseAnchorsAsync>d__);
		}

		private Task EraseAnchorByUuidAsync(OVRSpatialAnchor anchor)
		{
			SpatialAnchorCoreBuildingBlock.<EraseAnchorByUuidAsync>d__29 <EraseAnchorByUuidAsync>d__;
			<EraseAnchorByUuidAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<EraseAnchorByUuidAsync>d__.<>4__this = this;
			<EraseAnchorByUuidAsync>d__.anchor = anchor;
			<EraseAnchorByUuidAsync>d__.<>1__state = -1;
			<EraseAnchorByUuidAsync>d__.<>t__builder.Start<SpatialAnchorCoreBuildingBlock.<EraseAnchorByUuidAsync>d__29>(ref <EraseAnchorByUuidAsync>d__);
			return <EraseAnchorByUuidAsync>d__.<>t__builder.Task;
		}

		internal static SpatialAnchorCoreBuildingBlock GetFirstInstance()
		{
			foreach (SpatialAnchorCoreBuildingBlock spatialAnchorCoreBuildingBlock in Object.FindObjectsByType<SpatialAnchorCoreBuildingBlock>(FindObjectsSortMode.None))
			{
				if (spatialAnchorCoreBuildingBlock != null && spatialAnchorCoreBuildingBlock.GetType() == typeof(SpatialAnchorCoreBuildingBlock))
				{
					return spatialAnchorCoreBuildingBlock;
				}
			}
			return null;
		}

		[Header("# Events")]
		[SerializeField]
		private UnityEvent<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult> _onAnchorCreateCompleted;

		[SerializeField]
		private UnityEvent<List<OVRSpatialAnchor>> _onAnchorsLoadCompleted;

		[SerializeField]
		private UnityEvent<OVRSpatialAnchor.OperationResult> _onAnchorsEraseAllCompleted;

		[SerializeField]
		private UnityEvent<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult> _onAnchorEraseCompleted;
	}
}
