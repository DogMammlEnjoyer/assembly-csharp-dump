using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class HandShapeDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.FingerFeatureStateProvider = (this._fingerFeatureStateProvider as IFingerFeatureStateProvider);
			this._material = this._target.material;
			this._material.color = (this._lastActiveValue ? this._activeColor : this._normalColor);
			if (this._fingerFeatureParent == null)
			{
				this._fingerFeatureParent = base.transform;
			}
		}

		protected virtual void Start()
		{
			Vector3 vector = Vector3.zero;
			foreach (var <>f__AnonymousType in from s in this.AllFeatureStates()
			group s by s.Item1 into @group
			select new
			{
				HandFinger = @group.Key,
				FingerFeatures = @group.SelectMany((ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>> item) => item.Item2)
			})
			{
				Vector3 vector2 = vector;
				foreach (ShapeRecognizer.FingerFeatureConfig config in <>f__AnonymousType.FingerFeatures)
				{
					FingerFeatureDebugVisual component = Object.Instantiate<GameObject>(this._fingerFeatureDebugVisualPrefab, this._fingerFeatureParent).GetComponent<FingerFeatureDebugVisual>();
					component.Initialize(<>f__AnonymousType.HandFinger, config, this.FingerFeatureStateProvider);
					Transform transform = component.transform;
					transform.localScale = this._fingerFeatureDebugLocalScale;
					transform.localRotation = Quaternion.identity;
					transform.localPosition = vector2;
					vector2 += this._fingerFeatureSpacingVec;
				}
				vector += this._fingerSpacingVec;
			}
			string text = "";
			foreach (ShapeRecognizer shapeRecognizer in this._shapeRecognizerActiveState.Shapes)
			{
				text += shapeRecognizer.ShapeName;
			}
			this._targetText.text = string.Format("{0} Hand: {1} ", this._shapeRecognizerActiveState.Handedness, text);
		}

		private IEnumerable<ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>>> AllFeatureStates()
		{
			foreach (ShapeRecognizer shapeRecognizer in this._shapeRecognizerActiveState.Shapes)
			{
				foreach (ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>> valueTuple in shapeRecognizer.GetFingerFeatureConfigs())
				{
					yield return valueTuple;
				}
				IEnumerator<ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>>> enumerator2 = null;
			}
			IEnumerator<ShapeRecognizer> enumerator = null;
			yield break;
			yield break;
		}

		protected virtual void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		protected virtual void Update()
		{
			bool active = this._shapeRecognizerActiveState.Active;
			if (this._lastActiveValue != active)
			{
				this._material.color = (active ? this._activeColor : this._normalColor);
				this._lastActiveValue = active;
			}
		}

		[SerializeField]
		[Interface(typeof(IFingerFeatureStateProvider), new Type[]
		{

		})]
		private Object _fingerFeatureStateProvider;

		private IFingerFeatureStateProvider FingerFeatureStateProvider;

		[SerializeField]
		private ShapeRecognizerActiveState _shapeRecognizerActiveState;

		[SerializeField]
		private Renderer _target;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _activeColor = Color.green;

		[SerializeField]
		private GameObject _fingerFeatureDebugVisualPrefab;

		[SerializeField]
		private Transform _fingerFeatureParent;

		[SerializeField]
		private Vector3 _fingerSpacingVec = new Vector3(0f, -1f, 0f);

		[SerializeField]
		private Vector3 _fingerFeatureSpacingVec = new Vector3(1f, 0f, 0f);

		[SerializeField]
		private Vector3 _fingerFeatureDebugLocalScale = new Vector3(0.3f, 0.3f, 0.3f);

		[SerializeField]
		private TextMeshPro _targetText;

		private Material _material;

		private bool _lastActiveValue;
	}
}
