using System;

namespace UnityEngine.Rendering.Universal
{
	internal struct ShadowEdge
	{
		public ShadowEdge(int indexA, int indexB)
		{
			this.v0 = indexA;
			this.v1 = indexB;
		}

		public void Reverse()
		{
			int num = this.v0;
			this.v0 = this.v1;
			this.v1 = num;
		}

		public int v0;

		public int v1;
	}
}
