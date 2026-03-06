using System;

namespace g3
{
	public interface ISpatial
	{
		bool SupportsNearestTriangle { get; }

		int FindNearestTriangle(Vector3d p, double fMaxDist = 1.7976931348623157E+308);

		bool SupportsTriangleRayIntersection { get; }

		int FindNearestHitTriangle(Ray3d ray, double fMaxDist = 1.7976931348623157E+308);

		bool SupportsPointContainment { get; }

		bool IsInside(Vector3d p);
	}
}
