using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TurnLocomotionBroadcaster : MonoBehaviour, ILocomotionEventBroadcaster
	{
		public float SnapTurnDegrees
		{
			get
			{
				return this._snapTurnDegrees;
			}
			set
			{
				this._snapTurnDegrees = value;
			}
		}

		public AnimationCurve SmoothTurnCurve
		{
			get
			{
				return this._smoothTurnCurve;
			}
			set
			{
				this._smoothTurnCurve = value;
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

		public void SnapTurnLeft()
		{
			this.SnapTurn(-1f);
		}

		public void SnapTurnRight()
		{
			this.SnapTurn(1f);
		}

		public void SnapTurn(float direction)
		{
			float num = Mathf.Sign(direction);
			Quaternion rotation = Quaternion.Euler(0f, num * this._snapTurnDegrees, 0f);
			LocomotionEvent obj = new LocomotionEvent(this.Identifier, rotation, LocomotionEvent.RotationType.Relative);
			this.WhenLocomotionPerformed(obj);
		}

		public void SmoothTurn(float direction)
		{
			float num = Mathf.Sign(direction);
			float num2 = this._smoothTurnCurve.Evaluate(Mathf.Abs(direction));
			Quaternion rotation = Quaternion.Euler(0f, num * num2, 0f);
			LocomotionEvent obj = new LocomotionEvent(this.Identifier, rotation, LocomotionEvent.RotationType.Velocity);
			this.WhenLocomotionPerformed(obj);
		}

		[SerializeField]
		[Tooltip("Degrees to instantly turn when in Snap turn mode. Note the direction is provided by the axis")]
		private float _snapTurnDegrees = 45f;

		[SerializeField]
		[Tooltip("Degrees to continuously rotate during selection when in Smooth turn mode, it is remapped from the Axis value")]
		private AnimationCurve _smoothTurnCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 100f);

		private UniqueIdentifier _identifier;
	}
}
