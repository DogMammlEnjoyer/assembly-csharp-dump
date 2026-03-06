using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

[ExecuteInEditMode]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-add-camera-rig/")]
public class OVRCameraRig : MonoBehaviour
{
	public Camera leftEyeCamera
	{
		get
		{
			if (!this.usePerEyeCameras)
			{
				return this._centerEyeCamera;
			}
			return this._leftEyeCamera;
		}
	}

	public Camera rightEyeCamera
	{
		get
		{
			if (!this.usePerEyeCameras)
			{
				return this._centerEyeCamera;
			}
			return this._rightEyeCamera;
		}
	}

	public Transform trackingSpace { get; private set; }

	public Transform leftEyeAnchor { get; private set; }

	public Transform centerEyeAnchor { get; private set; }

	public Transform rightEyeAnchor { get; private set; }

	public Transform leftHandAnchor { get; private set; }

	public Transform rightHandAnchor { get; private set; }

	public Transform leftHandAnchorDetached { get; private set; }

	public Transform rightHandAnchorDetached { get; private set; }

	public Transform leftControllerInHandAnchor { get; private set; }

	public Transform leftHandOnControllerAnchor { get; private set; }

	public Transform rightControllerInHandAnchor { get; private set; }

	public Transform rightHandOnControllerAnchor { get; private set; }

	public Transform leftControllerAnchor { get; private set; }

	public Transform rightControllerAnchor { get; private set; }

	public Transform trackerAnchor { get; private set; }

	public event Action<OVRCameraRig> UpdatedAnchors;

	public event Action<Transform> TrackingSpaceChanged;

	protected virtual void Awake()
	{
		this._skipUpdate = true;
		this.EnsureGameObjectIntegrity();
	}

	protected virtual void Start()
	{
		this.UpdateAnchors(true, true);
		Application.onBeforeRender += this.OnBeforeRenderCallback;
	}

	protected virtual void FixedUpdate()
	{
		if (this.useFixedUpdateForTracking)
		{
			this.UpdateAnchors(true, true);
		}
	}

	protected virtual void Update()
	{
		this._skipUpdate = false;
		if (!this.useFixedUpdateForTracking)
		{
			this.UpdateAnchors(true, true);
		}
	}

	protected virtual void OnDestroy()
	{
		Application.onBeforeRender -= this.OnBeforeRenderCallback;
	}

