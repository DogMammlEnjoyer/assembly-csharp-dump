using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.LowLevelPhysics
{
	public struct GeometryHolder
	{
		private void SetGeometry<T>(T geometry) where T : struct, IGeometry
		{
			this.m_Type = (int)geometry.GeometryType;
			UnsafeUtility.CopyStructureToPtr<T>(ref geometry, UnsafeUtility.AddressOf<uint>(ref this.m_DataStart));
		}

		public T As<T>() where T : struct, IGeometry
		{
			T result = default(T);
			bool flag = result.GeometryType != (GeometryType)this.m_Type;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Unable to get geometry of type {0} from a geometry holder that stores {1}.", result.GeometryType, this.m_Type));
			}
			UnsafeUtility.CopyPtrToStructure<T>(UnsafeUtility.AddressOf<uint>(ref this.m_DataStart), out result);
			return result;
		}

		public static GeometryHolder Create<T>(T geometry) where T : struct, IGeometry
		{
			GeometryHolder result = new GeometryHolder
			{
				m_DataStart = 0U,
				m_Type = -1,
				m_FakePointer0 = new IntPtr((long)((ulong)-559038737)),
				m_FakePointer1 = new IntPtr((long)((ulong)-559038737))
			};
			result.SetGeometry<T>(geometry);
			return result;
		}

		public GeometryType Type
		{
			get
			{
				return (GeometryType)this.m_Type;
			}
		}

		private int m_Type;

		private uint m_DataStart;

		private IntPtr m_FakePointer0;

		private IntPtr m_FakePointer1;

		[FixedBuffer(typeof(uint), 6)]
		private GeometryHolder.<m_Blob>e__FixedBuffer m_Blob;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <m_Blob>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
