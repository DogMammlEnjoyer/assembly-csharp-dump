using System;

namespace UnityEngine.Rendering
{
	internal struct DrawKey : IEquatable<DrawKey>
	{
		public bool Equals(DrawKey other)
		{
			return this.meshID == other.meshID && this.submeshIndex == other.submeshIndex && this.activeMeshLod == other.activeMeshLod && this.materialID == other.materialID && this.flags == other.flags && this.transparentInstanceId == other.transparentInstanceId && this.overridenComponents == other.overridenComponents && this.range.Equals(other.range) && this.lightmapIndex == other.lightmapIndex;
		}

		public override int GetHashCode()
		{
			return (int)((((((((((BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.HasSortingPosition) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + (int)this.meshID.value) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + this.submeshIndex) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + this.activeMeshLod) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + (int)this.materialID.value) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + (int)this.flags) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + this.transparentInstanceId) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + this.range.GetHashCode()) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + (int)this.overridenComponents) * (BatchDrawCommandFlags.FlipWinding | BatchDrawCommandFlags.HasMotion | BatchDrawCommandFlags.IsLightMapped | BatchDrawCommandFlags.LODCrossFadeKeyword) + this.lightmapIndex);
		}

		public BatchMeshID meshID;

		public int submeshIndex;

		public int activeMeshLod;

		public BatchMaterialID materialID;

		public BatchDrawCommandFlags flags;

		public int transparentInstanceId;

		public uint overridenComponents;

		public RangeKey range;

		public int lightmapIndex;
	}
}
