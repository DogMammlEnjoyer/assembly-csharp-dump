using System;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.IMGUIModule",
		"UnityEngine.UIElementsModule"
	})]
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextGenerationSettings.h")]
	[UsedByNativeCode("TextGenerationSettings")]
	internal struct NativeTextGenerationSettings
	{
		public bool hasLink
		{
			get
			{
				bool result;
				if (this.textSpans != null)
				{
					result = Array.Exists<TextSpan>(this.textSpans, (TextSpan span) => span.linkID != -1);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		public readonly TextSpan CreateTextSpan()
		{
			return new TextSpan
			{
				fontAsset = this.fontAsset,
				fontSize = this.fontSize,
				color = this.color,
				fontStyle = this.fontStyle,
				fontWeight = this.fontWeight,
				alignment = this.horizontalAlignment,
				linkID = -1
			};
		}

		public string GetTextSpanContent(int spanIndex)
		{
			bool flag = string.IsNullOrEmpty(this.text);
			if (flag)
			{
				throw new InvalidOperationException("The text property is null or empty.");
			}
			bool flag2 = this.textSpans == null || spanIndex < 0 || spanIndex >= this.textSpans.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("spanIndex", "Invalid span index.");
			}
			TextSpan textSpan = this.textSpans[spanIndex];
			bool flag3 = textSpan.startIndex < 0 || textSpan.startIndex >= this.text.Length || textSpan.startIndex + textSpan.length > this.text.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("spanIndex", "Invalid startIndex or length for the current text.");
			}
			return this.text.Substring(textSpan.startIndex, textSpan.length);
		}

		public static NativeTextGenerationSettings Default
		{
			get
			{
				return new NativeTextGenerationSettings
				{
					fontStyle = FontStyles.Normal,
					fontWeight = TextFontWeight.Regular,
					color = Color.black
				};
			}
		}

		internal NativeTextGenerationSettings(NativeTextGenerationSettings tgs)
		{
			this.text = tgs.text;
			this.fontSize = tgs.fontSize;
			this.bestFit = tgs.bestFit;
			this.maxFontSize = tgs.maxFontSize;
			this.minFontSize = tgs.minFontSize;
			this.screenWidth = tgs.screenWidth;
			this.screenHeight = tgs.screenHeight;
			this.wordWrap = tgs.wordWrap;
			this.horizontalAlignment = tgs.horizontalAlignment;
			this.verticalAlignment = tgs.verticalAlignment;
			this.color = tgs.color;
			this.fontAsset = tgs.fontAsset;
			this.textSettings = tgs.textSettings;
			this.fontStyle = tgs.fontStyle;
			this.fontWeight = tgs.fontWeight;
			this.languageDirection = tgs.languageDirection;
			this.vertexPadding = tgs.vertexPadding;
			this.overflow = tgs.overflow;
			this.textSpans = ((tgs.textSpans != null) ? ((TextSpan[])tgs.textSpans.Clone()) : null);
			this.characterSpacing = tgs.characterSpacing;
			this.wordSpacing = tgs.wordSpacing;
			this.paragraphSpacing = tgs.paragraphSpacing;
		}

		public override string ToString()
		{
			string text = "null";
			bool flag = this.textSpans != null;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				for (int i = 0; i < this.textSpans.Length; i++)
				{
					bool flag2 = i > 0;
					if (flag2)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(this.textSpans[i].ToString());
				}
				stringBuilder.Append("]");
				text = stringBuilder.ToString();
			}
			return string.Concat(new string[]
			{
				string.Format("{0}: {1}\n", "fontAsset", this.fontAsset),
				string.Format("{0}: {1}\n", "textSettings", this.textSettings),
				"text: ",
				this.text,
				"\n",
				string.Format("{0}: {1}\n", "screenWidth", this.screenWidth),
				string.Format("{0}: {1}\n", "screenHeight", this.screenHeight),
				string.Format("{0}: {1}\n", "fontSize", this.fontSize),
				string.Format("{0}: {1}\n", "bestFit", this.bestFit),
				string.Format("{0}: {1}\n", "maxFontSize", this.maxFontSize),
				string.Format("{0}: {1}\n", "minFontSize", this.minFontSize),
				string.Format("{0}: {1}\n", "wordWrap", this.wordWrap),
				string.Format("{0}: {1}\n", "languageDirection", this.languageDirection),
				string.Format("{0}: {1}\n", "horizontalAlignment", this.horizontalAlignment),
				string.Format("{0}: {1}\n", "verticalAlignment", this.verticalAlignment),
				string.Format("{0}: {1}\n", "color", this.color),
				string.Format("{0}: {1}\n", "fontStyle", this.fontStyle),
				string.Format("{0}: {1}\n", "fontWeight", this.fontWeight),
				string.Format("{0}: {1}\n", "vertexPadding", this.vertexPadding),
				string.Format("{0}: {1}\n", "overflow", this.overflow),
				"textSpans: ",
				text,
				"\n",
				string.Format("{0}: {1}\n", "characterSpacing", this.characterSpacing),
				string.Format("{0}: {1}\n", "paragraphSpacing", this.paragraphSpacing),
				string.Format("{0}: {1}\n", "wordSpacing", this.wordSpacing)
			});
		}

		public IntPtr fontAsset;

		public IntPtr textSettings;

		public string text;

		public int screenWidth;

		public int screenHeight;

		public WhiteSpace wordWrap;

		public TextOverflow overflow;

		public LanguageDirection languageDirection;

		public int vertexPadding;

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal HorizontalAlignment horizontalAlignment;

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal VerticalAlignment verticalAlignment;

		public int fontSize;

		public bool bestFit;

		public int maxFontSize;

		public int minFontSize;

		public FontStyles fontStyle;

		public TextFontWeight fontWeight;

		public TextSpan[] textSpans;

		public Color32 color;

		public int characterSpacing;

		public int wordSpacing;

		public int paragraphSpacing;
	}
}
