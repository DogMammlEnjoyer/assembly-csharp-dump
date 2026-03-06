using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class AutoMoveTowardsTarget : IMovement
	{
		public Pose Pose
		{
			get
			{
				return this._tween.Pose;
			}
		}

		public bool Stopped
		{
			get
			{
				return this._tween == null || this._tween.Stopped;
			}
		}

		public bool Aborting { get; private set; }

		public int Identifier
		{
			get
			{
				return this._identifier.ID;
			}
		}

		public AutoMoveTowardsTarget(PoseTravelData travellingData, IPointableElement pointableElement)
		{
			this._identifier = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
			this._travellingData = travellingData;
			this._pointableElement = pointableElement;
		}

		public void MoveTo(Pose target)
		{
			this.AbortSelfAligment();
			this._target = target;
			this._tween = this._travellingData.CreateTween(this._source, target);
			if (!this._eventRegistered)
			{
				this._pointableElement.WhenPointerEventRaised += this.HandlePointerEventRaised;
				this._eventRegistered = true;
			}
		}

		public void UpdateTarget(Pose target)
		{
			this._target = target;
			this._tween.UpdateTarget(this._target);
		}

		public void StopAndSetPose(Pose pose)
		{
			if (this._eventRegistered)
			{
				this._pointableElement.WhenPointerEventRaised -= this.HandlePointerEventRaised;
				this._eventRegistered = false;
			}
			this._source = pose;
			if (this._tween != null && !this._tween.Stopped)
			{
				this.GeneratePointerEvent(PointerEventType.Hover);
				this.GeneratePointerEvent(PointerEventType.Select);
				this.Aborting = true;
				this.WhenAborted(this);
			}
		}

		public void Tick()
		{
			this._tween.Tick();
			if (this.Aborting)
			{
				this.GeneratePointerEvent(PointerEventType.Move);
				if (this._tween.Stopped)
				{
					this.AbortSelfAligment();
				}
			}
		}

		private void HandlePointerEventRaised(PointerEvent evt)
		{
			if (evt.Type == PointerEventType.Select || evt.Type == PointerEventType.Unselect)
			{
				this.AbortSelfAligment();
			}
		}

		private void AbortSelfAligment()
		{
			if (this.Aborting)
			{
				this.Aborting = false;
				this.GeneratePointerEvent(PointerEventType.Unselect);
				this.GeneratePointerEvent(PointerEventType.Unhover);
			}
		}

		private void GeneratePointerEvent(PointerEventType pointerEventType)
		{
			PointerEvent evt = new PointerEvent(this.Identifier, pointerEventType, this.Pose, null);
			this._pointableElement.ProcessPointerEvent(evt);
		}

		private PoseTravelData _travellingData;

		private IPointableElement _pointableElement;

		public Action<AutoMoveTowardsTarget> WhenAborted = delegate(AutoMoveTowardsTarget <p0>)
		{
		};

		private UniqueIdentifier _identifier;

		private Tween _tween;

		private Pose _target;

		private Pose _source;

		private bool _eventRegistered;
	}
}
