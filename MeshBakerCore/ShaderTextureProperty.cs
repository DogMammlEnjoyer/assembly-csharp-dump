using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class ShaderTextureProperty
	{
		public ShaderTextureProperty(string n, bool norm)
		{
			this.name = n;
			this.isNormalMap = norm;
			this.isGammaCorrected = !this.isNormalMap;
			this.isNormalDontKnow = false;
		}

		public ShaderTextureProperty(string n, bool norm, bool isGamma, bool isNormalDontKnow)
		{
			this.name = n;
			this.isNormalMap = norm;
			this.isGammaCorrected = isGamma;
			this.isNormalDontKnow = isNormalDontKnow;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ShaderTextureProperty))
			{
				return false;
			}
			ShaderTextureProperty shaderTextureProperty = (ShaderTextureProperty)obj;
			return this.name.Equals(shaderTextureProperty.name) && this.isNormalMap == shaderTextureProperty.isNormalMap;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static string[] GetNames(List<ShaderTextureProperty> props)
		{
			string[] array = new string[props.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = props[i].name;
			}
			return array;
		}

		public string name;

		public bool isNormalMap;

		public bool isGammaCorrected;

		[HideInInspector]
		public bool isNormalDontKnow;
	}
}
