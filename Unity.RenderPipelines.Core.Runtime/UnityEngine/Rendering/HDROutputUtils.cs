using System;

namespace UnityEngine.Rendering
{
	public static class HDROutputUtils
	{
		public static bool GetColorSpaceForGamut(ColorGamut gamut, out int colorspace)
		{
			if (ColorGamutUtility.GetWhitePoint(gamut) != WhitePoint.D65)
			{
				Debug.LogWarningFormat("{0} white point is currently unsupported for outputting to HDR.", new object[]
				{
					gamut.ToString()
				});
				colorspace = -1;
				return false;
			}
			switch (ColorGamutUtility.GetColorPrimaries(gamut))
			{
			case ColorPrimaries.Rec709:
				colorspace = 0;
				return true;
			case ColorPrimaries.Rec2020:
				colorspace = 1;
				return true;
			case ColorPrimaries.P3:
				colorspace = 2;
				return true;
			default:
				Debug.LogWarningFormat("{0} color space is currently unsupported for outputting to HDR.", new object[]
				{
					gamut.ToString()
				});
				colorspace = -1;
				return false;
			}
		}

		public static bool GetColorEncodingForGamut(ColorGamut gamut, out int encoding)
		{
			switch (ColorGamutUtility.GetTransferFunction(gamut))
			{
			case TransferFunction.sRGB:
				encoding = 0;
				return true;
			case TransferFunction.PQ:
				encoding = 2;
				return true;
			case TransferFunction.Linear:
				encoding = 3;
				return true;
			case TransferFunction.Gamma22:
				encoding = 4;
				return true;
			}
			Debug.LogWarningFormat("{0} color encoding is currently unsupported for outputting to HDR.", new object[]
			{
				gamut.ToString()
			});
			encoding = -1;
			return false;
		}

