using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	[ExecuteAlways]
	public class MaterialPropertyBlockEditor : MonoBehaviour
	{
		public List<Renderer> Renderers
		{
			get
			{
				return this._renderers;
			}
			set
			{
				this._renderers = value;
			}
		}

		public List<MaterialPropertyVector> VectorProperties
		{
			get
			{
				return this._vectorProperties;
			}
			set
			{
				this._vectorProperties = value;
			}
		}

		public List<MaterialPropertyColor> ColorProperties
		{
			get
			{
				return this._colorProperties;
			}
			set
			{
				this._colorProperties = value;
			}
		}

		public List<MaterialPropertyFloat> FloatProperties
		{
			get
			{
				return this._floatProperties;
			}
			set
			{
				this._floatProperties = value;
			}
		}

		public MaterialPropertyBlock MaterialPropertyBlock
		{
			get
			{
				if (this._materialPropertyBlock == null)
				{
					this._materialPropertyBlock = new MaterialPropertyBlock();
				}
				return this._materialPropertyBlock;
			}
		}

		protected virtual void Awake()
		{
			if (this._renderers == null)
			{
				Renderer component = base.GetComponent<Renderer>();
				if (component != null)
				{
					this._renderers = new List<Renderer>
					{
						component
					};
				}
			}
			this.UpdateMaterialPropertyBlock();
		}

		public void UpdateMaterialPropertyBlock()
		{
			MaterialPropertyBlock materialPropertyBlock = this.MaterialPropertyBlock;
			if (this._vectorProperties != null)
			{
				foreach (MaterialPropertyVector materialPropertyVector in this._vectorProperties)
				{
					this._materialPropertyBlock.SetVector(materialPropertyVector.name, materialPropertyVector.value);
				}
			}
			if (this._colorProperties != null)
			{
				foreach (MaterialPropertyColor materialPropertyColor in this._colorProperties)
				{
					this._materialPropertyBlock.SetColor(materialPropertyColor.name, materialPropertyColor.value);
				}
			}
			if (this._floatProperties != null)
			{
				foreach (MaterialPropertyFloat materialPropertyFloat in this._floatProperties)
				{
					this._materialPropertyBlock.SetFloat(materialPropertyFloat.name, materialPropertyFloat.value);
				}
			}
			if (this._renderers != null)
			{
				foreach (Renderer renderer in this._renderers)
				{
					renderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
		}

		protected virtual void Update()
		{
			if (this._updateEveryFrame)
			{
				this.UpdateMaterialPropertyBlock();
			}
		}

		[SerializeField]
		private List<Renderer> _renderers;

		[SerializeField]
		private List<MaterialPropertyVector> _vectorProperties;

		[SerializeField]
		private List<MaterialPropertyColor> _colorProperties;

		[SerializeField]
		private List<MaterialPropertyFloat> _floatProperties;

		[SerializeField]
		private bool _updateEveryFrame = true;

		private MaterialPropertyBlock _materialPropertyBlock;
	}
}
