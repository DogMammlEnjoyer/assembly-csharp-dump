using System;
using System.Collections.Generic;
using Technie.PhysicsCreator.Rigid;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class FaceAlignmentBoxFitter
	{
		public void Fit(Hull hull, Vector3[] meshVertices, int[] meshIndices)
		{
			if (meshIndices.Length < 3)
			{
				return;
			}
			List<Triangle> list = hull.FindSelectedTriangles(meshVertices, meshIndices);
			list.Sort(new TriangleAreaSorter());
			List<TriangleBucket> list2 = new List<TriangleBucket>();
			using (List<Triangle>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Triangle triangle = enumerator.Current;
					TriangleBucket triangleBucket = this.FindBestBucket(triangle, 30f, list2);
					if (triangleBucket != null)
					{
						triangleBucket.Add(triangle);
					}
					else
					{
						triangleBucket = new TriangleBucket(triangle);
						list2.Add(triangleBucket);
					}
				}
				goto IL_7D;
			}
			IL_76:
			this.MergeClosestBuckets(list2);
			IL_7D:
			if (list2.Count <= 3)
			{
				list2.Sort(new TriangleBucketSorter());
				Vector3[] selectedVertices = hull.GetSelectedVertices(meshVertices, meshIndices);
				RotatedBoxFitter.ApplyToHull(RotatedBoxFitter.FindTightestBox(this.CreateConstructionPlane(list2[0], (list2.Count > 1) ? list2[1] : null, (list2.Count > 2) ? list2[2] : null), selectedVertices), hull);
				return;
			}
			goto IL_76;
		}

		private TriangleBucket FindBestBucket(Triangle tri, float thresholdAngleDeg, List<TriangleBucket> buckets)
		{
			TriangleBucket result = null;
			float num = float.PositiveInfinity;
			foreach (TriangleBucket triangleBucket in buckets)
			{
				float num2 = Vector3.Angle(tri.normal, triangleBucket.GetAverageNormal());
				if (num2 < thresholdAngleDeg && num2 < num)
				{
					num = num2;
					result = triangleBucket;
				}
				else
				{
					float num3 = Vector3.Angle(tri.normal * -1f, triangleBucket.GetAverageNormal());
					if (num3 < thresholdAngleDeg && num3 < num)
					{
						tri.normal *= -1f;
						num = num3;
						result = triangleBucket;
					}
				}
			}
			return result;
		}

		private void MergeClosestBuckets(List<TriangleBucket> buckets)
		{
			TriangleBucket triangleBucket = null;
			TriangleBucket triangleBucket2 = null;
			float num = float.PositiveInfinity;
			for (int i = 0; i < buckets.Count; i++)
			{
				for (int j = i + 1; j < buckets.Count; j++)
				{
					TriangleBucket triangleBucket3 = buckets[i];
					TriangleBucket triangleBucket4 = buckets[j];
					float num2 = Vector3.Angle(triangleBucket3.GetAverageNormal(), triangleBucket4.GetAverageNormal());
					if (num2 < num)
					{
						num = num2;
						triangleBucket = triangleBucket3;
						triangleBucket2 = triangleBucket4;
					}
				}
			}
			if (triangleBucket != null && triangleBucket2 != null)
			{
				buckets.Remove(triangleBucket2);
				triangleBucket.Add(triangleBucket2);
			}
		}

		private ConstructionPlane CreateConstructionPlane(TriangleBucket primaryBucket, TriangleBucket secondaryBucket, TriangleBucket tertiaryBucket)
		{
			if (primaryBucket != null && secondaryBucket != null)
			{
				Vector3 averageNormal = primaryBucket.GetAverageNormal();
				Vector3 t = Vector3.Cross(averageNormal, secondaryBucket.GetAverageNormal());
				return new ConstructionPlane(primaryBucket.GetAverageCenter(), averageNormal, t);
			}
			if (primaryBucket != null)
			{
				Vector3 averageNormal2 = primaryBucket.GetAverageNormal();
				Vector3 averageCenter = primaryBucket.GetAverageCenter();
				Vector3 t2 = Vector3.Cross(averageNormal2, (Vector3.Dot(averageNormal2, Vector3.up) > 0.5f) ? Vector3.right : Vector3.up);
				return new ConstructionPlane(averageCenter, averageNormal2, t2);
			}
			return null;
		}
	}
}
