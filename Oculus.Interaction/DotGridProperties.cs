using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[ExecuteAlways]
	public class DotGridProperties : MonoBehaviour
	{
		public int Columns
		{
			get
			{
				return this._columns;
			}
			set
			{
				this._columns = value;
			}
		}

		public int Rows
		{
			get
			{
				return this._rows;
			}
			set
			{
				this._rows = value;
			}
		}

		public float Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
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

		protected virtual void Start()
		{
			this._change = true;
		}

		protected virtual void Update()
		{
			if (!this._change || this._materialPropertyBlockEditor == null)
			{
				return;
			}
			MaterialPropertyBlock materialPropertyBlock = this._materialPropertyBlockEditor.MaterialPropertyBlock;
			materialPropertyBlock.SetColor(this._colorShaderID, this._color);
			materialPropertyBlock.SetVector(this._dimensionsShaderID, new Vector4((float)this._columns, (float)this._rows, this._radius, 0f));
			this._materialPropertyBlockEditor.UpdateMaterialPropertyBlock();
			this._change = false;
		}

		protected virtual void OnValidate()
		{
			this._change = true;
		}

		public void InjectAllDotGridProperties(MaterialPropertyBlockEditor editor)
		{
			this.InjectMaterialPropertyBlockEditor(editor);
		}

		public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor editor)
		{
			this._materialPropertyBlockEditor = editor;
		}

		[SerializeField]
		private MaterialPropertyBlockEditor _materialPropertyBlockEditor;

		[SerializeField]
		private int _columns;

		[SerializeField]
		private int _rows;

		[SerializeField]
		private float _radius;

		[SerializeField]
		private Color _color;

		private bool _change;

		private readonly int _colorShaderID = Shader.PropertyToID("_Color");

		private readonly int _dimensionsShaderID = Shader.PropertyToID("_Dimensions");
	}
}
