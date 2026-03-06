using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	internal class UITKTextHandle : TextHandle
	{
		private List<ValueTuple<int, RichTextTagParser.TagType, string>> Links
		{
			get
			{
				List<ValueTuple<int, RichTextTagParser.TagType, string>> result;
				if ((result = this.m_Links) == null)
				{
					result = (this.m_Links = new List<ValueTuple<int, RichTextTagParser.TagType, string>>());
				}
				return result;
			}
		}

		private void ComputeNativeTextSize(in RenderedText textToMeasure, float width, float height, float? fontsize = null)
		{
			bool flag = !this.ConvertUssToNativeTextGenerationSettings(fontsize);
			if (!flag)
			{
				this.nativeSettings.text = ((textToMeasure.valueLength > 0) ? textToMeasure.CreateString() : "​");
				this.nativeSettings.screenWidth = (float.IsNaN(width) ? -1 : ((int)(width * 64f)));
				this.nativeSettings.screenHeight = (float.IsNaN(height) ? -1 : ((int)(height * 64f)));
				bool flag2 = this.m_TextElement.enableRichText && !string.IsNullOrEmpty(this.nativeSettings.text);
				if (flag2)
				{
					RichTextTagParser.CreateTextGenerationSettingsArray(ref this.nativeSettings, this.Links, this.atgHyperlinkColor);
				}
				else
				{
					this.nativeSettings.textSpans = null;
				}
				this.pixelPreferedSize = this.textLib.MeasureText(this.nativeSettings, IntPtr.Zero);
			}
		}

		public ValueTuple<NativeTextInfo, bool> UpdateNative(bool generateNativeSettings = true)
		{
			bool flag = generateNativeSettings && !this.ConvertUssToNativeTextGenerationSettings(null);
			ValueTuple<NativeTextInfo, bool> result;
			if (flag)
			{
				result = new ValueTuple<NativeTextInfo, bool>(default(NativeTextInfo), false);
			}
			else
			{
				bool flag2 = this.m_TextElement.enableRichText && !string.IsNullOrEmpty(this.nativeSettings.text);
				if (flag2)
				{
					RichTextTagParser.CreateTextGenerationSettingsArray(ref this.nativeSettings, this.Links, this.atgHyperlinkColor);
				}
				else
				{
					this.nativeSettings.textSpans = null;
				}
				bool flag3 = this.nativeSettings.hasLink && this.textGenerationInfo == IntPtr.Zero;
				if (flag3)
				{
					this.textGenerationInfo = TextGenerationInfo.Create();
					if (this.m_ATGTextEventHandler == null)
					{
						this.m_ATGTextEventHandler = new ATGTextEventHandler(this.m_TextElement);
					}
				}
				NativeTextInfo nativeTextInfo = this.textLib.GenerateText(this.nativeSettings, this.textGenerationInfo);
				this.m_IsElided = nativeTextInfo.isElided;
				result = new ValueTuple<NativeTextInfo, bool>(nativeTextInfo, true);
			}
			return result;
		}

		public void CacheTextGenerationInfo()
		{
			bool flag = !base.useAdvancedText;
			if (flag)
			{
				Debug.LogError("CacheTextGenerationInfo should only be called for ATG.");
			}
			else
			{
				bool flag2 = this.textGenerationInfo == IntPtr.Zero;
				if (flag2)
				{
					this.textGenerationInfo = TextGenerationInfo.Create();
				}
			}
		}

		public void ProcessMeshInfos(NativeTextInfo textInfo)
		{
			this.textLib.ProcessMeshInfos(textInfo, this.nativeSettings);
		}

		private ValueTuple<bool, bool> hasLinkAndHyperlink()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = this.m_Links != null;
			if (flag3)
			{
				foreach (ValueTuple<int, RichTextTagParser.TagType, string> valueTuple in this.Links)
				{
					RichTextTagParser.TagType item = valueTuple.Item2;
					flag = (flag || item == RichTextTagParser.TagType.Link);
					flag2 = (flag2 || item == RichTextTagParser.TagType.Hyperlink);
					bool flag4 = flag && flag2;
					if (flag4)
					{
						break;
					}
				}
			}
			return new ValueTuple<bool, bool>(flag, flag2);
		}

		internal ValueTuple<RichTextTagParser.TagType, string> ATGFindIntersectingLink(Vector2 point)
		{
			Debug.Assert(base.useAdvancedText);
			bool flag = this.textGenerationInfo == IntPtr.Zero;
			ValueTuple<RichTextTagParser.TagType, string> result;
			if (flag)
			{
				Debug.LogError("TextGenerationInfo pointer is null.");
				result = new ValueTuple<RichTextTagParser.TagType, string>(RichTextTagParser.TagType.Unknown, null);
			}
			else
			{
				int num = TextLib.FindIntersectingLink(point * this.GetPixelsPerPoint(), this.textGenerationInfo);
				bool flag2 = num == -1;
				if (flag2)
				{
					result = new ValueTuple<RichTextTagParser.TagType, string>(RichTextTagParser.TagType.Unknown, null);
				}
				else
				{
					result = new ValueTuple<RichTextTagParser.TagType, string>(this.m_Links[num].Item2, this.m_Links[num].Item3);
				}
			}
			return result;
		}

		internal void UpdateATGTextEventHandler()
		{
			bool flag = this.m_ATGTextEventHandler == null;
			if (!flag)
			{
				ValueTuple<bool, bool> valueTuple = this.hasLinkAndHyperlink();
				bool item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				bool flag2 = item;
				if (flag2)
				{
					this.m_ATGTextEventHandler.RegisterLinkTagCallbacks();
				}
				else
				{
					this.m_ATGTextEventHandler.UnRegisterLinkTagCallbacks();
				}
				bool flag3 = item2;
				if (flag3)
				{
					this.m_ATGTextEventHandler.RegisterHyperlinkCallbacks();
				}
				else
				{
					this.m_ATGTextEventHandler.UnRegisterHyperlinkCallbacks();
				}
			}
		}

		internal unsafe bool ConvertUssToNativeTextGenerationSettings(float? fontsize = null)
		{
			FontAsset fontAsset = TextUtilities.GetFontAsset(this.m_TextElement);
			bool flag = fontAsset.atlasPopulationMode == AtlasPopulationMode.Static;
			bool result;
			if (flag)
			{
				Debug.LogError("Advanced text system cannot render using static font asset " + fontAsset.faceInfo.familyName);
				result = false;
			}
			else
			{
				float pixelsPerPoint = this.GetPixelsPerPoint();
				ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
				this.nativeSettings.textSettings = TextUtilities.GetTextSettingsFrom(this.m_TextElement).nativeTextSettings;
				this.nativeSettings.text = ((this.m_TextElement.isElided && !this.TextLibraryCanElide()) ? new RenderedText(this.m_TextElement.elidedText) : this.m_TextElement.renderedText).CreateString();
				float num = fontsize ?? computedStyle.fontSize.value;
				this.nativeSettings.fontSize = (int)(num * 64f * pixelsPerPoint);
				this.nativeSettings.bestFit = (computedStyle.unityTextAutoSize.mode == TextAutoSizeMode.BestFit);
				this.nativeSettings.maxFontSize = (int)(computedStyle.unityTextAutoSize.maxSize.value * 64f * pixelsPerPoint);
				this.nativeSettings.minFontSize = (int)(computedStyle.unityTextAutoSize.minSize.value * 64f * pixelsPerPoint);
				this.nativeSettings.wordWrap = computedStyle.whiteSpace.toTextCore(this.m_TextElement.isInputField);
				this.nativeSettings.overflow = computedStyle.textOverflow.toTextCore(computedStyle.overflow);
				this.nativeSettings.horizontalAlignment = TextGeneratorUtilities.GetHorizontalAlignment(computedStyle.unityTextAlign);
				this.nativeSettings.verticalAlignment = TextGeneratorUtilities.GetVerticalAlignment(computedStyle.unityTextAlign);
				this.nativeSettings.characterSpacing = (int)(computedStyle.letterSpacing.value * 64f);
				this.nativeSettings.wordSpacing = (int)(computedStyle.wordSpacing.value * 64f);
				this.nativeSettings.paragraphSpacing = (int)(computedStyle.unityParagraphSpacing.value * 64f);
				this.nativeSettings.color = computedStyle.color;
				this.nativeSettings.fontAsset = fontAsset.nativeFontAsset;
				this.nativeSettings.languageDirection = this.m_TextElement.localLanguageDirection.toTextCore();
				this.nativeSettings.vertexPadding = (int)(this.GetVertexPadding(fontAsset) * 64f);
				FontStyles fontStyles = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
				this.nativeSettings.fontStyle = (fontStyles & ~FontStyles.Bold);
				this.nativeSettings.fontWeight = (((fontStyles & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : TextFontWeight.Regular);
				Vector2 vector = this.m_TextElement.contentRect.size;
				bool flag2 = Mathf.Abs(vector.x - this.ATGRoundedSizes.x) < 0.01f && Mathf.Abs(vector.y - this.ATGRoundedSizes.y) < 0.01f;
				if (flag2)
				{
					vector = this.ATGMeasuredSizes;
				}
				else
				{
					this.ATGRoundedSizes = vector;
					this.ATGMeasuredSizes = vector;
				}
				this.nativeSettings.screenWidth = (int)(vector.x * 64f * pixelsPerPoint);
				this.nativeSettings.screenHeight = (int)(vector.y * 64f * pixelsPerPoint);
				result = true;
			}
			return result;
		}

		private TextAsset GetICUAsset()
		{
			bool flag = this.m_TextElement.panel == null;
			if (flag)
			{
				throw new InvalidOperationException("Text cannot be processed on elements not in a panel");
			}
			TextAsset textAsset = ((PanelSettings)((RuntimePanel)this.m_TextElement.panel).ownerObject).m_ICUDataAsset;
			bool flag2 = textAsset != null;
			TextAsset result;
			if (flag2)
			{
				result = textAsset;
			}
			else
			{
				textAsset = UITKTextHandle.GetICUAssetStaticFalback();
				bool flag3 = textAsset != null;
				if (flag3)
				{
					result = textAsset;
				}
				else
				{
					Debug.LogError("ICU Data not available. The data should be automatically assigned to the PanelSettings in the editor if the advanced text option is enable in the project settings. It will not be present on PanelSettings created at runtime, so make sure the build contains at least one PanelSettings asset");
					result = null;
				}
			}
			return result;
		}

		internal static TextAsset GetICUAssetStaticFalback()
		{
			foreach (TextAsset textAsset in Resources.FindObjectsOfTypeAll<TextAsset>())
			{
				bool flag = textAsset.name == "icudt73l";
				if (flag)
				{
					return textAsset;
				}
			}
			return null;
		}

		protected internal TextLib textLib
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				this.InitTextLib();
				return UITKTextHandle.s_TextLib;
			}
		}

		protected internal void InitTextLib()
		{
			if (UITKTextHandle.s_TextLib == null)
			{
				UITKTextHandle.s_TextLib = new TextLib(this.GetICUAsset().bytes);
			}
		}

		public UITKTextHandle(TextElement te)
		{
			this.m_TextElement = te;
			this.m_TextEventHandler = new TextEventHandler(te);
		}

		protected override float GetPixelsPerPoint()
		{
			TextElement textElement = this.m_TextElement;
			return (textElement != null) ? textElement.scaledPixelsPerPoint : 1f;
		}

		internal float LastPixelPerPoint { get; set; }

		public override void SetDirty()
		{
			this.MeasuredWidth = null;
			base.SetDirty();
		}

		internal float? MeasuredWidth { get; set; }

		internal float RoundedWidth { get; set; }

		internal Vector2 ATGMeasuredSizes { get; set; }

		internal Vector2 ATGRoundedSizes { get; set; }

		public Vector2 ComputeTextSize(in RenderedText textToMeasure, float width, float height, float? fontsize = null)
		{
			float pixelsPerPoint = this.GetPixelsPerPoint();
			width = Mathf.Floor(width * pixelsPerPoint);
			height = Mathf.Floor(height * pixelsPerPoint);
			bool flag = TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
			if (flag)
			{
				this.ComputeNativeTextSize(textToMeasure, width, height, fontsize);
			}
			else
			{
				this.ConvertUssToTextGenerationSettings(false, fontsize);
				TextHandle.settings.renderedText = textToMeasure;
				TextHandle.settings.screenRect = new Rect(0f, 0f, width, height);
				base.UpdatePreferredValues(TextHandle.settings);
			}
			return base.preferredSize;
		}

		public void ComputeSettingsAndUpdate()
		{
			bool useAdvancedText = base.useAdvancedText;
			if (useAdvancedText)
			{
				this.UpdateNative(true);
				this.UpdateATGTextEventHandler();
			}
			else
			{
				this.UpdateMesh();
				this.HandleATag();
				this.HandleLinkTag();
				this.HandleLinkAndATagCallbacks();
			}
		}

		public void HandleATag()
		{
			TextEventHandler textEventHandler = this.m_TextEventHandler;
			if (textEventHandler != null)
			{
				textEventHandler.HandleATag();
			}
		}

		public void HandleLinkTag()
		{
			TextEventHandler textEventHandler = this.m_TextEventHandler;
			if (textEventHandler != null)
			{
				textEventHandler.HandleLinkTag();
			}
		}

		public void HandleLinkAndATagCallbacks()
		{
			TextEventHandler textEventHandler = this.m_TextEventHandler;
			if (textEventHandler != null)
			{
				textEventHandler.HandleLinkAndATagCallbacks();
			}
		}

		public void UpdateMesh()
		{
			this.ConvertUssToTextGenerationSettings(true, null);
			int hashCode = TextHandle.settings.GetHashCode();
			bool flag = this.m_PreviousGenerationSettingsHash == hashCode && !this.isDirty;
			if (flag)
			{
				base.AddTextInfoToTemporaryCache(hashCode);
			}
			else
			{
				base.RemoveTextInfoFromTemporaryCache();
				base.UpdateWithHash(hashCode);
			}
		}

		public override void AddToPermanentCacheAndGenerateMesh()
		{
			bool useAdvancedText = base.useAdvancedText;
			if (useAdvancedText)
			{
				this.CacheTextGenerationInfo();
				this.UpdateNative(true);
				this.UpdateATGTextEventHandler();
			}
			else
			{
				bool flag = this.ConvertUssToTextGenerationSettings(true, null);
				if (flag)
				{
					base.AddToPermanentCacheAndGenerateMesh();
				}
			}
		}

		private unsafe TextOverflowMode GetTextOverflowMode()
		{
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			bool flag = computedStyle.textOverflow == TextOverflow.Clip;
			TextOverflowMode result;
			if (flag)
			{
				result = TextOverflowMode.Masking;
			}
			else
			{
				bool flag2 = computedStyle.textOverflow != TextOverflow.Ellipsis;
				if (flag2)
				{
					result = TextOverflowMode.Overflow;
				}
				else
				{
					bool flag3 = !this.TextLibraryCanElide();
					if (flag3)
					{
						result = TextOverflowMode.Masking;
					}
					else
					{
						bool flag4 = computedStyle.overflow == OverflowInternal.Hidden;
						if (flag4)
						{
							result = TextOverflowMode.Ellipsis;
						}
						else
						{
							result = TextOverflowMode.Overflow;
						}
					}
				}
			}
			return result;
		}

		internal unsafe virtual bool ConvertUssToTextGenerationSettings(bool populateScreenRect, float? fontsize = null)
		{
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			TextGenerationSettings settings = TextHandle.settings;
			bool flag = computedStyle.unityTextAutoSize != TextAutoSize.None();
			if (flag)
			{
				Debug.LogWarning("TextAutoSize is not supported with the Standard TextGenerator. Please use Advanced Text Generation instead.");
			}
			settings.text = string.Empty;
			settings.isIMGUI = false;
			settings.textSettings = TextUtilities.GetTextSettingsFrom(this.m_TextElement);
			bool flag2 = settings.textSettings == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				settings.fontAsset = TextUtilities.GetFontAsset(this.m_TextElement);
				bool flag3 = settings.fontAsset == null;
				if (flag3)
				{
					result = false;
				}
				else
				{
					settings.extraPadding = this.GetVertexPadding(settings.fontAsset);
					settings.renderedText = ((this.m_TextElement.isElided && !this.TextLibraryCanElide()) ? new RenderedText(this.m_TextElement.elidedText) : this.m_TextElement.renderedText);
					settings.isPlaceholder = this.m_TextElement.showPlaceholderText;
					float pixelsPerPoint = this.GetPixelsPerPoint();
					float num = fontsize ?? computedStyle.fontSize.value;
					settings.fontSize = (int)Math.Round((double)(num * pixelsPerPoint), MidpointRounding.AwayFromZero);
					settings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
					settings.textAlignment = TextGeneratorUtilities.LegacyAlignmentToNewAlignment(computedStyle.unityTextAlign);
					settings.textWrappingMode = computedStyle.whiteSpace.toTextWrappingMode(this.m_TextElement.isInputField && !this.m_TextElement.edition.multiline);
					settings.richText = this.m_TextElement.enableRichText;
					settings.overflowMode = this.GetTextOverflowMode();
					settings.characterSpacing = computedStyle.letterSpacing.value;
					settings.wordSpacing = computedStyle.wordSpacing.value;
					settings.paragraphSpacing = computedStyle.unityParagraphSpacing.value;
					settings.color = computedStyle.color;
					settings.color *= this.m_TextElement.playModeTintColor;
					settings.shouldConvertToLinearSpace = false;
					settings.parseControlCharacters = this.m_TextElement.parseEscapeSequences;
					settings.isRightToLeft = (this.m_TextElement.localLanguageDirection == LanguageDirection.RTL);
					settings.emojiFallbackSupport = this.m_TextElement.emojiFallbackSupport;
					TextHandle.settings.pixelsPerPoint = pixelsPerPoint;
					if (populateScreenRect)
					{
						Vector2 size = this.m_TextElement.contentRect.size;
						bool flag4 = this.MeasuredWidth != null && Mathf.Abs(size.x - this.RoundedWidth) < 0.01f && this.LastPixelPerPoint == pixelsPerPoint;
						if (flag4)
						{
							size.x = this.MeasuredWidth.Value;
						}
						else
						{
							this.RoundedWidth = size.x;
							this.MeasuredWidth = null;
							this.LastPixelPerPoint = pixelsPerPoint;
						}
						size.x *= pixelsPerPoint;
						size.y *= pixelsPerPoint;
						bool flag5 = settings.fontAsset.IsBitmap();
						if (flag5)
						{
							size.x = Mathf.Round(size.x);
							size.y = Mathf.Round(size.y);
						}
						settings.screenRect = new Rect(Vector2.zero, size);
					}
					result = true;
				}
			}
			return result;
		}

		internal bool TextLibraryCanElide()
		{
			return this.m_TextElement.computedStyle.unityTextOverflowPosition == TextOverflowPosition.End;
		}

		internal unsafe float GetVertexPadding(FontAsset fontAsset)
		{
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			float num = computedStyle.unityTextOutlineWidth / 2f;
			float num2 = Mathf.Abs(computedStyle.textShadow.offset.x);
			float num3 = Mathf.Abs(computedStyle.textShadow.offset.y);
			float num4 = Mathf.Abs(computedStyle.textShadow.blurRadius);
			bool flag = num <= 0f && num2 <= 0f && num3 <= 0f && num4 <= 0f;
			float result;
			if (flag)
			{
				result = UITKTextHandle.k_MinPadding;
			}
			else
			{
				float a = Mathf.Max(num2 + num4, num);
				float b = Mathf.Max(num3 + num4, num);
				float num5 = Mathf.Max(a, b) + UITKTextHandle.k_MinPadding;
				float num6 = TextHandle.ConvertPixelUnitsToTextCoreRelativeUnits(computedStyle.fontSize.value, fontAsset);
				int num7 = fontAsset.atlasPadding + 1;
				result = Mathf.Min(num5 * num6 * (float)num7, (float)num7);
			}
			return result;
		}

		internal override bool IsAdvancedTextEnabledForElement()
		{
			return TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
		}

		internal void ReleaseResourcesIfPossible()
		{
			bool flag = TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
			bool flag2 = this.wasAdvancedTextEnabledForElement && !flag && this.textGenerationInfo != IntPtr.Zero;
			if (flag2)
			{
				TextGenerationInfo.Destroy(this.textGenerationInfo);
				this.textGenerationInfo = IntPtr.Zero;
				ATGTextEventHandler atgtextEventHandler = this.m_ATGTextEventHandler;
				if (atgtextEventHandler != null)
				{
					atgtextEventHandler.OnDestroy();
				}
				this.m_ATGTextEventHandler = null;
				this.m_TextEventHandler = new TextEventHandler(this.m_TextElement);
			}
			else
			{
				bool flag3 = !this.wasAdvancedTextEnabledForElement && flag;
				if (flag3)
				{
					TextHandle.s_PermanentCache.RemoveTextInfoFromCache(this);
					TextHandle.s_TemporaryCache.RemoveTextInfoFromCache(this);
					TextEventHandler textEventHandler = this.m_TextEventHandler;
					if (textEventHandler != null)
					{
						textEventHandler.OnDestroy();
					}
					this.m_TextEventHandler = null;
					this.m_ATGTextEventHandler = new ATGTextEventHandler(this.m_TextElement);
				}
			}
			this.wasAdvancedTextEnabledForElement = flag;
		}

		public override bool IsPlaceholder
		{
			get
			{
				return base.useAdvancedText ? this.m_TextElement.showPlaceholderText : base.IsPlaceholder;
			}
		}

		public bool IsElided()
		{
			bool flag = string.IsNullOrEmpty(this.m_TextElement.text);
			return flag || this.m_IsElided;
		}

		internal ATGTextEventHandler m_ATGTextEventHandler;

		private List<ValueTuple<int, RichTextTagParser.TagType, string>> m_Links;

		internal Color atgHyperlinkColor = Color.blue;

		private static TextLib s_TextLib;

		internal TextEventHandler m_TextEventHandler;

		protected TextElement m_TextElement;

		internal static readonly float k_MinPadding = 6f;

		private bool wasAdvancedTextEnabledForElement;
	}
}
