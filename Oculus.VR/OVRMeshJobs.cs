using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public class OVRMeshJobs
{
	public struct TransformToUnitySpaceJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			this.Vertices[index] = this.MeshVerticesPosition[index].FromFlippedXVector3f();
			this.Normals[index] = this.MeshNormals[index].FromFlippedXVector3f();
			this.UV[index] = new Vector2
			{
				x = this.MeshUV[index].x,
				y = -this.MeshUV[index].y
			};
			OVRPlugin.Vector4f vector4f = this.MeshBoneWeights[index];
			OVRPlugin.Vector4s vector4s = this.MeshBoneIndices[index];
			this.BoneWeights[index] = new BoneWeight
			{
				boneIndex0 = (int)vector4s.x,
				weight0 = vector4f.x,
				boneIndex1 = (int)vector4s.y,
				weight1 = vector4f.y,
				boneIndex2 = (int)vector4s.z,
				weight2 = vector4f.z,
				boneIndex3 = (int)vector4s.w,
				weight3 = vector4f.w
			};
		}

		public NativeArray<Vector3> Vertices;

		public NativeArray<Vector3> Normals;

		public NativeArray<Vector2> UV;

		public NativeArray<BoneWeight> BoneWeights;

		public NativeArray<OVRPlugin.Vector3f> MeshVerticesPosition;

		public NativeArray<OVRPlugin.Vector3f> MeshNormals;

		public NativeArray<OVRPlugin.Vector2f> MeshUV;

		public NativeArray<OVRPlugin.Vector4f> MeshBoneWeights;

		public NativeArray<OVRPlugin.Vector4s> MeshBoneIndices;
	}

	public struct TransformTrianglesJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			this.Triangles[index] = (uint)this.MeshIndices[this.NumIndices - index - 1];
		}

		public NativeArray<uint> Triangles;

		[ReadOnly]
		public NativeArray<short> MeshIndices;

		public int NumIndices;
	}

	public struct NativeArrayHelper<T> : IDisposable where T : struct
	{
		public unsafe NativeArrayHelper(T[] ovrArray, int length)
		{
			this._handle = GCHandle.Alloc(ovrArray, GCHandleType.Pinned);
			IntPtr value = this._handle.AddrOfPinnedObject();
			this.UnityNativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)value, length, Allocator.None);
		}

		public void Dispose()
		{
			this._handle.Free();
		}

		public NativeArray<T> UnityNativeArray;

		private GCHandle _handle;
	}
}
