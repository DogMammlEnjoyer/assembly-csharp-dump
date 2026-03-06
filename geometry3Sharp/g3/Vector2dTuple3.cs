using System;

namespace g3
{
	public struct Vector2dTuple3
	{
		public Vector2dTuple3(Vector2d v0, Vector2d v1, Vector2d v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		public Vector2d this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.V0;
				}
				if (key != 1)
				{
					return this.V2;
				}
				return this.V1;
			}
			set
			{
				if (key == 0)
				{
					this.V0 = value;
					return;
				}
				if (key == 1)
				{
					this.V1 = value;
					return;
				}
				this.V2 = value;
			}
		}

		public Vector2d V0;

		public Vector2d V1;

		public Vector2d V2;
	}
}
