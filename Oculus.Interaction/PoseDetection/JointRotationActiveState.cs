using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class JointRotationActiveState : MonoBehaviour, IActiveState, ITimeConsumer
	{
		public IHand Hand { get; private set; }

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

		public IReadOnlyList<JointRotationActiveState.JointRotationFeatureConfig> FeatureConfigs
		{
			get
			{
				return this._featureConfigurations.Values;
			}
		}

		public IReadOnlyDictionary<JointRotationActiveState.JointRotationFeatureConfig, JointRotationActiveState.JointRotationFeatureState> FeatureStates
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
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			IList<HandJointId> list = new List<HandJointId>();
			foreach (JointRotationActiveState.JointRotationFeatureConfig jointRotationFeatureConfig in this.FeatureConfigs)
			{
				list.Add(jointRotationFeatureConfig.Feature);
				this._featureStates.Add(jointRotationFeatureConfig, default(JointRotationActiveState.JointRotationFeatureState));
			}
			this._jointDeltaConfig = new JointDeltaConfig(base.GetInstanceID(), list);
			this._lastUpdateTime = this._timeProvider();
			this.EndStart(ref this._started);
		}

		private bool CheckAllJointRotations()
		{
			bool flag = true;
			float num = this._timeProvider() - this._lastUpdateTime;
			float num2 = this._internalState ? (this._degreesPerSecond + this._thresholdWidth * 0.5f) : (this._degreesPerSecond - this._thresholdWidth * 0.5f);
			num2 *= num;
			foreach (JointRotationActiveState.JointRotationFeatureConfig jointRotationFeatureConfig in this.FeatureConfigs)
			{
				Pose wristPose;
				Pose pose;
				Quaternion quaternion;
				if (this.Hand.GetJointPose(HandJointId.HandWristRoot, out wristPose) && this.Hand.GetJointPose(jointRotationFeatureConfig.Feature, out pose) && this.JointDeltaProvider.GetRotationDelta(jointRotationFeatureConfig.Feature, out quaternion))
				{
					Vector3 worldTargetAxis = this.GetWorldTargetAxis(wristPose, jointRotationFeatureConfig);
					float num3;
					Vector3 lhs;
					quaternion.ToAngleAxis(out num3, out lhs);
					float num4 = Mathf.Abs(Vector3.Dot(lhs, worldTargetAxis));
					float num5 = num3 * num4;
					this._featureStates[jointRotationFeatureConfig] = new JointRotationActiveState.JointRotationFeatureState(worldTargetAxis, (num2 <= 0f) ? 1f : Mathf.Clamp01(num5 / num2));
					bool flag2 = num5 > num2;
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
			bool flag = this.CheckAllJointRotations();
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

		private Vector3 GetWorldTargetAxis(Pose wristPose, JointRotationActiveState.JointRotationFeatureConfig config)
		{
			JointRotationActiveState.RelativeTo relativeTo = config.RelativeTo;
			if (relativeTo == JointRotationActiveState.RelativeTo.Hand || relativeTo != JointRotationActiveState.RelativeTo.World)
			{
				return this.GetHandAxisVector(config.HandAxis, wristPose);
			}
			return this.GetWorldAxisVector(config.WorldAxis);
		}

		private Vector3 GetWorldAxisVector(JointRotationActiveState.WorldAxis axis)
		{
			switch (axis)
			{
			default:
				return Vector3.right;
			case JointRotationActiveState.WorldAxis.NegativeX:
				return Vector3.left;
			case JointRotationActiveState.WorldAxis.PositiveY:
				return Vector3.up;
			case JointRotationActiveState.WorldAxis.NegativeY:
				return Vector3.down;
			case JointRotationActiveState.WorldAxis.PositiveZ:
				return Vector3.forward;
			case JointRotationActiveState.WorldAxis.NegativeZ:
				return Vector3.back;
			}
		}

		private Vector3 GetHandAxisVector(JointRotationActiveState.HandAxis axis, Pose wristPose)
		{
			switch (axis)
			{
			case JointRotationActiveState.HandAxis.Pronation:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftProximal : Constants.RightProximal);
			case JointRotationActiveState.HandAxis.Supination:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftDistal : Constants.RightDistal);
			case JointRotationActiveState.HandAxis.RadialDeviation:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftPalmar : Constants.RightPalmar);
			case JointRotationActiveState.HandAxis.UlnarDeviation:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftDorsal : Constants.RightDorsal);
			case JointRotationActiveState.HandAxis.Extension:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftThumbSide : Constants.RightThumbSide);
			case JointRotationActiveState.HandAxis.Flexion:
				return wristPose.rotation * ((this.Hand.Handedness == Handedness.Left) ? Constants.LeftPinkySide : Constants.RightPinkySide);
			default:
				return Vector3.zero;
			}
		}

		public void InjectAllJointRotationActiveState(JointRotationActiveState.JointRotationFeatureConfigList featureConfigs, IHand hand, IJointDeltaProvider jointDeltaProvider)
		{
			this.InjectFeatureConfigList(featureConfigs);
			this.InjectHand(hand);
			this.InjectJointDeltaProvider(jointDeltaProvider);
		}

		public void InjectFeatureConfigList(JointRotationActiveState.JointRotationFeatureConfigList featureConfigs)
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

		[SerializeField]
		private JointRotationActiveState.JointRotationFeatureConfigList _featureConfigs;

		[SerializeField]
		private JointRotationActiveState.JointRotationFeatureConfigList _featureConfigurations;

		[Tooltip("The angular velocity used for the detection threshold, in degrees per second.")]
		[SerializeField]
		[Min(0f)]
		private float _degreesPerSecond = 120f;

		[Tooltip("The degrees per second value will be modified by this width to create differing enter/exit thresholds. Used to prevent chattering at the threshold edge.")]
		[SerializeField]
		[Min(0f)]
		private float _thresholdWidth = 30f;

		[Tooltip("A new state must be maintaned for at least this many seconds before the Active property changes.")]
		[SerializeField]
		[Min(0f)]
		private float _minTimeInState = 0.05f;

		private Func<float> _timeProvider = () => Time.time;

		private Dictionary<JointRotationActiveState.JointRotationFeatureConfig, JointRotationActiveState.JointRotationFeatureState> _featureStates = new Dictionary<JointRotationActiveState.JointRotationFeatureConfig, JointRotationActiveState.JointRotationFeatureState>();

		private JointDeltaConfig _jointDeltaConfig;

		private IJointDeltaProvider JointDeltaProvider;

		private int _lastStateUpdateFrame;

		private float _lastStateChangeTime;

		private float _lastUpdateTime;

		private bool _internalState;

		private bool _activeState;

		protected bool _started;

		public enum RelativeTo
		{
			Hand,
			World
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

		public enum HandAxis
		{
			Pronation,
			Supination,
			RadialDeviation,
			UlnarDeviation,
			Extension,
			Flexion
		}

		[Serializable]
		public struct JointRotationFeatureState
		{
			public JointRotationFeatureState(Vector3 targetAxis, float amount)
			{
				this.TargetAxis = targetAxis;
				this.Amount = amount;
			}

			public readonly Vector3 TargetAxis;

			public readonly float Amount;
		}

		[Serializable]
		public class JointRotationFeatureConfigList
		{
			public List<JointRotationActiveState.JointRotationFeatureConfig> Values
			{
				get
				{
					return this._values;
				}
			}

			[SerializeField]
			private List<JointRotationActiveState.JointRotationFeatureConfig> _values;
		}

		[Serializable]
		public class JointRotationFeatureConfig : FeatureConfigBase<HandJointId>
		{
			public JointRotationActiveState.RelativeTo RelativeTo
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

			public JointRotationActiveState.WorldAxis WorldAxis
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

			public JointRotationActiveState.HandAxis HandAxis
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

			[Tooltip("The detection axis will be in this coordinate space.")]
			[SerializeField]
			private JointRotationActiveState.RelativeTo _relativeTo;

			[Tooltip("The world axis used for detection.")]
			[SerializeField]
			private JointRotationActiveState.WorldAxis _worldAxis = JointRotationActiveState.WorldAxis.PositiveZ;

			[Tooltip("The axis of the hand root pose used for detection.")]
			[SerializeField]
			private JointRotationActiveState.HandAxis _handAxis = JointRotationActiveState.HandAxis.RadialDeviation;
		}
	}
}
