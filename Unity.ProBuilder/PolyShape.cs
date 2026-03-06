using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[ExcludeFromPreset]
	[ExcludeFromObjectFactory]
	[ProGridsConditionalSnap]
	public sealed class PolyShape : MonoBehaviour
	{
		public ReadOnlyCollection<Vector3> controlPoints
		{
			get
			{
				return new ReadOnlyCollection<Vector3>(this.m_Points);
			}
		}

		public void SetControlPoints(IList<Vector3> points)
		{
			this.m_Points = points.ToList<Vector3>();
		}

		public float extrude
		{
			get
			{
				return this.m_Extrude;
			}
			set
			{
				this.m_Extrude = value;
			}
		}

		internal PolyShape.PolyEditMode polyEditMode
		{
			get
			{
				return this.m_EditMode;
			}
			set
			{
				this.m_EditMode = value;
			}
		}

		public bool flipNormals
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

		internal ProBuilderMesh mesh
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

		private bool IsSnapEnabled()
		{
			return this.isOnGrid;
		}

		private const string k_IconPath = "Packages/com.unity.probuilder/Content/Icons/EditableMesh/EditableMesh.png";

		private ProBuilderMesh m_Mesh;

		[FormerlySerializedAs("points")]
		[SerializeField]
		internal List<Vector3> m_Points = new List<Vector3>();

		[FormerlySerializedAs("extrude")]
		[SerializeField]
		private float m_Extrude;

		[FormerlySerializedAs("polyEditMode")]
		[SerializeField]
		private PolyShape.PolyEditMode m_EditMode;

		[FormerlySerializedAs("flipNormals")]
		[SerializeField]
		private bool m_FlipNormals;

		[SerializeField]
		internal bool isOnGrid = true;

		internal enum PolyEditMode
		{
			None,
			Path,
			Height,
			Edit
		}
	}
}
