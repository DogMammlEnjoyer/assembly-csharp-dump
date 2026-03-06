using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public class PowerOfTwoTextureAtlas : Texture2DAtlas
	{
		public PowerOfTwoTextureAtlas(int size, int mipPadding, GraphicsFormat format, FilterMode filterMode = FilterMode.Point, string name = "", bool useMipMap = true) : base(size, size, format, filterMode, true, name, useMipMap)
		{
			this.m_MipPadding = mipPadding;
			int num = size & size - 1;
		}

		public int mipPadding
		{
			get
			{
				return this.m_MipPadding;
			}
		}

		private int GetTexturePadding()
		{
			return (int)Mathf.Pow(2f, (float)this.m_MipPadding) * 2;
		}

		public Vector4 GetPayloadScaleOffset(Texture texture, in Vector4 scaleOffset)
		{
			int texturePadding = this.GetTexturePadding();
			Vector2 vector = Vector2.one * (float)texturePadding;
			Vector2 powerOfTwoTextureSize = this.GetPowerOfTwoTextureSize(texture);
			return PowerOfTwoTextureAtlas.GetPayloadScaleOffset(powerOfTwoTextureSize, vector, scaleOffset);
		}

		public static Vector4 GetPayloadScaleOffset(in Vector2 textureSize, in Vector2 paddingSize, in Vector4 scaleOffset)
		{
			Vector2 a = new Vector2(scaleOffset.x, scaleOffset.y);
			Vector2 a2 = new Vector2(scaleOffset.z, scaleOffset.w);
			Vector2 b = (textureSize + paddingSize) / textureSize;
			Vector2 b2 = paddingSize / 2f / (textureSize + paddingSize);
			Vector2 vector = a / b;
			Vector2 vector2 = a2 + a * b2;
			return new Vector4(vector.x, vector.y, vector2.x, vector2.y);
		}

		private void Blit2DTexture(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips, PowerOfTwoTextureAtlas.BlitType blitType)
		{
			int num = base.GetTextureMipmapCount(texture.width, texture.height);
			int texturePadding = this.GetTexturePadding();
			Vector2 powerOfTwoTextureSize = this.GetPowerOfTwoTextureSize(texture);
			bool bilinear = texture.filterMode > FilterMode.Point;
			if (!blitMips)
			{
				num = 1;
			}
			using (new ProfilingScope(cmd, ProfilingSampler.Get<CoreProfileId>(CoreProfileId.BlitTextureInPotAtlas)))
			{
				for (int i = 0; i < num; i++)
				{
					cmd.SetRenderTarget(this.m_AtlasTexture, i);
					switch (blitType)
					{
					case PowerOfTwoTextureAtlas.BlitType.Padding:
						Blitter.BlitQuadWithPadding(cmd, texture, powerOfTwoTextureSize, sourceScaleOffset, scaleOffset, i, bilinear, texturePadding);
						break;
					case PowerOfTwoTextureAtlas.BlitType.PaddingMultiply:
						Blitter.BlitQuadWithPaddingMultiply(cmd, texture, powerOfTwoTextureSize, sourceScaleOffset, scaleOffset, i, bilinear, texturePadding);
						break;
					case PowerOfTwoTextureAtlas.BlitType.OctahedralPadding:
						Blitter.BlitOctahedralWithPadding(cmd, texture, powerOfTwoTextureSize, sourceScaleOffset, scaleOffset, i, bilinear, texturePadding);
						break;
					case PowerOfTwoTextureAtlas.BlitType.OctahedralPaddingMultiply:
						Blitter.BlitOctahedralWithPaddingMultiply(cmd, texture, powerOfTwoTextureSize, sourceScaleOffset, scaleOffset, i, bilinear, texturePadding);
						break;
					}
				}
			}
		}

		public override void BlitTexture(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips = true, int overrideInstanceID = -1)
		{
			if (base.Is2D(texture))
			{
				this.Blit2DTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips, PowerOfTwoTextureAtlas.BlitType.Padding);
				base.MarkGPUTextureValid((overrideInstanceID != -1) ? overrideInstanceID : texture.GetInstanceID(), blitMips);
			}
		}

		public void BlitTextureMultiply(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips = true, int overrideInstanceID = -1)
		{
			if (base.Is2D(texture))
			{
				this.Blit2DTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips, PowerOfTwoTextureAtlas.BlitType.PaddingMultiply);
				base.MarkGPUTextureValid((overrideInstanceID != -1) ? overrideInstanceID : texture.GetInstanceID(), blitMips);
			}
		}

		public override void BlitOctahedralTexture(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips = true, int overrideInstanceID = -1)
		{
			if (base.Is2D(texture))
			{
				this.Blit2DTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips, PowerOfTwoTextureAtlas.BlitType.OctahedralPadding);
				base.MarkGPUTextureValid((overrideInstanceID != -1) ? overrideInstanceID : texture.GetInstanceID(), blitMips);
			}
		}

		public void BlitOctahedralTextureMultiply(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips = true, int overrideInstanceID = -1)
		{
			if (base.Is2D(texture))
			{
				this.Blit2DTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips, PowerOfTwoTextureAtlas.BlitType.OctahedralPaddingMultiply);
				base.MarkGPUTextureValid((overrideInstanceID != -1) ? overrideInstanceID : texture.GetInstanceID(), blitMips);
			}
		}

		private void TextureSizeToPowerOfTwo(Texture texture, ref int width, ref int height)
		{
			width = Mathf.NextPowerOfTwo(width);
			height = Mathf.NextPowerOfTwo(height);
		}

		private Vector2 GetPowerOfTwoTextureSize(Texture texture)
		{
			int width = texture.width;
			int height = texture.height;
			this.TextureSizeToPowerOfTwo(texture, ref width, ref height);
			return new Vector2((float)width, (float)height);
		}

		public override bool AllocateTexture(CommandBuffer cmd, ref Vector4 scaleOffset, Texture texture, int width, int height, int overrideInstanceID = -1)
		{
			if (height != width)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Can't place ",
					(texture != null) ? texture.ToString() : null,
					" in the atlas ",
					this.m_AtlasTexture.name,
					": Only squared texture are allowed in this atlas."
				}));
				return false;
			}
			this.TextureSizeToPowerOfTwo(texture, ref height, ref width);
			return base.AllocateTexture(cmd, ref scaleOffset, texture, width, height, -1);
		}

		public void ResetRequestedTexture()
		{
			this.m_RequestedTextures.Clear();
		}

		public bool ReserveSpace(Texture texture)
		{
			return this.ReserveSpace(texture, texture.width, texture.height);
		}

		public bool ReserveSpace(Texture texture, int width, int height)
		{
			return this.ReserveSpace(base.GetTextureID(texture), width, height);
		}

		public bool ReserveSpace(Texture textureA, Texture textureB, int width, int height)
		{
			return this.ReserveSpace(base.GetTextureID(textureA, textureB), width, height);
		}

		public bool ReserveSpace(int id, int width, int height)
		{
			this.m_RequestedTextures[id] = new Vector2Int(width, height);
			Vector2Int cachedTextureSize = base.GetCachedTextureSize(id);
			Vector4 vector;
			if (!base.IsCached(out vector, id) || cachedTextureSize.x != width || cachedTextureSize.y != height)
			{
				Vector4 zero = Vector4.zero;
				if (!this.AllocateTextureWithoutBlit(id, width, height, ref zero))
				{
					return false;
				}
			}
			return true;
		}

		public bool RelayoutEntries()
		{
			List<ValueTuple<int, Vector2Int>> list = new List<ValueTuple<int, Vector2Int>>();
			foreach (KeyValuePair<int, Vector2Int> keyValuePair in this.m_RequestedTextures)
			{
				list.Add(new ValueTuple<int, Vector2Int>(keyValuePair.Key, keyValuePair.Value));
			}
			base.ResetAllocator();
			list.Sort(([TupleElementNames(new string[]
			{
				"instanceId",
				"size"
			})] ValueTuple<int, Vector2Int> c1, [TupleElementNames(new string[]
			{
				"instanceId",
				"size"
			})] ValueTuple<int, Vector2Int> c2) => c2.Item2.magnitude.CompareTo(c1.Item2.magnitude));
			bool flag = true;
			Vector4 zero = Vector4.zero;
			foreach (ValueTuple<int, Vector2Int> valueTuple in list)
			{
				bool flag2 = flag;
				int item = valueTuple.Item1;
				Vector2Int item2 = valueTuple.Item2;
				int x = item2.x;
				item2 = valueTuple.Item2;
				flag = (flag2 & this.AllocateTextureWithoutBlit(item, x, item2.y, ref zero));
			}
			return flag;
		}

		public static long GetApproxCacheSizeInByte(int nbElement, int resolution, bool hasMipmap, GraphicsFormat format)
		{
			return (long)((double)(nbElement * resolution * resolution) * (double)((hasMipmap ? 1.33f : 1f) * GraphicsFormatUtility.GetBlockSize(format)));
		}

		public static int GetMaxCacheSizeForWeightInByte(int weight, bool hasMipmap, GraphicsFormat format)
		{
			float num = GraphicsFormatUtility.GetBlockSize(format) * (hasMipmap ? 1.33f : 1f);
			return CoreUtils.PreviousPowerOfTwo((int)Mathf.Sqrt((float)weight / num));
		}

		private readonly int m_MipPadding;

		private const float k_MipmapFactorApprox = 1.33f;

		private Dictionary<int, Vector2Int> m_RequestedTextures = new Dictionary<int, Vector2Int>();

		private enum BlitType
		{
			Padding,
			PaddingMultiply,
			OctahedralPadding,
			OctahedralPaddingMultiply
		}
	}
}
