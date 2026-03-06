using System;
using UnityEngine;

namespace Drawing.Examples
{
	public class GizmoCharacterExample : MonoBehaviourGizmos
	{
		private void Start()
		{
			this.seed = Random.value * 1000f;
			this.startPosition = base.transform.position;
		}

		private Vector3 GetSmoothRandomVelocity(float time, Vector3 position)
		{
			float num = time * this.movementNoiseScale + this.seed;
			float x = 2f * Mathf.PerlinNoise(num, num + 5341.2314f) - 1f;
			float z = 2f * Mathf.PerlinNoise(num + 92.9842f, -num + 231.85146f) - 1f;
			Vector3 vector = new Vector3(x, 0f, z);
			vector += (this.startPosition - position) * this.startPointAttractionStrength;
			vector.y = 0f;
			return vector;
		}

		private void PlotFuturePath(float time, Vector3 position)
		{
			float num = 0.05f;
			for (int i = 0; i < this.futurePathPlotSteps; i++)
			{
				Vector3 smoothRandomVelocity = this.GetSmoothRandomVelocity(time + (float)i * num, position);
				int num2 = i - this.plotStartStep;
				if (num2 >= 0 && num2 % this.plotEveryNSteps == 0)
				{
					Draw.Arrowhead(position, smoothRandomVelocity, 0.1f, this.gizmoColor);
				}
				position += smoothRandomVelocity.normalized * num;
			}
		}

		private void Update()
		{
			this.PlotFuturePath(Time.time, base.transform.position);
			Vector3 smoothRandomVelocity = this.GetSmoothRandomVelocity(Time.time, base.transform.position);
			base.transform.rotation = Quaternion.LookRotation(smoothRandomVelocity);
			base.transform.position += base.transform.forward * Time.deltaTime;
		}

		public override void DrawGizmos()
		{
			using (Draw.InLocalSpace(base.transform))
			{
				Draw.WireCylinder(Vector3.zero, Vector3.up, 2f, 0.5f, this.gizmoColor);
				Draw.ArrowheadArc(Vector3.zero, Vector3.forward, 0.55f, this.gizmoColor);
				Draw.Label2D(Vector3.zero, base.gameObject.name, 14f, LabelAlignment.TopCenter.withPixelOffset(0f, -20f), this.gizmoColor2);
			}
		}

		public Color gizmoColor = new Color(1f, 0.34509805f, 0.33333334f);

		public Color gizmoColor2 = new Color(0.30980393f, 0.8f, 0.92941177f);

		public float movementNoiseScale = 0.2f;

		public float startPointAttractionStrength = 0.05f;

		public int futurePathPlotSteps = 100;

		public int plotStartStep = 10;

		public int plotEveryNSteps = 10;

		private float seed;

		private Vector3 startPosition;
	}
}
