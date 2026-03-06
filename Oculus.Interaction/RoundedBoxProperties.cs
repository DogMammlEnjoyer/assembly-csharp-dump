using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[ExecuteAlways]
	public class RoundedBoxProperties : MonoBehaviour
	{
		public float Width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = value;
				this.UpdateSize();
			}
		}

		public float Height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
				this.UpdateSize();
			}
		}

		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

		public Color BorderColor
		{
			get
			{
				return this._borderColor;
			}
			set
			{
				this._borderColor = value;
			}
		}

		public float RadiusTopLeft
		{
			get
			{
				return this._radiusTopLeft;
			}
			set
			{
				this._radiusTopLeft = value;
			}
		}

		public float RadiusTopRight
		{
			get
			{
				return this._radiusTopRight;
			}
			set
			{
				this._radiusTopRight = value;
			}
		}

		public float RadiusBottomLeft
		{
			get
			{
				return this._radiusBottomLeft;
			}
			set
			{
				this._radiusBottomLeft = value;
			}
		}

		public float RadiusBottomRight
		{
			get
			{
				return this._radiusBottomRight;
			}
			set
			{
				this._radiusBottomRight = value;
			}
		}

		public float BorderInnerRadius
		{
			get
			{
				return this._borderInnerRadius;
			}
			set
			{
				this._borderInnerRadius = value;
			}
		}

		public float BorderOuterRadius
		{
			get
			{
				return this._borderOuterRadius;
			}
			set
			{
				this._borderOuterRadius = value;
				this.UpdateSize();
			}
		}

		protected virtual void Awake()
		{
			this.UpdateSize();
			this.UpdateMaterialPropertyBlock();
		}

		protected virtual void Start()
		{
			this.UpdateSize();
			this.UpdateMaterialPropertyBlock();
		}

		private void UpdateSize()
		{
			base.transform.localScale = new Vector3(this._width + this._borderOuterRadius * 2f, this._height + this._borderOuterRadius * 2f, 1f);
			this.UpdateMaterialPropertyBlock();
		}

		private void UpdateMaterialPropertyBlock()
		{
			if (this._editor == null)
			{
				this._editor = base.GetComponent<MaterialPropertyBlockEditor>();
				if (this._editor == null)
				{
					return;
				}
			}
			MaterialPropertyBlock materialPropertyBlock = this._editor.MaterialPropertyBlock;
			materialPropertyBlock.SetColor(this._colorShaderID, this._color);
			materialPropertyBlock.SetColor(this._borderColorShaderID, this._borderColor);
			materialPropertyBlock.SetVector(this._radiiShaderID, new Vector4(this._radiusTopRight, this._radiusBottomRight, this._radiusTopLeft, this._radiusBottomLeft));
			materialPropertyBlock.SetVector(this._dimensionsShaderID, new Vector4(base.transform.localScale.x, base.transform.localScale.y, this._borderInnerRadius, this._borderOuterRadius));
			this._editor.UpdateMaterialPropertyBlock();
		}

		protected virtual void OnValidate()
		{
			this.UpdateSize();
			this.UpdateMaterialPropertyBlock();
		}

		[SerializeField]
		private MaterialPropertyBlockEditor _editor;

		[SerializeField]
		private float _width = 1f;

		[SerializeField]
		private float _height = 1f;

		[SerializeField]
		private Color _color = Color.white;

		[SerializeField]
		private Color _borderColor = Color.black;

		[SerializeField]
		private float _radiusTopLeft;

		[SerializeField]
		private float _radiusTopRight;

		[SerializeField]
		private float _radiusBottomLeft;

		[SerializeField]
		private float _radiusBottomRight;

		[SerializeField]
		private float _borderInnerRadius;

		[SerializeField]
		private float _borderOuterRadius;

		private readonly int _colorShaderID = Shader.PropertyToID("_Color");

		private readonly int _borderColorShaderID = Shader.PropertyToID("_BorderColor");

		private readonly int _radiiShaderID = Shader.PropertyToID("_Radii");

		private readonly int _dimensionsShaderID = Shader.PropertyToID("_Dimensions");
	}
}
