using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Splines.ExtrusionShapes;

namespace UnityEngine.Splines
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("Splines/Spline Extrude")]
	[ExecuteAlways]
	public class SplineExtrude : MonoBehaviour
	{
		internal bool CanCapEnds
		{
			get
			{
				return this.m_CanCapEnds;
			}
		}

		internal IExtrudeShape Shape
		{
			get
			{
				return this.m_Shape;
			}
			set
			{
				this.m_Shape = value;
			}
		}

		[Obsolete("Use Container instead.", false)]
		public SplineContainer container
		{
			get
			{
				return this.Container;
			}
		}

		public SplineContainer Container
		{
			get
			{
				return this.m_Container;
			}
			set
			{
				this.m_Container = value;
			}
		}

		[Obsolete("Use RebuildOnSplineChange instead.", false)]
		public bool rebuildOnSplineChange
		{
			get
			{
				return this.RebuildOnSplineChange;
			}
		}

		public bool RebuildOnSplineChange
		{
			get
			{
				return this.m_RebuildOnSplineChange;
			}
			set
			{
				this.m_RebuildOnSplineChange = value;
				if (!value)
				{
					this.m_RebuildRequested = value;
				}
			}
		}

		[Obsolete("Use RebuildFrequency instead.", false)]
		public int rebuildFrequency
		{
			get
			{
				return this.RebuildFrequency;
			}
		}

		public int RebuildFrequency
		{
			get
			{
				return this.m_RebuildFrequency;
			}
			set
			{
				this.m_RebuildFrequency = Mathf.Max(value, 1);
			}
		}

		[Obsolete("Use Sides instead.", false)]
		public int sides
		{
			get
			{
				return this.Sides;
			}
		}

		public int Sides
		{
			get
			{
				return this.m_Sides;
			}
			set
			{
				this.m_Sides = Mathf.Max(value, 3);
				if (this.m_Shape == null)
				{
					this.m_Shape = new Circle
					{
						SideCount = this.m_Sides
					};
				}
			}
		}

		[Obsolete("Use SegmentsPerUnit instead.", false)]
		public float segmentsPerUnit
		{
			get
			{
				return this.SegmentsPerUnit;
			}
		}

		public float SegmentsPerUnit
		{
			get
			{
				return this.m_SegmentsPerUnit;
			}
			set
			{
				this.m_SegmentsPerUnit = Mathf.Max(value, 0.0001f);
			}
		}

		[Obsolete("Use Capped instead.", false)]
		public bool capped
		{
			get
			{
				return this.Capped;
			}
		}

		public bool Capped
		{
			get
			{
				return this.m_Capped;
			}
			set
			{
				this.m_Capped = value;
			}
		}

		[Obsolete("Use Radius instead.", false)]
		public float radius
		{
			get
			{
				return this.Radius;
			}
		}

		public float Radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = Mathf.Max(value, 1E-05f);
			}
		}

		[Obsolete("Use Range instead.", false)]
		public Vector2 range
		{
			get
			{
				return this.Range;
			}
		}

		public Vector2 Range
		{
			get
			{
				return this.m_Range;
			}
			set
			{
				this.m_Range = new Vector2(Mathf.Min(value.x, value.y), Mathf.Max(value.x, value.y));
			}
		}

		public bool FlipNormals
		{
			get
			{
				return this.m_FlipNormals;
			}
			set
			{
				this.m_FlipNormals = value;
			}
		}

		public Mesh targetMesh
		{
			get
			{
				return this.m_TargetMesh;
			}
			set
			{
				if (this.m_TargetMesh == value)
				{
					return;
				}
				this.CleanupMesh();
				this.m_TargetMesh = value;
				this.EnsureMeshExists();
				this.Rebuild();
			}
		}

		[Obsolete("Use Spline instead.", false)]
		public Spline spline
		{
			get
			{
				return this.Spline;
			}
		}

		public Spline Spline
		{
			get
			{
				SplineContainer container = this.m_Container;
				if (container == null)
				{
					return null;
				}
				return container.Spline;
			}
		}

		public IReadOnlyList<Spline> Splines
		{
			get
			{
				SplineContainer container = this.m_Container;
				if (container == null)
				{
					return null;
				}
				return container.Splines;
			}
		}

		internal void Reset()
		{
			base.TryGetComponent<SplineContainer>(out this.m_Container);
			MeshRenderer meshRenderer;
			if (base.TryGetComponent<MeshRenderer>(out meshRenderer) && meshRenderer.sharedMaterial == null)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Material sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
				Object.DestroyImmediate(gameObject);
				meshRenderer.sharedMaterial = sharedMaterial;
			}
			this.Rebuild();
		}

		private bool IsNullOrEmptyContainer()
		{
			return this.m_Container == null || this.m_Container.Spline == null || this.m_Container.Splines.Count == 0;
		}

		internal void SetSplineContainerOnGO()
		{
			SplineContainer component = base.gameObject.GetComponent<SplineContainer>();
			if (component != null && component != this.m_Container)
			{
				this.m_Container = component;
			}
		}

		private void OnEnable()
		{
			this.EnsureMeshExists();
			Spline.Changed += this.OnSplineChanged;
			if (!this.IsNullOrEmptyContainer())
			{
				this.Rebuild();
			}
		}

		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChanged;
			this.CleanupMesh();
		}

		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
		{
			if (!this.m_RebuildOnSplineChange)
			{
				return;
			}
			bool flag = this.m_Container != null && this.Splines.Contains(spline);
			SplineShape splineShape = this.m_Shape as SplineShape;
			bool flag2 = splineShape != null && splineShape.Spline != null && splineShape.Spline.Equals(spline);
			this.m_RebuildRequested |= (flag || flag2);
		}

		private void Update()
		{
			if (this.m_RebuildRequested && Time.time >= this.m_NextScheduledRebuild)
			{
				this.Rebuild();
			}
		}

		private void EnsureMeshExists()
		{
			if (this.m_Mesh == null)
			{
				if (this.targetMesh != null)
				{
					this.m_Mesh = this.targetMesh;
				}
				else
				{
					this.m_Mesh = new Mesh
					{
						name = "<Spline Extruded Mesh>"
					};
					this.m_Mesh.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			MeshFilter meshFilter;
			if (base.TryGetComponent<MeshFilter>(out meshFilter))
			{
				meshFilter.hideFlags = HideFlags.NotEditable;
				meshFilter.sharedMesh = this.m_Mesh;
			}
		}

		private void CleanupMesh()
		{
			MeshFilter meshFilter;
			if (base.TryGetComponent<MeshFilter>(out meshFilter))
			{
				meshFilter.hideFlags = HideFlags.None;
				meshFilter.sharedMesh = null;
			}
			if (this.m_Mesh != this.m_TargetMesh)
			{
				Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
		}

		public void Rebuild()
		{
			if (this.m_Shape == null)
			{
				this.m_Shape = new Circle
				{
					SideCount = this.m_Sides
				};
			}
			if (this.m_Mesh == null)
			{
				return;
			}
			if (this.IsNullOrEmptyContainer())
			{
				if (Application.isPlaying)
				{
					Debug.LogError(SplineExtrude.k_EmptyContainerError, this);
				}
				return;
			}
			this.m_Mesh.Clear();
			if (this.m_Range.x == this.m_Range.y)
			{
				return;
			}
			ExtrudeSettings<IExtrudeShape> settings = new ExtrudeSettings<IExtrudeShape>(this.m_Shape)
			{
				Radius = this.m_Radius,
				CapEnds = this.m_Capped,
				Range = this.m_Range,
				FlipNormals = this.m_FlipNormals
			};
			SplineMesh.Extrude<Spline, IExtrudeShape>(this.m_Container.Splines, this.m_Mesh, settings, this.SegmentsPerUnit);
			this.m_CanCapEnds = (SplineMesh.s_IsConvex && !this.Spline.Closed);
			this.AutosmoothNormals();
			this.m_NextScheduledRebuild = Time.time + 1f / (float)this.m_RebuildFrequency;
			if (this.m_UpdateColliders)
			{
				MeshCollider meshCollider;
				if (base.TryGetComponent<MeshCollider>(out meshCollider))
				{
					meshCollider.sharedMesh = this.m_Mesh;
				}
				BoxCollider boxCollider;
				if (base.TryGetComponent<BoxCollider>(out boxCollider))
				{
					boxCollider.center = this.m_Mesh.bounds.center;
					boxCollider.size = this.m_Mesh.bounds.size;
				}
				SphereCollider sphereCollider;
				if (base.TryGetComponent<SphereCollider>(out sphereCollider))
				{
					sphereCollider.center = this.m_Mesh.bounds.center;
					Vector3 extents = this.m_Mesh.bounds.extents;
					sphereCollider.radius = Mathf.Max(new float[]
					{
						extents.x,
						extents.y,
						extents.z
					});
				}
			}
			this.m_RebuildRequested = false;
		}

		private void AutosmoothNormals()
		{
			Vector3[] vertices = this.m_Mesh.vertices;
			int[] triangles = this.m_Mesh.triangles;
			Vector3[] array = new Vector3[vertices.Length];
			Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
			Dictionary<int, List<int>> dictionary2 = new Dictionary<int, List<int>>();
			for (int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 b = vertices[triangles[i]];
				Vector3 a = vertices[triangles[i + 1]];
				Vector3 a2 = vertices[triangles[i + 2]];
				Vector3 normalized = Vector3.Cross(a - b, a2 - b).normalized;
				int num = i / 3;
				dictionary[num] = normalized;
				for (int j = 0; j < 3; j++)
				{
					int key = triangles[i + j];
					if (!dictionary2.ContainsKey(key))
					{
						dictionary2[key] = new List<int>();
					}
					dictionary2[key].Add(num);
				}
			}
			foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary2)
			{
				int key2 = keyValuePair.Key;
				List<int> value = keyValuePair.Value;
				Vector3 a3 = Vector3.zero;
				foreach (int num2 in value)
				{
					Vector3 vector = dictionary[num2];
					bool flag = true;
					foreach (int num3 in value)
					{
						if (num2 != num3)
						{
							Vector3 to = dictionary[num3];
							if (Vector3.Angle(vector, to) > this.m_AutosmoothAngle)
							{
								flag = false;
								break;
							}
						}
					}
					if (!flag)
					{
						array[key2] = vector;
						break;
					}
					a3 += vector;
				}
				if (array[key2] == Vector3.zero)
				{
					array[key2] = a3.normalized;
				}
			}
			this.m_Mesh.normals = array;
		}

		internal Mesh CreateMeshAsset()
		{
			return new Mesh
			{
				name = base.name
			};
		}

		[SerializeField]
		[Tooltip("The Spline to extrude.")]
		private SplineContainer m_Container;

		[SerializeField]
		[Tooltip("The mesh that should be extruded. If none, a temporary mesh will be created.")]
		private Mesh m_TargetMesh;

		[SerializeField]
		[Tooltip("Enable to regenerate the extruded mesh when the target Spline is modified. Disable this option if the Spline will not be modified at runtime.")]
		private bool m_RebuildOnSplineChange = true;

		[SerializeField]
		[Tooltip("The maximum number of times per-second that the mesh will be rebuilt.")]
		private int m_RebuildFrequency = 30;

		[SerializeField]
		[Tooltip("Automatically update any Mesh, Box, or Sphere collider components when the mesh is extruded.")]
		private bool m_UpdateColliders = true;

		[SerializeField]
		[Tooltip("The number of sides that comprise the radius of the mesh.")]
		private int m_Sides = 8;

		[SerializeField]
		[Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
		private float m_SegmentsPerUnit = 4f;

		[SerializeField]
		[Tooltip("Indicates if the start and end of the mesh are filled. When the target Spline is closed or when the profile of the shape to extrude is concave, this setting is ignored.")]
		private bool m_Capped = true;

		[SerializeField]
		[Tooltip("The radius of the extruded mesh.")]
		private float m_Radius = 0.25f;

		[SerializeField]
		[Tooltip("The section of the Spline to extrude.")]
		private Vector2 m_Range = new Vector2(0f, 1f);

		[SerializeField]
		[Tooltip("Set true to reverse the winding order of vertices so that the face normals are inverted.")]
		private bool m_FlipNormals;

		private Mesh m_Mesh;

		private float m_NextScheduledRebuild;

		private float m_AutosmoothAngle = 180f;

		private bool m_RebuildRequested;

		private bool m_CanCapEnds;

		[SerializeReference]
		private IExtrudeShape m_Shape;

		internal static readonly string k_EmptyContainerError = "Spline Extrude does not have a valid SplineContainer set.";
	}
}
