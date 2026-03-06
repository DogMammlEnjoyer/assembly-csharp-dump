using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class FingerFeatureStateProvider : MonoBehaviour, IFingerFeatureStateProvider, ITimeConsumer
	{
		public IHand Hand { get; private set; }

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public static FingerShapes DefaultFingerShapes { get; } = new FingerShapes();

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this._state = new FingerFeatureStateDictionary();
			this._handJointPoses = ReadOnlyHandJointPoses.Empty;
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
				this.ReadStateThresholds();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandDataAvailable;
				this._handJointPoses = ReadOnlyHandJointPoses.Empty;
			}
		}

		private void ReadStateThresholds()
		{
			HandFingerFlags handFingerFlags = HandFingerFlags.None;
			foreach (FingerFeatureStateProvider.FingerStateThresholds fingerStateThresholds in this._fingerStateThresholds)
			{
				handFingerFlags |= HandFingerUtils.ToFlags(fingerStateThresholds.Finger);
				HandFinger finger = fingerStateThresholds.Finger;
				FeatureStateProvider<FingerFeature, string> featureStateProvider = this._state.GetStateProvider(finger);
				if (featureStateProvider == null)
				{
					Func<float> timeProvider = () => this._timeProvider();
					featureStateProvider = new FeatureStateProvider<FingerFeature, string>((FingerFeature feature) => this.GetFeatureValue(finger, feature), (FingerFeature feature) => (int)feature, timeProvider);
					this._state.InitializeFinger(fingerStateThresholds.Finger, featureStateProvider);
				}
				featureStateProvider.InitializeThresholds(fingerStateThresholds.StateThresholds);
			}
		}

		private void HandDataAvailable()
		{
			int currentDataVersion = this.Hand.CurrentDataVersion;
			if (!this.Hand.GetJointPosesFromWrist(out this._handJointPoses))
			{
				return;
			}
			if (!this._disableProactiveEvaluation)
			{
				for (int i = 0; i < 5; i++)
				{
					FeatureStateProvider<FingerFeature, string> stateProvider = this._state.GetStateProvider((HandFinger)i);
					stateProvider.LastUpdatedFrameId = currentDataVersion;
					stateProvider.ReadTouchedFeatureStates();
				}
				return;
			}
			for (int j = 0; j < 5; j++)
			{
				this._state.GetStateProvider((HandFinger)j).LastUpdatedFrameId = currentDataVersion;
			}
		}

		public bool GetCurrentState(HandFinger finger, FingerFeature fingerFeature, out string currentState)
		{
			if (!this.IsDataValid())
			{
				currentState = null;
				return false;
			}
			currentState = this.GetCurrentFingerFeatureState(finger, fingerFeature);
			return currentState != null;
		}

		private string GetCurrentFingerFeatureState(HandFinger finger, FingerFeature fingerFeature)
		{
			return this._state.GetStateProvider(finger).GetCurrentFeatureState(fingerFeature);
		}

		public float? GetFeatureValue(HandFinger finger, FingerFeature fingerFeature)
		{
			if (!this.IsDataValid())
			{
				return null;
			}
			return new float?(this._fingerShapes.GetValue(finger, fingerFeature, this.Hand));
		}

		private bool IsDataValid()
		{
			return this._handJointPoses.Count > 0;
		}

		public FingerShapes GetValueProvider(HandFinger finger)
		{
			return this._fingerShapes;
		}

		public bool IsStateActive(HandFinger finger, FingerFeature feature, FeatureStateActiveMode mode, string stateId)
		{
			string currentFingerFeatureState = this.GetCurrentFingerFeatureState(finger, feature);
			if (mode != FeatureStateActiveMode.Is)
			{
				return mode == FeatureStateActiveMode.IsNot && currentFingerFeatureState != stateId;
			}
			return currentFingerFeatureState == stateId;
		}

		public void InjectAllFingerFeatureStateProvider(IHand hand, List<FingerFeatureStateProvider.FingerStateThresholds> fingerStateThresholds, FingerShapes fingerShapes, bool disableProactiveEvaluation)
		{
			this.InjectHand(hand);
			this.InjectFingerStateThresholds(fingerStateThresholds);
			this.InjectFingerShapes(fingerShapes);
			this.InjectDisableProactiveEvaluation(disableProactiveEvaluation);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectFingerStateThresholds(List<FingerFeatureStateProvider.FingerStateThresholds> fingerStateThresholds)
		{
			this._fingerStateThresholds = fingerStateThresholds;
		}

		public void InjectFingerShapes(FingerShapes fingerShapes)
		{
			this._fingerShapes = fingerShapes;
		}

		public void InjectDisableProactiveEvaluation(bool disableProactiveEvaluation)
		{
			this._disableProactiveEvaluation = disableProactiveEvaluation;
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
		[Tooltip("Data source used to retrieve finger bone rotations.")]
		private Object _hand;

		[SerializeField]
		[Tooltip("Contains state transition threasholds for each finger. Must contain 5 entries (one for each finger). Each finger must exist in the list exactly once.")]
		private List<FingerFeatureStateProvider.FingerStateThresholds> _fingerStateThresholds;

		[Header("Advanced Settings")]
		[SerializeField]
		[Tooltip("If true, disables proactive evaluation of any FingerFeature that has been queried at least once. This will force lazy-evaluation of state within calls to IsStateActive, which means you must call IsStateActive for each feature manually each frame to avoid missing transitions between states.")]
		private bool _disableProactiveEvaluation;

		protected bool _started;

		private FingerFeatureStateDictionary _state;

		private Func<float> _timeProvider = () => Time.time;

		private FingerShapes _fingerShapes = FingerFeatureStateProvider.DefaultFingerShapes;

		private ReadOnlyHandJointPoses _handJointPoses;

		[Serializable]
		public struct FingerStateThresholds
		{
			[Tooltip("Which finger the state thresholds apply to.")]
			public HandFinger Finger;

			[Tooltip("State threshold configuration")]
			public FingerFeatureStateThresholds StateThresholds;
		}
	}
}
