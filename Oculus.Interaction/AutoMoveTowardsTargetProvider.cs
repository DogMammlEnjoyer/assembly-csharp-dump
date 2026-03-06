using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class AutoMoveTowardsTargetProvider : MonoBehaviour, IMovementProvider
	{
		public PoseTravelData TravellingData
		{
			get
			{
				return this._travellingData;
			}
			set
			{
				this._travellingData = value;
			}
		}

		public IPointableElement PointableElement { get; private set; }

		protected virtual void Awake()
		{
			this.PointableElement = (this._pointableElement as IPointableElement);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		private void LateUpdate()
		{
			for (int i = this._movers.Count - 1; i >= 0; i--)
			{
				AutoMoveTowardsTarget autoMoveTowardsTarget = this._movers[i];
				if (autoMoveTowardsTarget.Aborting)
				{
					autoMoveTowardsTarget.Tick();
					if (autoMoveTowardsTarget.Stopped)
					{
						this._movers.Remove(autoMoveTowardsTarget);
					}
				}
			}
		}

		public IMovement CreateMovement()
		{
			AutoMoveTowardsTarget autoMoveTowardsTarget = new AutoMoveTowardsTarget(this._travellingData, this.PointableElement);
			autoMoveTowardsTarget.WhenAborted = (Action<AutoMoveTowardsTarget>)Delegate.Combine(autoMoveTowardsTarget.WhenAborted, new Action<AutoMoveTowardsTarget>(this.HandleAborted));
			return autoMoveTowardsTarget;
		}

		private void HandleAborted(AutoMoveTowardsTarget mover)
		{
			mover.WhenAborted = (Action<AutoMoveTowardsTarget>)Delegate.Remove(mover.WhenAborted, new Action<AutoMoveTowardsTarget>(this.HandleAborted));
			this._movers.Add(mover);
		}

		public void InjectAllAutoMoveTowardsTargetProvider(IPointableElement pointableElement)
		{
			this.InjectPointableElement(pointableElement);
		}

		public void InjectPointableElement(IPointableElement pointableElement)
		{
			this.PointableElement = pointableElement;
			this._pointableElement = (pointableElement as Object);
		}

		[SerializeField]
		private PoseTravelData _travellingData = PoseTravelData.DEFAULT;

		[SerializeField]
		[Interface(typeof(IPointableElement), new Type[]
		{

		})]
		private Object _pointableElement;

		private bool _started;

		public List<AutoMoveTowardsTarget> _movers = new List<AutoMoveTowardsTarget>();
	}
}
