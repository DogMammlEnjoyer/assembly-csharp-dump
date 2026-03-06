using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.SmartTweenableVariables;

namespace UnityEngine.XR.Interaction.Toolkit.UI.BodyUI
{
	[AddComponentMenu("XR/Hand Menu", 22)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.UI.BodyUI.HandMenu.html")]
	public class HandMenu : MonoBehaviour
	{
		public GameObject handMenuUIGameObject
		{
			get
			{
				return this.m_HandMenuUIGameObject;
			}
			set
			{
				this.m_HandMenuUIGameObject = value;
			}
		}

		public HandMenu.MenuHandedness menuHandedness
		{
			get
			{
				return this.m_MenuHandedness;
			}
			set
			{
				this.m_MenuHandedness = value;
			}
		}

		public HandMenu.UpDirection handMenuUpDirection
		{
			get
			{
				return this.m_HandMenuUpDirection;
			}
			set
			{
				this.m_HandMenuUpDirection = value;
			}
		}

		public Transform leftPalmAnchor
		{
			get
			{
				return this.m_LeftPalmAnchor;
			}
			set
			{
				this.m_LeftPalmAnchor = value;
			}
		}

		public Transform rightPalmAnchor
		{
			get
			{
				return this.m_RightPalmAnchor;
			}
			set
			{
				this.m_RightPalmAnchor = value;
			}
		}

		public float minFollowDistance
		{
			get
			{
				return this.m_MinFollowDistance;
			}
			set
			{
				this.m_MinFollowDistance = value;
				this.m_HandAnchorSmartFollow.minDistanceAllowed = value;
			}
		}

		public float maxFollowDistance
		{
			get
			{
				return this.m_MaxFollowDistance;
			}
			set
			{
				this.m_MaxFollowDistance = value;
				this.m_HandAnchorSmartFollow.maxDistanceAllowed = value;
			}
		}

		public float minToMaxDelaySeconds
		{
			get
			{
				return this.m_MinToMaxDelaySeconds;
			}
			set
			{
				this.m_MinToMaxDelaySeconds = value;
				this.m_HandAnchorSmartFollow.minToMaxDelaySeconds = value;
			}
		}

		public bool hideMenuWhenGazeDiverges
		{
			get
			{
				return this.m_HideMenuWhenGazeDiverges;
			}
			set
			{
				this.m_HideMenuWhenGazeDiverges = value;
			}
		}

		public float menuVisibleGazeDivergenceThreshold
		{
			get
			{
				return this.m_MenuVisibleGazeAngleDivergenceThreshold;
			}
			set
			{
				this.m_MenuVisibleGazeAngleDivergenceThreshold = value;
				this.m_MenuVisibilityDotThreshold = HandMenu.AngleToDot(value);
			}
		}

		public bool animateMenuHideAndRevel
		{
			get
			{
				return this.m_AnimateMenuHideAndReveal;
			}
			set
			{
				this.m_AnimateMenuHideAndReveal = value;
			}
		}

		public float revealHideAnimationDuration
		{
			get
			{
				return this.m_RevealHideAnimationDuration;
			}
			set
			{
				this.m_RevealHideAnimationDuration = value;
			}
		}

		public bool hideMenuOnSelect
		{
			get
			{
				return this.m_HideMenuOnSelect;
			}
			set
			{
				this.m_HideMenuOnSelect = value;
			}
		}

		public XRInteractionManager interactionManager
		{
			get
			{
				return this.m_InteractionManager;
			}
			set
			{
				this.m_InteractionManager = value;
			}
		}

		protected void Awake()
		{
			this.m_HandAnchorSmartFollow.minDistanceAllowed = this.m_MinFollowDistance;
			this.m_HandAnchorSmartFollow.maxDistanceAllowed = this.m_MaxFollowDistance;
			this.m_HandAnchorSmartFollow.minToMaxDelaySeconds = this.m_MinToMaxDelaySeconds;
			this.m_RightOffsetRoot = new GameObject("Right Offset Root").transform;
			this.m_RightOffsetRoot.transform.SetParent(this.m_RightPalmAnchor);
			this.m_LeftOffsetRoot = new GameObject("Left Offset Root").transform;
			this.m_LeftOffsetRoot.transform.SetParent(this.m_LeftPalmAnchor);
		}

		protected void OnEnable()
		{
			if (this.m_LeftPalmAnchor == null || this.m_RightPalmAnchor == null)
			{
				Debug.LogError(string.Format("Missing palm anchor transform reference. Disabling {0} component.", this), this);
				base.enabled = false;
				return;
			}
			if (this.m_HandMenuUIGameObject == null)
			{
				Debug.LogError(string.Format("Missing Hand Menu UI GameObject reference. Disabling {0} component.", this), this);
				base.enabled = false;
				return;
			}
			if (this.m_ControllerFollowPreset == null || this.m_HandTrackingFollowPreset == null)
			{
				Debug.LogError(string.Format("Missing Follow Preset reference. Disabling {0} component.", this), this);
				base.enabled = false;
				return;
			}
			this.m_HandAnchorSmartFollow.Value = this.m_HandMenuUIGameObject.transform.position;
			this.m_BindingsGroup.AddBinding(this.m_HandAnchorSmartFollow.Subscribe(delegate(float3 newPosition)
			{
				this.m_HandMenuUIGameObject.transform.position = newPosition;
			}));
			this.m_RotTweenFollow.Value = this.m_HandMenuUIGameObject.transform.rotation;
			this.m_BindingsGroup.AddBinding(this.m_RotTweenFollow.Subscribe(delegate(Quaternion newRot)
			{
				this.m_HandMenuUIGameObject.transform.rotation = newRot;
			}));
			this.m_InitialMenuLocalScale = this.m_HandMenuUIGameObject.transform.localScale;
			this.m_MenuScaleTweenable.Value = this.m_InitialMenuLocalScale;
			this.m_BindingsGroup.AddBinding(this.m_MenuScaleTweenable.Subscribe(delegate(float3 value)
			{
				this.m_HandMenuUIGameObject.transform.localScale = value;
			}));
			this.m_BindingsGroup.AddBinding(XRInputModalityManager.currentInputMode.SubscribeAndUpdate(new Action<XRInputModalityManager.InputMode>(this.OnInputModeChanged)));
			this.m_MenuVisibleBindableVariable.Value = false;
			this.m_BindingsGroup.AddBinding(this.m_MenuVisibleBindableVariable.SubscribeAndUpdate(delegate(bool value)
			{
				if (value)
				{
					this.ShowMenu();
					return;
				}
				this.HideMenu();
			}));
		}

		protected void OnDisable()
		{
			if (this.m_ShowCoroutine != null)
			{
				base.StopCoroutine(this.m_ShowCoroutine);
				this.m_ShowCoroutine = null;
			}
			if (this.m_HideCoroutine != null)
			{
				base.StopCoroutine(this.m_HideCoroutine);
				this.m_HideCoroutine = null;
			}
			this.m_BindingsGroup.Clear();
			this.m_HandMenuUIGameObject.transform.localScale = this.m_InitialMenuLocalScale;
			this.m_HandMenuUIGameObject.SetActive(true);
			this.OnMenuVisible();
		}

		protected void OnDestroy()
		{
			this.m_HandAnchorSmartFollow.Dispose();
		}

		protected void OnValidate()
		{
			this.m_HandAnchorSmartFollow.minDistanceAllowed = this.m_MinFollowDistance;
			this.m_HandAnchorSmartFollow.maxDistanceAllowed = this.m_MaxFollowDistance;
			this.m_HandAnchorSmartFollow.minToMaxDelaySeconds = this.m_MinToMaxDelaySeconds;
			this.m_MenuVisibilityDotThreshold = HandMenu.AngleToDot(this.m_MenuVisibleGazeAngleDivergenceThreshold);
		}

		private void OnInputModeChanged(XRInputModalityManager.InputMode newInputMode)
		{
			this.m_CurrentInputMode = newInputMode;
			FollowPreset currentPreset = this.GetCurrentPreset();
			if (currentPreset == null)
			{
				return;
			}
			currentPreset.ApplyPreset(this.m_LeftOffsetRoot, this.m_RightOffsetRoot);
		}

		private FollowPreset GetCurrentPreset()
		{
			if (this.m_CurrentInputMode == XRInputModalityManager.InputMode.MotionController)
			{
				return this.m_ControllerFollowPreset.Value;
			}
			return this.m_HandTrackingFollowPreset.Value;
		}

		private void ShowMenu()
		{
			if (this.m_HideCoroutine != null)
			{
				base.StopCoroutine(this.m_HideCoroutine);
				this.m_HideCoroutine = null;
			}
			this.m_HandMenuUIGameObject.SetActive(true);
			if (this.m_AnimateMenuHideAndReveal && this.m_ShowCoroutine == null)
			{
				this.m_ShowCoroutine = base.StartCoroutine(this.m_MenuScaleTweenable.PlaySequence(this.m_MenuScaleTweenable.Value, this.m_InitialMenuLocalScale, this.m_RevealHideAnimationDuration, new Action(this.OnMenuVisible)));
				return;
			}
			this.OnMenuVisible();
		}

		private void OnMenuVisible()
		{
			this.m_ShowCoroutine = null;
			this.m_WasMenuHiddenLastFrame = false;
		}

		private void HideMenu()
		{
			if (this.m_ShowCoroutine != null)
			{
				base.StopCoroutine(this.m_ShowCoroutine);
				this.m_ShowCoroutine = null;
			}
			if (this.m_AnimateMenuHideAndReveal && this.m_HideCoroutine == null)
			{
				this.m_HideCoroutine = base.StartCoroutine(this.m_MenuScaleTweenable.PlaySequence(this.m_MenuScaleTweenable.Value, Vector3.zero, this.m_RevealHideAnimationDuration, new Action(this.OnMenuHidden)));
				return;
			}
			this.OnMenuHidden();
		}

		private void OnMenuHidden()
		{
			this.m_HandMenuUIGameObject.SetActive(false);
			this.m_WasMenuHiddenLastFrame = true;
			this.m_HideCoroutine = null;
		}

		protected void LateUpdate()
		{
			if (this.m_CurrentInputMode == XRInputModalityManager.InputMode.None)
			{
				this.m_MenuVisibleBindableVariable.Value = false;
				return;
			}
			bool flag = false;
			FollowPreset currentPreset = this.GetCurrentPreset();
			HandMenu.MenuHandedness menuHandedness;
			Transform lastValidCameraTransform;
			Transform lastValidPalmAnchor;
			Transform lastValidPalmAnchorOffset;
			if (this.TryGetTrackedAnchors(this.m_MenuHandedness, currentPreset, out menuHandedness, out lastValidCameraTransform, out lastValidPalmAnchor, out lastValidPalmAnchorOffset))
			{
				this.m_LastValidCameraTransform = lastValidCameraTransform;
				this.m_LastValidPalmAnchor = lastValidPalmAnchor;
				this.m_LastValidPalmAnchorOffset = lastValidPalmAnchorOffset;
				this.m_LastValidTrackingTime = Time.unscaledTime;
				flag = true;
			}
			if (!flag)
			{
				if (Time.unscaledTime - this.m_LastValidTrackingTime > currentPreset.hideDelaySeconds)
				{
					this.m_MenuVisibleBindableVariable.Value = false;
				}
				if (this.m_LastValidCameraTransform == null || this.m_LastValidPalmAnchor == null || this.m_LastValidPalmAnchorOffset == null)
				{
					return;
				}
			}
			Vector3 normalized = (this.m_LastValidPalmAnchorOffset.position - this.m_LastValidCameraTransform.position).normalized;
			if (flag)
			{
				if (this.m_HideMenuWhenGazeDiverges)
				{
					Vector3 forward = this.m_LastValidCameraTransform.forward;
					flag = (Vector3.Dot(normalized, forward) > this.m_MenuVisibilityDotThreshold);
				}
				this.m_MenuVisibleBindableVariable.Value = flag;
			}
			if (!this.m_HandMenuUIGameObject.activeSelf)
			{
				return;
			}
			Pose worldPose = this.m_LastValidPalmAnchorOffset.GetWorldPose();
			Vector3 position = worldPose.position;
			Quaternion rotation = worldPose.rotation;
			if (menuHandedness == HandMenu.MenuHandedness.Left || menuHandedness == HandMenu.MenuHandedness.Right)
			{
				Vector3 referenceAxisForTrackingAnchor = currentPreset.GetReferenceAxisForTrackingAnchor(this.m_LastValidPalmAnchor, menuHandedness == HandMenu.MenuHandedness.Right);
				Vector3 rhs = -normalized;
				if (currentPreset.snapToGaze && Vector3.Dot(referenceAxisForTrackingAnchor, rhs) > currentPreset.snapToGazeDotThreshold)
				{
					Vector3 referenceUpDirection = this.GetReferenceUpDirection(this.m_LastValidCameraTransform);
					BurstMathUtility.OrthogonalLookRotation(normalized, referenceUpDirection, out rotation);
				}
			}
			this.m_HandAnchorSmartFollow.target = position;
			this.m_RotTweenFollow.target = rotation;
			if (!this.m_WasMenuHiddenLastFrame && currentPreset.allowSmoothing)
			{
				this.m_HandAnchorSmartFollow.HandleSmartTween(Time.deltaTime, currentPreset.followLowerSmoothingValue, currentPreset.followUpperSmoothingValue);
				this.m_RotTweenFollow.HandleTween(Time.deltaTime * currentPreset.followLowerSmoothingValue);
				return;
			}
			this.m_HandAnchorSmartFollow.HandleTween(1f);
			if (currentPreset.allowSmoothing)
			{
				this.m_RotTweenFollow.HandleTween(Time.deltaTime * currentPreset.followLowerSmoothingValue);
				return;
			}
			this.m_RotTweenFollow.HandleTween(1f);
		}

		private bool TryGetTrackedAnchors(HandMenu.MenuHandedness desiredHandedness, in FollowPreset currentPreset, out HandMenu.MenuHandedness targetHandedness, out Transform cameraTransform, out Transform palmAnchor, out Transform palmAnchorOffset)
		{
			palmAnchor = null;
			palmAnchorOffset = null;
			targetHandedness = HandMenu.MenuHandedness.None;
			if (!this.TryGetCamera(out cameraTransform) || desiredHandedness == HandMenu.MenuHandedness.None)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			XRInteractionManager xrinteractionManager;
			if (this.m_HideMenuOnSelect && this.TryGetInteractionManager(out xrinteractionManager))
			{
				flag = xrinteractionManager.IsHandSelecting(InteractorHandedness.Left);
				flag2 = xrinteractionManager.IsHandSelecting(InteractorHandedness.Right);
			}
			bool flag3 = !flag && this.PalmMeetsRequirements(cameraTransform, this.m_LeftPalmAnchor, false, currentPreset);
			bool flag4 = !flag2 && this.PalmMeetsRequirements(cameraTransform, this.m_RightPalmAnchor, true, currentPreset);
			if (!flag3 && !flag4)
			{
				return false;
			}
			if (desiredHandedness == HandMenu.MenuHandedness.Either)
			{
				if (flag3 && flag4)
				{
					HandMenu.MenuHandedness menuHandedness = (this.m_LastHandThatMetRequirements == HandMenu.MenuHandedness.Right) ? HandMenu.MenuHandedness.Right : HandMenu.MenuHandedness.Left;
					this.GetTransformAnchorsForHandedness(menuHandedness, out palmAnchor, out palmAnchorOffset);
					targetHandedness = menuHandedness;
					return true;
				}
				if (flag3)
				{
					this.GetTransformAnchorsForHandedness(HandMenu.MenuHandedness.Left, out palmAnchor, out palmAnchorOffset);
					this.m_LastHandThatMetRequirements = HandMenu.MenuHandedness.Left;
					targetHandedness = HandMenu.MenuHandedness.Left;
					return true;
				}
				this.GetTransformAnchorsForHandedness(HandMenu.MenuHandedness.Right, out palmAnchor, out palmAnchorOffset);
				this.m_LastHandThatMetRequirements = HandMenu.MenuHandedness.Right;
				targetHandedness = HandMenu.MenuHandedness.Right;
				return true;
			}
			else if (desiredHandedness == HandMenu.MenuHandedness.Left)
			{
				if (flag3)
				{
					this.GetTransformAnchorsForHandedness(HandMenu.MenuHandedness.Left, out palmAnchor, out palmAnchorOffset);
					this.m_LastHandThatMetRequirements = HandMenu.MenuHandedness.Left;
					targetHandedness = HandMenu.MenuHandedness.Left;
					return true;
				}
				palmAnchor = null;
				palmAnchorOffset = null;
				return false;
			}
			else
			{
				if (desiredHandedness != HandMenu.MenuHandedness.Right)
				{
					return false;
				}
				if (flag4)
				{
					this.GetTransformAnchorsForHandedness(HandMenu.MenuHandedness.Right, out palmAnchor, out palmAnchorOffset);
					this.m_LastHandThatMetRequirements = HandMenu.MenuHandedness.Right;
					targetHandedness = HandMenu.MenuHandedness.Right;
					return true;
				}
				palmAnchor = null;
				palmAnchorOffset = null;
				return false;
			}
		}

		private bool TryGetInteractionManager(out XRInteractionManager manager)
		{
			if (this.m_InteractionManager != null)
			{
				manager = this.m_InteractionManager;
				return true;
			}
			if (ComponentLocatorUtility<XRInteractionManager>.TryFindComponent(out this.m_InteractionManager))
			{
				manager = this.m_InteractionManager;
				return true;
			}
			manager = null;
			return false;
		}

		private void GetTransformAnchorsForHandedness(HandMenu.MenuHandedness handedness, out Transform palmAnchor, out Transform palmAnchorOffset)
		{
			if (handedness == HandMenu.MenuHandedness.Left)
			{
				palmAnchor = this.m_LeftPalmAnchor;
				palmAnchorOffset = this.m_LeftOffsetRoot;
				return;
			}
			if (handedness == HandMenu.MenuHandedness.Right)
			{
				palmAnchor = this.m_RightPalmAnchor;
				palmAnchorOffset = this.m_RightOffsetRoot;
				return;
			}
			palmAnchor = null;
			palmAnchorOffset = null;
		}

		private Vector3 GetReferenceUpDirection(Transform cameraTransform)
		{
			switch (this.m_HandMenuUpDirection)
			{
			case HandMenu.UpDirection.WorldUp:
				return Vector3.up;
			case HandMenu.UpDirection.CameraUp:
				return cameraTransform.up;
			}
			return base.transform.up;
		}

		private bool PalmMeetsRequirements(Transform cameraTransform, Transform palmAnchor, bool isRightHand, in FollowPreset currentPresent)
		{
			if (currentPresent == null)
			{
				return false;
			}
			Vector3 referenceAxisForTrackingAnchor = currentPresent.GetReferenceAxisForTrackingAnchor(palmAnchor, isRightHand);
			Vector3 referenceUpDirection = this.GetReferenceUpDirection(cameraTransform);
			bool flag = !currentPresent.requirePalmFacingUser || Vector3.Dot(referenceAxisForTrackingAnchor, -cameraTransform.forward) > currentPresent.palmFacingUserDotThreshold;
			bool flag2 = !currentPresent.requirePalmFacingUp || Vector3.Dot(referenceAxisForTrackingAnchor, referenceUpDirection) > currentPresent.palmFacingUpDotThreshold;
			return flag && flag2;
		}

		private bool TryGetCamera(out Transform cameraTransform)
		{
			if (this.m_CameraTransform == null)
			{
				Camera main = Camera.main;
				if (main == null)
				{
					cameraTransform = null;
					return false;
				}
				this.m_CameraTransform = main.transform;
			}
			cameraTransform = this.m_CameraTransform;
			return true;
		}

		private static float AngleToDot(float angleDeg)
		{
			return Mathf.Cos(0.017453292f * angleDeg);
		}

		[SerializeField]
		[Tooltip("Child GameObject used to hold the hand menu UI. This is the transform that moves each frame.")]
		private GameObject m_HandMenuUIGameObject;

		[Header("Hand alignment")]
		[SerializeField]
		[Tooltip("Which hand should the menu anchor to. None will disable the hand menu. Either will try to follow the first hand to meet requirements.")]
		private HandMenu.MenuHandedness m_MenuHandedness = HandMenu.MenuHandedness.Either;

		[SerializeField]
		[Tooltip("Determines the up direction of the menu when the hand menu is looking at the camera.")]
		private HandMenu.UpDirection m_HandMenuUpDirection = HandMenu.UpDirection.TransformUp;

		[Header("Palm anchor")]
		[SerializeField]
		[Tooltip("Anchor associated with the left palm pose for the hand.")]
		private Transform m_LeftPalmAnchor;

		[SerializeField]
		[Tooltip("Anchor associated with the right palm pose for the hand.")]
		private Transform m_RightPalmAnchor;

		[Header("Position follow config.")]
		[SerializeField]
		[Tooltip("Minimum distance in meters from target before which tween starts.")]
		private float m_MinFollowDistance = 0.005f;

		[SerializeField]
		[Tooltip("Maximum distance in meters from target before tween targets, when time threshold is reached.")]
		private float m_MaxFollowDistance = 0.03f;

		[SerializeField]
		[Tooltip("Time required to elapse before the max distance allowed goes from the min distance to the max.")]
		private float m_MinToMaxDelaySeconds = 1f;

		[Header("Gaze Alignment Config")]
		[SerializeField]
		[Tooltip("If true, menu will hide when gaze to menu origin's divergence angle is above the threshold. In other words, the menu will only show if looking roughly in it's direction.")]
		private bool m_HideMenuWhenGazeDiverges = true;

		[SerializeField]
		[Tooltip("Only show menu if gaze to menu origin's divergence angle is below this value.")]
		private float m_MenuVisibleGazeAngleDivergenceThreshold = 35f;

		private float m_MenuVisibilityDotThreshold;

		private readonly SmartFollowVector3TweenableVariable m_HandAnchorSmartFollow = new SmartFollowVector3TweenableVariable(0.01f, 0.3f, 3f);

		private readonly QuaternionTweenableVariable m_RotTweenFollow = new QuaternionTweenableVariable();

		private readonly Vector3TweenableVariable m_MenuScaleTweenable = new Vector3TweenableVariable();

		private readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

		private Transform m_CameraTransform;

		private bool m_WasMenuHiddenLastFrame = true;

		private HandMenu.MenuHandedness m_LastHandThatMetRequirements = HandMenu.MenuHandedness.Left;

		[Header("Animation Settings")]
		[SerializeField]
		[Tooltip("Should the menu animate when it is revealed or hidden.")]
		private bool m_AnimateMenuHideAndReveal = true;

		[SerializeField]
		[Tooltip("Duration of the reveal/hide animation in seconds.")]
		private float m_RevealHideAnimationDuration = 0.15f;

		[Header("Selection Behavior")]
		[SerializeField]
		[Tooltip("Should the menu hide when a selection is made with the hand for which the menu is anchored to.")]
		private bool m_HideMenuOnSelect = true;

		[SerializeField]
		[Tooltip("XR Interaction Manager used to determine if a hand is selecting. Will find one if None. Used for Hide Menu On Select.")]
		private XRInteractionManager m_InteractionManager;

		[Header("Follow presets")]
		[SerializeField]
		private FollowPresetDatumProperty m_HandTrackingFollowPreset;

		[SerializeField]
		private FollowPresetDatumProperty m_ControllerFollowPreset;

		private XRInputModalityManager.InputMode m_CurrentInputMode;

		private Transform m_LeftOffsetRoot;

		private Transform m_RightOffsetRoot;

		private Coroutine m_HideCoroutine;

		private Coroutine m_ShowCoroutine;

		private Transform m_LastValidCameraTransform;

		private Transform m_LastValidPalmAnchor;

		private Transform m_LastValidPalmAnchorOffset;

		private Vector3 m_InitialMenuLocalScale = Vector3.one;

		private readonly BindableVariable<bool> m_MenuVisibleBindableVariable = new BindableVariable<bool>(false, true, null, false);

		private float m_LastValidTrackingTime;

		public enum UpDirection
		{
			WorldUp,
			TransformUp,
			CameraUp
		}

		public enum MenuHandedness
		{
			None,
			Left,
			Right,
			Either
		}
	}
}
