using System;

namespace UnityEngine.UIElements.UIR
{
	internal class DefaultElementBuilder : BaseElementBuilder
	{
		public DefaultElementBuilder(RenderTreeManager renderTreeManager)
		{
			this.m_RenderTreeManager = renderTreeManager;
		}

		public override bool RequiresStencilMask(VisualElement ve)
		{
			return UIRUtility.IsRoundRect(ve) || UIRUtility.IsVectorImageBackground(ve);
		}

		protected unsafe override void DrawVisualElementBackground(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderData renderData = mgc.renderData;
			bool flag = visualElement.layout.width <= 1E-30f || visualElement.layout.height <= 1E-30f;
			if (!flag)
			{
				ComputedStyle computedStyle = *visualElement.computedStyle;
				Color backgroundColor = computedStyle.backgroundColor;
				renderData.backgroundAlpha = backgroundColor.a;
				bool flag2 = backgroundColor.a > 1E-30f;
				if (flag2)
				{
					MeshGenerator.RectangleParams rectParams = new MeshGenerator.RectangleParams
					{
						rect = visualElement.rect,
						color = backgroundColor,
						colorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.backgroundColorID),
						playmodeTintColor = visualElement.playModeTintColor
					};
					MeshGenerator.GetVisualElementRadii(visualElement, out rectParams.topLeftRadius, out rectParams.bottomLeftRadius, out rectParams.topRightRadius, out rectParams.bottomRightRadius);
					MeshGenerator.AdjustBackgroundSizeForBorders(visualElement, ref rectParams);
					mgc.meshGenerator.DrawRectangle(rectParams);
				}
				Vector4 vector = new Vector4((float)computedStyle.unitySliceLeft, (float)computedStyle.unitySliceTop, (float)computedStyle.unitySliceRight, (float)computedStyle.unitySliceBottom);
				MeshGenerator.RectangleParams rectangleParams = default(MeshGenerator.RectangleParams);
				MeshGenerator.GetVisualElementRadii(visualElement, out rectangleParams.topLeftRadius, out rectangleParams.bottomLeftRadius, out rectangleParams.topRightRadius, out rectangleParams.bottomRightRadius);
				Background backgroundImage = computedStyle.backgroundImage;
				bool flag3 = backgroundImage.texture != null || backgroundImage.sprite != null || backgroundImage.vectorImage != null || backgroundImage.renderTexture != null;
				if (flag3)
				{
					MeshGenerator.RectangleParams rectangleParams2 = default(MeshGenerator.RectangleParams);
					float num = visualElement.resolvedStyle.unitySliceScale;
					Color playModeTintColor = visualElement.playModeTintColor;
					bool flag4;
					ScaleMode scaleMode = BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(computedStyle.backgroundPositionX, computedStyle.backgroundPositionY, computedStyle.backgroundRepeat, computedStyle.backgroundSize, out flag4);
					bool flag5 = backgroundImage.texture != null;
					if (flag5)
					{
						bool flag6 = Mathf.RoundToInt(vector.x) != 0 || Mathf.RoundToInt(vector.y) != 0 || Mathf.RoundToInt(vector.z) != 0 || Mathf.RoundToInt(vector.w) != 0;
						rectangleParams2 = MeshGenerator.RectangleParams.MakeTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.texture, flag6 ? (flag4 ? scaleMode : ScaleMode.StretchToFill) : ScaleMode.ScaleToFit, playModeTintColor);
						rectangleParams2.rect = new Rect(0f, 0f, (float)rectangleParams2.texture.width, (float)rectangleParams2.texture.height);
					}
					else
					{
						bool flag7 = backgroundImage.sprite != null;
						if (flag7)
						{
							bool flag8 = !flag4 || scaleMode == ScaleMode.ScaleAndCrop;
							rectangleParams2 = MeshGenerator.RectangleParams.MakeSprite(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.sprite, flag8 ? ScaleMode.StretchToFill : scaleMode, playModeTintColor, rectangleParams.HasRadius(0.001f), ref vector, flag8);
							bool flag9 = rectangleParams2.texture != null;
							if (flag9)
							{
								rectangleParams2.rect = new Rect(0f, 0f, backgroundImage.sprite.rect.width, backgroundImage.sprite.rect.height);
							}
							num *= UIElementsUtility.PixelsPerUnitScaleForElement(visualElement, backgroundImage.sprite);
						}
						else
						{
							bool flag10 = backgroundImage.renderTexture != null;
							if (flag10)
							{
								rectangleParams2 = MeshGenerator.RectangleParams.MakeTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.renderTexture, ScaleMode.ScaleToFit, playModeTintColor);
								rectangleParams2.rect = new Rect(0f, 0f, (float)rectangleParams2.texture.width, (float)rectangleParams2.texture.height);
							}
							else
							{
								bool flag11 = backgroundImage.vectorImage != null;
								if (flag11)
								{
									bool flag12 = !flag4 || scaleMode == ScaleMode.ScaleAndCrop;
									rectangleParams2 = MeshGenerator.RectangleParams.MakeVectorTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.vectorImage, flag12 ? ScaleMode.StretchToFill : scaleMode, playModeTintColor);
									rectangleParams2.rect = new Rect(0f, 0f, rectangleParams2.vectorImage.size.x, rectangleParams2.vectorImage.size.y);
								}
							}
						}
					}
					rectangleParams2.topLeftRadius = rectangleParams.topLeftRadius;
					rectangleParams2.topRightRadius = rectangleParams.topRightRadius;
					rectangleParams2.bottomRightRadius = rectangleParams.bottomRightRadius;
					rectangleParams2.bottomLeftRadius = rectangleParams.bottomLeftRadius;
					bool flag13 = vector != Vector4.zero;
					if (flag13)
					{
						rectangleParams2.leftSlice = Mathf.RoundToInt(vector.x);
						rectangleParams2.topSlice = Mathf.RoundToInt(vector.y);
						rectangleParams2.rightSlice = Mathf.RoundToInt(vector.z);
						rectangleParams2.bottomSlice = Mathf.RoundToInt(vector.w);
						rectangleParams2.sliceScale = num;
						bool flag14 = computedStyle.unitySliceType == SliceType.Tiled;
						if (flag14)
						{
							rectangleParams2.meshFlags |= MeshGenerationContext.MeshFlags.SliceTiled;
						}
						bool flag15 = !flag4;
						if (flag15)
						{
							rectangleParams2.backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
							rectangleParams2.backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
							rectangleParams2.backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.StretchToFill);
							rectangleParams2.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.StretchToFill);
						}
						else
						{
							rectangleParams2.backgroundPositionX = computedStyle.backgroundPositionX;
							rectangleParams2.backgroundPositionY = computedStyle.backgroundPositionY;
							rectangleParams2.backgroundRepeat = computedStyle.backgroundRepeat;
							rectangleParams2.backgroundSize = computedStyle.backgroundSize;
						}
					}
					else
					{
						rectangleParams2.backgroundPositionX = computedStyle.backgroundPositionX;
						rectangleParams2.backgroundPositionY = computedStyle.backgroundPositionY;
						rectangleParams2.backgroundRepeat = computedStyle.backgroundRepeat;
						rectangleParams2.backgroundSize = computedStyle.backgroundSize;
					}
					rectangleParams2.color = computedStyle.unityBackgroundImageTintColor;
					rectangleParams2.colorPage = ColorPage.Init(this.m_RenderTreeManager, visualElement.renderData.tintColorID);
					MeshGenerator.AdjustBackgroundSizeForBorders(visualElement, ref rectangleParams2);
					bool flag16 = rectangleParams2.texture != null || rectangleParams2.vectorImage != null;
					if (flag16)
					{
						mgc.meshGenerator.DrawRectangleRepeat(rectangleParams2, visualElement.rect, visualElement.scaledPixelsPerPoint);
					}
					else
					{
						mgc.meshGenerator.DrawRectangle(rectangleParams2);
					}
				}
			}
		}

		protected override void DrawVisualElementBorder(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderData renderData = mgc.renderData;
			bool flag = visualElement.layout.width >= 1E-30f && visualElement.layout.height >= 1E-30f;
			if (flag)
			{
				IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
				bool flag2 = (resolvedStyle.borderLeftColor != Color.clear && resolvedStyle.borderLeftWidth > 0f) || (resolvedStyle.borderTopColor != Color.clear && resolvedStyle.borderTopWidth > 0f) || (resolvedStyle.borderRightColor != Color.clear && resolvedStyle.borderRightWidth > 0f) || (resolvedStyle.borderBottomColor != Color.clear && resolvedStyle.borderBottomWidth > 0f);
				if (flag2)
				{
					MeshGenerator.BorderParams borderParams = new MeshGenerator.BorderParams
					{
						rect = visualElement.rect,
						leftColor = resolvedStyle.borderLeftColor,
						topColor = resolvedStyle.borderTopColor,
						rightColor = resolvedStyle.borderRightColor,
						bottomColor = resolvedStyle.borderBottomColor,
						leftWidth = resolvedStyle.borderLeftWidth,
						topWidth = resolvedStyle.borderTopWidth,
						rightWidth = resolvedStyle.borderRightWidth,
						bottomWidth = resolvedStyle.borderBottomWidth,
						leftColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderLeftColorID),
						topColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderTopColorID),
						rightColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderRightColorID),
						bottomColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderBottomColorID),
						playmodeTintColor = visualElement.playModeTintColor
					};
					MeshGenerator.GetVisualElementRadii(visualElement, out borderParams.topLeftRadius, out borderParams.bottomLeftRadius, out borderParams.topRightRadius, out borderParams.bottomRightRadius);
					mgc.meshGenerator.DrawBorder(borderParams);
				}
			}
		}

		protected override void DrawVisualElementStencilMask(MeshGenerationContext mgc)
		{
			bool flag = UIRUtility.IsVectorImageBackground(mgc.visualElement);
			if (flag)
			{
				this.DrawVisualElementBackground(mgc);
			}
			else
			{
				DefaultElementBuilder.GenerateStencilClipEntryForRoundedRectBackground(mgc);
			}
		}

		private static void GenerateStencilClipEntryForRoundedRectBackground(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			bool flag = visualElement.layout.width <= 1E-30f || visualElement.layout.height <= 1E-30f;
			if (!flag)
			{
				IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
				Vector2 a;
				Vector2 a2;
				Vector2 a3;
				Vector2 a4;
				MeshGenerator.GetVisualElementRadii(visualElement, out a, out a2, out a3, out a4);
				float borderTopWidth = resolvedStyle.borderTopWidth;
				float borderLeftWidth = resolvedStyle.borderLeftWidth;
				float borderBottomWidth = resolvedStyle.borderBottomWidth;
				float borderRightWidth = resolvedStyle.borderRightWidth;
				MeshGenerator.RectangleParams rectParams = new MeshGenerator.RectangleParams
				{
					rect = visualElement.rect,
					color = Color.white,
					topLeftRadius = Vector2.Max(Vector2.zero, a - new Vector2(borderLeftWidth, borderTopWidth)),
					topRightRadius = Vector2.Max(Vector2.zero, a3 - new Vector2(borderRightWidth, borderTopWidth)),
					bottomLeftRadius = Vector2.Max(Vector2.zero, a2 - new Vector2(borderLeftWidth, borderBottomWidth)),
					bottomRightRadius = Vector2.Max(Vector2.zero, a4 - new Vector2(borderRightWidth, borderBottomWidth)),
					playmodeTintColor = visualElement.playModeTintColor
				};
				rectParams.rect.x = rectParams.rect.x + borderLeftWidth;
				rectParams.rect.y = rectParams.rect.y + borderTopWidth;
				rectParams.rect.width = rectParams.rect.width - (borderLeftWidth + borderRightWidth);
				rectParams.rect.height = rectParams.rect.height - (borderTopWidth + borderBottomWidth);
				bool flag2 = visualElement.computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox;
				if (flag2)
				{
					rectParams.rect.x = rectParams.rect.x + resolvedStyle.paddingLeft;
					rectParams.rect.y = rectParams.rect.y + resolvedStyle.paddingTop;
					rectParams.rect.width = rectParams.rect.width - (resolvedStyle.paddingLeft + resolvedStyle.paddingRight);
					rectParams.rect.height = rectParams.rect.height - (resolvedStyle.paddingTop + resolvedStyle.paddingBottom);
				}
				mgc.meshGenerator.DrawRectangle(rectParams);
			}
		}

		public override void ScheduleMeshGenerationJobs(MeshGenerationContext mgc)
		{
			mgc.meshGenerator.ScheduleJobs(mgc);
			bool hasPainter2D = mgc.hasPainter2D;
			if (hasPainter2D)
			{
				mgc.painter2D.ScheduleJobs(mgc);
			}
		}

		private RenderTreeManager m_RenderTreeManager;
	}
}
