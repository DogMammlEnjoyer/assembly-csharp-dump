using System;
using System.Text;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class AdvancingFront
	{
		public AdvancingFront(AdvancingFrontNode head, AdvancingFrontNode tail)
		{
			this.Head = head;
			this.Tail = tail;
			this.Search = head;
			this.AddNode(head);
			this.AddNode(tail);
		}

		public void AddNode(AdvancingFrontNode node)
		{
		}

		public void RemoveNode(AdvancingFrontNode node)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (AdvancingFrontNode advancingFrontNode = this.Head; advancingFrontNode != this.Tail; advancingFrontNode = advancingFrontNode.Next)
			{
				stringBuilder.Append(advancingFrontNode.Point.X).Append("->");
			}
			stringBuilder.Append(this.Tail.Point.X);
			return stringBuilder.ToString();
		}

		private AdvancingFrontNode FindSearchNode(double x)
		{
			return this.Search;
		}

		public AdvancingFrontNode LocateNode(TriangulationPoint point)
		{
			return this.LocateNode(point.X);
		}

		private AdvancingFrontNode LocateNode(double x)
		{
			AdvancingFrontNode advancingFrontNode = this.FindSearchNode(x);
			if (x < advancingFrontNode.Value)
			{
				while ((advancingFrontNode = advancingFrontNode.Prev) != null)
				{
					if (x >= advancingFrontNode.Value)
					{
						this.Search = advancingFrontNode;
						return advancingFrontNode;
					}
				}
			}
			else
			{
				while ((advancingFrontNode = advancingFrontNode.Next) != null)
				{
					if (x < advancingFrontNode.Value)
					{
						this.Search = advancingFrontNode.Prev;
						return advancingFrontNode.Prev;
					}
				}
			}
			return null;
		}

		public AdvancingFrontNode LocatePoint(TriangulationPoint point)
		{
			double x = point.X;
			AdvancingFrontNode advancingFrontNode = this.FindSearchNode(x);
			double x2 = advancingFrontNode.Point.X;
			if (x == x2)
			{
				if (point != advancingFrontNode.Point)
				{
					if (point == advancingFrontNode.Prev.Point)
					{
						advancingFrontNode = advancingFrontNode.Prev;
					}
					else
					{
						if (point != advancingFrontNode.Next.Point)
						{
							throw new Exception("Failed to find Node for given afront point");
						}
						advancingFrontNode = advancingFrontNode.Next;
					}
				}
			}
			else if (x < x2)
			{
				while ((advancingFrontNode = advancingFrontNode.Prev) != null)
				{
					if (point == advancingFrontNode.Point)
					{
						break;
					}
				}
			}
			else
			{
				while ((advancingFrontNode = advancingFrontNode.Next) != null && point != advancingFrontNode.Point)
				{
				}
			}
			this.Search = advancingFrontNode;
			return advancingFrontNode;
		}

		public AdvancingFrontNode Head;

		public AdvancingFrontNode Tail;

		protected AdvancingFrontNode Search;
	}
}
