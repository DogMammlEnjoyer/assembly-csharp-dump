using System;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

namespace FastSurfaceNets
{
	public class SurfaceNetsWorld : MonoBehaviour
	{
		private void Awake()
		{
			this.Generate();
		}

		private void Generate()
		{
			this.DestroyChildren();
			for (int i = -this.radius.x; i <= this.radius.x; i++)
			{
				for (int j = -this.radius.y; j <= this.radius.y; j++)
				{
					for (int k = -this.radius.z; k <= this.radius.z; k++)
					{
						int3 @int = new int3(i, j, k);
						SurfaceNetsChunk surfaceNetsChunk = Object.Instantiate<SurfaceNetsChunk>(this.chunkPrefab, base.transform);
						surfaceNetsChunk.Id = @int;
						surfaceNetsChunk.parameters = this.parameters;
						surfaceNetsChunk.name = string.Format("SurfaceNetsChunk_{0}_{1}_{2}", @int.x, @int.y, @int.z);
						surfaceNetsChunk.transform.localPosition = @int.ToFloat3() * 32f;
						surfaceNetsChunk.BuildChunk();
					}
				}
			}
		}

		private void DestroyChildren()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = base.transform.GetChild(i);
				if (child != null)
				{
					JamUtil.Destroy(child.gameObject);
				}
			}
		}

		public SurfaceNetsChunk chunkPrefab;

		public int3 radius;

		public GenerationParameters parameters;
	}
}
