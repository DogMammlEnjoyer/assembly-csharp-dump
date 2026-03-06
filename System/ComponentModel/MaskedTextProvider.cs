using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace System.ComponentModel
{
	/// <summary>Represents a mask-parsing service that can be used by any number of controls that support masking, such as the <see cref="T:System.Windows.Forms.MaskedTextBox" /> control.</summary>
	public class MaskedTextProvider : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		public MaskedTextProvider(string mask) : this(mask, null, true, '_', '\0', false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask and ASCII restriction value.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		/// <param name="restrictToAscii">
		///   <see langword="true" /> to restrict input to ASCII-compatible characters; otherwise <see langword="false" /> to allow the entire Unicode set.</param>
		public MaskedTextProvider(string mask, bool restrictToAscii) : this(mask, null, true, '_', '\0', restrictToAscii)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask and culture.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that is used to set region-sensitive separator characters.</param>
		public MaskedTextProvider(string mask, CultureInfo culture) : this(mask, culture, true, '_', '\0', false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask, culture, and ASCII restriction value.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that is used to set region-sensitive separator characters.</param>
		/// <param name="restrictToAscii">
		///   <see langword="true" /> to restrict input to ASCII-compatible characters; otherwise <see langword="false" /> to allow the entire Unicode set.</param>
		public MaskedTextProvider(string mask, CultureInfo culture, bool restrictToAscii) : this(mask, culture, true, '_', '\0', restrictToAscii)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask, password character, and prompt usage value.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		/// <param name="passwordChar">A <see cref="T:System.Char" /> that will be displayed for characters entered into a password string.</param>
		/// <param name="allowPromptAsInput">
		///   <see langword="true" /> to allow the prompt character as input; otherwise <see langword="false" />.</param>
		public MaskedTextProvider(string mask, char passwordChar, bool allowPromptAsInput) : this(mask, null, allowPromptAsInput, '_', passwordChar, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask, culture, password character, and prompt usage value.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that is used to set region-sensitive separator characters.</param>
		/// <param name="passwordChar">A <see cref="T:System.Char" /> that will be displayed for characters entered into a password string.</param>
		/// <param name="allowPromptAsInput">
		///   <see langword="true" /> to allow the prompt character as input; otherwise <see langword="false" />.</param>
		public MaskedTextProvider(string mask, CultureInfo culture, char passwordChar, bool allowPromptAsInput) : this(mask, culture, allowPromptAsInput, '_', passwordChar, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.MaskedTextProvider" /> class using the specified mask, culture, prompt usage value, prompt character, password character, and ASCII restriction value.</summary>
		/// <param name="mask">A <see cref="T:System.String" /> that represents the input mask.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that is used to set region-sensitive separator characters.</param>
		/// <param name="allowPromptAsInput">A <see cref="T:System.Boolean" /> value that specifies whether the prompt character should be allowed as a valid input character.</param>
		/// <param name="promptChar">A <see cref="T:System.Char" /> that will be displayed as a placeholder for user input.</param>
		/// <param name="passwordChar">A <see cref="T:System.Char" /> that will be displayed for characters entered into a password string.</param>
		/// <param name="restrictToAscii">
		///   <see langword="true" /> to restrict input to ASCII-compatible characters; otherwise <see langword="false" /> to allow the entire Unicode set.</param>
		/// <exception cref="T:System.ArgumentException">The mask parameter is <see langword="null" /> or <see cref="F:System.String.Empty" />.  
		///  -or-  
		///  The mask contains one or more non-printable characters.</exception>
		public MaskedTextProvider(string mask, CultureInfo culture, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
		{
			if (string.IsNullOrEmpty(mask))
			{
				throw new ArgumentException(SR.Format("The Mask value cannot be null or empty.", Array.Empty<object>()), "mask");
			}
			for (int i = 0; i < mask.Length; i++)
			{
				if (!MaskedTextProvider.IsPrintableChar(mask[i]))
				{
					throw new ArgumentException("The specified mask contains invalid characters.");
				}
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			this._flagState = default(BitVector32);
			this.Mask = mask;
			this._promptChar = promptChar;
			this._passwordChar = passwordChar;
			if (culture.IsNeutralCulture)
			{
				foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
				{
					if (culture.Equals(cultureInfo.Parent))
					{
						this.Culture = cultureInfo;
						break;
					}
				}
				if (this.Culture == null)
				{
					this.Culture = CultureInfo.InvariantCulture;
				}
			}
			else
			{
				this.Culture = culture;
			}
			if (!this.Culture.IsReadOnly)
			{
				this.Culture = CultureInfo.ReadOnly(this.Culture);
			}
			this._flagState[MaskedTextProvider.s_ALLOW_PROMPT_AS_INPUT] = allowPromptAsInput;
			this._flagState[MaskedTextProvider.s_ASCII_ONLY] = restrictToAscii;
			this._flagState[MaskedTextProvider.s_INCLUDE_PROMPT] = false;
			this._flagState[MaskedTextProvider.s_INCLUDE_LITERALS] = true;
			this._flagState[MaskedTextProvider.s_RESET_ON_PROMPT] = true;
			this._flagState[MaskedTextProvider.s_SKIP_SPACE] = true;
			this._flagState[MaskedTextProvider.s_RESET_ON_LITERALS] = true;
			this.Initialize();
		}

		private void Initialize()
		{
			this._testString = new StringBuilder();
			this._stringDescriptor = new List<MaskedTextProvider.CharDescriptor>();
			MaskedTextProvider.CaseConversion caseConversion = MaskedTextProvider.CaseConversion.None;
			bool flag = false;
			int num = 0;
			MaskedTextProvider.CharType charType = MaskedTextProvider.CharType.Literal;
			string text = string.Empty;
			int i = 0;
			while (i < this.Mask.Length)
			{
				char c = this.Mask[i];
				if (!flag)
				{
					if (c <= 'C')
					{
						switch (c)
						{
						case '#':
							goto IL_19E;
						case '$':
							text = this.Culture.NumberFormat.CurrencySymbol;
							charType = MaskedTextProvider.CharType.Separator;
							goto IL_1BE;
						case '%':
							goto IL_1B8;
						case '&':
							break;
						default:
							switch (c)
							{
							case ',':
								text = this.Culture.NumberFormat.NumberGroupSeparator;
								charType = MaskedTextProvider.CharType.Separator;
								goto IL_1BE;
							case '-':
								goto IL_1B8;
							case '.':
								text = this.Culture.NumberFormat.NumberDecimalSeparator;
								charType = MaskedTextProvider.CharType.Separator;
								goto IL_1BE;
							case '/':
								text = this.Culture.DateTimeFormat.DateSeparator;
								charType = MaskedTextProvider.CharType.Separator;
								goto IL_1BE;
							case '0':
								break;
							default:
								switch (c)
								{
								case '9':
								case '?':
								case 'C':
									goto IL_19E;
								case ':':
									text = this.Culture.DateTimeFormat.TimeSeparator;
									charType = MaskedTextProvider.CharType.Separator;
									goto IL_1BE;
								case ';':
								case '=':
								case '@':
								case 'B':
									goto IL_1B8;
								case '<':
									caseConversion = MaskedTextProvider.CaseConversion.ToLower;
									goto IL_22A;
								case '>':
									caseConversion = MaskedTextProvider.CaseConversion.ToUpper;
									goto IL_22A;
								case 'A':
									break;
								default:
									goto IL_1B8;
								}
								break;
							}
							break;
						}
					}
					else if (c <= '\\')
					{
						if (c != 'L')
						{
							if (c != '\\')
							{
								goto IL_1B8;
							}
							flag = true;
							charType = MaskedTextProvider.CharType.Literal;
							goto IL_22A;
						}
					}
					else
					{
						if (c == 'a')
						{
							goto IL_19E;
						}
						if (c != '|')
						{
							goto IL_1B8;
						}
						caseConversion = MaskedTextProvider.CaseConversion.None;
						goto IL_22A;
					}
					this._requiredEditChars++;
					c = this._promptChar;
					charType = MaskedTextProvider.CharType.EditRequired;
					goto IL_1BE;
					IL_19E:
					this._optionalEditChars++;
					c = this._promptChar;
					charType = MaskedTextProvider.CharType.EditOptional;
					goto IL_1BE;
					IL_1B8:
					charType = MaskedTextProvider.CharType.Literal;
					goto IL_1BE;
				}
				flag = false;
				goto IL_1BE;
				IL_22A:
				i++;
				continue;
				IL_1BE:
				MaskedTextProvider.CharDescriptor charDescriptor = new MaskedTextProvider.CharDescriptor(i, charType);
				if (MaskedTextProvider.IsEditPosition(charDescriptor))
				{
					charDescriptor.CaseConversion = caseConversion;
				}
				if (charType != MaskedTextProvider.CharType.Separator)
				{
					text = c.ToString();
				}
				foreach (char value in text)
				{
					this._testString.Append(value);
					this._stringDescriptor.Add(charDescriptor);
					num++;
				}
				goto IL_22A;
			}
			this._testString.Capacity = this._testString.Length;
		}

		/// <summary>Gets a value indicating whether the prompt character should be treated as a valid input character or not.</summary>
		/// <returns>
		///   <see langword="true" /> if the user can enter <see cref="P:System.ComponentModel.MaskedTextProvider.PromptChar" /> into the control; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool AllowPromptAsInput
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_ALLOW_PROMPT_AS_INPUT];
			}
		}

		/// <summary>Gets the number of editable character positions that have already been successfully assigned an input value.</summary>
		/// <returns>An <see cref="T:System.Int32" /> containing the number of editable character positions in the input mask that have already been assigned a character value in the formatted string.</returns>
		public int AssignedEditPositionCount { get; private set; }

		/// <summary>Gets the number of editable character positions in the input mask that have not yet been assigned an input value.</summary>
		/// <returns>An <see cref="T:System.Int32" /> containing the number of editable character positions that not yet been assigned a character value.</returns>
		public int AvailableEditPositionCount
		{
			get
			{
				return this.EditPositionCount - this.AssignedEditPositionCount;
			}
		}

		/// <summary>Creates a copy of the current <see cref="T:System.ComponentModel.MaskedTextProvider" />.</summary>
		/// <returns>The <see cref="T:System.ComponentModel.MaskedTextProvider" /> object this method creates, cast as an object.</returns>
		public object Clone()
		{
			Type type = base.GetType();
			MaskedTextProvider maskedTextProvider;
			if (type == MaskedTextProvider.s_maskTextProviderType)
			{
				maskedTextProvider = new MaskedTextProvider(this.Mask, this.Culture, this.AllowPromptAsInput, this.PromptChar, this.PasswordChar, this.AsciiOnly);
			}
			else
			{
				object[] args = new object[]
				{
					this.Mask,
					this.Culture,
					this.AllowPromptAsInput,
					this.PromptChar,
					this.PasswordChar,
					this.AsciiOnly
				};
				maskedTextProvider = (SecurityUtils.SecureCreateInstance(type, args) as MaskedTextProvider);
			}
			maskedTextProvider.ResetOnPrompt = false;
			maskedTextProvider.ResetOnSpace = false;
			maskedTextProvider.SkipLiterals = false;
			for (int i = 0; i < this._testString.Length; i++)
			{
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
				if (MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned)
				{
					maskedTextProvider.Replace(this._testString[i], i);
				}
			}
			maskedTextProvider.ResetOnPrompt = this.ResetOnPrompt;
			maskedTextProvider.ResetOnSpace = this.ResetOnSpace;
			maskedTextProvider.SkipLiterals = this.SkipLiterals;
			maskedTextProvider.IncludeLiterals = this.IncludeLiterals;
			maskedTextProvider.IncludePrompt = this.IncludePrompt;
			return maskedTextProvider;
		}

		/// <summary>Gets the culture that determines the value of the localizable separators and placeholders in the input mask.</summary>
		/// <returns>A <see cref="T:System.Globalization.CultureInfo" /> containing the culture information associated with the input mask.</returns>
		public CultureInfo Culture { get; }

		/// <summary>Gets the default password character used obscure user input.</summary>
		/// <returns>A <see cref="T:System.Char" /> that represents the default password character.</returns>
		public static char DefaultPasswordChar
		{
			get
			{
				return '*';
			}
		}

		/// <summary>Gets the number of editable positions in the formatted string.</summary>
		/// <returns>An <see cref="T:System.Int32" /> containing the number of editable positions in the formatted string.</returns>
		public int EditPositionCount
		{
			get
			{
				return this._optionalEditChars + this._requiredEditChars;
			}
		}

		/// <summary>Gets a newly created enumerator for the editable positions in the formatted string.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that supports enumeration over the editable positions in the formatted string.</returns>
		public IEnumerator EditPositions
		{
			get
			{
				List<int> list = new List<int>();
				int num = 0;
				using (List<MaskedTextProvider.CharDescriptor>.Enumerator enumerator = this._stringDescriptor.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (MaskedTextProvider.IsEditPosition(enumerator.Current))
						{
							list.Add(num);
						}
						num++;
					}
				}
				return ((IEnumerable)list).GetEnumerator();
			}
		}

		/// <summary>Gets or sets a value that indicates whether literal characters in the input mask should be included in the formatted string.</summary>
		/// <returns>
		///   <see langword="true" /> if literals are included; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool IncludeLiterals
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_INCLUDE_LITERALS];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_INCLUDE_LITERALS] = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether <see cref="P:System.Windows.Forms.MaskedTextBox.PromptChar" /> is used to represent the absence of user input when displaying the formatted string.</summary>
		/// <returns>
		///   <see langword="true" /> if the prompt character is used to represent the positions where no user input was provided; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool IncludePrompt
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_INCLUDE_PROMPT];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_INCLUDE_PROMPT] = value;
			}
		}

		/// <summary>Gets a value indicating whether the mask accepts characters outside of the ASCII character set.</summary>
		/// <returns>
		///   <see langword="true" /> if only ASCII is accepted; <see langword="false" /> if <see cref="T:System.ComponentModel.MaskedTextProvider" /> can accept any arbitrary Unicode character. The default is <see langword="false" />.</returns>
		public bool AsciiOnly
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_ASCII_ONLY];
			}
		}

		/// <summary>Gets or sets a value that determines whether password protection should be applied to the formatted string.</summary>
		/// <returns>
		///   <see langword="true" /> if the input string is to be treated as a password string; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IsPassword
		{
			get
			{
				return this._passwordChar > '\0';
			}
			set
			{
				if (this.IsPassword != value)
				{
					this._passwordChar = (value ? MaskedTextProvider.DefaultPasswordChar : '\0');
				}
			}
		}

		/// <summary>Gets the upper bound of the range of invalid indexes.</summary>
		/// <returns>A value representing the largest invalid index, as determined by the provider implementation. For example, if the lowest valid index is 0, this property will return -1.</returns>
		public static int InvalidIndex
		{
			get
			{
				return -1;
			}
		}

		/// <summary>Gets the index in the mask of the rightmost input character that has been assigned to the mask.</summary>
		/// <returns>If at least one input character has been assigned to the mask, an <see cref="T:System.Int32" /> containing the index of rightmost assigned position; otherwise, if no position has been assigned, <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int LastAssignedPosition
		{
			get
			{
				return this.FindAssignedEditPositionFrom(this._testString.Length - 1, false);
			}
		}

		/// <summary>Gets the length of the mask, absent any mask modifier characters.</summary>
		/// <returns>An <see cref="T:System.Int32" /> containing the number of positions in the mask, excluding characters that modify mask input.</returns>
		public int Length
		{
			get
			{
				return this._testString.Length;
			}
		}

		/// <summary>Gets the input mask.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the full mask.</returns>
		public string Mask { get; }

		/// <summary>Gets a value indicating whether all required inputs have been entered into the formatted string.</summary>
		/// <returns>
		///   <see langword="true" /> if all required input has been entered into the mask; otherwise, <see langword="false" />.</returns>
		public bool MaskCompleted
		{
			get
			{
				return this._requiredCharCount == this._requiredEditChars;
			}
		}

		/// <summary>Gets a value indicating whether all required and optional inputs have been entered into the formatted string.</summary>
		/// <returns>
		///   <see langword="true" /> if all required and optional inputs have been entered; otherwise, <see langword="false" />.</returns>
		public bool MaskFull
		{
			get
			{
				return this.AssignedEditPositionCount == this.EditPositionCount;
			}
		}

		/// <summary>Gets or sets the character to be substituted for the actual input characters.</summary>
		/// <returns>The <see cref="T:System.Char" /> value used as the password character.</returns>
		/// <exception cref="T:System.InvalidOperationException">The password character specified when setting this property is the same as the current prompt character, <see cref="P:System.ComponentModel.MaskedTextProvider.PromptChar" />. The two are required to be different.</exception>
		/// <exception cref="T:System.ArgumentException">The character specified when setting this property is not a valid password character, as determined by the <see cref="M:System.ComponentModel.MaskedTextProvider.IsValidPasswordChar(System.Char)" /> method.</exception>
		public char PasswordChar
		{
			get
			{
				return this._passwordChar;
			}
			set
			{
				if (value == this._promptChar)
				{
					throw new InvalidOperationException("The PasswordChar and PromptChar values cannot be the same.");
				}
				if (!MaskedTextProvider.IsValidPasswordChar(value) && value != '\0')
				{
					throw new ArgumentException("The specified character value is not allowed for this property.");
				}
				if (value != this._passwordChar)
				{
					this._passwordChar = value;
				}
			}
		}

		/// <summary>Gets or sets the character used to represent the absence of user input for all available edit positions.</summary>
		/// <returns>The character used to prompt the user for input. The default is an underscore (_).</returns>
		/// <exception cref="T:System.InvalidOperationException">The prompt character specified when setting this property is the same as the current password character, <see cref="P:System.ComponentModel.MaskedTextProvider.PasswordChar" />. The two are required to be different.</exception>
		/// <exception cref="T:System.ArgumentException">The character specified when setting this property is not a valid password character, as determined by the <see cref="M:System.ComponentModel.MaskedTextProvider.IsValidPasswordChar(System.Char)" /> method.</exception>
		public char PromptChar
		{
			get
			{
				return this._promptChar;
			}
			set
			{
				if (value == this._passwordChar)
				{
					throw new InvalidOperationException("The PasswordChar and PromptChar values cannot be the same.");
				}
				if (!MaskedTextProvider.IsPrintableChar(value))
				{
					throw new ArgumentException("The specified character value is not allowed for this property.");
				}
				if (value != this._promptChar)
				{
					this._promptChar = value;
					for (int i = 0; i < this._testString.Length; i++)
					{
						MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
						if (this.IsEditPosition(i) && !charDescriptor.IsAssigned)
						{
							this._testString[i] = this._promptChar;
						}
					}
				}
			}
		}

		/// <summary>Gets or sets a value that determines how an input character that matches the prompt character should be handled.</summary>
		/// <returns>
		///   <see langword="true" /> if the prompt character entered as input causes the current editable position in the mask to be reset; otherwise, <see langword="false" /> to indicate that the prompt character is to be processed as a normal input character. The default is <see langword="true" />.</returns>
		public bool ResetOnPrompt
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_RESET_ON_PROMPT];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_RESET_ON_PROMPT] = value;
			}
		}

		/// <summary>Gets or sets a value that determines how a space input character should be handled.</summary>
		/// <returns>
		///   <see langword="true" /> if the space input character causes the current editable position in the mask to be reset; otherwise, <see langword="false" /> to indicate that it is to be processed as a normal input character. The default is <see langword="true" />.</returns>
		public bool ResetOnSpace
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_SKIP_SPACE];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_SKIP_SPACE] = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether literal character positions in the mask can be overwritten by their same values.</summary>
		/// <returns>
		///   <see langword="true" /> to allow literals to be added back; otherwise, <see langword="false" /> to not allow the user to overwrite literal characters. The default is <see langword="true" />.</returns>
		public bool SkipLiterals
		{
			get
			{
				return this._flagState[MaskedTextProvider.s_RESET_ON_LITERALS];
			}
			set
			{
				this._flagState[MaskedTextProvider.s_RESET_ON_LITERALS] = value;
			}
		}

		/// <summary>Gets the element at the specified position in the formatted string.</summary>
		/// <param name="index">A zero-based index of the element to retrieve.</param>
		/// <returns>The <see cref="T:System.Char" /> at the specified position in the formatted string.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">
		///   <paramref name="index" /> is less than zero or greater than or equal to the <see cref="P:System.ComponentModel.MaskedTextProvider.Length" /> of the mask.</exception>
		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= this._testString.Length)
				{
					throw new IndexOutOfRangeException(index.ToString(CultureInfo.CurrentCulture));
				}
				return this._testString[index];
			}
		}

		/// <summary>Adds the specified input character to the end of the formatted string.</summary>
		/// <param name="input">A <see cref="T:System.Char" /> value to be appended to the formatted string.</param>
		/// <returns>
		///   <see langword="true" /> if the input character was added successfully; otherwise <see langword="false" />.</returns>
		public bool Add(char input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Add(input, out num, out maskedTextResultHint);
		}

		/// <summary>Adds the specified input character to the end of the formatted string, and then outputs position and descriptive information.</summary>
		/// <param name="input">A <see cref="T:System.Char" /> value to be appended to the formatted string.</param>
		/// <param name="testPosition">The zero-based position in the formatted string where the attempt was made to add the character. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the input character was added successfully; otherwise <see langword="false" />.</returns>
		public bool Add(char input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			int lastAssignedPosition = this.LastAssignedPosition;
			if (lastAssignedPosition == this._testString.Length - 1)
			{
				testPosition = this._testString.Length;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			testPosition = lastAssignedPosition + 1;
			testPosition = this.FindEditPositionFrom(testPosition, true);
			if (testPosition == -1)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = this._testString.Length;
				return false;
			}
			return this.TestSetChar(input, testPosition, out resultHint);
		}

		/// <summary>Adds the characters in the specified input string to the end of the formatted string.</summary>
		/// <param name="input">A <see cref="T:System.String" /> containing character values to be appended to the formatted string.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters from the input string were added successfully; otherwise <see langword="false" /> to indicate that no characters were added.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> parameter is <see langword="null" />.</exception>
		public bool Add(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Add(input, out num, out maskedTextResultHint);
		}

		/// <summary>Adds the characters in the specified input string to the end of the formatted string, and then outputs position and descriptive information.</summary>
		/// <param name="input">A <see cref="T:System.String" /> containing character values to be appended to the formatted string.</param>
		/// <param name="testPosition">The zero-based position in the formatted string where the attempt was made to add the character. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters from the input string were added successfully; otherwise <see langword="false" /> to indicate that no characters were added.</returns>
		public bool Add(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			testPosition = this.LastAssignedPosition + 1;
			if (input.Length == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			return this.TestSetString(input, testPosition, out testPosition, out resultHint);
		}

		/// <summary>Clears all the editable input characters from the formatted string, replacing them with prompt characters.</summary>
		public void Clear()
		{
			MaskedTextResultHint maskedTextResultHint;
			this.Clear(out maskedTextResultHint);
		}

		/// <summary>Clears all the editable input characters from the formatted string, replacing them with prompt characters, and then outputs descriptive information.</summary>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the operation. An output parameter.</param>
		public void Clear(out MaskedTextResultHint resultHint)
		{
			if (this.AssignedEditPositionCount == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				return;
			}
			resultHint = MaskedTextResultHint.Success;
			for (int i = 0; i < this._testString.Length; i++)
			{
				this.ResetChar(i);
			}
		}

		/// <summary>Returns the position of the first assigned editable position after the specified position using the specified search direction.</summary>
		/// <param name="position">The zero-based position in the formatted string to start the search.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first assigned editable position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindAssignedEditPositionFrom(int position, bool direction)
		{
			if (this.AssignedEditPositionCount == 0)
			{
				return -1;
			}
			int startPosition;
			int endPosition;
			if (direction)
			{
				startPosition = position;
				endPosition = this._testString.Length - 1;
			}
			else
			{
				startPosition = 0;
				endPosition = position;
			}
			return this.FindAssignedEditPositionInRange(startPosition, endPosition, direction);
		}

		/// <summary>Returns the position of the first assigned editable position between the specified positions using the specified search direction.</summary>
		/// <param name="startPosition">The zero-based position in the formatted string where the search starts.</param>
		/// <param name="endPosition">The zero-based position in the formatted string where the search ends.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first assigned editable position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindAssignedEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			if (this.AssignedEditPositionCount == 0)
			{
				return -1;
			}
			return this.FindEditPositionInRange(startPosition, endPosition, direction, 2);
		}

		/// <summary>Returns the position of the first editable position after the specified position using the specified search direction.</summary>
		/// <param name="position">The zero-based position in the formatted string to start the search.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first editable position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindEditPositionFrom(int position, bool direction)
		{
			int startPosition;
			int endPosition;
			if (direction)
			{
				startPosition = position;
				endPosition = this._testString.Length - 1;
			}
			else
			{
				startPosition = 0;
				endPosition = position;
			}
			return this.FindEditPositionInRange(startPosition, endPosition, direction);
		}

		/// <summary>Returns the position of the first editable position between the specified positions using the specified search direction.</summary>
		/// <param name="startPosition">The zero-based position in the formatted string where the search starts.</param>
		/// <param name="endPosition">The zero-based position in the formatted string where the search ends.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first editable position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			MaskedTextProvider.CharType charTypeFlags = MaskedTextProvider.CharType.EditOptional | MaskedTextProvider.CharType.EditRequired;
			return this.FindPositionInRange(startPosition, endPosition, direction, charTypeFlags);
		}

		private int FindEditPositionInRange(int startPosition, int endPosition, bool direction, byte assignedStatus)
		{
			int num;
			for (;;)
			{
				num = this.FindEditPositionInRange(startPosition, endPosition, direction);
				if (num == -1)
				{
					return -1;
				}
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[num];
				if (assignedStatus != 1)
				{
					if (assignedStatus != 2)
					{
						break;
					}
					if (charDescriptor.IsAssigned)
					{
						return num;
					}
				}
				else if (!charDescriptor.IsAssigned)
				{
					return num;
				}
				if (direction)
				{
					startPosition++;
				}
				else
				{
					endPosition--;
				}
				if (startPosition > endPosition)
				{
					return -1;
				}
			}
			return num;
		}

		/// <summary>Returns the position of the first non-editable position after the specified position using the specified search direction.</summary>
		/// <param name="position">The zero-based position in the formatted string to start the search.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first literal position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindNonEditPositionFrom(int position, bool direction)
		{
			int startPosition;
			int endPosition;
			if (direction)
			{
				startPosition = position;
				endPosition = this._testString.Length - 1;
			}
			else
			{
				startPosition = 0;
				endPosition = position;
			}
			return this.FindNonEditPositionInRange(startPosition, endPosition, direction);
		}

		/// <summary>Returns the position of the first non-editable position between the specified positions using the specified search direction.</summary>
		/// <param name="startPosition">The zero-based position in the formatted string where the search starts.</param>
		/// <param name="endPosition">The zero-based position in the formatted string where the search ends.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first literal position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindNonEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			MaskedTextProvider.CharType charTypeFlags = MaskedTextProvider.CharType.Separator | MaskedTextProvider.CharType.Literal;
			return this.FindPositionInRange(startPosition, endPosition, direction, charTypeFlags);
		}

		private int FindPositionInRange(int startPosition, int endPosition, bool direction, MaskedTextProvider.CharType charTypeFlags)
		{
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (endPosition >= this._testString.Length)
			{
				endPosition = this._testString.Length - 1;
			}
			if (startPosition > endPosition)
			{
				return -1;
			}
			while (startPosition <= endPosition)
			{
				int num;
				if (!direction)
				{
					endPosition = (num = endPosition) - 1;
				}
				else
				{
					startPosition = (num = startPosition) + 1;
				}
				int num2 = num;
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[num2];
				if ((charDescriptor.CharType & charTypeFlags) == charDescriptor.CharType)
				{
					return num2;
				}
			}
			return -1;
		}

		/// <summary>Returns the position of the first unassigned editable position after the specified position using the specified search direction.</summary>
		/// <param name="position">The zero-based position in the formatted string to start the search.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first unassigned editable position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindUnassignedEditPositionFrom(int position, bool direction)
		{
			int startPosition;
			int endPosition;
			if (direction)
			{
				startPosition = position;
				endPosition = this._testString.Length - 1;
			}
			else
			{
				startPosition = 0;
				endPosition = position;
			}
			return this.FindEditPositionInRange(startPosition, endPosition, direction, 1);
		}

		/// <summary>Returns the position of the first unassigned editable position between the specified positions using the specified search direction.</summary>
		/// <param name="startPosition">The zero-based position in the formatted string where the search starts.</param>
		/// <param name="endPosition">The zero-based position in the formatted string where the search ends.</param>
		/// <param name="direction">A <see cref="T:System.Boolean" /> indicating the search direction; either <see langword="true" /> to search forward or <see langword="false" /> to search backward.</param>
		/// <returns>If successful, an <see cref="T:System.Int32" /> representing the zero-based position of the first unassigned editable position encountered; otherwise <see cref="P:System.ComponentModel.MaskedTextProvider.InvalidIndex" />.</returns>
		public int FindUnassignedEditPositionInRange(int startPosition, int endPosition, bool direction)
		{
			for (;;)
			{
				int num = this.FindEditPositionInRange(startPosition, endPosition, direction, 0);
				if (num == -1)
				{
					break;
				}
				if (!this._stringDescriptor[num].IsAssigned)
				{
					return num;
				}
				if (direction)
				{
					startPosition++;
				}
				else
				{
					endPosition--;
				}
			}
			return -1;
		}

		/// <summary>Determines whether the specified <see cref="T:System.ComponentModel.MaskedTextResultHint" /> denotes success or failure.</summary>
		/// <param name="hint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> value typically obtained as an output parameter from a previous operation.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.ComponentModel.MaskedTextResultHint" /> value represents a success; otherwise, <see langword="false" /> if it represents failure.</returns>
		public static bool GetOperationResultFromHint(MaskedTextResultHint hint)
		{
			return hint > MaskedTextResultHint.Unknown;
		}

		/// <summary>Inserts the specified character at the specified position within the formatted string.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> to be inserted.</param>
		/// <param name="position">The zero-based position in the formatted string to insert the character.</param>
		/// <returns>
		///   <see langword="true" /> if the insertion was successful; otherwise, <see langword="false" />.</returns>
		public bool InsertAt(char input, int position)
		{
			return position >= 0 && position < this._testString.Length && this.InsertAt(input.ToString(), position);
		}

		/// <summary>Inserts the specified character at the specified position within the formatted string, returning the last insertion position and the status of the operation.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> to be inserted.</param>
		/// <param name="position">The zero-based position in the formatted string to insert the character.</param>
		/// <param name="testPosition">If the method is successful, the last position where a character was inserted; otherwise, the first position where the insertion failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the insertion operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the insertion was successful; otherwise, <see langword="false" />.</returns>
		public bool InsertAt(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			return this.InsertAt(input.ToString(), position, out testPosition, out resultHint);
		}

		/// <summary>Inserts the specified string at a specified position within the formatted string.</summary>
		/// <param name="input">The <see cref="T:System.String" /> to be inserted.</param>
		/// <param name="position">The zero-based position in the formatted string to insert the input string.</param>
		/// <returns>
		///   <see langword="true" /> if the insertion was successful; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> parameter is <see langword="null" />.</exception>
		public bool InsertAt(string input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.InsertAt(input, position, out num, out maskedTextResultHint);
		}

		/// <summary>Inserts the specified string at a specified position within the formatted string, returning the last insertion position and the status of the operation.</summary>
		/// <param name="input">The <see cref="T:System.String" /> to be inserted.</param>
		/// <param name="position">The zero-based position in the formatted string to insert the input string.</param>
		/// <param name="testPosition">If the method is successful, the last position where a character was inserted; otherwise, the first position where the insertion failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the insertion operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the insertion was successful; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> parameter is <see langword="null" />.</exception>
		public bool InsertAt(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (position < 0 || position >= this._testString.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			return this.InsertAtInt(input, position, out testPosition, out resultHint, false);
		}

		private bool InsertAtInt(string input, int position, out int testPosition, out MaskedTextResultHint resultHint, bool testOnly)
		{
			if (input.Length == 0)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			if (!this.TestString(input, position, out testPosition, out resultHint))
			{
				return false;
			}
			int i = this.FindEditPositionFrom(position, true);
			bool flag = this.FindAssignedEditPositionInRange(i, testPosition, true) != -1;
			int lastAssignedPosition = this.LastAssignedPosition;
			if (flag && testPosition == this._testString.Length - 1)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = this._testString.Length;
				return false;
			}
			int num = this.FindEditPositionFrom(testPosition + 1, true);
			if (flag)
			{
				MaskedTextResultHint maskedTextResultHint = MaskedTextResultHint.Unknown;
				while (num != -1)
				{
					if (this._stringDescriptor[i].IsAssigned && !this.TestChar(this._testString[i], num, out maskedTextResultHint))
					{
						resultHint = maskedTextResultHint;
						testPosition = num;
						return false;
					}
					if (i != lastAssignedPosition)
					{
						i = this.FindEditPositionFrom(i + 1, true);
						num = this.FindEditPositionFrom(num + 1, true);
					}
					else
					{
						if (maskedTextResultHint > resultHint)
						{
							resultHint = maskedTextResultHint;
							goto IL_EF;
						}
						goto IL_EF;
					}
				}
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = this._testString.Length;
				return false;
			}
			IL_EF:
			if (testOnly)
			{
				return true;
			}
			if (flag)
			{
				while (i >= position)
				{
					if (this._stringDescriptor[i].IsAssigned)
					{
						this.SetChar(this._testString[i], num);
					}
					else
					{
						this.ResetChar(num);
					}
					num = this.FindEditPositionFrom(num - 1, false);
					i = this.FindEditPositionFrom(i - 1, false);
				}
			}
			this.SetString(input, position);
			return true;
		}

		private static bool IsAscii(char c)
		{
			return c >= '!' && c <= '~';
		}

		private static bool IsAciiAlphanumeric(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		private static bool IsAlphanumeric(char c)
		{
			return char.IsLetter(c) || char.IsDigit(c);
		}

		private static bool IsAsciiLetter(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		/// <summary>Determines whether the specified position is available for assignment.</summary>
		/// <param name="position">The zero-based position in the mask to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified position in the formatted string is editable and has not been assigned to yet; otherwise <see langword="false" />.</returns>
		public bool IsAvailablePosition(int position)
		{
			if (position < 0 || position >= this._testString.Length)
			{
				return false;
			}
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			return MaskedTextProvider.IsEditPosition(charDescriptor) && !charDescriptor.IsAssigned;
		}

		/// <summary>Determines whether the specified position is editable.</summary>
		/// <param name="position">The zero-based position in the mask to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified position in the formatted string is editable; otherwise <see langword="false" />.</returns>
		public bool IsEditPosition(int position)
		{
			return position >= 0 && position < this._testString.Length && MaskedTextProvider.IsEditPosition(this._stringDescriptor[position]);
		}

		private static bool IsEditPosition(MaskedTextProvider.CharDescriptor charDescriptor)
		{
			return charDescriptor.CharType == MaskedTextProvider.CharType.EditRequired || charDescriptor.CharType == MaskedTextProvider.CharType.EditOptional;
		}

		private static bool IsLiteralPosition(MaskedTextProvider.CharDescriptor charDescriptor)
		{
			return charDescriptor.CharType == MaskedTextProvider.CharType.Literal || charDescriptor.CharType == MaskedTextProvider.CharType.Separator;
		}

		private static bool IsPrintableChar(char c)
		{
			return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ';
		}

		/// <summary>Determines whether the specified character is a valid input character.</summary>
		/// <param name="c">The <see cref="T:System.Char" /> value to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified character contains a valid input value; otherwise <see langword="false" />.</returns>
		public static bool IsValidInputChar(char c)
		{
			return MaskedTextProvider.IsPrintableChar(c);
		}

		/// <summary>Determines whether the specified character is a valid mask character.</summary>
		/// <param name="c">The <see cref="T:System.Char" /> value to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified character contains a valid mask value; otherwise <see langword="false" />.</returns>
		public static bool IsValidMaskChar(char c)
		{
			return MaskedTextProvider.IsPrintableChar(c);
		}

		/// <summary>Determines whether the specified character is a valid password character.</summary>
		/// <param name="c">The <see cref="T:System.Char" /> value to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified character contains a valid password value; otherwise <see langword="false" />.</returns>
		public static bool IsValidPasswordChar(char c)
		{
			return MaskedTextProvider.IsPrintableChar(c) || c == '\0';
		}

		/// <summary>Removes the last assigned character from the formatted string.</summary>
		/// <returns>
		///   <see langword="true" /> if the character was successfully removed; otherwise, <see langword="false" />.</returns>
		public bool Remove()
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Remove(out num, out maskedTextResultHint);
		}

		/// <summary>Removes the last assigned character from the formatted string, and then outputs the removal position and descriptive information.</summary>
		/// <param name="testPosition">The zero-based position in the formatted string where the character was actually removed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully removed; otherwise, <see langword="false" />.</returns>
		public bool Remove(out int testPosition, out MaskedTextResultHint resultHint)
		{
			int lastAssignedPosition = this.LastAssignedPosition;
			if (lastAssignedPosition == -1)
			{
				testPosition = 0;
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			this.ResetChar(lastAssignedPosition);
			testPosition = lastAssignedPosition;
			resultHint = MaskedTextResultHint.Success;
			return true;
		}

		/// <summary>Removes the assigned character at the specified position from the formatted string.</summary>
		/// <param name="position">The zero-based position of the assigned character to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully removed; otherwise, <see langword="false" />.</returns>
		public bool RemoveAt(int position)
		{
			return this.RemoveAt(position, position);
		}

		/// <summary>Removes the assigned characters between the specified positions from the formatted string.</summary>
		/// <param name="startPosition">The zero-based index of the first assigned character to remove.</param>
		/// <param name="endPosition">The zero-based index of the last assigned character to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully removed; otherwise, <see langword="false" />.</returns>
		public bool RemoveAt(int startPosition, int endPosition)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.RemoveAt(startPosition, endPosition, out num, out maskedTextResultHint);
		}

		/// <summary>Removes the assigned characters between the specified positions from the formatted string, and then outputs the removal position and descriptive information.</summary>
		/// <param name="startPosition">The zero-based index of the first assigned character to remove.</param>
		/// <param name="endPosition">The zero-based index of the last assigned character to remove.</param>
		/// <param name="testPosition">If successful, the zero-based position in the formatted string of where the characters were actually removed; otherwise, the first position where the operation failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully removed; otherwise, <see langword="false" />.</returns>
		public bool RemoveAt(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (endPosition >= this._testString.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			return this.RemoveAtInt(startPosition, endPosition, out testPosition, out resultHint, false);
		}

		private bool RemoveAtInt(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint, bool testOnly)
		{
			int lastAssignedPosition = this.LastAssignedPosition;
			int num = this.FindEditPositionInRange(startPosition, endPosition, true);
			resultHint = MaskedTextResultHint.NoEffect;
			if (num == -1 || num > lastAssignedPosition)
			{
				testPosition = startPosition;
				return true;
			}
			testPosition = startPosition;
			bool flag = endPosition < lastAssignedPosition;
			if (this.FindAssignedEditPositionInRange(startPosition, endPosition, true) != -1)
			{
				resultHint = MaskedTextResultHint.Success;
			}
			if (flag)
			{
				int num2 = this.FindEditPositionFrom(endPosition + 1, true);
				int num3 = num2;
				startPosition = num;
				MaskedTextResultHint maskedTextResultHint;
				for (;;)
				{
					char c = this._testString[num2];
					MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[num2];
					if ((c != this.PromptChar || charDescriptor.IsAssigned) && !this.TestChar(c, num, out maskedTextResultHint))
					{
						break;
					}
					if (num2 == lastAssignedPosition)
					{
						goto IL_B0;
					}
					num2 = this.FindEditPositionFrom(num2 + 1, true);
					num = this.FindEditPositionFrom(num + 1, true);
				}
				resultHint = maskedTextResultHint;
				testPosition = num;
				return false;
				IL_B0:
				if (MaskedTextResultHint.SideEffect > resultHint)
				{
					resultHint = MaskedTextResultHint.SideEffect;
				}
				if (testOnly)
				{
					return true;
				}
				num2 = num3;
				num = startPosition;
				for (;;)
				{
					char c2 = this._testString[num2];
					MaskedTextProvider.CharDescriptor charDescriptor2 = this._stringDescriptor[num2];
					if (c2 == this.PromptChar && !charDescriptor2.IsAssigned)
					{
						this.ResetChar(num);
					}
					else
					{
						this.SetChar(c2, num);
						this.ResetChar(num2);
					}
					if (num2 == lastAssignedPosition)
					{
						break;
					}
					num2 = this.FindEditPositionFrom(num2 + 1, true);
					num = this.FindEditPositionFrom(num + 1, true);
				}
				startPosition = num + 1;
			}
			if (startPosition <= endPosition)
			{
				this.ResetString(startPosition, endPosition);
			}
			return true;
		}

		/// <summary>Replaces a single character at or beyond the specified position with the specified character value.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> value that replaces the existing value.</param>
		/// <param name="position">The zero-based position to search for the first editable character to replace.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully replaced; otherwise, <see langword="false" />.</returns>
		public bool Replace(char input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Replace(input, position, out num, out maskedTextResultHint);
		}

		/// <summary>Replaces a single character at or beyond the specified position with the specified character value, and then outputs the removal position and descriptive information.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> value that replaces the existing value.</param>
		/// <param name="position">The zero-based position to search for the first editable character to replace.</param>
		/// <param name="testPosition">If successful, the zero-based position in the formatted string where the last character was actually replaced; otherwise, the first position where the operation failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the replacement operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully replaced; otherwise, <see langword="false" />.</returns>
		public bool Replace(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (position < 0 || position >= this._testString.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			testPosition = position;
			if (!this.TestEscapeChar(input, testPosition))
			{
				testPosition = this.FindEditPositionFrom(testPosition, true);
			}
			if (testPosition == -1)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = position;
				return false;
			}
			return this.TestSetChar(input, testPosition, out resultHint);
		}

		/// <summary>Replaces a single character between the specified starting and ending positions with the specified character value, and then outputs the removal position and descriptive information.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> value that replaces the existing value.</param>
		/// <param name="startPosition">The zero-based position in the formatted string where the replacement starts.</param>
		/// <param name="endPosition">The zero-based position in the formatted string where the replacement ends.</param>
		/// <param name="testPosition">If successful, the zero-based position in the formatted string where the last character was actually replaced; otherwise, the first position where the operation failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the replacement operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the character was successfully replaced; otherwise, <see langword="false" />.</returns>
		public bool Replace(char input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (endPosition >= this._testString.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition == endPosition)
			{
				testPosition = startPosition;
				return this.TestSetChar(input, startPosition, out resultHint);
			}
			return this.Replace(input.ToString(), startPosition, endPosition, out testPosition, out resultHint);
		}

		/// <summary>Replaces a range of editable characters starting at the specified position with the specified string.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value used to replace the existing editable characters.</param>
		/// <param name="position">The zero-based position to search for the first editable character to replace.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters were successfully replaced; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> parameter is <see langword="null" />.</exception>
		public bool Replace(string input, int position)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Replace(input, position, out num, out maskedTextResultHint);
		}

		/// <summary>Replaces a range of editable characters starting at the specified position with the specified string, and then outputs the removal position and descriptive information.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value used to replace the existing editable characters.</param>
		/// <param name="position">The zero-based position to search for the first editable character to replace.</param>
		/// <param name="testPosition">If successful, the zero-based position in the formatted string where the last character was actually replaced; otherwise, the first position where the operation failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the replacement operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters were successfully replaced; otherwise, <see langword="false" />.</returns>
		public bool Replace(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (position < 0 || position >= this._testString.Length)
			{
				testPosition = position;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input.Length == 0)
			{
				return this.RemoveAt(position, position, out testPosition, out resultHint);
			}
			return this.TestSetString(input, position, out testPosition, out resultHint);
		}

		/// <summary>Replaces a range of editable characters between the specified starting and ending positions with the specified string, and then outputs the removal position and descriptive information.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value used to replace the existing editable characters.</param>
		/// <param name="startPosition">The zero-based position in the formatted string where the replacement starts.</param>
		/// <param name="endPosition">The zero-based position in the formatted string where the replacement ends.</param>
		/// <param name="testPosition">If successful, the zero-based position in the formatted string where the last character was actually replaced; otherwise, the first position where the operation failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the replacement operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters were successfully replaced; otherwise, <see langword="false" />.</returns>
		public bool Replace(string input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (endPosition >= this._testString.Length)
			{
				testPosition = endPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (startPosition < 0 || startPosition > endPosition)
			{
				testPosition = startPosition;
				resultHint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			if (input.Length == 0)
			{
				return this.RemoveAt(startPosition, endPosition, out testPosition, out resultHint);
			}
			if (!this.TestString(input, startPosition, out testPosition, out resultHint))
			{
				return false;
			}
			if (this.AssignedEditPositionCount > 0)
			{
				if (testPosition < endPosition)
				{
					int num;
					MaskedTextResultHint maskedTextResultHint;
					if (!this.RemoveAtInt(testPosition + 1, endPosition, out num, out maskedTextResultHint, false))
					{
						testPosition = num;
						resultHint = maskedTextResultHint;
						return false;
					}
					if (maskedTextResultHint == MaskedTextResultHint.Success && resultHint != maskedTextResultHint)
					{
						resultHint = MaskedTextResultHint.SideEffect;
					}
				}
				else if (testPosition > endPosition)
				{
					int lastAssignedPosition = this.LastAssignedPosition;
					int i = testPosition + 1;
					int num2 = endPosition + 1;
					MaskedTextResultHint maskedTextResultHint;
					for (;;)
					{
						num2 = this.FindEditPositionFrom(num2, true);
						i = this.FindEditPositionFrom(i, true);
						if (i == -1)
						{
							goto Block_12;
						}
						if (!this.TestChar(this._testString[num2], i, out maskedTextResultHint))
						{
							goto Block_13;
						}
						if (maskedTextResultHint == MaskedTextResultHint.Success && resultHint != maskedTextResultHint)
						{
							resultHint = MaskedTextResultHint.Success;
						}
						if (num2 == lastAssignedPosition)
						{
							break;
						}
						num2++;
						i++;
					}
					while (i > testPosition)
					{
						this.SetChar(this._testString[num2], i);
						num2 = this.FindEditPositionFrom(num2 - 1, false);
						i = this.FindEditPositionFrom(i - 1, false);
					}
					goto IL_162;
					Block_12:
					testPosition = this._testString.Length;
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
					Block_13:
					testPosition = i;
					resultHint = maskedTextResultHint;
					return false;
				}
			}
			IL_162:
			this.SetString(input, startPosition);
			return true;
		}

		private void ResetChar(int testPosition)
		{
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[testPosition];
			if (this.IsEditPosition(testPosition) && charDescriptor.IsAssigned)
			{
				charDescriptor.IsAssigned = false;
				this._testString[testPosition] = this._promptChar;
				int assignedEditPositionCount = this.AssignedEditPositionCount;
				this.AssignedEditPositionCount = assignedEditPositionCount - 1;
				if (charDescriptor.CharType == MaskedTextProvider.CharType.EditRequired)
				{
					this._requiredCharCount--;
				}
			}
		}

		private void ResetString(int startPosition, int endPosition)
		{
			startPosition = this.FindAssignedEditPositionFrom(startPosition, true);
			if (startPosition != -1)
			{
				endPosition = this.FindAssignedEditPositionFrom(endPosition, false);
				while (startPosition <= endPosition)
				{
					startPosition = this.FindAssignedEditPositionFrom(startPosition, true);
					this.ResetChar(startPosition);
					startPosition++;
				}
			}
		}

		/// <summary>Sets the formatted string to the specified input string.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value used to set the formatted string.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters were successfully set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> parameter is <see langword="null" />.</exception>
		public bool Set(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.Set(input, out num, out maskedTextResultHint);
		}

		/// <summary>Sets the formatted string to the specified input string, and then outputs the removal position and descriptive information.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value used to set the formatted string.</param>
		/// <param name="testPosition">If successful, the zero-based position in the formatted string where the last character was actually set; otherwise, the first position where the operation failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the set operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if all the characters were successfully set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> parameter is <see langword="null" />.</exception>
		public bool Set(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			resultHint = MaskedTextResultHint.Unknown;
			testPosition = 0;
			if (input.Length == 0)
			{
				this.Clear(out resultHint);
				return true;
			}
			if (!this.TestSetString(input, testPosition, out testPosition, out resultHint))
			{
				return false;
			}
			int num = this.FindAssignedEditPositionFrom(testPosition + 1, true);
			if (num != -1)
			{
				this.ResetString(num, this._testString.Length - 1);
			}
			return true;
		}

		private void SetChar(char input, int position)
		{
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			this.SetChar(input, position, charDescriptor);
		}

		private void SetChar(char input, int position, MaskedTextProvider.CharDescriptor charDescriptor)
		{
			MaskedTextProvider.CharDescriptor charDescriptor2 = this._stringDescriptor[position];
			if (this.TestEscapeChar(input, position, charDescriptor))
			{
				this.ResetChar(position);
				return;
			}
			if (char.IsLetter(input))
			{
				if (char.IsUpper(input))
				{
					if (charDescriptor.CaseConversion == MaskedTextProvider.CaseConversion.ToLower)
					{
						input = this.Culture.TextInfo.ToLower(input);
					}
				}
				else if (charDescriptor.CaseConversion == MaskedTextProvider.CaseConversion.ToUpper)
				{
					input = this.Culture.TextInfo.ToUpper(input);
				}
			}
			this._testString[position] = input;
			if (!charDescriptor.IsAssigned)
			{
				charDescriptor.IsAssigned = true;
				int assignedEditPositionCount = this.AssignedEditPositionCount;
				this.AssignedEditPositionCount = assignedEditPositionCount + 1;
				if (charDescriptor.CharType == MaskedTextProvider.CharType.EditRequired)
				{
					this._requiredCharCount++;
				}
			}
		}

		private void SetString(string input, int testPosition)
		{
			foreach (char input2 in input)
			{
				if (!this.TestEscapeChar(input2, testPosition))
				{
					testPosition = this.FindEditPositionFrom(testPosition, true);
				}
				this.SetChar(input2, testPosition);
				testPosition++;
			}
		}

		private bool TestChar(char input, int position, out MaskedTextResultHint resultHint)
		{
			if (!MaskedTextProvider.IsPrintableChar(input))
			{
				resultHint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[position];
			if (MaskedTextProvider.IsLiteralPosition(charDescriptor))
			{
				if (this.SkipLiterals && input == this._testString[position])
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
					return true;
				}
				resultHint = MaskedTextResultHint.NonEditPosition;
				return false;
			}
			else
			{
				if (input == this._promptChar)
				{
					if (this.ResetOnPrompt)
					{
						if (MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned)
						{
							resultHint = MaskedTextResultHint.SideEffect;
						}
						else
						{
							resultHint = MaskedTextResultHint.CharacterEscaped;
						}
						return true;
					}
					if (!this.AllowPromptAsInput)
					{
						resultHint = MaskedTextResultHint.PromptCharNotAllowed;
						return false;
					}
				}
				if (input == ' ' && this.ResetOnSpace)
				{
					if (MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned)
					{
						resultHint = MaskedTextResultHint.SideEffect;
					}
					else
					{
						resultHint = MaskedTextResultHint.CharacterEscaped;
					}
					return true;
				}
				char c = this.Mask[charDescriptor.MaskPosition];
				if (c <= '0')
				{
					if (c != '#')
					{
						if (c != '&')
						{
							if (c == '0')
							{
								if (!char.IsDigit(input))
								{
									resultHint = MaskedTextResultHint.DigitExpected;
									return false;
								}
							}
						}
						else if (!MaskedTextProvider.IsAscii(input) && this.AsciiOnly)
						{
							resultHint = MaskedTextResultHint.AsciiCharacterExpected;
							return false;
						}
					}
					else if (!char.IsDigit(input) && input != '-' && input != '+' && input != ' ')
					{
						resultHint = MaskedTextResultHint.DigitExpected;
						return false;
					}
				}
				else if (c <= 'C')
				{
					if (c != '9')
					{
						switch (c)
						{
						case '?':
							if (!char.IsLetter(input) && input != ' ')
							{
								resultHint = MaskedTextResultHint.LetterExpected;
								return false;
							}
							if (!MaskedTextProvider.IsAsciiLetter(input) && this.AsciiOnly)
							{
								resultHint = MaskedTextResultHint.AsciiCharacterExpected;
								return false;
							}
							break;
						case 'A':
							if (!MaskedTextProvider.IsAlphanumeric(input))
							{
								resultHint = MaskedTextResultHint.AlphanumericCharacterExpected;
								return false;
							}
							if (!MaskedTextProvider.IsAciiAlphanumeric(input) && this.AsciiOnly)
							{
								resultHint = MaskedTextResultHint.AsciiCharacterExpected;
								return false;
							}
							break;
						case 'C':
							if (!MaskedTextProvider.IsAscii(input) && this.AsciiOnly && input != ' ')
							{
								resultHint = MaskedTextResultHint.AsciiCharacterExpected;
								return false;
							}
							break;
						}
					}
					else if (!char.IsDigit(input) && input != ' ')
					{
						resultHint = MaskedTextResultHint.DigitExpected;
						return false;
					}
				}
				else if (c != 'L')
				{
					if (c == 'a')
					{
						if (!MaskedTextProvider.IsAlphanumeric(input) && input != ' ')
						{
							resultHint = MaskedTextResultHint.AlphanumericCharacterExpected;
							return false;
						}
						if (!MaskedTextProvider.IsAciiAlphanumeric(input) && this.AsciiOnly)
						{
							resultHint = MaskedTextResultHint.AsciiCharacterExpected;
							return false;
						}
					}
				}
				else
				{
					if (!char.IsLetter(input))
					{
						resultHint = MaskedTextResultHint.LetterExpected;
						return false;
					}
					if (!MaskedTextProvider.IsAsciiLetter(input) && this.AsciiOnly)
					{
						resultHint = MaskedTextResultHint.AsciiCharacterExpected;
						return false;
					}
				}
				if (input == this._testString[position] && charDescriptor.IsAssigned)
				{
					resultHint = MaskedTextResultHint.NoEffect;
				}
				else
				{
					resultHint = MaskedTextResultHint.Success;
				}
				return true;
			}
		}

		private bool TestEscapeChar(char input, int position)
		{
			MaskedTextProvider.CharDescriptor charDex = this._stringDescriptor[position];
			return this.TestEscapeChar(input, position, charDex);
		}

		private bool TestEscapeChar(char input, int position, MaskedTextProvider.CharDescriptor charDex)
		{
			if (MaskedTextProvider.IsLiteralPosition(charDex))
			{
				return this.SkipLiterals && input == this._testString[position];
			}
			return (this.ResetOnPrompt && input == this._promptChar) || (this.ResetOnSpace && input == ' ');
		}

		private bool TestSetChar(char input, int position, out MaskedTextResultHint resultHint)
		{
			if (this.TestChar(input, position, out resultHint))
			{
				if (resultHint == MaskedTextResultHint.Success || resultHint == MaskedTextResultHint.SideEffect)
				{
					this.SetChar(input, position);
				}
				return true;
			}
			return false;
		}

		private bool TestSetString(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			if (this.TestString(input, position, out testPosition, out resultHint))
			{
				this.SetString(input, position);
				return true;
			}
			return false;
		}

		private bool TestString(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
		{
			resultHint = MaskedTextResultHint.Unknown;
			testPosition = position;
			if (input.Length == 0)
			{
				return true;
			}
			MaskedTextResultHint maskedTextResultHint = resultHint;
			foreach (char input2 in input)
			{
				if (testPosition >= this._testString.Length)
				{
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
				}
				if (!this.TestEscapeChar(input2, testPosition))
				{
					testPosition = this.FindEditPositionFrom(testPosition, true);
					if (testPosition == -1)
					{
						testPosition = this._testString.Length;
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						return false;
					}
				}
				if (!this.TestChar(input2, testPosition, out maskedTextResultHint))
				{
					resultHint = maskedTextResultHint;
					return false;
				}
				if (maskedTextResultHint > resultHint)
				{
					resultHint = maskedTextResultHint;
				}
				testPosition++;
			}
			testPosition--;
			return true;
		}

		/// <summary>Returns the formatted string in a displayable form.</summary>
		/// <returns>The formatted <see cref="T:System.String" /> that includes prompts and mask literals.</returns>
		public string ToDisplayString()
		{
			if (!this.IsPassword || this.AssignedEditPositionCount == 0)
			{
				return this._testString.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder(this._testString.Length);
			for (int i = 0; i < this._testString.Length; i++)
			{
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
				stringBuilder.Append((MaskedTextProvider.IsEditPosition(charDescriptor) && charDescriptor.IsAssigned) ? this._passwordChar : this._testString[i]);
			}
			return stringBuilder.ToString();
		}

		/// <summary>Returns the formatted string that includes all the assigned character values.</summary>
		/// <returns>The formatted <see cref="T:System.String" /> that includes all the assigned character values.</returns>
		public override string ToString()
		{
			return this.ToString(true, this.IncludePrompt, this.IncludeLiterals, 0, this._testString.Length);
		}

		/// <summary>Returns the formatted string, optionally including password characters.</summary>
		/// <param name="ignorePasswordChar">
		///   <see langword="true" /> to return the actual editable characters; otherwise, <see langword="false" /> to indicate that the <see cref="P:System.ComponentModel.MaskedTextProvider.PasswordChar" /> property is to be honored.</param>
		/// <returns>The formatted <see cref="T:System.String" /> that includes literals, prompts, and optionally password characters.</returns>
		public string ToString(bool ignorePasswordChar)
		{
			return this.ToString(ignorePasswordChar, this.IncludePrompt, this.IncludeLiterals, 0, this._testString.Length);
		}

		/// <summary>Returns a substring of the formatted string.</summary>
		/// <param name="startPosition">The zero-based position in the formatted string where the output begins.</param>
		/// <param name="length">The number of characters to return.</param>
		/// <returns>If successful, a substring of the formatted <see cref="T:System.String" />, which includes all the assigned character values; otherwise the <see cref="F:System.String.Empty" /> string.</returns>
		public string ToString(int startPosition, int length)
		{
			return this.ToString(true, this.IncludePrompt, this.IncludeLiterals, startPosition, length);
		}

		/// <summary>Returns a substring of the formatted string, optionally including password characters.</summary>
		/// <param name="ignorePasswordChar">
		///   <see langword="true" /> to return the actual editable characters; otherwise, <see langword="false" /> to indicate that the <see cref="P:System.ComponentModel.MaskedTextProvider.PasswordChar" /> property is to be honored.</param>
		/// <param name="startPosition">The zero-based position in the formatted string where the output begins.</param>
		/// <param name="length">The number of characters to return.</param>
		/// <returns>If successful, a substring of the formatted <see cref="T:System.String" />, which includes literals, prompts, and optionally password characters; otherwise the <see cref="F:System.String.Empty" /> string.</returns>
		public string ToString(bool ignorePasswordChar, int startPosition, int length)
		{
			return this.ToString(ignorePasswordChar, this.IncludePrompt, this.IncludeLiterals, startPosition, length);
		}

		/// <summary>Returns the formatted string, optionally including prompt and literal characters.</summary>
		/// <param name="includePrompt">
		///   <see langword="true" /> to include prompt characters in the return string; otherwise, <see langword="false" />.</param>
		/// <param name="includeLiterals">
		///   <see langword="true" /> to include literal characters in the return string; otherwise, <see langword="false" />.</param>
		/// <returns>The formatted <see cref="T:System.String" /> that includes all the assigned character values and optionally includes literals and prompts.</returns>
		public string ToString(bool includePrompt, bool includeLiterals)
		{
			return this.ToString(true, includePrompt, includeLiterals, 0, this._testString.Length);
		}

		/// <summary>Returns a substring of the formatted string, optionally including prompt and literal characters.</summary>
		/// <param name="includePrompt">
		///   <see langword="true" /> to include prompt characters in the return string; otherwise, <see langword="false" />.</param>
		/// <param name="includeLiterals">
		///   <see langword="true" /> to include literal characters in the return string; otherwise, <see langword="false" />.</param>
		/// <param name="startPosition">The zero-based position in the formatted string where the output begins.</param>
		/// <param name="length">The number of characters to return.</param>
		/// <returns>If successful, a substring of the formatted <see cref="T:System.String" />, which includes all the assigned character values and optionally includes literals and prompts; otherwise the <see cref="F:System.String.Empty" /> string.</returns>
		public string ToString(bool includePrompt, bool includeLiterals, int startPosition, int length)
		{
			return this.ToString(true, includePrompt, includeLiterals, startPosition, length);
		}

		/// <summary>Returns a substring of the formatted string, optionally including prompt, literal, and password characters.</summary>
		/// <param name="ignorePasswordChar">
		///   <see langword="true" /> to return the actual editable characters; otherwise, <see langword="false" /> to indicate that the <see cref="P:System.ComponentModel.MaskedTextProvider.PasswordChar" /> property is to be honored.</param>
		/// <param name="includePrompt">
		///   <see langword="true" /> to include prompt characters in the return string; otherwise, <see langword="false" />.</param>
		/// <param name="includeLiterals">
		///   <see langword="true" /> to return literal characters in the return string; otherwise, <see langword="false" />.</param>
		/// <param name="startPosition">The zero-based position in the formatted string where the output begins.</param>
		/// <param name="length">The number of characters to return.</param>
		/// <returns>If successful, a substring of the formatted <see cref="T:System.String" />, which includes all the assigned character values and optionally includes literals, prompts, and password characters; otherwise the <see cref="F:System.String.Empty" /> string.</returns>
		public string ToString(bool ignorePasswordChar, bool includePrompt, bool includeLiterals, int startPosition, int length)
		{
			if (length <= 0)
			{
				return string.Empty;
			}
			if (startPosition < 0)
			{
				startPosition = 0;
			}
			if (startPosition >= this._testString.Length)
			{
				return string.Empty;
			}
			int num = this._testString.Length - startPosition;
			if (length > num)
			{
				length = num;
			}
			if ((!this.IsPassword || ignorePasswordChar) && (includePrompt && includeLiterals))
			{
				return this._testString.ToString(startPosition, length);
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = startPosition + length - 1;
			if (!includePrompt)
			{
				int num3 = includeLiterals ? this.FindNonEditPositionInRange(startPosition, num2, false) : MaskedTextProvider.InvalidIndex;
				int num4 = this.FindAssignedEditPositionInRange((num3 == MaskedTextProvider.InvalidIndex) ? startPosition : num3, num2, false);
				num2 = ((num4 != MaskedTextProvider.InvalidIndex) ? num4 : num3);
				if (num2 == MaskedTextProvider.InvalidIndex)
				{
					return string.Empty;
				}
			}
			int i = startPosition;
			while (i <= num2)
			{
				char value = this._testString[i];
				MaskedTextProvider.CharDescriptor charDescriptor = this._stringDescriptor[i];
				MaskedTextProvider.CharType charType = charDescriptor.CharType;
				if (charType - MaskedTextProvider.CharType.EditOptional > 1)
				{
					if (charType != MaskedTextProvider.CharType.Separator && charType != MaskedTextProvider.CharType.Literal)
					{
						goto IL_12F;
					}
					if (includeLiterals)
					{
						goto IL_12F;
					}
				}
				else if (charDescriptor.IsAssigned)
				{
					if (!this.IsPassword || ignorePasswordChar)
					{
						goto IL_12F;
					}
					stringBuilder.Append(this._passwordChar);
				}
				else
				{
					if (includePrompt)
					{
						goto IL_12F;
					}
					stringBuilder.Append(' ');
				}
				IL_138:
				i++;
				continue;
				IL_12F:
				stringBuilder.Append(value);
				goto IL_138;
			}
			return stringBuilder.ToString();
		}

		/// <summary>Tests whether the specified character could be set successfully at the specified position.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> value to test.</param>
		/// <param name="position">The position in the mask to test the input character against.</param>
		/// <param name="hint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the specified character is valid for the specified position; otherwise, <see langword="false" />.</returns>
		public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
		{
			hint = MaskedTextResultHint.NoEffect;
			if (position < 0 || position >= this._testString.Length)
			{
				hint = MaskedTextResultHint.PositionOutOfRange;
				return false;
			}
			return this.TestChar(input, position, out hint);
		}

		/// <summary>Tests whether the specified character would be escaped at the specified position.</summary>
		/// <param name="input">The <see cref="T:System.Char" /> value to test.</param>
		/// <param name="position">The position in the mask to test the input character against.</param>
		/// <returns>
		///   <see langword="true" /> if the specified character would be escaped at the specified position; otherwise, <see langword="false" />.</returns>
		public bool VerifyEscapeChar(char input, int position)
		{
			return position >= 0 && position < this._testString.Length && this.TestEscapeChar(input, position);
		}

		/// <summary>Tests whether the specified string could be set successfully.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value to test.</param>
		/// <returns>
		///   <see langword="true" /> if the specified string represents valid input; otherwise, <see langword="false" />.</returns>
		public bool VerifyString(string input)
		{
			int num;
			MaskedTextResultHint maskedTextResultHint;
			return this.VerifyString(input, out num, out maskedTextResultHint);
		}

		/// <summary>Tests whether the specified string could be set successfully, and then outputs position and descriptive information.</summary>
		/// <param name="input">The <see cref="T:System.String" /> value to test.</param>
		/// <param name="testPosition">If successful, the zero-based position of the last character actually tested; otherwise, the first position where the test failed. An output parameter.</param>
		/// <param name="resultHint">A <see cref="T:System.ComponentModel.MaskedTextResultHint" /> that succinctly describes the result of the test operation. An output parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the specified string represents valid input; otherwise, <see langword="false" />.</returns>
		public bool VerifyString(string input, out int testPosition, out MaskedTextResultHint resultHint)
		{
			testPosition = 0;
			if (input == null || input.Length == 0)
			{
				resultHint = MaskedTextResultHint.NoEffect;
				return true;
			}
			return this.TestString(input, 0, out testPosition, out resultHint);
		}

		private const char SPACE_CHAR = ' ';

		private const char DEFAULT_PROMPT_CHAR = '_';

		private const char NULL_PASSWORD_CHAR = '\0';

		private const bool DEFAULT_ALLOW_PROMPT = true;

		private const int INVALID_INDEX = -1;

		private const byte EDIT_ANY = 0;

		private const byte EDIT_UNASSIGNED = 1;

		private const byte EDIT_ASSIGNED = 2;

		private const bool FORWARD = true;

		private const bool BACKWARD = false;

		private static int s_ASCII_ONLY = BitVector32.CreateMask();

		private static int s_ALLOW_PROMPT_AS_INPUT = BitVector32.CreateMask(MaskedTextProvider.s_ASCII_ONLY);

		private static int s_INCLUDE_PROMPT = BitVector32.CreateMask(MaskedTextProvider.s_ALLOW_PROMPT_AS_INPUT);

		private static int s_INCLUDE_LITERALS = BitVector32.CreateMask(MaskedTextProvider.s_INCLUDE_PROMPT);

		private static int s_RESET_ON_PROMPT = BitVector32.CreateMask(MaskedTextProvider.s_INCLUDE_LITERALS);

		private static int s_RESET_ON_LITERALS = BitVector32.CreateMask(MaskedTextProvider.s_RESET_ON_PROMPT);

		private static int s_SKIP_SPACE = BitVector32.CreateMask(MaskedTextProvider.s_RESET_ON_LITERALS);

		private static Type s_maskTextProviderType = typeof(MaskedTextProvider);

		private BitVector32 _flagState;

		private StringBuilder _testString;

		private int _requiredCharCount;

		private int _requiredEditChars;

		private int _optionalEditChars;

		private char _passwordChar;

		private char _promptChar;

		private List<MaskedTextProvider.CharDescriptor> _stringDescriptor;

		private enum CaseConversion
		{
			None,
			ToLower,
			ToUpper
		}

		[Flags]
		private enum CharType
		{
			EditOptional = 1,
			EditRequired = 2,
			Separator = 4,
			Literal = 8,
			Modifier = 16
		}

		private class CharDescriptor
		{
			public CharDescriptor(int maskPos, MaskedTextProvider.CharType charType)
			{
				this.MaskPosition = maskPos;
				this.CharType = charType;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "MaskPosition[{0}] <CaseConversion.{1}><CharType.{2}><IsAssigned: {3}", new object[]
				{
					this.MaskPosition,
					this.CaseConversion,
					this.CharType,
					this.IsAssigned
				});
			}

			public int MaskPosition;

			public MaskedTextProvider.CaseConversion CaseConversion;

			public MaskedTextProvider.CharType CharType;

			public bool IsAssigned;
		}
	}
}
