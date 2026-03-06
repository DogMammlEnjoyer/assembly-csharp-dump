using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class PointableCanvasUnityEventWrapper : MonoBehaviour
	{
		private bool ShouldFireEvent(PointableCanvasEventArgs args)
		{
			return !(args.Canvas != this.PointableCanvas.Canvas) && (!this._suppressWhileDragging || !args.Dragging);
		}

		private void PointableCanvasModule_WhenSelectableHoverEnter(PointableCanvasEventArgs args)
		{
			if (this.ShouldFireEvent(args))
			{
				this._whenBeginHighlight.Invoke();
			}
		}

		private void PointableCanvasModule_WhenSelectableHoverExit(PointableCanvasEventArgs args)
		{
			if (this.ShouldFireEvent(args))
			{
				this._whenEndHighlight.Invoke();
			}
		}

		private void PointableCanvasModule_WhenSelectableSelected(PointableCanvasEventArgs args)
		{
			if (this.ShouldFireEvent(args))
			{
				if (args.Hovered == null)
				{
					this._whenSelectedEmpty.Invoke();
					return;
				}
				this._whenSelectedHovered.Invoke();
			}
		}

		private void PointableCanvasModule_WhenSelectableUnselected(PointableCanvasEventArgs args)
		{
			if (this.ShouldFireEvent(args))
			{
				if (args.Hovered == null)
				{
					this._whenUnselectedEmpty.Invoke();
					return;
				}
				this._whenUnselectedHovered.Invoke();
			}
		}

		protected virtual void Awake()
		{
			this.PointableCanvas = (this._pointableCanvas as IPointableCanvas);
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
				PointableCanvasModule.WhenSelectableHovered += this.PointableCanvasModule_WhenSelectableHoverEnter;
				PointableCanvasModule.WhenSelectableUnhovered += this.PointableCanvasModule_WhenSelectableHoverExit;
				PointableCanvasModule.WhenSelected += this.PointableCanvasModule_WhenSelectableSelected;
				PointableCanvasModule.WhenUnselected += this.PointableCanvasModule_WhenSelectableUnselected;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				PointableCanvasModule.WhenSelectableHovered -= this.PointableCanvasModule_WhenSelectableHoverEnter;
				PointableCanvasModule.WhenSelectableUnhovered -= this.PointableCanvasModule_WhenSelectableHoverExit;
				PointableCanvasModule.WhenSelected -= this.PointableCanvasModule_WhenSelectableSelected;
				PointableCanvasModule.WhenUnselected -= this.PointableCanvasModule_WhenSelectableUnselected;
			}
		}

		[SerializeField]
		[Interface(typeof(IPointableCanvas), new Type[]
		{

		})]
		private Object _pointableCanvas;

		private IPointableCanvas PointableCanvas;

		[SerializeField]
		[Tooltip("Selection and hover events will not be fired while dragging.")]
		private bool _suppressWhileDragging = true;

		[SerializeField]
		[Tooltip("Raised when beginning hover of a uGUI selectable")]
		private UnityEvent _whenBeginHighlight;

		[SerializeField]
		[Tooltip("Raised when ending hover of a uGUI selectable")]
		private UnityEvent _whenEndHighlight;

		[SerializeField]
		[Tooltip("Raised when selecting a hovered uGUI selectable")]
		private UnityEvent _whenSelectedHovered;

		[SerializeField]
		[Tooltip("Raised when selecting with no uGUI selectable hovered")]
		private UnityEvent _whenSelectedEmpty;

		[SerializeField]
		[Tooltip("Raised when deselecting a hovered uGUI selectable")]
		private UnityEvent _whenUnselectedHovered;

		[SerializeField]
		[Tooltip("Raised when deselecting with no uGUI selectable hovered")]
		private UnityEvent _whenUnselectedEmpty;

		protected bool _started;
	}
}
