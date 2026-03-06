using System;
using Unity.Collections;

namespace UnityEngine.Rendering.RendererUtils
{
	public struct RendererListDesc
	{
		public uint batchLayerMask { readonly get; set; }

		internal CullingResults cullingResult { readonly get; private set; }

		internal Camera camera { readonly get; set; }

		internal ShaderTagId passName { readonly get; private set; }

		internal ShaderTagId[] passNames { readonly get; private set; }

		public RendererListDesc(ShaderTagId passName, CullingResults cullingResult, Camera camera)
		{
			this = default(RendererListDesc);
			this.passName = passName;
			this.passNames = null;
			this.cullingResult = cullingResult;
			this.camera = camera;
			this.layerMask = -1;
			this.renderingLayerMask = uint.MaxValue;
			this.batchLayerMask = uint.MaxValue;
			this.overrideMaterialPassIndex = 0;
			this.overrideShaderPassIndex = 0;
		}

		public RendererListDesc(ShaderTagId[] passNames, CullingResults cullingResult, Camera camera)
		{
			this = default(RendererListDesc);
			this.passNames = passNames;
			this.passName = ShaderTagId.none;
			this.cullingResult = cullingResult;
			this.camera = camera;
			this.layerMask = -1;
			this.renderingLayerMask = uint.MaxValue;
			this.batchLayerMask = uint.MaxValue;
			this.overrideMaterialPassIndex = 0;
		}

		public bool IsValid()
		{
			bool flag = this.camera == null || (this.passName == ShaderTagId.none && (this.passNames == null || this.passNames.Length == 0));
			return !flag;
		}

		public static RendererListParams ConvertToParameters(in RendererListDesc desc)
		{
			RendererListDesc rendererListDesc = desc;
			bool flag = !rendererListDesc.IsValid();
			RendererListParams result;
			if (flag)
			{
				result = RendererListParams.Invalid;
			}
			else
			{
				RendererListParams rendererListParams = default(RendererListParams);
				SortingSettings sortingSettings = new SortingSettings(desc.camera)
				{
					criteria = desc.sortingCriteria
				};
				DrawingSettings drawSettings = new DrawingSettings(RendererListDesc.s_EmptyName, sortingSettings)
				{
					perObjectData = desc.rendererConfiguration
				};
				bool flag2 = desc.passName != ShaderTagId.none;
				if (flag2)
				{
					Debug.Assert(desc.passNames == null);
					drawSettings.SetShaderPassName(0, desc.passName);
				}
				else
				{
					for (int i = 0; i < desc.passNames.Length; i++)
					{
						drawSettings.SetShaderPassName(i, desc.passNames[i]);
					}
				}
				bool flag3 = desc.overrideShader != null;
				if (flag3)
				{
					drawSettings.overrideShader = desc.overrideShader;
					drawSettings.overrideShaderPassIndex = desc.overrideShaderPassIndex;
				}
				bool flag4 = desc.overrideMaterial != null;
				if (flag4)
				{
					drawSettings.overrideMaterial = desc.overrideMaterial;
					drawSettings.overrideMaterialPassIndex = desc.overrideMaterialPassIndex;
				}
				FilteringSettings filteringSettings = new FilteringSettings(new RenderQueueRange?(desc.renderQueueRange), desc.layerMask, desc.renderingLayerMask, 0)
				{
					excludeMotionVectorObjects = desc.excludeObjectMotionVectors,
					batchLayerMask = desc.batchLayerMask
				};
				rendererListParams.cullingResults = desc.cullingResult;
				rendererListParams.drawSettings = drawSettings;
				rendererListParams.filteringSettings = filteringSettings;
				rendererListParams.tagName = ShaderTagId.none;
				rendererListParams.isPassTagName = false;
				bool flag5 = desc.stateBlock != null && desc.stateBlock != null;
				if (flag5)
				{
					NativeArray<RenderStateBlock> value = new NativeArray<RenderStateBlock>(1, Allocator.Temp, NativeArrayOptions.ClearMemory);
					value[0] = desc.stateBlock.Value;
					rendererListParams.stateBlocks = new NativeArray<RenderStateBlock>?(value);
					NativeArray<ShaderTagId> value2 = new NativeArray<ShaderTagId>(1, Allocator.Temp, NativeArrayOptions.ClearMemory);
					value2[0] = ShaderTagId.none;
					rendererListParams.tagValues = new NativeArray<ShaderTagId>?(value2);
				}
				result = rendererListParams;
			}
			return result;
		}

		public SortingCriteria sortingCriteria;

		public PerObjectData rendererConfiguration;

		public RenderQueueRange renderQueueRange;

		public RenderStateBlock? stateBlock;

		public Shader overrideShader;

		public Material overrideMaterial;

		public bool excludeObjectMotionVectors;

		public int layerMask;

		public uint renderingLayerMask;

		public int overrideMaterialPassIndex;

		public int overrideShaderPassIndex;

		private static readonly ShaderTagId s_EmptyName = new ShaderTagId("");
	}
}
