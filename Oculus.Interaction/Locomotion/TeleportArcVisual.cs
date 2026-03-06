using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TeleportArcVisual : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._interactor.WhenPostprocessed += this.HandleInteractorPostProcessed;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._interactor.WhenPostprocessed -= this.HandleInteractorPostProcessed;
			}
		}

		protected virtual void HandleInteractorPostProcessed()
		{
			int pointsCount = this._interactor.TeleportArc.PointsCount;
			if (this._positions == null || this._positions.Length != pointsCount)
			{
				this._positions = new Vector3[pointsCount];
				this._arcRenderer.positionCount = pointsCount;
			}
			for (int i = 0; i < pointsCount; i++)
			{
				this._positions[i] = this._interactor.TeleportArc.PointAtIndex(i);
			}
			this._arcRenderer.SetPositions(this._positions);
		}

		public void InjectAllTeleportArcVisual(TeleportInteractor interactor, LineRenderer arcRenderer)
		{
			this.InjectInteractor(interactor);
			this.InjectArcRenderer(arcRenderer);
		}

		public void InjectInteractor(TeleportInteractor interactor)
		{
			this._interactor = interactor;
		}

		public void InjectArcRenderer(LineRenderer arcRenderer)
		{
			this._arcRenderer = arcRenderer;
		}

		[SerializeField]
		private TeleportInteractor _interactor;

		[SerializeField]
		private LineRenderer _arcRenderer;

		private Vector3[] _positions;

		protected bool _started;
	}
}
