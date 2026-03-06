using System;

namespace UnityEngine.Rendering
{
	internal class AtlasAllocator
	{
		public AtlasAllocator(int width, int height, bool potPadding)
		{
			this.m_Root = new AtlasAllocator.AtlasNode();
			this.m_Root.m_Rect.Set((float)width, (float)height, 0f, 0f);
			this.m_Width = width;
			this.m_Height = height;
			this.powerOfTwoPadding = potPadding;
			this.m_NodePool = new ObjectPool<AtlasAllocator.AtlasNode>(delegate(AtlasAllocator.AtlasNode _)
			{
			}, delegate(AtlasAllocator.AtlasNode _)
			{
			}, true);
		}

		public bool Allocate(ref Vector4 result, int width, int height)
		{
			AtlasAllocator.AtlasNode atlasNode = this.m_Root.Allocate(ref this.m_NodePool, width, height, this.powerOfTwoPadding);
			if (atlasNode != null)
			{
				result = atlasNode.m_Rect;
				return true;
			}
			result = Vector4.zero;
			return false;
		}

		public void Reset()
		{
			this.m_Root.Release(ref this.m_NodePool);
			this.m_Root.m_Rect.Set((float)this.m_Width, (float)this.m_Height, 0f, 0f);
		}

		private AtlasAllocator.AtlasNode m_Root;

		private int m_Width;

		private int m_Height;

		private bool powerOfTwoPadding;

		private ObjectPool<AtlasAllocator.AtlasNode> m_NodePool;

		private class AtlasNode
		{
			public AtlasAllocator.AtlasNode Allocate(ref ObjectPool<AtlasAllocator.AtlasNode> pool, int width, int height, bool powerOfTwoPadding)
			{
				if (this.m_RightChild != null)
				{
					AtlasAllocator.AtlasNode atlasNode = this.m_RightChild.Allocate(ref pool, width, height, powerOfTwoPadding);
					if (atlasNode == null)
					{
						atlasNode = this.m_BottomChild.Allocate(ref pool, width, height, powerOfTwoPadding);
					}
					return atlasNode;
				}
				int num = 0;
				int num2 = 0;
				if (powerOfTwoPadding)
				{
					num = (int)this.m_Rect.x % width;
					num2 = (int)this.m_Rect.y % height;
				}
				if ((float)width <= this.m_Rect.x - (float)num && (float)height <= this.m_Rect.y - (float)num2)
				{
					this.m_RightChild = pool.Get();
					this.m_BottomChild = pool.Get();
					this.m_Rect.z = this.m_Rect.z + (float)num;
					this.m_Rect.w = this.m_Rect.w + (float)num2;
					this.m_Rect.x = this.m_Rect.x - (float)num;
					this.m_Rect.y = this.m_Rect.y - (float)num2;
					if (width > height)
					{
						this.m_RightChild.m_Rect.z = this.m_Rect.z + (float)width;
						this.m_RightChild.m_Rect.w = this.m_Rect.w;
						this.m_RightChild.m_Rect.x = this.m_Rect.x - (float)width;
						this.m_RightChild.m_Rect.y = (float)height;
						this.m_BottomChild.m_Rect.z = this.m_Rect.z;
						this.m_BottomChild.m_Rect.w = this.m_Rect.w + (float)height;
						this.m_BottomChild.m_Rect.x = this.m_Rect.x;
						this.m_BottomChild.m_Rect.y = this.m_Rect.y - (float)height;
					}
					else
					{
						this.m_RightChild.m_Rect.z = this.m_Rect.z + (float)width;
						this.m_RightChild.m_Rect.w = this.m_Rect.w;
						this.m_RightChild.m_Rect.x = this.m_Rect.x - (float)width;
						this.m_RightChild.m_Rect.y = this.m_Rect.y;
						this.m_BottomChild.m_Rect.z = this.m_Rect.z;
						this.m_BottomChild.m_Rect.w = this.m_Rect.w + (float)height;
						this.m_BottomChild.m_Rect.x = (float)width;
						this.m_BottomChild.m_Rect.y = this.m_Rect.y - (float)height;
					}
					this.m_Rect.x = (float)width;
					this.m_Rect.y = (float)height;
					return this;
				}
				return null;
			}

			public void Release(ref ObjectPool<AtlasAllocator.AtlasNode> pool)
			{
				if (this.m_RightChild != null)
				{
					this.m_RightChild.Release(ref pool);
					this.m_BottomChild.Release(ref pool);
					pool.Release(this.m_RightChild);
					pool.Release(this.m_BottomChild);
				}
				this.m_RightChild = null;
				this.m_BottomChild = null;
				this.m_Rect = Vector4.zero;
			}

			public AtlasAllocator.AtlasNode m_RightChild;

			public AtlasAllocator.AtlasNode m_BottomChild;

			public Vector4 m_Rect = new Vector4(0f, 0f, 0f, 0f);
		}
	}
}
