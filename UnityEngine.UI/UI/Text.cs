using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	[AddComponentMenu("UI/Legacy/Text", 100)]
	public class Text : MaskableGraphic, ILayoutElement
	{
		protected Text()
		{
			base.useLegacyMeshGeneration = false;
		}

		public TextGenerator cachedTextGenerator
		{
			get
			{
				TextGenerator result;
				if ((result = this.m_TextCache) == null)
				{
					result = (this.m_TextCache = ((this.m_Text.Length != 0) ? new TextGenerator(this.m_Text.Length) : new TextGenerator()));
				}
				return result;
			}
		}

		public TextGenerator cachedTextGeneratorForLayout
		{
			get
			{
				TextGenerator result;
				if ((result = this.m_TextCacheForLayout) == null)
				{
					result = (this.m_TextCacheForLayout = new TextGenerator());
				}
				return result;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (this.font != null && this.font.material != null && this.font.material.mainTexture != null)
				{
					return this.font.material.mainTexture;
				}
				if (this.m_Material != null)
				{
					return this.m_Material.mainTexture;
				}
				return base.mainTexture;
			}
		}

		public void FontTextureChanged()
		{
			if (!this)
			{
				return;
			}
			if (this.m_DisableFontTextureRebuiltCallback)
			{
				return;
			}
			this.cachedTextGenerator.Invalidate();
			if (!this.IsActive())
			{
				return;
			}
			if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
			{
				this.UpdateGeometry();
				return;
			}
			this.SetAllDirty();
		}

		public Font font
		{
			get
			{
				return this.m_FontData.font;
			}
			set
			{
				if (this.m_FontData.font == value)
				{
					return;
				}
				if (base.isActiveAndEnabled)
				{
					FontUpdateTracker.UntrackText(this);
				}
				this.m_FontData.font = value;
				if (base.isActiveAndEnabled)
				{
					FontUpdateTracker.TrackText(this);
				}
				this.SetAllDirty();
			}
		}

		public virtual string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this.m_Text != value)
					{
						this.m_Text = value;
						this.SetVerticesDirty();
						this.SetLayoutDirty();
					}
					return;
				}
				if (string.IsNullOrEmpty(this.m_Text))
				{
					return;
				}
				this.m_Text = "";
				this.SetVerticesDirty();
			}
		}

		public bool supportRichText
		{
			get
			{
				return this.m_FontData.richText;
			}
			set
			{
				if (this.m_FontData.richText == value)
				{
					return;
				}
				this.m_FontData.richText = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool resizeTextForBestFit
		{
			get
			{
				return this.m_FontData.bestFit;
			}
			set
			{
				if (this.m_FontData.bestFit == value)
				{
					return;
				}
				this.m_FontData.bestFit = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public int resizeTextMinSize
		{
			get
			{
				return this.m_FontData.minSize;
			}
			set
			{
				if (this.m_FontData.minSize == value)
				{
					return;
				}
				this.m_FontData.minSize = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public int resizeTextMaxSize
		{
			get
			{
				return this.m_FontData.maxSize;
			}
			set
			{
				if (this.m_FontData.maxSize == value)
				{
					return;
				}
				this.m_FontData.maxSize = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public TextAnchor alignment
		{
			get
			{
				return this.m_FontData.alignment;
			}
			set
			{
				if (this.m_FontData.alignment == value)
				{
					return;
				}
				this.m_FontData.alignment = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool alignByGeometry
		{
			get
			{
				return this.m_FontData.alignByGeometry;
			}
			set
			{
				if (this.m_FontData.alignByGeometry == value)
				{
					return;
				}
				this.m_FontData.alignByGeometry = value;
				this.SetVerticesDirty();
			}
		}

		public int fontSize
		{
			get
			{
				return this.m_FontData.fontSize;
			}
			set
			{
				if (this.m_FontData.fontSize == value)
				{
					return;
				}
				this.m_FontData.fontSize = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public HorizontalWrapMode horizontalOverflow
		{
			get
			{
				return this.m_FontData.horizontalOverflow;
			}
			set
			{
				if (this.m_FontData.horizontalOverflow == value)
				{
					return;
				}
				this.m_FontData.horizontalOverflow = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public VerticalWrapMode verticalOverflow
		{
			get
			{
				return this.m_FontData.verticalOverflow;
			}
			set
			{
				if (this.m_FontData.verticalOverflow == value)
				{
					return;
				}
				this.m_FontData.verticalOverflow = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_FontData.lineSpacing;
			}
			set
			{
				if (this.m_FontData.lineSpacing == value)
				{
					return;
				}
				this.m_FontData.lineSpacing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public FontStyle fontStyle
		{
			get
			{
				return this.m_FontData.fontStyle;
			}
			set
			{
				if (this.m_FontData.fontStyle == value)
				{
					return;
				}
				this.m_FontData.fontStyle = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				if (!canvas)
				{
					return 1f;
				}
				if (!this.font || this.font.dynamic)
				{
					return canvas.scaleFactor;
				}
				if (this.m_FontData.fontSize <= 0 || this.font.fontSize <= 0)
				{
					return 1f;
				}
				return (float)this.font.fontSize / (float)this.m_FontData.fontSize;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.cachedTextGenerator.Invalidate();
			FontUpdateTracker.TrackText(this);
		}

		protected override void OnDisable()
		{
			FontUpdateTracker.UntrackText(this);
			base.OnDisable();
		}

		protected override void UpdateGeometry()
		{
			if (this.font != null)
			{
				base.UpdateGeometry();
			}
		}

		internal void AssignDefaultFont()
		{
			this.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		}

		internal void AssignDefaultFontIfNecessary()
		{
			if (this.font == null)
			{
				this.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			}
		}

		public TextGenerationSettings GetGenerationSettings(Vector2 extents)
		{
			TextGenerationSettings result = default(TextGenerationSettings);
			result.generationExtents = extents;
			if (this.font != null && this.font.dynamic)
			{
				result.fontSize = this.m_FontData.fontSize;
				result.resizeTextMinSize = this.m_FontData.minSize;
				result.resizeTextMaxSize = this.m_FontData.maxSize;
			}
			result.textAnchor = this.m_FontData.alignment;
			result.alignByGeometry = this.m_FontData.alignByGeometry;
			result.scaleFactor = this.pixelsPerUnit;
			result.color = this.color;
			result.font = this.font;
			result.pivot = base.rectTransform.pivot;
			result.richText = this.m_FontData.richText;
			result.lineSpacing = this.m_FontData.lineSpacing;
			result.fontStyle = this.m_FontData.fontStyle;
			result.resizeTextForBestFit = this.m_FontData.bestFit;
			result.updateBounds = false;
			result.horizontalOverflow = this.m_FontData.horizontalOverflow;
			result.verticalOverflow = this.m_FontData.verticalOverflow;
			return result;
		}

		public static Vector2 GetTextAnchorPivot(TextAnchor anchor)
		{
			switch (anchor)
			{
			case TextAnchor.UpperLeft:
				return new Vector2(0f, 1f);
			case TextAnchor.UpperCenter:
				return new Vector2(0.5f, 1f);
			case TextAnchor.UpperRight:
				return new Vector2(1f, 1f);
			case TextAnchor.MiddleLeft:
				return new Vector2(0f, 0.5f);
			case TextAnchor.MiddleCenter:
				return new Vector2(0.5f, 0.5f);
			case TextAnchor.MiddleRight:
				return new Vector2(1f, 0.5f);
			case TextAnchor.LowerLeft:
				return new Vector2(0f, 0f);
			case TextAnchor.LowerCenter:
				return new Vector2(0.5f, 0f);
			case TextAnchor.LowerRight:
				return new Vector2(1f, 0f);
			default:
				return Vector2.zero;
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (this.font == null)
			{
				return;
			}
			this.m_DisableFontTextureRebuiltCallback = true;
			Vector2 size = base.rectTransform.rect.size;
			TextGenerationSettings generationSettings = this.GetGenerationSettings(size);
			this.cachedTextGenerator.PopulateWithErrors(this.text, generationSettings, base.gameObject);
			IList<UIVertex> verts = this.cachedTextGenerator.verts;
			float d = 1f / this.pixelsPerUnit;
			int count = verts.Count;
			if (count <= 0)
			{
				toFill.Clear();
				return;
			}
			Vector2 vector = new Vector2(verts[0].position.x, verts[0].position.y) * d;
			vector = base.PixelAdjustPoint(vector) - vector;
			toFill.Clear();
			if (vector != Vector2.zero)
			{
				for (int i = 0; i < count; i++)
				{
					int num = i & 3;
					this.m_TempVerts[num] = verts[i];
					UIVertex[] tempVerts = this.m_TempVerts;
					int num2 = num;
					tempVerts[num2].position = tempVerts[num2].position * d;
					UIVertex[] tempVerts2 = this.m_TempVerts;
					int num3 = num;
					tempVerts2[num3].position.x = tempVerts2[num3].position.x + vector.x;
					UIVertex[] tempVerts3 = this.m_TempVerts;
					int num4 = num;
					tempVerts3[num4].position.y = tempVerts3[num4].position.y + vector.y;
					if (num == 3)
					{
						toFill.AddUIVertexQuad(this.m_TempVerts);
					}
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					int num5 = j & 3;
					this.m_TempVerts[num5] = verts[j];
					UIVertex[] tempVerts4 = this.m_TempVerts;
					int num6 = num5;
					tempVerts4[num6].position = tempVerts4[num6].position * d;
					if (num5 == 3)
					{
						toFill.AddUIVertexQuad(this.m_TempVerts);
					}
				}
			}
			this.m_DisableFontTextureRebuiltCallback = false;
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				TextGenerationSettings generationSettings = this.GetGenerationSettings(Vector2.zero);
				return this.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.pixelsPerUnit;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				TextGenerationSettings generationSettings = this.GetGenerationSettings(new Vector2(base.GetPixelAdjustedRect().size.x, 0f));
				return this.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.pixelsPerUnit;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		[SerializeField]
		private FontData m_FontData = FontData.defaultFontData;

		[TextArea(3, 10)]
		[SerializeField]
		protected string m_Text = string.Empty;

		private TextGenerator m_TextCache;

		private TextGenerator m_TextCacheForLayout;

		protected static Material s_DefaultText;

		[NonSerialized]
		protected bool m_DisableFontTextureRebuiltCallback;

		private readonly UIVertex[] m_TempVerts = new UIVertex[4];
	}
}
