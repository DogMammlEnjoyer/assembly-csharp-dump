using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class TransformFeatureVectorDebugVisual : MonoBehaviour
	{
		public IHand Hand { get; private set; }

		protected virtual void Awake()
		{
			this._lineRenderer.enabled = false;
		}

		public void Initialize(TransformFeature feature, bool trackingHandVector, TransformFeatureVectorDebugParentVisual parent, Color lineColor)
		{
			this._isInitialized = true;
			this._lineRenderer.enabled = true;
			this._lineRenderer.positionCount = 2;
			this._lineRenderer.startColor = lineColor;
			this._lineRenderer.endColor = lineColor;
			this._feature = feature;
			this._trackingHandVector = trackingHandVector;
			this._parent = parent;
		}

		protected virtual void Update()
		{
			if (!this._isInitialized)
			{
				return;
			}
			Vector3? vector = null;
			Vector3? vector2 = null;
			this._parent.GetTransformFeatureVectorAndWristPos(this._feature, this._trackingHandVector, ref vector, ref vector2);
			if (vector == null || vector2 == null)
			{
				if (this._lineRenderer.enabled)
				{
					this._lineRenderer.enabled = false;
				}
				return;
			}
			if (!this._lineRenderer.enabled)
			{
				this._lineRenderer.enabled = true;
			}
			if (Mathf.Abs(this._lineRenderer.startWidth - this._lineWidth) > Mathf.Epsilon)
			{
				this._lineRenderer.startWidth = this._lineWidth;
				this._lineRenderer.endWidth = this._lineWidth;
			}
			this._lineRenderer.SetPosition(0, vector2.Value);
			this._lineRenderer.SetPosition(1, vector2.Value + this._lineScale * vector.Value);
		}

		[SerializeField]
		private LineRenderer _lineRenderer;

		[SerializeField]
		private float _lineWidth = 0.005f;

		[SerializeField]
		private float _lineScale = 0.1f;

		private bool _isInitialized;

		private TransformFeature _feature;

		private TransformFeatureVectorDebugParentVisual _parent;

		private bool _trackingHandVector;
	}
}
