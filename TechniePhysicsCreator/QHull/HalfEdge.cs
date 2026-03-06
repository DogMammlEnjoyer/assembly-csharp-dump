using System;

namespace Technie.PhysicsCreator.QHull
{
	public class HalfEdge
	{
		public HalfEdge(Vertex v, Face f)
		{
			this.vertex = v;
			this.face = f;
		}

		public HalfEdge()
		{
		}

		public void setNext(HalfEdge edge)
		{
			this.next = edge;
		}

		public HalfEdge getNext()
		{
			return this.next;
		}

		public void setPrev(HalfEdge edge)
		{
			this.prev = edge;
		}

		public HalfEdge getPrev()
		{
			return this.prev;
		}

		public Face getFace()
		{
			return this.face;
		}

		public HalfEdge getOpposite()
		{
			return this.opposite;
		}

		public void setOpposite(HalfEdge edge)
		{
			this.opposite = edge;
			edge.opposite = this;
		}

		public Vertex head()
		{
			return this.vertex;
		}

		public Vertex tail()
		{
			if (this.prev == null)
			{
				return null;
			}
			return this.prev.vertex;
		}

		public Face oppositeFace()
		{
			if (this.opposite == null)
			{
				return null;
			}
			return this.opposite.face;
		}

		public string getVertexString()
		{
			if (this.tail() != null)
			{
				return this.tail().index.ToString() + "-" + this.head().index.ToString();
			}
			return "?-" + this.head().index.ToString();
		}

		public double length()
		{
			if (this.tail() != null)
			{
				return this.head().pnt.distance(this.tail().pnt);
			}
			return -1.0;
		}

		public double lengthSquared()
		{
			if (this.tail() != null)
			{
				return this.head().pnt.distanceSquared(this.tail().pnt);
			}
			return -1.0;
		}

		public Vertex vertex;

		public Face face;

		public HalfEdge next;

		public HalfEdge prev;

		public HalfEdge opposite;
	}
}
