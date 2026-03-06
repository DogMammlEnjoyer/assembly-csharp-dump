using System;

namespace g3
{
	public struct Vector3fTuple3
	{
		public Vector3fTuple3(Vector3f v0, Vector3f v1, Vector3f v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		public Vector3f this[int key]
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

		public Vector3f V0;

		public Vector3f V1;

		public Vector3f V2;
	}
}
