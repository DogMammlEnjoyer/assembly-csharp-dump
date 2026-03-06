using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-handtracking/")]
[Feature(Feature.Hands)]
public class OVRHand : MonoBehaviour, OVRInputModule.InputSource, OVRSkeleton.IOVRSkeletonDataProvider, OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider, OVRMesh.IOVRMeshDataProvider, OVRMeshRenderer.IOVRMeshRendererDataProvider
{
	private static OVRHandSkeletonVersion GlobalHandSkeletonVersion
	{
		get
		{
			return OVRRuntimeSettings.Instance.HandSkeletonVersion;
		}
	}

	public bool IsDataValid { get; private set; }

	public bool IsDataHighConfidence { get; private set; }

	public bool IsTracked { get; private set; }

	public bool IsSystemGestureInProgress { get; private set; }

	public bool IsPointerPoseValid { get; private set; }

	public Transform PointerPose
	{
		get
		{
			if (this._pointerPoseGO == null)
			{
				this.InitializePointerPose();
			}
			return this._pointerPoseGO.transform;
		}
	}

	public float HandScale { get; private set; }

	public OVRHand.TrackingConfidence HandConfidence { get; private set; }

	public bool IsDominantHand { get; private set; }

	private void InitializePointerPose()
	{
		this._pointerPoseGO = new GameObject(string.Format("{0} {1}", this.HandType, "PointerPose"));
		Object.DontDestroyOnLoad(this._pointerPoseGO);
		this._pointerPoseGO.hideFlags = HideFlags.HideAndDontSave;
		if (this._pointerPoseRoot != null)
		{
			this.PointerPose.SetParent(this._pointerPoseRoot, false);
		}
	}

	private void Awake()
	{
		if (this._pointerPoseGO == null)
		{
			this.InitializePointerPose();
		}
		if (this.RayHelper != null)
		{
			this.RayHelper.transform.SetParent(this.PointerPose, false);
		}
		this.GetHandState(OVRPlugin.Step.Render);
	}

	private void Update()
	{
		this.GetHandState(OVRPlugin.Step.Render);
		bool fingerIsPinching = this.GetFingerIsPinching(OVRHand.HandFinger.Index);
		this._wasReleased = (!fingerIsPinching && this._wasIndexPinching);
		this._wasIndexPinching = fingerIsPinching;
		if (this.RayHelper && !this.IsActive() && this.RayHelper.isActiveAndEnabled)
		{
			this.RayHelper.gameObject.SetActive(false);
		}
	}

	private void FixedUpdate()
	{
		if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
		{
			this.GetHandState(OVRPlugin.Step.Physics);
		}
		if (this.RayHelper != null)
		{
			this.RayHelper.gameObject.SetActive(this.IsDataValid);
		}
	}

	private void OnDestroy()
	{
		if (this._pointerPoseGO != null)
		{
			Object.Destroy(this._pointerPoseGO);
		}
	}

