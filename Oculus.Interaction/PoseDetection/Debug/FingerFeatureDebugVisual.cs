using System;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class FingerFeatureDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this._material = this._target.material;
			this._material.color = (this._lastActiveValue ? this._activeColor : this._normalColor);
		}

		protected virtual void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		public void Initialize(HandFinger handFinger, ShapeRecognizer.FingerFeatureConfig config, IFingerFeatureStateProvider fingerFeatureState)
		{
			this._initialized = true;
			this._handFinger = handFinger;
			this._featureConfig = config;
			this._fingerFeatureState = fingerFeatureState;
		}

		protected virtual void Update()
		{
			if (!this._initialized)
			{
				return;
			}
			FingerFeature feature = this._featureConfig.Feature;
			bool flag = false;
			string text;
			if (this._fingerFeatureState.GetCurrentState(this._handFinger, feature, out text))
			{
				float? featureValue = this._fingerFeatureState.GetFeatureValue(this._handFinger, feature);
				flag = this._fingerFeatureState.IsStateActive(this._handFinger, feature, this._featureConfig.Mode, this._featureConfig.State);
				string text2 = (featureValue != null) ? featureValue.Value.ToString("F2") : "--";
				this._targetText.text = string.Concat(new string[]
				{
					string.Format("{0} {1}", this._handFinger, feature),
					text,
					" (",
					text2,
					")"
				});
			}
			else
			{
				this._targetText.text = string.Format("{0} {1}\n", this._handFinger, feature);
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

		private IFingerFeatureStateProvider _fingerFeatureState;

		private Material _material;

		private bool _lastActiveValue;

		private HandFinger _handFinger;

		private ShapeRecognizer.FingerFeatureConfig _featureConfig;

		private bool _initialized;
	}
}
