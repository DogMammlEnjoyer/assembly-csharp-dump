using System;

namespace UnityEngine.ProBuilder.Csg
{
	internal struct Vertex
	{
		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.hasPosition = true;
				this.m_Position = value;
			}
		}

		public Color color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				this.hasColor = true;
				this.m_Color = value;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.hasNormal = true;
				this.m_Normal = value;
			}
		}

		public Vector4 tangent
		{
			get
			{
				return this.m_Tangent;
			}
			set
			{
				this.hasTangent = true;
				this.m_Tangent = value;
			}
		}

		public Vector2 uv0
		{
			get
			{
				return this.m_UV0;
			}
			set
			{
				this.hasUV0 = true;
				this.m_UV0 = value;
			}
		}

		public Vector2 uv2
		{
			get
			{
				return this.m_UV2;
			}
			set
			{
				this.hasUV2 = true;
				this.m_UV2 = value;
			}
		}

		public Vector4 uv3
		{
			get
			{
				return this.m_UV3;
			}
			set
			{
				this.hasUV3 = true;
				this.m_UV3 = value;
			}
		}

		public Vector4 uv4
		{
			get
			{
				return this.m_UV4;
			}
			set
			{
				this.hasUV4 = true;
				this.m_UV4 = value;
			}
		}

		public bool HasArrays(VertexAttributes attribute)
		{
			return (this.m_Attributes & attribute) == attribute;
		}

		public bool hasPosition
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Position) == VertexAttributes.Position;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Position) : (this.m_Attributes & ~VertexAttributes.Position));
			}
		}

		public bool hasColor
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Color) == VertexAttributes.Color;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Color) : (this.m_Attributes & ~VertexAttributes.Color));
			}
		}

		public bool hasNormal
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Normal) == VertexAttributes.Normal;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Normal) : (this.m_Attributes & ~VertexAttributes.Normal));
			}
		}

		public bool hasTangent
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Tangent) == VertexAttributes.Tangent;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Tangent) : (this.m_Attributes & ~VertexAttributes.Tangent));
			}
		}

		public bool hasUV0
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Texture0) == VertexAttributes.Texture0;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Texture0) : (this.m_Attributes & ~VertexAttributes.Texture0));
			}
		}

		public bool hasUV2
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Texture1) == VertexAttributes.Texture1;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Texture1) : (this.m_Attributes & ~VertexAttributes.Texture1));
			}
		}

		public bool hasUV3
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Texture2) == VertexAttributes.Texture2;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Texture2) : (this.m_Attributes & ~VertexAttributes.Texture2));
			}
		}

		public bool hasUV4
		{
			get
			{
				return (this.m_Attributes & VertexAttributes.Texture3) == VertexAttributes.Texture3;
			}
			private set
			{
				this.m_Attributes = (value ? (this.m_Attributes | VertexAttributes.Texture3) : (this.m_Attributes & ~VertexAttributes.Texture3));
			}
		}

		public void Flip()
		{
			if (this.hasNormal)
			{
				this.m_Normal *= -1f;
			}
			if (this.hasTangent)
			{
				this.m_Tangent *= -1f;
			}
		}

		private Vector3 m_Position;

		private Color m_Color;

		private Vector3 m_Normal;

		private Vector4 m_Tangent;

		private Vector2 m_UV0;

		private Vector2 m_UV2;

		private Vector4 m_UV3;

		private Vector4 m_UV4;

		private VertexAttributes m_Attributes;
	}
}
