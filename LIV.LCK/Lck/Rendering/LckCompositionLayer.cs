using System;
using UnityEngine;

namespace Liv.Lck.Rendering
{
	[Serializable]
	public class LckCompositionLayer : ILckCompositionLayer
	{
		public virtual Texture CurrentTexture
		{
			get
			{
				return null;
			}
		}

		string ILckCompositionLayer.Name
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		Material ILckCompositionLayer.BlendMaterial
		{
			get
			{
				return this.BlendMaterial;
			}
			set
			{
				this.BlendMaterial = value;
			}
		}

		bool ILckCompositionLayer.IsActive
		{
			get
			{
				return this.IsActive;
			}
			set
			{
				this.IsActive = value;
			}
		}

		[Tooltip("A descriptive name for this layer. Can be used to find and control it at runtime.")]
		public string Name;

		[Tooltip("The material used to perform the blend.")]
		public Material BlendMaterial;

		public bool IsActive = true;
	}
}
