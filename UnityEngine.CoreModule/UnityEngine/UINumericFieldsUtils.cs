using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule",
		"UnityEditor.UIBuilderModule"
	})]
	internal static class UINumericFieldsUtils
	{
		public static bool TryConvertStringToDouble(string str, out double value)
		{
			ExpressionEvaluator.Expression expression;
			return UINumericFieldsUtils.TryConvertStringToDouble(str, out value, out expression);
		}

		public static bool TryConvertStringToDouble(string str, out double value, out ExpressionEvaluator.Expression expr)
		{
			expr = null;
			string text = str.ToLower();
			string text2 = text;
			string a = text2;
			if (!(a == "inf") && !(a == "infinity"))
			{
				if (!(a == "-inf") && !(a == "-infinity"))
				{
					if (!(a == "nan"))
					{
						return ExpressionEvaluator.Evaluate<double>(str, out value, out expr);
					}
					value = double.NaN;
				}
				else
				{
					value = double.NegativeInfinity;
				}
			}
			else
			{
				value = double.PositiveInfinity;
			}
			return true;
		}

		public static bool TryConvertStringToDouble(string str, string initialValueAsString, out double value, out ExpressionEvaluator.Expression expression)
		{
			bool flag = UINumericFieldsUtils.TryConvertStringToDouble(str, out value, out expression);
			bool flag2 = !flag && expression != null && !string.IsNullOrEmpty(initialValueAsString);
			if (flag2)
			{
				double num;
				ExpressionEvaluator.Expression expression2;
				bool flag3 = UINumericFieldsUtils.TryConvertStringToDouble(initialValueAsString, out num, out expression2);
				if (flag3)
				{
					value = num;
					flag = expression.Evaluate<double>(ref value, 0, 1);
				}
			}
			return flag;
		}

		public static bool TryConvertStringToFloat(string str, string initialValueAsString, out float value, out ExpressionEvaluator.Expression expression)
		{
			double value2;
			bool result = UINumericFieldsUtils.TryConvertStringToDouble(str, initialValueAsString, out value2, out expression);
			value = Mathf.ClampToFloat(value2);
			return result;
		}

		public static bool TryConvertStringToLong(string str, out long value)
		{
			ExpressionEvaluator.Expression expression;
			return ExpressionEvaluator.Evaluate<long>(str, out value, out expression);
		}

		public static bool TryConvertStringToLong(string str, out long value, out ExpressionEvaluator.Expression expr)
		{
			return ExpressionEvaluator.Evaluate<long>(str, out value, out expr);
		}

		public static bool TryConvertStringToLong(string str, string initialValueAsString, out long value, out ExpressionEvaluator.Expression expression)
		{
			bool flag = UINumericFieldsUtils.TryConvertStringToLong(str, out value, out expression);
			bool flag2 = !flag && expression != null && !string.IsNullOrEmpty(initialValueAsString);
			if (flag2)
			{
				long num;
				ExpressionEvaluator.Expression expression2;
				bool flag3 = UINumericFieldsUtils.TryConvertStringToLong(initialValueAsString, out num, out expression2);
				if (flag3)
				{
					value = num;
					flag = expression.Evaluate<long>(ref value, 0, 1);
				}
			}
			return flag;
		}

		public static bool TryConvertStringToULong(string str, out ulong value, out ExpressionEvaluator.Expression expr)
		{
			return ExpressionEvaluator.Evaluate<ulong>(str, out value, out expr);
		}

		public static bool TryConvertStringToULong(string str, string initialValueAsString, out ulong value, out ExpressionEvaluator.Expression expression)
		{
			bool flag = UINumericFieldsUtils.TryConvertStringToULong(str, out value, out expression);
			bool flag2 = !flag && expression != null && !string.IsNullOrEmpty(initialValueAsString);
			if (flag2)
			{
				ulong num;
				ExpressionEvaluator.Expression expression2;
				bool flag3 = UINumericFieldsUtils.TryConvertStringToULong(initialValueAsString, out num, out expression2);
				if (flag3)
				{
					value = num;
					flag = expression.Evaluate<ulong>(ref value, 0, 1);
				}
			}
			return flag;
		}

		public static bool TryConvertStringToInt(string str, string initialValueAsString, out int value, out ExpressionEvaluator.Expression expression)
		{
			long value2;
			bool result = UINumericFieldsUtils.TryConvertStringToLong(str, initialValueAsString, out value2, out expression);
			value = Mathf.ClampToInt(value2);
			return result;
		}

		public static bool TryConvertStringToUInt(string str, string initialValueAsString, out uint value, out ExpressionEvaluator.Expression expression)
		{
			long value2;
			bool result = UINumericFieldsUtils.TryConvertStringToLong(str, initialValueAsString, out value2, out expression);
			value = Mathf.ClampToUInt(value2);
			return result;
		}

		public static readonly string k_AllowedCharactersForFloat = "inftynaeINFTYNAE0123456789.,-*/+%^()cosqrludxvRL=pP#";

		public static readonly string k_AllowedCharactersForInt = "0123456789-*/+%^()cosintaqrtelfundxvRL,=pPI#";

		public static readonly string k_DoubleFieldFormatString = "R";

		public static readonly string k_FloatFieldFormatString = "g7";

		public static readonly string k_IntFieldFormatString = "#######0";
	}
}