	protected virtual void UpdateAnchors(bool updateEyeAnchors, bool updateHandAnchors)
	{
		if (!OVRManager.OVRManagerinitialized)
		{
			return;
		}
		this.EnsureGameObjectIntegrity();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._skipUpdate)
		{
			this.centerEyeAnchor.FromOVRPose(OVRPose.identity, true);
			this.leftEyeAnchor.FromOVRPose(OVRPose.identity, true);
			this.rightEyeAnchor.FromOVRPose(OVRPose.identity, true);
			return;
		}
		bool monoscopic = OVRManager.instance.monoscopic;
		bool flag = OVRNodeStateProperties.IsHmdPresent();
		OVRPose pose = OVRManager.tracker.GetPose(0);
		this.trackerAnchor.localRotation = pose.orientation;
		Quaternion localRotation = Quaternion.Euler(-OVRManager.instance.headPoseRelativeOffsetRotation.x, -OVRManager.instance.headPoseRelativeOffsetRotation.y, OVRManager.instance.headPoseRelativeOffsetRotation.z);
		if (updateEyeAnchors)
		{
			if (flag)
			{
				Vector3 zero = Vector3.zero;
				Quaternion identity = Quaternion.identity;
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.CenterEye, NodeStatePropertyType.Position, OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out zero))
				{
					this.centerEyeAnchor.localPosition = zero;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.CenterEye, NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out identity))
				{
					this.centerEyeAnchor.localRotation = identity;
				}
			}
			else
			{
				this.centerEyeAnchor.localRotation = localRotation;
				this.centerEyeAnchor.localPosition = OVRManager.instance.headPoseRelativeOffsetTranslation;
			}
			if (!flag || monoscopic)
			{
				this.leftEyeAnchor.localPosition = this.centerEyeAnchor.localPosition;
				this.rightEyeAnchor.localPosition = this.centerEyeAnchor.localPosition;
				this.leftEyeAnchor.localRotation = this.centerEyeAnchor.localRotation;
				this.rightEyeAnchor.localRotation = this.centerEyeAnchor.localRotation;
			}
			else
			{
				Vector3 zero2 = Vector3.zero;
				Vector3 zero3 = Vector3.zero;
				Quaternion identity2 = Quaternion.identity;
				Quaternion identity3 = Quaternion.identity;
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftEye, NodeStatePropertyType.Position, OVRPlugin.Node.EyeLeft, OVRPlugin.Step.Render, out zero2))
				{
					this.leftEyeAnchor.localPosition = zero2;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightEye, NodeStatePropertyType.Position, OVRPlugin.Node.EyeRight, OVRPlugin.Step.Render, out zero3))
				{
					this.rightEyeAnchor.localPosition = zero3;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.LeftEye, NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeLeft, OVRPlugin.Step.Render, out identity2))
				{
					this.leftEyeAnchor.localRotation = identity2;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.RightEye, NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeRight, OVRPlugin.Step.Render, out identity3))
				{
					this.rightEyeAnchor.localRotation = identity3;
				}
			}
		}
		if (updateHandAnchors)
		{
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
			{
				Vector3 zero4 = Vector3.zero;
				Vector3 zero5 = Vector3.zero;
				Quaternion identity4 = Quaternion.identity;
				Quaternion identity5 = Quaternion.identity;
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.LeftHand, NodeStatePropertyType.Position, OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render, out zero4))
				{
					this.leftHandAnchor.localPosition = zero4;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.RightHand, NodeStatePropertyType.Position, OVRPlugin.Node.HandRight, OVRPlugin.Step.Render, out zero5))
				{
					this.rightHandAnchor.localPosition = zero5;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.LeftHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render, out identity4))
				{
					this.leftHandAnchor.localRotation = identity4;
				}
				if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.RightHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.HandRight, OVRPlugin.Step.Render, out identity5))
				{
					this.rightHandAnchor.localRotation = identity5;
				}
			}
			else
			{
				OVRInput.Controller controller = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.LeftHanded);
				OVRInput.Controller controller2 = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);
				if (controller == OVRInput.Controller.None)
				{
					if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LHand))
					{
						controller = OVRInput.Controller.LHand;
					}
					else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch))
					{
						controller = OVRInput.Controller.LTouch;
					}
				}
				if (controller2 == OVRInput.Controller.None)
				{
					if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RHand))
					{
						controller2 = OVRInput.Controller.RHand;
					}
					else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch))
					{
						controller2 = OVRInput.Controller.RTouch;
					}
				}
				this.leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(controller);
				this.rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(controller2);
				this.leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(controller);
				this.rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(controller2);
				OVRInput.ControllerInHandState controllerIsInHandState = OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandLeft);
				if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
				{
					this.leftHandAnchorDetached.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
					this.leftHandAnchorDetached.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
					this.leftHandOnControllerAnchor.localPosition = Vector3.zero;
					this.leftHandOnControllerAnchor.localRotation = Quaternion.identity;
				}
				else if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerInHand)
				{
					Vector3 position = this.trackingSpace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand));
					this.leftHandOnControllerAnchor.localPosition = this.leftHandAnchor.InverseTransformPoint(position);
					this.leftHandOnControllerAnchor.localRotation = Quaternion.Inverse(this.leftHandAnchor.localRotation) * OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
					this.leftHandAnchorDetached.localPosition = Vector3.zero;
					this.leftHandAnchorDetached.localRotation = Quaternion.identity;
				}
				else
				{
					this.leftHandAnchorDetached.localPosition = Vector3.zero;
					this.leftHandAnchorDetached.localRotation = Quaternion.identity;
					this.leftHandOnControllerAnchor.localPosition = Vector3.zero;
					this.leftHandOnControllerAnchor.localRotation = Quaternion.identity;
				}
				controllerIsInHandState = OVRInput.GetControllerIsInHandState(OVRInput.Hand.HandRight);
				if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
				{
					this.rightHandAnchorDetached.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
					this.rightHandAnchorDetached.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
					this.rightHandOnControllerAnchor.localPosition = Vector3.zero;
					this.rightHandOnControllerAnchor.localRotation = Quaternion.identity;
				}
				else if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerInHand)
				{
					Vector3 position2 = this.trackingSpace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand));
					this.rightHandOnControllerAnchor.localPosition = this.rightHandAnchor.InverseTransformPoint(position2);
					this.rightHandOnControllerAnchor.localRotation = Quaternion.Inverse(this.rightHandAnchor.localRotation) * OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand);
					this.rightHandAnchorDetached.localPosition = Vector3.zero;
					this.rightHandAnchorDetached.localRotation = Quaternion.identity;
				}
				else
				{
					this.rightHandAnchorDetached.localPosition = Vector3.zero;
					this.rightHandAnchorDetached.localRotation = Quaternion.identity;
					this.rightHandOnControllerAnchor.localPosition = Vector3.zero;
					this.rightHandOnControllerAnchor.localRotation = Quaternion.identity;
				}
			}
			this.trackerAnchor.localPosition = pose.position;
			OVRPose ovrpose = OVRPose.identity;
			OVRPose ovrpose2 = OVRPose.identity;
			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
			{
				ovrpose = OVRManager.GetOpenVRControllerOffset(XRNode.LeftHand);
				ovrpose2 = OVRManager.GetOpenVRControllerOffset(XRNode.RightHand);
				OVRManager.SetOpenVRLocalPose(this.trackingSpace.InverseTransformPoint(this.leftControllerAnchor.position), this.trackingSpace.InverseTransformPoint(this.rightControllerAnchor.position), Quaternion.Inverse(this.trackingSpace.rotation) * this.leftControllerAnchor.rotation, Quaternion.Inverse(this.trackingSpace.rotation) * this.rightControllerAnchor.rotation);
			}
			this.rightControllerAnchor.localPosition = ovrpose2.position;
			this.rightControllerAnchor.localRotation = ovrpose2.orientation;
			this.leftControllerAnchor.localPosition = ovrpose.position;
			this.leftControllerAnchor.localRotation = ovrpose.orientation;
		}
		if (OVRManager.instance.LateLatching)
		{
			XRDisplaySubsystem currentDisplaySubsystem = OVRManager.GetCurrentDisplaySubsystem();
			if (currentDisplaySubsystem != null)
			{
				currentDisplaySubsystem.MarkTransformLateLatched(this.centerEyeAnchor.transform, XRDisplaySubsystem.LateLatchNode.Head);
				currentDisplaySubsystem.MarkTransformLateLatched(this.leftHandAnchor, XRDisplaySubsystem.LateLatchNode.LeftHand);
				currentDisplaySubsystem.MarkTransformLateLatched(this.rightHandAnchor, XRDisplaySubsystem.LateLatchNode.RightHand);
			}
		}
		this.RaiseUpdatedAnchorsEvent();
		this.CheckForTrackingSpaceChangesAndRaiseEvent();
	}

	protected virtual void OnBeforeRenderCallback()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			bool lateControllerUpdate = OVRManager.instance.LateControllerUpdate;
			this.UpdateAnchors(true, lateControllerUpdate);
		}
	}

	protected virtual void CheckForTrackingSpaceChangesAndRaiseEvent()
	{
		if (this.trackingSpace == null)
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = this.trackingSpace.localToWorldMatrix;
		bool flag = this.TrackingSpaceChanged != null && !this._previousTrackingSpaceTransform.Equals(localToWorldMatrix);
		this._previousTrackingSpaceTransform = localToWorldMatrix;
		if (flag)
		{
			this.TrackingSpaceChanged(this.trackingSpace);
		}
	}

	protected virtual void RaiseUpdatedAnchorsEvent()
	{
		if (this.UpdatedAnchors != null)
		{
			this.UpdatedAnchors(this);
		}
	}

	public virtual void EnsureGameObjectIntegrity()
	{
		bool flag = OVRManager.instance != null && OVRManager.instance.monoscopic;
		if (this.trackingSpace == null)
		{
			this.trackingSpace = this.ConfigureAnchor(null, this.trackingSpaceName);
			this._previousTrackingSpaceTransform = this.trackingSpace.localToWorldMatrix;
		}
		if (this.leftEyeAnchor == null)
		{
			this.leftEyeAnchor = this.ConfigureAnchor(this.trackingSpace, this.leftEyeAnchorName);
		}
		if (this.centerEyeAnchor == null)
		{
			this.centerEyeAnchor = this.ConfigureAnchor(this.trackingSpace, this.centerEyeAnchorName);
		}
		if (this.rightEyeAnchor == null)
		{
			this.rightEyeAnchor = this.ConfigureAnchor(this.trackingSpace, this.rightEyeAnchorName);
		}
		if (this.leftHandAnchor == null)
		{
			this.leftHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.leftHandAnchorName);
		}
		if (this.rightHandAnchor == null)
		{
			this.rightHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.rightHandAnchorName);
		}
		if (this.leftHandAnchorDetached == null)
		{
			this.leftHandAnchorDetached = this.ConfigureAnchor(this.trackingSpace, this.leftHandAnchorDetachedName);
		}
		if (this.rightHandAnchorDetached == null)
		{
			this.rightHandAnchorDetached = this.ConfigureAnchor(this.trackingSpace, this.rightHandAnchorDetachedName);
		}
		if (this.leftControllerInHandAnchor == null)
		{
			this.leftControllerInHandAnchor = this.ConfigureAnchor(this.leftHandAnchor, this.leftControllerInHandAnchorName);
		}
		if (this.leftHandOnControllerAnchor == null)
		{
			this.leftHandOnControllerAnchor = this.ConfigureAnchor(this.leftControllerInHandAnchor, this.leftHandOnControllerAnchorName);
		}
		if (this.rightControllerInHandAnchor == null)
		{
			this.rightControllerInHandAnchor = this.ConfigureAnchor(this.rightHandAnchor, this.rightControllerInHandAnchorName);
		}
		if (this.rightHandOnControllerAnchor == null)
		{
			this.rightHandOnControllerAnchor = this.ConfigureAnchor(this.rightControllerInHandAnchor, this.rightHandOnControllerAnchorName);
		}
		if (this.trackerAnchor == null)
		{
			this.trackerAnchor = this.ConfigureAnchor(this.trackingSpace, this.trackerAnchorName);
		}
		if (this.leftControllerAnchor == null)
		{
			this.leftControllerAnchor = this.ConfigureAnchor(this.leftHandAnchor, this.leftControllerAnchorName);
		}
		if (this.rightControllerAnchor == null)
		{
			this.rightControllerAnchor = this.ConfigureAnchor(this.rightHandAnchor, this.rightControllerAnchorName);
		}
		if (this._centerEyeCamera == null || this._leftEyeCamera == null || this._rightEyeCamera == null)
		{
			this._centerEyeCamera = this.centerEyeAnchor.GetComponent<Camera>();
			this._leftEyeCamera = this.leftEyeAnchor.GetComponent<Camera>();
			this._rightEyeCamera = this.rightEyeAnchor.GetComponent<Camera>();
			if (this._centerEyeCamera == null)
			{
				this._centerEyeCamera = this.centerEyeAnchor.gameObject.AddComponent<Camera>();
				this._centerEyeCamera.tag = "MainCamera";
			}
			if (this._leftEyeCamera == null)
			{
				this._leftEyeCamera = this.leftEyeAnchor.gameObject.AddComponent<Camera>();
				this._leftEyeCamera.tag = "MainCamera";
			}
			if (this._rightEyeCamera == null)
			{
				this._rightEyeCamera = this.rightEyeAnchor.gameObject.AddComponent<Camera>();
				this._rightEyeCamera.tag = "MainCamera";
			}
			if (GraphicsSettings.currentRenderPipeline == null)
			{
				this._centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
				this._leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
				this._rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
			}
		}
		if (GraphicsSettings.currentRenderPipeline == null)
		{
			if (flag && !OVRPlugin.EyeTextureArrayEnabled)
			{
				if (this._centerEyeCamera.stereoTargetEye != StereoTargetEyeMask.Left)
				{
					this._centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
				}
			}
			else if (this._centerEyeCamera.stereoTargetEye != StereoTargetEyeMask.Both)
			{
				this._centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
			}
		}
		if (this.disableEyeAnchorCameras)
		{
			this._centerEyeCamera.enabled = false;
			this._leftEyeCamera.enabled = false;
			this._rightEyeCamera.enabled = false;
			return;
		}
		if (this._centerEyeCamera.enabled == this.usePerEyeCameras || this._leftEyeCamera.enabled == !this.usePerEyeCameras || this._rightEyeCamera.enabled == (!this.usePerEyeCameras || (flag && !OVRPlugin.EyeTextureArrayEnabled)))
		{
			this._skipUpdate = true;
		}
		this._centerEyeCamera.enabled = !this.usePerEyeCameras;
		this._leftEyeCamera.enabled = this.usePerEyeCameras;
		this._rightEyeCamera.enabled = (this.usePerEyeCameras && (!flag || OVRPlugin.EyeTextureArrayEnabled));
	}

	protected virtual Transform ConfigureAnchor(Transform root, string name)
	{
		Transform transform = (root != null) ? root.Find(name) : null;
		if (transform == null)
		{
			transform = base.transform.Find(name);
		}
		if (transform == null)
		{
			transform = new GameObject(name).transform;
		}
		transform.name = name;
		transform.parent = ((root != null) ? root : base.transform);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	public virtual Matrix4x4 ComputeTrackReferenceMatrix()
	{
		if (this.centerEyeAnchor == null)
		{
			Debug.LogError("centerEyeAnchor is required");
			return Matrix4x4.identity;
		}
		OVRPose identity = OVRPose.identity;
		Vector3 position;
		if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.Position, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out position))
		{
			identity.position = position;
		}
		Quaternion orientation;
		if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.Head, NodeStatePropertyType.Orientation, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out orientation))
		{
			identity.orientation = orientation;
		}
		OVRPose ovrpose = identity.Inverse();
		Matrix4x4 rhs = Matrix4x4.TRS(ovrpose.position, ovrpose.orientation, Vector3.one);
		return this.centerEyeAnchor.localToWorldMatrix * rhs;
	}

	protected void CheckForAnchorsInParent()
	{
		Transform parent = base.transform.parent;
		while (parent)
		{
			this.<CheckForAnchorsInParent>g__Check|105_0<OVRSpatialAnchor>(parent);
			this.<CheckForAnchorsInParent>g__Check|105_0<OVRSceneAnchor>(parent);
			parent = parent.parent;
		}
	}

	[CompilerGenerated]
	private void <CheckForAnchorsInParent>g__Check|105_0<T>(Transform node) where T : MonoBehaviour
	{
		T component = node.GetComponent<T>();
		if (component && component.enabled)
		{
			component.enabled = false;
			Debug.LogError(string.Concat(new string[]
			{
				"The ",
				typeof(T).Name,
				" '",
				component.name,
				"' is a parent of the OVRCameraRig '",
				base.name,
				"', which is not allowed. An ",
				typeof(T).Name,
				" may not be the parent of an OVRCameraRig because the OVRCameraRig defines the tracking space for the anchor, and its transform is relative to the OVRCameraRig."
			}));
		}
	}

	public bool usePerEyeCameras;

	public bool useFixedUpdateForTracking;

	public bool disableEyeAnchorCameras;

	protected bool _skipUpdate;

	protected readonly string trackingSpaceName = "TrackingSpace";

	protected readonly string trackerAnchorName = "TrackerAnchor";

	protected readonly string leftEyeAnchorName = "LeftEyeAnchor";

	protected readonly string centerEyeAnchorName = "CenterEyeAnchor";

	protected readonly string rightEyeAnchorName = "RightEyeAnchor";

	protected readonly string leftHandAnchorName = "LeftHandAnchor";

	protected readonly string rightHandAnchorName = "RightHandAnchor";

	protected readonly string leftControllerAnchorName = "LeftControllerAnchor";

	protected readonly string rightControllerAnchorName = "RightControllerAnchor";

	protected readonly string leftHandAnchorDetachedName = "LeftHandAnchorDetached";

	protected readonly string rightHandAnchorDetachedName = "RightHandAnchorDetached";

	protected readonly string leftControllerInHandAnchorName = "LeftControllerInHandAnchor";

	protected readonly string leftHandOnControllerAnchorName = "LeftHandOnControllerAnchor";

	protected readonly string rightControllerInHandAnchorName = "RightControllerInHandAnchor";

	protected readonly string rightHandOnControllerAnchorName = "RightHandOnControllerAnchor";

	protected Camera _centerEyeCamera;

	protected Camera _leftEyeCamera;

	protected Camera _rightEyeCamera;

	protected Matrix4x4 _previousTrackingSpaceTransform;
}
