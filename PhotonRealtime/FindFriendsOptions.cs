using System;

namespace Photon.Realtime
{
	public class FindFriendsOptions
	{
		internal int ToIntFlags()
		{
			int num = 0;
			if (this.CreatedOnGs)
			{
				num |= 1;
			}
			if (this.Visible)
			{
				num |= 2;
			}
			if (this.Open)
			{
				num |= 4;
			}
			return num;
		}

		public bool CreatedOnGs;

		public bool Visible = true;

		public bool Open = true;
	}
}
