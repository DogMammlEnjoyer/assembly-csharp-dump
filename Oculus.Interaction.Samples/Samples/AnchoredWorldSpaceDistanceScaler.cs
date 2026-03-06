using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class AnchoredWorldSpaceDistanceScaler : MonoBehaviour
	{
		private void Start()
		{
			this._parentAnchorOffset = this._parentAnchor.InverseTransformPoint(this._localAnchor.position);
			this._originalLocalScale = base.transform.localScale;
			this._originalParentLocalScale = base.transform.parent.localScale;
			this._originalCombinedScale = Vector3.Scale(this._originalParentLocalScale, this._originalLocalScale);
		}

		private void LateUpdate()
		{
			Vector3 position = this._parentAnchor.TransformPoint(this._parentAnchorOffset);
			Vector3 vector = base.transform.InverseTransformPoint(position);
			Vector3 localScale = base.transform.localScale;
			localScale.Scale(new Vector3((Mathf.Abs(this._localAnchor.localPosition.x) < Mathf.Epsilon) ? 1f : (vector.x / this._localAnchor.localPosition.x), (Mathf.Abs(this._localAnchor.localPosition.y) < Mathf.Epsilon) ? 1f : (vector.y / this._localAnchor.localPosition.y), (Mathf.Abs(this._localAnchor.localPosition.z) < Mathf.Epsilon) ? 1f : (vector.z / this._localAnchor.localPosition.z)));
			if (this._scalingMode == AnchoredWorldSpaceDistanceScaler.ScalingMode.ThreeDimensional)
			{
				Vector3 vector2 = Vector3.Scale(base.transform.parent.localScale, localScale);
				if (vector2.x / vector2.y > this._originalCombinedScale.x / this._originalCombinedScale.y)
				{
					float num = vector2.y / this._originalCombinedScale.y;
					localScale.x = this._originalCombinedScale.x * num / base.transform.parent.localScale.x;
					localScale.z = this._originalCombinedScale.z * num / base.transform.parent.localScale.z;
				}
				else
				{
					float num2 = vector2.x / this._originalCombinedScale.x;
					localScale.y = this._originalCombinedScale.y * num2 / base.transform.parent.localScale.y;
					localScale.z = this._originalCombinedScale.z * num2 / base.transform.parent.localScale.z;
				}
			}
			else
			{
				localScale.z = this._originalParentLocalScale.z * this._originalLocalScale.z / base.transform.parent.localScale.z;
			}
			base.transform.localScale = localScale;
		}

		[SerializeField]
		private Transform _parentAnchor;

		[SerializeField]
		private Transform _localAnchor;

		[SerializeField]
		[Tooltip("Choose whether content should be scaled as two- or three-dimensional")]
		private AnchoredWorldSpaceDistanceScaler.ScalingMode _scalingMode;

		private Vector3 _parentAnchorOffset;

		private Vector3 _originalLocalScale;

		private Vector3 _originalParentLocalScale;

		private Vector3 _originalCombinedScale;

		public enum ScalingMode
		{
			TwoDimensional,
			ThreeDimensional
		}
	}
}
