using System;
using Unity.Collections;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Rendering/2D/Shadow Caster 2D")]
	[MovedFrom(false, "UnityEngine.Experimental.Rendering.Universal", "com.unity.render-pipelines.universal", null)]
	public class ShadowCaster2D : ShadowCasterGroup2D, ISerializationCallbackReceiver
	{
		internal ShadowCaster2D.EdgeProcessing edgeProcessing
		{
			get
			{
				return (ShadowCaster2D.EdgeProcessing)this.m_ShadowMesh.edgeProcessing;
			}
			set
			{
				this.m_ShadowMesh.edgeProcessing = (ShadowMesh2D.EdgeProcessing)value;
			}
		}

		public Mesh mesh
		{
			get
			{
				return this.m_ShadowMesh.mesh;
			}
		}

		public BoundingSphere boundingSphere
		{
			get
			{
				return this.m_ShadowMesh.boundingSphere;
			}
		}

		public float trimEdge
		{
			get
			{
				return this.m_ShadowMesh.trimEdge;
			}
			set
			{
				this.m_ShadowMesh.trimEdge = value;
			}
		}

		public float alphaCutoff
		{
			get
			{
				return this.m_AlphaCutoff;
			}
			set
			{
				this.m_AlphaCutoff = value;
			}
		}

		public Vector3[] shapePath
		{
			get
			{
				return this.m_ShapePath;
			}
		}

		internal int shapePathHash
		{
			get
			{
				return this.m_ShapePathHash;
			}
			set
			{
				this.m_ShapePathHash = value;
			}
		}

		internal ShadowCaster2D.ShadowCastingSources shadowCastingSource
		{
			get
			{
				return this.m_ShadowCastingSource;
			}
			set
			{
				this.m_ShadowCastingSource = value;
			}
		}

		internal Component shadowShape2DComponent
		{
			get
			{
				return this.m_ShadowShape2DComponent;
			}
			set
			{
				this.m_ShadowShape2DComponent = value;
			}
		}

		internal ShadowShape2DProvider shadowShape2DProvider
		{
			get
			{
				return this.m_ShadowShape2DProvider;
			}
			set
			{
				this.m_ShadowShape2DProvider = value;
			}
		}

		internal int spriteMaterialCount
		{
			get
			{
				return this.m_SpriteMaterialCount;
			}
		}

		internal override void CacheValues()
		{
			this.m_CachedPosition = base.transform.position;
			this.m_CachedLossyScale = base.transform.lossyScale;
			this.m_CachedRotation = base.transform.rotation;
			bool flag;
			bool flag2;
			this.m_ShadowMesh.GetFlip(out flag, out flag2);
			Vector3 s = new Vector3((float)(flag ? -1 : 1), (float)(flag2 ? -1 : 1), 1f);
			this.m_CachedShadowMatrix = Matrix4x4.TRS(this.m_CachedPosition, this.m_CachedRotation, s);
			this.m_CachedInverseShadowMatrix = this.m_CachedShadowMatrix.inverse;
			this.m_CachedLocalToWorldMatrix = base.transform.localToWorldMatrix;
		}

		public ShadowCaster2D.ShadowCastingOptions castingOption
		{
			get
			{
				return this.m_CastingOption;
			}
			set
			{
				this.m_CastingOption = value;
			}
		}

		[Obsolete("useRendererSilhoutte is deprecated. Use selfShadows instead")]
		public bool useRendererSilhouette
		{
			get
			{
				return this.m_UseRendererSilhouette && this.m_HasRenderer;
			}
			set
			{
				this.m_UseRendererSilhouette = value;
			}
		}

		public bool selfShadows
		{
			get
			{
				return this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow || this.castingOption == ShadowCaster2D.ShadowCastingOptions.SelfShadow;
			}
			set
			{
				if (value)
				{
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;
						return;
					}
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.NoShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.SelfShadow;
						return;
					}
				}
				else
				{
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.CastShadow;
						return;
					}
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.SelfShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.NoShadow;
					}
				}
			}
		}

		public bool castsShadows
		{
			get
			{
				return this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastShadow || this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;
			}
			set
			{
				if (value)
				{
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.SelfShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;
						return;
					}
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.NoShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.CastShadow;
						return;
					}
				}
				else
				{
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.SelfShadow;
						return;
					}
					if (this.castingOption == ShadowCaster2D.ShadowCastingOptions.CastShadow)
					{
						this.castingOption = ShadowCaster2D.ShadowCastingOptions.NoShadow;
					}
				}
			}
		}

		private static int[] SetDefaultSortingLayers()
		{
			int num = SortingLayer.layers.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = SortingLayer.layers[i].id;
			}
			return array;
		}

		internal bool IsLit(Light2D light)
		{
			Vector3 vector;
			vector.x = light.m_CachedPosition.x - this.boundingSphere.position.x;
			vector.y = light.m_CachedPosition.y - this.boundingSphere.position.y;
			vector.z = light.m_CachedPosition.z - this.boundingSphere.position.z;
			float num = Vector3.SqrMagnitude(vector);
			float num2 = light.boundingSphere.radius + this.boundingSphere.radius;
			return num <= num2 * num2;
		}

		internal bool IsShadowedLayer(int layer)
		{
			return this.m_ApplyToSortingLayers != null && Array.IndexOf<int>(this.m_ApplyToSortingLayers, layer) >= 0;
		}

		private void SetShadowShape(ShadowMesh2D shadowMesh)
		{
			this.m_ForceShadowMeshRebuild = false;
			if (this.m_ShadowCastingSource == ShadowCaster2D.ShadowCastingSources.ShapeEditor)
			{
				NativeArray<Vector3> vertices = new NativeArray<Vector3>(this.m_ShapePath, Allocator.Temp);
				NativeArray<int> indices = new NativeArray<int>(2 * this.m_ShapePath.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				int value = this.m_ShapePath.Length - 1;
				for (int i = 0; i < this.m_ShapePath.Length; i++)
				{
					int num = i << 1;
					indices[num] = value;
					indices[num + 1] = i;
					value = i;
				}
				shadowMesh.SetShapeWithLines(vertices, indices, false);
				vertices.Dispose();
				indices.Dispose();
			}
			if (this.m_ShadowCastingSource == ShadowCaster2D.ShadowCastingSources.ShapeProvider)
			{
				ShapeProviderUtility.PersistantDataCreated(this.m_ShadowShape2DProvider, this.m_ShadowShape2DComponent, shadowMesh);
			}
		}

		private void Awake()
		{
			if (this.m_ShadowCastingSource < ShadowCaster2D.ShadowCastingSources.None)
			{
				this.m_ShadowCastingSource = ShadowCaster2D.ShadowCastingSources.ShapeEditor;
			}
			Vector3 zero = Vector3.zero;
			Vector3 position = base.transform.position;
			if (base.transform.lossyScale.x != 0f && base.transform.lossyScale.y != 0f)
			{
				zero = new Vector3(1f / base.transform.lossyScale.x, 1f / base.transform.lossyScale.y);
				position = new Vector3(zero.x * -base.transform.position.x, zero.y * -base.transform.position.y);
			}
			if (this.m_ApplyToSortingLayers == null)
			{
				this.m_ApplyToSortingLayers = ShadowCaster2D.SetDefaultSortingLayers();
			}
			Bounds bounds = new Bounds(base.transform.position, Vector3.one);
			Renderer component = base.GetComponent<Renderer>();
			if (component != null)
			{
				bounds = component.bounds;
				this.m_SpriteMaterialCount = component.sharedMaterials.Length;
			}
			if (this.m_ShapePath == null || this.m_ShapePath.Length == 0)
			{
				this.m_ShapePath = new Vector3[]
				{
					position + new Vector3(zero.x * bounds.min.x, zero.y * bounds.min.y),
					position + new Vector3(zero.x * bounds.min.x, zero.y * bounds.max.y),
					position + new Vector3(zero.x * bounds.max.x, zero.y * bounds.max.y),
					position + new Vector3(zero.x * bounds.max.x, zero.y * bounds.min.y)
				};
			}
			if (this.m_ShadowMesh == null)
			{
				ShadowMesh2D shadowMesh2D = new ShadowMesh2D();
				this.SetShadowShape(shadowMesh2D);
				this.m_ShadowMesh = shadowMesh2D;
			}
		}

		protected void OnEnable()
		{
			if (this.m_ShadowShape2DProvider != null)
			{
				this.m_ShadowShape2DProvider.Enabled(this.m_ShadowShape2DComponent, this.m_ShadowMesh);
			}
			this.m_ShadowCasterGroup = null;
		}

		protected void OnDisable()
		{
			ShadowCasterGroup2DManager.RemoveFromShadowCasterGroup(this, this.m_ShadowCasterGroup);
			if (this.m_ShadowShape2DProvider != null)
			{
				this.m_ShadowShape2DProvider.Disabled(this.m_ShadowShape2DComponent, this.m_ShadowMesh);
			}
		}

		public void Update()
		{
			Renderer renderer;
			this.m_HasRenderer = base.TryGetComponent<Renderer>(out renderer);
			bool flag = LightUtility.CheckForChange((int)this.m_ShadowCastingSource, ref this.m_PreviousShadowCastingSource);
			flag |= LightUtility.CheckForChange((int)this.edgeProcessing, ref this.m_PreviousEdgeProcessing);
			flag |= (this.edgeProcessing != ShadowCaster2D.EdgeProcessing.None && LightUtility.CheckForChange(this.trimEdge, ref this.m_PreviousTrimEdge));
			flag |= this.m_ForceShadowMeshRebuild;
			if (this.m_ShadowCastingSource == ShadowCaster2D.ShadowCastingSources.ShapeEditor)
			{
				flag |= LightUtility.CheckForChange(this.m_ShapePathHash, ref this.m_PreviousPathHash);
				if (flag)
				{
					this.SetShadowShape(this.m_ShadowMesh);
				}
			}
			else if ((flag || LightUtility.CheckForChange(this.m_ShadowShape2DComponent, ref this.m_PreviousShadowShape2DSource)) && this.m_ShadowShape2DComponent != null)
			{
				this.SetShadowShape(this.m_ShadowMesh);
			}
			this.m_PreviousShadowCasterGroup = this.m_ShadowCasterGroup;
			if (ShadowCasterGroup2DManager.AddToShadowCasterGroup(this, ref this.m_ShadowCasterGroup, ref this.m_Priority) && this.m_ShadowCasterGroup != null)
			{
				if (this.m_PreviousShadowCasterGroup == this)
				{
					ShadowCasterGroup2DManager.RemoveGroup(this);
				}
				ShadowCasterGroup2DManager.RemoveFromShadowCasterGroup(this, this.m_PreviousShadowCasterGroup);
				if (this.m_ShadowCasterGroup == this)
				{
					ShadowCasterGroup2DManager.AddGroup(this);
				}
			}
			if (LightUtility.CheckForChange(this.m_ShadowGroup, ref this.m_PreviousShadowGroup))
			{
				ShadowCasterGroup2DManager.RemoveGroup(this);
				ShadowCasterGroup2DManager.AddGroup(this);
			}
			if (LightUtility.CheckForChange(this.m_CastsShadows, ref this.m_PreviousCastsShadows))
			{
				ShadowCasterGroup2DManager.AddGroup(this);
			}
			if (this.m_ShadowMesh != null)
			{
				this.m_ShadowMesh.UpdateBoundingSphere(base.transform);
			}
		}

		public void OnBeforeSerialize()
		{
			this.m_ComponentVersion = ShadowCaster2D.ComponentVersions.Version_5;
		}

		public void OnAfterDeserialize()
		{
			if (this.m_ComponentVersion < ShadowCaster2D.ComponentVersions.Version_2)
			{
				if (this.m_SelfShadows && this.m_CastsShadows)
				{
					this.m_CastingOption = ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;
				}
				else if (this.m_SelfShadows)
				{
					this.m_CastingOption = ShadowCaster2D.ShadowCastingOptions.SelfShadow;
				}
				else if (this.m_CastsShadows)
				{
					this.m_CastingOption = ShadowCaster2D.ShadowCastingOptions.CastShadow;
				}
				else
				{
					this.m_CastingOption = ShadowCaster2D.ShadowCastingOptions.NoShadow;
				}
			}
			if (this.m_ComponentVersion < ShadowCaster2D.ComponentVersions.Version_3)
			{
				this.m_ShadowMesh = null;
				this.m_ForceShadowMeshRebuild = true;
			}
		}

		private const ShadowCaster2D.ComponentVersions k_CurrentComponentVersion = ShadowCaster2D.ComponentVersions.Version_5;

		[SerializeField]
		private ShadowCaster2D.ComponentVersions m_ComponentVersion;

		[SerializeField]
		private bool m_HasRenderer;

		[SerializeField]
		private bool m_UseRendererSilhouette = true;

		[SerializeField]
		private bool m_CastsShadows = true;

		[SerializeField]
		private bool m_SelfShadows;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_AlphaCutoff = 0.1f;

		[SerializeField]
		private int[] m_ApplyToSortingLayers;

		[SerializeField]
		private Vector3[] m_ShapePath;

		[SerializeField]
		private int m_ShapePathHash;

		[SerializeField]
		private int m_InstanceId;

		[SerializeField]
		private Component m_ShadowShape2DComponent;

		[SerializeReference]
		private ShadowShape2DProvider m_ShadowShape2DProvider;

		[SerializeField]
		private ShadowCaster2D.ShadowCastingSources m_ShadowCastingSource = (ShadowCaster2D.ShadowCastingSources)(-1);

		[SerializeField]
		internal ShadowMesh2D m_ShadowMesh;

		[SerializeField]
		private ShadowCaster2D.ShadowCastingOptions m_CastingOption = ShadowCaster2D.ShadowCastingOptions.CastShadow;

		[SerializeField]
		internal float m_PreviousTrimEdge;

		[SerializeField]
		internal int m_PreviousEdgeProcessing;

		[SerializeField]
		internal int m_PreviousShadowCastingSource;

		[SerializeField]
		internal Component m_PreviousShadowShape2DSource;

		internal ShadowCasterGroup2D m_ShadowCasterGroup;

		internal ShadowCasterGroup2D m_PreviousShadowCasterGroup;

		internal bool m_ForceShadowMeshRebuild;

		private int m_PreviousShadowGroup;

		private bool m_PreviousCastsShadows = true;

		private int m_PreviousPathHash;

		private int m_SpriteMaterialCount;

		internal Vector3 m_CachedPosition;

		internal Vector3 m_CachedLossyScale;

		internal Quaternion m_CachedRotation;

		internal Matrix4x4 m_CachedShadowMatrix;

		internal Matrix4x4 m_CachedInverseShadowMatrix;

		internal Matrix4x4 m_CachedLocalToWorldMatrix;

		internal enum ComponentVersions
		{
			Version_Unserialized,
			Version_1,
			Version_2,
			Version_3,
			Version_4,
			Version_5
		}

		internal enum ShadowCastingSources
		{
			None,
			ShapeEditor,
			ShapeProvider
		}

		public enum ShadowCastingOptions
		{
			SelfShadow,
			CastShadow,
			CastAndSelfShadow,
			NoShadow
		}

		internal enum EdgeProcessing
		{
			None,
			Clipping
		}
	}
}
