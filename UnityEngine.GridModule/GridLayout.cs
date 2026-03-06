using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Modules/Grid/Public/Grid.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[RequireComponent(typeof(Transform))]
	public class GridLayout : Behaviour
	{
		public Vector3 cellSize
		{
			[FreeFunction("GridLayoutBindings::GetCellSize", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 result;
				GridLayout.get_cellSize_Injected(intPtr, out result);
				return result;
			}
		}

		public Vector3 cellGap
		{
			[FreeFunction("GridLayoutBindings::GetCellGap", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 result;
				GridLayout.get_cellGap_Injected(intPtr, out result);
				return result;
			}
		}

		public GridLayout.CellLayout cellLayout
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GridLayout.get_cellLayout_Injected(intPtr);
			}
		}

		public GridLayout.CellSwizzle cellSwizzle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GridLayout.get_cellSwizzle_Injected(intPtr);
			}
		}

		[FreeFunction("GridLayoutBindings::GetBoundsLocal", HasExplicitThis = true)]
		public Bounds GetBoundsLocal(Vector3Int cellPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Bounds result;
			GridLayout.GetBoundsLocal_Injected(intPtr, ref cellPosition, out result);
			return result;
		}

		public Bounds GetBoundsLocal(Vector3 origin, Vector3 size)
		{
			return this.GetBoundsLocalOriginSize(origin, size);
		}

		[FreeFunction("GridLayoutBindings::GetBoundsLocalOriginSize", HasExplicitThis = true)]
		private Bounds GetBoundsLocalOriginSize(Vector3 origin, Vector3 size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Bounds result;
			GridLayout.GetBoundsLocalOriginSize_Injected(intPtr, ref origin, ref size, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::CellToLocal", HasExplicitThis = true)]
		public Vector3 CellToLocal(Vector3Int cellPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.CellToLocal_Injected(intPtr, ref cellPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::LocalToCell", HasExplicitThis = true)]
		public Vector3Int LocalToCell(Vector3 localPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3Int result;
			GridLayout.LocalToCell_Injected(intPtr, ref localPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::CellToLocalInterpolated", HasExplicitThis = true)]
		public Vector3 CellToLocalInterpolated(Vector3 cellPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.CellToLocalInterpolated_Injected(intPtr, ref cellPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::LocalToCellInterpolated", HasExplicitThis = true)]
		public Vector3 LocalToCellInterpolated(Vector3 localPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.LocalToCellInterpolated_Injected(intPtr, ref localPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::CellToWorld", HasExplicitThis = true)]
		public Vector3 CellToWorld(Vector3Int cellPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.CellToWorld_Injected(intPtr, ref cellPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::WorldToCell", HasExplicitThis = true)]
		public Vector3Int WorldToCell(Vector3 worldPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3Int result;
			GridLayout.WorldToCell_Injected(intPtr, ref worldPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::LocalToWorld", HasExplicitThis = true)]
		public Vector3 LocalToWorld(Vector3 localPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.LocalToWorld_Injected(intPtr, ref localPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::WorldToLocal", HasExplicitThis = true)]
		public Vector3 WorldToLocal(Vector3 worldPosition)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.WorldToLocal_Injected(intPtr, ref worldPosition, out result);
			return result;
		}

		[FreeFunction("GridLayoutBindings::GetLayoutCellCenter", HasExplicitThis = true)]
		public Vector3 GetLayoutCellCenter()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GridLayout>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 result;
			GridLayout.GetLayoutCellCenter_Injected(intPtr, out result);
			return result;
		}

		[RequiredByNativeCode]
		private void DoNothing()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cellSize_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cellGap_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GridLayout.CellLayout get_cellLayout_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GridLayout.CellSwizzle get_cellSwizzle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBoundsLocal_Injected(IntPtr _unity_self, [In] ref Vector3Int cellPosition, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBoundsLocalOriginSize_Injected(IntPtr _unity_self, [In] ref Vector3 origin, [In] ref Vector3 size, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CellToLocal_Injected(IntPtr _unity_self, [In] ref Vector3Int cellPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LocalToCell_Injected(IntPtr _unity_self, [In] ref Vector3 localPosition, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CellToLocalInterpolated_Injected(IntPtr _unity_self, [In] ref Vector3 cellPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LocalToCellInterpolated_Injected(IntPtr _unity_self, [In] ref Vector3 localPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CellToWorld_Injected(IntPtr _unity_self, [In] ref Vector3Int cellPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WorldToCell_Injected(IntPtr _unity_self, [In] ref Vector3 worldPosition, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LocalToWorld_Injected(IntPtr _unity_self, [In] ref Vector3 localPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WorldToLocal_Injected(IntPtr _unity_self, [In] ref Vector3 worldPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLayoutCellCenter_Injected(IntPtr _unity_self, out Vector3 ret);

		public enum CellLayout
		{
			Rectangle,
			Hexagon,
			Isometric,
			IsometricZAsY
		}

		public enum CellSwizzle
		{
			XYZ,
			XZY,
			YXZ,
			YZX,
			ZXY,
			ZYX
		}
	}
}
