using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class JointVelocityActiveState : MonoBehaviour, IActiveState, ITimeConsumer
	{
		public IHand Hand { get; private set; }

		public IJointDeltaProvider JointDeltaProvider { get; private set; }

		public IHmd Hmd { get; private set; }

		public bool Active
		{
			get
			{
				if (!base.isActiveAndEnabled)
				{
					return false;
				}
				this.UpdateActiveState();
				return this._activeState;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public IReadOnlyList<JointVelocityActiveState.JointVelocityFeatureConfig> FeatureConfigs
		{
			get
			{
				return this._featureConfigurations.Values;
			}
		}

		public IReadOnlyDictionary<JointVelocityActiveState.JointVelocityFeatureConfig, JointVelocityActiveState.JointVelocityFeatureState> FeatureStates
		{
			get
			{
				return this._featureStates;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.JointDeltaProvider = (this._jointDeltaProvider as IJointDeltaProvider);
			if (this._hmd != null)
			{
				this.Hmd = (this._hmd as IHmd);
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			IList<HandJointId> list = new List<HandJointId>();
			foreach (JointVelocityActiveState.JointVelocityFeatureConfig jointVelocityFeatureConfig in this.FeatureConfigs)
			{
				list.Add(jointVelocityFeatureConfig.Feature);
				this._featureStates.Add(jointVelocityFeatureConfig, default(JointVelocityActiveState.JointVelocityFeatureState));
			}
			this._jointDeltaConfig = new JointDeltaConfig(base.GetInstanceID(), list);
			this._lastUpdateTime = this._timeProvider();
			this.EndStart(ref this._started);
		}

		private bool CheckAllJointVelocities()
		{
			bool flag = true;
			float num = this._timeProvider() - this._lastUpdateTime;
			float num2 = this._internalState ? (this._minVelocity + this._thresholdWidth * 0.5f) : (this._minVelocity - this._thresholdWidth * 0.5f);
			num2 *= num;
			foreach (JointVelocityActiveState.JointVelocityFeatureConfig jointVelocityFeatureConfig in this.FeatureConfigs)
			{
				Pose wristPose;
				Pose pose;
				Vector3 lhs;
				if (this.Hand.GetJointPose(HandJointId.HandWristRoot, out wristPose) && this.Hand.GetJointPose(jointVelocityFeatureConfig.Feature, out pose) && this.JointDeltaProvider.GetPositionDelta(jointVelocityFeatureConfig.Feature, out lhs))
				{
					Vector3 worldTargetVector = this.GetWorldTargetVector(wristPose, jointVelocityFeatureConfig);
					float num3 = Vector3.Dot(lhs, worldTargetVector);
					this._featureStates[jointVelocityFeatureConfig] = new JointVelocityActiveState.JointVelocityFeatureState(worldTargetVector, (num2 > 0f) ? Mathf.Clamp01(num3 / num2) : 1f);
					bool flag2 = num3 > num2;
					flag = (flag && flag2);
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		protected virtual void Update()
		{
			this.UpdateActiveState();
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.JointDeltaProvider.RegisterConfig(this._jointDeltaConfig);
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.JointDeltaProvider.UnRegisterConfig(this._jointDeltaConfig);
			}
		}

		private void UpdateActiveState()
		{
			if (Time.frameCount <= this._lastStateUpdateFrame)
			{
				return;
			}
			this._lastStateUpdateFrame = Time.frameCount;
			bool flag = this.CheckAllJointVelocities();
			if (flag != this._internalState)
			{
				this._internalState = flag;
				this._lastStateChangeTime = this._timeProvider();
			}
			if (this._timeProvider() - this._lastStateChangeTime >= this._minTimeInState)
			{
				this._activeState = this._internalState;
			}
			this._lastUpdateTime = this._timeProvider();
		}

		private Vector3 GetWorldTargetVector(Pose wristPose, JointVelocityActiveState.JointVelocityFeatureConfig config)
		{
			switch (config.RelativeTo)
			{
			default:
				return this.GetHandAxisVector(config.HandAxis, wristPose);
			case JointVelocityActiveState.RelativeTo.World:
				return this.GetWorldAxisVector(config.WorldAxis);
			case JointVelocityActiveState.RelativeTo.Head:
				return this.GetHeadAxisVector(config.HeadAxis);
			}
		}

		private Vector3 GetWorldAxisVector(JointVelocityActiveState.WorldAxis axis)
		{
			switch (axis)
			{
			default:
				return Vector3.right;
			case JointVelocityActiveState.WorldAxis.NegativeX:
				return Vector3.left;
			case JointVelocityActiveState.WorldAxis.PositiveY:
				return Vector3.up;
			case JointVelocityActiveState.WorldAxis.NegativeY:
				return Vector3.down;
			case JointVelocityActiveState.WorldAxis.PositiveZ:
				return Vector3.forward;
			case JointVelocityActiveState.WorldAxis.NegativeZ:
				return Vector3.back;
			}
		}

		private Vector3 GetHandAxisVector(JointVelocityActiveState.HandAxis axis, Pose wristPose)
		{
			switch (axis)
			{
			case JointVelocityActiveState.HandAxis.PalmForward:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftPalmar : Constants.RightPalmar);
			case JointVelocityActiveState.HandAxis.PalmBackward:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftDorsal : Constants.RightDorsal);
			case JointVelocityActiveState.HandAxis.WristUp:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftThumbSide : Constants.RightThumbSide);
			case JointVelocityActiveState.HandAxis.WristDown:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftPinkySide : Constants.RightPinkySide);
			case JointVelocityActiveState.HandAxis.WristForward:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftDistal : Constants.RightDistal);
			case JointVelocityActiveState.HandAxis.WristBackward:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftProximal : Constants.RightProximal);
			default:
				return Vector3.zero;
			}
		}

		private Vector3 GetHeadAxisVector(JointVelocityActiveState.HeadAxis axis)
		{
			Pose pose;
			this.Hmd.TryGetRootPose(out pose);
			Vector3 result;
			switch (axis)
			{
			case JointVelocityActiveState.HeadAxis.HeadForward:
				result = pose.forward;
				break;
			case JointVelocityActiveState.HeadAxis.HeadBackward:
				result = -pose.forward;
				break;
			case JointVelocityActiveState.HeadAxis.HeadUp:
				result = pose.up;
				break;
			case JointVelocityActiveState.HeadAxis.HeadDown:
				result = -pose.up;
				break;
			case JointVelocityActiveState.HeadAxis.HeadLeft:
				result = -pose.right;
				break;
			case JointVelocityActiveState.HeadAxis.HeadRight:
				result = pose.right;
				break;
			default:
				result = Vector3.zero;
				break;
			}
			return result;
		}

		public void InjectAllJointVelocityActiveState(JointVelocityActiveState.JointVelocityFeatureConfigList featureConfigs, IHand hand, IJointDeltaProvider jointDeltaProvider)
		{
			this.InjectFeatureConfigList(featureConfigs);
			this.InjectHand(hand);
			this.InjectJointDeltaProvider(jointDeltaProvider);
		}

		public void InjectFeatureConfigList(JointVelocityActiveState.JointVelocityFeatureConfigList featureConfigs)
		{
			this._featureConfigs = featureConfigs;
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectJointDeltaProvider(IJointDeltaProvider jointDeltaProvider)
		{
			this.JointDeltaProvider = jointDeltaProvider;
			this._jointDeltaProvider = (jointDeltaProvider as Object);
		}

		[Obsolete("Use SetTimeProvider()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public void InjectOptionalHmd(IHmd hmd)
		{
			this._hmd = (hmd as Object);
			this.Hmd = hmd;
		}

		[Tooltip("Provided joints will be sourced from this IHand.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("JointDeltaProvider caches joint deltas to avoid unnecessary recomputing of deltas.")]
		[SerializeField]
		[Interface(typeof(IJointDeltaProvider), new Type[]
		{

		})]
		private Object _jointDeltaProvider;

		[Tooltip("Reference to the Hmd providing the HeadAxis pose.")]
		[SerializeField]
		[Optional]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		[SerializeField]
		private JointVelocityActiveState.JointVelocityFeatureConfigList _featureConfigs;

		[SerializeField]
		private JointVelocityActiveState.JointVelocityFeatureConfigList _featureConfigurations;

		[Tooltip("The velocity used for the detection threshold, in units per second.")]
		[SerializeField]
		[Min(0f)]
		private float _minVelocity = 0.5f;

		[Tooltip("The min velocity value will be modified by this width to create differing enter/exit thresholds. Used to prevent chattering at the threshold edge.")]
		[SerializeField]
		[Min(0f)]
		private float _thresholdWidth = 0.02f;

		[Tooltip("A new state must be maintaned for at least this many seconds before the Active property changes.")]
		[SerializeField]
		[Min(0f)]
		private float _minTimeInState = 0.05f;

		private Func<float> _timeProvider = () => Time.time;

		private Dictionary<JointVelocityActiveState.JointVelocityFeatureConfig, JointVelocityActiveState.JointVelocityFeatureState> _featureStates = new Dictionary<JointVelocityActiveState.JointVelocityFeatureConfig, JointVelocityActiveState.JointVelocityFeatureState>();

		private JointDeltaConfig _jointDeltaConfig;

		private int _lastStateUpdateFrame;

		private float _lastStateChangeTime;

		private float _lastUpdateTime;

		private bool _internalState;

		private bool _activeState;

		protected bool _started;

		public enum RelativeTo
		{
			Hand,
			World,
			Head
		}

		public enum WorldAxis
		{
			PositiveX,
			NegativeX,
			PositiveY,
			NegativeY,
			PositiveZ,
			NegativeZ
		}

		public enum HeadAxis
		{
			HeadForward,
			HeadBackward,
			HeadUp,
			HeadDown,
			HeadLeft,
			HeadRight
		}

		public enum HandAxis
		{
			PalmForward,
			PalmBackward,
			WristUp,
			WristDown,
			WristForward,
			WristBackward
		}

		[Serializable]
		public struct JointVelocityFeatureState
		{
			public JointVelocityFeatureState(Vector3 targetVector, float velocity)
			{
				this.TargetVector = targetVector;
				this.Amount = velocity;
			}

			public readonly Vector3 TargetVector;

			public readonly float Amount;
		}

		[Serializable]
		public class JointVelocityFeatureConfigList
		{
			public List<JointVelocityActiveState.JointVelocityFeatureConfig> Values
			{
				get
				{
					return this._values;
				}
			}

			[SerializeField]
			private List<JointVelocityActiveState.JointVelocityFeatureConfig> _values;
		}

		[Serializable]
		public class JointVelocityFeatureConfig : FeatureConfigBase<HandJointId>
		{
			public JointVelocityActiveState.RelativeTo RelativeTo
			{
				get
				{
					return this._relativeTo;
				}
				set
				{
					this._relativeTo = value;
				}
			}

			public JointVelocityActiveState.WorldAxis WorldAxis
			{
				get
				{
					return this._worldAxis;
				}
				set
				{
					this._worldAxis = value;
				}
			}

			public JointVelocityActiveState.HandAxis HandAxis
			{
				get
				{
					return this._handAxis;
				}
				set
				{
					this._handAxis = value;
				}
			}

			public JointVelocityActiveState.HeadAxis HeadAxis
			{
				get
				{
					return this._headAxis;
				}
				set
				{
					this._headAxis = value;
				}
			}

			[Tooltip("The detection axis will be in this coordinate space.")]
			[SerializeField]
			private JointVelocityActiveState.RelativeTo _relativeTo;

			[Tooltip("The world axis used for detection.")]
			[SerializeField]
			private JointVelocityActiveState.WorldAxis _worldAxis = JointVelocityActiveState.WorldAxis.PositiveZ;

			[Tooltip("The axis of the hand root pose used for detection.")]
			[SerializeField]
			private JointVelocityActiveState.HandAxis _handAxis = JointVelocityActiveState.HandAxis.WristForward;

			[Tooltip("The axis of the head pose used for detection.")]
			[SerializeField]
			private JointVelocityActiveState.HeadAxis _headAxis;
		}
	}
}
