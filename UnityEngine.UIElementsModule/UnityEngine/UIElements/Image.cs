using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	public class Image : VisualElement
	{
		[CreateProperty]
		public Texture image
		{
			get
			{
				return this.m_Image;
			}
			set
			{
				bool flag = this.m_Image == value && this.m_ImageIsInline;
				if (!flag)
				{
					this.m_ImageIsInline = (value != null);
					this.SetProperty<Texture, Sprite, VectorImage>(value, ref this.m_Image, ref this.m_Sprite, ref this.m_VectorImage, Image.imageProperty);
				}
			}
		}

		[CreateProperty]
		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				bool flag = this.m_Sprite == value && this.m_ImageIsInline;
				if (!flag)
				{
					this.m_ImageIsInline = (value != null);
					this.SetProperty<Sprite, Texture, VectorImage>(value, ref this.m_Sprite, ref this.m_Image, ref this.m_VectorImage, Image.spriteProperty);
				}
			}
		}

		[CreateProperty]
		public VectorImage vectorImage
		{
			get
			{
				return this.m_VectorImage;
			}
			set
			{
				bool flag = this.m_VectorImage == value && this.m_ImageIsInline;
				if (!flag)
				{
					this.m_ImageIsInline = (value != null);
					this.SetProperty<VectorImage, Texture, Sprite>(value, ref this.m_VectorImage, ref this.m_Image, ref this.m_Sprite, Image.vectorImageProperty);
				}
			}
		}

		[CreateProperty]
		public Rect sourceRect
		{
			get
			{
				return this.GetSourceRect();
			}
			set
			{
				bool flag = this.GetSourceRect() == value;
				if (!flag)
				{
					bool flag2 = this.sprite != null;
					if (flag2)
					{
						Debug.LogError("Cannot set sourceRect on a sprite image");
					}
					else
					{
						this.CalculateUV(value);
						base.NotifyPropertyChanged(Image.sourceRectProperty);
					}
				}
			}
		}

		[CreateProperty]
		public Rect uv
		{
			get
			{
				return this.m_UV;
			}
			set
			{
				bool flag = this.m_UV == value;
				if (!flag)
				{
					this.m_UV = value;
					base.NotifyPropertyChanged(Image.uvProperty);
				}
			}
		}

		[CreateProperty]
		public ScaleMode scaleMode
		{
			get
			{
				return this.m_ScaleMode;
			}
			set
			{
				bool flag = this.m_ScaleMode == value && this.m_ScaleModeIsInline;
				if (!flag)
				{
					this.m_ScaleModeIsInline = true;
					this.SetScaleMode(value);
				}
			}
		}

		[CreateProperty]
		public Color tintColor
		{
			get
			{
				return this.m_TintColor;
			}
			set
			{
				bool flag = this.m_TintColor == value && this.m_TintColorIsInline;
				if (!flag)
				{
					this.m_TintColorIsInline = true;
					this.SetTintColor(value);
				}
			}
		}

		public Image()
		{
			base.AddToClassList(Image.ussClassName);
			this.m_ScaleMode = ScaleMode.ScaleToFit;
			this.m_TintColor = Color.white;
			this.m_UV = new Rect(0f, 0f, 1f, 1f);
			base.requireMeasureFunction = true;
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
		}

		private Vector2 GetTextureDisplaySize(Texture texture)
		{
			Vector2 zero = Vector2.zero;
			bool flag = texture != null;
			if (flag)
			{
				zero = new Vector2((float)texture.width, (float)texture.height);
			}
			return zero;
		}

		private Vector2 GetTextureDisplaySize(Sprite sprite)
		{
			Vector2 result = Vector2.zero;
			bool flag = sprite != null;
			if (flag)
			{
				float d = UIElementsUtility.PixelsPerUnitScaleForElement(this, sprite);
				result = sprite.bounds.size * sprite.pixelsPerUnit * d;
			}
			return result;
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			bool flag = this.image == null && this.sprite == null && this.vectorImage == null;
			Vector2 result;
			if (flag)
			{
				result = new Vector2(num, num2);
			}
			else
			{
				Vector2 vector = Vector2.zero;
				bool flag2 = this.image != null;
				if (flag2)
				{
					vector = this.GetTextureDisplaySize(this.image);
				}
				else
				{
					bool flag3 = this.sprite != null;
					if (flag3)
					{
						vector = this.GetTextureDisplaySize(this.sprite);
					}
					else
					{
						vector = this.vectorImage.size;
					}
				}
				Rect sourceRect = this.sourceRect;
				bool flag4 = sourceRect != Rect.zero;
				num = (flag4 ? Mathf.Abs(sourceRect.width) : vector.x);
				num2 = (flag4 ? Mathf.Abs(sourceRect.height) : vector.y);
				bool flag5 = widthMode == VisualElement.MeasureMode.AtMost;
				if (flag5)
				{
					num = Mathf.Min(num, desiredWidth);
				}
				bool flag6 = heightMode == VisualElement.MeasureMode.AtMost;
				if (flag6)
				{
					num2 = Mathf.Min(num2, desiredHeight);
				}
				result = new Vector2(num, num2);
			}
			return result;
		}

		private void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			bool flag = this.image == null && this.sprite == null && this.vectorImage == null;
			if (!flag)
			{
				Rect rect = GUIUtility.AlignRectToDevice(base.contentRect);
				VisualElement visualElement = mgc.visualElement;
				Color playModeTintColor = (visualElement != null) ? visualElement.playModeTintColor : Color.white;
				MeshGenerator.RectangleParams rectParams = default(MeshGenerator.RectangleParams);
				bool flag2 = this.image != null;
				if (flag2)
				{
					rectParams = MeshGenerator.RectangleParams.MakeTextured(rect, this.uv, this.image, this.scaleMode, playModeTintColor);
				}
				else
				{
					bool flag3 = this.sprite != null;
					if (flag3)
					{
						Vector4 zero = Vector4.zero;
						rectParams = MeshGenerator.RectangleParams.MakeSprite(rect, this.uv, this.sprite, this.scaleMode, playModeTintColor, false, ref zero, false);
					}
					else
					{
						bool flag4 = this.vectorImage != null;
						if (flag4)
						{
							rectParams = MeshGenerator.RectangleParams.MakeVectorTextured(rect, this.uv, this.vectorImage, this.scaleMode, playModeTintColor);
						}
					}
				}
				rectParams.color = this.tintColor;
				mgc.meshGenerator.DrawRectangle(rectParams);
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			this.ReadCustomProperties(e.customStyle);
		}

		private void ReadCustomProperties(ICustomStyle customStyleProvider)
		{
			bool flag = !this.m_ImageIsInline;
			if (flag)
			{
				Texture2D src;
				bool flag2 = customStyleProvider.TryGetValue(Image.s_ImageProperty, out src);
				if (flag2)
				{
					this.SetProperty<Texture, Sprite, VectorImage>(src, ref this.m_Image, ref this.m_Sprite, ref this.m_VectorImage, Image.imageProperty);
				}
				else
				{
					Sprite src2;
					bool flag3 = customStyleProvider.TryGetValue(Image.s_SpriteProperty, out src2);
					if (flag3)
					{
						this.SetProperty<Sprite, Texture, VectorImage>(src2, ref this.m_Sprite, ref this.m_Image, ref this.m_VectorImage, Image.spriteProperty);
					}
					else
					{
						VectorImage src3;
						bool flag4 = customStyleProvider.TryGetValue(Image.s_VectorImageProperty, out src3);
						if (flag4)
						{
							this.SetProperty<VectorImage, Texture, Sprite>(src3, ref this.m_VectorImage, ref this.m_Image, ref this.m_Sprite, Image.vectorImageProperty);
						}
						else
						{
							this.ClearProperty();
						}
					}
				}
			}
			string value;
			bool flag5 = !this.m_ScaleModeIsInline && customStyleProvider.TryGetValue(Image.s_ScaleModeProperty, out value);
			if (flag5)
			{
				int scaleMode;
				StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.ScaleMode, value, out scaleMode);
				this.SetScaleMode((ScaleMode)scaleMode);
			}
			bool flag6 = !this.m_TintColorIsInline;
			if (flag6)
			{
				Color tintColor;
				bool flag7 = customStyleProvider.TryGetValue(Image.s_TintColorProperty, out tintColor);
				if (flag7)
				{
					this.SetTintColor(tintColor);
				}
				else
				{
					this.SetTintColor(Color.white);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetProperty<T0, T1, T2>(T0 src, ref T0 dst, ref T1 alt0, ref T2 alt1, BindingId binding) where T0 : Object where T1 : Object where T2 : Object
		{
			bool flag = src == dst;
			if (!flag)
			{
				dst = src;
				bool flag2 = dst != null;
				if (flag2)
				{
					alt0 = default(T1);
					alt1 = default(T2);
				}
				bool flag3 = dst == null;
				if (flag3)
				{
					this.uv = new Rect(0f, 0f, 1f, 1f);
					this.ReadCustomProperties(base.customStyle);
				}
				base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
				base.NotifyPropertyChanged(binding);
			}
		}

		private void ClearProperty()
		{
			bool imageIsInline = this.m_ImageIsInline;
			if (!imageIsInline)
			{
				this.image = null;
				this.sprite = null;
				this.vectorImage = null;
			}
		}

		private void SetScaleMode(ScaleMode mode)
		{
			bool flag = this.m_ScaleMode != mode;
			if (flag)
			{
				this.m_ScaleMode = mode;
				base.IncrementVersion(VersionChangeType.Repaint);
				base.NotifyPropertyChanged(Image.scaleModeProperty);
			}
		}

		private void SetTintColor(Color color)
		{
			bool flag = this.m_TintColor != color;
			if (flag)
			{
				this.m_TintColor = color;
				base.IncrementVersion(VersionChangeType.Repaint);
				base.NotifyPropertyChanged(Image.tintColorProperty);
			}
		}

		private void CalculateUV(Rect srcRect)
		{
			this.m_UV = new Rect(0f, 0f, 1f, 1f);
			Vector2 vector = Vector2.zero;
			Texture image = this.image;
			bool flag = image != null;
			if (flag)
			{
				vector = this.GetTextureDisplaySize(image);
			}
			VectorImage vectorImage = this.vectorImage;
			bool flag2 = vectorImage != null;
			if (flag2)
			{
				vector = vectorImage.size;
			}
			bool flag3 = vector != Vector2.zero;
			if (flag3)
			{
				this.m_UV.x = srcRect.x / vector.x;
				this.m_UV.width = srcRect.width / vector.x;
				this.m_UV.height = srcRect.height / vector.y;
				this.m_UV.y = 1f - this.m_UV.height - srcRect.y / vector.y;
			}
		}

		private Rect GetSourceRect()
		{
			Rect zero = Rect.zero;
			Vector2 vector = Vector2.zero;
			Texture image = this.image;
			bool flag = image != null;
			if (flag)
			{
				vector = this.GetTextureDisplaySize(image);
			}
			VectorImage vectorImage = this.vectorImage;
			bool flag2 = vectorImage != null;
			if (flag2)
			{
				vector = vectorImage.size;
			}
			bool flag3 = vector != Vector2.zero;
			if (flag3)
			{
				zero.x = this.uv.x * vector.x;
				zero.width = this.uv.width * vector.x;
				zero.y = (1f - this.uv.y - this.uv.height) * vector.y;
				zero.height = this.uv.height * vector.y;
			}
			return zero;
		}

		internal static readonly BindingId imageProperty = "image";

		internal static readonly BindingId spriteProperty = "sprite";

		internal static readonly BindingId vectorImageProperty = "vectorImage";

		internal static readonly BindingId sourceRectProperty = "sourceRect";

		internal static readonly BindingId uvProperty = "uv";

		internal static readonly BindingId scaleModeProperty = "scaleMode";

		internal static readonly BindingId tintColorProperty = "tintColor";

		private ScaleMode m_ScaleMode;

		private Texture m_Image;

		private Sprite m_Sprite;

		private VectorImage m_VectorImage;

		private Rect m_UV;

		private Color m_TintColor;

		internal bool m_ImageIsInline;

		private bool m_ScaleModeIsInline;

		private bool m_TintColorIsInline;

		public static readonly string ussClassName = "unity-image";

		private static CustomStyleProperty<Texture2D> s_ImageProperty = new CustomStyleProperty<Texture2D>("--unity-image");

		private static CustomStyleProperty<Sprite> s_SpriteProperty = new CustomStyleProperty<Sprite>("--unity-image");

		private static CustomStyleProperty<VectorImage> s_VectorImageProperty = new CustomStyleProperty<VectorImage>("--unity-image");

		private static CustomStyleProperty<string> s_ScaleModeProperty = new CustomStyleProperty<string>("--unity-image-size");

		private static CustomStyleProperty<Color> s_TintColorProperty = new CustomStyleProperty<Color>("--unity-image-tint-color");

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			public override object CreateInstance()
			{
				return new Image();
			}
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Image, Image.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}
		}
	}
}
