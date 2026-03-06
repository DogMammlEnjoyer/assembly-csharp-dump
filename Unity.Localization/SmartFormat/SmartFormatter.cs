using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Core.Output;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Localization.SmartFormat.Core.Settings;

namespace UnityEngine.Localization.SmartFormat
{
	[Serializable]
	public class SmartFormatter : ISerializationCallbackReceiver
	{
		public event EventHandler<FormattingErrorEventArgs> OnFormattingFailure;

		public List<ISource> SourceExtensions
		{
			get
			{
				return this.m_Sources;
			}
		}

		public List<IFormatter> FormatterExtensions
		{
			get
			{
				return this.m_Formatters;
			}
		}

		public SmartFormatter()
		{
			this.m_Settings = new SmartSettings();
			this.m_Parser = new Parser(this.m_Settings);
			this.m_Sources = new List<ISource>();
			this.m_Formatters = new List<IFormatter>();
		}

		public List<string> GetNotEmptyFormatterExtensionNames()
		{
			if (this.m_NotEmptyFormatterExtensionNames != null)
			{
				return this.m_NotEmptyFormatterExtensionNames;
			}
			this.m_NotEmptyFormatterExtensionNames = new List<string>();
			foreach (IFormatter formatter in this.FormatterExtensions)
			{
				if (((formatter != null) ? formatter.Names : null) != null)
				{
					foreach (string text in formatter.Names)
					{
						if (!string.IsNullOrEmpty(text))
						{
							this.m_NotEmptyFormatterExtensionNames.Add(text);
						}
					}
				}
			}
			return this.m_NotEmptyFormatterExtensionNames;
		}

		public void AddExtensions(params ISource[] sourceExtensions)
		{
			this.SourceExtensions.InsertRange(0, sourceExtensions);
		}

		public void AddExtensions(params IFormatter[] formatterExtensions)
		{
			this.m_NotEmptyFormatterExtensionNames = null;
			this.FormatterExtensions.InsertRange(0, formatterExtensions);
		}

		public T GetSourceExtension<T>() where T : class, ISource
		{
			return this.SourceExtensions.OfType<T>().FirstOrDefault<T>();
		}

		public T GetFormatterExtension<T>() where T : class, IFormatter
		{
			return this.FormatterExtensions.OfType<T>().FirstOrDefault<T>();
		}

		public Parser Parser
		{
			get
			{
				return this.m_Parser;
			}
			set
			{
				this.m_Parser = value;
			}
		}

