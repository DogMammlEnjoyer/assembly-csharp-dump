using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnityEngine.ProBuilder.MeshOperations
{
	public sealed class MeshImporter
	{
		public MeshImporter(GameObject gameObject)
		{
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			this.m_SourceMesh = component.sharedMesh;
			if (this.m_SourceMesh == null)
			{
				throw new ArgumentNullException("gameObject", "GameObject does not contain a valid MeshFilter.sharedMesh.");
			}
			this.m_Destination = gameObject.DemandComponent<ProBuilderMesh>();
			MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
			this.m_SourceMaterials = ((component2 != null) ? component2.sharedMaterials : null);
		}

		public MeshImporter(Mesh sourceMesh, Material[] sourceMaterials, ProBuilderMesh destination)
		{
			if (sourceMesh == null)
			{
				throw new ArgumentNullException("sourceMesh");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			this.m_SourceMesh = sourceMesh;
			this.m_SourceMaterials = sourceMaterials;
			this.m_Destination = destination;
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MeshImporter(ProBuilderMesh destination)
		{
			this.m_Destination = destination;
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Import(GameObject go, MeshImportSettings importSettings = null)
		{
			try
			{
				this.m_SourceMesh = go.GetComponent<MeshFilter>().sharedMesh;
				MeshRenderer component = go.GetComponent<MeshRenderer>();
				this.m_SourceMaterials = ((component != null) ? component.sharedMaterials : null);
				this.Import(importSettings);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
			return true;
		}

		public void Import(MeshImportSettings importSettings = null)
		{
			if (importSettings == null)
			{
				importSettings = MeshImporter.k_DefaultImportSettings;
			}
			Vertex[] vertices = this.m_SourceMesh.GetVertices();
			List<Vertex> list = new List<Vertex>();
			List<Face> list2 = new List<Face>();
			int num = 0;
			int num2 = (this.m_SourceMaterials != null) ? this.m_SourceMaterials.Length : 0;
			for (int i = 0; i < this.m_SourceMesh.subMeshCount; i++)
			{
				MeshTopology topology = this.m_SourceMesh.GetTopology(i);
				if (topology != MeshTopology.Triangles)
				{
					if (topology != MeshTopology.Quads)
					{
						throw new NotSupportedException("ProBuilder only supports importing triangle and quad meshes.");
					}
					int[] indices = this.m_SourceMesh.GetIndices(i);
					for (int j = 0; j < indices.Length; j += 4)
					{
						list2.Add(new Face(new int[]
						{
							num,
							num + 1,
							num + 2,
							num + 2,
							num + 3,
							num
						}, Math.Clamp(i, 0, num2 - 1), AutoUnwrapSettings.tile, 0, -1, -1, true));
						list.Add(vertices[indices[j]]);
						list.Add(vertices[indices[j + 1]]);
						list.Add(vertices[indices[j + 2]]);
						list.Add(vertices[indices[j + 3]]);
						num += 4;
					}
				}
				else
				{
					int[] indices2 = this.m_SourceMesh.GetIndices(i);
					for (int k = 0; k < indices2.Length; k += 3)
					{
						list2.Add(new Face(new int[]
						{
							num,
							num + 1,
							num + 2
						}, Math.Clamp(i, 0, num2 - 1), AutoUnwrapSettings.tile, 0, -1, -1, true));
						list.Add(vertices[indices2[k]]);
						list.Add(vertices[indices2[k + 1]]);
						list.Add(vertices[indices2[k + 2]]);
						num += 3;
					}
				}
			}
			this.m_Vertices = list.ToArray();
			this.m_Destination.Clear();
			this.m_Destination.SetVertices(this.m_Vertices, false);
			this.m_Destination.faces = list2;
			this.m_Destination.sharedVertices = SharedVertex.GetSharedVerticesWithPositions(this.m_Destination.positionsInternal);
			this.m_Destination.sharedTextures = new SharedVertex[0];
			if (importSettings.quads)
			{
				this.m_Destination.ToQuads(this.m_Destination.facesInternal, !importSettings.smoothing);
			}
			if (importSettings.smoothing)
			{
				Smoothing.ApplySmoothingGroups(this.m_Destination, this.m_Destination.facesInternal, importSettings.smoothingAngle, (from x in this.m_Vertices
				select x.normal).ToArray<Vector3>());
				MergeElements.CollapseCoincidentVertices(this.m_Destination, this.m_Destination.facesInternal);
			}
		}

		private static readonly MeshImportSettings k_DefaultImportSettings = new MeshImportSettings
		{
			quads = true,
			smoothing = true,
			smoothingAngle = 1f
		};

		private Mesh m_SourceMesh;

		private Material[] m_SourceMaterials;

		private ProBuilderMesh m_Destination;

		private Vertex[] m_Vertices;
	}
}
