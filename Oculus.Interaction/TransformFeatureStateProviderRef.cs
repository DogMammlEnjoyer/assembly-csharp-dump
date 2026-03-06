using System;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TransformFeatureStateProviderRef : MonoBehaviour, ITransformFeatureStateProvider
	{
		public ITransformFeatureStateProvider TransformFeatureStateProvider { get; private set; }

		protected virtual void Awake()
		{
			this.TransformFeatureStateProvider = (this._transformFeatureStateProvider as ITransformFeatureStateProvider);
		}

		protected virtual void Start()
		{
		}

		public bool IsStateActive(TransformConfig config, TransformFeature feature, FeatureStateActiveMode mode, string stateId)
		{
			return this.TransformFeatureStateProvider.IsStateActive(config, feature, mode, stateId);
		}

		public bool GetCurrentState(TransformConfig config, TransformFeature transformFeature, out string currentState)
		{
			return this.TransformFeatureStateProvider.GetCurrentState(config, transformFeature, out currentState);
		}

		public void RegisterConfig(TransformConfig transformConfig)
		{
			this.TransformFeatureStateProvider.RegisterConfig(transformConfig);
		}

		public void UnRegisterConfig(TransformConfig transformConfig)
		{
			this.TransformFeatureStateProvider.UnRegisterConfig(transformConfig);
		}

		public void GetFeatureVectorAndWristPos(TransformConfig config, TransformFeature transformFeature, bool isHandVector, ref Vector3? featureVec, ref Vector3? wristPos)
		{
			this.TransformFeatureStateProvider.GetFeatureVectorAndWristPos(config, transformFeature, isHandVector, ref featureVec, ref wristPos);
		}

		public void InjectAllTransformFeatureStateProviderRef(ITransformFeatureStateProvider transformFeatureStateProvider)
		{
			this.InjectTransformFeatureStateProvider(transformFeatureStateProvider);
		}

		public void InjectTransformFeatureStateProvider(ITransformFeatureStateProvider transformFeatureStateProvider)
		{
			this._transformFeatureStateProvider = (transformFeatureStateProvider as Object);
			this.TransformFeatureStateProvider = transformFeatureStateProvider;
		}

		[SerializeField]
		[Interface(typeof(ITransformFeatureStateProvider), new Type[]
		{

		})]
		private Object _transformFeatureStateProvider;
	}
}
