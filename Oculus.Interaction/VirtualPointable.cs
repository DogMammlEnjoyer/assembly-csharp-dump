using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class VirtualPointable : MonoBehaviour, IPointable
	{
		public event Action<PointerEvent> WhenPointerEventRaised = delegate(PointerEvent <p0>)
		{
		};

		protected virtual void Awake()
		{
			this._id = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
		}

		protected virtual void Update()
		{
			if (this._currentlyGrabbing == this._grabFlag)
			{
				if (this._currentlyGrabbing)
				{
					this.WhenPointerEventRaised(new PointerEvent(this._id.ID, PointerEventType.Move, base.transform.GetPose(Space.World), null));
				}
				return;
			}
			this._currentlyGrabbing = this._grabFlag;
			if (this._currentlyGrabbing)
			{
				this.WhenPointerEventRaised(new PointerEvent(this._id.ID, PointerEventType.Hover, base.transform.GetPose(Space.World), null));
				this.WhenPointerEventRaised(new PointerEvent(this._id.ID, PointerEventType.Select, base.transform.GetPose(Space.World), null));
				return;
			}
			this.WhenPointerEventRaised(new PointerEvent(this._id.ID, PointerEventType.Unselect, base.transform.GetPose(Space.World), null));
			this.WhenPointerEventRaised(new PointerEvent(this._id.ID, PointerEventType.Unhover, base.transform.GetPose(Space.World), null));
		}

		public void SetGrabFlag(bool grabFlag)
		{
			this._grabFlag = grabFlag;
		}

		protected virtual void OnDestroy()
		{
			UniqueIdentifier.Release(this._id);
		}

		[SerializeField]
		private bool _grabFlag;

		private UniqueIdentifier _id;

		private bool _currentlyGrabbing;
	}
}
