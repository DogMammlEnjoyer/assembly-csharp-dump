using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderMaterialPropertyCacheHelper
	{
		private bool AllNonTexturePropertyValuesAreEqual(string prop)
		{
			bool flag = false;
			object obj = null;
			foreach (TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair materialPropertyPair in this.nonTexturePropertyValuesForSourceMaterials.Keys)
			{
				if (materialPropertyPair.property.Equals(prop))
				{
					if (!flag)
					{
						obj = this.nonTexturePropertyValuesForSourceMaterials[materialPropertyPair];
						flag = true;
					}
					else if (!obj.Equals(this.nonTexturePropertyValuesForSourceMaterials[materialPropertyPair]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void CacheMaterialProperty(Material m, string property, object value)
		{
			this.nonTexturePropertyValuesForSourceMaterials[new TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair(m, property)] = value;
		}

		public object GetValueIfAllSourceAreTheSameOrDefault(string property, object defaultValue)
		{
			if (this.AllNonTexturePropertyValuesAreEqual(property))
			{
				foreach (TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair materialPropertyPair in this.nonTexturePropertyValuesForSourceMaterials.Keys)
				{
					if (materialPropertyPair.property.Equals(property))
					{
						return this.nonTexturePropertyValuesForSourceMaterials[materialPropertyPair];
					}
				}
				return defaultValue;
			}
			return defaultValue;
		}

		private Dictionary<TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair, object> nonTexturePropertyValuesForSourceMaterials = new Dictionary<TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair, object>();

		private struct MaterialPropertyPair
		{
			public MaterialPropertyPair(Material m, string prop)
			{
				this.material = m;
				this.property = prop;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair))
				{
					return false;
				}
				TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair materialPropertyPair = (TextureBlenderMaterialPropertyCacheHelper.MaterialPropertyPair)obj;
				return this.material.Equals(materialPropertyPair.material) && !(this.property != materialPropertyPair.property);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public Material material;

			public string property;
		}
	}
}
