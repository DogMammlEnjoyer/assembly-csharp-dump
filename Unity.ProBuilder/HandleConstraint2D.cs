using System;

namespace UnityEngine.ProBuilder
{
	internal sealed class HandleConstraint2D
	{
		public HandleConstraint2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public HandleConstraint2D Inverse()
		{
			return new HandleConstraint2D((this.x == 1) ? 0 : 1, (this.y == 1) ? 0 : 1);
		}

		public Vector2 Mask(Vector2 v)
		{
			v.x *= (float)this.x;
			v.y *= (float)this.y;
			return v;
		}

		public Vector2 InverseMask(Vector2 v)
		{
			v.x *= ((this.x == 1) ? 0f : 1f);
			v.y *= ((this.y == 1) ? 0f : 1f);
			return v;
		}

		public static bool operator ==(HandleConstraint2D a, HandleConstraint2D b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(HandleConstraint2D a, HandleConstraint2D b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			return o is HandleConstraint2D && ((HandleConstraint2D)o).x == this.x && ((HandleConstraint2D)o).y == this.y;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"(",
				this.x.ToString(),
				", ",
				this.y.ToString(),
				")"
			});
		}

		public int x;

		public int y;

		public static readonly HandleConstraint2D None = new HandleConstraint2D(1, 1);
	}
}
