using System;
using UnityEngine;

namespace Meta.XR.BuildingBlocks
{
	public class SpatialAnchorSpawnerBuildingBlock : MonoBehaviour
	{
		public GameObject AnchorPrefab
		{
			get
			{
				return this._anchorPrefab;
			}
			set
			{
				this._anchorPrefab = value;
				if (this._anchorPrefabTransform)
				{
					Object.Destroy(this._anchorPrefabTransform.gameObject);
				}
				this._anchorPrefabTransform = Object.Instantiate<GameObject>(this.AnchorPrefab).transform;
				this.FollowHand = this._followHand;
			}
		}

		public bool FollowHand
		{
			get
			{
				return this._followHand;
			}
			set
			{
				this._followHand = value;
				if (this._followHand)
				{
					this._initialPosition = this._anchorPrefabTransform.position;
					this._initialRotation = this._anchorPrefabTransform.rotation;
					this._anchorPrefabTransform.parent = this._cameraRig.rightControllerAnchor;
					this._anchorPrefabTransform.localPosition = Vector3.zero;
					this._anchorPrefabTransform.localRotation = Quaternion.identity;
					return;
				}
				this._anchorPrefabTransform.parent = null;
				this._anchorPrefabTransform.SetPositionAndRotation(this._initialPosition, this._initialRotation);
			}
		}

		private void Awake()
		{
			this._spatialAnchorCore = SpatialAnchorCoreBuildingBlock.GetFirstInstance();
			this._cameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
			this.AnchorPrefab = this._anchorPrefab;
			this.FollowHand = this._followHand;
		}

		public void SpawnSpatialAnchor(Vector3 position, Quaternion rotation)
		{
			this._spatialAnchorCore.InstantiateSpatialAnchor(this.AnchorPrefab, position, rotation);
		}

		internal void SpawnSpatialAnchor()
		{
			if (!this.FollowHand)
			{
				this.SpawnSpatialAnchor(this.AnchorPrefab.transform.position, this.AnchorPrefab.transform.rotation);
				return;
			}
			this.SpawnSpatialAnchor(this._anchorPrefabTransform.position, this._anchorPrefabTransform.rotation);
		}

		[Tooltip("A placeholder object to place in the anchor's position.")]
		[SerializeField]
		private GameObject _anchorPrefab;

		[Tooltip("Anchor prefab GameObject will follow the user's right hand.")]
		[SerializeField]
		private bool _followHand = true;

		private SpatialAnchorCoreBuildingBlock _spatialAnchorCore;

		private OVRCameraRig _cameraRig;

		private Transform _anchorPrefabTransform;

		private Vector3 _initialPosition;

		private Quaternion _initialRotation;
	}
}
