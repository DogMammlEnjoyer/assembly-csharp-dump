using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection.Debug
{
	public class JointVelocityDebugVisual : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this._lineRenderers = new List<LineRenderer>();
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.ResetLines();
			}
		}

		protected virtual void Update()
		{
			this.ResetLines();
			foreach (JointVelocityActiveState.JointVelocityFeatureConfig jointVelocityFeatureConfig in this._jointVelocity.FeatureConfigs)
			{
				Pose pose;
				JointVelocityActiveState.JointVelocityFeatureState jointVelocityFeatureState;
				if (this._jointVelocity.Hand.GetJointPose(jointVelocityFeatureConfig.Feature, out pose) && this._jointVelocity.FeatureStates.TryGetValue(jointVelocityFeatureConfig, out jointVelocityFeatureState))
				{
					this.DrawDebugLine(pose.position, jointVelocityFeatureState.TargetVector, jointVelocityFeatureState.Amount);
				}
			}
		}

		private void DrawDebugLine(Vector3 jointPos, Vector3 direction, float amount)
		{
			Vector3 b = direction.normalized * this._rendererLineLength;
			if (amount >= 1f)
			{
				this.AddLine(jointPos, jointPos + b, Color.green);
				return;
			}
			Vector3 vector = Vector3.Lerp(jointPos, jointPos + b, amount);
			this.AddLine(jointPos, vector, Color.yellow);
			this.AddLine(vector, jointPos + b, Color.red);
		}

		private void ResetLines()
		{
			foreach (LineRenderer lineRenderer in this._lineRenderers)
			{
				if (lineRenderer != null)
				{
					lineRenderer.enabled = false;
				}
			}
			this._enabledRendererCount = 0;
		}

		private void AddLine(Vector3 start, Vector3 end, Color color)
		{
			LineRenderer lineRenderer;
			if (this._enabledRendererCount == this._lineRenderers.Count)
			{
				lineRenderer = new GameObject().AddComponent<LineRenderer>();
				lineRenderer.startWidth = this._rendererLineWidth;
				lineRenderer.endWidth = this._rendererLineWidth;
				lineRenderer.positionCount = 2;
				lineRenderer.material = this._lineRendererMaterial;
				this._lineRenderers.Add(lineRenderer);
			}
			else
			{
				lineRenderer = this._lineRenderers[this._enabledRendererCount];
			}
			this._enabledRendererCount++;
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, end);
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
		}

		[SerializeField]
		private JointVelocityActiveState _jointVelocity;

		[SerializeField]
		private Material _lineRendererMaterial;

		[SerializeField]
		private float _rendererLineWidth = 0.005f;

		[SerializeField]
		private float _rendererLineLength = 0.1f;

		private List<LineRenderer> _lineRenderers;

		private int _enabledRendererCount;

		protected bool _started;
	}
}