		public SmartSettings Settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				this.m_Settings = value;
			}
		}

		public string Format(string format, params object[] args)
		{
			return this.Format(null, args, format);
		}

		public string Format(IList<object> args, string format)
		{
			return this.Format(null, args, format);
		}

		public string Format(IFormatProvider provider, string format, params object[] args)
		{
			return this.Format(provider, args, format);
		}

		public string Format(IFormatProvider provider, IList<object> args, string format)
		{
			args = (args ?? SmartFormatter.k_Empty);
			StringOutput stringOutput;
			string result;
			using (StringOutputPool.Get(format.Length + args.Count * 8, out stringOutput))
			{
				Format format2 = this.Parser.ParseFormat(format, this.GetNotEmptyFormatterExtensionNames());
				object current = (args.Count > 0) ? args[0] : args;
				FormatDetails formatDetails = FormatDetailsPool.Get(this, format2, args, null, provider, stringOutput);
				this.Format(formatDetails, format2, current);
				FormatDetailsPool.Release(formatDetails);
				FormatItemPool.ReleaseFormat(format2);
				result = stringOutput.ToString();
			}
			return result;
		}

		public void FormatInto(IOutput output, string format, params object[] args)
		{
			args = (args ?? SmartFormatter.k_Empty);
			Format format2 = this.Parser.ParseFormat(format, this.GetNotEmptyFormatterExtensionNames());
			object current = (args.Length != 0) ? args[0] : args;
			FormatDetails formatDetails = FormatDetailsPool.Get(this, format2, args, null, null, output);
			this.Format(formatDetails, format2, current);
			FormatDetailsPool.Release(formatDetails);
			FormatItemPool.ReleaseFormat(format2);
		}

		public string FormatWithCache(ref FormatCache cache, string format, IList<object> args)
		{
			return this.FormatWithCache(ref cache, format, null, args);
		}

		public string FormatWithCache(ref FormatCache cache, string format, IFormatProvider formatProvider, IList<object> args)
		{
			args = (args ?? SmartFormatter.k_Empty);
			StringOutput stringOutput;
			string result;
			using (StringOutputPool.Get(format.Length + args.Count * 8, out stringOutput))
			{
				if (cache == null)
				{
					cache = FormatCachePool.Get(this.Parser.ParseFormat(format, this.GetNotEmptyFormatterExtensionNames()));
				}
				object current = (args.Count > 0) ? args[0] : args;
				FormatDetails formatDetails = FormatDetailsPool.Get(this, cache.Format, args, cache, formatProvider, stringOutput);
				this.Format(formatDetails, cache.Format, current);
				FormatDetailsPool.Release(formatDetails);
				result = stringOutput.ToString();
			}
			return result;
		}

		public void FormatWithCacheInto(ref FormatCache cache, IOutput output, string format, params object[] args)
		{
			args = (args ?? SmartFormatter.k_Empty);
			if (cache == null)
			{
				cache = FormatCachePool.Get(this.Parser.ParseFormat(format, this.GetNotEmptyFormatterExtensionNames()));
			}
			object current = (args.Length != 0) ? args[0] : args;
			FormatDetails formatDetails = FormatDetailsPool.Get(this, cache.Format, args, cache, null, output);
			this.Format(formatDetails, cache.Format, current);
			FormatDetailsPool.Release(formatDetails);
		}

		private void Format(FormatDetails formatDetails, Format format, object current)
		{
			FormattingInfo formattingInfo = FormattingInfoPool.Get(formatDetails, format, current);
			this.Format(formattingInfo);
			FormattingInfoPool.Release(formattingInfo);
		}

		public virtual void Format(FormattingInfo formattingInfo)
		{
			if (formattingInfo.Format == null)
			{
				return;
			}
			this.CheckForExtensions();
			foreach (FormatItem formatItem in formattingInfo.Format.Items)
			{
				LiteralText literalText = formatItem as LiteralText;
				if (literalText != null)
				{
					formattingInfo.Write(literalText.ToString());
				}
				else
				{
					Placeholder placeholder = (Placeholder)formatItem;
					FormattingInfo formattingInfo2 = formattingInfo.CreateChild(placeholder);
					try
					{
						this.EvaluateSelectors(formattingInfo2);
					}
					catch (DataNotReadyException ex)
					{
						if (!string.IsNullOrEmpty(ex.Text))
						{
							formattingInfo.Write(ex.Text);
						}
						continue;
					}
					catch (Exception innerException)
					{
						Format format = placeholder.Format;
						int startIndex = (format != null) ? format.startIndex : placeholder.Selectors.Last<Selector>().endIndex;
						this.FormatError(formatItem, innerException, startIndex, formattingInfo2);
						continue;
					}
					try
					{
						this.EvaluateFormatters(formattingInfo2);
					}
					catch (Exception innerException2)
					{
						Format format2 = placeholder.Format;
						int startIndex2 = (format2 != null) ? format2.startIndex : placeholder.Selectors.Last<Selector>().endIndex;
						this.FormatError(formatItem, innerException2, startIndex2, formattingInfo2);
					}
				}
			}
		}

		private void FormatError(FormatItem errorItem, Exception innerException, int startIndex, FormattingInfo formattingInfo)
		{
			EventHandler<FormattingErrorEventArgs> onFormattingFailure = this.OnFormattingFailure;
			if (onFormattingFailure != null)
			{
				onFormattingFailure(this, new FormattingErrorEventArgs(errorItem.RawText, startIndex, this.Settings.FormatErrorAction > ErrorAction.ThrowError));
			}
			switch (this.Settings.FormatErrorAction)
			{
			case ErrorAction.ThrowError:
				throw (innerException as FormattingException) ?? new FormattingException(errorItem, innerException, startIndex);
			case ErrorAction.OutputErrorInResult:
				formattingInfo.FormatDetails.FormattingException = ((innerException as FormattingException) ?? new FormattingException(errorItem, innerException, startIndex));
				formattingInfo.Write(innerException.Message);
				formattingInfo.FormatDetails.FormattingException = null;
				return;
			case ErrorAction.Ignore:
				return;
			case ErrorAction.MaintainTokens:
				formattingInfo.Write(formattingInfo.Placeholder.RawText);
				return;
			default:
				return;
			}
		}

		private void CheckForExtensions()
		{
			if (this.SourceExtensions.Count == 0)
			{
				throw new InvalidOperationException("No source extensions are available. Please add at least one source extension, such as the DefaultSource.");
			}
			if (this.FormatterExtensions.Count == 0)
			{
				throw new InvalidOperationException("No formatter extensions are available. Please add at least one formatter extension, such as the DefaultFormatter.");
			}
		}

		private void EvaluateSelectors(FormattingInfo formattingInfo)
		{
			if (formattingInfo.Placeholder == null)
			{
				return;
			}
			bool flag = true;
			foreach (Selector selector in formattingInfo.Placeholder.Selectors)
			{
				formattingInfo.Selector = selector;
				formattingInfo.Result = null;
				bool flag2 = this.InvokeSourceExtensions(formattingInfo);
				if (flag2)
				{
					formattingInfo.CurrentValue = formattingInfo.Result;
				}
				if (flag)
				{
					flag = false;
					FormattingInfo formattingInfo2 = formattingInfo;
					while (!flag2 && formattingInfo2.Parent != null)
					{
						formattingInfo2 = formattingInfo2.Parent;
						formattingInfo2.Selector = selector;
						formattingInfo2.Result = null;
						flag2 = this.InvokeSourceExtensions(formattingInfo2);
						if (flag2)
						{
							formattingInfo.CurrentValue = formattingInfo2.Result;
						}
					}
				}
				if (!flag2)
				{
					throw formattingInfo.FormattingException("Could not evaluate the selector \"" + selector.RawText + "\"", selector, -1);
				}
			}
		}

		private bool InvokeSourceExtensions(FormattingInfo formattingInfo)
		{
			using (List<ISource>.Enumerator enumerator = this.SourceExtensions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TryEvaluateSelector(formattingInfo))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void EvaluateFormatters(FormattingInfo formattingInfo)
		{
			if (!this.InvokeFormatterExtensions(formattingInfo))
			{
				throw formattingInfo.FormattingException("No suitable Formatter could be found", formattingInfo.Format, -1);
			}
		}

		private bool InvokeFormatterExtensions(FormattingInfo formattingInfo)
		{
			if (formattingInfo.Placeholder == null)
			{
				return false;
			}
			string formatterName = formattingInfo.Placeholder.FormatterName;
			foreach (IFormatter formatter in this.FormatterExtensions)
			{
				if (formatter.Names.Contains(formatterName) && formatter.TryEvaluateFormat(formattingInfo))
				{
					return true;
				}
			}
			return false;
		}

		public void OnBeforeSerialize()
		{
			this.m_NotEmptyFormatterExtensionNames = null;
		}

		public void OnAfterDeserialize()
		{
			this.m_NotEmptyFormatterExtensionNames = null;
		}

		[SerializeReference]
		private SmartSettings m_Settings;

		[SerializeReference]
		private Parser m_Parser;

		[SerializeReference]
		private List<ISource> m_Sources;

		[SerializeReference]
		private List<IFormatter> m_Formatters;

		private List<string> m_NotEmptyFormatterExtensionNames;

		private static readonly object[] k_Empty = new object[1];
	}
}
