using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class DistantInteractionLineRendererVisual : DistantInteractionLineVisual
	{
		protected override void Start()
		{
			base.Start();
			this._lineRenderer.positionCount = base.NumLinePoints;
		}

		protected override void RenderLine(Vector3[] linePoints)
		{
			this._lineRenderer.SetPositions(linePoints);
			this._lineRenderer.enabled = true;
		}

		protected override void HideLine()
		{
			this._lineRenderer.enabled = false;
		}

		public void InjectAllDistantInteractionLineRendererVisual(IDistanceInteractor interactor, LineRenderer lineRenderer)
		{
			base.InjectDistanceInteractor(interactor);
			this.InjectLineRenderer(lineRenderer);
		}

		public void InjectLineRenderer(LineRenderer lineRenderer)
		{
			this._lineRenderer = lineRenderer;
		}

		[SerializeField]
		private LineRenderer _lineRenderer;
	}
}
