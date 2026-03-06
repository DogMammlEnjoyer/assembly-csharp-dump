using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class HandShapeSkeletalDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			foreach (var <>f__AnonymousType in from s in this.AllFeatureStates()
			group s by s.Item1 into @group
			select new
			{
				HandFinger = @group.Key,
				FingerFeatures = @group.SelectMany((ValueTuple<HandFinger, IReadOnlyList<ShapeRecognizer.FingerFeatureConfig>> item) => item.Item2)
			})
			{
				foreach (ShapeRecognizer.FingerFeatureConfig fingerFeatureConfig in <>f__AnonymousType.FingerFeatures)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this._fingerFeatureDebugVisualPrefab);
					gameObject.GetComponent<FingerFeatureSkeletalDebugVisual>().Initialize(this._shapeRecognizerActiveState.Hand, <>f__AnonymousType.HandFinger, fingerFeatureConfig);
					Transform transform = gameObject.transform;
					transform.parent = base.transform;
					transform.localScale = Vector3.one;
					transform.localRotation = Quaternion.identity;
					transform.localPosition = Vector3.zero;
				}
			}
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

		[SerializeField]
		private ShapeRecognizerActiveState _shapeRecognizerActiveState;

		[SerializeField]
		private GameObject _fingerFeatureDebugVisualPrefab;
	}
}
