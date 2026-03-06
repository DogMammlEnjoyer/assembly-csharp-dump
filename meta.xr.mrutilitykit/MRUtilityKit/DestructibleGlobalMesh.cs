using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	public struct DestructibleGlobalMesh
	{
		private bool Equals(DestructibleGlobalMesh other)
		{
			return this.DestructibleMeshComponent == other.DestructibleMeshComponent && object.Equals(this.MaxPointsCount, other.MaxPointsCount) && Mathf.Approximately(this.PointsPerUnitX, other.PointsPerUnitX) && Mathf.Approximately(this.PointsPerUnitY, other.PointsPerUnitY);
		}

		public override bool Equals(object obj)
		{
			if (obj is DestructibleGlobalMesh)
			{
				DestructibleGlobalMesh other = (DestructibleGlobalMesh)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<DestructibleMeshComponent, int, float, float>(this.DestructibleMeshComponent, this.MaxPointsCount, this.PointsPerUnitX, this.PointsPerUnitY);
		}

		public static bool operator ==(DestructibleGlobalMesh left, DestructibleGlobalMesh right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(DestructibleGlobalMesh left, DestructibleGlobalMesh right)
		{
			return !left.Equals(right);
		}

		public DestructibleMeshComponent DestructibleMeshComponent;

		public int MaxPointsCount;

		public float PointsPerUnitX;

		public float PointsPerUnitY;
	}
}
