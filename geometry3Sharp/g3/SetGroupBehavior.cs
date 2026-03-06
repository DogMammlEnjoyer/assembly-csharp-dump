using System;

namespace g3
{
	public struct SetGroupBehavior
	{
		public SetGroupBehavior(SetGroupBehavior.Modes mode, int id = 0)
		{
			this.Mode = mode;
			this.SetGroupID = id;
		}

		public int GetGroupID(DMesh3 mesh)
		{
			if (this.Mode == SetGroupBehavior.Modes.Ignore)
			{
				return -1;
			}
			if (this.Mode == SetGroupBehavior.Modes.AutoGenerate)
			{
				return mesh.AllocateTriangleGroup();
			}
			return this.SetGroupID;
		}

		public static SetGroupBehavior Ignore
		{
			get
			{
				return new SetGroupBehavior(SetGroupBehavior.Modes.Ignore, 0);
			}
		}

		public static SetGroupBehavior AutoGenerate
		{
			get
			{
				return new SetGroupBehavior(SetGroupBehavior.Modes.AutoGenerate, 0);
			}
		}

		public static SetGroupBehavior SetTo(int groupID)
		{
			return new SetGroupBehavior(SetGroupBehavior.Modes.UseConstant, groupID);
		}

		private SetGroupBehavior.Modes Mode;

		private int SetGroupID;

		public enum Modes
		{
			Ignore,
			AutoGenerate,
			UseConstant
		}
	}
}
