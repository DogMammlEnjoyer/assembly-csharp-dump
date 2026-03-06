using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class TransformRecognizerActiveState : MonoBehaviour, IActiveState
	{
		public IHand Hand { get; private set; }

		public IReadOnlyList<TransformFeatureConfig> FeatureConfigs
		{
			get
			{
				return this._transformFeatureConfigs.Values;
			}
		}

		public TransformConfig TransformConfig
		{
			get
			{
				return this._transformConfig;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.TransformFeatureStateProvider = (this._transformFeatureStateProvider as ITransformFeatureStateProvider);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._transformConfig.InstanceId = base.GetInstanceID();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.TransformFeatureStateProvider.RegisterConfig(this._transformConfig);
				this.InitStateProvider();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.TransformFeatureStateProvider.UnRegisterConfig(this._transformConfig);
			}
		}

		private void InitStateProvider()
		{
			foreach (TransformFeatureConfig transformFeatureConfig in this.FeatureConfigs)
			{
				string text;
				this.TransformFeatureStateProvider.GetCurrentState(this._transformConfig, transformFeatureConfig.Feature, out text);
			}
		}

		public void GetFeatureVectorAndWristPos(TransformFeature feature, bool isHandVector, ref Vector3? featureVec, ref Vector3? wristPos)
		{
			this.TransformFeatureStateProvider.GetFeatureVectorAndWristPos(this.TransformConfig, feature, isHandVector, ref featureVec, ref wristPos);
		}

		public bool Active
		{
			get
			{
				if (!base.isActiveAndEnabled)
				{
					return false;
				}
				foreach (TransformFeatureConfig transformFeatureConfig in this.FeatureConfigs)
				{
					if (!this.TransformFeatureStateProvider.IsStateActive(this._transformConfig, transformFeatureConfig.Feature, transformFeatureConfig.Mode, transformFeatureConfig.State))
					{
						return false;
					}
				}
				return true;
			}
		}

		public void InjectAllTransformRecognizerActiveState(IHand hand, ITransformFeatureStateProvider transformFeatureStateProvider, TransformFeatureConfigList transformFeatureList, TransformConfig transformConfig)
		{
			this.InjectHand(hand);
			this.InjectTransformFeatureStateProvider(transformFeatureStateProvider);
			this.InjectTransformFeatureList(transformFeatureList);
			this.InjectTransformConfig(transformConfig);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectTransformFeatureStateProvider(ITransformFeatureStateProvider transformFeatureStateProvider)
		{
			this.TransformFeatureStateProvider = transformFeatureStateProvider;
			this._transformFeatureStateProvider = (transformFeatureStateProvider as Object);
		}

		public void InjectTransformFeatureList(TransformFeatureConfigList transformFeatureList)
		{
			this._transformFeatureConfigs = transformFeatureList;
		}

		public void InjectTransformConfig(TransformConfig transformConfig)
		{
			this._transformConfig = transformConfig;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[Interface(typeof(ITransformFeatureStateProvider), new Type[]
		{

		})]
		private Object _transformFeatureStateProvider;

		protected ITransformFeatureStateProvider TransformFeatureStateProvider;

		[SerializeField]
		private TransformFeatureConfigList _transformFeatureConfigs;

		[SerializeField]
		[Tooltip("State provider uses this to determine the state of features during real time, so edit at runtime at your own risk.")]
		private TransformConfig _transformConfig;

		protected bool _started;
	}
}
