using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal static class UxmlUtility
	{
		public static List<string> ParseStringListAttribute(string itemList)
		{
			bool flag = string.IsNullOrEmpty((itemList != null) ? itemList.Trim() : null);
			List<string> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string[] array = itemList.Split(',', StringSplitOptions.None);
				bool flag2 = array.Length != 0;
				if (flag2)
				{
					List<string> list = new List<string>();
					foreach (string text in array)
					{
						list.Add(text.Trim());
					}
					result = list;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static string EncodeListItem(string item)
		{
			return (item == null) ? string.Empty : item.Replace(",", "%2C");
		}

		public static string DecodeListItem(string item)
		{
			return item.Replace("%2C", ",");
		}

		public static void MoveListItem(IList list, int src, int dst)
		{
			object value = list[src];
			list.RemoveAt(src);
			list.Insert(dst, value);
		}

		public static float ParseFloat(string value, float defaultValue = 0f)
		{
			float num;
			return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static byte ParseByte(string value, byte defaultValue = 0)
		{
			byte b;
			return byte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out b) ? b : defaultValue;
		}

		public static sbyte ParseSByte(string value, sbyte defaultValue = 0)
		{
			sbyte b;
			return sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out b) ? b : defaultValue;
		}

		public static short ParseShort(string value, short defaultValue = 0)
		{
			short num;
			return short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static ushort ParseUShort(string value, ushort defaultValue = 0)
		{
			ushort num;
			return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static int ParseInt(string value, int defaultValue = 0)
		{
			int num;
			return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static uint ParseUint(string value, uint defaultValue = 0U)
		{
			uint num;
			return uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) ? num : defaultValue;
		}

		public static float TryParseFloatAttribute(string attributeName, IUxmlAttributes bag, ref int foundAttributeCounter)
		{
			string value;
			bool flag = bag.TryGetAttributeValue(attributeName, out value);
			float result;
			if (flag)
			{
				foundAttributeCounter++;
				result = UxmlUtility.ParseFloat(value, 0f);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		public static int TryParseIntAttribute(string attributeName, IUxmlAttributes bag, ref int foundAttributeCounter)
		{
			string value;
			bool flag = bag.TryGetAttributeValue(attributeName, out value);
			int result;
			if (flag)
			{
				foundAttributeCounter++;
				result = UxmlUtility.ParseInt(value, 0);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static Type ParseType(string value, Type defaultType = null)
		{
			try
			{
				bool flag = !string.IsNullOrEmpty(value);
				if (flag)
				{
					return Type.GetType(value, true);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return defaultType;
		}

		public static string ValidateUxmlName(string name)
		{
			bool flag = !char.IsLetter(name[0]) && name[0] != '_';
			string result;
			if (flag)
			{
				result = "Element names must start with a letter or underscore";
			}
			else
			{
				bool flag2 = name.StartsWith("xml", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					result = "Element names cannot start with the letters xml (or XML, or Xml, etc)";
				}
				else
				{
					for (int i = 1; i < name.Length; i++)
					{
						char c = name[i];
						bool flag3 = char.IsWhiteSpace(c) || (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.');
						if (flag3)
						{
							return string.Format("The character '{0}' is invalid. Element names can contain letters, digits, hyphens, underscores, and periods.", c);
						}
					}
					result = null;
				}
			}
			return result;
		}

		public static string TypeToString(Type value)
		{
			bool flag = value == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = value.FullName + ", " + value.Assembly.GetName().Name;
			}
			return result;
		}

		public static string ValueToString(Bounds value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3},{4},{5}", new object[]
			{
				value.center.x,
				value.center.y,
				value.center.z,
				value.size.x,
				value.size.y,
				value.size.z
			}));
		}

		public static string ValueToString(BoundsInt value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3},{4},{5}", new object[]
			{
				value.position.x,
				value.position.y,
				value.position.z,
				value.size.x,
				value.size.y,
				value.size.z
			}));
		}

		public static string ValueToString(Rect value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3}", new object[]
			{
				value.x,
				value.y,
				value.width,
				value.height
			}));
		}

		public static string ValueToString(RectInt value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3}", new object[]
			{
				value.x,
				value.y,
				value.width,
				value.height
			}));
		}

		public static string ValueToString(Vector2 value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1}", new object[]
			{
				value.x,
				value.y
			}));
		}

		public static string ValueToString(Vector2Int value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1}", new object[]
			{
				value.x,
				value.y
			}));
		}

		public static string ValueToString(Vector3 value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2}", new object[]
			{
				value.x,
				value.y,
				value.z
			}));
		}

		public static string ValueToString(Vector3Int value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2}", new object[]
			{
				value.x,
				value.y,
				value.z
			}));
		}

		public static string ValueToString(Vector4 value)
		{
			return FormattableString.Invariant(FormattableStringFactory.Create("{0},{1},{2},{3}", new object[]
			{
				value.x,
				value.y,
				value.z,
				value.w
			}));
		}

		public static object CloneObject(object value)
		{
			bool flag = value != null && !(value is string) && !(value is Type) && value.GetType().IsClass;
			object result;
			if (flag)
			{
				result = UxmlSerializedDataUtility.CopySerialized(value);
			}
			else
			{
				result = value;
			}
			return result;
		}

		private const string s_CommaEncoded = "%2C";
	}
}