		public static void ConfigureHDROutput(Material material, ColorGamut gamut, HDROutputUtils.Operation operations)
		{
			int value;
			int value2;
			if (!HDROutputUtils.GetColorSpaceForGamut(gamut, out value) || !HDROutputUtils.GetColorEncodingForGamut(gamut, out value2))
			{
				return;
			}
			material.SetInteger(HDROutputUtils.ShaderPropertyId.hdrColorSpace, value);
			material.SetInteger(HDROutputUtils.ShaderPropertyId.hdrEncoding, value2);
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDRColorSpaceConversionAndEncoding.name, operations.HasFlag(HDROutputUtils.Operation.ColorConversion) && operations.HasFlag(HDROutputUtils.Operation.ColorEncoding));
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDREncoding.name, operations.HasFlag(HDROutputUtils.Operation.ColorEncoding) && !operations.HasFlag(HDROutputUtils.Operation.ColorConversion));
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDRColorSpaceConversion.name, operations.HasFlag(HDROutputUtils.Operation.ColorConversion) && !operations.HasFlag(HDROutputUtils.Operation.ColorEncoding));
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDRInput.name, operations == HDROutputUtils.Operation.None);
		}

		public static void ConfigureHDROutput(MaterialPropertyBlock properties, ColorGamut gamut)
		{
			int value;
			int value2;
			if (!HDROutputUtils.GetColorSpaceForGamut(gamut, out value) || !HDROutputUtils.GetColorEncodingForGamut(gamut, out value2))
			{
				return;
			}
			properties.SetInteger(HDROutputUtils.ShaderPropertyId.hdrColorSpace, value);
			properties.SetInteger(HDROutputUtils.ShaderPropertyId.hdrEncoding, value2);
		}

		public static void ConfigureHDROutput(Material material, HDROutputUtils.Operation operations)
		{
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDRColorSpaceConversionAndEncoding.name, operations.HasFlag(HDROutputUtils.Operation.ColorConversion) && operations.HasFlag(HDROutputUtils.Operation.ColorEncoding));
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDREncoding.name, operations.HasFlag(HDROutputUtils.Operation.ColorEncoding) && !operations.HasFlag(HDROutputUtils.Operation.ColorConversion));
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDRColorSpaceConversion.name, operations.HasFlag(HDROutputUtils.Operation.ColorConversion) && !operations.HasFlag(HDROutputUtils.Operation.ColorEncoding));
			CoreUtils.SetKeyword(material, HDROutputUtils.ShaderKeywords.HDRInput.name, operations == HDROutputUtils.Operation.None);
		}

		public static void ConfigureHDROutput(ComputeShader computeShader, ColorGamut gamut, HDROutputUtils.Operation operations)
		{
			int val;
			int val2;
			if (!HDROutputUtils.GetColorSpaceForGamut(gamut, out val) || !HDROutputUtils.GetColorEncodingForGamut(gamut, out val2))
			{
				return;
			}
			computeShader.SetInt(HDROutputUtils.ShaderPropertyId.hdrColorSpace, val);
			computeShader.SetInt(HDROutputUtils.ShaderPropertyId.hdrEncoding, val2);
			CoreUtils.SetKeyword(computeShader, HDROutputUtils.ShaderKeywords.HDRColorSpaceConversionAndEncoding.name, operations.HasFlag(HDROutputUtils.Operation.ColorConversion) && operations.HasFlag(HDROutputUtils.Operation.ColorEncoding));
			CoreUtils.SetKeyword(computeShader, HDROutputUtils.ShaderKeywords.HDREncoding.name, operations.HasFlag(HDROutputUtils.Operation.ColorEncoding) && !operations.HasFlag(HDROutputUtils.Operation.ColorConversion));
			CoreUtils.SetKeyword(computeShader, HDROutputUtils.ShaderKeywords.HDRColorSpaceConversion.name, operations.HasFlag(HDROutputUtils.Operation.ColorConversion) && !operations.HasFlag(HDROutputUtils.Operation.ColorEncoding));
			CoreUtils.SetKeyword(computeShader, HDROutputUtils.ShaderKeywords.HDRInput.name, operations == HDROutputUtils.Operation.None);
		}

		public static bool IsShaderVariantValid(ShaderKeywordSet shaderKeywordSet, bool isHDREnabled)
		{
			bool flag = shaderKeywordSet.IsEnabled(HDROutputUtils.ShaderKeywords.HDREncoding) || shaderKeywordSet.IsEnabled(HDROutputUtils.ShaderKeywords.HDRColorSpaceConversion) || shaderKeywordSet.IsEnabled(HDROutputUtils.ShaderKeywords.HDRColorSpaceConversionAndEncoding) || shaderKeywordSet.IsEnabled(HDROutputUtils.ShaderKeywords.HDRInput);
			return isHDREnabled || !flag;
		}

		[Flags]
		public enum Operation
		{
			None = 0,
			ColorConversion = 1,
			ColorEncoding = 2
		}

		public struct HDRDisplayInformation
		{
			public HDRDisplayInformation(int maxFullFrameToneMapLuminance, int maxToneMapLuminance, int minToneMapLuminance, float hdrPaperWhiteNits)
			{
				this.maxFullFrameToneMapLuminance = maxFullFrameToneMapLuminance;
				this.maxToneMapLuminance = maxToneMapLuminance;
				this.minToneMapLuminance = minToneMapLuminance;
				this.paperWhiteNits = hdrPaperWhiteNits;
			}

			public int maxFullFrameToneMapLuminance;

			public int maxToneMapLuminance;

			public int minToneMapLuminance;

			public float paperWhiteNits;
		}

		public static class ShaderKeywords
		{
			public const string HDR_COLORSPACE_CONVERSION = "HDR_COLORSPACE_CONVERSION";

			public const string HDR_ENCODING = "HDR_ENCODING";

			public const string HDR_COLORSPACE_CONVERSION_AND_ENCODING = "HDR_COLORSPACE_CONVERSION_AND_ENCODING";

			public const string HDR_INPUT = "HDR_INPUT";

			internal static readonly ShaderKeyword HDRColorSpaceConversion = new ShaderKeyword("HDR_COLORSPACE_CONVERSION");

			internal static readonly ShaderKeyword HDREncoding = new ShaderKeyword("HDR_ENCODING");

			internal static readonly ShaderKeyword HDRColorSpaceConversionAndEncoding = new ShaderKeyword("HDR_COLORSPACE_CONVERSION_AND_ENCODING");

			internal static readonly ShaderKeyword HDRInput = new ShaderKeyword("HDR_INPUT");
		}

		private static class ShaderPropertyId
		{
			public static readonly int hdrColorSpace = Shader.PropertyToID("_HDRColorspace");

			public static readonly int hdrEncoding = Shader.PropertyToID("_HDREncoding");
		}
	}
}
