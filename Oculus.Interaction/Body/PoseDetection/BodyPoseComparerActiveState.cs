using System;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	public class BodyPoseComparerActiveState : MonoBehaviour, IActiveState, ITimeConsumer
	{
		public float MinTimeInState
		{
			get
			{
				return this._minTimeInState;
			}
			set
			{
				this._minTimeInState = value;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public IReadOnlyDictionary<BodyPoseComparerActiveState.JointComparerConfig, BodyPoseComparerActiveState.BodyPoseComparerFeatureState> FeatureStates
		{
			get
			{
				return this._featureStates;
			}
		}

		protected virtual void Awake()
		{
			this.PoseA = (this._poseA as IBodyPose);
			this.PoseB = (this._poseB as IBodyPose);
		}

		protected virtual void Start()
		{
		}

		public bool Active
		{
			get
			{
				if (!base.isActiveAndEnabled)
				{
					return false;
				}
				bool internalActive = this._internalActive;
				this._internalActive = true;
				foreach (BodyPoseComparerActiveState.JointComparerConfig jointComparerConfig in this._configs)
				{
					float num = internalActive ? (jointComparerConfig.MaxDelta + jointComparerConfig.Width / 2f) : (jointComparerConfig.MaxDelta - jointComparerConfig.Width / 2f);
					float num2;
					bool flag = this.GetJointDelta(jointComparerConfig.Joint, out num2) && Mathf.Abs(num2) <= num;
					this._featureStates[jointComparerConfig] = new BodyPoseComparerActiveState.BodyPoseComparerFeatureState(num2, num);
					this._internalActive = (this._internalActive && flag);
				}
				float num3 = this._timeProvider();
				if (internalActive != this._internalActive)
				{
					this._lastStateChangeTime = num3;
				}
				if (num3 - this._lastStateChangeTime >= this._minTimeInState)
				{
					this._isActive = this._internalActive;
				}
				return this._isActive;
			}
		}

		private bool GetJointDelta(BodyJointId joint, out float delta)
		{
			Pose pose;
			Pose pose2;
			if (!this.PoseA.GetJointPoseLocal(joint, out pose) || !this.PoseB.GetJointPoseLocal(joint, out pose2))
			{
				delta = 0f;
				return false;
			}
			delta = Quaternion.Angle(pose.rotation, pose2.rotation);
			return true;
		}

		public void InjectAllBodyPoseComparerActiveState(IBodyPose poseA, IBodyPose poseB, IEnumerable<BodyPoseComparerActiveState.JointComparerConfig> configs)
		{
			this.InjectPoseA(poseA);
			this.InjectPoseB(poseB);
			this.InjectJoints(configs);
		}

		public void InjectPoseA(IBodyPose poseA)
		{
			this._poseA = (poseA as Object);
			this.PoseA = poseA;
		}

		public void InjectPoseB(IBodyPose poseB)
		{
			this._poseB = (poseB as Object);
			this.PoseB = poseB;
		}

		public void InjectJoints(IEnumerable<BodyPoseComparerActiveState.JointComparerConfig> configs)
		{
			this._configs = new List<BodyPoseComparerActiveState.JointComparerConfig>(configs);
		}

		[Obsolete("Use SetTimeProvider()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		[Tooltip("The first body pose to compare.")]
		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _poseA;

		private IBodyPose PoseA;

		[Tooltip("The second body pose to compare.")]
		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _poseB;

		private IBodyPose PoseB;

		[SerializeField]
		private List<BodyPoseComparerActiveState.JointComparerConfig> _configs = new List<BodyPoseComparerActiveState.JointComparerConfig>
		{
			new BodyPoseComparerActiveState.JointComparerConfig()
		};

		[Tooltip("A new state must be maintaned for at least this many seconds before the Active property changes.")]
		[SerializeField]
		private float _minTimeInState = 0.05f;

		private Func<float> _timeProvider = () => Time.time;

		private Dictionary<BodyPoseComparerActiveState.JointComparerConfig, BodyPoseComparerActiveState.BodyPoseComparerFeatureState> _featureStates = new Dictionary<BodyPoseComparerActiveState.JointComparerConfig, BodyPoseComparerActiveState.BodyPoseComparerFeatureState>();

		private bool _isActive;

		private bool _internalActive;

		private float _lastStateChangeTime;

		public struct BodyPoseComparerFeatureState
		{
			public BodyPoseComparerFeatureState(float delta, float maxDelta)
			{
				this.Delta = delta;
				this.MaxDelta = maxDelta;
			}

			public readonly float Delta;

			public readonly float MaxDelta;
		}

		[Serializable]
		public class JointComparerConfig
		{
			[Tooltip("The joint to compare from each Body Pose")]
			public BodyJointId Joint = BodyJointId.Body_Head;

			[Min(0f)]
			[Tooltip("The maximum angle that two joint rotations can be from each other to be considered equal.")]
			public float MaxDelta = 30f;

			[Tooltip("The width of the threshold when transitioning states. Width / 2 is added to MaxDelta when leaving Active state, and subtracted when entering.")]
			[Min(0f)]
			public float Width = 4f;
		}
	}
}
