using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[ExcludeFromPreset]
	[ExcludeFromObjectFactory]
	[RequireComponent(typeof(ProBuilderMesh))]
	internal sealed class BezierShape : MonoBehaviour
	{
		public bool isEditing
		{
			get
			{
				return this.m_IsEditing;
			}
			set
			{
				this.m_IsEditing = value;
			}
		}

		public ProBuilderMesh mesh
		{
			get
			{
				if (this.m_Mesh == null)
				{
					this.m_Mesh = base.GetComponent<ProBuilderMesh>();
				}
				return this.m_Mesh;
			}
			set
			{
				this.m_Mesh = value;
			}
		}

		public void Init()
		{
			Vector3 vector = new Vector3(0f, 0f, 2f);
			Vector3 vector2 = new Vector3(3f, 0f, 0f);
			this.points.Add(new BezierPoint(Vector3.zero, -vector, vector, Quaternion.identity));
			this.points.Add(new BezierPoint(vector2, vector2 + vector, vector2 + -vector, Quaternion.identity));
		}

		public void Refresh()
		{
			if (this.points.Count < 2)
			{
				this.mesh.Clear();
				this.mesh.ToMesh(MeshTopology.Triangles);
				this.mesh.Refresh(RefreshMask.All);
				return;
			}
			ProBuilderMesh mesh = this.mesh;
			Spline.Extrude(this.points, this.radius, this.columns, this.rows, this.closeLoop, this.smooth, ref mesh);
		}

		public List<BezierPoint> points = new List<BezierPoint>();

		public bool closeLoop;

		public float radius = 0.5f;

		public int rows = 8;

		public int columns = 16;

		public bool smooth = true;

		[SerializeField]
		private bool m_IsEditing;

		private ProBuilderMesh m_Mesh;
	}
}
