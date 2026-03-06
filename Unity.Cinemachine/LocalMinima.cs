using System;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal struct LocalMinima
	{
		public LocalMinima(Vertex vertex, PathType polytype, bool isOpen = false)
		{
			this.vertex = vertex;
			this.polytype = polytype;
			this.isOpen = isOpen;
		}

		public static bool operator ==(LocalMinima lm1, LocalMinima lm2)
		{
			return lm1.vertex == lm2.vertex;
		}

		public static bool operator !=(LocalMinima lm1, LocalMinima lm2)
		{
			return !(lm1 == lm2);
		}

		public override bool Equals(object obj)
		{
			if (obj is LocalMinima)
			{
				LocalMinima lm = (LocalMinima)obj;
				return this == lm;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.vertex.GetHashCode();
		}

		public readonly Vertex vertex;

		public readonly PathType polytype;

		public readonly bool isOpen;
	}
}
