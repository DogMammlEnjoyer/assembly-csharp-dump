using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class FingerFeatureSkeletalDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.UpdateFeatureActiveValueAndVisual(false);
		}

		private void UpdateFeatureActiveValueAndVisual(bool newValue)
		{
			Color color = newValue ? this._activeColor : this._normalColor;
			this._lineRenderer.startColor = color;
			this._lineRenderer.endColor = color;
			this._lastFeatureActiveValue = newValue;
		}

		public void Initialize(IHand hand, HandFinger finger, ShapeRecognizer.FingerFeatureConfig fingerFeatureConfig)
		{
			this._hand = hand;
			this._initialized = true;
			FingerShapes valueProvider = this._fingerFeatureStateProvider.GetValueProvider(finger);
			this._jointsCovered = valueProvider.GetJointsAffected(finger, fingerFeatureConfig.Feature);
			this._finger = finger;
			this._fingerFeatureConfig = fingerFeatureConfig;
			this._initializedPositions = false;
		}

		protected virtual void Update()
		{
			if (!this._initialized || !this._hand.IsTrackedDataValid)
			{
				this.ToggleLineRendererEnableState(false);
				return;
			}
			this.ToggleLineRendererEnableState(true);
			this.UpdateDebugSkeletonLineRendererJoints();
			this.UpdateFeatureActiveValue();
		}

		private void ToggleLineRendererEnableState(bool enableState)
		{
			if (this._lineRenderer.enabled == enableState)
			{
				return;
			}
			this._lineRenderer.enabled = enableState;
		}

		private void UpdateDebugSkeletonLineRendererJoints()
		{
			if (!this._initializedPositions)
			{
				this._lineRenderer.positionCount = this._jointsCovered.Count;
				this._initializedPositions = true;
			}
			if (Mathf.Abs(this._lineRenderer.startWidth - this._lineWidth) > Mathf.Epsilon)
			{
				this._lineRenderer.startWidth = this._lineWidth;
				this._lineRenderer.endWidth = this._lineWidth;
			}
			int count = this._jointsCovered.Count;
			for (int i = 0; i < count; i++)
			{
				Pose pose;
				if (this._hand.GetJointPose(this._jointsCovered[i], out pose))
				{
					this._lineRenderer.SetPosition(i, pose.position);
				}
			}
		}

		private void UpdateFeatureActiveValue()
		{
			bool flag = this._fingerFeatureStateProvider.IsStateActive(this._finger, this._fingerFeatureConfig.Feature, this._fingerFeatureConfig.Mode, this._fingerFeatureConfig.State);
			if (flag != this._lastFeatureActiveValue)
			{
				this.UpdateFeatureActiveValueAndVisual(flag);
			}
		}

		[SerializeField]
		private FingerFeatureStateProvider _fingerFeatureStateProvider;

		[SerializeField]
		private LineRenderer _lineRenderer;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _activeColor = Color.green;

		[SerializeField]
		private float _lineWidth = 0.005f;

		private IHand _hand;

		private bool _lastFeatureActiveValue;

		private IReadOnlyList<HandJointId> _jointsCovered;

		private HandFinger _finger;

		private ShapeRecognizer.FingerFeatureConfig _fingerFeatureConfig;

		private bool _initializedPositions;

		private bool _initialized;
	}
}
