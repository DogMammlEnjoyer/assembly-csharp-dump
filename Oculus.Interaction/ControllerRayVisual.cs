using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ControllerRayVisual : MonoBehaviour
	{
		public float MaxRayVisualLength
		{
			get
			{
				return this._maxRayVisualLength;
			}
			set
			{
				this._maxRayVisualLength = value;
			}
		}

		public Color HoverColor0
		{
			get
			{
				return this._hoverColor0;
			}
			set
			{
				this._hoverColor0 = value;
			}
		}

		public Color HoverColor1
		{
			get
			{
				return this._hoverColor1;
			}
			set
			{
				this._hoverColor1 = value;
			}
		}

		public Color SelectColor0
		{
			get
			{
				return this._selectColor0;
			}
			set
			{
				this._selectColor0 = value;
			}
		}

		public Color SelectColor1
		{
			get
			{
				return this._selectColor1;
			}
			set
			{
				this._selectColor1 = value;
			}
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
				this._rayInteractor.WhenPostprocessed += this.UpdateVisual;
				this._rayInteractor.WhenStateChanged += this.HandleStateChanged;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed -= this.UpdateVisual;
				this._rayInteractor.WhenStateChanged -= this.HandleStateChanged;
			}
		}

		private void HandleStateChanged(InteractorStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		private void UpdateVisual()
		{
			if (this._rayInteractor.State == InteractorState.Disabled || (this._hideWhenNoInteractable && this._rayInteractor.Interactable == null))
			{
				this._renderer.enabled = false;
				return;
			}
			this._renderer.enabled = true;
			base.transform.SetPositionAndRotation(this._rayInteractor.Origin, this._rayInteractor.Rotation);
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, Mathf.Min(this._maxRayVisualLength, (this._rayInteractor.End - base.transform.position).magnitude));
			this._materialPropertyBlockEditor.MaterialPropertyBlock.SetColor(this._shaderColor0, (this._rayInteractor.State == InteractorState.Select) ? this._selectColor0 : this._hoverColor0);
			this._materialPropertyBlockEditor.MaterialPropertyBlock.SetColor(this._shaderColor1, (this._rayInteractor.State == InteractorState.Select) ? this._selectColor1 : this._hoverColor1);
		}

		public void InjectAllControllerRayVisual(RayInteractor rayInteractor, Renderer renderer, MaterialPropertyBlockEditor materialPropertyBlockEditor)
		{
			this.InjectRayInteractor(rayInteractor);
			this.InjectRenderer(renderer);
			this.InjectMaterialPropertyBlockEditor(materialPropertyBlockEditor);
		}

		public void InjectRayInteractor(RayInteractor rayInteractor)
		{
			this._rayInteractor = rayInteractor;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor materialPropertyBlockEditor)
		{
			this._materialPropertyBlockEditor = materialPropertyBlockEditor;
		}

		[SerializeField]
		private RayInteractor _rayInteractor;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private MaterialPropertyBlockEditor _materialPropertyBlockEditor;

		[SerializeField]
		private float _maxRayVisualLength = 0.5f;

		[SerializeField]
		private Color _hoverColor0 = Color.white;

		[SerializeField]
		private Color _hoverColor1 = Color.white;

		[SerializeField]
		private Color _selectColor0 = Color.blue;

		[SerializeField]
		private Color _selectColor1 = Color.blue;

		[SerializeField]
		private bool _hideWhenNoInteractable;

		private int _shaderColor0 = Shader.PropertyToID("_Color0");

		private int _shaderColor1 = Shader.PropertyToID("_Color1");

		private bool _started;
	}
}
