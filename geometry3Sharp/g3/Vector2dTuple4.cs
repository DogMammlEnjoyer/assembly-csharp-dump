using System;

namespace g3
{
	public struct Vector2dTuple4
	{
		public Vector2dTuple4(Vector2d v0, Vector2d v1, Vector2d v2, Vector2d v3)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
			this.V3 = v3;
		}

		public Vector2d this[int key]
		{
			get
			{
				if (key <= 1)
				{
					if (key != 1)
					{
						return this.V0;
					}
					return this.V1;
				}
				else
				{
					if (key != 2)
					{
						return this.V3;
					}
					return this.V2;
				}
			}
			set
			{
				if (key > 1)
				{
					if (key == 2)
					{
						this.V2 = value;
						return;
					}
					this.V3 = value;
					return;
				}
				else
				{
					if (key == 1)
					{
						this.V0 = value;
						return;
					}
					this.V1 = value;
					return;
				}
			}
		}

		public Vector2d V0;

		public Vector2d V1;

		public Vector2d V2;

		public Vector2d V3;
	}
}
