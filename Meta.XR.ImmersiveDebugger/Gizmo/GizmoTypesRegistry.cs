using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Gizmo
{
	internal static class GizmoTypesRegistry
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			Dictionary<ValueTuple<DebugGizmoType, Type>, GizmoTypeInfo> gizmoTypeInfos = GizmoTypesRegistry.GizmoTypeInfos;
			if (gizmoTypeInfos == null)
			{
				return;
			}
			gizmoTypeInfos.Clear();
		}

		public static void RegisterGizmoType(DebugGizmoType gizmoType, Type dataSourceType, Action<object> renderDelegate)
		{
			GizmoTypesRegistry.GizmoTypeInfos.Add(new ValueTuple<DebugGizmoType, Type>(gizmoType, dataSourceType), new GizmoTypeInfo(renderDelegate));
		}

		public static void InitGizmos()
		{
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Axis, typeof(Pose), delegate(object dataSource)
			{
				if (dataSource is Pose)
				{
					Pose pose = (Pose)dataSource;
					DebugGizmos.DrawAxis(pose, 0.1f);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Axis, typeof(Transform), delegate(object dataSource)
			{
				Transform transform = dataSource as Transform;
				if (transform != null)
				{
					DebugGizmos.DrawAxis(transform, 0.1f);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Point, typeof(Vector3), delegate(object dataSource)
			{
				if (dataSource is Vector3)
				{
					Vector3 p = (Vector3)dataSource;
					DebugGizmos.DrawPoint(p, null);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Point, typeof(Transform), delegate(object dataSource)
			{
				Transform transform = dataSource as Transform;
				if (transform != null)
				{
					DebugGizmos.DrawPoint(transform.position, null);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Line, typeof(Tuple<Vector3, Vector3>), delegate(object dataSource)
			{
				Tuple<Vector3, Vector3> tuple = dataSource as Tuple<Vector3, Vector3>;
				if (tuple != null)
				{
					DebugGizmos.DrawLine(tuple.Item1, tuple.Item2, null);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Lines, typeof(Vector3[]), delegate(object dataSource)
			{
				Vector3[] array = dataSource as Vector3[];
				if (array != null)
				{
					for (int i = 1; i < array.Length; i++)
					{
						DebugGizmos.DrawLine(array[i - 1], array[i], null);
					}
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Plane, typeof(Tuple<Pose, float, float>), delegate(object dataSource)
			{
				Tuple<Pose, float, float> tuple = dataSource as Tuple<Pose, float, float>;
				if (tuple != null)
				{
					DebugGizmos.DrawPlane(tuple.Item1, tuple.Item2, tuple.Item3);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Cube, typeof(Tuple<Vector3, float>), delegate(object dataSource)
			{
				Tuple<Vector3, float> tuple = dataSource as Tuple<Vector3, float>;
				if (tuple != null)
				{
					DebugGizmos.DrawWireCube(tuple.Item1, tuple.Item2, null);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.TopCenterBox, typeof(Tuple<Pose, float, float, float>), delegate(object dataSource)
			{
				Tuple<Pose, float, float, float> tuple = dataSource as Tuple<Pose, float, float, float>;
				if (tuple != null)
				{
					DebugGizmos.DrawBox(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, true);
				}
			});
			GizmoTypesRegistry.RegisterGizmoType(DebugGizmoType.Box, typeof(Tuple<Pose, float, float, float>), delegate(object dataSource)
			{
				Tuple<Pose, float, float, float> tuple = dataSource as Tuple<Pose, float, float, float>;
				if (tuple != null)
				{
					DebugGizmos.DrawBox(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, false);
				}
			});
		}

		public static bool IsValidDataTypeForGizmoType(Type type, DebugGizmoType gizmoType)
		{
			GizmoTypeInfo gizmoTypeInfo;
			if (GizmoTypesRegistry.GizmoTypeInfos.TryGetValue(new ValueTuple<DebugGizmoType, Type>(gizmoType, type), out gizmoTypeInfo))
			{
				return true;
			}
			Debug.LogWarning(string.Format("{0} not found in GizmoTypeInfos, please registerGizmoType.", gizmoType));
			return false;
		}

		public static void RenderGizmo(DebugGizmoType type, object dataSource)
		{
			if (dataSource == null)
			{
				return;
			}
			GizmoTypeInfo gizmoTypeInfo;
			if (GizmoTypesRegistry.GizmoTypeInfos.TryGetValue(new ValueTuple<DebugGizmoType, Type>(type, dataSource.GetType()), out gizmoTypeInfo))
			{
				gizmoTypeInfo.RenderDelegate(dataSource);
			}
		}

		private static readonly Dictionary<ValueTuple<DebugGizmoType, Type>, GizmoTypeInfo> GizmoTypeInfos = new Dictionary<ValueTuple<DebugGizmoType, Type>, GizmoTypeInfo>();
	}
}
