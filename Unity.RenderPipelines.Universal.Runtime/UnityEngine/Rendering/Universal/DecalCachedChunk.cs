using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalCachedChunk : DecalChunk
	{
		public override void RemoveAtSwapBack(int entityIndex)
		{
			base.RemoveAtSwapBack<float4x4>(ref this.decalToWorlds, entityIndex, base.count);
			base.RemoveAtSwapBack<float4x4>(ref this.normalToWorlds, entityIndex, base.count);
			base.RemoveAtSwapBack<float4x4>(ref this.sizeOffsets, entityIndex, base.count);
			base.RemoveAtSwapBack<float2>(ref this.drawDistances, entityIndex, base.count);
			base.RemoveAtSwapBack<float2>(ref this.angleFades, entityIndex, base.count);
			base.RemoveAtSwapBack<float4>(ref this.uvScaleBias, entityIndex, base.count);
			base.RemoveAtSwapBack<int>(ref this.layerMasks, entityIndex, base.count);
			base.RemoveAtSwapBack<ulong>(ref this.sceneLayerMasks, entityIndex, base.count);
			base.RemoveAtSwapBack<float>(ref this.fadeFactors, entityIndex, base.count);
			base.RemoveAtSwapBack<BoundingSphere>(ref this.boundingSphereArray, entityIndex, base.count);
			base.RemoveAtSwapBack<BoundingSphere>(ref this.boundingSpheres, entityIndex, base.count);
			base.RemoveAtSwapBack<DecalScaleMode>(ref this.scaleModes, entityIndex, base.count);
			base.RemoveAtSwapBack<uint>(ref this.renderingLayerMasks, entityIndex, base.count);
			base.RemoveAtSwapBack<float3>(ref this.positions, entityIndex, base.count);
			base.RemoveAtSwapBack<quaternion>(ref this.rotation, entityIndex, base.count);
			base.RemoveAtSwapBack<float3>(ref this.scales, entityIndex, base.count);
			base.RemoveAtSwapBack<bool>(ref this.dirty, entityIndex, base.count);
			int count = base.count;
			base.count = count - 1;
		}

		public override void SetCapacity(int newCapacity)
		{
			ref this.decalToWorlds.ResizeArray(newCapacity);
			ref this.normalToWorlds.ResizeArray(newCapacity);
			ref this.sizeOffsets.ResizeArray(newCapacity);
			ref this.drawDistances.ResizeArray(newCapacity);
			ref this.angleFades.ResizeArray(newCapacity);
			ref this.uvScaleBias.ResizeArray(newCapacity);
			ref this.layerMasks.ResizeArray(newCapacity);
			ref this.sceneLayerMasks.ResizeArray(newCapacity);
			ref this.fadeFactors.ResizeArray(newCapacity);
			ref this.boundingSpheres.ResizeArray(newCapacity);
			ref this.scaleModes.ResizeArray(newCapacity);
			ref this.renderingLayerMasks.ResizeArray(newCapacity);
			ref this.positions.ResizeArray(newCapacity);
			ref this.rotation.ResizeArray(newCapacity);
			ref this.scales.ResizeArray(newCapacity);
			ref this.dirty.ResizeArray(newCapacity);
			ArrayExtensions.ResizeArray<BoundingSphere>(ref this.boundingSphereArray, newCapacity);
			base.capacity = newCapacity;
		}

		public override void Dispose()
		{
			if (base.capacity == 0)
			{
				return;
			}
			this.decalToWorlds.Dispose();
			this.normalToWorlds.Dispose();
			this.sizeOffsets.Dispose();
			this.drawDistances.Dispose();
			this.angleFades.Dispose();
			this.uvScaleBias.Dispose();
			this.layerMasks.Dispose();
			this.sceneLayerMasks.Dispose();
			this.fadeFactors.Dispose();
			this.boundingSpheres.Dispose();
			this.scaleModes.Dispose();
			this.renderingLayerMasks.Dispose();
			this.positions.Dispose();
			this.rotation.Dispose();
			this.scales.Dispose();
			this.dirty.Dispose();
			base.count = 0;
			base.capacity = 0;
		}

		public MaterialPropertyBlock propertyBlock;

		public int passIndexDBuffer;

		public int passIndexEmissive;

		public int passIndexScreenSpace;

		public int passIndexGBuffer;

		public int drawOrder;

		public bool isCreated;

		public NativeArray<float4x4> decalToWorlds;

		public NativeArray<float4x4> normalToWorlds;

		public NativeArray<float4x4> sizeOffsets;

		public NativeArray<float2> drawDistances;

		public NativeArray<float2> angleFades;

		public NativeArray<float4> uvScaleBias;

		public NativeArray<int> layerMasks;

		public NativeArray<ulong> sceneLayerMasks;

		public NativeArray<float> fadeFactors;

		public NativeArray<BoundingSphere> boundingSpheres;

		public NativeArray<DecalScaleMode> scaleModes;

		public NativeArray<uint> renderingLayerMasks;

		public NativeArray<float3> positions;

		public NativeArray<quaternion> rotation;

		public NativeArray<float3> scales;

		public NativeArray<bool> dirty;

		public BoundingSphere[] boundingSphereArray;
	}
}
