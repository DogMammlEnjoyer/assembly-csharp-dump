using System;

namespace UnityEngine.Rendering
{
	internal static class ProbeVolumePositioning
	{
		public static bool OBBIntersect(in ProbeReferenceVolume.Volume a, in ProbeReferenceVolume.Volume b)
		{
			ProbeReferenceVolume.Volume volume = a;
			Vector3 a2;
			Vector3 vector;
			volume.CalculateCenterAndSize(out a2, out vector);
			volume = b;
			Vector3 b2;
			Vector3 vector2;
			volume.CalculateCenterAndSize(out b2, out vector2);
			float num = vector.sqrMagnitude / 2f;
			float num2 = vector2.sqrMagnitude / 2f;
			if (Vector3.SqrMagnitude(a2 - b2) > num + num2)
			{
				return false;
			}
			Vector3[] axes = ProbeVolumePositioning.m_Axes;
			int num3 = 0;
			Vector3 vector3 = a.X;
			axes[num3] = vector3.normalized;
			Vector3[] axes2 = ProbeVolumePositioning.m_Axes;
			int num4 = 1;
			vector3 = a.Y;
			axes2[num4] = vector3.normalized;
			Vector3[] axes3 = ProbeVolumePositioning.m_Axes;
			int num5 = 2;
			vector3 = a.Z;
			axes3[num5] = vector3.normalized;
			Vector3[] axes4 = ProbeVolumePositioning.m_Axes;
			int num6 = 3;
			vector3 = b.X;
			axes4[num6] = vector3.normalized;
			Vector3[] axes5 = ProbeVolumePositioning.m_Axes;
			int num7 = 4;
			vector3 = b.Y;
			axes5[num7] = vector3.normalized;
			Vector3[] axes6 = ProbeVolumePositioning.m_Axes;
			int num8 = 5;
			vector3 = b.Z;
			axes6[num8] = vector3.normalized;
			for (int i = 0; i < 6; i++)
			{
				Vector2 vector4 = ProbeVolumePositioning.ProjectOBB(a, ProbeVolumePositioning.m_Axes[i]);
				Vector2 vector5 = ProbeVolumePositioning.ProjectOBB(b, ProbeVolumePositioning.m_Axes[i]);
				if (vector4.y < vector5.x || vector5.y < vector4.x)
				{
					return false;
				}
			}
			return true;
		}

		public static bool OBBContains(in ProbeReferenceVolume.Volume obb, Vector3 point)
		{
			Vector3 vector = obb.X;
			float sqrMagnitude = vector.sqrMagnitude;
			vector = obb.Y;
			float sqrMagnitude2 = vector.sqrMagnitude;
			vector = obb.Z;
			float sqrMagnitude3 = vector.sqrMagnitude;
			point -= obb.corner;
			point = new Vector3(Vector3.Dot(point, obb.X), Vector3.Dot(point, obb.Y), Vector3.Dot(point, obb.Z));
			return 0f < point.x && point.x < sqrMagnitude && 0f < point.y && point.y < sqrMagnitude2 && 0f < point.z && point.z < sqrMagnitude3;
		}

		public static bool OBBAABBIntersect(in ProbeReferenceVolume.Volume a, in Bounds b, in Bounds aAABB)
		{
			Bounds bounds = aAABB;
			if (!bounds.Intersects(b))
			{
				return false;
			}
			bounds = b;
			Vector3 min = bounds.min;
			bounds = b;
			Vector3 max = bounds.max;
			ProbeVolumePositioning.m_AABBCorners[0] = new Vector3(min.x, min.y, min.z);
			ProbeVolumePositioning.m_AABBCorners[1] = new Vector3(max.x, min.y, min.z);
			ProbeVolumePositioning.m_AABBCorners[2] = new Vector3(max.x, max.y, min.z);
			ProbeVolumePositioning.m_AABBCorners[3] = new Vector3(min.x, max.y, min.z);
			ProbeVolumePositioning.m_AABBCorners[4] = new Vector3(min.x, min.y, max.z);
			ProbeVolumePositioning.m_AABBCorners[5] = new Vector3(max.x, min.y, max.z);
			ProbeVolumePositioning.m_AABBCorners[6] = new Vector3(max.x, max.y, max.z);
			ProbeVolumePositioning.m_AABBCorners[7] = new Vector3(min.x, max.y, max.z);
			Vector3[] axes = ProbeVolumePositioning.m_Axes;
			int num = 0;
			Vector3 vector = a.X;
			axes[num] = vector.normalized;
			Vector3[] axes2 = ProbeVolumePositioning.m_Axes;
			int num2 = 1;
			vector = a.Y;
			axes2[num2] = vector.normalized;
			Vector3[] axes3 = ProbeVolumePositioning.m_Axes;
			int num3 = 2;
			vector = a.Z;
			axes3[num3] = vector.normalized;
			for (int i = 0; i < 3; i++)
			{
				Vector2 vector2 = ProbeVolumePositioning.ProjectOBB(a, ProbeVolumePositioning.m_Axes[i]);
				Vector2 vector3 = ProbeVolumePositioning.ProjectAABB(ProbeVolumePositioning.m_AABBCorners, ProbeVolumePositioning.m_Axes[i]);
				if (vector2.y < vector3.x || vector3.y < vector2.x)
				{
					return false;
				}
			}
			return true;
		}

		private static Vector2 ProjectOBB(in ProbeReferenceVolume.Volume a, Vector3 axis)
		{
			float num = Vector3.Dot(axis, a.corner);
			float num2 = num;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						Vector3 rhs = a.corner + a.X * (float)i + a.Y * (float)j + a.Z * (float)k;
						float num3 = Vector3.Dot(axis, rhs);
						if (num3 < num)
						{
							num = num3;
						}
						else if (num3 > num2)
						{
							num2 = num3;
						}
					}
				}
			}
			return new Vector2(num, num2);
		}

		private static Vector2 ProjectAABB(in Vector3[] corners, Vector3 axis)
		{
			float num = Vector3.Dot(axis, corners[0]);
			float num2 = num;
			for (int i = 1; i < 8; i++)
			{
				float num3 = Vector3.Dot(axis, corners[i]);
				if (num3 < num)
				{
					num = num3;
				}
				else if (num3 > num2)
				{
					num2 = num3;
				}
			}
			return new Vector2(num, num2);
		}

		internal static Vector3[] m_Axes = new Vector3[6];

		internal static Vector3[] m_AABBCorners = new Vector3[8];
	}
}
