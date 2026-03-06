using System;

namespace UnityEngine.UIElements
{
	internal static class StyleValueFunctionExtension
	{
		public static StyleValueFunction FromUssString(string ussValue)
		{
			ussValue = ussValue.ToLowerInvariant();
			string text = ussValue;
			string text2 = text;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 1811665304U)
			{
				if (num <= 829202337U)
				{
					if (num != 630476745U)
					{
						if (num == 829202337U)
						{
							if (text2 == "invert")
							{
								return StyleValueFunction.FilterInvert;
							}
						}
					}
					else if (text2 == "sepia")
					{
						return StyleValueFunction.FilterSepia;
					}
				}
				else if (num != 1302218235U)
				{
					if (num != 1547034252U)
					{
						if (num == 1811665304U)
						{
							if (text2 == "blur")
							{
								return StyleValueFunction.FilterBlur;
							}
						}
					}
					else if (text2 == "tint")
					{
						return StyleValueFunction.FilterTint;
					}
				}
				else if (text2 == "linear-gradient")
				{
					return StyleValueFunction.LinearGradient;
				}
			}
			else if (num <= 2317739966U)
			{
				if (num != 2022607796U)
				{
					if (num == 2317739966U)
					{
						if (text2 == "var")
						{
							return StyleValueFunction.Var;
						}
					}
				}
				else if (text2 == "env")
				{
					return StyleValueFunction.Env;
				}
			}
			else if (num != 2913447899U)
			{
				if (num != 3334659430U)
				{
					if (num == 3552172496U)
					{
						if (text2 == "grayscale")
						{
							return StyleValueFunction.FilterGrayscale;
						}
					}
				}
				else if (text2 == "opacity")
				{
					return StyleValueFunction.FilterOpacity;
				}
			}
			else if (text2 == "none")
			{
				return StyleValueFunction.NoneFilter;
			}
			throw new ArgumentOutOfRangeException("ussValue", ussValue, "Unknown function name");
		}

		public static string ToUssString(this StyleValueFunction svf)
		{
			string result;
			switch (svf)
			{
			case StyleValueFunction.Var:
				result = "var";
				break;
			case StyleValueFunction.Env:
				result = "env";
				break;
			case StyleValueFunction.LinearGradient:
				result = "linear-gradient";
				break;
			case StyleValueFunction.NoneFilter:
				result = "none";
				break;
			case StyleValueFunction.CustomFilter:
				result = "filter";
				break;
			case StyleValueFunction.FilterTint:
				result = "tint";
				break;
			case StyleValueFunction.FilterOpacity:
				result = "opacity";
				break;
			case StyleValueFunction.FilterInvert:
				result = "invert";
				break;
			case StyleValueFunction.FilterGrayscale:
				result = "grayscale";
				break;
			case StyleValueFunction.FilterSepia:
				result = "sepia";
				break;
			case StyleValueFunction.FilterBlur:
				result = "blur";
				break;
			default:
				throw new ArgumentOutOfRangeException("svf", svf, "Unknown StyleValueFunction");
			}
			return result;
		}

		public const string k_Var = "var";

		public const string k_Env = "env";

		public const string k_LinearGradient = "linear-gradient";

		public const string k_NoneFilter = "none";

		public const string k_CustomFilter = "filter";

		public const string k_FilterTint = "tint";

		public const string k_FilterOpacity = "opacity";

		public const string k_FilterInvert = "invert";

		public const string k_FilterGrayscale = "grayscale";

		public const string k_FilterSepia = "sepia";

		public const string k_FilterBlur = "blur";
	}
}
