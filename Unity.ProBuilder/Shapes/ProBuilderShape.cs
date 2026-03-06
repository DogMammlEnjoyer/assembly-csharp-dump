using System;
using UnityEngine.ProBuilder.MeshOperations;

namespace UnityEngine.ProBuilder.Shapes
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	internal sealed class ProBuilderShape : MonoBehaviour
	{
		public Shape shape
		{
			get
			{
				return this.m_Shape;
			}
		}

		public Vector3 size
		{
			get
			{
				return this.m_Size;
			}
			set
			{
				this.m_Size.x = ((Math.Abs(value.x) == 0f) ? (Mathf.Sign(this.m_Size.x) * 0.001f) : value.x);
				this.m_Size.y = value.y;
				this.m_Size.z = ((Math.Abs(value.z) == 0f) ? (Mathf.Sign(this.m_Size.z) * 0.001f) : value.z);
			}
		}

		public Quaternion shapeRotation
		{
			get
			{
				return this.m_ShapeRotation;
			}
			set
			{
				this.m_ShapeRotation = value;
			}
		}

		public Vector3 shapeWorldCenter
		{
			get
			{
				return base.transform.TransformPoint(this.m_LocalCenter);
			}
		}

		public Bounds editionBounds
		{
			get
			{
				this.m_EditionBounds.center = this.m_LocalCenter;
				this.m_EditionBounds.size = this.m_Size;
				if (Mathf.Abs(this.m_Size.y) < Mathf.Epsilon)
				{
					this.m_EditionBounds.size = new Vector3(this.m_Size.x, 0f, this.m_Size.z);
				}
				return this.m_EditionBounds;
			}
		}

		public Bounds shapeLocalBounds
		{
			get
			{
				return new Bounds(this.m_LocalCenter, this.size);
			}
		}

		public Bounds shapeWorldBounds
		{
			get
			{
				return new Bounds(this.shapeWorldCenter, this.size);
			}
		}

		public bool isEditable
		{
			get
			{
				return this.m_UnmodifiedMeshVersion == this.mesh.versionIndex;
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
				if (this.m_Mesh == null)
				{
					this.m_Mesh = base.gameObject.AddComponent<ProBuilderMesh>();
				}
				return this.m_Mesh;
			}
		}

		private void OnValidate()
		{
			this.m_Size.x = ((Math.Abs(this.m_Size.x) == 0f) ? 0.001f : this.m_Size.x);
			this.m_Size.z = ((Math.Abs(this.m_Size.z) == 0f) ? 0.001f : this.m_Size.z);
		}

		internal void UpdateShape()
		{
			if (base.gameObject == null || base.gameObject.hideFlags == HideFlags.HideAndDontSave)
			{
				return;
			}
			this.Rebuild(this.mesh.transform.position, this.mesh.transform.rotation, new Bounds(this.shapeWorldCenter, this.size));
		}

		internal void UpdateBounds(Bounds bounds)
		{
			this.Rebuild(this.mesh.transform.position, this.mesh.transform.rotation, bounds);
		}

		internal void Rebuild(Vector3 pivotPosition, Quaternion rotation, Bounds bounds)
		{
			Transform transform = base.transform;
			transform.position = bounds.center;
			transform.rotation = rotation;
			this.size = bounds.size;
			this.Rebuild();
			this.mesh.SetPivot(pivotPosition);
			this.m_LocalCenter = this.mesh.transform.InverseTransformPoint(bounds.center);
			this.m_UnmodifiedMeshVersion = this.mesh.versionIndex;
		}

		internal void Rebuild(Bounds bounds, Quaternion rotation)
		{
			Transform transform = base.transform;
			transform.position = bounds.center;
			transform.rotation = rotation;
			this.size = bounds.size;
			this.Rebuild();
			this.m_UnmodifiedMeshVersion = this.mesh.versionIndex;
		}

		private void Rebuild()
		{
			if (base.gameObject == null || base.gameObject.hideFlags == HideFlags.HideAndDontSave)
			{
				return;
			}
			Bounds currentSize = this.m_Shape.RebuildMesh(this.mesh, this.size, this.shapeRotation);
			currentSize.size = currentSize.size.Abs();
			MeshUtility.FitToSize(this.mesh, currentSize, this.size);
		}

		internal void SetShape(Shape shape)
		{
			this.m_Shape = shape;
			if (this.m_Shape is Plane || this.m_Shape is Sprite)
			{
				Bounds bounds = new Bounds(this.m_LocalCenter, this.size);
				Vector3 center = bounds.center;
				Vector3 size = bounds.size;
				center.y = 0f;
				size.y = 0f;
				this.m_LocalCenter = center;
				this.size = size;
				this.m_Size.y = 0f;
			}
			this.UpdateShape();
			this.m_UnmodifiedMeshVersion = this.mesh.versionIndex;
		}

		internal void RotateInsideBounds(Quaternion deltaRotation)
		{
			this.shapeRotation = deltaRotation * this.shapeRotation;
			Bounds bounds = new Bounds(this.mesh.transform.TransformPoint(this.m_LocalCenter), this.size);
			this.Rebuild(this.mesh.transform.position, this.mesh.transform.rotation, bounds);
		}

		private const string k_IconPath = "Packages/com.unity.probuilder/Content/Icons/EditableMesh/EditableMesh.png";

		[SerializeReference]
		private Shape m_Shape = new Cube();

		[SerializeField]
		private Quaternion m_ShapeRotation = Quaternion.identity;

		private ProBuilderMesh m_Mesh;

		[SerializeField]
		internal ushort m_UnmodifiedMeshVersion;

		[SerializeField]
		private Vector3 m_Size = Vector3.one;

		private Bounds m_EditionBounds;

		[SerializeField]
		private Vector3 m_LocalCenter;
	}
}
