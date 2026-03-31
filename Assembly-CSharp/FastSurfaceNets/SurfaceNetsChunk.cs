using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

namespace FastSurfaceNets
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SurfaceNetsChunk : MonoBehaviour
	{
		private void Awake()
		{
			if (this.autoGenerate)
			{
				this.BuildChunk();
			}
		}

		private void OnDestroy()
		{
			if (this.sdf.IsCreated)
			{
				this.sdf.Dispose();
				this.sdf = default(NativeArray<byte>);
			}
		}

		public void BuildChunk()
		{
			SurfaceNetsChunk.<BuildChunk>d__13 <BuildChunk>d__;
			<BuildChunk>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<BuildChunk>d__.<>4__this = this;
			<BuildChunk>d__.<>1__state = -1;
			<BuildChunk>d__.<>t__builder.Start<SurfaceNetsChunk.<BuildChunk>d__13>(ref <BuildChunk>d__);
		}

		private void FillChunk()
		{
			if (this.parameters.generateShape)
			{
				int x = this.shape.x;
				int num = this.shape.x * this.shape.y;
				int3 @int = this.parameters.shapeMin - this.chunkPosition + this.min;
				int3 int2 = this.parameters.shapeMax - this.chunkPosition + this.min;
				for (int i = 0; i < this.shape.z; i++)
				{
					for (int j = 0; j < this.shape.y; j++)
					{
						int num2 = i * num + j * x;
						for (int k = 0; k < this.shape.x; k++)
						{
							float value;
							if (this.parameters.generateShape)
							{
								value = ((k >= @int.x && k <= int2.x && j >= @int.y && j <= int2.y && i >= @int.z && i <= int2.z) ? 1f : -1f);
							}
							else
							{
								float3 @float = (this.chunkPosition + new int3(k, j, i)).ToFloat3();
								value = noise.snoise(@float * this.parameters.noiseScale) - @float.y / this.parameters.heightScale;
							}
							this.sdf[num2 + k] = value.ToByte();
						}
					}
				}
				return;
			}
			new FillChunkJob
			{
				sdf = this.sdf,
				shape = this.shape,
				chunkPosition = this.chunkPosition,
				shapeMin = this.parameters.shapeMin,
				shapeMax = this.parameters.shapeMax,
				noiseScale = this.parameters.noiseScale,
				heightScale = this.parameters.heightScale,
				min = this.min,
				max = this.max,
				strideY = this.shape.x,
				strideZ = this.shape.x * this.shape.y
			}.Schedule(this.shape.x * this.shape.y * this.shape.z, 64, default(JobHandle)).Complete();
		}

		private void OnDrawGizmosSelected()
		{
			if (!this.mesh || this.mesh.vertexCount < 3)
			{
				return;
			}
			Gizmos.color = Color.green;
			int vertexCount = this.mesh.vertexCount;
			Vector3[] vertices = this.mesh.vertices;
			Vector3[] normals = this.mesh.normals;
			for (int i = 0; i < vertexCount; i++)
			{
				Gizmos.DrawLine(base.transform.position + vertices[i], base.transform.position + vertices[i] + normals[i] * 0.25f);
			}
		}

		public int3 Id;

		public GenerationParameters parameters;

		public const int ChunkSize = 32;

		public bool autoGenerate = true;

		private const int Pad = 1;

		private int3 chunkPosition;

		private NativeArray<byte> sdf;

		private int3 min;

		private int3 max;

		private int3 shape;

		private Mesh mesh;
	}
}
