using System;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class TransformRecognizerDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this._material = this._target.material;
			this._material.color = (this._lastActiveValue ? this._activeColor : this._normalColor);
			if (this._debugVisualParent == null)
			{
				this._debugVisualParent = base.transform;
			}
		}

		protected virtual void Start()
		{
			Vector3 vector = Vector3.zero;
			string text = "";
			foreach (TransformFeatureConfig transformFeatureConfig in this._transformRecognizerActiveState.FeatureConfigs)
			{
				TransformFeatureDebugVisual component = Object.Instantiate<GameObject>(this._transformFeatureDebugVisualPrefab, this._debugVisualParent).GetComponent<TransformFeatureDebugVisual>();
				component.Initialize(this._transformRecognizerActiveState.Hand.Handedness, transformFeatureConfig, this._transformFeatureStateProvider, this._transformRecognizerActiveState);
				Transform transform = component.transform;
				transform.localScale = this._featureDebugLocalScale;
				transform.localRotation = Quaternion.identity;
				transform.localPosition = vector;
				vector += this._featureSpacingVec;
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n  ";
				}
				text += string.Format("{0} {1} ({2})", transformFeatureConfig.Mode, transformFeatureConfig.State, this._transformRecognizerActiveState.Hand.Handedness);
			}
			this._targetText.text = (text ?? "");
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		private bool AllActive()
		{
			return this._transformRecognizerActiveState.Active;
		}

		protected virtual void Update()
		{
			bool flag = this.AllActive();
			if (this._lastActiveValue != flag)
			{
				this._material.color = (flag ? this._activeColor : this._normalColor);
				this._lastActiveValue = flag;
			}
		}

		[SerializeField]
		private Hand _hand;

		[SerializeField]
		private TransformFeatureStateProvider _transformFeatureStateProvider;

		[SerializeField]
		private TransformRecognizerActiveState _transformRecognizerActiveState;

		[SerializeField]
		private Renderer _target;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _activeColor = Color.green;

		[SerializeField]
		private GameObject _transformFeatureDebugVisualPrefab;

		[SerializeField]
		private Transform _debugVisualParent;

		[SerializeField]
		private Vector3 _featureSpacingVec = new Vector3(1f, 0f, 0f);

		[SerializeField]
		private Vector3 _featureDebugLocalScale = new Vector3(0.3f, 0.3f, 0.3f);

		[SerializeField]
		private TextMeshPro _targetText;

		private Material _material;

		private bool _lastActiveValue;
	}
}
