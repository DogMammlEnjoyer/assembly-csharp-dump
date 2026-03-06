using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class TemplateFormatter : FormatterBase
	{
		private IDictionary<string, Format> Templates
		{
			get
			{
				if (this.m_TemplatesDict == null)
				{
					IEqualityComparer<string> caseSensitivityComparer = this.Formatter.Settings.GetCaseSensitivityComparer();
					this.m_TemplatesDict = new Dictionary<string, Format>(caseSensitivityComparer);
					foreach (TemplateFormatter.Template template in this.m_Templates)
					{
						if (!string.IsNullOrEmpty(template.name))
						{
							try
							{
								this.m_TemplatesDict[template.name] = this.Formatter.Parser.ParseFormat(template.text, this.Formatter.GetNotEmptyFormatterExtensionNames());
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
						}
					}
				}
				return this.m_TemplatesDict;
			}
		}

		public SmartFormatter Formatter
		{
			get
			{
				return this.m_Formatter ?? LocalizationSettings.StringDatabase.SmartFormatter;
			}
			set
			{
				this.m_Formatter = value;
			}
		}

		public TemplateFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"template",
					"t"
				};
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			string text = formattingInfo.FormatterOptions;
			if (text == string.Empty)
			{
				if (formattingInfo.Format.HasNested)
				{
					return false;
				}
				text = formattingInfo.Format.RawText;
			}
			Format format;
			if (this.Templates.TryGetValue(text, out format))
			{
				formattingInfo.Write(format, formattingInfo.CurrentValue);
				return true;
			}
			if (base.Names.Contains(formattingInfo.Placeholder.FormatterName))
			{
				throw new FormatException(string.Concat(new string[]
				{
					"Formatter '",
					formattingInfo.Placeholder.FormatterName,
					"' found no registered template named '",
					text,
					"'"
				}));
			}
			return false;
		}

		public void Register(string templateName, string template)
		{
			Format value = this.Formatter.Parser.ParseFormat(template, this.Formatter.GetNotEmptyFormatterExtensionNames());
			this.Templates.Add(templateName, value);
		}

		public bool Remove(string templateName)
		{
			return this.Templates.Remove(templateName);
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			this.m_TemplatesDict = null;
		}

		public void Clear()
		{
			this.Templates.Clear();
		}

		[SerializeField]
		private List<TemplateFormatter.Template> m_Templates = new List<TemplateFormatter.Template>();

		private IDictionary<string, Format> m_TemplatesDict;

		[NonSerialized]
		private SmartFormatter m_Formatter;

		[Serializable]
		private class Template
		{
			public Format Format { get; set; }

			public string name;

			public string text;
		}
	}
}
