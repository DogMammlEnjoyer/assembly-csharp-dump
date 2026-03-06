using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class RayInteractorCursorVisual : MonoBehaviour
	{
		public Transform PlayerHead
		{
			get
			{
				return this._playerHead;
			}
			set
			{
				this._playerHead = value;
				if (this._started && value == null)
				{
					base.transform.localScale = this._startScale;
				}
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

		public Color OutlineColor
		{
			get
			{
				return this._outlineColor;
			}
			set
			{
				this._outlineColor = value;
			}
		}

		public float OffsetAlongNormal
		{
			get
			{
				return this._offsetAlongNormal;
			}
			set
			{
				this._offsetAlongNormal = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.UpdateVisual();
			this._startScale = base.transform.localScale;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed += this.UpdateVisual;
				this._rayInteractor.WhenStateChanged += this.UpdateVisualState;
				this.UpdateVisual();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._rayInteractor.WhenPostprocessed -= this.UpdateVisual;
				this._rayInteractor.WhenStateChanged -= this.UpdateVisualState;
			}
		}

		private void UpdateVisual()
		{
			if (this._rayInteractor.State == InteractorState.Disabled)
			{
				if (this._renderer.enabled)
				{
					this._renderer.enabled = false;
				}
				return;
			}
			if (this._rayInteractor.CollisionInfo == null)
			{
				this._renderer.enabled = false;
				return;
			}
			if (!this._renderer.enabled)
			{
				this._renderer.enabled = true;
			}
			Vector3 normal = this._rayInteractor.CollisionInfo.Value.Normal;
			base.transform.position = this._rayInteractor.End + normal * this._offsetAlongNormal;
			base.transform.rotation = Quaternion.LookRotation(this._rayInteractor.CollisionInfo.Value.Normal, Vector3.up);
			if (this.PlayerHead != null)
			{
				float d = Vector3.Distance(base.transform.position, this.PlayerHead.position);
				base.transform.localScale = this._startScale * d;
			}
			bool flag = this._rayInteractor.State == InteractorState.Select;
			this._renderer.material.SetFloat(this._shaderRadialGradientScale, flag ? 0.12f : 0.2f);
			this._renderer.material.SetFloat(this._shaderRadialGradientIntensity, 1f);
			this._renderer.material.SetFloat(this._shaderRadialGradientBackgroundOpacity, 1f);
			this._renderer.material.SetColor(this._shaderInnerColor, flag ? this._selectColor : this._hoverColor);
			this._renderer.material.SetColor(this._shaderOutlineColor, this._outlineColor);
		}

		private void UpdateVisualState(InteractorStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		public void InjectAllRayInteractorCursorVisual(RayInteractor rayInteractor, Renderer renderer)
		{
			this.InjectRayInteractor(rayInteractor);
			this.InjectRenderer(renderer);
		}

		public void InjectRayInteractor(RayInteractor rayInteractor)
		{
			this._rayInteractor = rayInteractor;
		}

		public void InjectRenderer(Renderer renderer)
		{
			this._renderer = renderer;
		}

		[SerializeField]
		private RayInteractor _rayInteractor;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Color _hoverColor = Color.black;

		[SerializeField]
		private Color _selectColor = Color.black;

		[SerializeField]
		private Color _outlineColor = Color.black;

		[SerializeField]
		private float _offsetAlongNormal = 0.005f;

		[Tooltip("Players head transform, used to maintain the same cursor size on screen as it is moved in the scene.")]
		[SerializeField]
		[Optional]
		private Transform _playerHead;

		private Vector3 _startScale;

		private int _shaderRadialGradientScale = Shader.PropertyToID("_RadialGradientScale");

		private int _shaderRadialGradientIntensity = Shader.PropertyToID("_RadialGradientIntensity");

		private int _shaderRadialGradientBackgroundOpacity = Shader.PropertyToID("_RadialGradientBackgroundOpacity");

		private int _shaderInnerColor = Shader.PropertyToID("_Color");

		private int _shaderOutlineColor = Shader.PropertyToID("_OutlineColor");

		protected bool _started;
	}
}
