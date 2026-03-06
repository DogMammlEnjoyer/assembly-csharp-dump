using System;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class TransformFeatureVectorDebugParentVisual : MonoBehaviour
	{
		public void GetTransformFeatureVectorAndWristPos(TransformFeature feature, bool isHandVector, ref Vector3? featureVec, ref Vector3? wristPos)
		{
			this._transformRecognizerActiveState.GetFeatureVectorAndWristPos(feature, isHandVector, ref featureVec, ref wristPos);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			foreach (TransformFeatureConfig transformFeatureConfig in this._transformRecognizerActiveState.FeatureConfigs)
			{
				TransformFeature feature = transformFeatureConfig.Feature;
				this.CreateVectorDebugView(feature, false);
				this.CreateVectorDebugView(feature, true);
			}
		}

		private void CreateVectorDebugView(TransformFeature feature, bool trackingHandVector)
		{
			TransformFeatureVectorDebugVisual component = Object.Instantiate<GameObject>(this._vectorVisualPrefab, base.transform).GetComponent<TransformFeatureVectorDebugVisual>();
			component.Initialize(feature, trackingHandVector, this, trackingHandVector ? Color.blue : Color.black);
			Transform transform = component.transform;
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.zero;
		}

		[SerializeField]
		private TransformRecognizerActiveState _transformRecognizerActiveState;

		[SerializeField]
		private GameObject _vectorVisualPrefab;
	}
}
