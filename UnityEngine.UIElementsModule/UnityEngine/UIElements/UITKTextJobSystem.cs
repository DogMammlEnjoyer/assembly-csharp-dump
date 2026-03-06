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
	internal class UITKTextJobSystem
	{
		public UITKTextJobSystem()
		{
			this.m_PrepareTextJobifiedCallback = new MeshGenerationCallback(this.PrepareTextJobified);
			this.m_GenerateTextJobifiedCallback = new MeshGenerationCallback(this.GenerateTextJobified);
			this.m_AddDrawEntriesCallback = new MeshGenerationCallback(this.AddDrawEntries);
		}

		private static void OnGetManagedJob(UITKTextJobSystem.ManagedJobData managedJobData)
		{
			managedJobData.vertices = null;
			managedJobData.indices = null;
			managedJobData.materials = null;
			managedJobData.renderModes = null;
			managedJobData.prepareSuccess = false;
		}

		internal void GenerateText(MeshGenerationContext mgc, TextElement textElement)
		{
			MeshGenerationNode node;
			mgc.InsertMeshGenerationNode(out node);
			UITKTextJobSystem.ManagedJobData managedJobData = UITKTextJobSystem.s_JobDataPool.Get();
			managedJobData.visualElement = textElement;
			managedJobData.node = node;
			this.textJobDatas.Add(managedJobData);
			bool flag = this.hasPendingTextWork;
			if (!flag)
			{
				this.hasPendingTextWork = true;
				this.textJobDatasHandle = GCHandle.Alloc(this.textJobDatas);
				mgc.AddMeshGenerationCallback(this.m_PrepareTextJobifiedCallback, null, MeshGenerationCallbackType.WorkThenFork, false);
			}
		}

		internal void PrepareTextJobified(MeshGenerationContext mgc, object _)
		{
			TextHandle.InitThreadArrays();
			PanelTextSettings.InitializeDefaultPanelTextSettingsIfNull();
			TextHandle.UpdateCurrentFrame();
			this.hasPendingTextWork = false;
			UITKTextJobSystem.PrepareTextJobData jobData = new UITKTextJobSystem.PrepareTextJobData
			{
				managedJobDataHandle = this.textJobDatasHandle
			};
			TextGenerator.IsExecutingJob = true;
			JobHandle jobHandle = jobData.ScheduleOrRunJob(this.textJobDatas.Count, 1, default(JobHandle));
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(this.m_GenerateTextJobifiedCallback, null, MeshGenerationCallbackType.Work, true);
		}

		private void GenerateTextJobified(MeshGenerationContext mgc, object _)
		{
			TextGenerator.IsExecutingJob = false;
			foreach (UITKTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(managedJobData.visualElement);
				if (textSettingsFrom != null)
				{
					UnicodeLineBreakingRules lineBreakingRules = textSettingsFrom.lineBreakingRules;
					if (lineBreakingRules != null)
					{
						lineBreakingRules.LoadLineBreakingRules();
					}
				}
				List<FontAsset> list = (textSettingsFrom != null) ? textSettingsFrom.fallbackOSFontAssets : null;
				bool prepareSuccess = managedJobData.prepareSuccess;
				if (!prepareSuccess)
				{
					managedJobData.visualElement.uitkTextHandle.ConvertUssToTextGenerationSettings(true, null);
					managedJobData.visualElement.uitkTextHandle.PrepareFontAsset();
				}
			}
			FontAsset.UpdateFontAssetsInUpdateQueue();
			TempMeshAllocator alloc;
			mgc.GetTempMeshAllocator(out alloc);
			UITKTextJobSystem.GenerateTextJobData jobData = new UITKTextJobSystem.GenerateTextJobData
			{
				managedJobDataHandle = this.textJobDatasHandle,
				alloc = alloc
			};
			TextHandle.UpdateCurrentFrame();
			TextGenerator.IsExecutingJob = true;
			JobHandle jobHandle = jobData.ScheduleOrRunJob(this.textJobDatas.Count, 1, default(JobHandle));
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(this.m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, true);
		}

		private static void ConvertMeshInfoToUIRVertex(MeshInfo[] meshInfos, TempMeshAllocator alloc, TextElement visualElement, ref List<Material> materials, ref List<NativeSlice<Vertex>> verticesArray, ref List<NativeSlice<ushort>> indicesArray, ref List<GlyphRenderMode> renderModes)
		{
			ObjectPool<List<Material>> obj = UITKTextJobSystem.s_MaterialsPool;
			lock (obj)
			{
				materials = UITKTextJobSystem.s_MaterialsPool.Get();
				verticesArray = UITKTextJobSystem.s_VerticesPool.Get();
				indicesArray = UITKTextJobSystem.s_IndicesPool.Get();
				renderModes = UITKTextJobSystem.s_RenderModesPool.Get();
			}
			Vector2 min = visualElement.contentRect.min;
			float inverseScale = 1f / visualElement.scaledPixelsPerPoint;
			bool hasMultipleColors = visualElement.uitkTextHandle.textInfo.hasMultipleColors;
			bool flag2 = hasMultipleColors;
			if (flag2)
			{
				visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			else
			{
				visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			foreach (MeshInfo meshInfo in meshInfos)
			{
				Debug.Assert((meshInfo.vertexCount & 3) == 0);
				int b = (int)((ulong)UIRenderDevice.maxVerticesPerPage & 18446744073709551612UL);
				int j = meshInfo.vertexCount;
				int num = 0;
				while (j > 0)
				{
					int num2 = Mathf.Min(j, b);
					int num3 = num2 >> 2;
					int indexCount = num3 * 6;
					materials.Add(meshInfo.material);
					renderModes.Add(meshInfo.glyphRenderMode);
					bool flag3 = meshInfo.glyphRenderMode != GlyphRenderMode.SMOOTH && meshInfo.glyphRenderMode != GlyphRenderMode.COLOR;
					bool isDynamicColor = meshInfo.applySDF && !hasMultipleColors && (RenderEvents.NeedsColorID(visualElement) || (flag3 && RenderEvents.NeedsTextCoreSettings(visualElement)));
					NativeSlice<Vertex> item;
					NativeSlice<ushort> item2;
					alloc.AllocateTempMesh(num2, indexCount, out item, out item2);
					int k = 0;
					int num4 = 0;
					while (k < num2)
					{
						item[k] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num], min, inverseScale, isDynamicColor, false);
						item[k + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num + 1], min, inverseScale, isDynamicColor, false);
						item[k + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num + 2], min, inverseScale, isDynamicColor, false);
						item[k + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num + 3], min, inverseScale, isDynamicColor, false);
						item2[num4] = (ushort)k;
						item2[num4 + 1] = (ushort)(k + 1);
						item2[num4 + 2] = (ushort)(k + 2);
						item2[num4 + 3] = (ushort)(k + 2);
						item2[num4 + 4] = (ushort)(k + 3);
						item2[num4 + 5] = (ushort)k;
						k += 4;
						num += 4;
						num4 += 6;
					}
					verticesArray.Add(item);
					indicesArray.Add(item2);
					j -= num2;
				}
				Debug.Assert(j == 0);
			}
		}

		private void AddDrawEntries(MeshGenerationContext mgc, object _)
		{
			TextGenerator.IsExecutingJob = false;
			foreach (UITKTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				TextElement visualElement = managedJobData.visualElement;
				mgc.Begin(managedJobData.node.GetParentEntry(), visualElement, visualElement.nestedRenderData ?? visualElement.renderData);
				visualElement.uitkTextHandle.HandleLinkAndATagCallbacks();
				Action<TextElement.GlyphsEnumerable> postProcessTextVertices = visualElement.PostProcessTextVertices;
				if (postProcessTextVertices != null)
				{
					postProcessTextVertices(new TextElement.GlyphsEnumerable(visualElement, managedJobData.vertices));
				}
				mgc.meshGenerator.DrawText(managedJobData.vertices, managedJobData.indices, managedJobData.materials, managedJobData.renderModes);
				managedJobData.visualElement.OnGenerateTextOver(mgc);
				mgc.End();
				managedJobData.Release();
			}
			this.textJobDatas.Clear();
			this.textJobDatasHandle.Free();
		}

		private static readonly ProfilerMarker k_ExecuteMarker = new ProfilerMarker("TextJob.GenerateText");

		private static readonly ProfilerMarker k_UpdateMainThreadMarker = new ProfilerMarker("TextJob.UpdateMainThread");

		private static readonly ProfilerMarker k_PrepareMainThreadMarker = new ProfilerMarker("TextJob.PrepareMainThread");

		private static readonly ProfilerMarker k_PrepareJobifiedMarker = new ProfilerMarker("TextJob.PrepareJobified");

		private GCHandle textJobDatasHandle;

		private List<UITKTextJobSystem.ManagedJobData> textJobDatas = new List<UITKTextJobSystem.ManagedJobData>();

		private bool hasPendingTextWork;

		private static ObjectPool<UITKTextJobSystem.ManagedJobData> s_JobDataPool = new ObjectPool<UITKTextJobSystem.ManagedJobData>(() => new UITKTextJobSystem.ManagedJobData(), new Action<UITKTextJobSystem.ManagedJobData>(UITKTextJobSystem.OnGetManagedJob), delegate(UITKTextJobSystem.ManagedJobData inst)
		{
			inst.visualElement = null;
		}, null, false, 10, 10000);

		private static ObjectPool<List<Material>> s_MaterialsPool = new ObjectPool<List<Material>>(() => new List<Material>(), null, delegate(List<Material> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<List<GlyphRenderMode>> s_RenderModesPool = new ObjectPool<List<GlyphRenderMode>>(() => new List<GlyphRenderMode>(), null, delegate(List<GlyphRenderMode> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<List<NativeSlice<Vertex>>> s_VerticesPool = new ObjectPool<List<NativeSlice<Vertex>>>(() => new List<NativeSlice<Vertex>>(), null, delegate(List<NativeSlice<Vertex>> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<List<NativeSlice<ushort>>> s_IndicesPool = new ObjectPool<List<NativeSlice<ushort>>>(() => new List<NativeSlice<ushort>>(), null, delegate(List<NativeSlice<ushort>> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		internal MeshGenerationCallback m_PrepareTextJobifiedCallback;

		internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

		internal MeshGenerationCallback m_AddDrawEntriesCallback;

		private class ManagedJobData
		{
			public void Release()
			{
				bool flag = this.materials != null;
				if (flag)
				{
					UITKTextJobSystem.s_MaterialsPool.Release(this.materials);
					UITKTextJobSystem.s_VerticesPool.Release(this.vertices);
					UITKTextJobSystem.s_IndicesPool.Release(this.indices);
					UITKTextJobSystem.s_RenderModesPool.Release(this.renderModes);
				}
				UITKTextJobSystem.s_JobDataPool.Release(this);
			}

			public TextElement visualElement;

			public MeshGenerationNode node;

			public List<Material> materials;

			public List<GlyphRenderMode> renderModes;

			public List<NativeSlice<Vertex>> vertices;

			public List<NativeSlice<ushort>> indices;

			public bool prepareSuccess;
		}

		private struct PrepareTextJobData : IJobParallelFor
		{
			public void Execute(int index)
			{
				List<UITKTextJobSystem.ManagedJobData> list = (List<UITKTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				UITKTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement visualElement = managedJobData.visualElement;
				managedJobData.prepareSuccess = visualElement.uitkTextHandle.ConvertUssToTextGenerationSettings(true, null);
				bool prepareSuccess = managedJobData.prepareSuccess;
				if (prepareSuccess)
				{
					managedJobData.prepareSuccess = visualElement.uitkTextHandle.PrepareFontAsset();
				}
			}

			public GCHandle managedJobDataHandle;
		}

		private struct GenerateTextJobData : IJobParallelFor
		{
			public void Execute(int index)
			{
				List<UITKTextJobSystem.ManagedJobData> list = (List<UITKTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				UITKTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement visualElement = managedJobData.visualElement;
				bool flag = visualElement.PostProcessTextVertices != null;
				if (flag)
				{
					visualElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				}
				visualElement.uitkTextHandle.UpdateMesh();
				TextInfo textInfo = visualElement.uitkTextHandle.textInfo;
				MeshInfo[] meshInfo = textInfo.meshInfo;
				List<Material> materials = null;
				List<NativeSlice<Vertex>> vertices = null;
				List<NativeSlice<ushort>> indices = null;
				List<GlyphRenderMode> renderModes = null;
				UITKTextJobSystem.ConvertMeshInfoToUIRVertex(meshInfo, this.alloc, visualElement, ref materials, ref vertices, ref indices, ref renderModes);
				managedJobData.materials = materials;
				managedJobData.vertices = vertices;
				managedJobData.indices = indices;
				managedJobData.renderModes = renderModes;
				visualElement.uitkTextHandle.HandleATag();
				visualElement.uitkTextHandle.HandleLinkTag();
			}

			public GCHandle managedJobDataHandle;

			[ReadOnly]
			public TempMeshAllocator alloc;
		}
	}
}
