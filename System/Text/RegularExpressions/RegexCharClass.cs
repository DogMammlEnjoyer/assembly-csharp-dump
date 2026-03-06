using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexCharClass
	{
		public RegexCharClass()
		{
			this._rangelist = new List<RegexCharClass.SingleRange>(6);
			this._canonical = true;
			this._categories = new StringBuilder();
		}

		private RegexCharClass(bool negate, List<RegexCharClass.SingleRange> ranges, StringBuilder categories, RegexCharClass subtraction)
		{
			this._rangelist = ranges;
			this._categories = categories;
			this._canonical = true;
			this._negate = negate;
			this._subtractor = subtraction;
		}

		public bool CanMerge
		{
			get
			{
				return !this._negate && this._subtractor == null;
			}
		}

		public bool Negate
		{
			set
			{
				this._negate = value;
			}
		}

		public void AddChar(char c)
		{
			this.AddRange(c, c);
		}

		public void AddCharClass(RegexCharClass cc)
		{
			if (!cc._canonical)
			{
				this._canonical = false;
			}
			else if (this._canonical && this.RangeCount() > 0 && cc.RangeCount() > 0 && cc.GetRangeAt(0).First <= this.GetRangeAt(this.RangeCount() - 1).Last)
			{
				this._canonical = false;
			}
			for (int i = 0; i < cc.RangeCount(); i++)
			{
				this._rangelist.Add(cc.GetRangeAt(i));
			}
			this._categories.Append(cc._categories.ToString());
		}

		private void AddSet(string set)
		{
			if (this._canonical && this.RangeCount() > 0 && set.Length > 0 && set[0] <= this.GetRangeAt(this.RangeCount() - 1).Last)
			{
				this._canonical = false;
			}
			int i;
			for (i = 0; i < set.Length - 1; i += 2)
			{
				this._rangelist.Add(new RegexCharClass.SingleRange(set[i], set[i + 1] - '\u0001'));
			}
			if (i < set.Length)
			{
				this._rangelist.Add(new RegexCharClass.SingleRange(set[i], char.MaxValue));
			}
		}

		public void AddSubtraction(RegexCharClass sub)
		{
			this._subtractor = sub;
		}

		public void AddRange(char first, char last)
		{
			this._rangelist.Add(new RegexCharClass.SingleRange(first, last));
			if (this._canonical && this._rangelist.Count > 0 && first <= this._rangelist[this._rangelist.Count - 1].Last)
			{
				this._canonical = false;
			}
		}

		public void AddCategoryFromName(string categoryName, bool invert, bool caseInsensitive, string pattern)
		{
			string text;
			if (RegexCharClass.s_definedCategories.TryGetValue(categoryName, out text) && !categoryName.Equals(RegexCharClass.s_internalRegexIgnoreCase))
			{
				if (caseInsensitive && (categoryName.Equals("Ll") || categoryName.Equals("Lu") || categoryName.Equals("Lt")))
				{
					text = RegexCharClass.s_definedCategories[RegexCharClass.s_internalRegexIgnoreCase];
				}
				if (invert)
				{
					text = RegexCharClass.NegateCategory(text);
				}
				this._categories.Append(text);
				return;
			}
			this.AddSet(RegexCharClass.SetFromProperty(categoryName, invert, pattern));
		}

		private void AddCategory(string category)
		{
			this._categories.Append(category);
		}

		public void AddLowercase(CultureInfo culture)
		{
			this._canonical = false;
			int count = this._rangelist.Count;
			for (int i = 0; i < count; i++)
			{
				RegexCharClass.SingleRange singleRange = this._rangelist[i];
				if (singleRange.First == singleRange.Last)
				{
					char c = culture.TextInfo.ToLower(singleRange.First);
					this._rangelist[i] = new RegexCharClass.SingleRange(c, c);
				}
				else
				{
					this.AddLowercaseRange(singleRange.First, singleRange.Last, culture);
				}
			}
		}

		private void AddLowercaseRange(char chMin, char chMax, CultureInfo culture)
		{
			int i = 0;
			int num = RegexCharClass.s_lcTable.Length;
			while (i < num)
			{
				int num2 = (i + num) / 2;
				if (RegexCharClass.s_lcTable[num2].ChMax < chMin)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2;
				}
			}
			if (i >= RegexCharClass.s_lcTable.Length)
			{
				return;
			}
			RegexCharClass.LowerCaseMapping lowerCaseMapping;
			while (i < RegexCharClass.s_lcTable.Length && (lowerCaseMapping = RegexCharClass.s_lcTable[i]).ChMin <= chMax)
			{
				char c;
				if ((c = lowerCaseMapping.ChMin) < chMin)
				{
					c = chMin;
				}
				char c2;
				if ((c2 = lowerCaseMapping.ChMax) > chMax)
				{
					c2 = chMax;
				}
				switch (lowerCaseMapping.LcOp)
				{
				case 0:
					c = (char)lowerCaseMapping.Data;
					c2 = (char)lowerCaseMapping.Data;
					break;
				case 1:
					c += (char)lowerCaseMapping.Data;
					c2 += (char)lowerCaseMapping.Data;
					break;
				case 2:
					c |= '\u0001';
					c2 |= '\u0001';
					break;
				case 3:
					c += (c & '\u0001');
					c2 += (c2 & '\u0001');
					break;
				}
				if (c < chMin || c2 > chMax)
				{
					this.AddRange(c, c2);
				}
				i++;
			}
		}

		public void AddWord(bool ecma, bool negate)
		{
			if (negate)
			{
				if (ecma)
				{
					this.AddSet("\00:A[_`a{İı");
					return;
				}
				this.AddCategory(RegexCharClass.s_notWord);
				return;
			}
			else
			{
				if (ecma)
				{
					this.AddSet("0:A[_`a{İı");
					return;
				}
				this.AddCategory(RegexCharClass.s_word);
				return;
			}
		}

		public void AddSpace(bool ecma, bool negate)
		{
			if (negate)
			{
				if (ecma)
				{
					this.AddSet("\0\t\u000e !");
					return;
				}
				this.AddCategory(RegexCharClass.s_notSpace);
				return;
			}
			else
			{
				if (ecma)
				{
					this.AddSet("\t\u000e !");
					return;
				}
				this.AddCategory(RegexCharClass.s_space);
				return;
			}
		}

		public void AddDigit(bool ecma, bool negate, string pattern)
		{
			if (!ecma)
			{
				this.AddCategoryFromName("Nd", negate, false, pattern);
				return;
			}
			if (negate)
			{
				this.AddSet("\00:");
				return;
			}
			this.AddSet("0:");
		}

		public static string ConvertOldStringsToClass(string set, string category)
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(set.Length + category.Length + 3);
			if (set.Length >= 2 && set[0] == '\0' && set[1] == '\0')
			{
				stringBuilder.Append('\u0001');
				stringBuilder.Append((char)(set.Length - 2));
				stringBuilder.Append((char)category.Length);
				stringBuilder.Append(set.Substring(2));
			}
			else
			{
				stringBuilder.Append('\0');
				stringBuilder.Append((char)set.Length);
				stringBuilder.Append((char)category.Length);
				stringBuilder.Append(set);
			}
			stringBuilder.Append(category);
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		public static char SingletonChar(string set)
		{
			return set[3];
		}

		public static bool IsMergeable(string charClass)
		{
			return !RegexCharClass.IsNegated(charClass) && !RegexCharClass.IsSubtraction(charClass);
		}

		public static bool IsEmpty(string charClass)
		{
			return charClass[2] == '\0' && charClass[0] == '\0' && charClass[1] == '\0' && !RegexCharClass.IsSubtraction(charClass);
		}

		public static bool IsSingleton(string set)
		{
			return set[0] == '\0' && set[2] == '\0' && set[1] == '\u0002' && !RegexCharClass.IsSubtraction(set) && (set[3] == char.MaxValue || set[3] + '\u0001' == set[4]);
		}

		public static bool IsSingletonInverse(string set)
		{
			return set[0] == '\u0001' && set[2] == '\0' && set[1] == '\u0002' && !RegexCharClass.IsSubtraction(set) && (set[3] == char.MaxValue || set[3] + '\u0001' == set[4]);
		}

		private static bool IsSubtraction(string charClass)
		{
			return charClass.Length > (int)('\u0003' + charClass[1] + charClass[2]);
		}

		private static bool IsNegated(string set)
		{
			return set != null && set[0] == '\u0001';
		}

		public static bool IsECMAWordChar(char ch)
		{
			return RegexCharClass.CharInClass(ch, "\0\n\00:A[_`a{İı");
		}

		public static bool IsWordChar(char ch)
		{
			return RegexCharClass.CharInClass(ch, RegexCharClass.WordClass) || ch == '‍' || ch == '‌';
		}

		public static bool CharInClass(char ch, string set)
		{
			return RegexCharClass.CharInClassRecursive(ch, set, 0);
		}

		private static bool CharInClassRecursive(char ch, string set, int start)
		{
			int num = (int)set[start + 1];
			int num2 = (int)set[start + 2];
			int num3 = start + 3 + num + num2;
			bool flag = false;
			if (set.Length > num3)
			{
				flag = RegexCharClass.CharInClassRecursive(ch, set, num3);
			}
			bool flag2 = RegexCharClass.CharInClassInternal(ch, set, start, num, num2);
			if (set[start] == '\u0001')
			{
				flag2 = !flag2;
			}
			return flag2 && !flag;
		}

		private static bool CharInClassInternal(char ch, string set, int start, int mySetLength, int myCategoryLength)
		{
			int num = start + 3;
			int num2 = num + mySetLength;
			while (num != num2)
			{
				int num3 = (num + num2) / 2;
				if (ch < set[num3])
				{
					num2 = num3;
				}
				else
				{
					num = num3 + 1;
				}
			}
			return (num & 1) == (start & 1) || (myCategoryLength != 0 && RegexCharClass.CharInCategory(ch, set, start, mySetLength, myCategoryLength));
		}

		private static bool CharInCategory(char ch, string set, int start, int mySetLength, int myCategoryLength)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
			int i = start + 3 + mySetLength;
			int num = i + myCategoryLength;
			while (i < num)
			{
				int num2 = (int)((short)set[i]);
				if (num2 == 0)
				{
					if (RegexCharClass.CharInCategoryGroup(ch, unicodeCategory, set, ref i))
					{
						return true;
					}
				}
				else if (num2 > 0)
				{
					if (num2 == 100)
					{
						if (char.IsWhiteSpace(ch))
						{
							return true;
						}
						i++;
						continue;
					}
					else
					{
						num2--;
						if (unicodeCategory == (UnicodeCategory)num2)
						{
							return true;
						}
					}
				}
				else if (num2 == -100)
				{
					if (!char.IsWhiteSpace(ch))
					{
						return true;
					}
					i++;
					continue;
				}
				else
				{
					num2 = -1 - num2;
					if (unicodeCategory != (UnicodeCategory)num2)
					{
						return true;
					}
				}
				i++;
			}
			return false;
		}

		private static bool CharInCategoryGroup(char ch, UnicodeCategory chcategory, string category, ref int i)
		{
			i++;
			int num = (int)((short)category[i]);
			if (num > 0)
			{
				bool flag = false;
				while (num != 0)
				{
					if (!flag)
					{
						num--;
						if (chcategory == (UnicodeCategory)num)
						{
							flag = true;
						}
					}
					i++;
					num = (int)((short)category[i]);
				}
				return flag;
			}
			bool flag2 = true;
			while (num != 0)
			{
				if (flag2)
				{
					num = -1 - num;
					if (chcategory == (UnicodeCategory)num)
					{
						flag2 = false;
					}
				}
				i++;
				num = (int)((short)category[i]);
			}
			return flag2;
		}

		private static string NegateCategory(string category)
		{
			if (category == null)
			{
				return null;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire(category.Length);
			foreach (short num in category)
			{
				stringBuilder.Append((char)(-(char)num));
			}
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		public static RegexCharClass Parse(string charClass)
		{
			return RegexCharClass.ParseRecursive(charClass, 0);
		}

		private static RegexCharClass ParseRecursive(string charClass, int start)
		{
			int num = (int)charClass[start + 1];
			int num2 = (int)charClass[start + 2];
			int num3 = start + 3 + num + num2;
			List<RegexCharClass.SingleRange> list = new List<RegexCharClass.SingleRange>(num);
			int i = start + 3;
			int num4 = i + num;
			while (i < num4)
			{
				char first = charClass[i];
				i++;
				char last;
				if (i < num4)
				{
					last = charClass[i] - '\u0001';
				}
				else
				{
					last = char.MaxValue;
				}
				i++;
				list.Add(new RegexCharClass.SingleRange(first, last));
			}
			RegexCharClass subtraction = null;
			if (charClass.Length > num3)
			{
				subtraction = RegexCharClass.ParseRecursive(charClass, num3);
			}
			return new RegexCharClass(charClass[start] == '\u0001', list, new StringBuilder(charClass.Substring(num4, num2)), subtraction);
		}

		private int RangeCount()
		{
			return this._rangelist.Count;
		}

		public string ToStringClass()
		{
			if (!this._canonical)
			{
				this.Canonicalize();
			}
			int num = this._rangelist.Count * 2;
			StringBuilder stringBuilder = StringBuilderCache.Acquire(num + this._categories.Length + 3);
			int num2;
			if (this._negate)
			{
				num2 = 1;
			}
			else
			{
				num2 = 0;
			}
			stringBuilder.Append((char)num2);
			stringBuilder.Append((char)num);
			stringBuilder.Append((char)this._categories.Length);
			for (int i = 0; i < this._rangelist.Count; i++)
			{
				RegexCharClass.SingleRange singleRange = this._rangelist[i];
				stringBuilder.Append(singleRange.First);
				if (singleRange.Last != '￿')
				{
					stringBuilder.Append(singleRange.Last + '\u0001');
				}
			}
			stringBuilder[1] = (char)(stringBuilder.Length - 3);
			stringBuilder.Append(this._categories);
			if (this._subtractor != null)
			{
				stringBuilder.Append(this._subtractor.ToStringClass());
			}
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		private RegexCharClass.SingleRange GetRangeAt(int i)
		{
			return this._rangelist[i];
		}

		private void Canonicalize()
		{
			this._canonical = true;
			this._rangelist.Sort(RegexCharClass.SingleRangeComparer.Instance);
			if (this._rangelist.Count > 1)
			{
				bool flag = false;
				int num = 1;
				int num2 = 0;
				for (;;)
				{
					IL_2F:
					char last = this._rangelist[num2].Last;
					while (num != this._rangelist.Count && last != '￿')
					{
						RegexCharClass.SingleRange singleRange;
						if ((singleRange = this._rangelist[num]).First <= last + '\u0001')
						{
							if (last < singleRange.Last)
							{
								last = singleRange.Last;
							}
							num++;
						}
						else
						{
							IL_8A:
							this._rangelist[num2] = new RegexCharClass.SingleRange(this._rangelist[num2].First, last);
							num2++;
							if (!flag)
							{
								if (num2 < num)
								{
									this._rangelist[num2] = this._rangelist[num];
								}
								num++;
								goto IL_2F;
							}
							goto IL_DA;
						}
					}
					flag = true;
					goto IL_8A;
				}
				IL_DA:
				this._rangelist.RemoveRange(num2, this._rangelist.Count - num2);
			}
		}

		private static string SetFromProperty(string capname, bool invert, string pattern)
		{
			int num = 0;
			int num2 = RegexCharClass.s_propTable.Length;
			while (num != num2)
			{
				int num3 = (num + num2) / 2;
				int num4 = string.Compare(capname, RegexCharClass.s_propTable[num3][0], StringComparison.Ordinal);
				if (num4 < 0)
				{
					num2 = num3;
				}
				else if (num4 > 0)
				{
					num = num3 + 1;
				}
				else
				{
					string text = RegexCharClass.s_propTable[num3][1];
					if (!invert)
					{
						return text;
					}
					if (text[0] == '\0')
					{
						return text.Substring(1);
					}
					return "\0" + text;
				}
			}
			throw new ArgumentException(SR.Format("parsing \"{0}\" - {1}", pattern, SR.Format("Unknown property '{0}'.", capname)));
		}

		private const int FLAGS = 0;

		private const int SETLENGTH = 1;

		private const int CATEGORYLENGTH = 2;

		private const int SETSTART = 3;

		private const string NullCharString = "\0";

		private const char NullChar = '\0';

		private const char LastChar = '￿';

		private const char GroupChar = '\0';

		private const short SpaceConst = 100;

		private const short NotSpaceConst = -100;

		private const char ZeroWidthJoiner = '‍';

		private const char ZeroWidthNonJoiner = '‌';

		private static readonly string s_internalRegexIgnoreCase = "__InternalRegexIgnoreCase__";

		private static readonly string s_space = "d";

		private static readonly string s_notSpace = "ﾜ";

		private static readonly string s_word = "\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0";

		private static readonly string s_notWord = "\0￾￼￻�￿￺￷￭\0";

		public static readonly string SpaceClass = "\0\0\u0001d";

		public static readonly string NotSpaceClass = "\u0001\0\u0001d";

		public static readonly string WordClass = "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0";

		public static readonly string NotWordClass = "\u0001\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0";

		public static readonly string DigitClass = "\0\0\u0001\t";

		public static readonly string NotDigitClass = "\0\0\u0001￷";

		private const string ECMASpaceSet = "\t\u000e !";

		private const string NotECMASpaceSet = "\0\t\u000e !";

		private const string ECMAWordSet = "0:A[_`a{İı";

		private const string NotECMAWordSet = "\00:A[_`a{İı";

		private const string ECMADigitSet = "0:";

		private const string NotECMADigitSet = "\00:";

		public const string ECMASpaceClass = "\0\u0004\0\t\u000e !";

		public const string NotECMASpaceClass = "\u0001\u0004\0\t\u000e !";

		public const string ECMAWordClass = "\0\n\00:A[_`a{İı";

		public const string NotECMAWordClass = "\u0001\n\00:A[_`a{İı";

		public const string ECMADigitClass = "\0\u0002\00:";

		public const string NotECMADigitClass = "\u0001\u0002\00:";

		public const string AnyClass = "\0\u0001\0\0";

		public const string EmptyClass = "\0\0\0";

		private const int DefinedCategoriesCapacity = 38;

		private static readonly Dictionary<string, string> s_definedCategories = new Dictionary<string, string>(38)
		{
			{
				"Cc",
				"\u000f"
			},
			{
				"Cf",
				"\u0010"
			},
			{
				"Cn",
				"\u001e"
			},
			{
				"Co",
				"\u0012"
			},
			{
				"Cs",
				"\u0011"
			},
			{
				"C",
				"\0\u000f\u0010\u001e\u0012\u0011\0"
			},
			{
				"Ll",
				"\u0002"
			},
			{
				"Lm",
				"\u0004"
			},
			{
				"Lo",
				"\u0005"
			},
			{
				"Lt",
				"\u0003"
			},
			{
				"Lu",
				"\u0001"
			},
			{
				"L",
				"\0\u0002\u0004\u0005\u0003\u0001\0"
			},
			{
				"__InternalRegexIgnoreCase__",
				"\0\u0002\u0003\u0001\0"
			},
			{
				"Mc",
				"\a"
			},
			{
				"Me",
				"\b"
			},
			{
				"Mn",
				"\u0006"
			},
			{
				"M",
				"\0\a\b\u0006\0"
			},
			{
				"Nd",
				"\t"
			},
			{
				"Nl",
				"\n"
			},
			{
				"No",
				"\v"
			},
			{
				"N",
				"\0\t\n\v\0"
			},
			{
				"Pc",
				"\u0013"
			},
			{
				"Pd",
				"\u0014"
			},
			{
				"Pe",
				"\u0016"
			},
			{
				"Po",
				"\u0019"
			},
			{
				"Ps",
				"\u0015"
			},
			{
				"Pf",
				"\u0018"
			},
			{
				"Pi",
				"\u0017"
			},
			{
				"P",
				"\0\u0013\u0014\u0016\u0019\u0015\u0018\u0017\0"
			},
			{
				"Sc",
				"\u001b"
			},
			{
				"Sk",
				"\u001c"
			},
			{
				"Sm",
				"\u001a"
			},
			{
				"So",
				"\u001d"
			},
			{
				"S",
				"\0\u001b\u001c\u001a\u001d\0"
			},
			{
				"Zl",
				"\r"
			},
			{
				"Zp",
				"\u000e"
			},
			{
				"Zs",
				"\f"
			},
			{
				"Z",
				"\0\r\u000e\f\0"
			}
		};

		private static readonly string[][] s_propTable = new string[][]
		{
			new string[]
			{
				"IsAlphabeticPresentationForms",
				"ﬀﭐ"
			},
			new string[]
			{
				"IsArabic",
				"؀܀"
			},
			new string[]
			{
				"IsArabicPresentationForms-A",
				"ﭐ︀"
			},
			new string[]
			{
				"IsArabicPresentationForms-B",
				"ﹰ＀"
			},
			new string[]
			{
				"IsArmenian",
				"԰֐"
			},
			new string[]
			{
				"IsArrows",
				"←∀"
			},
			new string[]
			{
				"IsBasicLatin",
				"\0\u0080"
			},
			new string[]
			{
				"IsBengali",
				"ঀ਀"
			},
			new string[]
			{
				"IsBlockElements",
				"▀■"
			},
			new string[]
			{
				"IsBopomofo",
				"㄀㄰"
			},
			new string[]
			{
				"IsBopomofoExtended",
				"ㆠ㇀"
			},
			new string[]
			{
				"IsBoxDrawing",
				"─▀"
			},
			new string[]
			{
				"IsBraillePatterns",
				"⠀⤀"
			},
			new string[]
			{
				"IsBuhid",
				"ᝀᝠ"
			},
			new string[]
			{
				"IsCJKCompatibility",
				"㌀㐀"
			},
			new string[]
			{
				"IsCJKCompatibilityForms",
				"︰﹐"
			},
			new string[]
			{
				"IsCJKCompatibilityIdeographs",
				"豈ﬀ"
			},
			new string[]
			{
				"IsCJKRadicalsSupplement",
				"⺀⼀"
			},
			new string[]
			{
				"IsCJKSymbolsandPunctuation",
				"\u3000぀"
			},
			new string[]
			{
				"IsCJKUnifiedIdeographs",
				"一ꀀ"
			},
			new string[]
			{
				"IsCJKUnifiedIdeographsExtensionA",
				"㐀䷀"
			},
			new string[]
			{
				"IsCherokee",
				"Ꭰ᐀"
			},
			new string[]
			{
				"IsCombiningDiacriticalMarks",
				"̀Ͱ"
			},
			new string[]
			{
				"IsCombiningDiacriticalMarksforSymbols",
				"⃐℀"
			},
			new string[]
			{
				"IsCombiningHalfMarks",
				"︠︰"
			},
			new string[]
			{
				"IsCombiningMarksforSymbols",
				"⃐℀"
			},
			new string[]
			{
				"IsControlPictures",
				"␀⑀"
			},
			new string[]
			{
				"IsCurrencySymbols",
				"₠⃐"
			},
			new string[]
			{
				"IsCyrillic",
				"ЀԀ"
			},
			new string[]
			{
				"IsCyrillicSupplement",
				"Ԁ԰"
			},
			new string[]
			{
				"IsDevanagari",
				"ऀঀ"
			},
			new string[]
			{
				"IsDingbats",
				"✀⟀"
			},
			new string[]
			{
				"IsEnclosedAlphanumerics",
				"①─"
			},
			new string[]
			{
				"IsEnclosedCJKLettersandMonths",
				"㈀㌀"
			},
			new string[]
			{
				"IsEthiopic",
				"ሀᎀ"
			},
			new string[]
			{
				"IsGeneralPunctuation",
				"\u2000⁰"
			},
			new string[]
			{
				"IsGeometricShapes",
				"■☀"
			},
			new string[]
			{
				"IsGeorgian",
				"Ⴀᄀ"
			},
			new string[]
			{
				"IsGreek",
				"ͰЀ"
			},
			new string[]
			{
				"IsGreekExtended",
				"ἀ\u2000"
			},
			new string[]
			{
				"IsGreekandCoptic",
				"ͰЀ"
			},
			new string[]
			{
				"IsGujarati",
				"઀଀"
			},
			new string[]
			{
				"IsGurmukhi",
				"਀઀"
			},
			new string[]
			{
				"IsHalfwidthandFullwidthForms",
				"＀￰"
			},
			new string[]
			{
				"IsHangulCompatibilityJamo",
				"㄰㆐"
			},
			new string[]
			{
				"IsHangulJamo",
				"ᄀሀ"
			},
			new string[]
			{
				"IsHangulSyllables",
				"가ힰ"
			},
			new string[]
			{
				"IsHanunoo",
				"ᜠᝀ"
			},
			new string[]
			{
				"IsHebrew",
				"֐؀"
			},
			new string[]
			{
				"IsHighPrivateUseSurrogates",
				"\udb80\udc00"
			},
			new string[]
			{
				"IsHighSurrogates",
				"\ud800\udb80"
			},
			new string[]
			{
				"IsHiragana",
				"぀゠"
			},
			new string[]
			{
				"IsIPAExtensions",
				"ɐʰ"
			},
			new string[]
			{
				"IsIdeographicDescriptionCharacters",
				"⿰\u3000"
			},
			new string[]
			{
				"IsKanbun",
				"㆐ㆠ"
			},
			new string[]
			{
				"IsKangxiRadicals",
				"⼀⿠"
			},
			new string[]
			{
				"IsKannada",
				"ಀഀ"
			},
			new string[]
			{
				"IsKatakana",
				"゠㄀"
			},
			new string[]
			{
				"IsKatakanaPhoneticExtensions",
				"ㇰ㈀"
			},
			new string[]
			{
				"IsKhmer",
				"ក᠀"
			},
			new string[]
			{
				"IsKhmerSymbols",
				"᧠ᨀ"
			},
			new string[]
			{
				"IsLao",
				"຀ༀ"
			},
			new string[]
			{
				"IsLatin-1Supplement",
				"\u0080Ā"
			},
			new string[]
			{
				"IsLatinExtended-A",
				"Āƀ"
			},
			new string[]
			{
				"IsLatinExtended-B",
				"ƀɐ"
			},
			new string[]
			{
				"IsLatinExtendedAdditional",
				"Ḁἀ"
			},
			new string[]
			{
				"IsLetterlikeSymbols",
				"℀⅐"
			},
			new string[]
			{
				"IsLimbu",
				"ᤀᥐ"
			},
			new string[]
			{
				"IsLowSurrogates",
				"\udc00"
			},
			new string[]
			{
				"IsMalayalam",
				"ഀ඀"
			},
			new string[]
			{
				"IsMathematicalOperators",
				"∀⌀"
			},
			new string[]
			{
				"IsMiscellaneousMathematicalSymbols-A",
				"⟀⟰"
			},
			new string[]
			{
				"IsMiscellaneousMathematicalSymbols-B",
				"⦀⨀"
			},
			new string[]
			{
				"IsMiscellaneousSymbols",
				"☀✀"
			},
			new string[]
			{
				"IsMiscellaneousSymbolsandArrows",
				"⬀Ⰰ"
			},
			new string[]
			{
				"IsMiscellaneousTechnical",
				"⌀␀"
			},
			new string[]
			{
				"IsMongolian",
				"᠀ᢰ"
			},
			new string[]
			{
				"IsMyanmar",
				"ကႠ"
			},
			new string[]
			{
				"IsNumberForms",
				"⅐←"
			},
			new string[]
			{
				"IsOgham",
				"\u1680ᚠ"
			},
			new string[]
			{
				"IsOpticalCharacterRecognition",
				"⑀①"
			},
			new string[]
			{
				"IsOriya",
				"଀஀"
			},
			new string[]
			{
				"IsPhoneticExtensions",
				"ᴀᶀ"
			},
			new string[]
			{
				"IsPrivateUse",
				"豈"
			},
			new string[]
			{
				"IsPrivateUseArea",
				"豈"
			},
			new string[]
			{
				"IsRunic",
				"ᚠᜀ"
			},
			new string[]
			{
				"IsSinhala",
				"඀฀"
			},
			new string[]
			{
				"IsSmallFormVariants",
				"﹐ﹰ"
			},
			new string[]
			{
				"IsSpacingModifierLetters",
				"ʰ̀"
			},
			new string[]
			{
				"IsSpecials",
				"￰"
			},
			new string[]
			{
				"IsSuperscriptsandSubscripts",
				"⁰₠"
			},
			new string[]
			{
				"IsSupplementalArrows-A",
				"⟰⠀"
			},
			new string[]
			{
				"IsSupplementalArrows-B",
				"⤀⦀"
			},
			new string[]
			{
				"IsSupplementalMathematicalOperators",
				"⨀⬀"
			},
			new string[]
			{
				"IsSyriac",
				"܀ݐ"
			},
			new string[]
			{
				"IsTagalog",
				"ᜀᜠ"
			},
			new string[]
			{
				"IsTagbanwa",
				"ᝠក"
			},
			new string[]
			{
				"IsTaiLe",
				"ᥐᦀ"
			},
			new string[]
			{
				"IsTamil",
				"஀ఀ"
			},
			new string[]
			{
				"IsTelugu",
				"ఀಀ"
			},
			new string[]
			{
				"IsThaana",
				"ހ߀"
			},
			new string[]
			{
				"IsThai",
				"฀຀"
			},
			new string[]
			{
				"IsTibetan",
				"ༀက"
			},
			new string[]
			{
				"IsUnifiedCanadianAboriginalSyllabics",
				"᐀\u1680"
			},
			new string[]
			{
				"IsVariationSelectors",
				"︀︐"
			},
			new string[]
			{
				"IsYiRadicals",
				"꒐ꓐ"
			},
			new string[]
			{
				"IsYiSyllables",
				"ꀀ꒐"
			},
			new string[]
			{
				"IsYijingHexagramSymbols",
				"䷀一"
			},
			new string[]
			{
				"_xmlC",
				"-/0;A[_`a{·¸À×Ø÷øĲĴĿŁŉŊſƀǄǍǱǴǶǺȘɐʩʻ˂ː˒̀͆͢͠Ά΋Ό΍Ύ΢ΣϏϐϗϚϛϜϝϞϟϠϡϢϴЁЍЎѐёѝў҂҃҇ҐӅӇӉӋӍӐӬӮӶӸӺԱ՗ՙ՚աևֺֻ֑֢֣־ֿ׀ׁ׃ׅׄא׫װ׳ءػـٓ٠٪ٰڸںڿۀۏې۔ە۩۪ۮ۰ۺँऄअऺ़ॎ॑ॕक़।०॰ঁ঄অ঍এ঑ও঩প঱ল঳শ঺়ঽা৅ে৉োৎৗ৘ড়৞য়৤০৲ਂਃਅ਋ਏ਑ਓ਩ਪ਱ਲ਴ਵ਷ਸ਺਼਽ਾ੃ੇ੉ੋ੎ਖ਼੝ਫ਼੟੦ੵઁ઄અઌઍ઎એ઒ઓ઩પ઱લ઴વ઺઼૆ે૊ો૎ૠૡ૦૰ଁ଄ଅ଍ଏ଑ଓ଩ପ଱ଲ଴ଶ଺଼ୄେ୉ୋ୎ୖ୘ଡ଼୞ୟୢ୦୰ஂ஄அ஋எ஑ஒ஖ங஛ஜ஝ஞ஠ண஥ந஫மஶஷ஺ா௃ெ௉ொ௎ௗ௘௧௰ఁఄఅ఍ఎ఑ఒ఩పఴవ఺ా౅ె౉ొ౎ౕ౗ౠౢ౦౰ಂ಄ಅ಍ಎ಑ಒ಩ಪ಴ವ಺ಾ೅ೆ೉ೊ೎ೕ೗ೞ೟ೠೢ೦೰ംഄഅ഍എ഑ഒഩപഺാൄെ൉ൊൎൗ൘ൠൢ൦൰กฯะ฻เ๏๐๚ກ຃ຄ຅ງຉຊ຋ຍຎດຘນຠມ຤ລ຦ວຨສຬອຯະ຺ົ຾ເ໅ໆ໇່໎໐໚༘༚༠༪༵༶༷༸༹༺༾཈ཉཪཱ྅྆ྌྐྖྗ྘ྙྮྱྸྐྵྺႠ჆აჷᄀᄁᄂᄄᄅᄈᄉᄊᄋᄍᄎᄓᄼᄽᄾᄿᅀᅁᅌᅍᅎᅏᅐᅑᅔᅖᅙᅚᅟᅢᅣᅤᅥᅦᅧᅨᅩᅪᅭᅯᅲᅴᅵᅶᆞᆟᆨᆩᆫᆬᆮᆰᆷᆹᆺᆻᆼᇃᇫᇬᇰᇱᇹᇺḀẜẠỺἀ἖Ἐ἞ἠ὆Ὀ὎ὐ὘Ὑ὚Ὓ὜Ὕ὞Ὗ὾ᾀ᾵ᾶ᾽ι᾿ῂ῅ῆ῍ῐ῔ῖ῜ῠ῭ῲ῵ῶ´⃐⃝⃡⃢Ω℧Kℬ℮ℯↀↃ々〆〇〈〡〰〱〶ぁゕ゙゛ゝゟァ・ーヿㄅㄭ一龦가힤"
			},
			new string[]
			{
				"_xmlD",
				"0:٠٪۰ۺ०॰০ৰ੦ੰ૦૰୦୰௧௰౦౰೦೰൦൰๐๚໐໚༠༪၀၊፩፲០៪᠐᠚０："
			},
			new string[]
			{
				"_xmlI",
				":;A[_`a{À×Ø÷øĲĴĿŁŉŊſƀǄǍǱǴǶǺȘɐʩʻ˂Ά·Έ΋Ό΍Ύ΢ΣϏϐϗϚϛϜϝϞϟϠϡϢϴЁЍЎѐёѝў҂ҐӅӇӉӋӍӐӬӮӶӸӺԱ՗ՙ՚աևא׫װ׳ءػفًٱڸںڿۀۏې۔ەۖۥۧअऺऽाक़ॢঅ঍এ঑ও঩প঱ল঳শ঺ড়৞য়ৢৰ৲ਅ਋ਏ਑ਓ਩ਪ਱ਲ਴ਵ਷ਸ਺ਖ਼੝ਫ਼੟ੲੵઅઌઍ઎એ઒ઓ઩પ઱લ઴વ઺ઽાૠૡଅ଍ଏ଑ଓ଩ପ଱ଲ଴ଶ଺ଽାଡ଼୞ୟୢஅ஋எ஑ஒ஖ங஛ஜ஝ஞ஠ண஥ந஫மஶஷ஺అ఍ఎ఑ఒ఩పఴవ఺ౠౢಅ಍ಎ಑ಒ಩ಪ಴ವ಺ೞ೟ೠೢഅ഍എ഑ഒഩപഺൠൢกฯะัาิเๆກ຃ຄ຅ງຉຊ຋ຍຎດຘນຠມ຤ລ຦ວຨສຬອຯະັາິຽ຾ເ໅ཀ཈ཉཪႠ჆აჷᄀᄁᄂᄄᄅᄈᄉᄊᄋᄍᄎᄓᄼᄽᄾᄿᅀᅁᅌᅍᅎᅏᅐᅑᅔᅖᅙᅚᅟᅢᅣᅤᅥᅦᅧᅨᅩᅪᅭᅯᅲᅴᅵᅶᆞᆟᆨᆩᆫᆬᆮᆰᆷᆹᆺᆻᆼᇃᇫᇬᇰᇱᇹᇺḀẜẠỺἀ἖Ἐ἞ἠ὆Ὀ὎ὐ὘Ὑ὚Ὓ὜Ὕ὞Ὗ὾ᾀ᾵ᾶ᾽ι᾿ῂ῅ῆ῍ῐ῔ῖ῜ῠ῭ῲ῵ῶ´Ω℧Kℬ℮ℯↀↃ〇〈〡〪ぁゕァ・ㄅㄭ一龦가힤"
			},
			new string[]
			{
				"_xmlW",
				"$%+,0:<?A[^_`{|}~\u007f¢«¬­®·¸»¼¿ÀȡȢȴɐʮʰ˯̀͐͠ͰʹͶͺͻ΄·Έ΋Ό΍Ύ΢ΣϏϐϷЀ҇҈ӏӐӶӸӺԀԐԱ՗ՙ՚աֈֺֻ֑֢֣־ֿ׀ׁ׃ׅׄא׫װ׳ءػـٖ٠٪ٮ۔ە۝۞ۮ۰ۿܐܭܰ݋ހ޲ँऄअऺ़ॎॐॕक़।०॰ঁ঄অ঍এ঑ও঩প঱ল঳শ঺়ঽা৅ে৉োৎৗ৘ড়৞য়৤০৻ਂਃਅ਋ਏ਑ਓ਩ਪ਱ਲ਴ਵ਷ਸ਺਼਽ਾ੃ੇ੉ੋ੎ਖ਼੝ਫ਼੟੦ੵઁ઄અઌઍ઎એ઒ઓ઩પ઱લ઴વ઺઼૆ે૊ો૎ૐ૑ૠૡ૦૰ଁ଄ଅ଍ଏ଑ଓ଩ପ଱ଲ଴ଶ଺଼ୄେ୉ୋ୎ୖ୘ଡ଼୞ୟୢ୦ୱஂ஄அ஋எ஑ஒ஖ங஛ஜ஝ஞ஠ண஥ந஫மஶஷ஺ா௃ெ௉ொ௎ௗ௘௧௳ఁఄఅ఍ఎ఑ఒ఩పఴవ఺ా౅ె౉ొ౎ౕ౗ౠౢ౦౰ಂ಄ಅ಍ಎ಑ಒ಩ಪ಴ವ಺ಾ೅ೆ೉ೊ೎ೕ೗ೞ೟ೠೢ೦೰ംഄഅ഍എ഑ഒഩപഺാൄെ൉ൊൎൗ൘ൠൢ൦൰ං඄අ඗ක඲ඳ඼ල඾ව෇්෋ා෕ූ෗ෘ෠ෲ෴ก฻฿๏๐๚ກ຃ຄ຅ງຉຊ຋ຍຎດຘນຠມ຤ລ຦ວຨສຬອ຺ົ຾ເ໅ໆ໇່໎໐໚ໜໞༀ༄༓༺༾཈ཉཫཱ྅྆ྌྐ྘ྙ྽྾࿍࿏࿐ကဢဣဨဩါာဳံ်၀၊ၐၚႠ჆აჹᄀᅚᅟᆣᆨᇺሀሇለቇቈ቉ቊ቎ቐ቗ቘ቙ቚ቞በኇኈ኉ኊ኎ነኯኰ኱ኲ኶ኸ኿ዀ዁ዂ዆ወዏዐ዗ዘዯደጏጐ጑ጒ጖ጘጟጠፇፈ፛፩፽ᎠᏵᐁ᙭ᙯᙷᚁ᚛ᚠ᛫ᛮᛱᜀᜍᜎ᜕ᜠ᜵ᝀ᝔ᝠ᝭ᝮ᝱ᝲ᝴ក។ៗ៘៛៝០៪᠋᠎᠐᠚ᠠᡸᢀᢪḀẜẠỺἀ἖Ἐ἞ἠ὆Ὀ὎ὐ὘Ὑ὚Ὓ὜Ὕ὞Ὗ὾ᾀ᾵ᾶ῅ῆ῔ῖ῜῝῰ῲ῵ῶ῿⁄⁅⁒⁓⁰⁲⁴⁽ⁿ₍₠₲⃫⃐℀℻ℽ⅌⅓ↄ←〈⌫⎴⎷⏏␀␧⑀⑋①⓿─☔☖☘☙♾⚀⚊✁✅✆✊✌✨✩❌❍❎❏❓❖❗❘❟❡❨❶➕➘➰➱➿⟐⟦⟰⦃⦙⧘⧜⧼⧾⬀⺀⺚⺛⻴⼀⿖⿰⿼〄〈〒〔〠〰〱〽〾぀ぁ゗゙゠ァ・ー㄀ㄅㄭㄱ㆏㆐ㆸㇰ㈝㈠㉄㉑㉼㉿㋌㋐㋿㌀㍷㍻㏞㏠㏿㐀䶶一龦ꀀ꒍꒐꓇가힤豈郞侮恵ﬀ﬇ﬓ﬘יִ﬷טּ﬽מּ﬿נּ﭂ףּ﭅צּ﮲ﯓ﴾ﵐ﶐ﶒ﷈ﷰ﷽︀︐︠︤﹢﹣﹤﹧﹩﹪ﹰ﹵ﹶ﻽＄％＋，０：＜？Ａ［＾＿｀｛｜｝～｟ｦ﾿ￂ￈ￊ￐ￒ￘ￚ￝￠￧￨￯￼￾"
			}
		};

		private const int LowercaseSet = 0;

		private const int LowercaseAdd = 1;

		private const int LowercaseBor = 2;

		private const int LowercaseBad = 3;

		private static readonly RegexCharClass.LowerCaseMapping[] s_lcTable = new RegexCharClass.LowerCaseMapping[]
		{
			new RegexCharClass.LowerCaseMapping('A', 'Z', 1, 32),
			new RegexCharClass.LowerCaseMapping('À', 'Þ', 1, 32),
			new RegexCharClass.LowerCaseMapping('Ā', 'Į', 2, 0),
			new RegexCharClass.LowerCaseMapping('İ', 'İ', 0, 105),
			new RegexCharClass.LowerCaseMapping('Ĳ', 'Ķ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ĺ', 'Ň', 3, 0),
			new RegexCharClass.LowerCaseMapping('Ŋ', 'Ŷ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ÿ', 'Ÿ', 0, 255),
			new RegexCharClass.LowerCaseMapping('Ź', 'Ž', 3, 0),
			new RegexCharClass.LowerCaseMapping('Ɓ', 'Ɓ', 0, 595),
			new RegexCharClass.LowerCaseMapping('Ƃ', 'Ƅ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ɔ', 'Ɔ', 0, 596),
			new RegexCharClass.LowerCaseMapping('Ƈ', 'Ƈ', 0, 392),
			new RegexCharClass.LowerCaseMapping('Ɖ', 'Ɗ', 1, 205),
			new RegexCharClass.LowerCaseMapping('Ƌ', 'Ƌ', 0, 396),
			new RegexCharClass.LowerCaseMapping('Ǝ', 'Ǝ', 0, 477),
			new RegexCharClass.LowerCaseMapping('Ə', 'Ə', 0, 601),
			new RegexCharClass.LowerCaseMapping('Ɛ', 'Ɛ', 0, 603),
			new RegexCharClass.LowerCaseMapping('Ƒ', 'Ƒ', 0, 402),
			new RegexCharClass.LowerCaseMapping('Ɠ', 'Ɠ', 0, 608),
			new RegexCharClass.LowerCaseMapping('Ɣ', 'Ɣ', 0, 611),
			new RegexCharClass.LowerCaseMapping('Ɩ', 'Ɩ', 0, 617),
			new RegexCharClass.LowerCaseMapping('Ɨ', 'Ɨ', 0, 616),
			new RegexCharClass.LowerCaseMapping('Ƙ', 'Ƙ', 0, 409),
			new RegexCharClass.LowerCaseMapping('Ɯ', 'Ɯ', 0, 623),
			new RegexCharClass.LowerCaseMapping('Ɲ', 'Ɲ', 0, 626),
			new RegexCharClass.LowerCaseMapping('Ɵ', 'Ɵ', 0, 629),
			new RegexCharClass.LowerCaseMapping('Ơ', 'Ƥ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ƨ', 'Ƨ', 0, 424),
			new RegexCharClass.LowerCaseMapping('Ʃ', 'Ʃ', 0, 643),
			new RegexCharClass.LowerCaseMapping('Ƭ', 'Ƭ', 0, 429),
			new RegexCharClass.LowerCaseMapping('Ʈ', 'Ʈ', 0, 648),
			new RegexCharClass.LowerCaseMapping('Ư', 'Ư', 0, 432),
			new RegexCharClass.LowerCaseMapping('Ʊ', 'Ʋ', 1, 217),
			new RegexCharClass.LowerCaseMapping('Ƴ', 'Ƶ', 3, 0),
			new RegexCharClass.LowerCaseMapping('Ʒ', 'Ʒ', 0, 658),
			new RegexCharClass.LowerCaseMapping('Ƹ', 'Ƹ', 0, 441),
			new RegexCharClass.LowerCaseMapping('Ƽ', 'Ƽ', 0, 445),
			new RegexCharClass.LowerCaseMapping('Ǆ', 'ǅ', 0, 454),
			new RegexCharClass.LowerCaseMapping('Ǉ', 'ǈ', 0, 457),
			new RegexCharClass.LowerCaseMapping('Ǌ', 'ǋ', 0, 460),
			new RegexCharClass.LowerCaseMapping('Ǎ', 'Ǜ', 3, 0),
			new RegexCharClass.LowerCaseMapping('Ǟ', 'Ǯ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ǳ', 'ǲ', 0, 499),
			new RegexCharClass.LowerCaseMapping('Ǵ', 'Ǵ', 0, 501),
			new RegexCharClass.LowerCaseMapping('Ǻ', 'Ȗ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ά', 'Ά', 0, 940),
			new RegexCharClass.LowerCaseMapping('Έ', 'Ί', 1, 37),
			new RegexCharClass.LowerCaseMapping('Ό', 'Ό', 0, 972),
			new RegexCharClass.LowerCaseMapping('Ύ', 'Ώ', 1, 63),
			new RegexCharClass.LowerCaseMapping('Α', 'Ϋ', 1, 32),
			new RegexCharClass.LowerCaseMapping('Ϣ', 'Ϯ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ё', 'Џ', 1, 80),
			new RegexCharClass.LowerCaseMapping('А', 'Я', 1, 32),
			new RegexCharClass.LowerCaseMapping('Ѡ', 'Ҁ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ґ', 'Ҿ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ӂ', 'Ӄ', 3, 0),
			new RegexCharClass.LowerCaseMapping('Ӈ', 'Ӈ', 0, 1224),
			new RegexCharClass.LowerCaseMapping('Ӌ', 'Ӌ', 0, 1228),
			new RegexCharClass.LowerCaseMapping('Ӑ', 'Ӫ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ӯ', 'Ӵ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ӹ', 'Ӹ', 0, 1273),
			new RegexCharClass.LowerCaseMapping('Ա', 'Ֆ', 1, 48),
			new RegexCharClass.LowerCaseMapping('Ⴀ', 'Ⴥ', 1, 48),
			new RegexCharClass.LowerCaseMapping('Ḁ', 'Ỹ', 2, 0),
			new RegexCharClass.LowerCaseMapping('Ἀ', 'Ἇ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ἐ', '἟', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ἠ', 'Ἧ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ἰ', 'Ἷ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ὀ', 'Ὅ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ὑ', 'Ὑ', 0, 8017),
			new RegexCharClass.LowerCaseMapping('Ὓ', 'Ὓ', 0, 8019),
			new RegexCharClass.LowerCaseMapping('Ὕ', 'Ὕ', 0, 8021),
			new RegexCharClass.LowerCaseMapping('Ὗ', 'Ὗ', 0, 8023),
			new RegexCharClass.LowerCaseMapping('Ὠ', 'Ὧ', 1, -8),
			new RegexCharClass.LowerCaseMapping('ᾈ', 'ᾏ', 1, -8),
			new RegexCharClass.LowerCaseMapping('ᾘ', 'ᾟ', 1, -8),
			new RegexCharClass.LowerCaseMapping('ᾨ', 'ᾯ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ᾰ', 'Ᾱ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ὰ', 'Ά', 1, -74),
			new RegexCharClass.LowerCaseMapping('ᾼ', 'ᾼ', 0, 8115),
			new RegexCharClass.LowerCaseMapping('Ὲ', 'Ή', 1, -86),
			new RegexCharClass.LowerCaseMapping('ῌ', 'ῌ', 0, 8131),
			new RegexCharClass.LowerCaseMapping('Ῐ', 'Ῑ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ὶ', 'Ί', 1, -100),
			new RegexCharClass.LowerCaseMapping('Ῠ', 'Ῡ', 1, -8),
			new RegexCharClass.LowerCaseMapping('Ὺ', 'Ύ', 1, -112),
			new RegexCharClass.LowerCaseMapping('Ῥ', 'Ῥ', 0, 8165),
			new RegexCharClass.LowerCaseMapping('Ὸ', 'Ό', 1, -128),
			new RegexCharClass.LowerCaseMapping('Ὼ', 'Ώ', 1, -126),
			new RegexCharClass.LowerCaseMapping('ῼ', 'ῼ', 0, 8179),
			new RegexCharClass.LowerCaseMapping('Ⅰ', 'Ⅿ', 1, 16),
			new RegexCharClass.LowerCaseMapping('Ⓐ', 'ⓐ', 1, 26),
			new RegexCharClass.LowerCaseMapping('Ａ', 'Ｚ', 1, 32)
		};

		private List<RegexCharClass.SingleRange> _rangelist;

		private StringBuilder _categories;

		private bool _canonical;

		private bool _negate;

		private RegexCharClass _subtractor;

		private readonly struct LowerCaseMapping
		{
			internal LowerCaseMapping(char chMin, char chMax, int lcOp, int data)
			{
				this.ChMin = chMin;
				this.ChMax = chMax;
				this.LcOp = lcOp;
				this.Data = data;
			}

			public readonly char ChMin;

			public readonly char ChMax;

			public readonly int LcOp;

			public readonly int Data;
		}

		private sealed class SingleRangeComparer : IComparer<RegexCharClass.SingleRange>
		{
			private SingleRangeComparer()
			{
			}

			public int Compare(RegexCharClass.SingleRange x, RegexCharClass.SingleRange y)
			{
				return x.First.CompareTo(y.First);
			}

			public static readonly RegexCharClass.SingleRangeComparer Instance = new RegexCharClass.SingleRangeComparer();
		}

		private readonly struct SingleRange
		{
			internal SingleRange(char first, char last)
			{
				this.First = first;
				this.Last = last;
			}

			public readonly char First;

			public readonly char Last;
		}
	}
}
