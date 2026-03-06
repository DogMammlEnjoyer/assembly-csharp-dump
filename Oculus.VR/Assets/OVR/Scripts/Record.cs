using System;

namespace Assets.OVR.Scripts
{
	internal class Record
	{
		public Record(int order, string cat, string msg)
		{
			this.sortOrder = order;
			this.category = cat;
			this.message = msg;
		}

		public int sortOrder;

		public string category;

		public string message;
	}
}
