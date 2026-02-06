using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusion.LagCompensation
{
	public class SnapshotHistoryDraw : IEnumerable<HitboxColliderContainerDraw>, IEnumerable
	{
		internal SnapshotHistoryDraw(HitboxBuffer buffer)
		{
			this._buffer = buffer;
		}

		public IEnumerator<HitboxColliderContainerDraw> GetEnumerator()
		{
			SnapshotHistoryDraw.<GetEnumerator>d__3 <GetEnumerator>d__ = new SnapshotHistoryDraw.<GetEnumerator>d__3(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private HitboxBuffer _buffer;

		private HitboxColliderContainerDraw _containerDraw = new HitboxColliderContainerDraw();
	}
}
