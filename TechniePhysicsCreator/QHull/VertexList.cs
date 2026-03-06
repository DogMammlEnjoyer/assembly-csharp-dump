using System;

namespace Technie.PhysicsCreator.QHull
{
	internal class VertexList
	{
		public void clear()
		{
			this.head = (this.tail = null);
		}

		public void add(Vertex vtx)
		{
			if (this.head == null)
			{
				this.head = vtx;
			}
			else
			{
				this.tail.next = vtx;
			}
			vtx.prev = this.tail;
			vtx.next = null;
			this.tail = vtx;
		}

		public void addAll(Vertex vtx)
		{
			if (this.head == null)
			{
				this.head = vtx;
			}
			else
			{
				this.tail.next = vtx;
			}
			vtx.prev = this.tail;
			while (vtx.next != null)
			{
				vtx = vtx.next;
			}
			this.tail = vtx;
		}

		public void delete(Vertex vtx)
		{
			if (vtx.prev == null)
			{
				this.head = vtx.next;
			}
			else
			{
				vtx.prev.next = vtx.next;
			}
			if (vtx.next == null)
			{
				this.tail = vtx.prev;
				return;
			}
			vtx.next.prev = vtx.prev;
		}

		public void delete(Vertex vtx1, Vertex vtx2)
		{
			if (vtx1.prev == null)
			{
				this.head = vtx2.next;
			}
			else
			{
				vtx1.prev.next = vtx2.next;
			}
			if (vtx2.next == null)
			{
				this.tail = vtx1.prev;
				return;
			}
			vtx2.next.prev = vtx1.prev;
		}

		public void insertBefore(Vertex vtx, Vertex next)
		{
			vtx.prev = next.prev;
			if (next.prev == null)
			{
				this.head = vtx;
			}
			else
			{
				next.prev.next = vtx;
			}
			vtx.next = next;
			next.prev = vtx;
		}

		public Vertex first()
		{
			return this.head;
		}

		public bool isEmpty()
		{
			return this.head == null;
		}

		private Vertex head;

		private Vertex tail;
	}
}