	private void GetHandState(OVRPlugin.Step step)
	{
		if (OVRPlugin.GetHandState(step, (OVRPlugin.Hand)this.HandType, ref this._handState))
		{
			this.IsTracked = ((this._handState.Status & OVRPlugin.HandStatus.HandTracked) > (OVRPlugin.HandStatus)0);
			this.IsSystemGestureInProgress = ((this._handState.Status & OVRPlugin.HandStatus.SystemGestureInProgress) > (OVRPlugin.HandStatus)0);
			this.IsPointerPoseValid = ((this._handState.Status & OVRPlugin.HandStatus.InputStateValid) > (OVRPlugin.HandStatus)0);
			this.IsDominantHand = ((this._handState.Status & OVRPlugin.HandStatus.DominantHand) > (OVRPlugin.HandStatus)0);
			this.PointerPose.localPosition = this._handState.PointerPose.Position.FromFlippedZVector3f();
			this.PointerPose.localRotation = this._handState.PointerPose.Orientation.FromFlippedZQuatf();
			this.HandScale = this._handState.HandScale;
			this.HandConfidence = (OVRHand.TrackingConfidence)this._handState.HandConfidence;
			this.IsDataValid = true;
			this.IsDataHighConfidence = (this.IsTracked && this.HandConfidence == OVRHand.TrackingConfidence.High);
			this._handTrackingStateValid = OVRPlugin.GetHandTrackingState(step, (OVRPlugin.Hand)this.HandType, ref this._handTrackingState);
			OVRInput.ControllerInHandState controllerIsInHandState = OVRInput.GetControllerIsInHandState((OVRInput.Hand)this.HandType);
			if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerInHand)
			{
				this.IsSystemGestureInProgress = false;
				this.IsPointerPoseValid = false;
			}
			switch (this.m_showState)
			{
			case OVRInput.InputDeviceShowState.ControllerInHandOrNoHand:
				if (controllerIsInHandState == OVRInput.ControllerInHandState.ControllerNotInHand)
				{
					this.IsDataValid = false;
				}
				break;
			case OVRInput.InputDeviceShowState.ControllerInHand:
				if (controllerIsInHandState != OVRInput.ControllerInHandState.ControllerInHand)
				{
					this.IsDataValid = false;
				}
				break;
			case OVRInput.InputDeviceShowState.ControllerNotInHand:
				if (controllerIsInHandState != OVRInput.ControllerInHandState.ControllerNotInHand)
				{
					this.IsDataValid = false;
				}
				break;
			case OVRInput.InputDeviceShowState.NoHand:
				if (controllerIsInHandState != OVRInput.ControllerInHandState.NoHand)
				{
					this.IsDataValid = false;
				}
				break;
			}
			if (OVRPlugin.HandSkeletonVersion != OVRHand.GlobalHandSkeletonVersion)
			{
				this.IsDataValid = false;
				return;
			}
		}
		else
		{
			this.IsTracked = false;
			this.IsSystemGestureInProgress = false;
			this.IsPointerPoseValid = false;
			this.PointerPose.localPosition = Vector3.zero;
			this.PointerPose.localRotation = Quaternion.identity;
			this.HandScale = 1f;
			this.HandConfidence = OVRHand.TrackingConfidence.Low;
			this.IsDataValid = false;
			this.IsDataHighConfidence = false;
			this._handTrackingStateValid = false;
		}
	}

	public bool GetFingerIsPinching(OVRHand.HandFinger finger)
	{
		return this.IsDataValid && (this._handState.Pinches & (OVRPlugin.HandFingerPinch)(1 << (int)finger)) > (OVRPlugin.HandFingerPinch)0;
	}

	public float GetFingerPinchStrength(OVRHand.HandFinger finger)
	{
		if (this.IsDataValid && this._handState.PinchStrength != null && this._handState.PinchStrength.Length == 5)
		{
			return this._handState.PinchStrength[(int)finger];
		}
		return 0f;
	}

	public OVRHand.TrackingConfidence GetFingerConfidence(OVRHand.HandFinger finger)
	{
		if (this.IsDataValid && this._handState.FingerConfidences != null && this._handState.FingerConfidences.Length == 5)
		{
			return (OVRHand.TrackingConfidence)this._handState.FingerConfidences[(int)finger];
		}
		return OVRHand.TrackingConfidence.Low;
	}

	OVRSkeleton.SkeletonType OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonType()
	{
		switch (this.HandType)
		{
		case OVRHand.Hand.HandLeft:
		{
			OVRHandSkeletonVersion globalHandSkeletonVersion = OVRHand.GlobalHandSkeletonVersion;
			OVRSkeleton.SkeletonType result;
			if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OVR)
			{
				if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OpenXR)
				{
					result = OVRSkeleton.SkeletonType.None;
				}
				else
				{
					result = OVRSkeleton.SkeletonType.XRHandLeft;
				}
			}
			else
			{
				result = OVRSkeleton.SkeletonType.HandLeft;
			}
			return result;
		}
		case OVRHand.Hand.HandRight:
		{
			OVRHandSkeletonVersion globalHandSkeletonVersion = OVRHand.GlobalHandSkeletonVersion;
			OVRSkeleton.SkeletonType result;
			if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OVR)
			{
				if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OpenXR)
				{
					result = OVRSkeleton.SkeletonType.None;
				}
				else
				{
					result = OVRSkeleton.SkeletonType.XRHandRight;
				}
			}
			else
			{
				result = OVRSkeleton.SkeletonType.HandRight;
			}
			return result;
		}
		}
		return OVRSkeleton.SkeletonType.None;
	}

	OVRSkeleton.SkeletonPoseData OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonPoseData()
	{
		OVRSkeleton.SkeletonPoseData result = default(OVRSkeleton.SkeletonPoseData);
		result.IsDataValid = this.IsDataValid;
		if (this.IsDataValid)
		{
			result.RootPose = this._handState.RootPose;
			result.RootScale = this._handState.HandScale;
			result.BoneRotations = this._handState.BoneRotations;
			result.BoneTranslations = this._handState.BonePositions;
			result.IsDataHighConfidence = (this.IsTracked && this.HandConfidence == OVRHand.TrackingConfidence.High);
		}
		return result;
	}

	OVRSkeletonRenderer.SkeletonRendererData OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider.GetSkeletonRendererData()
	{
		OVRSkeletonRenderer.SkeletonRendererData result = default(OVRSkeletonRenderer.SkeletonRendererData);
		result.IsDataValid = this.IsDataValid;
		if (this.IsDataValid)
		{
			result.RootScale = this._handState.HandScale;
			result.IsDataHighConfidence = (this.IsTracked && this.HandConfidence == OVRHand.TrackingConfidence.High);
			result.ShouldUseSystemGestureMaterial = this.IsSystemGestureInProgress;
		}
		return result;
	}

	public OVRHand.MicrogestureType GetMicrogestureType()
	{
		OVRPlugin.SendMicrogestureHint();
		if (!this._handTrackingStateValid)
		{
			return OVRHand.MicrogestureType.Invalid;
		}
		int microgesture = (int)this._handTrackingState.Microgesture;
		if (microgesture < 0 || microgesture > 5)
		{
			return OVRHand.MicrogestureType.Invalid;
		}
		return (OVRHand.MicrogestureType)microgesture;
	}

	OVRMesh.MeshType OVRMesh.IOVRMeshDataProvider.GetMeshType()
	{
		switch (this.HandType)
		{
		case OVRHand.Hand.None:
			return OVRMesh.MeshType.None;
		case OVRHand.Hand.HandLeft:
		{
			OVRHandSkeletonVersion globalHandSkeletonVersion = OVRHand.GlobalHandSkeletonVersion;
			OVRMesh.MeshType result;
			if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OVR)
			{
				if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OpenXR)
				{
					result = OVRMesh.MeshType.None;
				}
				else
				{
					result = OVRMesh.MeshType.XRHandLeft;
				}
			}
			else
			{
				result = OVRMesh.MeshType.HandLeft;
			}
			return result;
		}
		case OVRHand.Hand.HandRight:
		{
			OVRHandSkeletonVersion globalHandSkeletonVersion = OVRHand.GlobalHandSkeletonVersion;
			OVRMesh.MeshType result;
			if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OVR)
			{
				if (globalHandSkeletonVersion != OVRHandSkeletonVersion.OpenXR)
				{
					result = OVRMesh.MeshType.None;
				}
				else
				{
					result = OVRMesh.MeshType.XRHandRight;
				}
			}
			else
			{
				result = OVRMesh.MeshType.HandRight;
			}
			return result;
		}
		default:
			return OVRMesh.MeshType.None;
		}
	}

	OVRMeshRenderer.MeshRendererData OVRMeshRenderer.IOVRMeshRendererDataProvider.GetMeshRendererData()
	{
		OVRMeshRenderer.MeshRendererData result = default(OVRMeshRenderer.MeshRendererData);
		result.IsDataValid = this.IsDataValid;
		if (this.IsDataValid)
		{
			result.IsDataHighConfidence = (this.IsTracked && this.HandConfidence == OVRHand.TrackingConfidence.High);
			result.ShouldUseSystemGestureMaterial = this.IsSystemGestureInProgress;
		}
		return result;
	}

	public void OnEnable()
	{
		OVRInputModule.TrackInputSource(this);
		SceneManager.activeSceneChanged += this.OnSceneChanged;
		if (this.RayHelper && this.ShouldShowHandUIRay())
		{
			this.RayHelper.gameObject.SetActive(true);
		}
	}

	public void OnDisable()
	{
		OVRInputModule.UntrackInputSource(this);
		SceneManager.activeSceneChanged -= this.OnSceneChanged;
		if (this.RayHelper)
		{
			this.RayHelper.gameObject.SetActive(false);
		}
	}

	private void OnSceneChanged(Scene unloading, Scene loading)
	{
		OVRInputModule.TrackInputSource(this);
	}

	public void OnValidate()
	{
		OVRSkeleton component = base.GetComponent<OVRSkeleton>();
		if (component != null && component.GetSkeletonType() != this.HandType.AsSkeletonType(OVRHand.GlobalHandSkeletonVersion))
		{
			component.SetSkeletonType(this.HandType.AsSkeletonType(OVRHand.GlobalHandSkeletonVersion));
		}
		OVRMesh component2 = base.GetComponent<OVRMesh>();
		if (component2 != null && component2.GetMeshType() != this.HandType.AsMeshType(OVRHand.GlobalHandSkeletonVersion))
		{
			component2.SetMeshType(this.HandType.AsMeshType(OVRHand.GlobalHandSkeletonVersion));
		}
	}

	public bool IsPressed()
	{
		return this.GetFingerIsPinching(OVRHand.HandFinger.Index);
	}

	public bool IsReleased()
	{
		return this._wasReleased;
	}

	public Transform GetPointerRayTransform()
	{
		this.PointerPose.name = base.name;
		return this.PointerPose;
	}

	private bool ShouldShowHandUIRay()
	{
		return this.m_showState != OVRInput.InputDeviceShowState.ControllerInHand || OVRPlugin.AreControllerDrivenHandPosesNatural();
	}

	public bool IsValid()
	{
		return this != null;
	}

	public bool IsActive()
	{
		return this.ShouldShowHandUIRay() && this.IsDataValid;
	}

	public OVRPlugin.Hand GetHand()
	{
		return (OVRPlugin.Hand)this.HandType;
	}

	public void UpdatePointerRay(OVRInputRayData rayData)
	{
		if (this.RayHelper)
		{
			rayData.ActivationStrength = this.GetFingerPinchStrength(OVRHand.HandFinger.Index);
			this.RayHelper.UpdatePointerRay(rayData);
		}
	}

	bool OVRSkeleton.IOVRSkeletonDataProvider.get_enabled()
	{
		return base.enabled;
	}

	[SerializeField]
	internal OVRHand.Hand HandType = OVRHand.Hand.None;

	[SerializeField]
	private Transform _pointerPoseRoot;

	public OVRInput.InputDeviceShowState m_showState = OVRInput.InputDeviceShowState.ControllerNotInHand;

	public OVRRayHelper RayHelper;

	private GameObject _pointerPoseGO;

	private OVRPlugin.HandState _handState;

	private bool _wasIndexPinching;

	private bool _wasReleased;

	private OVRPlugin.HandTrackingState _handTrackingState;

	private bool _handTrackingStateValid;

	public enum Hand
	{
		None = -1,
		HandLeft,
		HandRight
	}

	public enum HandFinger
	{
		Thumb,
		Index,
		Middle,
		Ring,
		Pinky,
		Max
	}

	public enum TrackingConfidence
	{
		Low,
		High = 1065353216
	}

	public enum MicrogestureType
	{
		NoGesture,
		SwipeLeft,
		SwipeRight,
		SwipeForward,
		SwipeBackward,
		ThumbTap,
		Invalid = -1
	}
}
