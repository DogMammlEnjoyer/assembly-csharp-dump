using System;

namespace UnityEngine.Rendering.Universal
{
	internal struct DecalSubDrawCall
	{
		public int count
		{
			get
			{
				return this.end - this.start;
			}
		}

		public int start;

		public int end;
	}
}
