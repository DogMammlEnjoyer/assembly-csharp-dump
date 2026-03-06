using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
	public static class GizmoHelpers
	{
		public static void DrawWirePlaneOriented(Vector3 position, Quaternion rotation, float size)
		{
			float num = size / 2f;
			Vector3 point = new Vector3(num, 0f, -num);
			Vector3 point2 = new Vector3(num, 0f, num);
			Vector3 point3 = new Vector3(-num, 0f, -num);
			Vector3 point4 = new Vector3(-num, 0f, num);
			Gizmos.DrawLine(rotation * point + position, rotation * point2 + position);
			Gizmos.DrawLine(rotation * point2 + position, rotation * point4 + position);
			Gizmos.DrawLine(rotation * point4 + position, rotation * point3 + position);
			Gizmos.DrawLine(rotation * point3 + position, rotation * point + position);
		}

		public static void DrawWireCubeOriented(Vector3 position, Quaternion rotation, float size)
		{
			float num = size / 2f;
			Vector3 point = new Vector3(num, 0f, -num);
			Vector3 point2 = new Vector3(num, 0f, num);
			Vector3 point3 = new Vector3(-num, 0f, -num);
			Vector3 point4 = new Vector3(-num, 0f, num);
			Vector3 point5 = new Vector3(num, size, -num);
			Vector3 point6 = new Vector3(num, size, num);
			Vector3 point7 = new Vector3(-num, size, -num);
			Vector3 point8 = new Vector3(-num, size, num);
			Gizmos.DrawLine(rotation * point + position, rotation * point2 + position);
			Gizmos.DrawLine(rotation * point2 + position, rotation * point4 + position);
			Gizmos.DrawLine(rotation * point4 + position, rotation * point3 + position);
			Gizmos.DrawLine(rotation * point3 + position, rotation * point + position);
			Gizmos.DrawLine(rotation * point5 + position, rotation * point6 + position);
			Gizmos.DrawLine(rotation * point6 + position, rotation * point8 + position);
			Gizmos.DrawLine(rotation * point8 + position, rotation * point7 + position);
			Gizmos.DrawLine(rotation * point7 + position, rotation * point5 + position);
			Gizmos.DrawLine(rotation * point5 + position, rotation * point + position);
			Gizmos.DrawLine(rotation * point6 + position, rotation * point2 + position);
			Gizmos.DrawLine(rotation * point8 + position, rotation * point4 + position);
			Gizmos.DrawLine(rotation * point7 + position, rotation * point3 + position);
		}

		public static void DrawAxisArrows(Transform transform, float size)
		{
			Vector3 position = transform.position;
			Gizmos.color = GizmoHelpers.s_ZAxisColor;
			Gizmos.DrawRay(position, transform.forward * size);
			Gizmos.color = GizmoHelpers.s_YAxisColor;
			Gizmos.DrawRay(position, transform.up * size);
			Gizmos.color = GizmoHelpers.s_XAxisColor;
			Gizmos.DrawRay(position, transform.right * size);
		}

		internal static void DrawCapsule(Vector3 center, float height, float radius, Vector3 axis, Color color)
		{
		}

		private static readonly Color s_XAxisColor = new Color(0.85882354f, 0.24313726f, 0.11372549f, 0.93f);

		private static readonly Color s_YAxisColor = new Color(0.6039216f, 0.9529412f, 0.28235295f, 0.93f);

		private static readonly Color s_ZAxisColor = new Color(0.22745098f, 0.47843137f, 0.972549f, 0.93f);

		private static readonly Dictionary<Vector3, ValueTuple<Vector3, Vector3>> s_AxisMapping = new Dictionary<Vector3, ValueTuple<Vector3, Vector3>>
		{
			{
				Vector3.up,
				new ValueTuple<Vector3, Vector3>(Vector3.forward, Vector3.right)
			},
			{
				Vector3.forward,
				new ValueTuple<Vector3, Vector3>(Vector3.right, Vector3.up)
			},
			{
				Vector3.right,
				new ValueTuple<Vector3, Vector3>(Vector3.up, Vector3.forward)
			}
		};
	}
}
