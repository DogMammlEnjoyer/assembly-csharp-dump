using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	[ExecuteAlways]
	public class DebugGizmos : MonoBehaviour
	{
		protected static DebugGizmos Root
		{
			get
			{
				if (DebugGizmos._root == null)
				{
					GameObject gameObject = GameObject.Find("Polyline Gizmos");
					if (gameObject != null)
					{
						DebugGizmos component = gameObject.GetComponent<DebugGizmos>();
						if (component != null)
						{
							DebugGizmos._root = component;
						}
					}
				}
				if (DebugGizmos._root == null)
				{
					DebugGizmos._root = new GameObject("Polyline Gizmos").AddComponent<DebugGizmos>();
				}
				return DebugGizmos._root;
			}
		}

		protected virtual void OnEnable()
		{
			if (DebugGizmos._root == null)
			{
				return;
			}
			if (DebugGizmos._root != this)
			{
				Object.Destroy(this);
			}
		}

		private PolylineRenderer Renderer
		{
			get
			{
				if (this._polylineRenderer == null)
				{
					this._polylineRenderer = new PolylineRenderer(null, DebugGizmos._renderSinglePass);
				}
				return this._polylineRenderer;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._polylineRenderer != null)
			{
				this._polylineRenderer.Cleanup();
				this._polylineRenderer = null;
			}
			bool isPlaying = Application.isPlaying;
		}

		protected void ClearSegments()
		{
			this._index = 0;
		}

		protected void RenderSegments()
		{
			this.Renderer.SetLines(this._points, this._colors, this._index);
			this.Renderer.RenderLines();
		}

		protected virtual void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.RenderSegments();
			this.ClearSegments();
		}

		protected void AddSegment(Vector3 p0, Vector3 p1, float width, Color color0, Color color1)
		{
			if (!this._addedSegmentSinceLastUpdate)
			{
				this.ClearSegments();
				this._addedSegmentSinceLastUpdate = true;
			}
			while (this._index + 2 > this._points.Count)
			{
				this._points.Add(default(Vector4));
				this._colors.Add(default(Color));
			}
			this._points[this._index] = new Vector4(p0.x, p0.y, p0.z, width);
			this._points[this._index + 1] = new Vector4(p1.x, p1.y, p1.z, width);
			this._colors[this._index] = color0;
			this._colors[this._index + 1] = color1;
			this._index += 2;
		}

		public static bool RenderSinglePass
		{
			get
			{
				return DebugGizmos._renderSinglePass;
			}
			set
			{
				if (DebugGizmos._renderSinglePass == value)
				{
					return;
				}
				DebugGizmos._renderSinglePass = value;
				if (DebugGizmos.Root != null)
				{
					Object.Destroy(DebugGizmos.Root);
				}
			}
		}

		public static void DrawPoint(Vector3 p0, Transform t = null)
		{
			if (t != null)
			{
				p0 = t.TransformPoint(p0);
			}
			DebugGizmos.Root.AddSegment(p0, p0, DebugGizmos.LineWidth, DebugGizmos.Color, DebugGizmos.Color);
		}

		public static void DrawLine(Vector3 p0, Vector3 p1, Transform t = null)
		{
			if (t != null)
			{
				p0 = t.TransformPoint(p0);
				p1 = t.TransformPoint(p1);
			}
			DebugGizmos.Root.AddSegment(p0, p1, DebugGizmos.LineWidth, DebugGizmos.Color, DebugGizmos.Color);
		}

		public static void DrawQuad(Vector3 center, float width, float height, Transform t = null)
		{
			Vector3 vector = new Vector3(-width / 2f, height / 2f) + center;
			Vector3 vector2 = new Vector3(width / 2f, height / 2f) + center;
			Vector3 vector3 = new Vector3(width / 2f, -height / 2f) + center;
			Vector3 vector4 = new Vector3(-width / 2f, -height / 2f) + center;
			DebugGizmos.DrawLine(vector, vector2, t);
			DebugGizmos.DrawLine(vector2, vector3, t);
			DebugGizmos.DrawLine(vector3, vector4, t);
			DebugGizmos.DrawLine(vector4, vector, t);
		}

		public static void DrawCurvedQuad(Vector3 center, float width, float height, float radius, Transform t = null, int divisions = 20)
		{
			Vector3[] array = new Vector3[divisions + 1];
			float num = width / radius;
			float num2 = num / (float)divisions;
			for (int i = 0; i <= divisions; i++)
			{
				float f = (float)i * num2 - num / 2f;
				Vector3 a = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f) - 1f) * radius;
				array[i] = a + center;
			}
			Vector3 b = new Vector3(0f, height / 2f, 0f);
			for (int j = 0; j < divisions; j++)
			{
				Vector3 a2 = array[j];
				Vector3 a3 = array[j + 1];
				DebugGizmos.DrawLine(a2 + b, a3 + b, t);
				DebugGizmos.DrawLine(a2 - b, a3 - b, t);
			}
			DebugGizmos.DrawLine(array[0] + b, array[0] - b, t);
			DebugGizmos.DrawLine(array[divisions] + b, array[divisions] - b, t);
		}

		public static void DrawWireCube(Vector3 center, float size, Transform t = null)
		{
			for (int i = 0; i < DebugGizmos.CUBE_SEGMENTS.Count; i += 2)
			{
				Vector3 p = DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i]] * size + center;
				Vector3 p2 = DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i + 1]] * size + center;
				DebugGizmos.DrawLine(p, p2, t);
			}
		}

		public static void DrawAxis(Vector3 position, Quaternion rotation, float size = 1f)
		{
			Color color = DebugGizmos.Color;
			DebugGizmos.Color = Color.red;
			DebugGizmos.DrawLine(position, position + rotation * Vector3.right * size, null);
			DebugGizmos.Color = Color.green;
			DebugGizmos.DrawLine(position, position + rotation * Vector3.up * size, null);
			DebugGizmos.Color = Color.blue;
			DebugGizmos.DrawLine(position, position + rotation * Vector3.forward * size, null);
			DebugGizmos.Color = color;
		}

		public static void DrawAxis(Pose pose, float size = 1f)
		{
			DebugGizmos.DrawAxis(pose.position, pose.rotation, size);
		}

		public static void DrawAxis(Transform t, float size = 1f)
		{
			DebugGizmos.DrawAxis(t.GetPose(Space.World), size);
		}

		private List<Vector4> _points = new List<Vector4>();

		private List<Color> _colors = new List<Color>();

		private int _index;

		private bool _addedSegmentSinceLastUpdate;

		protected static DebugGizmos _root = null;

		private PolylineRenderer _polylineRenderer;

		private static bool _renderSinglePass = true;

		public static Color Color = Color.black;

		public static float LineWidth = 0.1f;

		private static readonly IReadOnlyList<Vector3> CUBE_POINTS = new List<Vector3>
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f)
		};

		private static readonly IReadOnlyList<int> CUBE_SEGMENTS = new List<int>
		{
			0,
			1,
			1,
			5,
			3,
			5,
			0,
			3,
			0,
			2,
			1,
			4,
			3,
			6,
			5,
			7,
			2,
			4,
			4,
			7,
			7,
			6,
			6,
			2
		};
	}
}
