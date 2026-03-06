using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[DefaultExecutionOrder(-90)]
	[Feature(Feature.Interaction)]
	public class OVRCameraRigRef : MonoBehaviour, IOVRCameraRigRef
	{
		public OVRCameraRig CameraRig
		{
			get
			{
				return this._ovrCameraRig;
			}
		}

		public OVRHand LeftHand
		{
			get
			{
				return this.GetHandCached(ref this._leftHand, this._ovrCameraRig.leftHandAnchor);
			}
		}

		public OVRHand RightHand
		{
			get
			{
				return this.GetHandCached(ref this._rightHand, this._ovrCameraRig.rightHandAnchor);
			}
		}

		public Transform LeftController
		{
			get
			{
				return this._ovrCameraRig.leftControllerAnchor;
			}
		}

		public Transform RightController
		{
			get
			{
				return this._ovrCameraRig.rightControllerAnchor;
			}
		}

		public event Action<bool> WhenInputDataDirtied = delegate(bool <p0>)
		{
		};

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void FixedUpdate()
		{
			this._isLateUpdate = false;
		}

		protected virtual void Update()
		{
			this._isLateUpdate = false;
		}

		protected virtual void LateUpdate()
		{
			this._isLateUpdate = true;
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.CameraRig.UpdatedAnchors += this.HandleInputDataDirtied;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.CameraRig.UpdatedAnchors -= this.HandleInputDataDirtied;
			}
		}

		private OVRHand GetHandCached(ref OVRHand cachedValue, Transform handAnchor)
		{
			if (cachedValue != null)
			{
				return cachedValue;
			}
			cachedValue = handAnchor.GetComponentInChildren<OVRHand>(true);
			bool requireOvrHands = this._requireOvrHands;
			return cachedValue;
		}

		private void HandleInputDataDirtied(OVRCameraRig cameraRig)
		{
			this.WhenInputDataDirtied(this._isLateUpdate);
		}

		public void InjectAllOVRCameraRigRef(OVRCameraRig ovrCameraRig, bool requireHands)
		{
			this.InjectInteractionOVRCameraRig(ovrCameraRig);
			this.InjectRequireHands(requireHands);
		}

		public void InjectInteractionOVRCameraRig(OVRCameraRig ovrCameraRig)
		{
			this._ovrCameraRig = ovrCameraRig;
			this._leftHand = null;
			this._rightHand = null;
		}

		public void InjectRequireHands(bool requireHands)
		{
			this._requireOvrHands = requireHands;
		}

		[Header("Configuration")]
		[SerializeField]
		private OVRCameraRig _ovrCameraRig;

		[SerializeField]
		private OVRHand _leftHand;

		[SerializeField]
		private OVRHand _rightHand;

		[SerializeField]
		private bool _requireOvrHands = true;

		protected bool _started;

		private bool _isLateUpdate;
	}
}
