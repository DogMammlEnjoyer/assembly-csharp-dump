using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule"
	})]
	internal static class RichTextTagParser
	{
		private unsafe static bool tagMatch(ReadOnlySpan<char> tagCandidate, [Nullable(1)] string tagName)
		{
			return tagCandidate.StartsWith(tagName.AsSpan()) && (tagCandidate.Length == tagName.Length || (!char.IsLetter((char)(*tagCandidate[tagName.Length])) && *tagCandidate[tagName.Length] != 45));
		}

		private unsafe static bool SpanToEnum(ReadOnlySpan<char> tagCandidate, out RichTextTagParser.TagType tagType, [Nullable(2)] out string error, out ReadOnlySpan<char> attribute)
		{
			for (int i = 0; i < RichTextTagParser.TagsInfo.Length; i++)
			{
				string name = RichTextTagParser.TagsInfo[i].name;
				bool flag = RichTextTagParser.tagMatch(tagCandidate, name);
				if (flag)
				{
					tagType = RichTextTagParser.TagsInfo[i].TagType;
					error = null;
					attribute = tagCandidate.Slice(name.Length);
					return true;
				}
			}
			bool flag2 = tagCandidate.Length > 4 && *tagCandidate[0] == 35;
			if (flag2)
			{
				tagType = RichTextTagParser.TagType.Color;
				error = null;
				attribute = tagCandidate;
				return true;
			}
			error = "Unknown tag: " + tagCandidate.ToString();
			tagType = RichTextTagParser.TagType.Unknown;
			attribute = null;
			return false;
		}

		[NullableContext(1)]
		internal static List<RichTextTagParser.Tag> FindTags(string inputStr, [Nullable(new byte[]
		{
			2,
			1
		})] List<RichTextTagParser.ParseError> errors = null)
		{
			char[] array = inputStr.ToCharArray();
			List<RichTextTagParser.Tag> list = new List<RichTextTagParser.Tag>();
			int num = 0;
			for (;;)
			{
				int num2 = Array.IndexOf<char>(array, '<', num);
				bool flag = num2 == -1;
				if (flag)
				{
					break;
				}
				int num3 = Array.IndexOf<char>(array, '>', num2);
				bool flag2 = num3 == -1;
				if (flag2)
				{
					break;
				}
				bool flag3 = array.Length > num2 + 1 && array[num2 + 1] == '/';
				bool flag4 = num3 == num2 + 1;
				if (flag4)
				{
					if (errors != null)
					{
						errors.Add(new RichTextTagParser.ParseError("Empty tag", num2));
					}
					num = num3 + 1;
				}
				else
				{
					num = num3 + 1;
					bool flag5 = !flag3;
					if (flag5)
					{
						Span<char> span = array.AsSpan(num2 + 1, num3 - num2 - 1);
						RichTextTagParser.TagType tagType;
						string text;
						ReadOnlySpan<char> readOnlySpan;
						bool flag6 = RichTextTagParser.SpanToEnum(span, out tagType, out text, out readOnlySpan);
						if (flag6)
						{
							RichTextTagParser.TagValue tagValue = null;
							bool flag7 = tagType == RichTextTagParser.TagType.Color;
							if (flag7)
							{
								readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
								Color value;
								ColorUtility.TryParseHtmlString(readOnlySpan.ToString(), out value);
								tagValue = new RichTextTagParser.TagValue(value);
								bool flag8 = tagValue == null;
								if (flag8)
								{
									if (errors != null)
									{
										errors.Add(new RichTextTagParser.ParseError("Invalid color value", num2));
									}
									num = num2 + 1;
									continue;
								}
							}
							bool flag9 = tagType == RichTextTagParser.TagType.Link || tagType == RichTextTagParser.TagType.Hyperlink;
							if (flag9)
							{
								bool flag10 = tagType == RichTextTagParser.TagType.Hyperlink && readOnlySpan.StartsWith(" href=");
								if (flag10)
								{
									readOnlySpan = readOnlySpan.Slice(" href=".Length);
								}
								readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
								string value2 = readOnlySpan.ToString();
								tagValue = new RichTextTagParser.TagValue(value2);
							}
							bool flag11 = tagType == RichTextTagParser.TagType.Align;
							if (flag11)
							{
								readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
								string value3 = readOnlySpan.ToString();
								HorizontalAlignment horizontalAlignment;
								bool flag12 = Enum.TryParse<HorizontalAlignment>(value3, true, out horizontalAlignment);
								if (flag12)
								{
									tagValue = new RichTextTagParser.TagValue(value3);
								}
								bool flag13 = tagValue == null;
								if (flag13)
								{
									if (errors != null)
									{
										errors.Add(new RichTextTagParser.ParseError(string.Format("Invalid {0} value", tagType), num2));
									}
									num = num2 + 1;
									continue;
								}
							}
							list.Add(new RichTextTagParser.Tag
							{
								tagType = tagType,
								start = num2,
								end = num3,
								isClosing = flag3,
								value = tagValue
							});
							bool flag14 = tagType == RichTextTagParser.TagType.NoParse;
							if (flag14)
							{
								bool flag15 = (num2 = array.AsSpan(num).IndexOf("</noparse>")) == -1;
								if (flag15)
								{
									break;
								}
								num2 += num;
								num3 = num2 + "</noparse>".Length;
								list.Add(new RichTextTagParser.Tag
								{
									tagType = RichTextTagParser.TagType.NoParse,
									start = num2,
									end = num3,
									isClosing = true
								});
								num = num3 + 1;
							}
						}
						else
						{
							bool flag16 = text != null;
							if (flag16)
							{
								if (errors != null)
								{
									errors.Add(new RichTextTagParser.ParseError(text, num2));
								}
							}
							num = num2 + 1;
						}
					}
					else
					{
						RichTextTagParser.TagType tagType2;
						string text2;
						ReadOnlySpan<char> readOnlySpan2;
						bool flag17 = RichTextTagParser.SpanToEnum(array.AsSpan(num2 + 2, num3 - num2 - 2), out tagType2, out text2, out readOnlySpan2);
						if (flag17)
						{
							list.Add(new RichTextTagParser.Tag
							{
								tagType = tagType2,
								start = num2,
								end = num3,
								isClosing = flag3
							});
						}
						else
						{
							bool flag18 = text2 != null;
							if (flag18)
							{
								if (errors != null)
								{
									errors.Add(new RichTextTagParser.ParseError(text2, num2));
								}
							}
							num = num2 + 1;
						}
					}
				}
			}
			return list;
		}

		private unsafe static ReadOnlySpan<char> GetAttributeSpan(ReadOnlySpan<char> atributeSection)
		{
			bool flag = atributeSection.Length >= 2 && *atributeSection[0] == 61;
			if (flag)
			{
				atributeSection = atributeSection.Slice(1);
			}
			bool flag2 = atributeSection.Length >= 2 && ((*atributeSection[0] == 34 && *atributeSection[atributeSection.Length - 1] == 34) || (*atributeSection[0] == 39 && *atributeSection[atributeSection.Length - 1] == 39));
			ReadOnlySpan<char> result;
			if (flag2)
			{
				result = atributeSection.Slice(1, atributeSection.Length - 2);
			}
			else
			{
				result = atributeSection;
			}
			return result;
		}

		[NullableContext(1)]
		internal unsafe static List<RichTextTagParser.Tag> PickResultingTags(List<RichTextTagParser.Tag> allTags, string input, int atPosition, [Nullable(2)] List<RichTextTagParser.Tag> applicableTags = null)
		{
			bool flag = applicableTags == null;
			if (flag)
			{
				applicableTags = new List<RichTextTagParser.Tag>();
			}
			else
			{
				applicableTags.Clear();
			}
			int num = 0;
			Debug.Assert(string.IsNullOrEmpty(input) || (atPosition < input.Length && atPosition >= 0), "Invalid position");
			Debug.Assert(num <= atPosition && num >= 0, "Invalid starting position");
			int num2 = 0;
			foreach (RichTextTagParser.Tag tag in allTags)
			{
				Debug.Assert(tag.start >= num2, "Tags are not sorted");
				num2 = tag.end + 1;
			}
			foreach (RichTextTagParser.Tag tag2 in applicableTags)
			{
				Debug.Assert(tag2.end <= num, "Tag end pass the point where we should start parsing");
				Debug.Assert(allTags.Contains(tag2));
			}
			int count = allTags.Count;
			Span<int?> span2;
			Span<int?> span3;
			int num4;
			checked
			{
				Span<int?> span = new Span<int?>(stackalloc byte[unchecked((UIntPtr)count) * (UIntPtr)sizeof(int?)], count);
				span2 = span;
				int num3 = RichTextTagParser.TagsInfo.Length;
				span = new Span<int?>(stackalloc byte[unchecked((UIntPtr)num3) * (UIntPtr)sizeof(int?)], num3);
				span3 = span;
				num4 = -1;
			}
			foreach (RichTextTagParser.Tag tag3 in allTags)
			{
				num4++;
				bool flag2 = tag3.end < num;
				if (!flag2)
				{
					bool flag3 = tag3.tagType == RichTextTagParser.TagType.NoParse;
					if (!flag3)
					{
						bool flag4 = tag3.start > atPosition;
						if (flag4)
						{
							break;
						}
						bool isClosing = tag3.isClosing;
						if (isClosing)
						{
							bool flag5 = span3[(int)tag3.tagType] != null;
							if (flag5)
							{
								bool flag6 = span2[num4] != null;
								if (flag6)
								{
									*span3[(int)tag3.tagType] = *span2[num4];
								}
								else
								{
									*span3[(int)tag3.tagType] = null;
								}
							}
						}
						else
						{
							int? num5 = *span3[(int)tag3.tagType];
							bool flag7 = num5 != null;
							if (flag7)
							{
								*span2[num4] = num5;
							}
							*span3[(int)tag3.tagType] = new int?(num4);
						}
					}
				}
			}
			int num6 = 0;
			foreach (RichTextTagParser.Tag tag4 in allTags)
			{
				int? num7 = *span3[(int)tag4.tagType];
				bool flag8 = num7 != null && num6 == num7.Value;
				if (flag8)
				{
					applicableTags.Add(tag4);
				}
				num6++;
			}
			return applicableTags;
		}

		[NullableContext(1)]
		internal static RichTextTagParser.Segment[] GenerateSegments(string input, List<RichTextTagParser.Tag> tags)
		{
			List<RichTextTagParser.Segment> list = new List<RichTextTagParser.Segment>();
			int num = 0;
			for (int i = 0; i < tags.Count; i++)
			{
				Debug.Assert(tags[i].start >= num);
				bool flag = tags[i].start > num;
				if (flag)
				{
					list.Add(new RichTextTagParser.Segment
					{
						start = num,
						end = tags[i].start - 1
					});
				}
				num = tags[i].end + 1;
			}
			bool flag2 = num < input.Length;
			if (flag2)
			{
				list.Add(new RichTextTagParser.Segment
				{
					start = num,
					end = input.Length - 1
				});
			}
			return list.ToArray();
		}

		[NullableContext(1)]
		internal static void ApplyStateToSegment(string input, List<RichTextTagParser.Tag> tags, RichTextTagParser.Segment[] segments)
		{
			for (int i = 0; i < segments.Length; i++)
			{
				segments[i].tags = RichTextTagParser.PickResultingTags(tags, input, segments[i].start, null);
			}
		}

		[NullableContext(1)]
		private static int AddLink(RichTextTagParser.TagType type, string value, [Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<int, RichTextTagParser.TagType, string>> links)
		{
			foreach (ValueTuple<int, RichTextTagParser.TagType, string> valueTuple in links)
			{
				int item = valueTuple.Item1;
				RichTextTagParser.TagType item2 = valueTuple.Item2;
				string item3 = valueTuple.Item3;
				bool flag = type == item2 && value == item3;
				if (flag)
				{
					return item;
				}
			}
			int count = links.Count;
			links.Add(new ValueTuple<int, RichTextTagParser.TagType, string>(count, type, value));
			return count;
		}

		private static TextSpan CreateTextSpan(RichTextTagParser.Segment segment, ref NativeTextGenerationSettings tgs, [Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<int, RichTextTagParser.TagType, string>> links, Color hyperlinkColor)
		{
			TextSpan textSpan = tgs.CreateTextSpan();
			bool flag = segment.tags == null;
			TextSpan result;
			if (flag)
			{
				result = textSpan;
			}
			else
			{
				for (int i = 0; i < segment.tags.Count; i++)
				{
					switch (segment.tags[i].tagType)
					{
					case RichTextTagParser.TagType.Hyperlink:
					{
						RichTextTagParser.TagType type = RichTextTagParser.TagType.Hyperlink;
						RichTextTagParser.TagValue value = segment.tags[i].value;
						textSpan.linkID = RichTextTagParser.AddLink(type, ((value != null) ? value.StringValue : null) ?? "", links);
						textSpan.color = hyperlinkColor;
						textSpan.fontStyle |= FontStyles.Underline;
						break;
					}
					case RichTextTagParser.TagType.Align:
						Enum.TryParse<HorizontalAlignment>(segment.tags[i].value.StringValue, true, out textSpan.alignment);
						break;
					case RichTextTagParser.TagType.AllCaps:
					case RichTextTagParser.TagType.Uppercase:
						textSpan.fontStyle |= FontStyles.UpperCase;
						break;
					case RichTextTagParser.TagType.Bold:
						textSpan.fontWeight = TextFontWeight.Bold;
						break;
					case RichTextTagParser.TagType.Color:
						textSpan.color = segment.tags[i].value.ColorValue;
						break;
					case RichTextTagParser.TagType.Italic:
						textSpan.fontStyle |= FontStyles.Italic;
						break;
					case RichTextTagParser.TagType.Link:
					{
						RichTextTagParser.TagType type2 = RichTextTagParser.TagType.Link;
						RichTextTagParser.TagValue value2 = segment.tags[i].value;
						textSpan.linkID = RichTextTagParser.AddLink(type2, ((value2 != null) ? value2.StringValue : null) ?? "", links);
						break;
					}
					case RichTextTagParser.TagType.Lowercase:
					case RichTextTagParser.TagType.SmallCaps:
						textSpan.fontStyle |= FontStyles.LowerCase;
						break;
					case RichTextTagParser.TagType.Mark:
						textSpan.fontStyle |= FontStyles.Highlight;
						break;
					case RichTextTagParser.TagType.NoParse:
					case RichTextTagParser.TagType.Unknown:
						throw new InvalidOperationException("Invalid tag type" + segment.tags[i].tagType.ToString());
					case RichTextTagParser.TagType.Strikethrough:
						textSpan.fontStyle |= FontStyles.Strikethrough;
						break;
					case RichTextTagParser.TagType.Subscript:
						textSpan.fontStyle |= FontStyles.Subscript;
						break;
					case RichTextTagParser.TagType.Superscript:
						textSpan.fontStyle |= FontStyles.Superscript;
						break;
					case RichTextTagParser.TagType.Underline:
						textSpan.fontStyle |= FontStyles.Underline;
						break;
					}
				}
				result = textSpan;
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal static void CreateTextGenerationSettingsArray(ref NativeTextGenerationSettings tgs, [Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<int, RichTextTagParser.TagType, string>> links, Color hyperlinkColor)
		{
			links.Clear();
			List<RichTextTagParser.Tag> tags = RichTextTagParser.FindTags(tgs.text, null);
			RichTextTagParser.Segment[] array = RichTextTagParser.GenerateSegments(tgs.text, tags);
			RichTextTagParser.ApplyStateToSegment(tgs.text, tags, array);
			StringBuilder stringBuilder = new StringBuilder(tgs.text.Length);
			tgs.textSpans = new TextSpan[array.Length];
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				RichTextTagParser.Segment segment = array[i];
				string text = tgs.text.Substring(segment.start, segment.end + 1 - segment.start);
				TextSpan textSpan = RichTextTagParser.CreateTextSpan(segment, ref tgs, links, hyperlinkColor);
				textSpan.startIndex = num;
				textSpan.length = text.Length;
				tgs.textSpans[i] = textSpan;
				stringBuilder.Append(text);
				num += text.Length;
			}
			tgs.text = stringBuilder.ToString();
		}

		[Nullable(1)]
		internal static readonly RichTextTagParser.TagTypeInfo[] TagsInfo = new RichTextTagParser.TagTypeInfo[]
		{
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Hyperlink, "a", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Align, "align", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.AllCaps, "allcaps", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Alpha, "alpha", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Bold, "b", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Br, "br", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Color, "color", RichTextTagParser.TagValueType.ColorValue, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.CSpace, "cspace", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Font, "font", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.FontWeight, "font-weight", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Italic, "i", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Indent, "indent", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.LineHeight, "line-height", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.LineIndent, "line-indent", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Link, "link", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Lowercase, "lowercase", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Mark, "mark", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Mspace, "mspace", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.NoBr, "nobr", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.NoParse, "noparse", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Strikethrough, "s", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Size, "size", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.SmallCaps, "smallcaps", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Space, "space", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Sprite, "sprite", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Style, "style", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Subscript, "sub", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Superscript, "sup", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Underline, "u", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Uppercase, "uppercase", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Pixels)
		};

		public enum TagType
		{
			Hyperlink,
			Align,
			AllCaps,
			Alpha,
			Bold,
			Br,
			Color,
			CSpace,
			Font,
			FontWeight,
			Italic,
			Indent,
			LineHeight,
			LineIndent,
			Link,
			Lowercase,
			Mark,
			Mspace,
			NoBr,
			NoParse,
			Strikethrough,
			Size,
			SmallCaps,
			Space,
			Sprite,
			Style,
			Subscript,
			Superscript,
			Underline,
			Uppercase,
			Unknown
		}

		[Nullable(0)]
		[NullableContext(1)]
		internal class TagTypeInfo : IEquatable<RichTextTagParser.TagTypeInfo>
		{
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(RichTextTagParser.TagTypeInfo);
				}
			}

			internal TagTypeInfo(RichTextTagParser.TagType tagType, string name, RichTextTagParser.TagValueType valueType = RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType unitType = RichTextTagParser.TagUnitType.Pixels)
			{
				this.TagType = tagType;
				this.name = name;
				this.valueType = valueType;
				this.unitType = unitType;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("TagTypeInfo");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("TagType = ");
				builder.Append(this.TagType.ToString());
				builder.Append(", name = ");
				builder.Append(this.name);
				builder.Append(", valueType = ");
				builder.Append(this.valueType.ToString());
				builder.Append(", unitType = ");
				builder.Append(this.unitType.ToString());
				return true;
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(RichTextTagParser.TagTypeInfo left, RichTextTagParser.TagTypeInfo right)
			{
				return !(left == right);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(RichTextTagParser.TagTypeInfo left, RichTextTagParser.TagTypeInfo right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<RichTextTagParser.TagType>.Default.GetHashCode(this.TagType)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.name)) * -1521134295 + EqualityComparer<RichTextTagParser.TagValueType>.Default.GetHashCode(this.valueType)) * -1521134295 + EqualityComparer<RichTextTagParser.TagUnitType>.Default.GetHashCode(this.unitType);
			}

			[CompilerGenerated]
			[NullableContext(2)]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RichTextTagParser.TagTypeInfo);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(RichTextTagParser.TagTypeInfo other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<RichTextTagParser.TagType>.Default.Equals(this.TagType, other.TagType) && EqualityComparer<string>.Default.Equals(this.name, other.name) && EqualityComparer<RichTextTagParser.TagValueType>.Default.Equals(this.valueType, other.valueType) && EqualityComparer<RichTextTagParser.TagUnitType>.Default.Equals(this.unitType, other.unitType));
			}

			[CompilerGenerated]
			protected TagTypeInfo(RichTextTagParser.TagTypeInfo original)
			{
				this.TagType = original.TagType;
				this.name = original.name;
				this.valueType = original.valueType;
				this.unitType = original.unitType;
			}

			public RichTextTagParser.TagType TagType;

			public string name;

			public RichTextTagParser.TagValueType valueType;

			public RichTextTagParser.TagUnitType unitType;
		}

		internal enum TagValueType
		{
			None,
			NumericalValue,
			StringValue,
			ColorValue = 4
		}

		internal enum TagUnitType
		{
			Pixels,
			FontUnits,
			Percentage
		}

		[NullableContext(2)]
		[Nullable(0)]
		internal class TagValue : IEquatable<RichTextTagParser.TagValue>
		{
			[CompilerGenerated]
			[Nullable(1)]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				[NullableContext(1)]
				get
				{
					return typeof(RichTextTagParser.TagValue);
				}
			}

			internal TagValue(float value)
			{
				this.type = RichTextTagParser.TagValueType.NumericalValue;
				this.m_numericalValue = value;
			}

			internal TagValue(Color value)
			{
				this.type = RichTextTagParser.TagValueType.ColorValue;
				this.m_colorValue = value;
			}

			[NullableContext(1)]
			internal TagValue(string value)
			{
				this.type = RichTextTagParser.TagValueType.StringValue;
				this.m_stringValue = value;
			}

			internal string StringValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.StringValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a string value");
					}
					return this.m_stringValue;
				}
			}

			internal float NumericalValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.NumericalValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a numerical value");
					}
					return this.m_numericalValue;
				}
			}

			internal Color ColorValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.ColorValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a color value");
					}
					return this.m_colorValue;
				}
			}

			[CompilerGenerated]
			[NullableContext(1)]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("TagValue");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[NullableContext(1)]
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				return false;
			}

			[CompilerGenerated]
			public static bool operator !=(RichTextTagParser.TagValue left, RichTextTagParser.TagValue right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(RichTextTagParser.TagValue left, RichTextTagParser.TagValue right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<RichTextTagParser.TagValueType>.Default.GetHashCode(this.type)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.m_stringValue)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.m_numericalValue)) * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(this.m_colorValue);
			}

			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RichTextTagParser.TagValue);
			}

			[CompilerGenerated]
			public virtual bool Equals(RichTextTagParser.TagValue other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<RichTextTagParser.TagValueType>.Default.Equals(this.type, other.type) && EqualityComparer<string>.Default.Equals(this.m_stringValue, other.m_stringValue) && EqualityComparer<float>.Default.Equals(this.m_numericalValue, other.m_numericalValue) && EqualityComparer<Color>.Default.Equals(this.m_colorValue, other.m_colorValue));
			}

			[CompilerGenerated]
			protected TagValue([Nullable(1)] RichTextTagParser.TagValue original)
			{
				this.type = original.type;
				this.m_stringValue = original.m_stringValue;
				this.m_numericalValue = original.m_numericalValue;
				this.m_colorValue = original.m_colorValue;
			}

			internal RichTextTagParser.TagValueType type;

			private string m_stringValue;

			private float m_numericalValue;

			private Color m_colorValue;
		}

		internal struct Tag
		{
			public RichTextTagParser.TagType tagType;

			public bool isClosing;

			public int start;

			public int end;

			[Nullable(2)]
			public RichTextTagParser.TagValue value;
		}

		public struct Segment
		{
			[Nullable(2)]
			public List<RichTextTagParser.Tag> tags;

			public int start;

			public int end;
		}

		[NullableContext(1)]
		[Nullable(0)]
		internal class ParseError : IEquatable<RichTextTagParser.ParseError>
		{
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(RichTextTagParser.ParseError);
				}
			}

			internal ParseError(string message, int position)
			{
				this.message = message;
				this.position = position;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("ParseError");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("position = ");
				builder.Append(this.position.ToString());
				builder.Append(", message = ");
				builder.Append(this.message);
				return true;
			}

			[CompilerGenerated]
			[NullableContext(2)]
			public static bool operator !=(RichTextTagParser.ParseError left, RichTextTagParser.ParseError right)
			{
				return !(left == right);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(RichTextTagParser.ParseError left, RichTextTagParser.ParseError right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.position)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.message);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RichTextTagParser.ParseError);
			}

			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(RichTextTagParser.ParseError other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(this.position, other.position) && EqualityComparer<string>.Default.Equals(this.message, other.message));
			}

			[CompilerGenerated]
			protected ParseError(RichTextTagParser.ParseError original)
			{
				this.position = original.position;
				this.message = original.message;
			}

			public readonly int position;

			public readonly string message;
		}
	}
}
