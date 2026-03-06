using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/BaseClasses/BitField.h")]
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
	[NativeClass("BitField", "struct BitField;")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct LayerMask
	{
		public static implicit operator int(LayerMask mask)
		{
			return mask.m_Mask;
		}

		public static implicit operator LayerMask(int intVal)
		{
			LayerMask result;
			result.m_Mask = intVal;
			return result;
		}

		public int value
		{
			get
			{
				return this.m_Mask;
			}
			set
			{
				this.m_Mask = value;
			}
		}

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[NativeMethod("LayerToString")]
		public static string LayerToName(int layer)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				LayerMask.LayerToName_Injected(layer, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[NativeMethod("StringToLayer")]
		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		public unsafe static int NameToLayer(string layerName)
		{
			int result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = layerName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = LayerMask.NameToLayer_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		public static int GetMask(params string[] layerNames)
		{
			bool flag = layerNames == null;
			if (flag)
			{
				throw new ArgumentNullException("layerNames");
			}
			int num = 0;
			foreach (string layerName in layerNames)
			{
				int num2 = LayerMask.NameToLayer(layerName);
				bool flag2 = num2 != -1;
				if (flag2)
				{
					num |= 1 << num2;
				}
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LayerToName_Injected(int layer, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int NameToLayer_Injected(ref ManagedSpanWrapper layerName);

		[NativeName("m_Bits")]
		private int m_Mask;
	}
}
