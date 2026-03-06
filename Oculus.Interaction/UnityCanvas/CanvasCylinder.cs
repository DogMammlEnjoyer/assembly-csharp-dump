using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
	public class CanvasCylinder : CanvasMesh, ICurvedPlane, ICylinderClipper
	{
		public float Radius
		{
			get
			{
				return this._cylinder.Radius;
			}
		}

		public Cylinder Cylinder
		{
			get
			{
				return this._cylinder;
			}
		}

		public float ArcDegrees { get; private set; }

		public float Rotation { get; private set; }

		public float Bottom { get; private set; }

		public float Top { get; private set; }

		private float CylinderRelativeScale
		{
			get
			{
				return this._cylinder.transform.lossyScale.x / base.transform.lossyScale.x;
			}
		}

		public bool GetCylinderSegment(out CylinderSegment segment)
		{
			segment = new CylinderSegment(this.Rotation, this.ArcDegrees, this.Bottom, this.Top);
			return this._started && base.isActiveAndEnabled;
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void UpdateImposter()
		{
			base.UpdateImposter();
			this.UpdateMeshPosition();
			this.UpdateCurvedPlane();
		}

		protected override Vector3 MeshInverseTransform(Vector3 localPosition)
		{
			float x = Mathf.Atan2(localPosition.x, localPosition.z + this.Radius) * this.Radius;
			float y = localPosition.y;
			return new Vector3(x, y);
		}

		protected override void GenerateMesh(out List<Vector3> verts, out List<int> tris, out List<Vector2> uvs)
		{
			CanvasCylinder.<>c__DisplayClass31_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			verts = new List<Vector3>();
			tris = new List<int>();
			uvs = new List<Vector2>();
			Vector2 worldSize = this.GetWorldSize();
			CS$<>8__locals1.scaledRadius = this.Radius * this.CylinderRelativeScale;
			CS$<>8__locals1.xPos = worldSize.x * 0.5f;
			CS$<>8__locals1.xNeg = -CS$<>8__locals1.xPos;
			CS$<>8__locals1.yPos = worldSize.y * 0.5f;
			CS$<>8__locals1.yNeg = -CS$<>8__locals1.yPos;
			CylinderOrientation orientation = this._orientation;
			Vector2Int vector2Int;
			if (orientation == CylinderOrientation.Vertical || orientation != CylinderOrientation.Horizontal)
			{
				vector2Int = this.<GenerateMesh>g__GetClampedResolution|31_0(CS$<>8__locals1.xPos, CS$<>8__locals1.yPos, ref CS$<>8__locals1);
			}
			else
			{
				vector2Int = this.<GenerateMesh>g__GetClampedResolution|31_0(CS$<>8__locals1.yPos, CS$<>8__locals1.xPos, ref CS$<>8__locals1);
			}
			for (int i = 0; i < vector2Int.y; i++)
			{
				for (int j = 0; j < vector2Int.x; j++)
				{
					float num = (float)j / ((float)vector2Int.x - 1f);
					float num2 = (float)i / ((float)vector2Int.y - 1f);
					verts.Add(this.<GenerateMesh>g__GetCurvedPoint|31_1(num, num2, ref CS$<>8__locals1));
					uvs.Add(new Vector2(num, num2));
				}
			}
			for (int k = 0; k < vector2Int.y - 1; k++)
			{
				for (int l = 0; l < vector2Int.x - 1; l++)
				{
					int num3 = l + k * vector2Int.x;
					int item = num3 + 1;
					int item2 = num3 + vector2Int.x;
					int item3 = num3 + 1 + vector2Int.x;
					tris.Add(num3);
					tris.Add(item3);
					tris.Add(item);
					tris.Add(num3);
					tris.Add(item2);
					tris.Add(item3);
				}
			}
		}

		private void UpdateMeshPosition()
		{
			Vector3 vector = this._cylinder.transform.InverseTransformPoint(base.transform.position);
			Vector3 b = new Vector3(0f, vector.y, 0f);
			Vector3 vector2 = vector - b;
			Vector3 vector3 = Mathf.Approximately(vector2.sqrMagnitude, 0f) ? Vector3.forward : vector2.normalized;
			CylinderOrientation orientation = this._orientation;
			Vector3 upwards;
			if (orientation == CylinderOrientation.Vertical || orientation != CylinderOrientation.Horizontal)
			{
				upwards = Vector3.up;
			}
			else
			{
				upwards = Vector3.right;
			}
			base.transform.position = this._cylinder.transform.TransformPoint(vector3 * this._cylinder.Radius + b);
			base.transform.rotation = this._cylinder.transform.rotation * Quaternion.LookRotation(vector3, upwards);
			if (this._meshCollider != null && this._meshCollider.transform != base.transform && !base.transform.IsChildOf(this._meshCollider.transform))
			{
				this._meshCollider.transform.position = base.transform.position;
				this._meshCollider.transform.rotation = base.transform.rotation;
				this._meshCollider.transform.localScale *= base.transform.lossyScale.x / this._meshCollider.transform.lossyScale.x;
			}
		}

		private Vector2 GetWorldSize()
		{
			Vector2Int baseResolutionToUse = this._canvasRenderTexture.GetBaseResolutionToUse();
			float x = this._canvasRenderTexture.PixelsToUnits((float)Mathf.RoundToInt((float)baseResolutionToUse.x));
			float y = this._canvasRenderTexture.PixelsToUnits((float)Mathf.RoundToInt((float)baseResolutionToUse.y));
			return new Vector2(x, y) / base.transform.lossyScale;
		}

		private void UpdateCurvedPlane()
		{
			Vector2 vector = this.GetWorldSize() / this.CylinderRelativeScale;
			CylinderOrientation orientation = this._orientation;
			float num;
			float num2;
			if (orientation == CylinderOrientation.Vertical || orientation != CylinderOrientation.Horizontal)
			{
				num = vector.x;
				num2 = vector.y;
			}
			else
			{
				num = vector.y;
				num2 = vector.x;
			}
			Vector3 vector2 = this.Cylinder.transform.InverseTransformPoint(base.transform.position);
			this.Rotation = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
			this.ArcDegrees = num * 0.5f / this.Radius * 2f * 57.29578f;
			this.Top = vector2.y + num2 * 0.5f;
			this.Bottom = vector2.y - num2 * 0.5f;
		}

		public void InjectAllCanvasCylinder(CanvasRenderTexture canvasRenderTexture, MeshFilter meshFilter, Cylinder cylinder, CylinderOrientation orientation)
		{
			base.InjectAllCanvasMesh(canvasRenderTexture, meshFilter);
			this.InjectCylinder(cylinder);
			this.InjectOrientation(orientation);
		}

		public void InjectCylinder(Cylinder cylinder)
		{
			this._cylinder = cylinder;
		}

		public void InjectOrientation(CylinderOrientation orientation)
		{
			this._orientation = orientation;
		}

		[CompilerGenerated]
		private Vector2Int <GenerateMesh>g__GetClampedResolution|31_0(float arcMax, float axisMax, ref CanvasCylinder.<>c__DisplayClass31_0 A_3)
		{
			int num = Mathf.Max(2, Mathf.RoundToInt(this._meshGeneration.VerticesPerDegree * 57.29578f * arcMax / A_3.scaledRadius));
			int num2 = Mathf.Max(2, Mathf.RoundToInt((float)num * axisMax / arcMax));
			num = Mathf.Clamp(num, 2, this._meshGeneration.MaxHorizontalResolution);
			num2 = Mathf.Clamp(num2, 2, this._meshGeneration.MaxVerticalResolution);
			return new Vector2Int(num, num2);
		}

		[CompilerGenerated]
		private Vector3 <GenerateMesh>g__GetCurvedPoint|31_1(float u, float v, ref CanvasCylinder.<>c__DisplayClass31_0 A_3)
		{
			float num = Mathf.Lerp(A_3.xNeg, A_3.xPos, u);
			float num2 = Mathf.Lerp(A_3.yNeg, A_3.yPos, v);
			CylinderOrientation orientation = this._orientation;
			Vector3 result;
			if (orientation == CylinderOrientation.Vertical || orientation != CylinderOrientation.Horizontal)
			{
				float f = num / A_3.scaledRadius;
				result.x = Mathf.Sin(f) * A_3.scaledRadius;
				result.y = num2;
				result.z = Mathf.Cos(f) * A_3.scaledRadius - A_3.scaledRadius;
			}
			else
			{
				float f = num2 / A_3.scaledRadius;
				result.x = num;
				result.y = Mathf.Sin(f) * A_3.scaledRadius;
				result.z = Mathf.Cos(f) * A_3.scaledRadius - A_3.scaledRadius;
			}
			return result;
		}

		public const int MIN_RESOLUTION = 2;

		[SerializeField]
		[Tooltip("The cylinder used to dictate the position and radius of the mesh.")]
		private Cylinder _cylinder;

		[SerializeField]
		[Tooltip("Determines how the mesh is projected on the cylinder wall. Vertical results in a left-to-right curvature, Horizontal results in a top-to-bottom curvature.")]
		private CylinderOrientation _orientation;

		[SerializeField]
		private CanvasCylinder.MeshGenerationSettings _meshGeneration = new CanvasCylinder.MeshGenerationSettings
		{
			VerticesPerDegree = 1.4f,
			MaxHorizontalResolution = 128,
			MaxVerticalResolution = 32
		};

		[Serializable]
		public struct MeshGenerationSettings
		{
			[Delayed]
			public float VerticesPerDegree;

			[Delayed]
			public int MaxHorizontalResolution;

			[Delayed]
			public int MaxVerticalResolution;
		}
	}
}
