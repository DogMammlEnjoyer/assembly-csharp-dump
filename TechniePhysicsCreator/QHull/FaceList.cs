using System;

namespace Technie.PhysicsCreator.QHull
{
	public class FaceList
	{
		public void clear()
		{
			this.head = (this.tail = null);
		}

		public void add(Face vtx)
		{
			if (this.head == null)
			{
				this.head = vtx;
			}
			else
			{
				this.tail.next = vtx;
			}
			vtx.next = null;
			this.tail = vtx;
		}

		public Face first()
		{
			return this.head;
		}

		public bool isEmpty()
		{
			return this.head == null;
		}

		private Face head;

		private Face tail;
	}
}
