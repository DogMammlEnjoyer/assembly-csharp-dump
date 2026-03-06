using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Gizmo
{
	[ExecuteAlways]
	public class DebugGizmos : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			DebugGizmos._root = null;
			DebugGizmos._renderSinglePass = true;
			DebugGizmos.Color = Color.black;
			DebugGizmos.LineWidth = 0.1f;
		}

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
					GameObject gameObject2 = new GameObject("Polyline Gizmos");
					DebugGizmos._root = gameObject2.AddComponent<DebugGizmos>();
					if (Application.isPlaying)
					{
						Object.DontDestroyOnLoad(gameObject2);
					}
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

		public static void DrawWireCube(Vector3 center, float size, Transform t = null)
		{
			for (int i = 0; i < DebugGizmos.CUBE_SEGMENTS.Count; i += 2)
			{
				Vector3 p = DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i]] * size + center;
				Vector3 p2 = DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i + 1]] * size + center;
				DebugGizmos.DrawLine(p, p2, t);
			}
		}

		private static void DrawAxis(Vector3 position, Quaternion rotation, float size = 0.1f)
		{
			using (new DebugGizmos.ColorScope(Color.black))
			{
				DebugGizmos.Color = Color.red;
				DebugGizmos.DrawLine(position, position + rotation * Vector3.right * size, null);
				DebugGizmos.Color = Color.green;
				DebugGizmos.DrawLine(position, position + rotation * Vector3.up * size, null);
				DebugGizmos.Color = Color.blue;
				DebugGizmos.DrawLine(position, position + rotation * Vector3.forward * size, null);
			}
		}

		public static void DrawAxis(Pose pose, float size = 0.1f)
		{
			DebugGizmos.DrawAxis(pose.position, pose.rotation, size);
		}

		public static void DrawAxis(Transform t, float size = 0.1f)
		{
			DebugGizmos.DrawAxis(new Pose(t.position, t.rotation), size);
		}

		private static void DrawPlane(Vector3 position, Quaternion rotation, float width, float height)
		{
			DebugGizmos.DrawAxis(position, rotation, 0.1f);
			Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
			for (int i = 0; i < DebugGizmos.PLANE_SEGMENTS.Count; i += 2)
			{
				Vector3 point = new Vector3(DebugGizmos.PLANE_POINTS[DebugGizmos.PLANE_SEGMENTS[i]].x * width, DebugGizmos.PLANE_POINTS[DebugGizmos.PLANE_SEGMENTS[i]].y * height, 0f);
				Vector3 point2 = new Vector3(DebugGizmos.PLANE_POINTS[DebugGizmos.PLANE_SEGMENTS[i + 1]].x * width, DebugGizmos.PLANE_POINTS[DebugGizmos.PLANE_SEGMENTS[i + 1]].y * height, 0f);
				DebugGizmos.DrawLine(matrix4x.MultiplyPoint3x4(point), matrix4x.MultiplyPoint3x4(point2), null);
			}
		}

		public static void DrawPlane(Pose pose, float width, float height)
		{
			DebugGizmos.DrawPlane(pose.position, pose.rotation, width, height);
		}

		private static void DrawBox(Vector3 position, Quaternion rotation, float width, float height, float depth, bool isPivotTopSurface)
		{
			DebugGizmos.DrawAxis(position, rotation, 0.1f);
			if (isPivotTopSurface)
			{
				Vector3 b = new Vector3(0f, depth / 2f, 0f);
				position -= b;
			}
			Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
			for (int i = 0; i < DebugGizmos.CUBE_SEGMENTS.Count; i += 2)
			{
				Vector3 point = new Vector3(DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i]].x * width, DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i]].y * height, DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i]].z * depth);
				Vector3 point2 = new Vector3(DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i + 1]].x * width, DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i + 1]].y * height, DebugGizmos.CUBE_POINTS[DebugGizmos.CUBE_SEGMENTS[i + 1]].z * depth);
				DebugGizmos.DrawLine(matrix4x.MultiplyPoint3x4(point), matrix4x.MultiplyPoint3x4(point2), null);
			}
		}

		public static void DrawBox(Pose pose, float width, float height, float depth, bool isPivotTopSurface = false)
		{
			DebugGizmos.DrawBox(pose.position, pose.rotation, width, height, depth, isPivotTopSurface);
		}

		private List<Vector4> _points = new List<Vector4>();

		private List<Color> _colors = new List<Color>();

		private int _index;

		private bool _addedSegmentSinceLastUpdate;

		protected static DebugGizmos _root;

		private PolylineRenderer _polylineRenderer;

		private static bool _renderSinglePass = true;

		public static Color Color = Color.black;

		public static float LineWidth = 0.1f;

		private static readonly IReadOnlyList<Vector2> PLANE_POINTS = new List<Vector2>
		{
			new Vector2(-0.5f, -0.5f),
			new Vector2(-0.5f, 0.5f),
			new Vector2(0.5f, -0.5f),
			new Vector2(0.5f, 0.5f)
		};

		private static readonly IReadOnlyList<int> PLANE_SEGMENTS = new List<int>
		{
			0,
			1,
			1,
			3,
			3,
			2,
			2,
			0
		};

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

		internal struct ColorScope : IDisposable
		{
			public ColorScope(Color color)
			{
				this._savedColor = DebugGizmos.Color;
				DebugGizmos.Color = color;
			}

			public void Dispose()
			{
				DebugGizmos.Color = this._savedColor;
			}

			private readonly Color _savedColor;
		}
	}
}
