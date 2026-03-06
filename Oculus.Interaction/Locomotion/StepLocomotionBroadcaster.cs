using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class StepLocomotionBroadcaster : MonoBehaviour, ILocomotionEventBroadcaster
	{
		public Transform Origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				this._origin = value;
			}
		}

		public float StepLength
		{
			get
			{
				return this._stepLength;
			}
			set
			{
				this._stepLength = value;
			}
		}

		public int Identifier
		{
			get
			{
				return this._identifier.ID;
			}
		}

		public event Action<LocomotionEvent> WhenLocomotionPerformed = delegate(LocomotionEvent <p0>)
		{
		};

		protected virtual void Awake()
		{
			this._identifier = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public void StepLeft()
		{
			this.Step(Vector2Int.left);
		}

		public void StepRight()
		{
			this.Step(Vector2Int.right);
		}

		public void StepForward()
		{
			this.Step(Vector2Int.up);
		}

		public void StepBackward()
		{
			this.Step(Vector2Int.down);
		}

		public void Step(Vector2Int relativeDirection)
		{
			Vector3 forward = this._origin.forward;
			Vector3 up = Vector3.up;
			Vector3 position = (Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up).normalized, up) * new Vector3((float)relativeDirection.x, 0f, (float)relativeDirection.y)).normalized * this._stepLength;
			LocomotionEvent obj = new LocomotionEvent(this.Identifier, position, LocomotionEvent.TranslationType.Relative);
			this.WhenLocomotionPerformed(obj);
		}

		[SerializeField]
		private Transform _origin;

		[SerializeField]
		private float _stepLength = 0.5f;

		protected bool _started;

		private UniqueIdentifier _identifier;
	}
}
