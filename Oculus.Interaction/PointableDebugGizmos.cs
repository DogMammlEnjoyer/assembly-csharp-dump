using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PointableDebugGizmos : MonoBehaviour
	{
		public float Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
			}
		}

		public Color HoverColor
		{
			get
			{
				return this._hoverColor;
			}
			set
			{
				this._hoverColor = value;
			}
		}

		public Color SelectColor
		{
			get
			{
				return this._selectColor;
			}
			set
			{
				this._selectColor = value;
			}
		}

		public bool DrawAxes
		{
			get
			{
				return this._drawAxes;
			}
			set
			{
				this._drawAxes = value;
			}
		}

		private void Reset()
		{
			IPointable component = base.GetComponent<IPointable>();
			this.InjectAllPointableDebugGizmos(component);
		}

		protected virtual void Awake()
		{
			this.Pointable = (this._pointable as IPointable);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._points = new Dictionary<int, PointableDebugGizmos.PointData>();
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
				this._points.Add(evt.Identifier, new PointableDebugGizmos.PointData
				{
					Pose = evt.Pose,
					Selecting = false
				});
				return;
			case PointerEventType.Unhover:
			case PointerEventType.Cancel:
				this._points.Remove(evt.Identifier);
				break;
			case PointerEventType.Select:
				this._points[evt.Identifier].Selecting = true;
				return;
			case PointerEventType.Unselect:
				if (this._points.ContainsKey(evt.Identifier))
				{
					this._points[evt.Identifier].Selecting = false;
					return;
				}
				break;
			case PointerEventType.Move:
				this._points[evt.Identifier].Pose = evt.Pose;
				return;
			default:
				return;
			}
		}

		protected virtual void LateUpdate()
		{
			foreach (PointableDebugGizmos.PointData pointData in this._points.Values)
			{
				DebugGizmos.LineWidth = this._radius;
				DebugGizmos.Color = (pointData.Selecting ? this._selectColor : this._hoverColor);
				DebugGizmos.DrawPoint(pointData.Pose.position, null);
				if (this._drawAxes)
				{
					DebugGizmos.LineWidth = this._radius / 2f;
					DebugGizmos.DrawAxis(pointData.Pose.position, pointData.Pose.rotation, this._radius * 2f);
				}
			}
		}

		public void InjectAllPointableDebugGizmos(IPointable pointable)
		{
			this.InjectPointable(pointable);
		}

		public void InjectPointable(IPointable pointable)
		{
			this._pointable = (pointable as Object);
			this.Pointable = pointable;
		}

		[SerializeField]
		[Interface(typeof(IPointable), new Type[]
		{

		})]
		private Object _pointable;

		[SerializeField]
		private float _radius = 0.01f;

		[SerializeField]
		private Color _hoverColor = Color.blue;

		[SerializeField]
		private Color _selectColor = Color.green;

		[SerializeField]
		private bool _drawAxes = true;

		private Dictionary<int, PointableDebugGizmos.PointData> _points;

		private IPointable Pointable;

		protected bool _started;

		private class PointData
		{
			public Pose Pose { get; set; }

			public bool Selecting { get; set; }
		}
	}
}
