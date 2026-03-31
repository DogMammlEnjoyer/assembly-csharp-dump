using System;
using UnityEngine;

namespace Voxels
{
	public class TextureArrayUtil : MonoBehaviour
	{
		private bool UnreadableTextureFound
		{
			get
			{
				return !this.TexturesReadable;
			}
		}

		private bool TexturesReadable
		{
			get
			{
				foreach (TextureEntry textureEntry in this.textureEntries)
				{
					if (textureEntry.Diffuse == null || textureEntry.Normal == null)
					{
						return false;
					}
					if (!textureEntry.Diffuse.isReadable || !textureEntry.Normal.isReadable)
					{
						return false;
					}
				}
				return true;
			}
		}

		public TextureEntry[] textureEntries;

		public Texture2DArray diffuseArray;

		public Texture2DArray normalArray;

		public Material material;

		public bool linearNormalMaps = true;

		public string diffuseName = "_Diffuse";

		public string normalName = "_Normal";
	}
}
