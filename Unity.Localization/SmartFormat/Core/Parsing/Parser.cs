using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Core.Settings;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	[Serializable]
	public class Parser
	{
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

		public event EventHandler<ParsingErrorEventArgs> OnParsingFailure;

		public Parser(SmartSettings settings)
		{
			this.m_Settings = settings;
		}

		public void AddAlphanumericSelectors()
		{
			this.m_AlphanumericSelectors = true;
		}

		public void AddAdditionalSelectorChars(string chars)
		{
			foreach (char value in chars)
			{
				if (this.m_AllowedSelectorChars.IndexOf(value) == -1)
				{
					this.m_AllowedSelectorChars += value.ToString();
				}
			}
		}

		public void AddOperators(string chars)
		{
			foreach (char value in chars)
			{
				if (this.m_Operators.IndexOf(value) == -1)
				{
					this.m_Operators += value.ToString();
				}
			}
		}

		public void UseAlternativeEscapeChar(char alternativeEscapeChar = '\\')
		{
			this.m_AlternativeEscapeChar = alternativeEscapeChar;
			this.m_AlternativeEscaping = true;
		}

		public void UseBraceEscaping()
		{
			this.m_AlternativeEscaping = false;
		}

		public void UseAlternativeBraces(char opening, char closing)
		{
			this.m_OpeningBrace = opening;
			this.m_ClosingBrace = closing;
		}

		public Format ParseFormat(string format, IList<string> formatterExtensionNames)
		{
			Format format2 = FormatItemPool.GetFormat(this.Settings, format);
			Format format3 = format2;
			Placeholder placeholder = null;
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			ParsingErrors parsingErrors = ParsingErrorsPool.Get(format2);
			if (Parser.s_ParsingErrorText == null)
			{
				Parser.s_ParsingErrorText = new Parser.ParsingErrorText();
			}
			char openingBrace = this.m_OpeningBrace;
			char closingBrace = this.m_ClosingBrace;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int i = 0;
			int length = format.Length;
			while (i < length)
			{
				char c = format[i];
				if (placeholder == null)
				{
					if (c == openingBrace)
					{
						if (i != num5)
						{
							format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, num5, i));
						}
						num5 = i + 1;
						if (!this.m_AlternativeEscaping)
						{
							int num8 = num5;
							if (num8 < length && format[num8] == openingBrace)
							{
								i++;
								goto IL_5ED;
							}
						}
						num4++;
						placeholder = FormatItemPool.GetPlaceholder(this.Settings, format3, i, num4);
						format3.Items.Add(placeholder);
						format3.HasNested = true;
						num6 = i + 1;
						num7 = 0;
						num = -1;
					}
					else if (c == closingBrace)
					{
						if (i != num5)
						{
							format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, num5, i));
						}
						num5 = i + 1;
						if (!this.m_AlternativeEscaping)
						{
							int num9 = num5;
							if (num9 < length && format[num9] == closingBrace)
							{
								i++;
								goto IL_5ED;
							}
						}
						if (format3.parent == null)
						{
							format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, i, i + 1));
							parsingErrors.AddIssue(Parser.s_ParsingErrorText[Parser.ParsingError.TooManyClosingBraces], i, i + 1);
						}
						else
						{
							num4--;
							format3.endIndex = i;
							format3.parent.endIndex = i + 1;
							format3 = (format3.parent.Parent as Format);
							num = -1;
						}
					}
					else if ((c == '\\' && this.Settings.ConvertCharacterStringLiterals) || (this.m_AlternativeEscaping && c == this.m_AlternativeEscapeChar))
					{
						num = -1;
						int num10 = i + 1;
						if (num10 < length && (format[num10] == openingBrace || format[num10] == closingBrace))
						{
							if (i != num5)
							{
								format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, num5, i));
							}
							num5 = i + 1;
							i++;
						}
						else
						{
							if (i != num5)
							{
								format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, num5, i));
							}
							if (i + 1 < format.Length && format[i + 1] == 'u')
							{
								num5 = i + 6;
							}
							else
							{
								num5 = i + 2;
							}
							if (num5 > length)
							{
								num5 = length;
							}
							format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, i, num5));
							i++;
						}
					}
					else if (num != -1)
					{
						if (c == '(')
						{
							if (num == i)
							{
								num = -1;
							}
							else
							{
								num2 = i;
							}
						}
						else if (c == ')' || c == ':')
						{
							if (c == ')')
							{
								bool flag = num2 != -1;
								int num11 = i + 1;
								bool flag2 = num11 < format.Length && (format[num11] == ':' || format[num11] == closingBrace);
								if (!flag || !flag2)
								{
									num = -1;
									goto IL_5ED;
								}
								num3 = i;
								if (format[num11] == ':')
								{
									i++;
								}
							}
							bool flag3 = num == i;
							bool flag4 = num2 != -1 && num3 == -1;
							if (flag3 || flag4)
							{
								num = -1;
							}
							else
							{
								num5 = i + 1;
								Placeholder parent = format3.parent;
								if (num2 == -1)
								{
									string text = format.Substring(num, i - num);
									if (Parser.FormatterNameExists(text, formatterExtensionNames))
									{
										parent.FormatterName = text;
									}
									else
									{
										num5 = format3.startIndex;
									}
								}
								else
								{
									string text2 = format.Substring(num, num2 - num);
									if (Parser.FormatterNameExists(text2, formatterExtensionNames))
									{
										parent.FormatterName = text2;
										parent.FormatterOptions = format.Substring(num2 + 1, num3 - (num2 + 1));
									}
									else
									{
										num5 = format3.startIndex;
									}
								}
								format3.startIndex = num5;
								num = -1;
							}
						}
					}
				}
				else if (this.m_Operators.IndexOf(c) != -1)
				{
					if (i != num5)
					{
						placeholder.Selectors.Add(FormatItemPool.GetSelector(this.Settings, placeholder, format, num5, i, num6, num7));
						num7++;
						num6 = i;
					}
					num5 = i + 1;
				}
				else if (c == ':')
				{
					if (i != num5)
					{
						placeholder.Selectors.Add(FormatItemPool.GetSelector(this.Settings, placeholder, format, num5, i, num6, num7));
					}
					else if (num6 != i)
					{
						parsingErrors.AddIssue(string.Format("'0x{0:X}': {1}", Convert.ToByte(c), Parser.s_ParsingErrorText[Parser.ParsingError.TrailingOperatorsInSelector]), num6, i);
					}
					num5 = i + 1;
					placeholder.Format = FormatItemPool.GetFormat(this.Settings, placeholder, i + 1);
					format3 = placeholder.Format;
					placeholder = null;
					num = num5;
					num2 = -1;
					num3 = -1;
				}
				else if (c == closingBrace)
				{
					if (i != num5)
					{
						placeholder.Selectors.Add(FormatItemPool.GetSelector(this.Settings, placeholder, format, num5, i, num6, num7));
					}
					else if (num6 != i)
					{
						parsingErrors.AddIssue(string.Format("'0x{0:X}': {1}", Convert.ToByte(c), Parser.s_ParsingErrorText[Parser.ParsingError.TrailingOperatorsInSelector]), num6, i);
					}
					num5 = i + 1;
					num4--;
					placeholder.endIndex = i + 1;
					format3 = (placeholder.Parent as Format);
					placeholder = null;
				}
				else if (('0' > c || c > '9') && (!this.m_AlphanumericSelectors || (('a' > c || c > 'z') && ('A' > c || c > 'Z'))) && this.m_AllowedSelectorChars.IndexOf(c) == -1)
				{
					parsingErrors.AddIssue(string.Format("'0x{0:X}': {1}", Convert.ToByte(c), Parser.s_ParsingErrorText[Parser.ParsingError.TrailingOperatorsInSelector]), i, i + 1);
				}
				IL_5ED:
				i++;
			}
			if (format3.parent != null || placeholder != null)
			{
				parsingErrors.AddIssue(Parser.s_ParsingErrorText[Parser.ParsingError.MissingClosingBrace], format.Length, format.Length);
				format3.endIndex = format.Length;
			}
			else if (num5 != format.Length)
			{
				format3.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format3, num5, format.Length));
			}
			while (format3.parent != null)
			{
				format3 = (format3.parent.Parent as Format);
				format3.endIndex = format.Length;
			}
			if (parsingErrors.HasIssues)
			{
				EventHandler<ParsingErrorEventArgs> onParsingFailure = this.OnParsingFailure;
				if (onParsingFailure != null)
				{
					onParsingFailure(this, new ParsingErrorEventArgs(parsingErrors, this.Settings.ParseErrorAction == ErrorAction.ThrowError));
				}
				return this.HandleParsingErrors(parsingErrors, format2);
			}
			ParsingErrorsPool.Release(parsingErrors);
			return format2;
		}

		private Format HandleParsingErrors(ParsingErrors parsingErrors, Format currentResult)
		{
			switch (this.Settings.ParseErrorAction)
			{
			case ErrorAction.ThrowError:
				throw parsingErrors;
			case ErrorAction.OutputErrorInResult:
			{
				Format format = FormatItemPool.GetFormat(this.Settings, parsingErrors.Message, 0, parsingErrors.Message.Length);
				format.Items.Add(FormatItemPool.GetLiteralText(this.Settings, format, 0));
				return format;
			}
			case ErrorAction.Ignore:
			{
				int j;
				int i;
				for (i = 0; i < currentResult.Items.Count; i = j + 1)
				{
					Placeholder placeholder = currentResult.Items[i] as Placeholder;
					if (placeholder != null && parsingErrors.Issues.Any((ParsingErrors.ParsingIssue errItem) => errItem.Index >= currentResult.Items[i].startIndex && errItem.Index <= currentResult.Items[i].endIndex))
					{
						currentResult.Items[i] = FormatItemPool.GetLiteralText(this.Settings, placeholder.Format ?? FormatItemPool.GetFormat(this.Settings, placeholder.baseString), placeholder.startIndex, placeholder.startIndex);
					}
					j = i;
				}
				return currentResult;
			}
			case ErrorAction.MaintainTokens:
			{
				int j;
				int i;
				for (i = 0; i < currentResult.Items.Count; i = j + 1)
				{
					Placeholder placeholder2 = currentResult.Items[i] as Placeholder;
					if (placeholder2 != null && parsingErrors.Issues.Any((ParsingErrors.ParsingIssue errItem) => errItem.Index >= currentResult.Items[i].startIndex && errItem.Index <= currentResult.Items[i].endIndex))
					{
						currentResult.Items[i] = FormatItemPool.GetLiteralText(this.Settings, placeholder2.Format ?? FormatItemPool.GetFormat(this.Settings, placeholder2.baseString), placeholder2.startIndex, placeholder2.endIndex);
					}
					j = i;
				}
				return currentResult;
			}
			default:
				throw new ArgumentException("Illegal type for ParsingErrors", parsingErrors);
			}
		}

		private static bool FormatterNameExists(string name, IList<string> formatterExtensionNames)
		{
			using (IEnumerator<string> enumerator = formatterExtensionNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == name)
					{
						return true;
					}
				}
			}
			return false;
		}

		[SerializeField]
		private char m_OpeningBrace = '{';

		[SerializeField]
		private char m_ClosingBrace = '}';

		[SerializeReference]
		[HideInInspector]
		private SmartSettings m_Settings;

		[Tooltip("If false, only digits are allowed as selectors. If true, selectors can be alpha-numeric. This allows optimized alpha-character detection. Specify any additional selector chars in AllowedSelectorChars.")]
		[SerializeField]
		private bool m_AlphanumericSelectors;

		[Tooltip("A list of allowable selector characters, to support additional selector syntaxes such as math. Digits are always included, and letters can be included with AlphanumericSelectors.")]
		[SerializeField]
		private string m_AllowedSelectorChars = "";

		[Tooltip("A list of characters that come between selectors. This can be \".\" for dot-notation, \"[]\" for arrays, or even math symbols. By default, there are no operators.")]
		[SerializeField]
		private string m_Operators = "";

		[Tooltip("If false, double-curly braces are escaped. If true, the AlternativeEscapeChar is used for escaping braces.")]
		[SerializeField]
		private bool m_AlternativeEscaping;

		[Tooltip("If AlternativeEscaping is true, then this character is used to escape curly braces.")]
		[SerializeField]
		private char m_AlternativeEscapeChar = '\\';

		[Tooltip("The character literal escape character e.g. for \t (TAB) and others. This is kind of overlapping functionality with `UseAlternativeEscapeChar` Note: In a future release escape characters for placeholders  and character literals should become the same.")]
		[SerializeField]
		internal const char m_CharLiteralEscapeChar = '\\';

		private static Parser.ParsingErrorText s_ParsingErrorText;

		public enum ParsingError
		{
			TooManyClosingBraces = 1,
			TrailingOperatorsInSelector,
			InvalidCharactersInSelector,
			MissingClosingBrace
		}

		internal class ParsingErrorText
		{
			internal ParsingErrorText()
			{
			}

			public string this[Parser.ParsingError parsingErrorKey]
			{
				get
				{
					return this._errors[parsingErrorKey];
				}
			}

			private readonly Dictionary<Parser.ParsingError, string> _errors = new Dictionary<Parser.ParsingError, string>
			{
				{
					Parser.ParsingError.TooManyClosingBraces,
					"Format string has too many closing braces"
				},
				{
					Parser.ParsingError.TrailingOperatorsInSelector,
					"There are trailing operators in the selector"
				},
				{
					Parser.ParsingError.InvalidCharactersInSelector,
					"Invalid character in the selector"
				},
				{
					Parser.ParsingError.MissingClosingBrace,
					"Format string is missing a closing brace"
				}
			};
		}
	}
}
