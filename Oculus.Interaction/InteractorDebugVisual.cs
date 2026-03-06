using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractorDebugVisual : MonoBehaviour
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
			this.InteractorView = (this._interactorView as IInteractorView);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._material = this._renderer.material;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.InteractorView.WhenStateChanged += this.UpdateVisualState;
				this.UpdateVisual();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.InteractorView.WhenStateChanged -= this.UpdateVisualState;
			}
		}

		private void UpdateVisual()
		{
			switch (this.InteractorView.State)
			{
			case InteractorState.Normal:
				this._material.color = this._normalColor;
				return;
			case InteractorState.Hover:
				this._material.color = this._hoverColor;
				return;
			case InteractorState.Select:
				this._material.color = this._selectColor;
				return;
			case InteractorState.Disabled:
				this._material.color = this._disabledColor;
				return;
			default:
				return;
			}
		}

		private void UpdateVisualState(InteractorStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		public void InjectAllInteractorDebugVisual(IInteractorView interactorView, Renderer renderer)
		{
			this.InjectInteractorView(interactorView);
			this.InjectRenderer(renderer);
		}

		public void InjectInteractorView(IInteractorView interactorView)
		{
			this._interactorView = (interactorView as Object);
			this.InteractorView = interactorView;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		[SerializeField]
		[Interface(typeof(IInteractorView), new Type[]
		{

		})]
		private Object _interactorView;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _hoverColor = Color.blue;

		[SerializeField]
		private Color _selectColor = Color.green;

		[SerializeField]
		private Color _disabledColor = Color.black;

		private IInteractorView InteractorView;

		private Material _material;

		protected bool _started;
	}
}
