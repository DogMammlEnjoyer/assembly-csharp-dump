using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public class BufferedRTHandleSystem : IDisposable
	{
		public int maxWidth
		{
			get
			{
				return this.m_RTHandleSystem.GetMaxWidth();
			}
		}

		public int maxHeight
		{
			get
			{
				return this.m_RTHandleSystem.GetMaxHeight();
			}
		}

		public RTHandleProperties rtHandleProperties
		{
			get
			{
				return this.m_RTHandleSystem.rtHandleProperties;
			}
		}

		public RTHandle GetFrameRT(int bufferId, int frameIndex)
		{
			if (!this.m_RTHandles.ContainsKey(bufferId))
			{
				return null;
			}
			return this.m_RTHandles[bufferId][frameIndex];
		}

		public void ClearBuffers(CommandBuffer cmd)
		{
			foreach (KeyValuePair<int, RTHandle[]> keyValuePair in this.m_RTHandles)
			{
				for (int i = 0; i < keyValuePair.Value.Length; i++)
				{
					CoreUtils.SetRenderTarget(cmd, keyValuePair.Value[i], ClearFlag.Color, Color.black, 0, CubemapFace.Unknown, -1);
				}
			}
		}

		public void AllocBuffer(int bufferId, Func<RTHandleSystem, int, RTHandle> allocator, int bufferCount)
		{
			RTHandle[] array = new RTHandle[bufferCount];
			this.m_RTHandles.Add(bufferId, array);
			array[0] = allocator(this.m_RTHandleSystem, 0);
			int i = 1;
			int num = array.Length;
			while (i < num)
			{
				array[i] = allocator(this.m_RTHandleSystem, i);
				this.m_RTHandleSystem.SwitchResizeMode(array[i], RTHandleSystem.ResizeMode.OnDemand);
				i++;
			}
		}

		public void AllocBuffer(int bufferId, int bufferCount, ref RenderTextureDescriptor descriptor, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool isShadowMap = false, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			RTHandle[] array = new RTHandle[bufferCount];
			this.m_RTHandles.Add(bufferId, array);
			RTHandleAllocInfo rthandleAllocInfo = RTHandles.GetRTHandleAllocInfo(descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name);
			rthandleAllocInfo.isShadowMap = isShadowMap;
			array[0] = this.m_RTHandleSystem.Alloc(descriptor.width, descriptor.height, rthandleAllocInfo);
			int i = 1;
			int num = array.Length;
			while (i < num)
			{
				array[i] = this.m_RTHandleSystem.Alloc(descriptor.width, descriptor.height, rthandleAllocInfo);
				this.m_RTHandleSystem.SwitchResizeMode(array[i], RTHandleSystem.ResizeMode.OnDemand);
				i++;
			}
		}

		public void ReleaseBuffer(int bufferId)
		{
			RTHandle[] array;
			if (this.m_RTHandles.TryGetValue(bufferId, out array))
			{
				foreach (RTHandle rth in array)
				{
					this.m_RTHandleSystem.Release(rth);
				}
			}
			this.m_RTHandles.Remove(bufferId);
		}

		public void SwapAndSetReferenceSize(int width, int height)
		{
			this.Swap();
			this.m_RTHandleSystem.SetReferenceSize(width, height);
		}

		public void ResetReferenceSize(int width, int height)
		{
			this.m_RTHandleSystem.ResetReferenceSize(width, height);
		}

		public int GetNumFramesAllocated(int bufferId)
		{
			if (!this.m_RTHandles.ContainsKey(bufferId))
			{
				return 0;
			}
			return this.m_RTHandles[bufferId].Length;
		}

		public Vector2 CalculateRatioAgainstMaxSize(int width, int height)
		{
			RTHandleSystem rthandleSystem = this.m_RTHandleSystem;
			Vector2Int vector2Int = new Vector2Int(width, height);
			return rthandleSystem.CalculateRatioAgainstMaxSize(vector2Int);
		}

		private void Swap()
		{
			foreach (KeyValuePair<int, RTHandle[]> keyValuePair in this.m_RTHandles)
			{
				if (keyValuePair.Value.Length > 1)
				{
					RTHandle rthandle = keyValuePair.Value[keyValuePair.Value.Length - 1];
					int i = 0;
					int num = keyValuePair.Value.Length - 1;
					while (i < num)
					{
						keyValuePair.Value[i + 1] = keyValuePair.Value[i];
						i++;
					}
					keyValuePair.Value[0] = rthandle;
					this.m_RTHandleSystem.SwitchResizeMode(keyValuePair.Value[0], RTHandleSystem.ResizeMode.Auto);
					this.m_RTHandleSystem.SwitchResizeMode(keyValuePair.Value[1], RTHandleSystem.ResizeMode.OnDemand);
				}
				else
				{
					this.m_RTHandleSystem.SwitchResizeMode(keyValuePair.Value[0], RTHandleSystem.ResizeMode.Auto);
				}
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.m_DisposedValue)
			{
				if (disposing)
				{
					this.ReleaseAll();
					this.m_RTHandleSystem.Dispose();
					this.m_RTHandleSystem = null;
				}
				this.m_DisposedValue = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public void ReleaseAll()
		{
			foreach (KeyValuePair<int, RTHandle[]> keyValuePair in this.m_RTHandles)
			{
				int i = 0;
				int num = keyValuePair.Value.Length;
				while (i < num)
				{
					this.m_RTHandleSystem.Release(keyValuePair.Value[i]);
					i++;
				}
			}
			this.m_RTHandles.Clear();
		}

		private Dictionary<int, RTHandle[]> m_RTHandles = new Dictionary<int, RTHandle[]>();

		private RTHandleSystem m_RTHandleSystem = new RTHandleSystem();

		private bool m_DisposedValue;
	}
}
