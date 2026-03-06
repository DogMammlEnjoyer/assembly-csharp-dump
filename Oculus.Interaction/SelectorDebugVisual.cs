using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class SelectorDebugVisual : MonoBehaviour
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

		protected virtual void Awake()
		{
			this.Selector = (this._selector as ISelector);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._material = this._renderer.material;
			this._material.color = this._normalColor;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Selector.WhenSelected += this.HandleSelected;
				this.Selector.WhenUnselected += this.HandleUnselected;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.HandleUnselected();
				this.Selector.WhenSelected -= this.HandleSelected;
				this.Selector.WhenUnselected -= this.HandleUnselected;
			}
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		private void HandleSelected()
		{
			if (this._selected)
			{
				return;
			}
			this._selected = true;
			this._material.color = this._selectColor;
		}

		private void HandleUnselected()
		{
			if (!this._selected)
			{
				return;
			}
			this._selected = false;
			this._material.color = this._normalColor;
		}

		public void InjectAllSelectorDebugVisual(ISelector selector, Renderer renderer)
		{
			this.InjectSelector(selector);
			this.InjectRenderer(renderer);
		}

		public void InjectSelector(ISelector selector)
		{
			this._selector = (selector as Object);
			this.Selector = selector;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		[SerializeField]
		[Interface(typeof(ISelector), new Type[]
		{

		})]
		private Object _selector;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _selectColor = Color.green;

		private ISelector Selector;

		private Material _material;

		private bool _selected;

		protected bool _started;
	}
}
