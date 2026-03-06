using System;

namespace UnityEngine.ProBuilder.Csg
{
	internal static class CSG
	{
		public static float epsilon
		{
			get
			{
				return CSG.s_Epsilon;
			}
			set
			{
				CSG.s_Epsilon = value;
			}
		}

		public static Model Perform(CSG.BooleanOp op, GameObject lhs, GameObject rhs)
		{
			switch (op)
			{
			case CSG.BooleanOp.Intersection:
				return CSG.Intersect(lhs, rhs);
			case CSG.BooleanOp.Union:
				return CSG.Union(lhs, rhs);
			case CSG.BooleanOp.Subtraction:
				return CSG.Subtract(lhs, rhs);
			default:
				return null;
			}
		}

		public static Model Union(GameObject lhs, GameObject rhs)
		{
			Model model = new Model(lhs);
			Model model2 = new Model(rhs);
			Node a = new Node(model.ToPolygons());
			Node b = new Node(model2.ToPolygons());
			return new Model(Node.Union(a, b).AllPolygons());
		}

		public static Model Subtract(GameObject lhs, GameObject rhs)
		{
			Model model = new Model(lhs);
			Model model2 = new Model(rhs);
			Node a = new Node(model.ToPolygons());
			Node b = new Node(model2.ToPolygons());
			return new Model(Node.Subtract(a, b).AllPolygons());
		}

		public static Model Intersect(GameObject lhs, GameObject rhs)
		{
			Model model = new Model(lhs);
			Model model2 = new Model(rhs);
			Node a = new Node(model.ToPolygons());
			Node b = new Node(model2.ToPolygons());
			return new Model(Node.Intersect(a, b).AllPolygons());
		}

		private const float k_DefaultEpsilon = 1E-05f;

		private static float s_Epsilon = 1E-05f;

		public enum BooleanOp
		{
			Intersection,
			Union,
			Subtraction
		}
	}
}
