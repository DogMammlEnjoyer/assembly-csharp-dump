using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.U2D;

namespace UnityEngine.Rendering.Universal
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.Universal", "Unity.RenderPipelines.Universal.Runtime", null)]
	[AddComponentMenu("Rendering/2D/Light 2D")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/index.html?subfolder=/manual/2DLightProperties.html")]
	public sealed class Light2D : Light2DBase, ISerializationCallbackReceiver
	{
		internal LightUtility.LightMeshVertex[] vertices
		{
			get
			{
				return this.m_Vertices;
			}
			set
			{
				this.m_Vertices = value;
			}
		}

		internal ushort[] indices
		{
			get
			{
				return this.m_Triangles;
			}
			set
			{
				this.m_Triangles = value;
			}
		}

		internal int batchSlotIndex
		{
			get
			{
				return this.m_BatchSlotIndex;
			}
			set
			{
				this.m_BatchSlotIndex = value;
			}
		}

		private int lightCookieSpriteInstanceID
		{
			get
			{
				Sprite lightCookieSprite = this.lightCookieSprite;
				if (lightCookieSprite == null)
				{
					return 0;
				}
				return lightCookieSprite.GetInstanceID();
			}
		}

		internal bool useCookieSprite
		{
			get
			{
				return (this.lightType == Light2D.LightType.Point || this.lightType == Light2D.LightType.Sprite) && this.lightCookieSprite != null && this.lightCookieSprite.texture != null;
			}
		}

		internal BoundingSphere boundingSphere { get; private set; }

		internal Mesh lightMesh
		{
			get
			{
				if (null == this.m_Mesh)
				{
					this.m_Mesh = new Mesh();
				}
				return this.m_Mesh;
			}
		}

		internal bool hasCachedMesh
		{
			get
			{
				return this.vertices.Length > 1 && this.indices.Length > 1;
			}
		}

		public Light2D.LightType lightType
		{
			get
			{
				return this.m_LightType;
			}
			set
			{
				if (this.m_LightType != value)
				{
					this.UpdateMesh(false);
				}
				this.m_LightType = value;
				Light2DManager.ErrorIfDuplicateGlobalLight(this);
			}
		}

		public int blendStyleIndex
		{
			get
			{
				return this.m_BlendStyleIndex;
			}
			set
			{
				this.m_BlendStyleIndex = value;
			}
		}

		public float shadowIntensity
		{
			get
			{
				return this.m_ShadowIntensity;
			}
			set
			{
				this.m_ShadowIntensity = Mathf.Clamp01(value);
			}
		}

		public float shadowSoftness
		{
			get
			{
				return this.m_ShadowSoftness;
			}
			set
			{
				this.m_ShadowSoftness = value;
			}
		}

		public bool shadowsEnabled
		{
			get
			{
				return this.m_ShadowsEnabled;
			}
			set
			{
				this.m_ShadowsEnabled = value;
			}
		}

		public float shadowVolumeIntensity
		{
			get
			{
				return this.m_ShadowVolumeIntensity;
			}
			set
			{
				this.m_ShadowVolumeIntensity = Mathf.Clamp01(value);
			}
		}

		public bool volumetricShadowsEnabled
		{
			get
			{
				return this.m_ShadowVolumeIntensityEnabled;
			}
			set
			{
				this.m_ShadowVolumeIntensityEnabled = value;
			}
		}

		public Color color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				this.m_Color = value;
			}
		}

		public float intensity
		{
			get
			{
				return this.m_Intensity;
			}
			set
			{
				this.m_Intensity = value;
			}
		}

		[Obsolete]
		public float volumeOpacity
		{
			get
			{
				return this.m_LightVolumeIntensity;
			}
		}

		public float volumeIntensity
		{
			get
			{
				return this.m_LightVolumeIntensity;
			}
			set
			{
				this.m_LightVolumeIntensity = value;
			}
		}

		[Obsolete]
		public bool volumeIntensityEnabled
		{
			get
			{
				return this.m_LightVolumeEnabled;
			}
			set
			{
				this.m_LightVolumeEnabled = value;
			}
		}

		public bool volumetricEnabled
		{
			get
			{
				return this.m_LightVolumeEnabled;
			}
			set
			{
				this.m_LightVolumeEnabled = value;
			}
		}

		public Sprite lightCookieSprite
		{
			get
			{
				if (this.m_LightType == Light2D.LightType.Point)
				{
					return this.m_DeprecatedPointLightCookieSprite;
				}
				return this.m_LightCookieSprite;
			}
			set
			{
				this.m_LightCookieSprite = value;
			}
		}

		public float falloffIntensity
		{
			get
			{
				return this.m_FalloffIntensity;
			}
			set
			{
				this.m_FalloffIntensity = Mathf.Clamp(value, 0f, 1f);
			}
		}

		public float shadowSoftnessFalloffIntensity
		{
			get
			{
				return this.m_ShadowSoftnessFalloffIntensity;
			}
			set
			{
				this.m_ShadowSoftnessFalloffIntensity = Mathf.Clamp(value, 0f, 1f);
			}
		}

		[Obsolete]
		public bool alphaBlendOnOverlap
		{
			get
			{
				return this.m_OverlapOperation == Light2D.OverlapOperation.AlphaBlend;
			}
		}

		public Light2D.OverlapOperation overlapOperation
		{
			get
			{
				return this.m_OverlapOperation;
			}
			set
			{
				this.m_OverlapOperation = value;
			}
		}

		public int lightOrder
		{
			get
			{
				return this.m_LightOrder;
			}
			set
			{
				this.m_LightOrder = value;
			}
		}

		public float normalMapDistance
		{
			get
			{
				return this.m_NormalMapDistance;
			}
		}

		public Light2D.NormalMapQuality normalMapQuality
		{
			get
			{
				return this.m_NormalMapQuality;
			}
		}

		public bool renderVolumetricShadows
		{
			get
			{
				return this.volumetricShadowsEnabled && this.shadowVolumeIntensity > 0f;
			}
		}

		public int[] targetSortingLayers
		{
			get
			{
				return this.m_ApplyToSortingLayers;
			}
			set
			{
				List<int> list = new List<int>();
				for (int i = 0; i < value.Length; i++)
				{
					int num = value[i];
					if (SortingLayer.IsValid(num))
					{
						list.Add(num);
					}
				}
				this.m_ApplyToSortingLayers = list.ToArray();
			}
		}

		private bool IsValidLayer(string name)
		{
			foreach (SortingLayer sortingLayer in Light2DManager.GetCachedSortingLayer())
			{
				if (sortingLayer.name == name)
				{
					return true;
				}
			}
			return false;
		}

		public bool AddTargetSortingLayer(string layerName)
		{
			List<int> list = new List<int>(this.m_ApplyToSortingLayers);
			int item = SortingLayer.NameToID(layerName);
			if (!this.IsValidLayer(layerName) || list.Contains(item))
			{
				return false;
			}
			list.Add(item);
			this.m_ApplyToSortingLayers = list.ToArray();
			return true;
		}

		public bool AddTargetSortingLayer(int layerID)
		{
			return this.AddTargetSortingLayer(SortingLayer.IDToName(layerID));
		}

		public bool RemoveTargetSortingLayer(string layerName)
		{
			List<int> list = new List<int>(this.m_ApplyToSortingLayers);
			int item = SortingLayer.NameToID(layerName);
			if (!this.IsValidLayer(layerName) || !list.Contains(item))
			{
				return false;
			}
			list.Remove(item);
			this.m_ApplyToSortingLayers = list.ToArray();
			return true;
		}

		public bool RemoveTargetSortingLayer(int layerID)
		{
			return this.RemoveTargetSortingLayer(SortingLayer.IDToName(layerID));
		}

		internal void MarkForUpdate()
		{
			this.forceUpdate = true;
		}

		internal void CacheValues()
		{
			this.m_CachedPosition = base.transform.position;
		}

		internal int GetTopMostLitLayer()
		{
			int result = int.MinValue;
			int num = 0;
			SortingLayer[] cachedSortingLayer = Light2DManager.GetCachedSortingLayer();
			for (int i = 0; i < this.m_ApplyToSortingLayers.Length; i++)
			{
				for (int j = cachedSortingLayer.Length - 1; j >= num; j--)
				{
					if (cachedSortingLayer[j].id == this.m_ApplyToSortingLayers[i])
					{
						result = cachedSortingLayer[j].value;
						num = j;
					}
				}
			}
			return result;
		}

		internal Bounds UpdateSpriteMesh()
		{
			if (this.m_LightCookieSprite == null && (this.m_Vertices.Length != 1 || this.m_Triangles.Length != 1))
			{
				this.m_Vertices = new LightUtility.LightMeshVertex[1];
				this.m_Triangles = new ushort[1];
			}
			return LightUtility.GenerateSpriteMesh(this, this.m_LightCookieSprite, LightBatch.GetBatchColor());
		}

		internal void UpdateBatchSlotIndex()
		{
			if (this.lightMesh && this.lightMesh.colors != null && this.lightMesh.colors.Length != 0)
			{
				this.m_BatchSlotIndex = LightBatch.GetBatchSlotIndex(this.lightMesh.colors[0].b);
			}
		}

		internal bool NeedsColorIndexBaking()
		{
			return this.lightMesh && LightBatch.isBatchingSupported && this.lightMesh.colors.Length != 0 && this.lightMesh.colors[0].b == 0f;
		}

		internal void UpdateCookieSpriteTexture()
		{
			RTHandle cookieSpriteTexture = this.m_CookieSpriteTexture;
			if (cookieSpriteTexture != null)
			{
				cookieSpriteTexture.Release();
			}
			if (this.useCookieSprite)
			{
				this.m_CookieSpriteTexture = RTHandles.Alloc(this.lightCookieSprite.texture);
			}
		}

		internal void UpdateMesh(bool forceUpdate = false)
		{
			int shapePathHash = LightUtility.GetShapePathHash(this.shapePath);
			bool flag = LightUtility.CheckForChange(this.m_ShapeLightFalloffSize, ref this.m_PreviousShapeLightFalloffSize);
			bool flag2 = LightUtility.CheckForChange(this.m_ShapeLightParametricRadius, ref this.m_PreviousShapeLightParametricRadius);
			bool flag3 = LightUtility.CheckForChange(this.m_ShapeLightParametricSides, ref this.m_PreviousShapeLightParametricSides);
			bool flag4 = LightUtility.CheckForChange(this.m_ShapeLightParametricAngleOffset, ref this.m_PreviousShapeLightParametricAngleOffset);
			bool flag5 = LightUtility.CheckForChange(this.lightCookieSpriteInstanceID, ref this.m_PreviousLightCookieSprite);
			bool flag6 = LightUtility.CheckForChange(shapePathHash, ref this.m_PreviousShapePathHash);
			bool flag7 = LightUtility.CheckForChange(this.m_LightType, ref this.m_PreviousLightType);
			if (flag || flag2 || flag3 || flag4 || flag5 || flag6 || flag7 || this.NeedsColorIndexBaking() || forceUpdate)
			{
				float batchColor = LightBatch.GetBatchColor();
				switch (this.m_LightType)
				{
				case Light2D.LightType.Parametric:
					this.m_LocalBounds = LightUtility.GenerateParametricMesh(this, this.m_ShapeLightParametricRadius, this.m_ShapeLightFalloffSize, this.m_ShapeLightParametricAngleOffset, this.m_ShapeLightParametricSides, batchColor);
					break;
				case Light2D.LightType.Freeform:
					this.m_LocalBounds = LightUtility.GenerateShapeMesh(this, this.m_ShapePath, this.m_ShapeLightFalloffSize, batchColor);
					break;
				case Light2D.LightType.Sprite:
					this.m_LocalBounds = this.UpdateSpriteMesh();
					break;
				case Light2D.LightType.Point:
					this.m_LocalBounds = LightUtility.GenerateParametricMesh(this, 1.412135f, 0f, 0f, 4, batchColor);
					break;
				}
				this.UpdateCookieSpriteTexture();
				this.UpdateBatchSlotIndex();
			}
		}

		internal void UpdateBoundingSphere()
		{
			if (this.isPointLight)
			{
				this.boundingSphere = new BoundingSphere(base.transform.position, this.m_PointLightOuterRadius);
				return;
			}
			Vector3 a = base.transform.TransformPoint(Vector3.Max(this.m_LocalBounds.max, this.m_LocalBounds.max + this.m_ShapeLightFalloffOffset));
			Vector3 b = base.transform.TransformPoint(Vector3.Min(this.m_LocalBounds.min, this.m_LocalBounds.min + this.m_ShapeLightFalloffOffset));
			Vector3 vector = 0.5f * (a + b);
			float rad = Vector3.Magnitude(a - vector);
			this.boundingSphere = new BoundingSphere(vector, rad);
		}

		internal bool IsLitLayer(int layer)
		{
			if (this.m_ApplyToSortingLayers == null)
			{
				return false;
			}
			for (int i = 0; i < this.m_ApplyToSortingLayers.Length; i++)
			{
				if (this.m_ApplyToSortingLayers[i] == layer)
				{
					return true;
				}
			}
			return false;
		}

		internal Matrix4x4 GetMatrix()
		{
			Matrix4x4 result = base.transform.localToWorldMatrix;
			if (this.lightType == Light2D.LightType.Point)
			{
				Vector3 s = new Vector3(this.pointLightOuterRadius, this.pointLightOuterRadius, this.pointLightOuterRadius);
				result = Matrix4x4.TRS(base.transform.position, base.transform.rotation, s);
			}
			return result;
		}

		private void Awake()
		{
			if (this.m_ApplyToSortingLayers == null)
			{
				this.m_ApplyToSortingLayers = new int[SortingLayer.layers.Length];
				for (int i = 0; i < this.m_ApplyToSortingLayers.Length; i++)
				{
					this.m_ApplyToSortingLayers[i] = SortingLayer.layers[i].id;
				}
			}
		}

		private void OnEnable()
		{
			this.m_PreviousLightCookieSprite = this.lightCookieSpriteInstanceID;
			Light2DManager.RegisterLight(this);
			this.UpdateCookieSpriteTexture();
		}

		private void OnDisable()
		{
			Light2DManager.DeregisterLight(this);
			RTHandle cookieSpriteTexture = this.m_CookieSpriteTexture;
			if (cookieSpriteTexture == null)
			{
				return;
			}
			cookieSpriteTexture.Release();
		}

		private void LateUpdate()
		{
			if (this.m_LightType == Light2D.LightType.Global)
			{
				return;
			}
			this.UpdateMesh(this.forceUpdate);
			this.UpdateBoundingSphere();
			this.forceUpdate = false;
		}

		public void OnBeforeSerialize()
		{
			this.m_ComponentVersion = Light2D.ComponentVersions.Version_2;
		}

		public void OnAfterDeserialize()
		{
			if (this.m_ComponentVersion == Light2D.ComponentVersions.Version_Unserialized)
			{
				this.m_ShadowVolumeIntensityEnabled = (this.m_ShadowVolumeIntensity > 0f);
				this.m_ShadowsEnabled = (this.m_ShadowIntensity > 0f);
				this.m_LightVolumeEnabled = (this.m_LightVolumeIntensity > 0f);
				this.m_NormalMapQuality = ((!this.m_UseNormalMap) ? Light2D.NormalMapQuality.Disabled : this.m_NormalMapQuality);
				this.m_OverlapOperation = (this.m_AlphaBlendOnOverlap ? Light2D.OverlapOperation.AlphaBlend : this.m_OverlapOperation);
				this.m_ComponentVersion = Light2D.ComponentVersions.Version_1;
			}
			if (this.m_ComponentVersion < Light2D.ComponentVersions.Version_2)
			{
				this.m_ShadowSoftness = 0f;
			}
		}

		public float pointLightInnerAngle
		{
			get
			{
				return this.m_PointLightInnerAngle;
			}
			set
			{
				this.m_PointLightInnerAngle = value;
			}
		}

		public float pointLightOuterAngle
		{
			get
			{
				return this.m_PointLightOuterAngle;
			}
			set
			{
				this.m_PointLightOuterAngle = value;
			}
		}

		public float pointLightInnerRadius
		{
			get
			{
				return this.m_PointLightInnerRadius;
			}
			set
			{
				this.m_PointLightInnerRadius = value;
			}
		}

		public float pointLightOuterRadius
		{
			get
			{
				return this.m_PointLightOuterRadius;
			}
			set
			{
				this.m_PointLightOuterRadius = value;
			}
		}

		[Obsolete("pointLightDistance has been changed to normalMapDistance", true)]
		public float pointLightDistance
		{
			get
			{
				return this.m_NormalMapDistance;
			}
		}

		[Obsolete("pointLightQuality has been changed to normalMapQuality", true)]
		public Light2D.NormalMapQuality pointLightQuality
		{
			get
			{
				return this.m_NormalMapQuality;
			}
		}

		internal bool isPointLight
		{
			get
			{
				return this.m_LightType == Light2D.LightType.Point;
			}
		}

		public int shapeLightParametricSides
		{
			get
			{
				return this.m_ShapeLightParametricSides;
			}
		}

		public float shapeLightParametricAngleOffset
		{
			get
			{
				return this.m_ShapeLightParametricAngleOffset;
			}
		}

		public float shapeLightParametricRadius
		{
			get
			{
				return this.m_ShapeLightParametricRadius;
			}
			internal set
			{
				this.m_ShapeLightParametricRadius = value;
			}
		}

		public float shapeLightFalloffSize
		{
			get
			{
				return this.m_ShapeLightFalloffSize;
			}
			set
			{
				this.m_ShapeLightFalloffSize = Mathf.Max(0f, value);
			}
		}

		public Vector3[] shapePath
		{
			get
			{
				return this.m_ShapePath;
			}
			internal set
			{
				this.m_ShapePath = value;
			}
		}

		public void SetShapePath(Vector3[] path)
		{
			this.m_ShapePath = path;
		}

		private const Light2D.ComponentVersions k_CurrentComponentVersion = Light2D.ComponentVersions.Version_2;

		[SerializeField]
		private Light2D.ComponentVersions m_ComponentVersion;

		[SerializeField]
		private Light2D.LightType m_LightType = Light2D.LightType.Point;

		[SerializeField]
		[FormerlySerializedAs("m_LightOperationIndex")]
		private int m_BlendStyleIndex;

		[SerializeField]
		private float m_FalloffIntensity = 0.5f;

		[ColorUsage(true)]
		[SerializeField]
		private Color m_Color = Color.white;

		[SerializeField]
		private float m_Intensity = 1f;

		[FormerlySerializedAs("m_LightVolumeOpacity")]
		[SerializeField]
		private float m_LightVolumeIntensity = 1f;

		[FormerlySerializedAs("m_LightVolumeIntensityEnabled")]
		[SerializeField]
		private bool m_LightVolumeEnabled;

		[SerializeField]
		private int[] m_ApplyToSortingLayers;

		[Reload("Textures/2D/Sparkle.png", ReloadAttribute.Package.Root)]
		[SerializeField]
		private Sprite m_LightCookieSprite;

		[FormerlySerializedAs("m_LightCookieSprite")]
		[SerializeField]
		private Sprite m_DeprecatedPointLightCookieSprite;

		[SerializeField]
		private int m_LightOrder;

		[SerializeField]
		private bool m_AlphaBlendOnOverlap;

		[SerializeField]
		private Light2D.OverlapOperation m_OverlapOperation;

		[FormerlySerializedAs("m_PointLightDistance")]
		[SerializeField]
		private float m_NormalMapDistance = 3f;

		[FormerlySerializedAs("m_PointLightQuality")]
		[SerializeField]
		private Light2D.NormalMapQuality m_NormalMapQuality = Light2D.NormalMapQuality.Disabled;

		[SerializeField]
		private bool m_UseNormalMap;

		[FormerlySerializedAs("m_ShadowIntensityEnabled")]
		[SerializeField]
		private bool m_ShadowsEnabled = true;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShadowIntensity = 0.75f;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShadowSoftness = 0.3f;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShadowSoftnessFalloffIntensity = 0.5f;

		[SerializeField]
		private bool m_ShadowVolumeIntensityEnabled;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShadowVolumeIntensity = 0.75f;

		private Mesh m_Mesh;

		[NonSerialized]
		private LightUtility.LightMeshVertex[] m_Vertices = new LightUtility.LightMeshVertex[1];

		[NonSerialized]
		private ushort[] m_Triangles = new ushort[1];

		private int m_PreviousLightCookieSprite;

		internal Vector3 m_CachedPosition;

		private int m_BatchSlotIndex;

		internal RTHandle m_CookieSpriteTexture;

		internal TextureHandle m_CookieSpriteTextureHandle;

		[SerializeField]
		private Bounds m_LocalBounds;

		internal bool forceUpdate;

		[SerializeField]
		private float m_PointLightInnerAngle = 360f;

		[SerializeField]
		private float m_PointLightOuterAngle = 360f;

		[SerializeField]
		private float m_PointLightInnerRadius;

		[SerializeField]
		private float m_PointLightOuterRadius = 1f;

		[SerializeField]
		private int m_ShapeLightParametricSides = 5;

		[SerializeField]
		private float m_ShapeLightParametricAngleOffset;

		[SerializeField]
		private float m_ShapeLightParametricRadius = 1f;

		[SerializeField]
		private float m_ShapeLightFalloffSize = 0.5f;

		[SerializeField]
		private Vector2 m_ShapeLightFalloffOffset = Vector2.zero;

		[SerializeField]
		private Vector3[] m_ShapePath;

		private float m_PreviousShapeLightFalloffSize = -1f;

		private int m_PreviousShapeLightParametricSides = -1;

		private float m_PreviousShapeLightParametricAngleOffset = -1f;

		private float m_PreviousShapeLightParametricRadius = -1f;

		private int m_PreviousShapePathHash = -1;

		private Light2D.LightType m_PreviousLightType;

		public enum DeprecatedLightType
		{
			Parametric
		}

		public enum LightType
		{
			Parametric,
			Freeform,
			Sprite,
			Point,
			Global
		}

		public enum NormalMapQuality
		{
			Disabled = 2,
			Fast = 0,
			Accurate
		}

		public enum OverlapOperation
		{
			Additive,
			AlphaBlend
		}

		private enum ComponentVersions
		{
			Version_Unserialized,
			Version_1,
			Version_2
		}
	}
}
