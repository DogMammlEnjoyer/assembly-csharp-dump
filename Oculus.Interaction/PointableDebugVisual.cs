using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PointableDebugVisual : MonoBehaviour
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

		protected virtual void Awake()
		{
			this.Pointable = (this._pointable as IPointable);
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
				this.Pointable.WhenPointerEventRaised += this.HandlePointerEventRaised;
				this.UpdateMaterialColor();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Pointable.WhenPointerEventRaised -= this.HandlePointerEventRaised;
			}
		}

		private void OnDestroy()
		{
			Object.Destroy(this._material);
		}

		private void HandlePointerEventRaised(PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Hover:
				this._hover = true;
				this.UpdateMaterialColor();
				return;
			case PointerEventType.Unhover:
				this._hover = false;
				this.UpdateMaterialColor();
				break;
			case PointerEventType.Select:
				this._select = true;
				this.UpdateMaterialColor();
				return;
			case PointerEventType.Unselect:
				this._select = false;
				this.UpdateMaterialColor();
				return;
			case PointerEventType.Move:
				break;
			default:
				return;
			}
		}

		private void UpdateMaterialColor()
		{
			this._material.color = (this._select ? this._selectColor : (this._hover ? this._hoverColor : this._normalColor));
		}

		public void InjectAllPointableDebugVisual(IPointable pointable, Renderer renderer)
		{
			this.InjectPointable(pointable);
			this.InjectRenderer(renderer);
		}

		public void InjectPointable(IPointable pointable)
		{
			this._pointable = (pointable as Object);
			this.Pointable = pointable;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		[SerializeField]
		[Interface(typeof(IPointable), new Type[]
		{

		})]
		private Object _pointable;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Color _normalColor = Color.red;

		[SerializeField]
		private Color _hoverColor = Color.blue;

		[SerializeField]
		private Color _selectColor = Color.green;

		private IPointable Pointable;

		private Material _material;

		private bool _hover;

		private bool _select;

		protected bool _started;
	}
}
