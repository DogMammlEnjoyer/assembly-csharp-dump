using System;

namespace g3
{
	public interface IParametricCurve2d
	{
		bool IsClosed { get; }

		double ParamLength { get; }

		Vector2d SampleT(double t);

		Vector2d TangentT(double t);

		bool HasArcLength { get; }

		double ArcLength { get; }

		Vector2d SampleArcLength(double a);

		void Reverse();

		bool IsTransformable { get; }

		void Transform(ITransform2 xform);

		IParametricCurve2d Clone();
	}
}
