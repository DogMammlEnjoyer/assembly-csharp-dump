using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class SlideLocomotionBroadcaster : MonoBehaviour, ILocomotionEventBroadcaster
	{
		public Transform Aiming
		{
			get
			{
				return this._aiming;
			}
			set
			{
				this._aiming = value;
			}
		}

		public AnimationCurve VerticalDeadZone
		{
			get
			{
				return this._verticalDeadZone;
			}
			set
			{
				this._verticalDeadZone = value;
			}
		}

		public AnimationCurve HorizontalDeadZone
		{
			get
			{
				return this._horizontalDeadZone;
			}
			set
			{
				this._horizontalDeadZone = value;
			}
		}

		public event Action<LocomotionEvent> WhenLocomotionPerformed
		{
			add
			{
				this._whenLocomotionPerformed = (Action<LocomotionEvent>)Delegate.Combine(this._whenLocomotionPerformed, value);
			}
			remove
			{
				this._whenLocomotionPerformed = (Action<LocomotionEvent>)Delegate.Remove(this._whenLocomotionPerformed, value);
			}
		}

		public int Identifier
		{
			get
			{
				return this._identifier.ID;
			}
		}

		protected virtual void Awake()
		{
			this._identifier = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
			this.Axis2D = (this._axis2D as IAxis2D);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void Update()
		{
			Vector2 vector = this.ProcessAxisSensitivity();
			Pose pose = this.StepDirection(new Vector3(vector.x, 0f, vector.y));
			if (!Mathf.Approximately(pose.position.sqrMagnitude, 0f))
			{
				LocomotionEvent obj = new LocomotionEvent(this.Identifier, pose, LocomotionEvent.TranslationType.Velocity, LocomotionEvent.RotationType.None);
				this._whenLocomotionPerformed(obj);
			}
		}

		private Vector2 ProcessAxisSensitivity()
		{
			Vector2 vector = this.Axis2D.Value();
			if (this._horizontalDeadZone != null)
			{
				vector.x = this._horizontalDeadZone.Evaluate(vector.x);
			}
			if (this._verticalDeadZone != null)
			{
				vector.y = this._verticalDeadZone.Evaluate(vector.y);
			}
			return vector;
		}

		private Pose StepDirection(Vector3 axisValue)
		{
			if (this._aiming == null)
			{
				return new Pose(axisValue, Quaternion.identity);
			}
			return new Pose(this._aiming.right * axisValue.x + this._aiming.up * axisValue.y + this._aiming.forward * axisValue.z, this._aiming.rotation);
		}

		public void InjectAllSlideLocomotionBroadcaster(IAxis2D axis2D)
		{
			this.InjectAxis2D(axis2D);
		}

		public void InjectAxis2D(IAxis2D axis2D)
		{
			this._axis2D = (axis2D as Object);
			this.Axis2D = axis2D;
		}

		[SerializeField]
		[Interface(typeof(IAxis2D), new Type[]
		{

		})]
		private Object _axis2D;

		private IAxis2D Axis2D;

		[SerializeField]
		[Optional]
		private Transform _aiming;

		[SerializeField]
		[Optional]
		private AnimationCurve _verticalDeadZone = AnimationCurve.Linear(-1f, -1f, 1f, 1f);

		[SerializeField]
		[Optional]
		private AnimationCurve _horizontalDeadZone = AnimationCurve.Linear(-1f, -1f, 1f, 1f);

		private Action<LocomotionEvent> _whenLocomotionPerformed = delegate(LocomotionEvent <p0>)
		{
		};

		private UniqueIdentifier _identifier;

		protected bool _started;
	}
}
