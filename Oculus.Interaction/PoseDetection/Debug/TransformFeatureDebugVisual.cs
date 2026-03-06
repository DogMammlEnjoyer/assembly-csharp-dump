using System;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class TransformFeatureDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this._material = this._target.material;
			this._material.color = (this._lastActiveValue ? this._activeColor : this._normalColor);
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		public void Initialize(Handedness handedness, TransformFeatureConfig targetConfig, TransformFeatureStateProvider transformFeatureStateProvider, TransformRecognizerActiveState transformActiveState)
		{
			this._handedness = handedness;
			this._initialized = true;
			this._transformFeatureStateProvider = transformFeatureStateProvider;
			this._transformRecognizerActiveState = transformActiveState;
			this._targetConfig = targetConfig;
		}

		protected virtual void Update()
		{
			if (!this._initialized)
			{
				return;
			}
			bool flag = false;
			TransformFeature feature = this._targetConfig.Feature;
			string text;
			if (this._transformFeatureStateProvider.GetCurrentState(this._transformRecognizerActiveState.TransformConfig, feature, out text))
			{
				float? featureValue = this._transformFeatureStateProvider.GetFeatureValue(this._transformRecognizerActiveState.TransformConfig, feature);
				flag = this._transformFeatureStateProvider.IsStateActive(this._transformRecognizerActiveState.TransformConfig, feature, this._targetConfig.Mode, this._targetConfig.State);
				string text2 = (featureValue != null) ? featureValue.Value.ToString("F2") : "--";
				this._targetText.text = string.Concat(new string[]
				{
					string.Format("{0}\n", feature),
					text,
					" (",
					text2,
					")"
				});
			}
			else
			{
				this._targetText.text = string.Format("{0}\n", feature);
			}
			if (flag != this._lastActiveValue)
			{
				this._material.color = (flag ? this._activeColor : this._normalColor);
				this._lastActiveValue = flag;
			}
		}

		[SerializeField]
		private Renderer _target;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _activeColor = Color.green;

		[SerializeField]
		private TextMeshPro _targetText;

		private TransformFeatureStateProvider _transformFeatureStateProvider;

		private TransformRecognizerActiveState _transformRecognizerActiveState;

		private Material _material;

		private bool _lastActiveValue;

		private TransformFeatureConfig _targetConfig;

		private bool _initialized;

		private Handedness _handedness;
	}
}
