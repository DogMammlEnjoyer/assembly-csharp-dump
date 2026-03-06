using System;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction
{
	public class FingerFeatureStateProviderRef : MonoBehaviour, IFingerFeatureStateProvider
	{
		public IFingerFeatureStateProvider FingerFeatureStateProvider { get; private set; }

		protected virtual void Awake()
		{
			this.FingerFeatureStateProvider = (this._fingerFeatureStateProvider as IFingerFeatureStateProvider);
		}

		protected virtual void Start()
		{
		}

		public bool GetCurrentState(HandFinger finger, FingerFeature fingerFeature, out string currentState)
		{
			return this.FingerFeatureStateProvider.GetCurrentState(finger, fingerFeature, out currentState);
		}

		public bool IsStateActive(HandFinger finger, FingerFeature feature, FeatureStateActiveMode mode, string stateId)
		{
			return this.FingerFeatureStateProvider.IsStateActive(finger, feature, mode, stateId);
		}

		public float? GetFeatureValue(HandFinger finger, FingerFeature fingerFeature)
		{
			return this.FingerFeatureStateProvider.GetFeatureValue(finger, fingerFeature);
		}

		public void InjectAllFingerFeatureStateProviderRef(IFingerFeatureStateProvider fingerFeatureStateProvider)
		{
			this.InjectFingerFeatureStateProvider(fingerFeatureStateProvider);
		}

		public void InjectFingerFeatureStateProvider(IFingerFeatureStateProvider fingerFeatureStateProvider)
		{
			this._fingerFeatureStateProvider = (fingerFeatureStateProvider as Object);
			this.FingerFeatureStateProvider = fingerFeatureStateProvider;
		}

		[SerializeField]
		[Interface(typeof(IFingerFeatureStateProvider), new Type[]
		{

		})]
		private Object _fingerFeatureStateProvider;
	}
}
