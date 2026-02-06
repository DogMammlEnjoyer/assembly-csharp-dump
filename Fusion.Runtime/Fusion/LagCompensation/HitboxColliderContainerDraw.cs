using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusion.LagCompensation
{
	public class HitboxColliderContainerDraw : IEnumerable<ColliderDrawInfo>, IEnumerable
	{
		public IEnumerator<ColliderDrawInfo> GetEnumerator()
		{
			HitboxColliderContainerDraw.<GetEnumerator>d__2 <GetEnumerator>d__ = new HitboxColliderContainerDraw.<GetEnumerator>d__2(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal HitboxBuffer.HitboxSnapshot _container;

		private ColliderDrawInfo _drawInfo = new ColliderDrawInfo();
	}
}
