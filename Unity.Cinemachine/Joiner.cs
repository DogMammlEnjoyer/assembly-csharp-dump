using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class Joiner
	{
		public Joiner(List<Joiner> joinerList, [Nullable(1)] OutPt op1, OutPt op2, Joiner nextH)
		{
			if (joinerList != null)
			{
				this.idx = joinerList.Count;
				joinerList.Add(this);
			}
			else
			{
				this.idx = -1;
			}
			this.nextH = nextH;
			this.op1 = op1;
			this.op2 = op2;
			this.next1 = op1.joiner;
			op1.joiner = this;
			if (op2 != null)
			{
				this.next2 = op2.joiner;
				op2.joiner = this;
				return;
			}
			this.next2 = null;
		}

		public int idx;

		[Nullable(1)]
		public OutPt op1;

		public OutPt op2;

		public Joiner next1;

		public Joiner next2;

		public Joiner nextH;
	}
}
