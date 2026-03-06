using System;

namespace UnityEngine.Rendering
{
	internal class ConstantBufferSingleton<CBType> : ConstantBuffer<CBType> where CBType : struct
	{
		internal static ConstantBufferSingleton<CBType> instance
		{
			get
			{
				if (ConstantBufferSingleton<CBType>.s_Instance == null)
				{
					ConstantBufferSingleton<CBType>.s_Instance = new ConstantBufferSingleton<CBType>();
					ConstantBuffer.Register(ConstantBufferSingleton<CBType>.s_Instance);
				}
				return ConstantBufferSingleton<CBType>.s_Instance;
			}
			set
			{
				ConstantBufferSingleton<CBType>.s_Instance = value;
			}
		}

		public override void Release()
		{
			base.Release();
			ConstantBufferSingleton<CBType>.s_Instance = null;
		}

		private static ConstantBufferSingleton<CBType> s_Instance;
	}
}
