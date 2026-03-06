using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ArcTubeVisual : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.InitializeVisuals();
			this.EndStart(ref this._started);
		}

		private void InitializeVisuals()
		{
			TubePoint[] points = this.InitializeSegment(new Vector2(this._minAngle, this._maxAngle));
			this._tubeRenderer.RenderTube(points, Space.Self);
		}

		private TubePoint[] InitializeSegment(Vector2 minMaxAngle)
		{
			float x = minMaxAngle.x;
			int num = Mathf.RoundToInt(Mathf.Repeat(minMaxAngle.y - x, 360f) / 1f);
			TubePoint[] array = new TubePoint[num];
			float num2 = 1f / (float)num;
			for (int i = 0; i < num; i++)
			{
				Quaternion quaternion = Quaternion.AngleAxis((float)(-(float)i) * 1f - x, Vector3.up);
				array[i] = new TubePoint
				{
					position = quaternion * Vector3.forward * this._radius,
					rotation = quaternion * ArcTubeVisual._rotationCorrectionLeft,
					relativeLength = (float)i * num2
				};
			}
			return array;
		}

		public void InjectAllArcTubeVisual(TubeRenderer tubeRenderer, float radius, float minAngle, float maxAngle)
		{
			this.InjectTubeRenderer(tubeRenderer);
			this.InjectRadius(radius);
			this.InjectMinAngle(minAngle);
			this.InjectMaxAngle(maxAngle);
		}

		public void InjectTubeRenderer(TubeRenderer tubeRenderer)
		{
			this._tubeRenderer = tubeRenderer;
		}

		public void InjectRadius(float radius)
		{
			this._radius = radius;
		}

		public void InjectMinAngle(float minAngle)
		{
			this._minAngle = minAngle;
		}

		public void InjectMaxAngle(float maxAngle)
		{
			this._maxAngle = maxAngle;
		}

		[Header("Visual renderers")]
		[SerializeField]
		private TubeRenderer _tubeRenderer;

		[Header("Visual parameters")]
		[SerializeField]
		private float _radius = 0.07f;

		[SerializeField]
		private float _minAngle;

		[SerializeField]
		private float _maxAngle = 45f;

		private const float _degreesPerSegment = 1f;

		private static readonly Quaternion _rotationCorrectionLeft = Quaternion.Euler(0f, -90f, 0f);

		protected bool _started;
	}
}
