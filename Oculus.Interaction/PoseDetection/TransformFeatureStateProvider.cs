using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class TransformFeatureStateProvider : MonoBehaviour, ITransformFeatureStateProvider, ITimeConsumer
	{
		public IHand Hand { get; private set; }

		public IHmd Hmd { get; private set; }

		public ITrackingToWorldTransformer TrackingToWorldTransformer { get; private set; }

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.Hmd = (this._hmd as IHmd);
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
			this._transformFeatureStateCollection = new TransformFeatureStateCollection();
		}

		public void RegisterConfig(TransformConfig transformConfig)
		{
			Func<float> timeProvider = () => this._timeProvider();
			this._transformFeatureStateCollection.RegisterConfig(transformConfig, this._jointData, timeProvider);
		}

		public void UnRegisterConfig(TransformConfig transformConfig)
		{
			this._transformFeatureStateCollection.UnRegisterConfig(transformConfig);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.HandDataAvailable;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandDataAvailable;
			}
		}

		private void HandDataAvailable()
		{
			this.UpdateJointData();
			this.UpdateStateForHand();
		}

		private void UpdateJointData()
		{
			this._jointData.IsValid = (this.Hand.GetJointPose(HandJointId.HandWristRoot, out this._jointData.WristPose) && this.Hmd.TryGetRootPose(out this._jointData.CenterEyePose));
			if (!this._jointData.IsValid)
			{
				return;
			}
			this._jointData.Handedness = this.Hand.Handedness;
			this._jointData.TrackingSystemUp = this.TrackingToWorldTransformer.Transform.up;
			this._jointData.TrackingSystemForward = this.TrackingToWorldTransformer.Transform.forward;
		}

		private void UpdateStateForHand()
		{
			this._transformFeatureStateCollection.UpdateFeatureStates(this.Hand.CurrentDataVersion, this._disableProactiveEvaluation);
		}

		public bool IsHandDataValid()
		{
			return this._jointData.IsValid;
		}

		public bool IsStateActive(TransformConfig config, TransformFeature feature, FeatureStateActiveMode mode, string stateId)
		{
			string currentFeatureState = this.GetCurrentFeatureState(config, feature);
			if (mode != FeatureStateActiveMode.Is)
			{
				return mode == FeatureStateActiveMode.IsNot && currentFeatureState != stateId;
			}
			return currentFeatureState == stateId;
		}

		private string GetCurrentFeatureState(TransformConfig config, TransformFeature feature)
		{
			return this._transformFeatureStateCollection.GetStateProvider(config).GetCurrentFeatureState(feature);
		}

		public bool GetCurrentState(TransformConfig config, TransformFeature transformFeature, out string currentState)
		{
			if (!this.IsHandDataValid())
			{
				currentState = null;
				return false;
			}
			currentState = this.GetCurrentFeatureState(config, transformFeature);
			return currentState != null;
		}

		public float? GetFeatureValue(TransformConfig config, TransformFeature transformFeature)
		{
			if (!this.IsHandDataValid())
			{
				return null;
			}
			return new float?(TransformFeatureValueProvider.GetValue(transformFeature, this._jointData, config));
		}

		public void GetFeatureVectorAndWristPos(TransformConfig config, TransformFeature transformFeature, bool isHandVector, ref Vector3? featureVec, ref Vector3? wristPos)
		{
			featureVec = null;
			wristPos = null;
			if (!this.IsHandDataValid())
			{
				return;
			}
			featureVec = new Vector3?(isHandVector ? TransformFeatureValueProvider.GetHandVectorForFeature(transformFeature, this._jointData) : TransformFeatureValueProvider.GetTargetVectorForFeature(transformFeature, this._jointData, config));
			wristPos = new Vector3?(this._jointData.WristPose.position);
		}

		public void InjectAllTransformFeatureStateProvider(IHand hand, IHmd hmd, bool disableProactiveEvaluation)
		{
			this.InjectHand(hand);
			this.InjectHmd(hmd);
			this._disableProactiveEvaluation = disableProactiveEvaluation;
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectHmd(IHmd hand)
		{
			this._hmd = (hand as Object);
			this.Hmd = hand;
		}

		public void InjectDisableProactiveEvaluation(bool disabled)
		{
			this._disableProactiveEvaluation = disabled;
		}

		[Obsolete("Use SetTimeProvider()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		[Header("Advanced Settings")]
		[SerializeField]
		[Tooltip("If true, disables proactive evaluation of any TransformFeature that has been queried at least once. This will force lazy-evaluation of state within calls to IsStateActive, which means you must do so each frame to avoid missing transitions between states.")]
		private bool _disableProactiveEvaluation;

		private Func<float> _timeProvider = () => Time.time;

		private TransformJointData _jointData = new TransformJointData();

		private TransformFeatureStateCollection _transformFeatureStateCollection;

		protected bool _started;
	}
}
