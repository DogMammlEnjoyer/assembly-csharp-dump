using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusion.LagCompensation
{
	public class BVHDraw : IEnumerable<BVHNodeDrawInfo>, IEnumerable
	{
		internal BVHDraw(HitboxBuffer buffer)
		{
			this._buffer = buffer;
			this._drawInfo = new BVHNodeDrawInfo(this._buffer);
		}

		public IEnumerator<BVHNodeDrawInfo> GetEnumerator()
		{
			BVHDraw.<GetEnumerator>d__4 <GetEnumerator>d__ = new BVHDraw.<GetEnumerator>d__4(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal HitboxBuffer _buffer;

		private BVHNodeDrawInfo _drawInfo;

		private Stack<BVHNode> _reusableStack = new Stack<BVHNode>();
	}
}
