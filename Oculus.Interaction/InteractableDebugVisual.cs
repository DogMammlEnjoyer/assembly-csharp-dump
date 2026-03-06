using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractableDebugVisual : MonoBehaviour
	{
		public Color NormalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				this._normalColor = value;
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

		public Color DisabledColor
		{
			get
			{
				return this._disabledColor;
			}
			set
			{
				this._disabledColor = value;
			}
		}

		protected virtual void Awake()
		{
			this.InteractableView = (this._interactableView as IInteractableView);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._material = this._renderer.material;
			this.UpdateVisual();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.InteractableView.WhenStateChanged += this.UpdateVisualState;
				this.UpdateVisual();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.InteractableView.WhenStateChanged -= this.UpdateVisualState;
			}
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		public void SetNormalColor(Color color)
		{
			this._normalColor = color;
			this.UpdateVisual();
		}

		private void UpdateVisual()
		{
			switch (this.InteractableView.State)
			{
			case InteractableState.Normal:
				this._material.color = this._normalColor;
				return;
			case InteractableState.Hover:
				this._material.color = this._hoverColor;
				return;
			case InteractableState.Select:
				this._material.color = this._selectColor;
				return;
			case InteractableState.Disabled:
				this._material.color = this._disabledColor;
				return;
			default:
				return;
			}
		}

		private void UpdateVisualState(InteractableStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		public void InjectAllInteractableDebugVisual(IInteractableView interactableView, Renderer renderer)
		{
			this.InjectInteractableView(interactableView);
			this.InjectRenderer(renderer);
		}

		public void InjectInteractableView(IInteractableView interactableView)
		{
			this._interactableView = (interactableView as Object);
			this.InteractableView = interactableView;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		[Tooltip("The interactable to monitor for state changes.")]
		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private Object _interactableView;

		[Tooltip("The mesh that will change color based on the current state.")]
		[SerializeField]
		private Renderer _renderer;

		[Tooltip("Displayed when the state is normal.")]
		[SerializeField]
		private Color _normalColor = Color.red;

		[Tooltip("Displayed when the state is hover.")]
		[SerializeField]
		private Color _hoverColor = Color.blue;

		[Tooltip("Displayed when the state is selected.")]
		[SerializeField]
		private Color _selectColor = Color.green;

		[Tooltip("Displayed when the state is disabled.")]
		[SerializeField]
		private Color _disabledColor = Color.black;

		private IInteractableView InteractableView;

		private Material _material;

		protected bool _started;
	}
}
