using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class RTHandleResourcePool
	{
		internal int staleResourceCapacity
		{
			get
			{
				return RTHandleResourcePool.s_StaleResourceMaxCapacity;
			}
			set
			{
				if (RTHandleResourcePool.s_StaleResourceMaxCapacity != value)
				{
					RTHandleResourcePool.s_StaleResourceMaxCapacity = value;
					this.Cleanup();
				}
			}
		}

		internal bool AddResourceToPool(in TextureDesc texDesc, RTHandle resource, int currentFrameIndex)
		{
			if (RTHandleResourcePool.s_CurrentStaleResourceCount >= RTHandleResourcePool.s_StaleResourceMaxCapacity)
			{
				return false;
			}
			int hashCodeWithNameHash = this.GetHashCodeWithNameHash(texDesc);
			SortedList<int, ValueTuple<RTHandle, int>> sortedList;
			if (!this.m_ResourcePool.TryGetValue(hashCodeWithNameHash, out sortedList))
			{
				sortedList = new SortedList<int, ValueTuple<RTHandle, int>>(RTHandleResourcePool.s_StaleResourceMaxCapacity);
				this.m_ResourcePool.Add(hashCodeWithNameHash, sortedList);
			}
			sortedList.Add(resource.GetInstanceID(), new ValueTuple<RTHandle, int>(resource, currentFrameIndex));
			RTHandleResourcePool.s_CurrentStaleResourceCount++;
			return true;
		}

		internal bool TryGetResource(in TextureDesc texDesc, out RTHandle resource, bool usepool = true)
		{
			int hashCodeWithNameHash = this.GetHashCodeWithNameHash(texDesc);
			SortedList<int, ValueTuple<RTHandle, int>> sortedList;
			if (usepool && this.m_ResourcePool.TryGetValue(hashCodeWithNameHash, out sortedList) && sortedList.Count > 0)
			{
				resource = sortedList.Values[sortedList.Count - 1].Item1;
				sortedList.RemoveAt(sortedList.Count - 1);
				RTHandleResourcePool.s_CurrentStaleResourceCount--;
				return true;
			}
			resource = null;
			return false;
		}

		internal void Cleanup()
		{
			foreach (KeyValuePair<int, SortedList<int, ValueTuple<RTHandle, int>>> keyValuePair in this.m_ResourcePool)
			{
				foreach (KeyValuePair<int, ValueTuple<RTHandle, int>> keyValuePair2 in keyValuePair.Value)
				{
					keyValuePair2.Value.Item1.Release();
				}
			}
			this.m_ResourcePool.Clear();
			RTHandleResourcePool.s_CurrentStaleResourceCount = 0;
		}

		protected static bool ShouldReleaseResource(int lastUsedFrameIndex, int currentFrameIndex)
		{
			return lastUsedFrameIndex + RTHandleResourcePool.s_StaleResourceLifetime < currentFrameIndex;
		}

		internal void PurgeUnusedResources(int currentFrameIndex)
		{
			this.m_RemoveList.Clear();
			foreach (KeyValuePair<int, SortedList<int, ValueTuple<RTHandle, int>>> keyValuePair in this.m_ResourcePool)
			{
				SortedList<int, ValueTuple<RTHandle, int>> value = keyValuePair.Value;
				IList<int> keys = value.Keys;
				IList<ValueTuple<RTHandle, int>> values = value.Values;
				for (int i = 0; i < value.Count; i++)
				{
					ValueTuple<RTHandle, int> valueTuple = values[i];
					if (RTHandleResourcePool.ShouldReleaseResource(valueTuple.Item2, currentFrameIndex))
					{
						valueTuple.Item1.Release();
						this.m_RemoveList.Add(keys[i]);
						RTHandleResourcePool.s_CurrentStaleResourceCount--;
					}
				}
				foreach (int key in this.m_RemoveList)
				{
					value.Remove(key);
				}
			}
		}

		internal void LogDebugInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("RTHandleResourcePool for frame {0}, Total stale resources {1}", Time.frameCount, RTHandleResourcePool.s_CurrentStaleResourceCount);
			stringBuilder.AppendLine();
			foreach (KeyValuePair<int, SortedList<int, ValueTuple<RTHandle, int>>> keyValuePair in this.m_ResourcePool)
			{
				SortedList<int, ValueTuple<RTHandle, int>> value = keyValuePair.Value;
				IList<int> keys = value.Keys;
				IList<ValueTuple<RTHandle, int>> values = value.Values;
				for (int i = 0; i < value.Count; i++)
				{
					ValueTuple<RTHandle, int> valueTuple = values[i];
					stringBuilder.AppendFormat("Resrouce in pool: Name {0} Last active frame index {1} Size {2} x {3} x {4}", new object[]
					{
						valueTuple.Item1.name,
						valueTuple.Item2,
						valueTuple.Item1.rt.descriptor.width,
						valueTuple.Item1.rt.descriptor.height,
						valueTuple.Item1.rt.descriptor.volumeDepth
					});
					stringBuilder.AppendLine();
				}
			}
			Debug.Log(stringBuilder);
		}

		internal int GetHashCodeWithNameHash(in TextureDesc texDesc)
		{
			TextureDesc textureDesc = texDesc;
			return textureDesc.GetHashCode() * 23 + texDesc.name.GetHashCode();
		}

		internal static TextureDesc CreateTextureDesc(RenderTextureDescriptor desc, TextureSizeMode textureSizeMode = TextureSizeMode.Explicit, int anisoLevel = 1, float mipMapBias = 0f, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "")
		{
			GraphicsFormat format = (desc.depthStencilFormat != GraphicsFormat.None) ? desc.depthStencilFormat : desc.graphicsFormat;
			return new TextureDesc(desc.width, desc.height, false, false)
			{
				sizeMode = textureSizeMode,
				slices = desc.volumeDepth,
				format = format,
				filterMode = filterMode,
				wrapMode = wrapMode,
				dimension = desc.dimension,
				enableRandomWrite = desc.enableRandomWrite,
				useMipMap = desc.useMipMap,
				autoGenerateMips = desc.autoGenerateMips,
				isShadowMap = (desc.shadowSamplingMode != ShadowSamplingMode.None),
				anisoLevel = anisoLevel,
				mipMapBias = mipMapBias,
				msaaSamples = (MSAASamples)desc.msaaSamples,
				bindTextureMS = desc.bindMS,
				useDynamicScale = desc.useDynamicScale,
				memoryless = RenderTextureMemoryless.None,
				vrUsage = VRTextureUsage.None,
				name = name,
				enableShadingRate = desc.enableShadingRate
			};
		}

		[TupleElementNames(new string[]
		{
			"resource",
			"frameIndex"
		})]
		protected Dictionary<int, SortedList<int, ValueTuple<RTHandle, int>>> m_ResourcePool = new Dictionary<int, SortedList<int, ValueTuple<RTHandle, int>>>();

		protected List<int> m_RemoveList = new List<int>(32);

		protected static int s_CurrentStaleResourceCount = 0;

		protected static int s_StaleResourceLifetime = 3;

		protected static int s_StaleResourceMaxCapacity = 32;
	}
}
