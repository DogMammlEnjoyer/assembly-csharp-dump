using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.SmartTweenableVariables;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	[AddComponentMenu("XR/Lazy Follow", 22)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.UI.LazyFollow.html")]
	public class LazyFollow : MonoBehaviour
	{
		public Transform target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
			}
		}

		public Vector3 targetOffset
		{
			get
			{
				return this.m_TargetOffset;
			}
			set
			{
				this.m_TargetOffset = value;
			}
		}

		public bool followInLocalSpace
		{
			get
			{
				return this.m_FollowInLocalSpace;
			}
			set
			{
				this.m_FollowInLocalSpace = value;
				this.ValidateFollowMode();
			}
		}

		public bool applyTargetInLocalSpace
		{
			get
			{
				return this.m_ApplyTargetInLocalSpace;
			}
			set
			{
				this.m_ApplyTargetInLocalSpace = value;
			}
		}

		public float movementSpeed
		{
			get
			{
				return this.m_MovementSpeed;
			}
			set
			{
				this.m_MovementSpeed = value;
				this.UpdateUpperAndLowerSpeedBounds();
			}
		}

		public float movementSpeedVariancePercentage
		{
			get
			{
				return this.m_MovementSpeedVariancePercentage;
			}
			set
			{
				this.m_MovementSpeedVariancePercentage = Mathf.Clamp(value, 0f, 0.999f);
				this.UpdateUpperAndLowerSpeedBounds();
			}
		}

		public bool snapOnEnable
		{
			get
			{
				return this.m_SnapOnEnable;
			}
			set
			{
				this.m_SnapOnEnable = value;
			}
		}

		public LazyFollow.PositionFollowMode positionFollowMode
		{
			get
			{
				return this.m_PositionFollowMode;
			}
			set
			{
				this.m_PositionFollowMode = value;
			}
		}

		public float minDistanceAllowed
		{
			get
			{
				return this.m_MinDistanceAllowed;
			}
			set
			{
				this.m_MinDistanceAllowed = value;
				if (this.m_Vector3TweenableVariable != null)
				{
					this.m_Vector3TweenableVariable.minDistanceAllowed = value;
				}
			}
		}

		public float maxDistanceAllowed
		{
			get
			{
				return this.m_MaxDistanceAllowed;
			}
			set
			{
				this.m_MaxDistanceAllowed = value;
				if (this.m_Vector3TweenableVariable != null)
				{
					this.m_Vector3TweenableVariable.maxDistanceAllowed = value;
				}
			}
		}

		public float timeUntilThresholdReachesMaxDistance
		{
			get
			{
				return this.m_TimeUntilThresholdReachesMaxDistance;
			}
			set
			{
				this.m_TimeUntilThresholdReachesMaxDistance = value;
				if (this.m_Vector3TweenableVariable != null)
				{
					this.m_Vector3TweenableVariable.minToMaxDelaySeconds = value;
				}
			}
		}

		public LazyFollow.RotationFollowMode rotationFollowMode
		{
			get
			{
				return this.m_RotationFollowMode;
			}
			set
			{
				this.m_RotationFollowMode = value;
				this.ValidateFollowMode();
			}
		}

		public float minAngleAllowed
		{
			get
			{
				return this.m_MinAngleAllowed;
			}
			set
			{
				this.m_MinAngleAllowed = value;
				if (this.m_QuaternionTweenableVariable != null)
				{
					this.m_QuaternionTweenableVariable.minAngleAllowed = value;
				}
			}
		}

		public float maxAngleAllowed
		{
			get
			{
				return this.m_MaxAngleAllowed;
			}
			set
			{
				this.m_MaxAngleAllowed = value;
				if (this.m_QuaternionTweenableVariable != null)
				{
					this.m_QuaternionTweenableVariable.maxAngleAllowed = value;
				}
			}
		}

		public float timeUntilThresholdReachesMaxAngle
		{
			get
			{
				return this.m_TimeUntilThresholdReachesMaxAngle;
			}
			set
			{
				this.m_TimeUntilThresholdReachesMaxAngle = value;
				if (this.m_QuaternionTweenableVariable != null)
				{
					this.m_QuaternionTweenableVariable.minToMaxDelaySeconds = value;
				}
			}
		}

		protected void OnValidate()
		{
			this.UpdateUpperAndLowerSpeedBounds();
			this.ValidateFollowMode();
			if (this.m_Vector3TweenableVariable != null)
			{
				this.m_Vector3TweenableVariable.minDistanceAllowed = this.m_MinDistanceAllowed;
				this.m_Vector3TweenableVariable.maxDistanceAllowed = this.m_MaxDistanceAllowed;
				this.m_Vector3TweenableVariable.minToMaxDelaySeconds = this.m_TimeUntilThresholdReachesMaxDistance;
			}
			if (this.m_QuaternionTweenableVariable != null)
			{
				this.m_QuaternionTweenableVariable.minAngleAllowed = this.m_MinAngleAllowed;
				this.m_QuaternionTweenableVariable.maxAngleAllowed = this.m_MaxAngleAllowed;
				this.m_QuaternionTweenableVariable.minToMaxDelaySeconds = this.m_TimeUntilThresholdReachesMaxAngle;
			}
		}

		protected void Awake()
		{
			this.m_Vector3TweenableVariable = new SmartFollowVector3TweenableVariable(this.m_MinDistanceAllowed, this.m_MaxDistanceAllowed, this.m_TimeUntilThresholdReachesMaxDistance);
			this.m_QuaternionTweenableVariable = new SmartFollowQuaternionTweenableVariable(this.m_MinAngleAllowed, this.m_MaxAngleAllowed, this.m_TimeUntilThresholdReachesMaxAngle);
			this.UpdateUpperAndLowerSpeedBounds();
			this.ValidateFollowMode();
		}

		protected void OnEnable()
		{
			if (this.m_Target == null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					this.m_Target = main.transform;
				}
			}
			Pose pose = this.followInLocalSpace ? base.transform.GetLocalPose() : base.transform.GetWorldPose();
			this.m_Vector3TweenableVariable.target = pose.position;
			this.m_QuaternionTweenableVariable.target = pose.rotation;
			this.m_BindingsGroup.AddBinding(this.m_Vector3TweenableVariable.SubscribeAndUpdate(new Action<float3>(this.UpdatePosition)));
			this.m_BindingsGroup.AddBinding(this.m_QuaternionTweenableVariable.SubscribeAndUpdate(new Action<Quaternion>(this.UpdateRotation)));
			if (this.m_SnapOnEnable)
			{
				Vector3 v;
				if (this.m_PositionFollowMode != LazyFollow.PositionFollowMode.None && this.TryGetThresholdTargetPosition(out v))
				{
					this.m_Vector3TweenableVariable.target = v;
				}
				Quaternion target;
				if (this.m_RotationFollowMode != LazyFollow.RotationFollowMode.None && this.TryGetThresholdTargetRotation(out target))
				{
					this.m_QuaternionTweenableVariable.target = target;
				}
				this.m_Vector3TweenableVariable.HandleTween(1f);
				this.m_QuaternionTweenableVariable.HandleTween(1f);
			}
		}

		protected void OnDisable()
		{
			this.m_BindingsGroup.Clear();
		}

		protected void OnDestroy()
		{
			SmartFollowVector3TweenableVariable vector3TweenableVariable = this.m_Vector3TweenableVariable;
			if (vector3TweenableVariable == null)
			{
				return;
			}
			vector3TweenableVariable.Dispose();
		}

		protected void LateUpdate()
		{
			if (this.m_Target == null)
			{
				return;
			}
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (this.m_PositionFollowMode != LazyFollow.PositionFollowMode.None)
			{
				Vector3 v;
				if (this.TryGetThresholdTargetPosition(out v))
				{
					this.m_Vector3TweenableVariable.target = v;
				}
				if (this.m_MovementSpeedVariancePercentage > 0f)
				{
					this.m_Vector3TweenableVariable.HandleSmartTween(unscaledDeltaTime, this.m_LowerMovementSpeed, this.m_UpperMovementSpeed);
				}
				else
				{
					this.m_Vector3TweenableVariable.HandleTween(unscaledDeltaTime * this.movementSpeed);
				}
			}
			if (this.m_RotationFollowMode != LazyFollow.RotationFollowMode.None)
			{
				Quaternion target;
				if (this.TryGetThresholdTargetRotation(out target))
				{
					this.m_QuaternionTweenableVariable.target = target;
				}
				if (this.m_MovementSpeedVariancePercentage > 0f)
				{
					this.m_QuaternionTweenableVariable.HandleSmartTween(unscaledDeltaTime, this.m_LowerMovementSpeed, this.m_UpperMovementSpeed);
					return;
				}
				this.m_QuaternionTweenableVariable.HandleTween(unscaledDeltaTime * this.movementSpeed);
			}
		}

		private void UpdatePosition(float3 position)
		{
			if (this.applyTargetInLocalSpace)
			{
				base.transform.localPosition = position;
				return;
			}
			base.transform.position = position;
		}

		private void UpdateRotation(Quaternion rotation)
		{
			if (this.applyTargetInLocalSpace)
			{
				base.transform.localRotation = rotation;
				return;
			}
			base.transform.rotation = rotation;
		}

		protected virtual bool TryGetThresholdTargetPosition(out Vector3 newTarget)
		{
			LazyFollow.PositionFollowMode positionFollowMode = this.m_PositionFollowMode;
			if (positionFollowMode != LazyFollow.PositionFollowMode.None)
			{
				if (positionFollowMode == LazyFollow.PositionFollowMode.Follow)
				{
					if (this.followInLocalSpace)
					{
						newTarget = this.m_Target.localPosition + this.m_TargetOffset;
					}
					else
					{
						newTarget = this.m_Target.position + this.m_Target.TransformVector(this.m_TargetOffset);
					}
					return this.m_Vector3TweenableVariable.IsNewTargetWithinThreshold(newTarget);
				}
				Debug.LogError(string.Format("Unhandled {0}={1}", "PositionFollowMode", this.m_PositionFollowMode), this);
			}
			newTarget = (this.followInLocalSpace ? base.transform.localPosition : base.transform.position);
			return false;
		}

		protected virtual bool TryGetThresholdTargetRotation(out Quaternion newTarget)
		{
			switch (this.m_RotationFollowMode)
			{
			case LazyFollow.RotationFollowMode.None:
				break;
			case LazyFollow.RotationFollowMode.LookAt:
			{
				Vector3 vector = base.transform.position - this.m_Target.position;
				Vector3 normalized = vector.normalized;
				vector = Vector3.up;
				BurstMathUtility.OrthogonalLookRotation(normalized, vector, out newTarget);
				goto IL_106;
			}
			case LazyFollow.RotationFollowMode.LookAtWithWorldUp:
			{
				Vector3 vector = base.transform.position - this.m_Target.position;
				Vector3 normalized2 = vector.normalized;
				vector = Vector3.up;
				BurstMathUtility.LookRotationWithForwardProjectedOnPlane(normalized2, vector, out newTarget);
				goto IL_106;
			}
			case LazyFollow.RotationFollowMode.Follow:
				newTarget = (this.followInLocalSpace ? this.m_Target.localRotation : this.m_Target.rotation);
				goto IL_106;
			default:
				Debug.LogError(string.Format("Unhandled {0}={1}", "RotationFollowMode", this.m_RotationFollowMode), this);
				break;
			}
			newTarget = (this.followInLocalSpace ? base.transform.localRotation : base.transform.rotation);
			return false;
			IL_106:
			return this.m_QuaternionTweenableVariable.IsNewTargetWithinThreshold(newTarget);
		}

		private void ValidateFollowMode()
		{
			if (!this.m_FollowInLocalSpace)
			{
				return;
			}
			if (this.m_RotationFollowMode == LazyFollow.RotationFollowMode.LookAt || this.m_RotationFollowMode == LazyFollow.RotationFollowMode.LookAtWithWorldUp)
			{
				if (Application.isPlaying)
				{
					this.m_FollowInLocalSpace = false;
					XRLoggingUtils.LogWarning("Cannot follow in local space if Rotation Follow Mode set to look at the target. Turning off Follow In Local Space.", this);
					return;
				}
				XRLoggingUtils.LogWarning("Cannot follow in local space if Rotation Follow Mode set to look at the target.", this);
			}
		}

		private void UpdateUpperAndLowerSpeedBounds()
		{
			if (this.m_MovementSpeedVariancePercentage > 0f)
			{
				this.m_LowerMovementSpeed = this.m_MovementSpeed - this.m_MovementSpeedVariancePercentage * this.m_MovementSpeed;
				this.m_UpperMovementSpeed = this.m_MovementSpeed * (1f + this.m_MovementSpeedVariancePercentage);
				return;
			}
			this.m_LowerMovementSpeed = this.m_MovementSpeed;
			this.m_UpperMovementSpeed = this.m_MovementSpeed;
		}

		private const float k_LowerSpeedVariance = 0f;

		private const float k_UpperSpeedVariance = 0.999f;

		[Header("Target Config")]
		[SerializeField]
		[Tooltip("(Optional) The object being followed. If not set, this will default to the main camera when this component is enabled.")]
		private Transform m_Target;

		[SerializeField]
		[Tooltip("The amount to offset the target's position when following. This position is relative/local to the target object.")]
		private Vector3 m_TargetOffset = new Vector3(0f, 0f, 0.5f);

		[Space]
		[SerializeField]
		[Tooltip("If true, read the local transform of the target to lazy follow, otherwise read the world transform. If using look at rotation follow modes, only world-space follow is supported.")]
		private bool m_FollowInLocalSpace;

		[SerializeField]
		[Tooltip("If true, apply the target offset in local space. If false, apply the target offset in world space.")]
		private bool m_ApplyTargetInLocalSpace;

		[Header("General Follow Params")]
		[SerializeField]
		[Tooltip("Movement speed used when smoothing to new target. Lower values mean the lazy follow lags further behind the target.")]
		private float m_MovementSpeed = 6f;

		[SerializeField]
		[Range(0f, 0.999f)]
		[Tooltip("Adjust movement speed based on distance from the target using a tolerance percentage. 0% for constant speed.")]
		private float m_MovementSpeedVariancePercentage = 0.25f;

		[SerializeField]
		[Tooltip("Snap to target position when this component is enabled.")]
		private bool m_SnapOnEnable = true;

		[Header("Position Follow Params")]
		[SerializeField]
		[Tooltip("Determines the follow mode used to determine a new rotation. Look At is best used with the target being the main camera.")]
		private LazyFollow.PositionFollowMode m_PositionFollowMode = LazyFollow.PositionFollowMode.Follow;

		[SerializeField]
		[Tooltip("Minimum distance from target before which a follow lazy follow starts.")]
		private float m_MinDistanceAllowed = 0.01f;

		[SerializeField]
		[Tooltip("Maximum distance from target before lazy follow targets, when time threshold is reached.")]
		private float m_MaxDistanceAllowed = 0.3f;

		[SerializeField]
		[Tooltip("Time required to elapse (in seconds) before the max distance allowed goes from the min distance to the max.")]
		private float m_TimeUntilThresholdReachesMaxDistance = 3f;

		[Header("Rotation Follow Params")]
		[SerializeField]
		[Tooltip("Determines the follow mode used to determine a new rotation. Look At is best used with the target being the main camera.")]
		private LazyFollow.RotationFollowMode m_RotationFollowMode = LazyFollow.RotationFollowMode.LookAt;

		[SerializeField]
		[Tooltip("Minimum angle offset (in degrees) from target before which lazy follow starts.")]
		private float m_MinAngleAllowed = 0.1f;

		[SerializeField]
		[Tooltip("Maximum angle offset (in degrees) from target before lazy follow targets, when time threshold is reached.")]
		private float m_MaxAngleAllowed = 5f;

		[SerializeField]
		[Tooltip("Time required to elapse (in seconds) before the max angle offset allowed goes from the min angle offset to the max.")]
		private float m_TimeUntilThresholdReachesMaxAngle = 3f;

		private float m_LowerMovementSpeed;

		private float m_UpperMovementSpeed;

		private readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

		private SmartFollowVector3TweenableVariable m_Vector3TweenableVariable;

		private SmartFollowQuaternionTweenableVariable m_QuaternionTweenableVariable;

		public enum PositionFollowMode
		{
			None,
			Follow
		}

		public enum RotationFollowMode
		{
			None,
			LookAt,
			LookAtWithWorldUp,
			Follow
		}
	}
}
