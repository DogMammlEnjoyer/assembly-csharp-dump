using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Extensions;

namespace UnityEngine.Localization.SmartFormat
{
	public static class Smart
	{
		public static string Format(string format, params object[] args)
		{
			return Smart.Default.Format(format, args);
		}

		public static string Format(IFormatProvider provider, string format, params object[] args)
		{
			return Smart.Default.Format(provider, format, args);
		}

		public static string Format(string format, object arg0, object arg1, object arg2)
		{
			return Smart.Format(format, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public static string Format(string format, object arg0, object arg1)
		{
			return Smart.Format(format, new object[]
			{
				arg0,
				arg1
			});
		}

		public static string Format(string format, object arg0)
		{
			return Smart.Default.Format(format, new object[]
			{
				arg0
			});
		}

		public static SmartFormatter Default { get; set; } = Smart.CreateDefaultSmartFormat();

		public static SmartFormatter CreateDefaultSmartFormat()
		{
			SmartFormatter smartFormatter = new SmartFormatter();
			ListFormatter listFormatter = new ListFormatter(smartFormatter);
			smartFormatter.AddExtensions(new ISource[]
			{
				listFormatter,
				new PersistentVariablesSource(smartFormatter),
				new DictionarySource(smartFormatter),
				new ValueTupleSource(smartFormatter),
				new XmlSource(smartFormatter),
				new ReflectionSource(smartFormatter),
				new DefaultSource(smartFormatter)
			});
			smartFormatter.AddExtensions(new IFormatter[]
			{
				listFormatter,
				new PluralLocalizationFormatter(),
				new ConditionalFormatter(),
				new TimeFormatter(),
				new XElementFormatter(),
				new ChooseFormatter(),
				new SubStringFormatter(),
				new IsMatchFormatter(),
				new DefaultFormatter()
			});
			return smartFormatter;
		}
	}
}
