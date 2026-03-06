using System;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Localization.SmartFormat.Core.Settings;
using UnityEngine.Pool;

namespace UnityEngine.Localization.SmartFormat
{
	internal static class FormatItemPool
	{
		public static LiteralText GetLiteralText(SmartSettings smartSettings, FormatItem parent, int startIndex)
		{
			LiteralText literalText = FormatItemPool.s_LiteralTextPool.Get();
			literalText.Init(smartSettings, parent, startIndex);
			return literalText;
		}

		public static LiteralText GetLiteralText(SmartSettings smartSettings, FormatItem parent, int startIndex, int endIndex)
		{
			LiteralText literalText = FormatItemPool.s_LiteralTextPool.Get();
			literalText.Init(smartSettings, parent, startIndex, endIndex);
			return literalText;
		}

		public static LiteralText GetLiteralText(SmartSettings smartSettings, FormatItem parent, string baseString, int startIndex, int endIndex)
		{
			LiteralText literalText = FormatItemPool.s_LiteralTextPool.Get();
			literalText.Init(smartSettings, parent, baseString, startIndex, endIndex);
			return literalText;
		}

		public static Format GetFormat(SmartSettings smartSettings, string baseString)
		{
			Format format = FormatItemPool.s_FormatPool.Get();
			format.Init(smartSettings, null, baseString, 0, baseString.Length);
			return format;
		}

		public static Format GetFormat(SmartSettings smartSettings, string baseString, int startIndex, int endIndex)
		{
			Format format = FormatItemPool.s_FormatPool.Get();
			format.Init(smartSettings, null, baseString, startIndex, endIndex);
			return format;
		}

		public static Format GetFormat(SmartSettings smartSettings, string baseString, int startIndex, int endIndex, bool nested)
		{
			Format format = FormatItemPool.s_FormatPool.Get();
			format.Init(smartSettings, null, baseString, startIndex, endIndex);
			format.HasNested = nested;
			return format;
		}

		public static Format GetFormat(SmartSettings smartSettings, Placeholder parent, int startIndex)
		{
			Format format = FormatItemPool.s_FormatPool.Get();
			format.Init(smartSettings, parent, startIndex);
			format.parent = parent;
			return format;
		}

		public static Placeholder GetPlaceholder(SmartSettings smartSettings, Format parent, int startIndex, int nestedDepth)
		{
			Placeholder placeholder = FormatItemPool.s_PlaceholderPool.Get();
			placeholder.Init(smartSettings, parent, startIndex);
			placeholder.NestedDepth = nestedDepth;
			placeholder.FormatterName = "";
			placeholder.FormatterOptions = "";
			return placeholder;
		}

		public static Placeholder GetPlaceholder(SmartSettings smartSettings, Format parent, int startIndex, int nestedDepth, Format itemFormat, int endIndex)
		{
			Placeholder placeholder = FormatItemPool.s_PlaceholderPool.Get();
			placeholder.Init(smartSettings, parent, startIndex, endIndex);
			placeholder.Format = itemFormat;
			placeholder.NestedDepth = nestedDepth;
			placeholder.FormatterName = "";
			placeholder.FormatterOptions = "";
			return placeholder;
		}

		public static Selector GetSelector(SmartSettings smartSettings, FormatItem parent, string baseString, int startIndex, int endIndex, int operatorStart, int selectorIndex)
		{
			Selector selector = FormatItemPool.s_SelectorPool.Get();
			selector.Init(smartSettings, parent, baseString, startIndex, endIndex);
			selector.operatorStart = operatorStart;
			selector.SelectorIndex = selectorIndex;
			return selector;
		}

		public static void ReleaseLiteralText(LiteralText literal)
		{
			FormatItemPool.s_LiteralTextPool.Release(literal);
		}

		public static void ReleaseFormat(Format format)
		{
			FormatItemPool.s_FormatPool.Release(format);
		}

		public static void ReleasePlaceholder(Placeholder placeholder)
		{
			FormatItemPool.s_PlaceholderPool.Release(placeholder);
		}

		public static void ReleaseSelector(Selector selector)
		{
			FormatItemPool.s_SelectorPool.Release(selector);
		}

		public static void Release(FormatItem format)
		{
			LiteralText literalText = format as LiteralText;
			if (literalText != null)
			{
				FormatItemPool.ReleaseLiteralText(literalText);
				return;
			}
			Format format2 = format as Format;
			if (format2 != null)
			{
				FormatItemPool.ReleaseFormat(format2);
				return;
			}
			Placeholder placeholder = format as Placeholder;
			if (placeholder != null)
			{
				FormatItemPool.ReleasePlaceholder(placeholder);
				return;
			}
			Selector selector = format as Selector;
			if (selector == null)
			{
				string str = "Unhandled type ";
				Type type = format.GetType();
				Debug.LogError(str + ((type != null) ? type.ToString() : null));
				return;
			}
			FormatItemPool.ReleaseSelector(selector);
		}

		internal static readonly ObjectPool<LiteralText> s_LiteralTextPool = new ObjectPool<LiteralText>(() => new LiteralText(), null, delegate(LiteralText lt)
		{
			lt.Clear();
		}, null, true, 10, 10000);

		internal static readonly ObjectPool<Format> s_FormatPool = new ObjectPool<Format>(() => new Format(), null, delegate(Format f)
		{
			f.ReleaseToPool();
		}, null, true, 10, 10000);

		internal static readonly ObjectPool<Placeholder> s_PlaceholderPool = new ObjectPool<Placeholder>(() => new Placeholder(), null, delegate(Placeholder p)
		{
			p.ReleaseToPool();
		}, null, true, 10, 10000);

		internal static readonly ObjectPool<Selector> s_SelectorPool = new ObjectPool<Selector>(() => new Selector(), null, delegate(Selector s)
		{
			s.Clear();
		}, null, true, 10, 10000);
	}
}
