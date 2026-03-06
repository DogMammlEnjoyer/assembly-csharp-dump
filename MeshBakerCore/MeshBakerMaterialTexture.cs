using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MeshBakerMaterialTexture
	{
		public Texture2D t
		{
			set
			{
				this._t = value;
			}
		}

		public DRect matTilingRect { get; private set; }

		public int isImportedAsNormalMap { get; private set; }

		public MeshBakerMaterialTexture(Texture tx, Vector2 matTilingOffset, Vector2 matTilingScale, float texelDens, int isImportedAsNormalMap)
		{
			if (tx is Texture2D)
			{
				this._t = (Texture2D)tx;
			}
			else if (!(tx == null))
			{
				Debug.LogError("An error occured. Texture must be Texture2D " + ((tx != null) ? tx.ToString() : null));
			}
			this.matTilingRect = new DRect(matTilingOffset, matTilingScale);
			this.texelDensity = texelDens;
			this.isImportedAsNormalMap = isImportedAsNormalMap;
		}

		public DRect GetEncapsulatingSamplingRect()
		{
			return this.encapsulatingSamplingRect;
		}

		public void SetEncapsulatingSamplingRect(MB_TexSet ts, DRect r)
		{
			this.encapsulatingSamplingRect = r;
		}

		public Texture2D GetTexture2D()
		{
			if (!MeshBakerMaterialTexture.readyToBuildAtlases)
			{
				Debug.LogError("This function should not be called before Step3. For steps 1 and 2 should always call methods like isNull, width, height");
				throw new Exception("GetTexture2D called before ready to build atlases");
			}
			return this._t;
		}

		public bool isNull
		{
			get
			{
				return this._t == null;
			}
		}

		public int width
		{
			get
			{
				if (this._t != null)
				{
					return this._t.width;
				}
				return 16;
			}
		}

		public int height
		{
			get
			{
				if (this._t != null)
				{
					return this._t.height;
				}
				return 16;
			}
		}

		public string GetTexName()
		{
			if (this._t != null)
			{
				return this._t.name;
			}
			return "null";
		}

		public bool AreTexturesEqual(MeshBakerMaterialTexture b)
		{
			return this._t == b._t;
		}

		private Texture2D _t;

		public float texelDensity;

		internal static bool readyToBuildAtlases;

		private DRect encapsulatingSamplingRect;
	}
}
