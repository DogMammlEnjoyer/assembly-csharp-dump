using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Pool;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class ATGTextJobSystem
	{
		public ATGTextJobSystem()
		{
			this.m_GenerateTextJobifiedCallback = new MeshGenerationCallback(this.GenerateTextJobified);
			this.m_AddDrawEntriesCallback = new MeshGenerationCallback(this.AddDrawEntries);
		}

		public void GenerateText(MeshGenerationContext mgc, TextElement textElement)
		{
			MeshGenerationNode node;
			mgc.InsertMeshGenerationNode(out node);
			ATGTextJobSystem.ManagedJobData managedJobData = ATGTextJobSystem.s_JobDataPool.Get();
			managedJobData.textElement = textElement;
			managedJobData.node = node;
			this.textJobDatas.Add(managedJobData);
			bool flag = this.hasPendingTextWork;
			if (!flag)
			{
				this.hasPendingTextWork = true;
				this.textJobDatasHandle = GCHandle.Alloc(this.textJobDatas);
				MeshGenerationCallbackType callbackType = ATGTextJobSystem.k_IsMultiThreaded ? MeshGenerationCallbackType.Fork : MeshGenerationCallbackType.Work;
				mgc.AddMeshGenerationCallback(this.m_GenerateTextJobifiedCallback, null, callbackType, false);
			}
		}

		private void GenerateTextJobified(MeshGenerationContext mgc, object _)
		{
			ATGTextJobSystem.GenerateTextJobData jobData = new ATGTextJobSystem.GenerateTextJobData
			{
				managedJobDataHandle = this.textJobDatasHandle
			};
			bool flag = this.textJobDatas.Count > 0;
			if (flag)
			{
				this.textJobDatas[0].textElement.uitkTextHandle.InitTextLib();
			}
			FontAsset.CreateHbFaceIfNeeded();
			for (int i = 0; i < this.textJobDatas.Count; i++)
			{
				ATGTextJobSystem.ManagedJobData managedJobData = this.textJobDatas[i];
				TextElement textElement = managedJobData.textElement;
				FontAsset fontAsset = TextUtilities.GetFontAsset(textElement);
				TextUtilities.GetTextSettingsFrom(textElement).UpdateNativeTextSettings();
				fontAsset.EnsureNativeFontAssetIsCreated();
				bool flag2 = textElement.computedStyle.unityFontDefinition.fontAsset == null;
				if (flag2)
				{
					textElement.uitkTextHandle.ConvertUssToNativeTextGenerationSettings(null);
				}
			}
			bool flag3 = ATGTextJobSystem.k_IsMultiThreaded;
			if (flag3)
			{
				JobHandle jobHandle = jobData.ScheduleOrRunJob(this.textJobDatas.Count, 1, default(JobHandle));
				mgc.AddMeshGenerationJob(jobHandle);
				mgc.AddMeshGenerationCallback(this.m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, true);
			}
			else
			{
				for (int j = 0; j < this.textJobDatas.Count; j++)
				{
					jobData.Execute(j);
				}
				mgc.AddMeshGenerationCallback(this.m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, false);
			}
		}

		private void AddDrawEntries(MeshGenerationContext mgc, object _)
		{
			foreach (ATGTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				bool success = managedJobData.success;
				if (success)
				{
					NativeTextInfo textInfo = managedJobData.textInfo;
					TextElement textElement = managedJobData.textElement;
					textElement.uitkTextHandle.ProcessMeshInfos(textInfo);
					textElement.uitkTextHandle.UpdateATGTextEventHandler();
					FontAsset.UpdateFontAssetsInUpdateQueue();
					TempMeshAllocator alloc;
					mgc.GetTempMeshAllocator(out alloc);
					ATGTextJobSystem.ConvertMeshInfoToUIRVertex(textInfo.meshInfos, alloc, textElement, this.atlases, this.verticesArray, this.indicesArray, this.renderModes, this.sdfScalesArray);
					Action<TextElement.GlyphsEnumerable> postProcessTextVertices = textElement.PostProcessTextVertices;
					if (postProcessTextVertices != null)
					{
						postProcessTextVertices(new TextElement.GlyphsEnumerable(textElement, this.verticesArray, textInfo.meshInfos));
					}
					mgc.Begin(managedJobData.node.GetParentEntry(), textElement, textElement.renderData);
					mgc.meshGenerator.DrawText(this.verticesArray, this.indicesArray, this.atlases, this.renderModes, this.sdfScalesArray);
					textElement.OnGenerateTextOverNative(mgc);
					this.atlases.Clear();
					this.verticesArray.Clear();
					this.indicesArray.Clear();
					this.renderModes.Clear();
					this.sdfScalesArray.Clear();
					mgc.End();
				}
				managedJobData.Release();
			}
			this.hasPendingTextWork = false;
			this.textJobDatas.Clear();
			this.textJobDatasHandle.Free();
		}

		private static void ConvertMeshInfoToUIRVertex(ATGMeshInfo[] meshInfos, TempMeshAllocator alloc, TextElement visualElement, List<Texture2D> atlases, List<NativeSlice<Vertex>> verticesArray, List<NativeSlice<ushort>> indicesArray, List<GlyphRenderMode> renderModes, List<float> sdfScales)
		{
			float inverseScale = 1f / visualElement.scaledPixelsPerPoint;
			foreach (ATGMeshInfo atgmeshInfo in meshInfos)
			{
				int b = (int)((ulong)UIRenderDevice.maxVerticesPerPage & 18446744073709551612UL);
				bool hasMultipleColors = atgmeshInfo.hasMultipleColors;
				bool flag = hasMultipleColors;
				if (flag)
				{
					visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
				}
				else
				{
					visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
				}
				for (int j = 0; j < atgmeshInfo.textElementInfoIndicesByAtlas.Count; j++)
				{
					List<int> list = atgmeshInfo.textElementInfoIndicesByAtlas[j];
					int k;
					int num;
					for (k = list.Count * 4; k > 0; k -= num)
					{
						num = Mathf.Min(k, b);
						int num2 = num >> 2;
						int indexCount = num2 * 6;
						FontAsset fontAsset = atgmeshInfo.fontAsset;
						atlases.Add(fontAsset.atlasTextures[j]);
						renderModes.Add(fontAsset.atlasRenderMode);
						float item = 0f;
						bool flag2 = !TextGeneratorUtilities.IsBitmapRendering(renderModes[renderModes.Count - 1]) && atlases[atlases.Count - 1].format == TextureFormat.Alpha8;
						if (flag2)
						{
							item = (float)(fontAsset.atlasPadding + 1);
						}
						sdfScales.Add(item);
						bool flag3 = fontAsset.atlasRenderMode != GlyphRenderMode.SMOOTH && fontAsset.atlasRenderMode != GlyphRenderMode.COLOR;
						bool isDynamicColor = !hasMultipleColors && (RenderEvents.NeedsColorID(visualElement) || (flag3 && RenderEvents.NeedsTextCoreSettings(visualElement)));
						NativeSlice<Vertex> item2;
						NativeSlice<ushort> item3;
						alloc.AllocateTempMesh(num, indexCount, out item2, out item3);
						Vector2 min = visualElement.contentRect.min;
						int l = 0;
						int num3 = 0;
						int num4 = 0;
						while (l < num)
						{
							bool isColorGlyph = fontAsset.atlasRenderMode == GlyphRenderMode.COLOR || fontAsset.atlasRenderMode == GlyphRenderMode.COLOR_HINTED;
							NativeTextElementInfo nativeTextElementInfo = atgmeshInfo.textElementInfos[list[num3]];
							item2[l] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomLeft, min, inverseScale, isDynamicColor, isColorGlyph);
							item2[l + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topLeft, min, inverseScale, isDynamicColor, isColorGlyph);
							item2[l + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topRight, min, inverseScale, isDynamicColor, isColorGlyph);
							item2[l + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomRight, min, inverseScale, isDynamicColor, isColorGlyph);
							item3[num4] = (ushort)l;
							item3[num4 + 1] = (ushort)(l + 1);
							item3[num4 + 2] = (ushort)(l + 2);
							item3[num4 + 3] = (ushort)(l + 2);
							item3[num4 + 4] = (ushort)(l + 3);
							item3[num4 + 5] = (ushort)l;
							l += 4;
							num3++;
							num4 += 6;
						}
						verticesArray.Add(item2);
						indicesArray.Add(item3);
					}
					Debug.Assert(k == 0);
				}
			}
		}

		private GCHandle textJobDatasHandle;

		private List<ATGTextJobSystem.ManagedJobData> textJobDatas = new List<ATGTextJobSystem.ManagedJobData>();

		private bool hasPendingTextWork;

		private static ObjectPool<ATGTextJobSystem.ManagedJobData> s_JobDataPool = new ObjectPool<ATGTextJobSystem.ManagedJobData>(() => new ATGTextJobSystem.ManagedJobData(), null, delegate(ATGTextJobSystem.ManagedJobData inst)
		{
			inst.textElement = null;
		}, null, false, 10, 10000);

		internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

		internal MeshGenerationCallback m_AddDrawEntriesCallback;

		private static readonly ProfilerMarker k_GenerateTextMarker = new ProfilerMarker("ATGTextJob.GenerateText");

		private static readonly ProfilerMarker k_ATGTextJobMarker = new ProfilerMarker("ATGTextJob");

		private static readonly bool k_IsMultiThreaded = true;

		private List<Texture2D> atlases = new List<Texture2D>();

		private List<float> sdfScalesArray = new List<float>();

		private List<NativeSlice<Vertex>> verticesArray = new List<NativeSlice<Vertex>>();

		private List<NativeSlice<ushort>> indicesArray = new List<NativeSlice<ushort>>();

		private List<GlyphRenderMode> renderModes = new List<GlyphRenderMode>();

		private class ManagedJobData
		{
			public void Release()
			{
				ATGTextJobSystem.s_JobDataPool.Release(this);
			}

			public TextElement textElement;

			public MeshGenerationNode node;

			public NativeTextInfo textInfo;

			public bool success;
		}

		private struct GenerateTextJobData : IJobParallelFor
		{
			public void Execute(int index)
			{
				List<ATGTextJobSystem.ManagedJobData> list = (List<ATGTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				ATGTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement textElement = managedJobData.textElement;
				bool generateNativeSettings = textElement.computedStyle.unityFontDefinition.fontAsset != null;
				bool flag = textElement.PostProcessTextVertices != null;
				if (flag)
				{
					textElement.uitkTextHandle.CacheTextGenerationInfo();
				}
				ATGTextJobSystem.ManagedJobData managedJobData2 = managedJobData;
				ATGTextJobSystem.ManagedJobData managedJobData3 = managedJobData;
				ValueTuple<NativeTextInfo, bool> valueTuple = textElement.uitkTextHandle.UpdateNative(generateNativeSettings);
				managedJobData2.textInfo = valueTuple.Item1;
				managedJobData3.success = valueTuple.Item2;
			}

			public GCHandle managedJobDataHandle;
		}
	}
}
