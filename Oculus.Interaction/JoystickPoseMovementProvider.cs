using System;
using System.Linq;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class JoystickPoseMovementProvider : MonoBehaviour, IMovementProvider
	{
		public float MoveSpeed
		{
			get
			{
				return this._moveSpeed;
			}
			set
			{
				this._moveSpeed = value;
			}
		}

		public float RotationSpeed
		{
			get
			{
				return this._rotationSpeed;
			}
			set
			{
				this._rotationSpeed = value;
			}
		}

		public float MinDistance
		{
			get
			{
				return this._minDistance;
			}
			set
			{
				this._minDistance = value;
			}
		}

		public float MaxDistance
		{
			get
			{
				return this._maxDistance;
			}
			set
			{
				this._maxDistance = value;
			}
		}

		private void Awake()
		{
			this._interactableView = (this._interactable as IInteractableView);
		}

		private void OnEnable()
		{
			if (this._interactableView != null)
			{
				this._interactableView.WhenSelectingInteractorViewAdded += this.OnSelectingInteractorViewAdded;
				this._interactableView.WhenSelectingInteractorViewRemoved += this.OnSelectingInteractorViewRemoved;
			}
		}

		private void OnDisable()
		{
			if (this._interactableView != null)
			{
				this._interactableView.WhenSelectingInteractorViewAdded -= this.OnSelectingInteractorViewAdded;
				this._interactableView.WhenSelectingInteractorViewRemoved -= this.OnSelectingInteractorViewRemoved;
			}
		}

		private void OnSelectingInteractorViewAdded(IInteractorView interactor)
		{
			this._latestSelectingInteractor = interactor;
		}

		private void OnSelectingInteractorViewRemoved(IInteractorView interactor)
		{
			if (this._latestSelectingInteractor == interactor)
			{
				this._latestSelectingInteractor = this._interactableView.SelectingInteractorViews.LastOrDefault<IInteractorView>();
			}
		}

		public IMovement CreateMovement()
		{
			IController controller = null;
			if (this._latestSelectingInteractor != null)
			{
				InteractorControllerDecorator.TryGetControllerForInteractor(this._latestSelectingInteractor, out controller);
			}
			return new JoystickPoseMovement(controller, this._moveSpeed, this._rotationSpeed, this._minDistance, this._maxDistance);
		}

		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private MonoBehaviour _interactable;

		private IInteractableView _interactableView;

		[FormerlySerializedAs("moveSpeed")]
		[SerializeField]
		[Optional]
		[Tooltip("The speed at which movement occurs.")]
		private float _moveSpeed = 0.04f;

		[FormerlySerializedAs("rotationSpeed")]
		[SerializeField]
		[Optional]
		[Tooltip("The speed at which rotation occurs.")]
		private float _rotationSpeed = 1f;

		[SerializeField]
		[Optional]
		[Range(0f, 10f)]
		[Tooltip("The minimum distance along the Z-axis for the grabbed object.")]
		private float _minDistance = 0.1f;

		[SerializeField]
		[Optional]
		[Range(1f, 10f)]
		[Tooltip("The maximum distance along the Z-axis for the grabbed object.")]
		private float _maxDistance = 3f;

		private IInteractorView _latestSelectingInteractor;
	}
}
