using System;

namespace g3
{
	public struct Vector3dTuple2
	{
		public Vector3dTuple2(Vector3d v0, Vector3d v1)
		{
			this.V0 = v0;
			this.V1 = v1;
		}

		public Vector3d this[int key]
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

		public Vector3d V0;

		public Vector3d V1;
	}
}
