using System;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class DPadUnityEventWrapper : MonoBehaviour
	{
		private IAxis2D Axis { get; set; }

		public float PositiveDeadZone
		{
			get
			{
				return this._positiveDeadZone;
			}
			set
			{
				this._positiveDeadZone = value;
			}
		}

		public float NegativeDeadZone
		{
			get
			{
				return this._negativeDeadZone;
			}
			set
			{
				this._negativeDeadZone = value;
			}
		}

		public UnityEvent WhenPressLeft
		{
			get
			{
				return this._whenPressLeft;
			}
		}

		public UnityEvent WhenPressRight
		{
			get
			{
				return this._whenPressRight;
			}
		}

		public UnityEvent WhenPressUp
		{
			get
			{
				return this._whenPressUp;
			}
		}

		public UnityEvent WhenPressDown
		{
			get
			{
				return this._whenPressDown;
			}
		}

		protected virtual void Awake()
		{
			this.Axis = (this._axis as IAxis2D);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			bool started = this._started;
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._lastDirection = Vector2Int.zero;
			}
		}

		protected virtual void Update()
		{
			Vector2Int vector2Int = this.AxisToDPadDirection(this.Axis.Value());
			if (this._lastDirection != vector2Int)
			{
				this._lastDirection = vector2Int;
				if (vector2Int.x < 0)
				{
					this._whenPressLeft.Invoke();
				}
				if (vector2Int.x > 0)
				{
					this._whenPressRight.Invoke();
				}
				if (vector2Int.y < 0)
				{
					this._whenPressDown.Invoke();
				}
				if (vector2Int.y > 0)
				{
					this._whenPressUp.Invoke();
				}
			}
		}

		private Vector2Int AxisToDPadDirection(Vector2 axisValue)
		{
			Vector2Int zero = Vector2Int.zero;
			if (Mathf.Abs(axisValue.x) > this._positiveDeadZone && Mathf.Abs(axisValue.y) < this._negativeDeadZone)
			{
				zero.x = ((axisValue.x >= 0f) ? 1 : -1);
			}
			if (Mathf.Abs(axisValue.y) > this._positiveDeadZone && Mathf.Abs(axisValue.x) < this._negativeDeadZone)
			{
				zero.y = ((axisValue.y >= 0f) ? 1 : -1);
			}
			return zero;
		}

		public void InjectAllDPadUnityEventWrapper(IAxis2D axis)
		{
			this.InjectAxis(axis);
		}

		public void InjectAxis(IAxis2D axis)
		{
			this.Axis = axis;
			this._axis = (axis as Object);
		}

		[SerializeField]
		[Interface(typeof(IAxis2D), new Type[]
		{

		})]
		private Object _axis;

		[SerializeField]
		private float _positiveDeadZone = 0.9f;

		[SerializeField]
		private float _negativeDeadZone = 0.5f;

		[SerializeField]
		private UnityEvent _whenPressLeft;

		[SerializeField]
		private UnityEvent _whenPressRight;

		[SerializeField]
		private UnityEvent _whenPressUp;

		[SerializeField]
		private UnityEvent _whenPressDown;

		protected bool _started;

		private Vector2Int _lastDirection = Vector2Int.zero;
	}
}
