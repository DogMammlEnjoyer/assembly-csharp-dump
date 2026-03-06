using System;

namespace UnityEngine.AI
{
	public struct NavMeshQueryFilter
	{
		internal float[] costs { readonly get; private set; }

		public int areaMask { readonly get; set; }

		public int agentTypeID { readonly get; set; }

		public float GetAreaCost(int areaIndex)
		{
			bool flag = this.costs == null;
			float result;
			if (flag)
			{
				bool flag2 = areaIndex < 0 || areaIndex >= 32;
				if (flag2)
				{
					string message = string.Format("The valid range is [0:{0}]", 31);
					throw new IndexOutOfRangeException(message);
				}
				result = 1f;
			}
			else
			{
				result = this.costs[areaIndex];
			}
			return result;
		}

		public void SetAreaCost(int areaIndex, float cost)
		{
			bool flag = this.costs == null;
			if (flag)
			{
				this.costs = new float[32];
				for (int i = 0; i < 32; i++)
				{
					this.costs[i] = 1f;
				}
			}
			this.costs[areaIndex] = cost;
		}

		private const int k_AreaCostElementCount = 32;
	}
}
