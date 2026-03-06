using System;

namespace g3
{
	public interface IParametricCurve3d
	{
		bool IsClosed { get; }

		double ParamLength { get; }

		Vector3d SampleT(double t);

		Vector3d TangentT(double t);

		bool HasArcLength { get; }

		double ArcLength { get; }

		Vector3d SampleArcLength(double a);

		void Reverse();

		IParametricCurve3d Clone();
	}
}
