using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus.Interaction.Input
{
	public class ScrollInputProvider : MonoBehaviour
	{
		private IAxis2D Axis2D { get; set; }

		private IInteractorView InteractorView { get; set; }

		private void Awake()
		{
			this.Axis2D = (this._axis2D as IAxis2D);
			this.InteractorView = (this._interactor as IInteractorView);
		}

		private void Start()
		{
			this.BeginStart(ref this._started, null);
			this._pointerEventData = new PointerEventData(EventSystem.current);
			this.EndStart(ref this._started);
		}

		private void OnEnable()
		{
			if (this._started)
			{
				PointableCanvasModule.WhenPointerStarted += this.HandlePointerStarted;
			}
		}

		private void OnDisable()
		{
			if (this._started)
			{
				PointableCanvasModule.WhenPointerStarted -= this.HandlePointerStarted;
				if (this._currentPointer != null)
				{
					this._currentPointer.WhenUpdated -= this.HandlePointerUpdated;
					this._currentPointer = null;
				}
			}
		}

		private void HandlePointerStarted(PointableCanvasModule.Pointer pointer)
		{
			if (pointer.Identifier == this.InteractorView.Identifier)
			{
				if (this._currentPointer != null)
				{
					this._currentPointer.WhenUpdated -= this.HandlePointerUpdated;
				}
				pointer.WhenUpdated += this.HandlePointerUpdated;
				this._currentPointer = pointer;
			}
		}

		private void HandlePointerUpdated(PointerEventData pointerEventData)
		{
			Vector2 vector = this.TryGetScrollData();
			if (vector != Vector2.zero)
			{
				this._pointerEventData.scrollDelta = vector;
				this._pointerEventData.position = pointerEventData.position;
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(pointerEventData.pointerCurrentRaycast.gameObject, this._pointerEventData, ExecuteEvents.scrollHandler);
			}
		}

		private Vector2 TryGetScrollData()
		{
			Vector2 vector = this.Axis2D.Value();
			if (vector.magnitude < this._deadZone)
			{
				return Vector2.zero;
			}
			vector = this.ApplyAxisSettings(vector);
			return vector * this._scrollSpeed;
		}

		private Vector2 ApplyAxisSettings(Vector2 input)
		{
			input.x = (this._scrollXAxis ? (this._invertXAxis ? (-input.x) : input.x) : 0f);
			input.y = (this._scrollYAxis ? (this._invertYAxis ? (-input.y) : input.y) : 0f);
			return input;
		}

		public void InjectAll(IAxis2D axis2D, IInteractorView interactorView)
		{
			this.InjectAxis2D(axis2D);
			this.InjectInteractorView(interactorView);
		}

		public void InjectAxis2D(IAxis2D axis2D)
		{
			this._axis2D = (axis2D as Object);
			this.Axis2D = axis2D;
		}

		public void InjectInteractorView(IInteractorView interactorView)
		{
			this._interactor = (interactorView as Object);
			this.InteractorView = interactorView;
		}

		[SerializeField]
		[Interface(typeof(IAxis2D), new Type[]
		{

		})]
		[Tooltip("Input 2D Axis from which the horizontal and vertical axis will be extracted")]
		private Object _axis2D;

		[SerializeField]
		[Interface(typeof(IInteractorView), new Type[]
		{

		})]
		private Object _interactor;

		[SerializeField]
		[Optional]
		[Tooltip("The speed at which scrolling occurs.")]
		private float _scrollSpeed = 5f;

		[SerializeField]
		[Optional]
		[Tooltip("The dead zone threshold for input.")]
		private float _deadZone = 0.1f;

		[SerializeField]
		[Optional]
		[Tooltip("Enable or disable scrolling on the X axis.")]
		private bool _scrollXAxis = true;

		[SerializeField]
		[Optional]
		[Tooltip("Enable or disable scrolling on the Y axis.")]
		private bool _scrollYAxis = true;

		[SerializeField]
		[Optional]
		[Tooltip("Invert the X axis input.")]
		private bool _invertXAxis;

		[SerializeField]
		[Optional]
		[Tooltip("Invert the Y axis input.")]
		private bool _invertYAxis;

		private PointerEventData _pointerEventData;

		private PointableCanvasModule.Pointer _currentPointer;

		private bool _started;
	}
}
