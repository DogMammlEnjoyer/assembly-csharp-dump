using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class SurfaceUtils
	{
		public static float ComputeDistanceAbove(ISurfacePatch surfacePatch, Vector3 point, float radius)
		{
			SurfaceHit surfaceHit;
			surfacePatch.BackingSurface.ClosestSurfacePoint(point, out surfaceHit, 0f);
			return Vector3.Dot(point - surfaceHit.Point, surfaceHit.Normal) - radius;
		}

		public static float ComputeTangentDistance(ISurfacePatch surfacePatch, Vector3 point, float radius)
		{
			SurfaceHit surfaceHit;
			surfacePatch.ClosestSurfacePoint(point, out surfaceHit, 0f);
			SurfaceHit surfaceHit2;
			surfacePatch.BackingSurface.ClosestSurfacePoint(point, out surfaceHit2, 0f);
			Vector3 vector = point - surfaceHit.Point;
			Vector3 b = Vector3.Dot(vector, surfaceHit2.Normal) * surfaceHit2.Normal;
			return (vector - b).magnitude - radius;
		}

		public static float ComputeDepth(ISurfacePatch surfacePatch, Vector3 point, float radius)
		{
			return Mathf.Max(0f, -SurfaceUtils.ComputeDistanceAbove(surfacePatch, point, radius));
		}

		public static float ComputeDistanceFrom(ISurfacePatch surfacePatch, Vector3 point, float radius)
		{
			SurfaceHit surfaceHit;
			surfacePatch.ClosestSurfacePoint(point, out surfaceHit, 0f);
			return (point - surfaceHit.Point).magnitude - radius;
		}
	}
}
