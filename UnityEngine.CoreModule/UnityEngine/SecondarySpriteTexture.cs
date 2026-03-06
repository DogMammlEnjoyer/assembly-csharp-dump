using System;

namespace UnityEngine
{
	[Serializable]
	public struct SecondarySpriteTexture : IEquatable<SecondarySpriteTexture>
	{
		public bool Equals(SecondarySpriteTexture other)
		{
			return this.name == other.name && object.Equals(this.texture, other.texture);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is SecondarySpriteTexture)
			{
				SecondarySpriteTexture other = (SecondarySpriteTexture)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<string, Texture2D>(this.name, this.texture);
		}

		public static bool operator ==(SecondarySpriteTexture lhs, SecondarySpriteTexture rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SecondarySpriteTexture lhs, SecondarySpriteTexture rhs)
		{
			return !(lhs == rhs);
		}

		public string name;

		public Texture2D texture;
	}
}
