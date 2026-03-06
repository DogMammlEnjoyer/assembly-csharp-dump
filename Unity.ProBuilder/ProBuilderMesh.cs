using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
	[AddComponentMenu("//ProBuilder MeshFilter")]
	[RequireComponent(typeof(MeshRenderer))]
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[ExcludeFromPreset]
	[ExcludeFromObjectFactory]
	public sealed class ProBuilderMesh : MonoBehaviour
	{
		public bool userCollisions { get; set; }

		public UnwrapParameters unwrapParameters
		{
			get
			{
				return this.m_UnwrapParameters;
			}
			set
			{
				this.m_UnwrapParameters = value;
			}
		}

		internal MeshRenderer renderer
		{
			get
			{
				if (!base.gameObject.TryGetComponent<MeshRenderer>(out this.m_MeshRenderer))
				{
					return null;
				}
				return this.m_MeshRenderer;
			}
		}

		internal MeshFilter filter
		{
			get
			{
				if (this.m_MeshFilter == null && !base.gameObject.TryGetComponent<MeshFilter>(out this.m_MeshFilter))
				{
					return null;
				}
				return this.m_MeshFilter;
			}
		}

		internal ushort versionIndex
		{
			get
			{
				return this.m_VersionIndex;
			}
		}

		internal ushort nonSerializedVersionIndex
		{
			get
			{
				return this.m_InstanceVersionIndex;
			}
		}

		public bool preserveMeshAssetOnDestroy
		{
			get
			{
				return this.m_PreserveMeshAssetOnDestroy;
			}
			set
			{
				this.m_PreserveMeshAssetOnDestroy = value;
			}
		}

		public bool HasArrays(MeshArrays channels)
		{
			bool flag = false;
			int vertexCount = this.vertexCount;
			flag |= ((channels & MeshArrays.Position) == MeshArrays.Position && this.m_Positions == null);
			flag |= ((channels & MeshArrays.Normal) == MeshArrays.Normal && (this.m_Normals == null || this.m_Normals.Length != vertexCount));
			flag |= ((channels & MeshArrays.Texture0) == MeshArrays.Texture0 && (this.m_Textures0 == null || this.m_Textures0.Length != vertexCount));
			flag |= ((channels & MeshArrays.Texture2) == MeshArrays.Texture2 && (this.m_Textures2 == null || this.m_Textures2.Count != vertexCount));
			flag |= ((channels & MeshArrays.Texture3) == MeshArrays.Texture3 && (this.m_Textures3 == null || this.m_Textures3.Count != vertexCount));
			flag |= ((channels & MeshArrays.Color) == MeshArrays.Color && (this.m_Colors == null || this.m_Colors.Length != vertexCount));
			flag |= ((channels & MeshArrays.Tangent) == MeshArrays.Tangent && (this.m_Tangents == null || this.m_Tangents.Length != vertexCount));
			if ((channels & MeshArrays.Texture1) == MeshArrays.Texture1 && this.mesh != null)
			{
				flag |= !this.mesh.HasVertexAttribute(VertexAttribute.TexCoord1);
			}
			return !flag;
		}

		internal Face[] facesInternal
		{
			get
			{
				return this.m_Faces;
			}
			set
			{
				this.m_Faces = value;
			}
		}

		public IList<Face> faces
		{
			get
			{
				return new ReadOnlyCollection<Face>(this.m_Faces);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_Faces = value.ToArray<Face>();
			}
		}

		internal void InvalidateSharedVertexLookup()
		{
			if (this.m_SharedVertexLookup == null)
			{
				this.m_SharedVertexLookup = new Dictionary<int, int>();
			}
			this.m_SharedVertexLookup.Clear();
			this.m_CacheValid &= ~ProBuilderMesh.CacheValidState.SharedVertex;
		}

		internal void InvalidateSharedTextureLookup()
		{
			if (this.m_SharedTextureLookup == null)
			{
				this.m_SharedTextureLookup = new Dictionary<int, int>();
			}
			this.m_SharedTextureLookup.Clear();
			this.m_CacheValid &= ~ProBuilderMesh.CacheValidState.SharedTexture;
		}

		internal void InvalidateFaces()
		{
			if (this.m_Faces == null)
			{
				this.m_Faces = new Face[0];
				return;
			}
			foreach (Face face in this.faces)
			{
				face.InvalidateCache();
			}
		}

		internal void InvalidateCaches()
		{
			this.InvalidateSharedVertexLookup();
			this.InvalidateSharedTextureLookup();
			this.InvalidateFaces();
			this.m_SelectedCacheDirty = true;
		}

		internal SharedVertex[] sharedVerticesInternal
		{
			get
			{
				return this.m_SharedVertices;
			}
			set
			{
				this.m_SharedVertices = value;
				this.InvalidateSharedVertexLookup();
			}
		}

		public IList<SharedVertex> sharedVertices
		{
			get
			{
				return new ReadOnlyCollection<SharedVertex>(this.m_SharedVertices);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int count = value.Count;
				this.m_SharedVertices = new SharedVertex[count];
				for (int i = 0; i < count; i++)
				{
					this.m_SharedVertices[i] = new SharedVertex(value[i]);
				}
				this.InvalidateSharedVertexLookup();
			}
		}

		internal Dictionary<int, int> sharedVertexLookup
		{
			get
			{
				if ((this.m_CacheValid & ProBuilderMesh.CacheValidState.SharedVertex) != ProBuilderMesh.CacheValidState.SharedVertex)
				{
					if (this.m_SharedVertexLookup == null)
					{
						this.m_SharedVertexLookup = new Dictionary<int, int>();
					}
					SharedVertex.GetSharedVertexLookup(this.m_SharedVertices, this.m_SharedVertexLookup);
					this.m_CacheValid |= ProBuilderMesh.CacheValidState.SharedVertex;
				}
				return this.m_SharedVertexLookup;
			}
		}

		internal void SetSharedVertices(IEnumerable<KeyValuePair<int, int>> indexes)
		{
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			this.m_SharedVertices = SharedVertex.ToSharedVertices(indexes);
			this.InvalidateSharedVertexLookup();
		}

		internal SharedVertex[] sharedTextures
		{
			get
			{
				return this.m_SharedTextures;
			}
			set
			{
				this.m_SharedTextures = value;
				this.InvalidateSharedTextureLookup();
			}
		}

		internal Dictionary<int, int> sharedTextureLookup
		{
			get
			{
				if ((this.m_CacheValid & ProBuilderMesh.CacheValidState.SharedTexture) != ProBuilderMesh.CacheValidState.SharedTexture)
				{
					this.m_CacheValid |= ProBuilderMesh.CacheValidState.SharedTexture;
					if (this.m_SharedTextureLookup == null)
					{
						this.m_SharedTextureLookup = new Dictionary<int, int>();
					}
					SharedVertex.GetSharedVertexLookup(this.m_SharedTextures, this.m_SharedTextureLookup);
				}
				return this.m_SharedTextureLookup;
			}
		}

		internal void SetSharedTextures(IEnumerable<KeyValuePair<int, int>> indexes)
		{
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			this.m_SharedTextures = SharedVertex.ToSharedVertices(indexes);
			this.InvalidateSharedTextureLookup();
		}

		internal Vector3[] positionsInternal
		{
			get
			{
				return this.m_Positions;
			}
			set
			{
				this.m_Positions = value;
			}
		}

		public IList<Vector3> positions
		{
			get
			{
				return new ReadOnlyCollection<Vector3>(this.m_Positions);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_Positions = value.ToArray<Vector3>();
			}
		}

		public Vertex[] GetVertices(IList<int> indexes = null)
		{
			int vertexCount = this.vertexCount;
			int num = (indexes != null) ? indexes.Count : this.vertexCount;
			Vertex[] array = new Vertex[num];
			Vector3[] positionsInternal = this.positionsInternal;
			Color[] colorsInternal = this.colorsInternal;
			Vector2[] texturesInternal = this.texturesInternal;
			Vector4[] tangents = this.GetTangents();
			Vector3[] normals = this.GetNormals();
			Vector2[] array2 = (this.mesh != null) ? this.mesh.uv2 : null;
			List<Vector4> list = new List<Vector4>();
			List<Vector4> list2 = new List<Vector4>();
			this.GetUVs(2, list);
			this.GetUVs(3, list2);
			bool flag = positionsInternal != null && positionsInternal.Length == vertexCount;
			bool flag2 = colorsInternal != null && colorsInternal.Length == vertexCount;
			bool flag3 = normals != null && normals.Length == vertexCount;
			bool flag4 = tangents != null && tangents.Length == vertexCount;
			bool flag5 = texturesInternal != null && texturesInternal.Length == vertexCount;
			bool flag6 = array2 != null && array2.Length == vertexCount;
			bool flag7 = list.Count == vertexCount;
			bool flag8 = list2.Count == vertexCount;
			for (int i = 0; i < num; i++)
			{
				array[i] = new Vertex();
				int num2 = (indexes == null) ? i : indexes[i];
				if (flag)
				{
					array[i].position = positionsInternal[num2];
				}
				if (flag2)
				{
					array[i].color = colorsInternal[num2];
				}
				if (flag3)
				{
					array[i].normal = normals[num2];
				}
				if (flag4)
				{
					array[i].tangent = tangents[num2];
				}
				if (flag5)
				{
					array[i].uv0 = texturesInternal[num2];
				}
				if (flag6)
				{
					array[i].uv2 = array2[num2];
				}
				if (flag7)
				{
					array[i].uv3 = list[num2];
				}
				if (flag8)
				{
					array[i].uv4 = list2[num2];
				}
			}
			return array;
		}

		internal void GetVerticesInList(IList<Vertex> vertices)
		{
			int vertexCount = this.vertexCount;
			vertices.Clear();
			Vector3[] positionsInternal = this.positionsInternal;
			Color[] colorsInternal = this.colorsInternal;
			Vector2[] texturesInternal = this.texturesInternal;
			Vector4[] tangents = this.GetTangents();
			Vector3[] normals = this.GetNormals();
			Vector2[] array = (this.mesh != null) ? this.mesh.uv2 : null;
			List<Vector4> list = new List<Vector4>();
			List<Vector4> list2 = new List<Vector4>();
			this.GetUVs(2, list);
			this.GetUVs(3, list2);
			bool flag = positionsInternal != null && positionsInternal.Length == vertexCount;
			bool flag2 = colorsInternal != null && colorsInternal.Length == vertexCount;
			bool flag3 = normals != null && normals.Length == vertexCount;
			bool flag4 = tangents != null && tangents.Length == vertexCount;
			bool flag5 = texturesInternal != null && texturesInternal.Length == vertexCount;
			bool flag6 = array != null && array.Length == vertexCount;
			bool flag7 = list.Count == vertexCount;
			bool flag8 = list2.Count == vertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				vertices.Add(new Vertex());
				if (flag)
				{
					vertices[i].position = positionsInternal[i];
				}
				if (flag2)
				{
					vertices[i].color = colorsInternal[i];
				}
				if (flag3)
				{
					vertices[i].normal = normals[i];
				}
				if (flag4)
				{
					vertices[i].tangent = tangents[i];
				}
				if (flag5)
				{
					vertices[i].uv0 = texturesInternal[i];
				}
				if (flag6)
				{
					vertices[i].uv2 = array[i];
				}
				if (flag7)
				{
					vertices[i].uv3 = list[i];
				}
				if (flag8)
				{
					vertices[i].uv4 = list2[i];
				}
			}
		}

		public void SetVertices(IList<Vertex> vertices, bool applyMesh = false)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			Vertex vertex = vertices.FirstOrDefault<Vertex>();
			if (vertex == null || !vertex.HasArrays(MeshArrays.Position))
			{
				this.Clear();
				return;
			}
			Vector3[] array;
			Color[] colors;
			Vector2[] array2;
			Vector3[] normals;
			Vector4[] tangents;
			Vector2[] uv;
			List<Vector4> list;
			List<Vector4> list2;
			Vertex.GetArrays(vertices, out array, out colors, out array2, out normals, out tangents, out uv, out list, out list2);
			this.m_Positions = array;
			this.m_Colors = colors;
			this.m_Normals = normals;
			this.m_Tangents = tangents;
			this.m_Textures0 = array2;
			this.m_Textures2 = list;
			this.m_Textures3 = list2;
			if (applyMesh)
			{
				Mesh mesh = this.mesh;
				if (vertex.HasArrays(MeshArrays.Position))
				{
					mesh.vertices = array;
				}
				if (vertex.HasArrays(MeshArrays.Color))
				{
					mesh.colors = colors;
				}
				if (vertex.HasArrays(MeshArrays.Texture0))
				{
					mesh.uv = array2;
				}
				if (vertex.HasArrays(MeshArrays.Normal))
				{
					mesh.normals = normals;
				}
				if (vertex.HasArrays(MeshArrays.Tangent))
				{
					mesh.tangents = tangents;
				}
				if (vertex.HasArrays(MeshArrays.Texture1))
				{
					mesh.uv2 = uv;
				}
				if (vertex.HasArrays(MeshArrays.Texture2))
				{
					mesh.SetUVs(2, list);
				}
				if (vertex.HasArrays(MeshArrays.Texture3))
				{
					mesh.SetUVs(3, list2);
				}
				this.IncrementVersionIndex();
			}
		}

		public IList<Vector3> normals
		{
			get
			{
				if (this.m_Normals == null)
				{
					return null;
				}
				return new ReadOnlyCollection<Vector3>(this.m_Normals);
			}
		}

		internal Vector3[] normalsInternal
		{
			get
			{
				return this.m_Normals;
			}
			set
			{
				this.m_Normals = value;
			}
		}

		public Vector3[] GetNormals()
		{
			if (!this.HasArrays(MeshArrays.Normal))
			{
				Normals.CalculateNormals(this);
			}
			return this.normals.ToArray<Vector3>();
		}

		internal Color[] colorsInternal
		{
			get
			{
				return this.m_Colors;
			}
			set
			{
				this.m_Colors = value;
			}
		}

		public IList<Color> colors
		{
			get
			{
				if (this.m_Colors == null)
				{
					return null;
				}
				return new ReadOnlyCollection<Color>(this.m_Colors);
			}
			set
			{
				if (value == null || value.Count == 0)
				{
					this.m_Colors = null;
					return;
				}
				if (value.Count != this.vertexCount)
				{
					throw new ArgumentOutOfRangeException("value", "Array length must match vertex count.");
				}
				this.m_Colors = value.ToArray<Color>();
			}
		}

		public Color[] GetColors()
		{
			if (this.HasArrays(MeshArrays.Color))
			{
				return this.colors.ToArray<Color>();
			}
			return ArrayUtility.Fill<Color>(Color.white, this.vertexCount);
		}

		public IList<Vector4> tangents
		{
			get
			{
				if (this.m_Tangents != null && this.m_Tangents.Length == this.vertexCount)
				{
					return new ReadOnlyCollection<Vector4>(this.m_Tangents);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.m_Tangents = null;
					return;
				}
				if (value.Count != this.vertexCount)
				{
					throw new ArgumentOutOfRangeException("value", "Tangent array length must match vertex count");
				}
				this.m_Tangents = value.ToArray<Vector4>();
			}
		}

		internal Vector4[] tangentsInternal
		{
			get
			{
				return this.m_Tangents;
			}
			set
			{
				this.m_Tangents = value;
			}
		}

		public Vector4[] GetTangents()
		{
			if (!this.HasArrays(MeshArrays.Tangent))
			{
				Normals.CalculateTangents(this);
			}
			return this.tangents.ToArray<Vector4>();
		}

		internal Vector2[] texturesInternal
		{
			get
			{
				return this.m_Textures0;
			}
			set
			{
				this.m_Textures0 = value;
			}
		}

		internal List<Vector4> textures2Internal
		{
			get
			{
				return this.m_Textures2;
			}
			set
			{
				this.m_Textures2 = value;
			}
		}

		internal List<Vector4> textures3Internal
		{
			get
			{
				return this.m_Textures3;
			}
			set
			{
				this.m_Textures3 = value;
			}
		}

		public IList<Vector2> textures
		{
			get
			{
				if (this.m_Textures0 == null)
				{
					return null;
				}
				return new ReadOnlyCollection<Vector2>(this.m_Textures0);
			}
			set
			{
				if (value == null)
				{
					this.m_Textures0 = null;
					return;
				}
				if (value.Count != this.vertexCount)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.m_Textures0 = value.ToArray<Vector2>();
			}
		}

		public void GetUVs(int channel, List<Vector4> uvs)
		{
			if (uvs == null)
			{
				throw new ArgumentNullException("uvs");
			}
			if (channel < 0 || channel > 3)
			{
				throw new ArgumentOutOfRangeException("channel");
			}
			uvs.Clear();
			switch (channel)
			{
			case 0:
				for (int i = 0; i < this.vertexCount; i++)
				{
					uvs.Add(this.m_Textures0[i]);
				}
				return;
			case 1:
				if (this.mesh != null && this.mesh.uv2 != null)
				{
					Vector2[] uv = this.mesh.uv2;
					for (int j = 0; j < uv.Length; j++)
					{
						uvs.Add(uv[j]);
					}
					return;
				}
				break;
			case 2:
				if (this.m_Textures2 != null)
				{
					uvs.AddRange(this.m_Textures2);
					return;
				}
				break;
			case 3:
				if (this.m_Textures3 != null)
				{
					uvs.AddRange(this.m_Textures3);
				}
				break;
			default:
				return;
			}
		}

		internal ReadOnlyCollection<Vector2> GetUVs(int channel)
		{
			if (channel == 0)
			{
				return new ReadOnlyCollection<Vector2>(this.m_Textures0);
			}
			if (channel == 1)
			{
				return new ReadOnlyCollection<Vector2>(this.mesh.uv2);
			}
			if (channel == 2)
			{
				if (this.m_Textures2 != null)
				{
					return new ReadOnlyCollection<Vector2>(this.m_Textures2.Cast<Vector2>().ToList<Vector2>());
				}
				return null;
			}
			else
			{
				if (channel != 3)
				{
					return null;
				}
				if (this.m_Textures3 != null)
				{
					return new ReadOnlyCollection<Vector2>(this.m_Textures3.Cast<Vector2>().ToList<Vector2>());
				}
				return null;
			}
		}

		public void SetUVs(int channel, List<Vector4> uvs)
		{
			switch (channel)
			{
			case 0:
			{
				Vector2[] textures;
				if (uvs == null)
				{
					textures = null;
				}
				else
				{
					textures = (from x in uvs
					select x).ToArray<Vector2>();
				}
				this.m_Textures0 = textures;
				return;
			}
			case 1:
			{
				Mesh mesh = this.mesh;
				Vector2[] uv;
				if (uvs == null)
				{
					uv = null;
				}
				else
				{
					uv = (from x in uvs
					select x).ToArray<Vector2>();
				}
				mesh.uv2 = uv;
				return;
			}
			case 2:
				this.m_Textures2 = ((uvs != null) ? new List<Vector4>(uvs) : null);
				return;
			case 3:
				this.m_Textures3 = ((uvs != null) ? new List<Vector4>(uvs) : null);
				return;
			default:
				return;
			}
		}

		public int faceCount
		{
			get
			{
				if (this.m_Faces != null)
				{
					return this.m_Faces.Length;
				}
				return 0;
			}
		}

		public int vertexCount
		{
			get
			{
				if (this.m_Positions != null)
				{
					return this.m_Positions.Length;
				}
				return 0;
			}
		}

		public int edgeCount
		{
			get
			{
				int num = 0;
				int i = 0;
				int faceCount = this.faceCount;
				while (i < faceCount)
				{
					num += this.facesInternal[i].edgesInternal.Length;
					i++;
				}
				return num;
			}
		}

		public int indexCount
		{
			get
			{
				if (this.m_Faces != null)
				{
					return this.m_Faces.Sum((Face x) => x.indexesInternal.Length);
				}
				return 0;
			}
		}

		public int triangleCount
		{
			get
			{
				if (this.m_Faces != null)
				{
					return this.m_Faces.Sum((Face x) => x.indexesInternal.Length) / 3;
				}
				return 0;
			}
		}

		public static event Action<ProBuilderMesh> meshWillBeDestroyed;

		internal static event Action<ProBuilderMesh> meshWasInitialized;

		internal static event Action<ProBuilderMesh> componentWillBeDestroyed;

		internal static event Action<ProBuilderMesh> componentHasBeenReset;

		public static event Action<ProBuilderMesh> elementSelectionChanged;

		internal Mesh mesh
		{
			get
			{
				if (this.m_Mesh == null && this.filter != null)
				{
					this.m_Mesh = this.filter.sharedMesh;
				}
				return this.m_Mesh;
			}
			set
			{
				this.m_Mesh = value;
			}
		}

		[Obsolete("InstanceID is not used to track mesh references as of 2023/04/12")]
		internal int id
		{
			get
			{
				return base.gameObject.GetInstanceID();
			}
		}

		public MeshSyncState meshSyncState
		{
			get
			{
				if (this.mesh == null)
				{
					return MeshSyncState.Null;
				}
				if (this.m_VersionIndex != this.m_InstanceVersionIndex && this.m_InstanceVersionIndex != 0)
				{
					return MeshSyncState.NeedsRebuild;
				}
				if (this.mesh.uv2 != null)
				{
					return MeshSyncState.InSync;
				}
				return MeshSyncState.Lightmap;
			}
		}

		internal int meshFormatVersion
		{
			get
			{
				return this.m_MeshFormatVersion;
			}
		}

		private void Awake()
		{
			this.EnsureMeshFilterIsAssigned();
			this.EnsureMeshColliderIsAssigned();
			this.ClearSelection();
			if (this.vertexCount > 0 && this.faceCount > 0 && this.meshSyncState == MeshSyncState.Null)
			{
				using (new ProBuilderMesh.NonVersionedEditScope(this))
				{
					this.Rebuild();
					Action<ProBuilderMesh> action = ProBuilderMesh.meshWasInitialized;
					if (action != null)
					{
						action(this);
					}
				}
			}
		}

		private void Reset()
		{
			if (this.meshSyncState != MeshSyncState.Null)
			{
				this.Rebuild();
				if (ProBuilderMesh.componentHasBeenReset != null)
				{
					ProBuilderMesh.componentHasBeenReset(this);
				}
			}
		}

		private void OnDestroy()
		{
			if (this.m_MeshFilter != null || base.TryGetComponent<MeshFilter>(out this.m_MeshFilter))
			{
				this.m_MeshFilter.hideFlags = HideFlags.None;
			}
			if (ProBuilderMesh.componentWillBeDestroyed != null)
			{
				ProBuilderMesh.componentWillBeDestroyed(this);
			}
			if (!this.preserveMeshAssetOnDestroy && Application.isEditor && !Application.isPlaying && Time.frameCount > 0)
			{
				this.DestroyUnityMesh();
			}
		}

		internal void DestroyUnityMesh()
		{
			if (ProBuilderMesh.meshWillBeDestroyed != null)
			{
				ProBuilderMesh.meshWillBeDestroyed(this);
				return;
			}
			Object.DestroyImmediate(base.gameObject.GetComponent<MeshFilter>().sharedMesh, true);
		}

		private void IncrementVersionIndex()
		{
			ushort num = this.m_VersionIndex + 1;
			this.m_VersionIndex = num;
			if (num == 0)
			{
				this.m_VersionIndex = 1;
			}
			this.m_InstanceVersionIndex = this.m_VersionIndex;
		}

		public void Clear()
		{
			this.m_Faces = new Face[0];
			this.m_Positions = new Vector3[0];
			this.m_Textures0 = new Vector2[0];
			this.m_Textures2 = null;
			this.m_Textures3 = null;
			this.m_Tangents = null;
			this.m_SharedVertices = new SharedVertex[0];
			this.m_SharedTextures = new SharedVertex[0];
			this.InvalidateSharedVertexLookup();
			this.InvalidateSharedTextureLookup();
			this.m_Colors = null;
			this.m_MeshFormatVersion = 2;
			this.IncrementVersionIndex();
			this.ClearSelection();
		}

		internal void EnsureMeshFilterIsAssigned()
		{
			if (this.filter == null)
			{
				this.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (!this.renderer.isPartOfStaticBatch && this.filter.sharedMesh != this.m_Mesh)
			{
				this.filter.sharedMesh = this.m_Mesh;
			}
		}

		internal static ProBuilderMesh CreateInstanceWithPoints(Vector3[] positions)
		{
			if (positions.Length % 4 != 0)
			{
				Log.Warning("Invalid Geometry. Make sure vertices in are pairs of 4 (faces).");
				return null;
			}
			ProBuilderMesh proBuilderMesh = new GameObject
			{
				name = "ProBuilder Mesh"
			}.AddComponent<ProBuilderMesh>();
			proBuilderMesh.m_MeshFormatVersion = 2;
			proBuilderMesh.GeometryWithPoints(positions);
			return proBuilderMesh;
		}

		public static ProBuilderMesh Create()
		{
			ProBuilderMesh proBuilderMesh = new GameObject().AddComponent<ProBuilderMesh>();
			proBuilderMesh.m_MeshFormatVersion = 2;
			proBuilderMesh.Clear();
			return proBuilderMesh;
		}

		public static ProBuilderMesh Create(IEnumerable<Vector3> positions, IEnumerable<Face> faces)
		{
			GameObject gameObject = new GameObject();
			ProBuilderMesh proBuilderMesh = gameObject.AddComponent<ProBuilderMesh>();
			gameObject.name = "ProBuilder Mesh";
			proBuilderMesh.m_MeshFormatVersion = 2;
			proBuilderMesh.RebuildWithPositionsAndFaces(positions, faces);
			return proBuilderMesh;
		}

		public static ProBuilderMesh Create(IList<Vertex> vertices, IList<Face> faces, IList<SharedVertex> sharedVertices = null, IList<SharedVertex> sharedTextures = null, IList<Material> materials = null)
		{
			ProBuilderMesh proBuilderMesh = new GameObject
			{
				name = "ProBuilder Mesh"
			}.AddComponent<ProBuilderMesh>();
			if (materials != null)
			{
				proBuilderMesh.renderer.sharedMaterials = materials.ToArray<Material>();
			}
			proBuilderMesh.m_MeshFormatVersion = 2;
			proBuilderMesh.SetVertices(vertices, false);
			proBuilderMesh.faces = faces;
			proBuilderMesh.sharedVertices = sharedVertices;
			proBuilderMesh.sharedTextures = ((sharedTextures != null) ? sharedTextures.ToArray<SharedVertex>() : null);
			proBuilderMesh.ToMesh(MeshTopology.Triangles);
			proBuilderMesh.Refresh(RefreshMask.All);
			return proBuilderMesh;
		}

		internal void GeometryWithPoints(Vector3[] points)
		{
			Face[] array = new Face[points.Length / 4];
			for (int i = 0; i < points.Length; i += 4)
			{
				array[i / 4] = new Face(new int[]
				{
					i,
					i + 1,
					i + 2,
					i + 1,
					i + 3,
					i + 2
				}, 0, AutoUnwrapSettings.tile, 0, -1, -1, false);
			}
			this.Clear();
			this.positions = points;
			this.m_Faces = array;
			this.m_SharedVertices = SharedVertex.GetSharedVerticesWithPositions(points);
			this.InvalidateCaches();
			this.ToMesh(MeshTopology.Triangles);
			this.Refresh(RefreshMask.All);
		}

		public void RebuildWithPositionsAndFaces(IEnumerable<Vector3> vertices, IEnumerable<Face> faces)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			this.Clear();
			this.m_Positions = vertices.ToArray<Vector3>();
			this.m_Faces = faces.ToArray<Face>();
			this.m_SharedVertices = SharedVertex.GetSharedVerticesWithPositions(this.m_Positions);
			this.InvalidateSharedVertexLookup();
			this.InvalidateSharedTextureLookup();
			this.ToMesh(MeshTopology.Triangles);
			this.Refresh(RefreshMask.All);
		}

		internal void Rebuild()
		{
			this.ToMesh(MeshTopology.Triangles);
			this.Refresh(RefreshMask.All);
		}

		public void ToMesh(MeshTopology preferredTopology = MeshTopology.Triangles)
		{
			bool usedInParticleSystem = false;
			if (this.mesh == null)
			{
				this.mesh = new Mesh
				{
					name = string.Format("pb_Mesh{0}", base.GetInstanceID())
				};
			}
			else if (this.mesh.vertexCount != this.vertexCount)
			{
				usedInParticleSystem = MeshUtility.IsUsedInParticleSystem(this);
				this.mesh.Clear();
			}
			this.mesh.indexFormat = ((this.vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			this.mesh.vertices = this.m_Positions;
			this.mesh.uv2 = null;
			if (this.m_MeshFormatVersion < 2)
			{
				if (this.m_MeshFormatVersion < 1)
				{
					Submesh.MapFaceMaterialsToSubmeshIndex(this);
				}
				if (this.m_MeshFormatVersion < 2)
				{
					UvUnwrapping.UpgradeAutoUVScaleOffset(this);
				}
				this.m_MeshFormatVersion = 2;
			}
			this.m_MeshFormatVersion = 2;
			int materialCount = MaterialUtility.GetMaterialCount(this.renderer);
			Submesh[] submeshes = Submesh.GetSubmeshes(this.facesInternal, materialCount, preferredTopology);
			this.mesh.subMeshCount = submeshes.Length;
			if (this.mesh.subMeshCount == 0)
			{
				this.FinalizeToMesh(usedInParticleSystem);
				return;
			}
			int num = 0;
			bool flag = false;
			for (int i = 0; i < this.mesh.subMeshCount; i++)
			{
				if (submeshes[i].m_Indexes.Length == 0)
				{
					if (!flag)
					{
						MaterialUtility.s_MaterialArray.Clear();
						this.renderer.GetSharedMaterials(MaterialUtility.s_MaterialArray);
						flag = true;
					}
					submeshes[i].submeshIndex = -1;
					MaterialUtility.s_MaterialArray.RemoveAt(num);
					foreach (Face face in this.facesInternal)
					{
						if (num < face.submeshIndex)
						{
							face.submeshIndex--;
						}
					}
				}
				else
				{
					submeshes[i].submeshIndex = num;
					int num2 = 0;
					int[] indexes = submeshes[i].m_Indexes;
					if (submeshes[i].m_Topology == MeshTopology.Triangles && indexes.Length % 3 == 0)
					{
						for (int k = 0; k < indexes.Length; k += 3)
						{
							if (k + 2 < indexes.Length && indexes[k] < this.positions.Count && indexes[k + 1] < this.positions.Count && indexes[k + 2] < this.positions.Count)
							{
								Vector3 lhs = this.positions[indexes[k + 1]] - this.positions[indexes[k]];
								Vector3 rhs = this.positions[indexes[k + 2]] - this.positions[indexes[k]];
								if (Vector3.Cross(lhs, rhs).sqrMagnitude < Mathf.Epsilon)
								{
									num2 += 3;
								}
								else
								{
									indexes[k - num2] = indexes[k];
									indexes[k - num2 + 1] = indexes[k + 1];
									indexes[k - num2 + 2] = indexes[k + 2];
								}
							}
						}
					}
					int[] array;
					if (num2 > 0)
					{
						array = new int[indexes.Length - num2];
						Array.Copy(indexes, 0, array, 0, array.Length);
					}
					else
					{
						array = submeshes[i].m_Indexes;
					}
					this.mesh.SetIndices(array, submeshes[i].m_Topology, submeshes[i].submeshIndex, false);
					num++;
				}
			}
			if (this.mesh.subMeshCount < materialCount)
			{
				int num3 = materialCount - this.mesh.subMeshCount;
				int index = MaterialUtility.s_MaterialArray.Count - num3;
				MaterialUtility.s_MaterialArray.RemoveRange(index, num3);
				flag = true;
			}
			if (flag)
			{
				this.renderer.sharedMaterials = MaterialUtility.s_MaterialArray.ToArray();
			}
			this.FinalizeToMesh(usedInParticleSystem);
		}

		private void FinalizeToMesh(bool usedInParticleSystem)
		{
			this.EnsureMeshFilterIsAssigned();
			if (usedInParticleSystem)
			{
				MeshUtility.RestoreParticleSystem(this);
			}
			this.IncrementVersionIndex();
		}

		public void MakeUnique()
		{
			Mesh mesh;
			if (!(this.mesh != null))
			{
				(mesh = new Mesh()).name = string.Format("pb_Mesh{0}", base.GetInstanceID());
			}
			else
			{
				mesh = Object.Instantiate<Mesh>(this.mesh);
			}
			this.mesh = mesh;
			if (this.meshSyncState == MeshSyncState.InSync)
			{
				this.filter.mesh = this.mesh;
				return;
			}
			this.ToMesh(MeshTopology.Triangles);
			this.Refresh(RefreshMask.All);
		}

		public void CopyFrom(ProBuilderMesh other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.Clear();
			this.positions = other.positions;
			this.sharedVertices = other.sharedVerticesInternal;
			this.SetSharedTextures(other.sharedTextureLookup);
			this.facesInternal = (from x in other.faces
			select new Face(x)).ToArray<Face>();
			List<Vector4> uvs = new List<Vector4>();
			for (int i = 0; i < 4; i++)
			{
				other.GetUVs(i, uvs);
				this.SetUVs(i, uvs);
			}
			this.tangents = other.tangents;
			this.colors = other.colors;
			this.userCollisions = other.userCollisions;
			this.selectable = other.selectable;
			this.unwrapParameters = new UnwrapParameters(other.unwrapParameters);
		}

		public void Refresh(RefreshMask mask = RefreshMask.All)
		{
			if ((mask & RefreshMask.UV) > (RefreshMask)0)
			{
				this.RefreshUV(this.facesInternal);
			}
			if ((mask & RefreshMask.Colors) > (RefreshMask)0)
			{
				this.RefreshColors();
			}
			if ((mask & RefreshMask.Normals) > (RefreshMask)0)
			{
				this.RefreshNormals();
			}
			if ((mask & RefreshMask.Tangents) > (RefreshMask)0)
			{
				this.RefreshTangents();
			}
			if ((mask & RefreshMask.Collisions) > (RefreshMask)0)
			{
				this.EnsureMeshColliderIsAssigned();
			}
			if ((mask & RefreshMask.Bounds) > (RefreshMask)0 && this.mesh != null)
			{
				this.mesh.RecalculateBounds();
			}
			this.IncrementVersionIndex();
		}

		internal void EnsureMeshColliderIsAssigned()
		{
			MeshCollider meshCollider;
			if (base.gameObject.TryGetComponent<MeshCollider>(out meshCollider))
			{
				meshCollider.sharedMesh = ((this.mesh != null && this.mesh.vertexCount > 0) ? this.mesh : null);
			}
		}

		internal int GetUnusedTextureGroup(int i = 1)
		{
			while (Array.Exists<Face>(this.facesInternal, (Face element) => element.textureGroup == i))
			{
				int i2 = i;
				i = i2 + 1;
			}
			return i;
		}

		private static bool IsValidTextureGroup(int group)
		{
			return group > 0;
		}

		internal int UnusedElementGroup(int i = 1)
		{
			while (Array.Exists<Face>(this.facesInternal, (Face element) => element.elementGroup == i))
			{
				int i2 = i;
				i = i2 + 1;
			}
			return i;
		}

		public void RefreshUV(IEnumerable<Face> facesToRefresh)
		{
			if (!this.HasArrays(MeshArrays.Texture0))
			{
				this.m_Textures0 = new Vector2[this.vertexCount];
				Face[] facesInternal = this.facesInternal;
				for (int i = 0; i < facesInternal.Length; i++)
				{
					facesInternal[i].manualUV = false;
				}
				facesToRefresh = this.facesInternal;
			}
			ProBuilderMesh.s_CachedHashSet.Clear();
			foreach (Face face in facesToRefresh)
			{
				if (!face.manualUV)
				{
					int[] indexesInternal = face.indexesInternal;
					if (indexesInternal == null || indexesInternal.Length >= 3)
					{
						int textureGroup = face.textureGroup;
						if (!ProBuilderMesh.IsValidTextureGroup(textureGroup))
						{
							UvUnwrapping.Unwrap(this, face, default(Vector3));
						}
						else if (ProBuilderMesh.s_CachedHashSet.Add(textureGroup))
						{
							UvUnwrapping.ProjectTextureGroup(this, textureGroup, face.uv);
						}
					}
				}
			}
			this.mesh.uv = this.m_Textures0;
			if (this.HasArrays(MeshArrays.Texture2))
			{
				this.mesh.SetUVs(2, this.m_Textures2);
			}
			if (this.HasArrays(MeshArrays.Texture3))
			{
				this.mesh.SetUVs(3, this.m_Textures3);
			}
			this.IncrementVersionIndex();
		}

		internal void SetGroupUV(AutoUnwrapSettings settings, int group)
		{
			if (!ProBuilderMesh.IsValidTextureGroup(group))
			{
				return;
			}
			foreach (Face face in this.facesInternal)
			{
				if (face.textureGroup == group)
				{
					face.uv = settings;
				}
			}
		}

		private void RefreshColors()
		{
			this.filter.sharedMesh.colors = this.m_Colors;
		}

		public void SetFaceColor(Face face, Color color)
		{
			if (face == null)
			{
				throw new ArgumentNullException("face");
			}
			if (!this.HasArrays(MeshArrays.Color))
			{
				this.m_Colors = ArrayUtility.Fill<Color>(Color.white, this.vertexCount);
			}
			foreach (int num in face.distinctIndexes)
			{
				this.m_Colors[num] = color;
			}
		}

		public void SetMaterial(IEnumerable<Face> faces, Material material)
		{
			Material[] sharedMaterials = this.renderer.sharedMaterials;
			int num = sharedMaterials.Length;
			int num2 = -1;
			int num3 = 0;
			while (num3 < num && num2 < 0)
			{
				if (sharedMaterials[num3] == material)
				{
					num2 = num3;
				}
				num3++;
			}
			if (num2 < 0)
			{
				bool[] array = new bool[num];
				foreach (Face face in this.m_Faces)
				{
					array[Math.Clamp(face.submeshIndex, 0, num - 1)] = true;
				}
				num2 = Array.IndexOf<bool>(array, false);
				if (num2 > -1)
				{
					sharedMaterials[num2] = material;
					this.renderer.sharedMaterials = sharedMaterials;
				}
				else
				{
					num2 = sharedMaterials.Length;
					Material[] array2 = new Material[num2 + 1];
					Array.Copy(sharedMaterials, array2, num2);
					array2[num2] = material;
					this.renderer.sharedMaterials = array2;
				}
			}
			foreach (Face face2 in faces)
			{
				face2.submeshIndex = num2;
			}
			this.IncrementVersionIndex();
		}

		private void RefreshNormals()
		{
			Normals.CalculateNormals(this);
			this.mesh.normals = this.m_Normals;
		}

		private void RefreshTangents()
		{
			Normals.CalculateTangents(this);
			this.mesh.tangents = this.m_Tangents;
		}

		internal int GetSharedVertexHandle(int vertex)
		{
			int result;
			if (this.m_SharedVertexLookup.TryGetValue(vertex, out result))
			{
				return result;
			}
			for (int i = 0; i < this.m_SharedVertices.Length; i++)
			{
				int j = 0;
				int count = this.m_SharedVertices[i].Count;
				while (j < count)
				{
					if (this.m_SharedVertices[i][j] == vertex)
					{
						return i;
					}
					j++;
				}
			}
			throw new ArgumentOutOfRangeException("vertex");
		}

		internal HashSet<int> GetSharedVertexHandles(IEnumerable<int> vertices)
		{
			Dictionary<int, int> sharedVertexLookup = this.sharedVertexLookup;
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int key in vertices)
			{
				hashSet.Add(sharedVertexLookup[key]);
			}
			return hashSet;
		}

		public List<int> GetCoincidentVertices(IEnumerable<int> vertices)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			List<int> list = new List<int>();
			this.GetCoincidentVertices(vertices, list);
			return list;
		}

		public void GetCoincidentVertices(IEnumerable<Face> faces, List<int> coincident)
		{
			if (faces == null)
			{
				throw new ArgumentNullException("faces");
			}
			if (coincident == null)
			{
				throw new ArgumentNullException("coincident");
			}
			coincident.Clear();
			ProBuilderMesh.s_CachedHashSet.Clear();
			Dictionary<int, int> sharedVertexLookup = this.sharedVertexLookup;
			foreach (Face face in faces)
			{
				foreach (int key in face.distinctIndexesInternal)
				{
					int num = sharedVertexLookup[key];
					if (ProBuilderMesh.s_CachedHashSet.Add(num))
					{
						SharedVertex sharedVertex = this.m_SharedVertices[num];
						int j = 0;
						int count = sharedVertex.Count;
						while (j < count)
						{
							coincident.Add(sharedVertex[j]);
							j++;
						}
					}
				}
			}
		}

		public void GetCoincidentVertices(IEnumerable<Edge> edges, List<int> coincident)
		{
			if (this.faces == null)
			{
				throw new ArgumentNullException("edges");
			}
			if (coincident == null)
			{
				throw new ArgumentNullException("coincident");
			}
			coincident.Clear();
			ProBuilderMesh.s_CachedHashSet.Clear();
			Dictionary<int, int> sharedVertexLookup = this.sharedVertexLookup;
			foreach (Edge edge in edges)
			{
				int num = sharedVertexLookup[edge.a];
				if (ProBuilderMesh.s_CachedHashSet.Add(num))
				{
					SharedVertex sharedVertex = this.m_SharedVertices[num];
					int i = 0;
					int count = sharedVertex.Count;
					while (i < count)
					{
						coincident.Add(sharedVertex[i]);
						i++;
					}
				}
				num = sharedVertexLookup[edge.b];
				if (ProBuilderMesh.s_CachedHashSet.Add(num))
				{
					SharedVertex sharedVertex2 = this.m_SharedVertices[num];
					int j = 0;
					int count2 = sharedVertex2.Count;
					while (j < count2)
					{
						coincident.Add(sharedVertex2[j]);
						j++;
					}
				}
			}
		}

		public void GetCoincidentVertices(IEnumerable<int> vertices, List<int> coincident)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			if (coincident == null)
			{
				throw new ArgumentNullException("coincident");
			}
			coincident.Clear();
			ProBuilderMesh.s_CachedHashSet.Clear();
			Dictionary<int, int> sharedVertexLookup = this.sharedVertexLookup;
			foreach (int key in vertices)
			{
				int num = sharedVertexLookup[key];
				if (ProBuilderMesh.s_CachedHashSet.Add(num))
				{
					SharedVertex sharedVertex = this.m_SharedVertices[num];
					int i = 0;
					int count = sharedVertex.Count;
					while (i < count)
					{
						coincident.Add(sharedVertex[i]);
						i++;
					}
				}
			}
		}

		public void GetCoincidentVertices(int vertex, List<int> coincident)
		{
			if (coincident == null)
			{
				throw new ArgumentNullException("coincident");
			}
			int num;
			if (!this.sharedVertexLookup.TryGetValue(vertex, out num))
			{
				throw new ArgumentOutOfRangeException("vertex");
			}
			SharedVertex sharedVertex = this.m_SharedVertices[num];
			int i = 0;
			int count = sharedVertex.Count;
			while (i < count)
			{
				coincident.Add(sharedVertex[i]);
				i++;
			}
		}

		public void SetVerticesCoincident(IEnumerable<int> vertices)
		{
			Dictionary<int, int> sharedVertexLookup = this.sharedVertexLookup;
			List<int> list = new List<int>();
			this.GetCoincidentVertices(vertices, list);
			SharedVertex.SetCoincident(ref sharedVertexLookup, list);
			this.SetSharedVertices(sharedVertexLookup);
		}

		internal void SetTexturesCoincident(IEnumerable<int> vertices)
		{
			Dictionary<int, int> sharedTextureLookup = this.sharedTextureLookup;
			SharedVertex.SetCoincident(ref sharedTextureLookup, vertices);
			this.SetSharedTextures(sharedTextureLookup);
		}

		internal void AddToSharedVertex(int sharedVertexHandle, int vertex)
		{
			if (sharedVertexHandle < 0 || sharedVertexHandle >= this.m_SharedVertices.Length)
			{
				throw new ArgumentOutOfRangeException("sharedVertexHandle");
			}
			this.m_SharedVertices[sharedVertexHandle].Add(vertex);
			this.InvalidateSharedVertexLookup();
		}

		internal void AddSharedVertex(SharedVertex vertex)
		{
			if (vertex == null)
			{
				throw new ArgumentNullException("vertex");
			}
			this.m_SharedVertices = this.m_SharedVertices.Add(vertex);
			this.InvalidateSharedVertexLookup();
		}

		public bool selectable
		{
			get
			{
				return this.m_IsSelectable;
			}
			set
			{
				this.m_IsSelectable = value;
			}
		}

		public int selectedFaceCount
		{
			get
			{
				return this.m_SelectedFaces.Length;
			}
		}

		public int selectedVertexCount
		{
			get
			{
				return this.m_SelectedVertices.Length;
			}
		}

		public int selectedEdgeCount
		{
			get
			{
				return this.m_SelectedEdges.Length;
			}
		}

		internal int selectedSharedVerticesCount
		{
			get
			{
				this.CacheSelection();
				return this.m_SelectedSharedVerticesCount;
			}
		}

		internal int selectedCoincidentVertexCount
		{
			get
			{
				this.CacheSelection();
				return this.m_SelectedCoincidentVertexCount;
			}
		}

		internal IEnumerable<int> selectedSharedVertices
		{
			get
			{
				this.CacheSelection();
				return this.m_SelectedSharedVertices;
			}
		}

		internal IEnumerable<int> selectedCoincidentVertices
		{
			get
			{
				this.CacheSelection();
				return this.m_SelectedCoincidentVertices;
			}
		}

		private void CacheSelection()
		{
			if (this.m_SelectedCacheDirty)
			{
				this.m_SelectedCacheDirty = false;
				this.m_SelectedSharedVertices.Clear();
				this.m_SelectedCoincidentVertices.Clear();
				Dictionary<int, int> sharedVertexLookup = this.sharedVertexLookup;
				this.m_SelectedSharedVerticesCount = 0;
				this.m_SelectedCoincidentVertexCount = 0;
				try
				{
					foreach (int key in this.m_SelectedVertices)
					{
						if (this.m_SelectedSharedVertices.Add(sharedVertexLookup[key]))
						{
							SharedVertex sharedVertex = this.sharedVerticesInternal[sharedVertexLookup[key]];
							this.m_SelectedSharedVerticesCount++;
							this.m_SelectedCoincidentVertexCount += sharedVertex.Count;
							this.m_SelectedCoincidentVertices.AddRange(sharedVertex);
						}
					}
				}
				catch
				{
					this.ClearSelection();
				}
			}
		}

		public Face[] GetSelectedFaces()
		{
			int num = this.m_SelectedFaces.Length;
			Face[] array = new Face[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = this.m_Faces[this.m_SelectedFaces[i]];
			}
			return array;
		}

		public ReadOnlyCollection<int> selectedFaceIndexes
		{
			get
			{
				return new ReadOnlyCollection<int>(this.m_SelectedFaces);
			}
		}

		public ReadOnlyCollection<int> selectedVertices
		{
			get
			{
				return new ReadOnlyCollection<int>(this.m_SelectedVertices);
			}
		}

		public ReadOnlyCollection<Edge> selectedEdges
		{
			get
			{
				return new ReadOnlyCollection<Edge>(this.m_SelectedEdges);
			}
		}

		internal Face[] selectedFacesInternal
		{
			get
			{
				return this.GetSelectedFaces();
			}
			set
			{
				this.m_SelectedFaces = (from x in value
				select Array.IndexOf<Face>(this.m_Faces, x)).ToArray<int>();
			}
		}

		internal int[] selectedFaceIndicesInternal
		{
			get
			{
				return this.m_SelectedFaces;
			}
			set
			{
				this.m_SelectedFaces = value;
			}
		}

		internal Edge[] selectedEdgesInternal
		{
			get
			{
				return this.m_SelectedEdges;
			}
			set
			{
				this.m_SelectedEdges = value;
			}
		}

		internal int[] selectedIndexesInternal
		{
			get
			{
				return this.m_SelectedVertices;
			}
			set
			{
				this.m_SelectedVertices = value;
			}
		}

		internal Face GetActiveFace()
		{
			if (this.selectedFaceCount < 1)
			{
				return null;
			}
			return this.m_Faces[this.selectedFaceIndicesInternal[this.selectedFaceCount - 1]];
		}

		internal Edge GetActiveEdge()
		{
			if (this.selectedEdgeCount < 1)
			{
				return Edge.Empty;
			}
			return this.m_SelectedEdges[this.selectedEdgeCount - 1];
		}

		internal int GetActiveVertex()
		{
			if (this.selectedVertexCount < 1)
			{
				return -1;
			}
			return this.m_SelectedVertices[this.selectedVertexCount - 1];
		}

		internal void AddToFaceSelection(int index)
		{
			if (index > -1)
			{
				this.SetSelectedFaces(this.m_SelectedFaces.Add(index));
			}
		}

		public void SetSelectedFaces(IEnumerable<Face> selected)
		{
			this.SetSelectedFaces((selected != null) ? (from x in selected
			select Array.IndexOf<Face>(this.facesInternal, x)) : null);
		}

		internal void SetSelectedFaces(IEnumerable<int> selected)
		{
			if (selected == null)
			{
				this.ClearSelection();
			}
			else
			{
				this.m_SelectedFaces = selected.ToArray<int>();
				this.m_SelectedVertices = this.m_SelectedFaces.SelectMany((int x) => this.facesInternal[x].distinctIndexesInternal).ToArray<int>();
				this.m_SelectedEdges = this.m_SelectedFaces.SelectMany((int x) => this.facesInternal[x].edges).ToArray<Edge>();
			}
			this.m_SelectedCacheDirty = true;
			if (ProBuilderMesh.elementSelectionChanged != null)
			{
				ProBuilderMesh.elementSelectionChanged(this);
			}
		}

		public void SetSelectedEdges(IEnumerable<Edge> edges)
		{
			if (edges == null)
			{
				this.ClearSelection();
			}
			else
			{
				this.m_SelectedFaces = new int[0];
				this.m_SelectedEdges = edges.ToArray<Edge>();
				this.m_SelectedVertices = this.m_SelectedEdges.AllTriangles();
			}
			this.m_SelectedCacheDirty = true;
			if (ProBuilderMesh.elementSelectionChanged != null)
			{
				ProBuilderMesh.elementSelectionChanged(this);
			}
		}

		public void SetSelectedVertices(IEnumerable<int> vertices)
		{
			this.m_SelectedFaces = new int[0];
			this.m_SelectedEdges = new Edge[0];
			this.m_SelectedVertices = ((vertices != null) ? vertices.Distinct<int>().ToArray<int>() : new int[0]);
			this.m_SelectedCacheDirty = true;
			if (ProBuilderMesh.elementSelectionChanged != null)
			{
				ProBuilderMesh.elementSelectionChanged(this);
			}
		}

		internal void RemoveFromFaceSelectionAtIndex(int index)
		{
			this.SetSelectedFaces(this.m_SelectedFaces.RemoveAt(index));
		}

		public void ClearSelection()
		{
			this.m_SelectedFaces = new int[0];
			this.m_SelectedEdges = new Edge[0];
			this.m_SelectedVertices = new int[0];
			this.m_SelectedCacheDirty = true;
		}

		internal const HideFlags k_MeshFilterHideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;

		private const string k_IconPath = "Packages/com.unity.probuilder/Content/Icons/EditableMesh/EditableMesh.png";

		private const int k_UVChannelCount = 4;

		internal const int k_MeshFormatVersion = 2;

		internal const int k_MeshFormatVersionSubmeshMaterialRefactor = 1;

		internal const int k_MeshFormatVersionAutoUVScaleOffset = 2;

		public const uint maxVertexCount = 65535U;

		[SerializeField]
		private int m_MeshFormatVersion;

		[SerializeField]
		[FormerlySerializedAs("_quads")]
		private Face[] m_Faces;

		[SerializeField]
		[FormerlySerializedAs("_sharedIndices")]
		[FormerlySerializedAs("m_SharedVertexes")]
		private SharedVertex[] m_SharedVertices;

		[NonSerialized]
		private ProBuilderMesh.CacheValidState m_CacheValid;

		[NonSerialized]
		private Dictionary<int, int> m_SharedVertexLookup;

		[SerializeField]
		[FormerlySerializedAs("_sharedIndicesUV")]
		private SharedVertex[] m_SharedTextures;

		[NonSerialized]
		private Dictionary<int, int> m_SharedTextureLookup;

		[SerializeField]
		[FormerlySerializedAs("_vertices")]
		private Vector3[] m_Positions;

		[SerializeField]
		[FormerlySerializedAs("_uv")]
		private Vector2[] m_Textures0;

		[SerializeField]
		[FormerlySerializedAs("_uv3")]
		private List<Vector4> m_Textures2;

		[SerializeField]
		[FormerlySerializedAs("_uv4")]
		private List<Vector4> m_Textures3;

		[SerializeField]
		[FormerlySerializedAs("_tangents")]
		private Vector4[] m_Tangents;

		[NonSerialized]
		private Vector3[] m_Normals;

		[SerializeField]
		[FormerlySerializedAs("_colors")]
		private Color[] m_Colors;

		[FormerlySerializedAs("unwrapParameters")]
		[SerializeField]
		private UnwrapParameters m_UnwrapParameters;

		[FormerlySerializedAs("dontDestroyMeshOnDelete")]
		[SerializeField]
		private bool m_PreserveMeshAssetOnDestroy;

		[SerializeField]
		internal string assetGuid;

		[SerializeField]
		private Mesh m_Mesh;

		[NonSerialized]
		private MeshRenderer m_MeshRenderer;

		[NonSerialized]
		private MeshFilter m_MeshFilter;

		internal const ushort k_UnitializedVersionIndex = 0;

		[SerializeField]
		private ushort m_VersionIndex;

		[NonSerialized]
		private ushort m_InstanceVersionIndex;

		private static HashSet<int> s_CachedHashSet = new HashSet<int>();

		[SerializeField]
		private bool m_IsSelectable = true;

		[SerializeField]
		private int[] m_SelectedFaces = new int[0];

		[SerializeField]
		private Edge[] m_SelectedEdges = new Edge[0];

		[SerializeField]
		private int[] m_SelectedVertices = new int[0];

		private bool m_SelectedCacheDirty;

		private int m_SelectedSharedVerticesCount;

		private int m_SelectedCoincidentVertexCount;

		private HashSet<int> m_SelectedSharedVertices = new HashSet<int>();

		private List<int> m_SelectedCoincidentVertices = new List<int>();

		[Flags]
		private enum CacheValidState : byte
		{
			SharedVertex = 1,
			SharedTexture = 2
		}

		internal struct NonVersionedEditScope : IDisposable
		{
			public NonVersionedEditScope(ProBuilderMesh mesh)
			{
				this.m_Mesh = mesh;
				this.m_VersionIndex = mesh.versionIndex;
			}

			public void Dispose()
			{
				this.m_Mesh.m_VersionIndex = this.m_VersionIndex;
				this.m_Mesh.m_InstanceVersionIndex = this.m_VersionIndex;
			}

			private readonly ProBuilderMesh m_Mesh;

			private readonly ushort m_VersionIndex;
		}
	}
}
