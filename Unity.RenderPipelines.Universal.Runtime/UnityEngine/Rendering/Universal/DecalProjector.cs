using System;

namespace UnityEngine.Rendering.Universal
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/URP Decal Projector")]
	public class DecalProjector : MonoBehaviour, ISerializationCallbackReceiver
	{
		internal static event DecalProjector.DecalProjectorAction onDecalAdd;

		internal static event DecalProjector.DecalProjectorAction onDecalRemove;

		internal static event DecalProjector.DecalProjectorAction onDecalPropertyChange;

		internal static event Action onAllDecalPropertyChange;

		internal static event DecalProjector.DecalProjectorAction onDecalMaterialChange;

		internal static Material defaultMaterial { get; set; }

		internal static bool isSupported
		{
			get
			{
				return DecalProjector.onDecalAdd != null;
			}
		}

		internal DecalEntity decalEntity { get; set; }

		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
				this.OnValidate();
			}
		}

		public float drawDistance
		{
			get
			{
				return this.m_DrawDistance;
			}
			set
			{
				this.m_DrawDistance = Mathf.Max(0f, value);
				this.OnValidate();
			}
		}

		public float fadeScale
		{
			get
			{
				return this.m_FadeScale;
			}
			set
			{
				this.m_FadeScale = Mathf.Clamp01(value);
				this.OnValidate();
			}
		}

		public float startAngleFade
		{
			get
			{
				return this.m_StartAngleFade;
			}
			set
			{
				this.m_StartAngleFade = Mathf.Clamp(value, 0f, 180f);
				this.OnValidate();
			}
		}

		public float endAngleFade
		{
			get
			{
				return this.m_EndAngleFade;
			}
			set
			{
				this.m_EndAngleFade = Mathf.Clamp(value, this.m_StartAngleFade, 180f);
				this.OnValidate();
			}
		}

		public Vector2 uvScale
		{
			get
			{
				return this.m_UVScale;
			}
			set
			{
				this.m_UVScale = value;
				this.OnValidate();
			}
		}

		public Vector2 uvBias
		{
			get
			{
				return this.m_UVBias;
			}
			set
			{
				this.m_UVBias = value;
				this.OnValidate();
			}
		}

		public RenderingLayerMask renderingLayerMask
		{
			get
			{
				return this.m_RenderingLayerMask;
			}
			set
			{
				this.m_RenderingLayerMask = value;
			}
		}

		public DecalScaleMode scaleMode
		{
			get
			{
				return this.m_ScaleMode;
			}
			set
			{
				this.m_ScaleMode = value;
				this.OnValidate();
			}
		}

		public Vector3 pivot
		{
			get
			{
				return this.m_Offset;
			}
			set
			{
				this.m_Offset = value;
				this.OnValidate();
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
				this.m_Size = value;
				this.OnValidate();
			}
		}

		public float fadeFactor
		{
			get
			{
				return this.m_FadeFactor;
			}
			set
			{
				this.m_FadeFactor = Mathf.Clamp01(value);
				this.OnValidate();
			}
		}

		internal Vector3 effectiveScale
		{
			get
			{
				if (this.m_ScaleMode != DecalScaleMode.InheritFromHierarchy)
				{
					return Vector3.one;
				}
				return base.transform.lossyScale;
			}
		}

		internal Vector3 decalSize
		{
			get
			{
				return new Vector3(this.m_Size.x, this.m_Size.z, this.m_Size.y);
			}
		}

		internal Vector3 decalOffset
		{
			get
			{
				return new Vector3(this.m_Offset.x, -this.m_Offset.z, this.m_Offset.y);
			}
		}

		internal Vector4 uvScaleBias
		{
			get
			{
				return new Vector4(this.m_UVScale.x, this.m_UVScale.y, this.m_UVBias.x, this.m_UVBias.y);
			}
		}

		private void InitMaterial()
		{
			this.m_Material == null;
		}

		private void OnEnable()
		{
			this.InitMaterial();
			this.m_OldMaterial = this.m_Material;
			DecalProjector.DecalProjectorAction decalProjectorAction = DecalProjector.onDecalAdd;
			if (decalProjectorAction == null)
			{
				return;
			}
			decalProjectorAction(this);
		}

		private void OnDisable()
		{
			DecalProjector.DecalProjectorAction decalProjectorAction = DecalProjector.onDecalRemove;
			if (decalProjectorAction == null)
			{
				return;
			}
			decalProjectorAction(this);
		}

		internal void OnValidate()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.m_Material != this.m_OldMaterial)
			{
				DecalProjector.DecalProjectorAction decalProjectorAction = DecalProjector.onDecalMaterialChange;
				if (decalProjectorAction != null)
				{
					decalProjectorAction(this);
				}
				this.m_OldMaterial = this.m_Material;
			}
			else
			{
				DecalProjector.DecalProjectorAction decalProjectorAction2 = DecalProjector.onDecalPropertyChange;
				if (decalProjectorAction2 != null)
				{
					decalProjectorAction2(this);
				}
			}
			this.m_OldDrawDistance = this.m_DrawDistance;
			this.m_OldFadeScale = this.m_FadeScale;
			this.m_OldStartAngleFade = this.m_StartAngleFade;
			this.m_OldEndAngleFade = this.m_EndAngleFade;
			this.m_OldUVScale = this.m_UVScale;
			this.m_OldUVBias = this.m_UVBias;
			this.m_OldScaleMode = this.m_ScaleMode;
			this.m_OldOffset = this.m_Offset;
			this.m_OldSize = this.m_Size;
			this.m_OldFadeFactor = this.m_FadeFactor;
		}

		private void OnDidApplyAnimationProperties()
		{
			if (this.m_OldMaterial != this.m_Material || Mathf.Abs(this.m_OldDrawDistance - this.m_DrawDistance) > Mathf.Epsilon || Mathf.Abs(this.m_OldFadeScale - this.m_FadeScale) > Mathf.Epsilon || Mathf.Abs(this.m_OldStartAngleFade - this.m_StartAngleFade) > Mathf.Epsilon || Mathf.Abs(this.m_OldEndAngleFade - this.m_EndAngleFade) > Mathf.Epsilon || this.m_OldUVScale != this.m_UVScale || this.m_OldUVBias != this.m_UVBias || this.m_OldScaleMode != this.m_ScaleMode || this.m_OldOffset != this.m_Offset || this.m_OldSize != this.m_Size || Mathf.Abs(this.m_OldFadeFactor - this.m_FadeFactor) > Mathf.Epsilon)
			{
				this.OnValidate();
			}
		}

		public bool IsValid()
		{
			return !(this.material == null) && (this.material.FindPass("DBufferProjector") != -1 || this.material.FindPass("DecalProjectorForwardEmissive") != -1 || this.material.FindPass("DecalScreenSpaceProjector") != -1 || this.material.FindPass("DecalGBufferProjector") != -1);
		}

		internal static void UpdateAllDecalProperties()
		{
			Action action = DecalProjector.onAllDecalPropertyChange;
			if (action == null)
			{
				return;
			}
			action();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this.version == DecalProjector.Version.Count)
			{
				this.version = DecalProjector.Version.RenderingLayerMask;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.version == DecalProjector.Version.Count)
			{
				this.version = DecalProjector.Version.Initial;
			}
			if (this.version < DecalProjector.Version.RenderingLayerMask)
			{
				this.m_RenderingLayerMask = this.m_DecalLayerMask;
				this.version = DecalProjector.Version.RenderingLayerMask;
			}
		}

		[SerializeField]
		private Material m_Material;

		[SerializeField]
		private float m_DrawDistance = 1000f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_FadeScale = 0.9f;

		[SerializeField]
		[Range(0f, 180f)]
		private float m_StartAngleFade = 180f;

		[SerializeField]
		[Range(0f, 180f)]
		private float m_EndAngleFade = 180f;

		[SerializeField]
		private Vector2 m_UVScale = new Vector2(1f, 1f);

		[SerializeField]
		private Vector2 m_UVBias = new Vector2(0f, 0f);

		[SerializeField]
		private RenderingLayerMask m_RenderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;

		[SerializeField]
		private DecalScaleMode m_ScaleMode;

		[SerializeField]
		internal Vector3 m_Offset = new Vector3(0f, 0f, 0.5f);

		[SerializeField]
		internal Vector3 m_Size = new Vector3(1f, 1f, 1f);

		[SerializeField]
		[Range(0f, 1f)]
		private float m_FadeFactor = 1f;

		private Material m_OldMaterial;

		private float m_OldDrawDistance = 1000f;

		private float m_OldFadeScale = 0.9f;

		private float m_OldStartAngleFade = 180f;

		private float m_OldEndAngleFade = 180f;

		private Vector2 m_OldUVScale = new Vector2(1f, 1f);

		private Vector2 m_OldUVBias = new Vector2(0f, 0f);

		private DecalScaleMode m_OldScaleMode;

		private Vector3 m_OldOffset = new Vector3(0f, 0f, 0.5f);

		private Vector3 m_OldSize = new Vector3(1f, 1f, 1f);

		private float m_OldFadeFactor = 1f;

		[SerializeField]
		private DecalProjector.Version version = DecalProjector.Version.Count;

		[SerializeField]
		[Obsolete("This field is only kept for migration purpose. Use m_RenderingLayersMask instead. #from(6000.2)", false)]
		private uint m_DecalLayerMask = 1U;

		internal delegate void DecalProjectorAction(DecalProjector decalProjector);

		private enum Version
		{
			Initial,
			RenderingLayerMask,
			Count
		}
	}
}
