using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TurnerEventBroadcaster : MonoBehaviour, ILocomotionEventBroadcaster
	{
		private IInteractor Interactor { get; set; }

		private IAxis1D Axis { get; set; }

		public TurnerEventBroadcaster.TurnMode TurnMethod
		{
			get
			{
				return this._turnMethod;
			}
			set
			{
				this._turnMethod = value;
			}
		}

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

		public bool FireSnapOnUnselect
		{
			get
			{
				return this._fireSnapOnUnselect;
			}
			set
			{
				this._fireSnapOnUnselect = value;
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
			this.Interactor = (this._interactor as IInteractor);
			this.Axis = (this._axis as IAxis1D);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Interactor.WhenStateChanged += this.HandleStateChanged;
				this.Interactor.WhenPostprocessed += this.HandlePostprocessed;
				this._wasSelecting = false;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Interactor.WhenStateChanged -= this.HandleStateChanged;
				this.Interactor.WhenPostprocessed -= this.HandlePostprocessed;
			}
		}

		public event Action<LocomotionEvent> WhenLocomotionPerformed
		{
			add
			{
				this._whenLocomotionEventRaised = (Action<LocomotionEvent>)Delegate.Combine(this._whenLocomotionEventRaised, value);
			}
			remove
			{
				this._whenLocomotionEventRaised = (Action<LocomotionEvent>)Delegate.Remove(this._whenLocomotionEventRaised, value);
			}
		}

		private void HandleStateChanged(InteractorStateChangeArgs obj)
		{
			if (obj.PreviousState == InteractorState.Select)
			{
				this._wasSelecting = this._fireSnapOnUnselect;
			}
		}

		private void HandlePostprocessed()
		{
			if (this._wasSelecting && this._fireSnapOnUnselect)
			{
				this._wasSelecting = false;
				if ((this.Interactor.State == InteractorState.Hover || this.Interactor.State == InteractorState.Normal) && this._turnMethod == TurnerEventBroadcaster.TurnMode.Snap)
				{
					this.SnapTurn(this.Axis.Value());
				}
			}
			if (this.Interactor.State == InteractorState.Select)
			{
				if (this._turnMethod == TurnerEventBroadcaster.TurnMode.Smooth)
				{
					this.SmoothTurn(this.Axis.Value());
					return;
				}
				if (this._turnMethod == TurnerEventBroadcaster.TurnMode.Snap && !this._fireSnapOnUnselect && !this._wasSelecting)
				{
					this._wasSelecting = true;
					this.SnapTurn(this.Axis.Value());
				}
			}
		}

		public void SnapTurn(float direction)
		{
			float num = Mathf.Sign(direction);
			Quaternion rotation = Quaternion.Euler(0f, num * this._snapTurnDegrees, 0f);
			LocomotionEvent obj = new LocomotionEvent(this.Identifier, rotation, LocomotionEvent.RotationType.Relative);
			this._whenLocomotionEventRaised(obj);
		}

		public void SmoothTurn(float direction)
		{
			float num = Mathf.Sign(direction);
			float num2 = this._smoothTurnCurve.Evaluate(Mathf.Abs(direction));
			Quaternion rotation = Quaternion.Euler(0f, num * num2, 0f);
			LocomotionEvent obj = new LocomotionEvent(this.Identifier, rotation, LocomotionEvent.RotationType.Velocity);
			this._whenLocomotionEventRaised(obj);
		}

		public void InjectAllTurnerEventBroadcaster(IInteractor interactor, IAxis1D axis)
		{
			this.InjectInteractor(interactor);
			this.InjectAxis(axis);
		}

		public void InjectInteractor(IInteractor interactor)
		{
			this._interactor = (interactor as Object);
			this.Interactor = interactor;
		}

		public void InjectAxis(IAxis1D axis)
		{
			this._axis = (axis as Object);
			this.Axis = axis;
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		[Tooltip("The interactor defines when the Locomotion events are sent based on its Select state.")]
		private Object _interactor;

		[SerializeField]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		[Tooltip("Axis from -1 to 1 indicating the turning direction and velocity.")]
		private Object _axis;

		[SerializeField]
		[Tooltip("Snap turn fires once during Select, while Smooth fires continuously during Select.")]
		private TurnerEventBroadcaster.TurnMode _turnMethod;

		[SerializeField]
		[Tooltip("Degrees to instantly turn when in Snap turn mode. Note the direction is provided by the axis")]
		private float _snapTurnDegrees = 45f;

		[SerializeField]
		[Tooltip("Degrees to continuously rotate during selection when in Smooth turn mode, it is remapped from the Axis value")]
		private AnimationCurve _smoothTurnCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 100f);

		[SerializeField]
		[Tooltip("When enabled, snap turn happens on unselect. If false it happens on select")]
		private bool _fireSnapOnUnselect = true;

		private UniqueIdentifier _identifier;

		private bool _wasSelecting;

		protected bool _started;

		private Action<LocomotionEvent> _whenLocomotionEventRaised = delegate(LocomotionEvent <p0>)
		{
		};

		public enum TurnMode
		{
			Snap,
			Smooth
		}
	}
}
