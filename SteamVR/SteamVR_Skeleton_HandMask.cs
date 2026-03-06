using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Skeleton_HandMask
	{
		public void SetFinger(int i, bool value)
		{
			this.values[i] = value;
			this.Apply();
		}

		public bool GetFinger(int i)
		{
			return this.values[i];
		}

		public SteamVR_Skeleton_HandMask()
		{
			this.values = new bool[6];
			this.Reset();
		}

		public void Reset()
		{
			this.values = new bool[6];
			for (int i = 0; i < 6; i++)
			{
				this.values[i] = true;
			}
			this.Apply();
		}

		protected void Apply()
		{
			this.palm = this.values[0];
			this.thumb = this.values[1];
			this.index = this.values[2];
			this.middle = this.values[3];
			this.ring = this.values[4];
			this.pinky = this.values[5];
		}

		public bool palm;

		public bool thumb;

		public bool index;

		public bool middle;

		public bool ring;

		public bool pinky;

		public bool[] values = new bool[6];

		public static readonly SteamVR_Skeleton_HandMask fullMask = new SteamVR_Skeleton_HandMask();
	}
}
