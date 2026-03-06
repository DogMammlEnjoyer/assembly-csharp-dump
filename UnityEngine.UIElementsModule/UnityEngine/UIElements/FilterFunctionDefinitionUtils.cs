using System;

namespace UnityEngine.UIElements
{
	internal static class FilterFunctionDefinitionUtils
	{
		public static string GetBuiltinFilterName(FilterFunctionType type)
		{
			string result;
			switch (type)
			{
			case FilterFunctionType.Tint:
				result = "tint";
				break;
			case FilterFunctionType.Opacity:
				result = "opacity";
				break;
			case FilterFunctionType.Invert:
				result = "invert";
				break;
			case FilterFunctionType.Grayscale:
				result = "grayscale";
				break;
			case FilterFunctionType.Sepia:
				result = "sepia";
				break;
			case FilterFunctionType.Blur:
				result = "blur";
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public static FilterFunctionDefinition GetBuiltinDefinition(FilterFunctionType type)
		{
			FilterFunctionDefinition result;
			switch (type)
			{
			case FilterFunctionType.Tint:
			{
				bool flag = FilterFunctionDefinitionUtils.s_TintDef == null;
				if (flag)
				{
					FilterFunctionDefinitionUtils.s_TintDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Tint);
				}
				result = FilterFunctionDefinitionUtils.s_TintDef;
				break;
			}
			case FilterFunctionType.Opacity:
			{
				bool flag2 = FilterFunctionDefinitionUtils.s_OpacityDef == null;
				if (flag2)
				{
					FilterFunctionDefinitionUtils.s_OpacityDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Opacity);
				}
				result = FilterFunctionDefinitionUtils.s_OpacityDef;
				break;
			}
			case FilterFunctionType.Invert:
			{
				bool flag3 = FilterFunctionDefinitionUtils.s_InvertDef == null;
				if (flag3)
				{
					FilterFunctionDefinitionUtils.s_InvertDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Invert);
				}
				result = FilterFunctionDefinitionUtils.s_InvertDef;
				break;
			}
			case FilterFunctionType.Grayscale:
			{
				bool flag4 = FilterFunctionDefinitionUtils.s_GrayscaleDef == null;
				if (flag4)
				{
					FilterFunctionDefinitionUtils.s_GrayscaleDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Grayscale);
				}
				result = FilterFunctionDefinitionUtils.s_GrayscaleDef;
				break;
			}
			case FilterFunctionType.Sepia:
			{
				bool flag5 = FilterFunctionDefinitionUtils.s_SepiaDef == null;
				if (flag5)
				{
					FilterFunctionDefinitionUtils.s_SepiaDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Sepia);
				}
				result = FilterFunctionDefinitionUtils.s_SepiaDef;
				break;
			}
			case FilterFunctionType.Blur:
			{
				bool flag6 = FilterFunctionDefinitionUtils.s_BlurDef == null;
				if (flag6)
				{
					FilterFunctionDefinitionUtils.s_BlurDef = FilterFunctionDefinitionUtils.CreateBlurFilterFunctionDefinition();
				}
				result = FilterFunctionDefinitionUtils.s_BlurDef;
				break;
			}
			default:
				result = null;
				break;
			}
			return result;
		}

		private static FilterFunctionDefinition CreateBlurFilterFunctionDefinition()
		{
			Material material = new Material(Shader.Find("Hidden/UIR/GaussianBlur"));
			material.hideFlags = HideFlags.HideAndDontSave;
			FilterFunctionDefinition filterFunctionDefinition = ScriptableObject.CreateInstance<FilterFunctionDefinition>();
			filterFunctionDefinition.hideFlags = HideFlags.HideAndDontSave;
			filterFunctionDefinition.filterName = FilterFunctionDefinitionUtils.GetBuiltinFilterName(FilterFunctionType.Blur);
			filterFunctionDefinition.parameters = new FilterParameterDeclaration[]
			{
				new FilterParameterDeclaration
				{
					interpolationDefaultValue = new FilterParameter
					{
						type = FilterParameterType.Float,
						floatValue = 0f
					},
					defaultValue = new FilterParameter
					{
						type = FilterParameterType.Float,
						floatValue = 0f
					}
				}
			};
			filterFunctionDefinition.passes = new PostProcessingPass[]
			{
				new PostProcessingPass
				{
					material = material,
					passIndex = 0,
					parameterBindings = new ParameterBinding[]
					{
						new ParameterBinding
						{
							index = 0,
							name = "_Sigma"
						}
					},
					readMargins = default(PostProcessingMargins),
					writeMargins = default(PostProcessingMargins)
				},
				new PostProcessingPass
				{
					material = material,
					passIndex = 1,
					parameterBindings = new ParameterBinding[]
					{
						new ParameterBinding
						{
							index = 0,
							name = "_Sigma"
						}
					},
					readMargins = default(PostProcessingMargins),
					writeMargins = default(PostProcessingMargins)
				}
			};
			filterFunctionDefinition.passes[0].computeRequiredReadMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeHorizontalBlurMargins);
			filterFunctionDefinition.passes[0].computeRequiredWriteMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeHorizontalBlurMargins);
			filterFunctionDefinition.passes[1].computeRequiredReadMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeVerticalBlurMargins);
			filterFunctionDefinition.passes[1].computeRequiredWriteMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeVerticalBlurMargins);
			return filterFunctionDefinition;
		}

		private static FilterFunctionDefinition CreateColorEffectFilterFunctionDefinition(FilterFunctionType filterType)
		{
			Material material = new Material(Shader.Find("Hidden/UIR/ColorEffect"));
			material.hideFlags = HideFlags.HideAndDontSave;
			FilterFunctionDefinition filterFunctionDefinition = ScriptableObject.CreateInstance<FilterFunctionDefinition>();
			filterFunctionDefinition.hideFlags = HideFlags.HideAndDontSave;
			filterFunctionDefinition.filterName = FilterFunctionDefinitionUtils.GetBuiltinFilterName(filterType);
			FilterParameter interpolationDefaultValue = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 0f
			};
			FilterParameter defaultValue = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 0f
			};
			switch (filterType)
			{
			case FilterFunctionType.Tint:
				interpolationDefaultValue = new FilterParameter
				{
					type = FilterParameterType.Color,
					colorValue = Color.white
				};
				defaultValue = new FilterParameter
				{
					type = FilterParameterType.Color,
					colorValue = Color.white
				};
				break;
			case FilterFunctionType.Opacity:
				interpolationDefaultValue = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 1f
				};
				defaultValue = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 1f
				};
				break;
			case FilterFunctionType.Invert:
			case FilterFunctionType.Grayscale:
			case FilterFunctionType.Sepia:
				defaultValue = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 1f
				};
				break;
			}
			filterFunctionDefinition.parameters = new FilterParameterDeclaration[]
			{
				new FilterParameterDeclaration
				{
					interpolationDefaultValue = interpolationDefaultValue,
					defaultValue = defaultValue
				}
			};
			filterFunctionDefinition.passes = new PostProcessingPass[]
			{
				new PostProcessingPass
				{
					material = material,
					passIndex = 0,
					parameterBindings = new ParameterBinding[]
					{
						new ParameterBinding
						{
							index = 0,
							name = ""
						}
					},
					readMargins = new PostProcessingMargins
					{
						left = 0f,
						top = 0f,
						right = 0f,
						bottom = 0f
					},
					writeMargins = new PostProcessingMargins
					{
						left = 0f,
						top = 0f,
						right = 0f,
						bottom = 0f
					}
				}
			};
			filterFunctionDefinition.passes[0].prepareMaterialPropertyBlockCallback = new PostProcessingPass.PrepareMaterialPropertyBlockDelegate(FilterFunctionDefinitionUtils.PrepareBuiltinColorEffectMaterialPropertyBlock);
			return filterFunctionDefinition;
		}

		private static PostProcessingMargins ComputeHorizontalBlurMargins(FilterFunction func)
		{
			float num = Math.Max(0f, func.parameters[0].floatValue);
			float num2 = num * 3f;
			return new PostProcessingMargins
			{
				left = num2,
				top = 0f,
				right = num2,
				bottom = 0f
			};
		}

		private static PostProcessingMargins ComputeVerticalBlurMargins(FilterFunction func)
		{
			float num = Math.Max(0f, func.parameters[0].floatValue);
			float num2 = num * 3f;
			return new PostProcessingMargins
			{
				left = 0f,
				top = num2,
				right = 0f,
				bottom = num2
			};
		}

		private static void PrepareBuiltinColorEffectMaterialPropertyBlock(MaterialPropertyBlock mpb, FilterFunction func)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			Color value = Color.white;
			float value2 = 0f;
			switch (func.type)
			{
			case FilterFunctionType.Tint:
				value = func.parameters[0].colorValue;
				break;
			case FilterFunctionType.Opacity:
				value.a = Mathf.Clamp01(func.parameters[0].floatValue);
				break;
			case FilterFunctionType.Invert:
				value2 = Mathf.Clamp01(func.parameters[0].floatValue);
				break;
			case FilterFunctionType.Grayscale:
			{
				float num = Mathf.Clamp01(func.parameters[0].floatValue);
				identity = new Matrix4x4(new Vector4(0.2126f + 0.7874f * (1f - num), 0.2126f - 0.2126f * (1f - num), 0.2126f - 0.2126f * (1f - num), 0f), new Vector4(0.7152f - 0.7152f * (1f - num), 0.7152f + 0.2848f * (1f - num), 0.7152f - 0.7152f * (1f - num), 0f), new Vector4(0.0722f - 0.0722f * (1f - num), 0.0722f - 0.0722f * (1f - num), 0.0722f + 0.9278f * (1f - num), 0f), new Vector4(0f, 0f, 0f, 1f));
				break;
			}
			case FilterFunctionType.Sepia:
			{
				float num2 = Mathf.Clamp01(func.parameters[0].floatValue);
				identity = new Matrix4x4(new Vector4(0.393f + 0.607f * (1f - num2), 0.349f - 0.349f * (1f - num2), 0.272f - 0.272f * (1f - num2), 0f), new Vector4(0.769f - 0.769f * (1f - num2), 0.686f + 0.314f * (1f - num2), 0.534f - 0.534f * (1f - num2), 0f), new Vector4(0.189f - 0.189f * (1f - num2), 0.168f - 0.168f * (1f - num2), 0.131f + 0.869f * (1f - num2), 0f), new Vector4(0f, 0f, 0f, 1f));
				break;
			}
			}
			mpb.SetMatrix("_ColorMatrix", identity);
			mpb.SetColor("_ColorTint", value);
			mpb.SetFloat("_ColorInvert", value2);
		}

		private static FilterFunctionDefinition s_BlurDef;

		private static FilterFunctionDefinition s_TintDef;

		private static FilterFunctionDefinition s_OpacityDef;

		private static FilterFunctionDefinition s_InvertDef;

		private static FilterFunctionDefinition s_GrayscaleDef;

		private static FilterFunctionDefinition s_SepiaDef;
	}
}
