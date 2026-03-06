using System;
using Unity.Mathematics;

namespace Drawing.Text
{
	internal struct SDFCharacter
	{
		public float2 uvTopLeft
		{
			get
			{
				return this.uvtopleft;
			}
		}

		public float2 uvTopRight
		{
			get
			{
				return new float2(this.uvbottomright.x, this.uvtopleft.y);
			}
		}

		public float2 uvBottomLeft
		{
			get
			{
				return new float2(this.uvtopleft.x, this.uvbottomright.y);
			}
		}

		public float2 uvBottomRight
		{
			get
			{
				return this.uvbottomright;
			}
		}

		public float2 vertexTopLeft
		{
			get
			{
				return this.vtopleft;
			}
		}

		public float2 vertexTopRight
		{
			get
			{
				return new float2(this.vbottomright.x, this.vtopleft.y);
			}
		}

		public float2 vertexBottomLeft
		{
			get
			{
				return new float2(this.vtopleft.x, this.vbottomright.y);
			}
		}

		public float2 vertexBottomRight
		{
			get
			{
				return this.vbottomright;
			}
		}

		public SDFCharacter(char codePoint, int x, int y, int width, int height, int originX, int originY, int advance, int textureWidth, int textureHeight, float defaultSize)
		{
			float2 rhs = new float2((float)textureWidth, (float)textureHeight);
			this.codePoint = codePoint;
			float2 @float = new float2((float)x, (float)y) / rhs;
			float2 float2 = new float2((float)(x + width), (float)(y + height)) / rhs;
			this.uvtopleft = new float2(@float.x, 1f - @float.y);
			this.uvbottomright = new float2(float2.x, 1f - float2.y);
			float2 lhs = new float2((float)(-(float)originX), (float)originY);
			this.vtopleft = (lhs + new float2(0f, 0f)) / defaultSize;
			this.vbottomright = (lhs + new float2((float)width, (float)(-(float)height))) / defaultSize;
			this.advance = (float)advance / defaultSize;
		}

		public char codePoint;

		private float2 uvtopleft;

		private float2 uvbottomright;

		private float2 vtopleft;

		private float2 vbottomright;

		public float advance;
	}
}
