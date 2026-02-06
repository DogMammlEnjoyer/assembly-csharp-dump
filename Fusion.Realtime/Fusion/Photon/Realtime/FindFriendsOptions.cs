using System;

namespace Fusion.Photon.Realtime
{
	internal class FindFriendsOptions
	{
		internal int ToIntFlags()
		{
			int num = 0;
			bool createdOnGs = this.CreatedOnGs;
			if (createdOnGs)
			{
				num |= 1;
			}
			bool visible = this.Visible;
			if (visible)
			{
				num |= 2;
			}
			bool open = this.Open;
			if (open)
			{
				num |= 4;
			}
			return num;
		}

		public bool CreatedOnGs = false;

		public bool Visible = false;

		public bool Open = false;
	}
}
