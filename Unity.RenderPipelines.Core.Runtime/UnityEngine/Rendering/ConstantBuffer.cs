using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public class ConstantBuffer
	{
		public static void PushGlobal<CBType>(CommandBuffer cmd, in CBType data, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType> instance = ConstantBufferSingleton<CBType>.instance;
			instance.UpdateData(cmd, data);
			instance.SetGlobal(cmd, shaderId);
		}

		public static void PushGlobal<CBType>(in CBType data, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType> instance = ConstantBufferSingleton<CBType>.instance;
			instance.UpdateData(data);
			instance.SetGlobal(shaderId);
		}

		public static void Push<CBType>(CommandBuffer cmd, in CBType data, ComputeShader cs, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType> instance = ConstantBufferSingleton<CBType>.instance;
			instance.UpdateData(cmd, data);
			instance.Set(cmd, cs, shaderId);
		}

		public static void Push<CBType>(in CBType data, ComputeShader cs, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType> instance = ConstantBufferSingleton<CBType>.instance;
			instance.UpdateData(data);
			instance.Set(cs, shaderId);
		}

		public static void Push<CBType>(CommandBuffer cmd, in CBType data, Material mat, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType> instance = ConstantBufferSingleton<CBType>.instance;
			instance.UpdateData(cmd, data);
			instance.Set(mat, shaderId);
		}

		public static void Push<CBType>(in CBType data, Material mat, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType> instance = ConstantBufferSingleton<CBType>.instance;
			instance.UpdateData(data);
			instance.Set(mat, shaderId);
		}

		public static void UpdateData<CBType>(CommandBuffer cmd, in CBType data) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.UpdateData(cmd, data);
		}

		public static void UpdateData<CBType>(in CBType data) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.UpdateData(data);
		}

		public static void SetGlobal<CBType>(CommandBuffer cmd, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.SetGlobal(cmd, shaderId);
		}

		public static void SetGlobal<CBType>(int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.SetGlobal(shaderId);
		}

		public static void Set<CBType>(CommandBuffer cmd, ComputeShader cs, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.Set(cmd, cs, shaderId);
		}

		public static void Set<CBType>(ComputeShader cs, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.Set(cs, shaderId);
		}

		public static void Set<CBType>(Material mat, int shaderId) where CBType : struct
		{
			ConstantBufferSingleton<CBType>.instance.Set(mat, shaderId);
		}

		public static void ReleaseAll()
		{
			foreach (ConstantBufferBase constantBufferBase in ConstantBuffer.m_RegisteredConstantBuffers)
			{
				constantBufferBase.Release();
			}
			ConstantBuffer.m_RegisteredConstantBuffers.Clear();
		}

		internal static void Register(ConstantBufferBase cb)
		{
			ConstantBuffer.m_RegisteredConstantBuffers.Add(cb);
		}

		private static List<ConstantBufferBase> m_RegisteredConstantBuffers = new List<ConstantBufferBase>();
	}
}
