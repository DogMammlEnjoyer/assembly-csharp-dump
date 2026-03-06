using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public class Texture2DAtlas
	{
		public static int maxMipLevelPadding
		{
			get
			{
				return Texture2DAtlas.s_MaxMipLevelPadding;
			}
		}

		public RTHandle AtlasTexture
		{
			get
			{
				return this.m_AtlasTexture;
			}
		}

		public Texture2DAtlas(int width, int height, GraphicsFormat format, FilterMode filterMode = FilterMode.Point, bool powerOfTwoPadding = false, string name = "", bool useMipMap = true)
		{
			this.m_Width = width;
			this.m_Height = height;
			this.m_Format = format;
			this.m_UseMipMaps = useMipMap;
			this.m_AtlasTexture = RTHandles.Alloc(this.m_Width, this.m_Height, this.m_Format, 1, filterMode, TextureWrapMode.Clamp, TextureDimension.Tex2D, false, useMipMap, false, false, 1, 0f, MSAASamples.None, false, false, false, RenderTextureMemoryless.None, VRTextureUsage.None, name);
			this.m_IsAtlasTextureOwner = true;
			int num = useMipMap ? this.GetTextureMipmapCount(this.m_Width, this.m_Height) : 1;
			for (int i = 0; i < num; i++)
			{
				Graphics.SetRenderTarget(this.m_AtlasTexture, i);
				GL.Clear(false, true, Color.clear);
			}
			this.m_AtlasAllocator = new AtlasAllocator(width, height, powerOfTwoPadding);
		}

		public void Release()
		{
			this.ResetAllocator();
			if (this.m_IsAtlasTextureOwner)
			{
				RTHandles.Release(this.m_AtlasTexture);
			}
		}

		public void ResetAllocator()
		{
			this.m_AtlasAllocator.Reset();
			this.m_AllocationCache.Clear();
			this.m_IsGPUTextureUpToDate.Clear();
		}

		public void ClearTarget(CommandBuffer cmd)
		{
			int num = this.m_UseMipMaps ? this.GetTextureMipmapCount(this.m_Width, this.m_Height) : 1;
			for (int i = 0; i < num; i++)
			{
				cmd.SetRenderTarget(this.m_AtlasTexture, i);
				Blitter.BlitQuad(cmd, Texture2D.blackTexture, Texture2DAtlas.fullScaleOffset, Texture2DAtlas.fullScaleOffset, i, true);
			}
			this.m_IsGPUTextureUpToDate.Clear();
		}

		private protected int GetTextureMipmapCount(int width, int height)
		{
			if (!this.m_UseMipMaps)
			{
				return 1;
			}
			return CoreUtils.GetMipCount((float)Mathf.Max(width, height));
		}

		private protected bool Is2D(Texture texture)
		{
			RenderTexture renderTexture = texture as RenderTexture;
			return texture is Texture2D || (renderTexture != null && renderTexture.dimension == TextureDimension.Tex2D);
		}

		private protected bool IsSingleChannelBlit(Texture source, Texture destination)
		{
			uint componentCount = GraphicsFormatUtility.GetComponentCount(source.graphicsFormat);
			uint componentCount2 = GraphicsFormatUtility.GetComponentCount(destination.graphicsFormat);
			if (componentCount == 1U || componentCount2 == 1U)
			{
				if (componentCount != componentCount2)
				{
					return true;
				}
				int num = 1 << (int)(GraphicsFormatUtility.GetSwizzleA(source.graphicsFormat) & (FormatSwizzle)7) << 24 | 1 << (int)(GraphicsFormatUtility.GetSwizzleB(source.graphicsFormat) & (FormatSwizzle)7) << 16 | 1 << (int)(GraphicsFormatUtility.GetSwizzleG(source.graphicsFormat) & (FormatSwizzle)7) << 8 | 1 << (int)(GraphicsFormatUtility.GetSwizzleR(source.graphicsFormat) & (FormatSwizzle)7);
				int num2 = 1 << (int)(GraphicsFormatUtility.GetSwizzleA(destination.graphicsFormat) & (FormatSwizzle)7) << 24 | 1 << (int)(GraphicsFormatUtility.GetSwizzleB(destination.graphicsFormat) & (FormatSwizzle)7) << 16 | 1 << (int)(GraphicsFormatUtility.GetSwizzleG(destination.graphicsFormat) & (FormatSwizzle)7) << 8 | 1 << (int)(GraphicsFormatUtility.GetSwizzleR(destination.graphicsFormat) & (FormatSwizzle)7);
				if (num != num2)
				{
					return true;
				}
			}
			return false;
		}

		private void Blit2DTexture(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips, Texture2DAtlas.BlitType blitType)
		{
			int num = this.GetTextureMipmapCount(texture.width, texture.height);
			if (!blitMips)
			{
				num = 1;
			}
			for (int i = 0; i < num; i++)
			{
				cmd.SetRenderTarget(this.m_AtlasTexture, i);
				switch (blitType)
				{
				case Texture2DAtlas.BlitType.Default:
					Blitter.BlitQuad(cmd, texture, sourceScaleOffset, scaleOffset, i, true);
					break;
				case Texture2DAtlas.BlitType.CubeTo2DOctahedral:
					Blitter.BlitCubeToOctahedral2DQuad(cmd, texture, scaleOffset, i);
					break;
				case Texture2DAtlas.BlitType.SingleChannel:
					Blitter.BlitQuadSingleChannel(cmd, texture, sourceScaleOffset, scaleOffset, i);
					break;
				case Texture2DAtlas.BlitType.CubeTo2DOctahedralSingleChannel:
					Blitter.BlitCubeToOctahedral2DQuadSingleChannel(cmd, texture, scaleOffset, i);
					break;
				}
			}
		}

		private protected void MarkGPUTextureValid(int instanceId, bool mipAreValid = false)
		{
			this.m_IsGPUTextureUpToDate[instanceId] = (mipAreValid ? 2 : 1);
		}

		private protected void MarkGPUTextureInvalid(int instanceId)
		{
			this.m_IsGPUTextureUpToDate[instanceId] = 0;
		}

		public virtual void BlitTexture(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips = true, int overrideInstanceID = -1)
		{
			if (this.Is2D(texture))
			{
				Texture2DAtlas.BlitType blitType = Texture2DAtlas.BlitType.Default;
				if (this.IsSingleChannelBlit(texture, this.m_AtlasTexture.m_RT))
				{
					blitType = Texture2DAtlas.BlitType.SingleChannel;
				}
				this.Blit2DTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips, blitType);
				int num = (overrideInstanceID != -1) ? overrideInstanceID : this.GetTextureID(texture);
				this.MarkGPUTextureValid(num, blitMips);
				this.m_TextureHashes[num] = CoreUtils.GetTextureHash(texture);
			}
		}

		public virtual void BlitOctahedralTexture(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, Vector4 sourceScaleOffset, bool blitMips = true, int overrideInstanceID = -1)
		{
			this.BlitTexture(cmd, scaleOffset, texture, sourceScaleOffset, blitMips, overrideInstanceID);
		}

		public virtual void BlitCubeTexture2D(CommandBuffer cmd, Vector4 scaleOffset, Texture texture, bool blitMips = true, int overrideInstanceID = -1)
		{
			if (texture.dimension == TextureDimension.Cube)
			{
				Texture2DAtlas.BlitType blitType = Texture2DAtlas.BlitType.CubeTo2DOctahedral;
				if (this.IsSingleChannelBlit(texture, this.m_AtlasTexture.m_RT))
				{
					blitType = Texture2DAtlas.BlitType.CubeTo2DOctahedralSingleChannel;
				}
				this.Blit2DTexture(cmd, scaleOffset, texture, new Vector4(1f, 1f, 0f, 0f), blitMips, blitType);
				int num = (overrideInstanceID != -1) ? overrideInstanceID : this.GetTextureID(texture);
				this.MarkGPUTextureValid(num, blitMips);
				this.m_TextureHashes[num] = CoreUtils.GetTextureHash(texture);
			}
		}

		public virtual bool AllocateTexture(CommandBuffer cmd, ref Vector4 scaleOffset, Texture texture, int width, int height, int overrideInstanceID = -1)
		{
			int num = (overrideInstanceID != -1) ? overrideInstanceID : this.GetTextureID(texture);
			bool flag = this.AllocateTextureWithoutBlit(num, width, height, ref scaleOffset);
			if (flag)
			{
				if (this.Is2D(texture))
				{
					this.BlitTexture(cmd, scaleOffset, texture, Texture2DAtlas.fullScaleOffset, true, -1);
				}
				else
				{
					this.BlitCubeTexture2D(cmd, scaleOffset, texture, true, -1);
				}
				this.MarkGPUTextureValid(num, true);
				this.m_TextureHashes[num] = CoreUtils.GetTextureHash(texture);
			}
			return flag;
		}

		public bool AllocateTextureWithoutBlit(Texture texture, int width, int height, ref Vector4 scaleOffset)
		{
			return this.AllocateTextureWithoutBlit(texture.GetInstanceID(), width, height, ref scaleOffset);
		}

		public virtual bool AllocateTextureWithoutBlit(int instanceId, int width, int height, ref Vector4 scaleOffset)
		{
			scaleOffset = Vector4.zero;
			if (this.m_AtlasAllocator.Allocate(ref scaleOffset, width, height))
			{
				scaleOffset.Scale(new Vector4(1f / (float)this.m_Width, 1f / (float)this.m_Height, 1f / (float)this.m_Width, 1f / (float)this.m_Height));
				this.m_AllocationCache[instanceId] = new ValueTuple<Vector4, Vector2Int>(scaleOffset, new Vector2Int(width, height));
				this.MarkGPUTextureInvalid(instanceId);
				this.m_TextureHashes[instanceId] = -1;
				return true;
			}
			return false;
		}

		private protected int GetTextureHash(Texture textureA, Texture textureB)
		{
			return CoreUtils.GetTextureHash(textureA) + 23 * CoreUtils.GetTextureHash(textureB);
		}

		public int GetTextureID(Texture texture)
		{
			return texture.GetInstanceID();
		}

		public int GetTextureID(Texture textureA, Texture textureB)
		{
			return this.GetTextureID(textureA) + 23 * this.GetTextureID(textureB);
		}

		public bool IsCached(out Vector4 scaleOffset, Texture textureA, Texture textureB)
		{
			return this.IsCached(out scaleOffset, this.GetTextureID(textureA, textureB));
		}

		public bool IsCached(out Vector4 scaleOffset, Texture texture)
		{
			return this.IsCached(out scaleOffset, this.GetTextureID(texture));
		}

		public bool IsCached(out Vector4 scaleOffset, int id)
		{
			ValueTuple<Vector4, Vector2Int> valueTuple;
			bool result = this.m_AllocationCache.TryGetValue(id, out valueTuple);
			scaleOffset = valueTuple.Item1;
			return result;
		}

		internal Vector2Int GetCachedTextureSize(int id)
		{
			ValueTuple<Vector4, Vector2Int> valueTuple;
			this.m_AllocationCache.TryGetValue(id, out valueTuple);
			return valueTuple.Item2;
		}

		public virtual bool NeedsUpdate(Texture texture, bool needMips = false)
		{
			RenderTexture renderTexture = texture as RenderTexture;
			int textureID = this.GetTextureID(texture);
			int textureHash = CoreUtils.GetTextureHash(texture);
			if (renderTexture != null)
			{
				int num;
				if (this.m_IsGPUTextureUpToDate.TryGetValue(textureID, out num))
				{
					if ((ulong)renderTexture.updateCount != (ulong)((long)num))
					{
						this.m_IsGPUTextureUpToDate[textureID] = (int)renderTexture.updateCount;
						return true;
					}
				}
				else
				{
					this.m_IsGPUTextureUpToDate[textureID] = (int)renderTexture.updateCount;
				}
			}
			else
			{
				int num2;
				if (this.m_TextureHashes.TryGetValue(textureID, out num2) && num2 != textureHash)
				{
					this.m_TextureHashes[textureID] = textureHash;
					return true;
				}
				int num3;
				if (this.m_IsGPUTextureUpToDate.TryGetValue(textureID, out num3))
				{
					return num3 == 0 || (needMips && num3 == 1);
				}
			}
			return false;
		}

		public virtual bool NeedsUpdate(int id, int updateCount, bool needMips = false)
		{
			int num;
			if (this.m_IsGPUTextureUpToDate.TryGetValue(id, out num))
			{
				if (updateCount != num)
				{
					this.m_IsGPUTextureUpToDate[id] = updateCount;
					return true;
				}
			}
			else
			{
				this.m_IsGPUTextureUpToDate[id] = updateCount;
			}
			return false;
		}

		public virtual bool NeedsUpdate(Texture textureA, Texture textureB, bool needMips = false)
		{
			RenderTexture renderTexture = textureA as RenderTexture;
			RenderTexture renderTexture2 = textureB as RenderTexture;
			int textureID = this.GetTextureID(textureA, textureB);
			int textureHash = this.GetTextureHash(textureA, textureB);
			if (renderTexture != null || renderTexture2 != null)
			{
				int num;
				if (this.m_IsGPUTextureUpToDate.TryGetValue(textureID, out num))
				{
					if (renderTexture != null && renderTexture2 != null && (ulong)Math.Min(renderTexture.updateCount, renderTexture2.updateCount) != (ulong)((long)num))
					{
						this.m_IsGPUTextureUpToDate[textureID] = (int)Math.Min(renderTexture.updateCount, renderTexture2.updateCount);
						return true;
					}
					if (renderTexture != null && (ulong)renderTexture.updateCount != (ulong)((long)num))
					{
						this.m_IsGPUTextureUpToDate[textureID] = (int)renderTexture.updateCount;
						return true;
					}
					if (renderTexture2 != null && (ulong)renderTexture2.updateCount != (ulong)((long)num))
					{
						this.m_IsGPUTextureUpToDate[textureID] = (int)renderTexture2.updateCount;
						return true;
					}
				}
				else
				{
					this.m_IsGPUTextureUpToDate[textureID] = textureHash;
				}
			}
			else
			{
				int num2;
				if (this.m_TextureHashes.TryGetValue(textureID, out num2) && num2 != textureHash)
				{
					this.m_TextureHashes[textureID] = textureID;
					return true;
				}
				int num3;
				if (this.m_IsGPUTextureUpToDate.TryGetValue(textureID, out num3))
				{
					return num3 == 0 || (needMips && num3 == 1);
				}
			}
			return false;
		}

		public virtual bool AddTexture(CommandBuffer cmd, ref Vector4 scaleOffset, Texture texture)
		{
			return this.IsCached(out scaleOffset, texture) || this.AllocateTexture(cmd, ref scaleOffset, texture, texture.width, texture.height, -1);
		}

		public virtual bool UpdateTexture(CommandBuffer cmd, Texture oldTexture, Texture newTexture, ref Vector4 scaleOffset, Vector4 sourceScaleOffset, bool updateIfNeeded = true, bool blitMips = true)
		{
			if (this.IsCached(out scaleOffset, oldTexture))
			{
				if (updateIfNeeded && this.NeedsUpdate(newTexture, false))
				{
					if (this.Is2D(newTexture))
					{
						this.BlitTexture(cmd, scaleOffset, newTexture, sourceScaleOffset, blitMips, -1);
					}
					else
					{
						this.BlitCubeTexture2D(cmd, scaleOffset, newTexture, blitMips, -1);
					}
					this.MarkGPUTextureValid(this.GetTextureID(newTexture), blitMips);
				}
				return true;
			}
			return this.AllocateTexture(cmd, ref scaleOffset, newTexture, newTexture.width, newTexture.height, -1);
		}

		public virtual bool UpdateTexture(CommandBuffer cmd, Texture texture, ref Vector4 scaleOffset, bool updateIfNeeded = true, bool blitMips = true)
		{
			return this.UpdateTexture(cmd, texture, texture, ref scaleOffset, Texture2DAtlas.fullScaleOffset, updateIfNeeded, blitMips);
		}

		internal bool EnsureTextureSlot(out bool isUploadNeeded, ref Vector4 scaleBias, int key, int width, int height)
		{
			isUploadNeeded = false;
			ValueTuple<Vector4, Vector2Int> valueTuple;
			if (this.m_AllocationCache.TryGetValue(key, out valueTuple))
			{
				scaleBias = valueTuple.Item1;
				return true;
			}
			if (!this.m_AtlasAllocator.Allocate(ref scaleBias, width, height))
			{
				return false;
			}
			isUploadNeeded = true;
			scaleBias.Scale(new Vector4(1f / (float)this.m_Width, 1f / (float)this.m_Height, 1f / (float)this.m_Width, 1f / (float)this.m_Height));
			this.m_AllocationCache.Add(key, new ValueTuple<Vector4, Vector2Int>(scaleBias, new Vector2Int(width, height)));
			return true;
		}

		private protected const int kGPUTexInvalid = 0;

		private protected const int kGPUTexValidMip0 = 1;

		private protected const int kGPUTexValidMipAll = 2;

		private protected RTHandle m_AtlasTexture;

		private protected int m_Width;

		private protected int m_Height;

		private protected GraphicsFormat m_Format;

		private protected bool m_UseMipMaps;

		private bool m_IsAtlasTextureOwner;

		private AtlasAllocator m_AtlasAllocator;

		[TupleElementNames(new string[]
		{
			"scaleOffset",
			"size"
		})]
		private Dictionary<int, ValueTuple<Vector4, Vector2Int>> m_AllocationCache = new Dictionary<int, ValueTuple<Vector4, Vector2Int>>();

		private Dictionary<int, int> m_IsGPUTextureUpToDate = new Dictionary<int, int>();

		private Dictionary<int, int> m_TextureHashes = new Dictionary<int, int>();

		private static readonly Vector4 fullScaleOffset = new Vector4(1f, 1f, 0f, 0f);

		private static readonly int s_MaxMipLevelPadding = 10;

		private enum BlitType
		{
			Default,
			CubeTo2DOctahedral,
			SingleChannel,
			CubeTo2DOctahedralSingleChannel
		}
	}
}
