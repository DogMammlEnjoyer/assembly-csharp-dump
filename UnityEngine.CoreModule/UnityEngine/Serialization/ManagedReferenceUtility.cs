using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Serialization
{
	[NativeHeader("Runtime/Serialize/ManagedReferenceUtility.h")]
	public sealed class ManagedReferenceUtility
	{
		[NativeMethod("SetManagedReferenceIdForObject")]
		private static bool SetManagedReferenceIdForObjectInternal(Object obj, object scriptObj, long refId)
		{
			return ManagedReferenceUtility.SetManagedReferenceIdForObjectInternal_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), scriptObj, refId);
		}

		public static bool SetManagedReferenceIdForObject(Object obj, object scriptObj, long refId)
		{
			bool flag = scriptObj == null;
			bool result;
			if (flag)
			{
				result = (refId == -2L);
			}
			else
			{
				Type type = scriptObj.GetType();
				bool flag2 = type == typeof(Object) || type.IsSubclassOf(typeof(Object));
				if (flag2)
				{
					throw new InvalidOperationException("Cannot assign an object deriving from UnityEngine.Object to a managed reference. This is not supported.");
				}
				result = ManagedReferenceUtility.SetManagedReferenceIdForObjectInternal(obj, scriptObj, refId);
			}
			return result;
		}

		[NativeMethod("GetManagedReferenceIdForObject")]
		private static long GetManagedReferenceIdForObjectInternal(Object obj, object scriptObj)
		{
			return ManagedReferenceUtility.GetManagedReferenceIdForObjectInternal_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), scriptObj);
		}

		public static long GetManagedReferenceIdForObject(Object obj, object scriptObj)
		{
			return ManagedReferenceUtility.GetManagedReferenceIdForObjectInternal(obj, scriptObj);
		}

		[NativeMethod("GetManagedReference")]
		private static object GetManagedReferenceInternal(Object obj, long id)
		{
			return ManagedReferenceUtility.GetManagedReferenceInternal_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), id);
		}

		public static object GetManagedReference(Object obj, long id)
		{
			return ManagedReferenceUtility.GetManagedReferenceInternal(obj, id);
		}

		[NativeMethod("GetManagedReferenceIds")]
		private static long[] GetManagedReferenceIdsForObjectInternal(Object obj)
		{
			long[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ManagedReferenceUtility.GetManagedReferenceIdsForObjectInternal_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				long[] array;
				blittableArrayWrapper.Unmarshal<long>(ref array);
				result = array;
			}
			return result;
		}

		public static long[] GetManagedReferenceIds(Object obj)
		{
			return ManagedReferenceUtility.GetManagedReferenceIdsForObjectInternal(obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetManagedReferenceIdForObjectInternal_Injected(IntPtr obj, object scriptObj, long refId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetManagedReferenceIdForObjectInternal_Injected(IntPtr obj, object scriptObj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetManagedReferenceInternal_Injected(IntPtr obj, long id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetManagedReferenceIdsForObjectInternal_Injected(IntPtr obj, out BlittableArrayWrapper ret);

		public const long RefIdUnknown = -1L;

		public const long RefIdNull = -2L;
	}
}
