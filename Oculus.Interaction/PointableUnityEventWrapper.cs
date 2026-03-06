using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class PointableUnityEventWrapper : MonoBehaviour
	{
		public UnityEvent<PointerEvent> WhenRelease
		{
			get
			{
				return this._whenRelease;
			}
		}

		public UnityEvent<PointerEvent> WhenHover
		{
			get
			{
				return this._whenHover;
			}
		}

		public UnityEvent<PointerEvent> WhenUnhover
		{
			get
			{
				return this._whenUnhover;
			}
		}

		public UnityEvent<PointerEvent> WhenSelect
		{
			get
			{
				return this._whenSelect;
			}
		}

		public UnityEvent<PointerEvent> WhenUnselect
		{
			get
			{
				return this._whenUnselect;
			}
		}

		public UnityEvent<PointerEvent> WhenMove
		{
			get
			{
				return this._whenMove;
			}
		}

		public UnityEvent<PointerEvent> WhenCancel
		{
			get
			{
				return this._whenCancel;
			}
		}

		protected virtual void Awake()
		{
			this.Pointable = (this._pointable as IPointable);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._pointers = new HashSet<int>();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Pointable.WhenPointerEventRaised += this.HandlePointerEventRaised;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Pointable.WhenPointerEventRaised -= this.HandlePointerEventRaised;
			}
		}

		private void HandlePointerEventRaised(PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Hover:
				this._whenHover.Invoke(evt);
				this._pointers.Add(evt.Identifier);
				return;
			case PointerEventType.Unhover:
				this._whenUnhover.Invoke(evt);
				this._pointers.Remove(evt.Identifier);
				return;
			case PointerEventType.Select:
				this._whenSelect.Invoke(evt);
				return;
			case PointerEventType.Unselect:
				if (this._pointers.Contains(evt.Identifier))
				{
					this._whenRelease.Invoke(evt);
				}
				this._whenUnselect.Invoke(evt);
				return;
			case PointerEventType.Move:
				this._whenMove.Invoke(evt);
				return;
			case PointerEventType.Cancel:
				this._whenCancel.Invoke(evt);
				this._pointers.Remove(evt.Identifier);
				return;
			default:
				return;
			}
		}

		public void InjectAllPointableUnityEventWrapper(IPointable pointable)
		{
			this.InjectPointable(pointable);
		}

		public void InjectPointable(IPointable pointable)
		{
			this._pointable = (pointable as Object);
			this.Pointable = pointable;
		}

		[Tooltip("The Pointable component to wrap.")]
		[SerializeField]
		[Interface(typeof(IPointable), new Type[]
		{

		})]
		private Object _pointable;

		private IPointable Pointable;

		private HashSet<int> _pointers;

		[Tooltip("Raised when the IPointable is released.")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenRelease;

		[Tooltip("Raised when the IPointable is hovered.")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenHover;

		[Tooltip("Raised when the IPointable is unhovered (it was hovered but now it isn't).")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenUnhover;

		[Tooltip("Raised when the IPointable is selected.")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenSelect;

		[Tooltip("Raised when the IPointable is unselected (it was selected but now it isn't).")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenUnselect;

		[Tooltip("Raised when the IPointable moves.")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenMove;

		[Tooltip("Raised when the IPointable is canceled.")]
		[SerializeField]
		private UnityEvent<PointerEvent> _whenCancel;

		protected bool _started;
	}
}
