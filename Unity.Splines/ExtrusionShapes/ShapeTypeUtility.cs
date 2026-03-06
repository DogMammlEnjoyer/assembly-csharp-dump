using System;

namespace UnityEngine.Splines.ExtrusionShapes
{
	internal static class ShapeTypeUtility
	{
		public static ShapeType GetShapeType(object obj)
		{
			ShapeType result;
			if (!(obj is Circle))
			{
				if (!(obj is Square))
				{
					if (!(obj is Road))
					{
						if (!(obj is SplineShape))
						{
							throw new ArgumentException("obj is not a recognized shape", "obj");
						}
						result = ShapeType.Spline;
					}
					else
					{
						result = ShapeType.Road;
					}
				}
				else
				{
					result = ShapeType.Square;
				}
			}
			else
			{
				result = ShapeType.Circle;
			}
			return result;
		}

		public static IExtrudeShape CreateShape(ShapeType type)
		{
			IExtrudeShape result;
			switch (type)
			{
			case ShapeType.Circle:
				result = new Circle();
				break;
			case ShapeType.Square:
				result = new Square();
				break;
			case ShapeType.Road:
				result = new Road();
				break;
			case ShapeType.Spline:
				result = new SplineShape();
				break;
			default:
				throw new ArgumentOutOfRangeException("type");
			}
			return result;
		}
	}
}
