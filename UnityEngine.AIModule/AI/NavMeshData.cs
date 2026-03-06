using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	[NativeHeader("Modules/AI/NavMesh/NavMesh.bindings.h")]
	public sealed class NavMeshData : Object
	{
		public NavMeshData()
		{
			NavMeshData.Internal_Create(this, 0);
		}

		public NavMeshData(int agentTypeID)
		{
			NavMeshData.Internal_Create(this, agentTypeID);
		}

		[StaticAccessor("NavMeshDataBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] NavMeshData mono, int agentTypeID);

		public Bounds sourceBounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds result;
				NavMeshData.get_sourceBounds_Injected(intPtr, out result);
				return result;
			}
		}

		public Vector3 position
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 result;
				NavMeshData.get_position_Injected(intPtr, out result);
				return result;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshData.set_position_Injected(intPtr, ref value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion result;
				NavMeshData.get_rotation_Injected(intPtr, out result);
				return result;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshData.set_rotation_Injected(intPtr, ref value);
			}
		}

		internal bool hasHeightMeshData
		{
			[NativeMethod("HasHeightMeshData")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshData.get_hasHeightMeshData_Injected(intPtr);
			}
		}

		internal NavMeshBuildSettings buildSettings
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshData>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshBuildSettings result;
				NavMeshData.get_buildSettings_Injected(intPtr, out result);
				return result;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sourceBounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_position_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasHeightMeshData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_buildSettings_Injected(IntPtr _unity_self, out NavMeshBuildSettings ret);
	}
}
