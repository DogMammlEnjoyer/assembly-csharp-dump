using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PointableElement : MonoBehaviour, IPointableElement, IPointable
	{
		public IPointableElement ForwardElement { get; private set; }

		public bool TransferOnSecondSelection
		{
			get
			{
				return this._transferOnSecondSelection;
			}
			set
			{
				this._transferOnSecondSelection = value;
			}
		}

		public bool AddNewPointsToFront
		{
			get
			{
				return this._addNewPointsToFront;
			}
			set
			{
				this._addNewPointsToFront = value;
			}
		}

		public event Action<PointerEvent> WhenPointerEventRaised = delegate(PointerEvent <p0>)
		{
		};

		public List<Pose> Points
		{
			get
			{
				return this._points;
			}
		}

		public int PointsCount
		{
			get
			{
				return this._points.Count;
			}
		}

		public List<Pose> SelectingPoints
		{
			get
			{
				return this._selectingPoints;
			}
		}

		public int SelectingPointsCount
		{
			get
			{
				return this._selectingPoints.Count;
			}
		}

		protected virtual void Awake()
		{
			this.ForwardElement = (this._forwardElement as IPointableElement);
			this._points = new List<Pose>();
			this._pointIds = new List<int>();
			this._selectingPoints = new List<Pose>();
			this._selectingPointIds = new List<int>();
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._forwardElement;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started && this.ForwardElement != null)
			{
				this.ForwardElement.WhenPointerEventRaised += this.HandlePointerEventRaised;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				while (this._selectingPoints.Count > 0)
				{
					this.Cancel(new PointerEvent(this._selectingPointIds[0], PointerEventType.Cancel, this._selectingPoints[0], null));
				}
				if (this.ForwardElement != null)
				{
					this.ForwardElement.WhenPointerEventRaised -= this.HandlePointerEventRaised;
				}
			}
		}

		private void HandlePointerEventRaised(PointerEvent evt)
		{
			if (evt.Type == PointerEventType.Cancel)
			{
				this.ProcessPointerEvent(evt);
			}
		}

		public virtual void ProcessPointerEvent(PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Hover:
				this.Hover(evt);
				return;
			case PointerEventType.Unhover:
				this.Unhover(evt);
				return;
			case PointerEventType.Select:
				this.Select(evt);
				return;
			case PointerEventType.Unselect:
				this.Unselect(evt);
				return;
			case PointerEventType.Move:
				this.Move(evt);
				return;
			case PointerEventType.Cancel:
				this.Cancel(evt);
				return;
			default:
				return;
			}
		}

		private void Hover(PointerEvent evt)
		{
			if (this._addNewPointsToFront)
			{
				this._pointIds.Insert(0, evt.Identifier);
				this._points.Insert(0, evt.Pose);
			}
			else
			{
				this._pointIds.Add(evt.Identifier);
				this._points.Add(evt.Pose);
			}
			this.PointableElementUpdated(evt);
		}

		private void Move(PointerEvent evt)
		{
			int num = this._pointIds.IndexOf(evt.Identifier);
			if (num == -1)
			{
				return;
			}
			this._points[num] = evt.Pose;
			num = this._selectingPointIds.IndexOf(evt.Identifier);
			if (num != -1)
			{
				this._selectingPoints[num] = evt.Pose;
			}
			this.PointableElementUpdated(evt);
		}

		private void Unhover(PointerEvent evt)
		{
			int num = this._pointIds.IndexOf(evt.Identifier);
			if (num == -1)
			{
				return;
			}
			this._pointIds.RemoveAt(num);
			this._points.RemoveAt(num);
			this.PointableElementUpdated(evt);
		}

		private void Select(PointerEvent evt)
		{
			if (this._selectingPoints.Count == 1 && this._transferOnSecondSelection)
			{
				this.Cancel(new PointerEvent(this._selectingPointIds[0], PointerEventType.Cancel, this._selectingPoints[0], null));
			}
			if (this._addNewPointsToFront)
			{
				this._selectingPointIds.Insert(0, evt.Identifier);
				this._selectingPoints.Insert(0, evt.Pose);
			}
			else
			{
				this._selectingPointIds.Add(evt.Identifier);
				this._selectingPoints.Add(evt.Pose);
			}
			this.PointableElementUpdated(evt);
		}

		private void Unselect(PointerEvent evt)
		{
			int num = this._selectingPointIds.IndexOf(evt.Identifier);
			if (num == -1)
			{
				return;
			}
			this._selectingPointIds.RemoveAt(num);
			this._selectingPoints.RemoveAt(num);
			this.PointableElementUpdated(evt);
		}

		private void Cancel(PointerEvent evt)
		{
			int num = this._selectingPointIds.IndexOf(evt.Identifier);
			if (num != -1)
			{
				this._selectingPointIds.RemoveAt(num);
				this._selectingPoints.RemoveAt(num);
			}
			num = this._pointIds.IndexOf(evt.Identifier);
			if (num != -1)
			{
				this._pointIds.RemoveAt(num);
				this._points.RemoveAt(num);
				this.PointableElementUpdated(evt);
				return;
			}
		}

		protected virtual void PointableElementUpdated(PointerEvent evt)
		{
			if (this.ForwardElement != null)
			{
				this.ForwardElement.ProcessPointerEvent(evt);
			}
			this.WhenPointerEventRaised(evt);
		}

		public void InjectOptionalForwardElement(IPointableElement forwardElement)
		{
			this.ForwardElement = forwardElement;
			this._forwardElement = (forwardElement as Object);
		}

		[Tooltip("If checked, if you’re selecting an object with one hand and then select it with the other hand, the original hand is forced to release the object.")]
		[SerializeField]
		private bool _transferOnSecondSelection;

		[Tooltip("If checked, when you select an object, that hand’s Vector3 points are added to the beginning of the list of Vector3 points instead of the end. This property has very unique usecases, so in most cases you should use the Transfer on Second Selection property instead.")]
		[SerializeField]
		private bool _addNewPointsToFront;

		[Tooltip("Events will be forwarded to this element. Can be used to chain multiple PointableElements together. However, we recommend using the Interactable's Forward Element field instead.")]
		[SerializeField]
		[Interface(typeof(IPointableElement), new Type[]
		{

		})]
		[Optional]
		private Object _forwardElement;

		protected List<Pose> _points;

		protected List<int> _pointIds;

		protected List<Pose> _selectingPoints;

		protected List<int> _selectingPointIds;

		protected bool _started;
	}
}
