using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

public class GorillaIOBT : MonoBehaviour
{
	public OVRInput.Controller leftActiveController { get; private set; }

	public OVRInput.Controller rightActiveController { get; private set; }

	public bool IsHandTracking
	{
		get
		{
			return this.leftActiveController == OVRInput.Controller.LHand || this.rightActiveController == OVRInput.Controller.RHand;
		}
	}

	public HandTrackingFingerCurl leftHandCurl { get; private set; }

	public HandTrackingFingerCurl rightHandCurl { get; private set; }

	public Transform trackingSpace { get; private set; }

	public Transform centerEyeAnchor { get; private set; }

	public Transform leftHandAnchor { get; private set; }

	public Transform rightHandAnchor { get; private set; }

	public Transform leftControllerAnchor { get; private set; }

	public Transform rightControllerAnchor { get; private set; }

	public event Action<GorillaIOBT> UpdatedAnchors;

	public event Action<Transform> TrackingSpaceChanged;

	protected virtual void Awake()
	{
		this._skipUpdate = true;
		this.EnsureGameObjectIntegrity();
		this.upperBodySkeleton = base.GetComponent<OVRSkeleton>();
	}

	protected virtual void Start()
	{
		this.UpdateAnchors();
		Application.onBeforeRender += this.OnBeforeRenderCallback;
	}

	protected virtual void Update()
	{
		this._skipUpdate = false;
		this.UpdateAnchors();
	}

	protected virtual void OnDestroy()
	{
		Application.onBeforeRender -= this.OnBeforeRenderCallback;
	}

	protected virtual void UpdateAnchors()
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
			return;
		}
		bool monoscopic = OVRManager.instance.monoscopic;
		OVRNodeStateProperties.IsHmdPresent();
		OVRManager.tracker.GetPose(0);
		Quaternion.Euler(-OVRManager.instance.headPoseRelativeOffsetRotation.x, -OVRManager.instance.headPoseRelativeOffsetRotation.y, OVRManager.instance.headPoseRelativeOffsetRotation.z);
		OVRInput.Controller leftActiveController = this.leftActiveController;
		OVRInput.Controller rightActiveController = this.rightActiveController;
		this.leftActiveController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.LeftHanded);
		this.rightActiveController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);
		if (this.leftActiveController == OVRInput.Controller.None)
		{
			if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LHand))
			{
				this.leftActiveController = OVRInput.Controller.LHand;
			}
			else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch))
			{
				this.leftActiveController = OVRInput.Controller.LTouch;
			}
		}
		if (this.rightActiveController == OVRInput.Controller.None)
		{
			if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RHand))
			{
				this.rightActiveController = OVRInput.Controller.RHand;
			}
			else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch))
			{
				this.rightActiveController = OVRInput.Controller.RTouch;
			}
		}
		if (leftActiveController == OVRInput.Controller.None && this.leftActiveController != OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingGainedClip);
		}
		else if (leftActiveController != OVRInput.Controller.None && this.leftActiveController == OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingLostClip);
		}
		if (rightActiveController == OVRInput.Controller.None && this.rightActiveController != OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingGainedClip);
		}
		else if (rightActiveController != OVRInput.Controller.None && this.rightActiveController == OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingLostClip);
		}
		if (this.leftActiveController == OVRInput.Controller.LHand)
		{
			this.leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(this.leftActiveController);
			this.leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(this.leftActiveController);
			this.leftHandAnchor.localRotation = this.leftHandAnchor.localRotation * Quaternion.Euler(0f, 90f, -90f);
		}
		if (this.rightActiveController == OVRInput.Controller.RHand)
		{
			this.rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(this.rightActiveController);
			this.rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(this.rightActiveController);
			this.rightHandAnchor.localRotation = this.rightHandAnchor.localRotation * Quaternion.Euler(0f, -90f, 90f);
		}
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
		GTPlayer.Instance.SetHandOffsets(true, new Vector3(0.03f, -0.16f, 0f), Quaternion.Euler(89f, 6f, 11f));
		GTPlayer.Instance.SetHandOffsets(false, new Vector3(-0.01f, -0.16f, 0f), Quaternion.Euler(89f, 6f, 11f));
		this.RaiseUpdatedAnchorsEvent();
		this.CheckForTrackingSpaceChangesAndRaiseEvent();
	}

	protected virtual void OnBeforeRenderCallback()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && OVRManager.instance.LateControllerUpdate)
		{
			this.UpdateAnchors();
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
		if (OVRManager.instance != null)
		{
			bool monoscopic = OVRManager.instance.monoscopic;
		}
		if (this.trackingSpace == null)
		{
			this.trackingSpace = this.ConfigureAnchor(null, this.trackingSpaceName);
			this._previousTrackingSpaceTransform = this.trackingSpace.localToWorldMatrix;
		}
		if (this.centerEyeAnchor == null)
		{
			this.centerEyeAnchor = this.ConfigureAnchor(this.trackingSpace, this.centerEyeAnchorName);
		}
		if (this.leftHandAnchor == null)
		{
			this.leftHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.leftHandAnchorName);
		}
		if (this.rightHandAnchor == null)
		{
			this.rightHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.rightHandAnchorName);
		}
		if (this.leftControllerAnchor == null)
		{
			this.leftControllerAnchor = this.ConfigureAnchor(this.leftHandAnchor, this.leftControllerAnchorName);
		}
		if (this.rightControllerAnchor == null)
		{
			this.rightControllerAnchor = this.ConfigureAnchor(this.rightHandAnchor, this.rightControllerAnchorName);
		}
		if (this.leftHandCurl == null)
		{
			Transform leftHandAnchor = this.leftHandAnchor;
			this.leftHandCurl = ((leftHandAnchor != null) ? leftHandAnchor.GetComponent<HandTrackingFingerCurl>() : null);
		}
		if (this.rightHandCurl == null)
		{
			Transform rightHandAnchor = this.rightHandAnchor;
			this.rightHandCurl = ((rightHandAnchor != null) ? rightHandAnchor.GetComponent<HandTrackingFingerCurl>() : null);
		}
	}

	protected Transform ConfigureAnchor(Transform root, string name)
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
			this.<CheckForAnchorsInParent>g__Check|71_0<OVRSpatialAnchor>(parent);
			this.<CheckForAnchorsInParent>g__Check|71_0<OVRSceneAnchor>(parent);
			parent = parent.parent;
		}
	}

	[CompilerGenerated]
	private void <CheckForAnchorsInParent>g__Check|71_0<T>(Transform node) where T : MonoBehaviour
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
				"' is a parent of the GorillaIOBT '",
				base.name,
				"', which is not allowed. An ",
				typeof(T).Name,
				" may not be the parent of an GorillaIOBT because the GorillaIOBT defines the tracking space for the anchor, and its transform is relative to the GorillaIOBT."
			}));
		}
	}

	private OVRSkeleton upperBodySkeleton;

	public AudioSource trackingChangedAudioSource;

	public AudioClip trackingGainedClip;

	public AudioClip trackingLostClip;

	protected bool _skipUpdate;

	protected readonly string trackingSpaceName = "TurnParent";

	protected readonly string centerEyeAnchorName = "Main Camera";

	protected readonly string leftHandAnchorName = "LeftHand Controller";

	protected readonly string rightHandAnchorName = "RightHand Controller";

	protected readonly string leftControllerAnchorName = "LeftControllerAnchor";

	protected readonly string rightControllerAnchorName = "RightControllerAnchor";

	protected Matrix4x4 _previousTrackingSpaceTransform;
}
