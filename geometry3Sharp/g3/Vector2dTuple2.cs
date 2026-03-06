using System;

namespace g3
{
	public struct Vector2dTuple2
	{
		public Vector2dTuple2(Vector2d v0, Vector2d v1)
		{
			this.V0 = v0;
			this.V1 = v1;
		}

		public Vector2d this[int key]
		{
			get
			{
				if (key != 0)
				{
					return this.V1;
				}
				return this.V0;
			}
			set
			{
				if (key == 0)
				{
					this.V0 = value;
					return;
				}
				this.V1 = value;
			}
		}

		public Vector2d V0;

		public Vector2d V1;
	}
}
