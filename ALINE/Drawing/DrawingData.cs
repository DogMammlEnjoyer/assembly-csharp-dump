using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Drawing.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

namespace Drawing
{
	public class DrawingData
	{
		private int adjustedSceneModeVersion
		{
			get
			{
				return this.sceneModeVersion + (Application.isPlaying ? 1000 : 0);
			}
		}

		internal int GetNextDrawOrderIndex()
		{
			this.currentDrawOrderIndex++;
			return this.currentDrawOrderIndex;
		}

		internal void PoolMesh(Mesh mesh)
		{
			this.stagingCachedMeshes.Add(mesh);
		}

		private void SortPooledMeshes()
		{
			this.cachedMeshes.Sort((Mesh a, Mesh b) => b.vertexCount - a.vertexCount);
		}

		internal Mesh GetMesh(int desiredVertexCount)
		{
			if (this.cachedMeshes.Count > 0)
			{
				int num = 0;
				int i = this.cachedMeshes.Count;
				while (i > num + 1)
				{
					int num2 = (num + i) / 2;
					if (this.cachedMeshes[num2].vertexCount < desiredVertexCount)
					{
						i = num2;
					}
					else
					{
						num = num2;
					}
				}
				Mesh result = this.cachedMeshes[num];
				if (num == 0)
				{
					this.lastTimeLargestCachedMeshWasUsed = this.version;
				}
				this.cachedMeshes.RemoveAt(num);
				return result;
			}
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			mesh.MarkDynamic();
			return mesh;
		}

		internal void LoadFontDataIfNecessary()
		{
			if (this.fontData.material == null)
			{
				SDFFont font = DefaultFonts.LoadDefaultFont();
				this.fontData.Dispose();
				this.fontData = new SDFLookupData(font);
			}
		}

		private static float CurrentTime
		{
			get
			{
				if (!Application.isPlaying)
				{
					return Time.realtimeSinceStartup;
				}
				return Time.time;
			}
		}

		private unsafe static void UpdateTime()
		{
			*SharedDrawingData.BurstTime.Data = DrawingData.CurrentTime;
		}

		public CommandBuilder GetBuilder(bool renderInGame = false)
		{
			DrawingData.UpdateTime();
			return new CommandBuilder(this, DrawingData.Hasher.NotSupplied, this.frameRedrawScope, default(RedrawScope), !renderInGame, false, this.adjustedSceneModeVersion);
		}

		internal CommandBuilder GetBuiltInBuilder(bool renderInGame = false)
		{
			DrawingData.UpdateTime();
			return new CommandBuilder(this, DrawingData.Hasher.NotSupplied, this.frameRedrawScope, default(RedrawScope), !renderInGame, true, this.adjustedSceneModeVersion);
		}

		public CommandBuilder GetBuilder(RedrawScope redrawScope, bool renderInGame = false)
		{
			DrawingData.UpdateTime();
			return new CommandBuilder(this, DrawingData.Hasher.NotSupplied, this.frameRedrawScope, redrawScope, !renderInGame, false, this.adjustedSceneModeVersion);
		}

		public CommandBuilder GetBuilder(DrawingData.Hasher hasher, RedrawScope redrawScope = default(RedrawScope), bool renderInGame = false)
		{
			if (!hasher.Equals(DrawingData.Hasher.NotSupplied))
			{
				this.DiscardData(hasher);
			}
			DrawingData.UpdateTime();
			return new CommandBuilder(this, hasher, this.frameRedrawScope, redrawScope, !renderInGame, false, this.adjustedSceneModeVersion);
		}

		public DrawingSettings.Settings settingsRef
		{
			get
			{
				if (this.settingsAsset == null)
				{
					this.settingsAsset = DrawingSettings.GetSettingsAsset();
					if (this.settingsAsset == null)
					{
						throw new InvalidOperationException("ALINE settings could not be found");
					}
				}
				return this.settingsAsset.settings;
			}
		}

		public int version { get; private set; } = 1;

		private void DiscardData(DrawingData.Hasher hasher)
		{
			this.processedData.ReleaseAllWithHash(this, hasher);
		}

		internal void OnChangingPlayMode()
		{
			this.sceneModeVersion++;
		}

		public bool Draw(DrawingData.Hasher hasher)
		{
			if (hasher.Equals(DrawingData.Hasher.NotSupplied))
			{
				throw new ArgumentException("Invalid hash value");
			}
			return this.processedData.SetVersion(hasher, this.version);
		}

		public bool Draw(DrawingData.Hasher hasher, RedrawScope scope)
		{
			if (hasher.Equals(DrawingData.Hasher.NotSupplied))
			{
				throw new ArgumentException("Invalid hash value");
			}
			this.processedData.SetCustomScope(hasher, scope);
			return this.processedData.SetVersion(hasher, this.version);
		}

		internal void Draw(RedrawScope scope)
		{
			if (scope.id != 0)
			{
				this.processedData.SetVersion(scope, this.version);
			}
		}

		internal void DrawUntilDisposed(RedrawScope scope)
		{
			if (scope.id != 0)
			{
				this.Draw(scope);
				this.persistentRedrawScopes.Add(scope.id);
			}
		}

		internal void DisposeRedrawScope(RedrawScope scope)
		{
			if (scope.id != 0)
			{
				this.processedData.SetVersion(scope, -1);
				this.persistentRedrawScopes.Remove(scope.id);
			}
		}

		public void TickFramePreRender()
		{
			this.data.DisposeCommandBuildersWithJobDependencies(this);
			this.processedData.FilterOldPersistentCommands(this.version, this.lastTickVersion, DrawingData.CurrentTime, this.adjustedSceneModeVersion);
			foreach (int id in this.persistentRedrawScopes)
			{
				this.processedData.SetVersion(new RedrawScope(this, id), this.version);
			}
			this.processedData.ReleaseDataOlderThan(this, this.lastTickVersion2 + 1);
			this.lastTickVersion2 = this.lastTickVersion;
			this.lastTickVersion = this.version;
			this.currentDrawOrderIndex = 0;
			this.cachedMeshes.AddRange(this.stagingCachedMeshes);
			this.stagingCachedMeshes.Clear();
			this.SortPooledMeshes();
			if (this.version - this.lastTimeLargestCachedMeshWasUsed > 60 && this.cachedMeshes.Count > 0)
			{
				Object.DestroyImmediate(this.cachedMeshes[0]);
				this.cachedMeshes.RemoveAt(0);
				this.lastTimeLargestCachedMeshWasUsed = this.version;
			}
		}

		public void PostRenderCleanup()
		{
			this.data.ReleaseAllUnused();
			int version = this.version;
			this.version = version + 1;
		}

		private int totalMemoryUsage
		{
			get
			{
				return this.data.memoryUsage + this.processedData.memoryUsage;
			}
		}

		private void LoadMaterials()
		{
			if (this.surfaceMaterial == null)
			{
				this.surfaceMaterial = Resources.Load<Material>("aline_surface_mat");
			}
			if (this.lineMaterial == null)
			{
				this.lineMaterial = Resources.Load<Material>("aline_outline_mat");
			}
			if (this.fontData.material == null)
			{
				SDFFont font = DefaultFonts.LoadDefaultFont();
				this.fontData.Dispose();
				this.fontData = new SDFLookupData(font);
			}
		}

		public DrawingData()
		{
			this.gizmosHandle = GCHandle.Alloc(this, GCHandleType.Weak);
			this.LoadMaterials();
		}

		private static int CeilLog2(int x)
		{
			return (int)math.ceil(math.log2((float)x));
		}

		public void Render(Camera cam, bool allowGizmos, DrawingData.CommandBufferWrapper commandBuffer, bool allowCameraDefault)
		{
			this.LoadMaterials();
			if (this.surfaceMaterial == null || this.lineMaterial == null)
			{
				return;
			}
			Plane[] planes = this.frustrumPlanes;
			GeometryUtility.CalculateFrustumPlanes(cam, planes);
			DrawingData.Range range;
			if (!this.cameraVersions.TryGetValue(cam, out range))
			{
				range = new DrawingData.Range
				{
					start = int.MinValue,
					end = int.MinValue
				};
			}
			if (range.end > this.lastTickVersion)
			{
				range.end = this.version + 1;
			}
			else
			{
				range = new DrawingData.Range
				{
					start = range.end,
					end = this.version + 1
				};
			}
			range.start = Mathf.Max(range.start, this.lastTickVersion2 + 1);
			DrawingSettings.Settings settingsRef = this.settingsRef;
			bool flag = false;
			commandBuffer.SetWireframe(false);
			if (!flag)
			{
				this.processedData.SubmitMeshes(this, cam, range.start, allowGizmos, allowCameraDefault);
				this.meshes.Clear();
				this.processedData.CollectMeshes(range.start, this.meshes, cam, allowGizmos, allowCameraDefault);
				this.processedData.PoolDynamicMeshes(this);
				this.meshes.Sort(DrawingData.meshSorter);
				int nameID = Shader.PropertyToID("_Color");
				int nameID2 = Shader.PropertyToID("_FadeColor");
				Color color = new Color(1f, 1f, 1f, settingsRef.solidOpacity);
				Color value = new Color(1f, 1f, 1f, settingsRef.solidOpacityBehindObjects);
				Color value2 = new Color(1f, 1f, 1f, settingsRef.lineOpacity);
				Color value3 = new Color(1f, 1f, 1f, settingsRef.lineOpacityBehindObjects);
				Color value4 = new Color(1f, 1f, 1f, settingsRef.textOpacity);
				Color value5 = new Color(1f, 1f, 1f, settingsRef.textOpacityBehindObjects);
				int i = 0;
				while (i < this.meshes.Count)
				{
					int num = i + 1;
					DrawingData.MeshType meshType = this.meshes[i].type & DrawingData.MeshType.BaseType;
					while (num < this.meshes.Count && (this.meshes[num].type & DrawingData.MeshType.BaseType) == meshType)
					{
						num++;
					}
					this.customMaterialProperties.Clear();
					Material material;
					switch (meshType)
					{
					case DrawingData.MeshType.Solid:
						material = this.surfaceMaterial;
						this.customMaterialProperties.SetColor(nameID, color);
						this.customMaterialProperties.SetColor(nameID2, value);
						break;
					case DrawingData.MeshType.Lines:
						material = this.lineMaterial;
						this.customMaterialProperties.SetColor(nameID, value2);
						this.customMaterialProperties.SetColor(nameID2, value3);
						break;
					case DrawingData.MeshType.Solid | DrawingData.MeshType.Lines:
						goto IL_2E5;
					case DrawingData.MeshType.Text:
						material = this.fontData.material;
						this.customMaterialProperties.SetColor(nameID, value4);
						this.customMaterialProperties.SetColor(nameID2, value5);
						break;
					default:
						goto IL_2E5;
					}
					for (int j = 0; j < material.passCount; j++)
					{
						for (int k = i; k < num; k++)
						{
							DrawingData.RenderedMeshWithType renderedMeshWithType = this.meshes[k];
							if ((renderedMeshWithType.type & DrawingData.MeshType.Custom) != (DrawingData.MeshType)0)
							{
								if (GeometryUtility.TestPlanesAABB(planes, DrawingData.TransformBoundingBox(renderedMeshWithType.matrix, renderedMeshWithType.mesh.bounds)))
								{
									this.customMaterialProperties.SetColor(nameID, color * renderedMeshWithType.color);
									commandBuffer.DrawMesh(renderedMeshWithType.mesh, renderedMeshWithType.matrix, material, 0, j, this.customMaterialProperties);
									this.customMaterialProperties.SetColor(nameID, color);
								}
							}
							else if (GeometryUtility.TestPlanesAABB(planes, renderedMeshWithType.mesh.bounds))
							{
								commandBuffer.DrawMesh(renderedMeshWithType.mesh, Matrix4x4.identity, material, 0, j, this.customMaterialProperties);
							}
						}
					}
					i = num;
					continue;
					IL_2E5:
					throw new InvalidOperationException("Invalid mesh type");
				}
				this.meshes.Clear();
			}
			this.cameraVersions[cam] = range;
		}

		private static Bounds TransformBoundingBox(Matrix4x4 matrix, Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			Bounds result = new Bounds(matrix.MultiplyPoint(min), Vector3.zero);
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(min.x, min.y, max.z)));
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(min.x, max.y, min.z)));
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(min.x, max.y, max.z)));
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(max.x, min.y, min.z)));
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(max.x, min.y, max.z)));
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(max.x, max.y, min.z)));
			result.Encapsulate(matrix.MultiplyPoint(new Vector3(max.x, max.y, max.z)));
			return result;
		}

		public void ClearData()
		{
			this.gizmosHandle.Free();
			this.data.Dispose();
			this.processedData.Dispose(this);
			for (int i = 0; i < this.cachedMeshes.Count; i++)
			{
				Object.DestroyImmediate(this.cachedMeshes[i]);
			}
			this.cachedMeshes.Clear();
			this.fontData.Dispose();
		}

		internal DrawingData.BuilderDataContainer data;

		internal DrawingData.ProcessedBuilderDataContainer processedData;

		private List<DrawingData.RenderedMeshWithType> meshes = new List<DrawingData.RenderedMeshWithType>();

		private List<Mesh> cachedMeshes = new List<Mesh>();

		private List<Mesh> stagingCachedMeshes = new List<Mesh>();

		private int lastTimeLargestCachedMeshWasUsed;

		internal SDFLookupData fontData;

		private int currentDrawOrderIndex;

		internal int sceneModeVersion;

		public Material surfaceMaterial;

		public Material lineMaterial;

		public Material textMaterial;

		public DrawingSettings settingsAsset;

		private int lastTickVersion;

		private int lastTickVersion2;

		private HashSet<int> persistentRedrawScopes = new HashSet<int>();

		internal GCHandle gizmosHandle;

		public RedrawScope frameRedrawScope;

		private Dictionary<Camera, DrawingData.Range> cameraVersions = new Dictionary<Camera, DrawingData.Range>();

		internal static readonly ProfilerMarker MarkerScheduleJobs = new ProfilerMarker("ScheduleJobs");

		internal static readonly ProfilerMarker MarkerAwaitUserDependencies = new ProfilerMarker("Await user dependencies");

		internal static readonly ProfilerMarker MarkerSchedule = new ProfilerMarker("Schedule");

		internal static readonly ProfilerMarker MarkerBuild = new ProfilerMarker("Build");

		internal static readonly ProfilerMarker MarkerPool = new ProfilerMarker("Pool");

		internal static readonly ProfilerMarker MarkerRelease = new ProfilerMarker("Release");

		internal static readonly ProfilerMarker MarkerBuildMeshes = new ProfilerMarker("Build Meshes");

		internal static readonly ProfilerMarker MarkerCollectMeshes = new ProfilerMarker("Collect Meshes");

		internal static readonly ProfilerMarker MarkerSortMeshes = new ProfilerMarker("Sort Meshes");

		internal static readonly ProfilerMarker LeakTracking = new ProfilerMarker("RedrawScope Leak Tracking");

		private static readonly DrawingData.MeshCompareByDrawingOrder meshSorter = new DrawingData.MeshCompareByDrawingOrder();

		private Plane[] frustrumPlanes = new Plane[6];

		private MaterialPropertyBlock customMaterialProperties = new MaterialPropertyBlock();

		public struct Hasher : IEquatable<DrawingData.Hasher>
		{
			public static DrawingData.Hasher NotSupplied
			{
				get
				{
					return new DrawingData.Hasher
					{
						hash = ulong.MaxValue
					};
				}
			}

			public static DrawingData.Hasher Create<T>(T init)
			{
				DrawingData.Hasher result = default(DrawingData.Hasher);
				result.Add<T>(init);
				return result;
			}

			public void Add<T>(T hash)
			{
				this.hash = (1572869UL * this.hash ^ (ulong)((long)hash.GetHashCode() + 12289L));
			}

			public ulong Hash
			{
				get
				{
					return this.hash;
				}
			}

			public override int GetHashCode()
			{
				return (int)this.hash;
			}

			public bool Equals(DrawingData.Hasher other)
			{
				return this.hash == other.hash;
			}

			private ulong hash;
		}

		internal struct ProcessedBuilderData
		{
			public bool isValid
			{
				get
				{
					return this.type > DrawingData.ProcessedBuilderData.Type.Invalid;
				}
			}

			public unsafe UnsafeAppendBuffer* splitterOutputPtr
			{
				get
				{
					return &((DrawingData.ProcessedBuilderData.MeshBuffers*)this.temporaryMeshBuffers.GetUnsafePtr<DrawingData.ProcessedBuilderData.MeshBuffers>())->splitterOutput;
				}
			}

			public void Init(DrawingData.ProcessedBuilderData.Type type, DrawingData.BuilderData.Meta meta)
			{
				this.submitted = false;
				this.type = type;
				this.meta = meta;
				if (this.meshes == null)
				{
					this.meshes = new List<DrawingData.MeshWithType>();
				}
				if (!this.temporaryMeshBuffers.IsCreated)
				{
					this.temporaryMeshBuffers = new NativeArray<DrawingData.ProcessedBuilderData.MeshBuffers>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					this.temporaryMeshBuffers[0] = new DrawingData.ProcessedBuilderData.MeshBuffers(Allocator.Persistent);
				}
			}

			public unsafe void SetSplitterJob(DrawingData gizmos, JobHandle splitterJob)
			{
				this.splitterJob = splitterJob;
				if (this.type == DrawingData.ProcessedBuilderData.Type.Static)
				{
					GeometryBuilder.CameraInfo cameraInfo = new GeometryBuilder.CameraInfo(null);
					this.buildJob = GeometryBuilder.Build(gizmos, (DrawingData.ProcessedBuilderData.MeshBuffers*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<DrawingData.ProcessedBuilderData.MeshBuffers>(this.temporaryMeshBuffers), ref cameraInfo, splitterJob);
					DrawingData.ProcessedBuilderData.SubmittedJobs++;
					if (DrawingData.ProcessedBuilderData.SubmittedJobs % 8 == 0)
					{
						JobHandle.ScheduleBatchedJobs();
					}
				}
			}

			public unsafe void SchedulePersistFilter(int version, int lastTickVersion, float time, int sceneModeVersion)
			{
				if (this.type != DrawingData.ProcessedBuilderData.Type.Persistent)
				{
					throw new InvalidOperationException();
				}
				if (this.meta.sceneModeVersion != sceneModeVersion)
				{
					this.meta.version = -1;
					return;
				}
				if (this.meta.version < lastTickVersion || this.submitted)
				{
					this.splitterJob.Complete();
					this.meta.version = version;
					if (this.temporaryMeshBuffers[0].splitterOutput.Length == 0)
					{
						this.meta.version = -1;
						return;
					}
					this.buildJob.Complete();
					this.splitterJob = new PersistentFilterJob
					{
						buffer = &((DrawingData.ProcessedBuilderData.MeshBuffers*)this.temporaryMeshBuffers.GetUnsafePtr<DrawingData.ProcessedBuilderData.MeshBuffers>())->splitterOutput,
						time = time
					}.Schedule(this.splitterJob);
				}
			}

			public bool IsValidForCamera(Camera camera, bool allowGizmos, bool allowCameraDefault)
			{
				if (!allowGizmos && this.meta.isGizmos)
				{
					return false;
				}
				if (this.meta.cameraTargets != null)
				{
					return this.meta.cameraTargets.Contains(camera);
				}
				return allowCameraDefault;
			}

			public unsafe void Schedule(DrawingData gizmos, ref GeometryBuilder.CameraInfo cameraInfo)
			{
				if (this.type != DrawingData.ProcessedBuilderData.Type.Static)
				{
					this.buildJob = GeometryBuilder.Build(gizmos, (DrawingData.ProcessedBuilderData.MeshBuffers*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<DrawingData.ProcessedBuilderData.MeshBuffers>(this.temporaryMeshBuffers), ref cameraInfo, this.splitterJob);
				}
			}

			public unsafe void BuildMeshes(DrawingData gizmos)
			{
				if (this.type == DrawingData.ProcessedBuilderData.Type.Static && this.submitted)
				{
					return;
				}
				this.buildJob.Complete();
				GeometryBuilder.BuildMesh(gizmos, this.meshes, (DrawingData.ProcessedBuilderData.MeshBuffers*)this.temporaryMeshBuffers.GetUnsafePtr<DrawingData.ProcessedBuilderData.MeshBuffers>());
				this.submitted = true;
			}

			public unsafe void CollectMeshes(List<DrawingData.RenderedMeshWithType> meshes)
			{
				List<DrawingData.MeshWithType> list = this.meshes;
				int num = 0;
				UnsafeAppendBuffer capturedState = this.temporaryMeshBuffers[0].capturedState;
				int num2 = capturedState.Length / UnsafeUtility.SizeOf<DrawingData.ProcessedBuilderData.CapturedState>();
				for (int i = 0; i < list.Count; i++)
				{
					Color color;
					Matrix4x4 matrix;
					int drawingOrderIndex;
					if ((list[i].type & DrawingData.MeshType.Custom) != (DrawingData.MeshType)0)
					{
						DrawingData.ProcessedBuilderData.CapturedState capturedState2 = *(DrawingData.ProcessedBuilderData.CapturedState*)(capturedState.Ptr + (IntPtr)num * (IntPtr)sizeof(DrawingData.ProcessedBuilderData.CapturedState));
						color = capturedState2.color;
						matrix = capturedState2.matrix;
						num++;
						drawingOrderIndex = this.meta.drawOrderIndex + 1;
					}
					else
					{
						color = Color.white;
						matrix = Matrix4x4.identity;
						drawingOrderIndex = this.meta.drawOrderIndex;
					}
					meshes.Add(new DrawingData.RenderedMeshWithType
					{
						mesh = list[i].mesh,
						type = list[i].type,
						drawingOrderIndex = drawingOrderIndex,
						color = color,
						matrix = matrix
					});
				}
			}

			private void PoolMeshes(DrawingData gizmos, bool includeCustom)
			{
				if (!this.isValid)
				{
					throw new InvalidOperationException();
				}
				int num = 0;
				for (int i = 0; i < this.meshes.Count; i++)
				{
					if ((this.meshes[i].type & DrawingData.MeshType.Custom) == (DrawingData.MeshType)0 || (includeCustom && (this.meshes[i].type & DrawingData.MeshType.Pool) != (DrawingData.MeshType)0))
					{
						gizmos.PoolMesh(this.meshes[i].mesh);
					}
					else
					{
						this.meshes[num] = this.meshes[i];
						num++;
					}
				}
				this.meshes.RemoveRange(num, this.meshes.Count - num);
			}

			public void PoolDynamicMeshes(DrawingData gizmos)
			{
				if (this.type == DrawingData.ProcessedBuilderData.Type.Static && this.submitted)
				{
					return;
				}
				this.PoolMeshes(gizmos, false);
			}

			public void Release(DrawingData gizmos)
			{
				if (!this.isValid)
				{
					throw new InvalidOperationException();
				}
				this.PoolMeshes(gizmos, true);
				this.meshes.Clear();
				this.type = DrawingData.ProcessedBuilderData.Type.Invalid;
				this.splitterJob.Complete();
				this.buildJob.Complete();
				DrawingData.ProcessedBuilderData.MeshBuffers value = this.temporaryMeshBuffers[0];
				value.DisposeIfLarge();
				this.temporaryMeshBuffers[0] = value;
			}

			public void Dispose()
			{
				if (this.isValid)
				{
					throw new InvalidOperationException();
				}
				this.splitterJob.Complete();
				this.buildJob.Complete();
				if (this.temporaryMeshBuffers.IsCreated)
				{
					this.temporaryMeshBuffers[0].Dispose();
					this.temporaryMeshBuffers.Dispose();
				}
			}

			public DrawingData.ProcessedBuilderData.Type type;

			public DrawingData.BuilderData.Meta meta;

			private bool submitted;

			public NativeArray<DrawingData.ProcessedBuilderData.MeshBuffers> temporaryMeshBuffers;

			private JobHandle buildJob;

			private JobHandle splitterJob;

			public List<DrawingData.MeshWithType> meshes;

			private static int SubmittedJobs;

			public enum Type
			{
				Invalid,
				Static,
				Dynamic,
				Persistent
			}

			public struct CapturedState
			{
				public Matrix4x4 matrix;

				public Color color;
			}

			public struct MeshBuffers
			{
				public MeshBuffers(Allocator allocator)
				{
					this.splitterOutput = new UnsafeAppendBuffer(0, 4, allocator);
					this.vertices = new UnsafeAppendBuffer(0, 4, allocator);
					this.triangles = new UnsafeAppendBuffer(0, 4, allocator);
					this.solidVertices = new UnsafeAppendBuffer(0, 4, allocator);
					this.solidTriangles = new UnsafeAppendBuffer(0, 4, allocator);
					this.textVertices = new UnsafeAppendBuffer(0, 4, allocator);
					this.textTriangles = new UnsafeAppendBuffer(0, 4, allocator);
					this.capturedState = new UnsafeAppendBuffer(0, 4, allocator);
					this.bounds = default(Bounds);
				}

				public void Dispose()
				{
					this.splitterOutput.Dispose();
					this.vertices.Dispose();
					this.triangles.Dispose();
					this.solidVertices.Dispose();
					this.solidTriangles.Dispose();
					this.textVertices.Dispose();
					this.textTriangles.Dispose();
					this.capturedState.Dispose();
				}

				private static void DisposeIfLarge(ref UnsafeAppendBuffer ls)
				{
					if (ls.Length * 3 < ls.Capacity && ls.Capacity > 1024)
					{
						AllocatorManager.AllocatorHandle allocator = ls.Allocator;
						ls.Dispose();
						ls = new UnsafeAppendBuffer(0, 4, allocator);
					}
				}

				public void DisposeIfLarge()
				{
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.splitterOutput);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.vertices);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.triangles);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.solidVertices);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.solidTriangles);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.textVertices);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.textTriangles);
					DrawingData.ProcessedBuilderData.MeshBuffers.DisposeIfLarge(ref this.capturedState);
				}

				public UnsafeAppendBuffer splitterOutput;

				public UnsafeAppendBuffer vertices;

				public UnsafeAppendBuffer triangles;

				public UnsafeAppendBuffer solidVertices;

				public UnsafeAppendBuffer solidTriangles;

				public UnsafeAppendBuffer textVertices;

				public UnsafeAppendBuffer textTriangles;

				public UnsafeAppendBuffer capturedState;

				public Bounds bounds;
			}
		}

		internal struct SubmittedMesh
		{
			public Mesh mesh;

			public bool temporary;
		}

		[BurstCompile]
		internal struct BuilderData : IDisposable
		{
			public DrawingData.BuilderData.State state { readonly get; private set; }

			public void Reserve(int dataIndex, bool isBuiltInCommandBuilder)
			{
				if (this.state != DrawingData.BuilderData.State.Free)
				{
					throw new InvalidOperationException();
				}
				this.state = DrawingData.BuilderData.State.Reserved;
				this.packedMeta = new DrawingData.BuilderData.BitPackedMeta(dataIndex, DrawingData.BuilderData.UniqueIDCounter++ & 32767, isBuiltInCommandBuilder);
			}

			public void Init(DrawingData.Hasher hasher, RedrawScope frameRedrawScope, RedrawScope customRedrawScope, bool isGizmos, int drawOrderIndex, int sceneModeVersion)
			{
				if (this.state != DrawingData.BuilderData.State.Reserved)
				{
					throw new InvalidOperationException();
				}
				this.meta = new DrawingData.BuilderData.Meta
				{
					hasher = hasher,
					redrawScope1 = frameRedrawScope,
					redrawScope2 = customRedrawScope,
					isGizmos = isGizmos,
					version = 0,
					drawOrderIndex = drawOrderIndex,
					sceneModeVersion = sceneModeVersion,
					cameraTargets = null
				};
				if (this.meshes == null)
				{
					this.meshes = new List<DrawingData.SubmittedMesh>();
				}
				if (!this.commandBuffers.IsCreated)
				{
					this.commandBuffers = new NativeArray<UnsafeAppendBuffer>(JobsUtility.ThreadIndexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					for (int i = 0; i < this.commandBuffers.Length; i++)
					{
						this.commandBuffers[i] = new UnsafeAppendBuffer(0, 4, Allocator.Persistent);
					}
				}
				this.state = DrawingData.BuilderData.State.Initialized;
			}

			public unsafe UnsafeAppendBuffer* bufferPtr
			{
				get
				{
					return (UnsafeAppendBuffer*)this.commandBuffers.GetUnsafePtr<UnsafeAppendBuffer>();
				}
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(DrawingData.BuilderData.AnyBuffersWrittenToDelegate))]
			private unsafe static bool AnyBuffersWrittenTo(UnsafeAppendBuffer* buffers, int numBuffers)
			{
				return DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$BurstDirectCall.Invoke(buffers, numBuffers);
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(DrawingData.BuilderData.AnyBuffersWrittenToDelegate))]
			private unsafe static void ResetAllBuffers(UnsafeAppendBuffer* buffers, int numBuffers)
			{
				DrawingData.BuilderData.ResetAllBuffers_000002FC$BurstDirectCall.Invoke(buffers, numBuffers);
			}

			public void SubmitWithDependency(GCHandle gcHandle, JobHandle dependency, AllowedDelay allowedDelay)
			{
				this.state = DrawingData.BuilderData.State.WaitingForUserDefinedJob;
				this.disposeDependency = dependency;
				this.disposeDependencyDelay = allowedDelay;
				this.disposeGCHandle = gcHandle;
			}

			public unsafe void Submit(DrawingData gizmos)
			{
				if (this.state != DrawingData.BuilderData.State.Initialized)
				{
					throw new InvalidOperationException();
				}
				if (this.meshes.Count == 0 && !DrawingData.BuilderData.AnyBuffersWrittenToInvoke((UnsafeAppendBuffer*)this.commandBuffers.GetUnsafeReadOnlyPtr<UnsafeAppendBuffer>(), this.commandBuffers.Length))
				{
					this.Release();
					return;
				}
				this.meta.version = gizmos.version;
				DrawingData.BuilderData.Meta meta = this.meta;
				meta.drawOrderIndex = this.meta.drawOrderIndex * 3;
				int index = gizmos.processedData.Reserve(DrawingData.ProcessedBuilderData.Type.Static, meta);
				meta.drawOrderIndex = this.meta.drawOrderIndex * 3 + 1;
				int index2 = gizmos.processedData.Reserve(DrawingData.ProcessedBuilderData.Type.Dynamic, meta);
				meta.drawOrderIndex = this.meta.drawOrderIndex + 1000000;
				int index3 = gizmos.processedData.Reserve(DrawingData.ProcessedBuilderData.Type.Persistent, meta);
				this.splitterJob = new StreamSplitter
				{
					inputBuffers = this.commandBuffers,
					staticBuffer = gizmos.processedData.Get(index).splitterOutputPtr,
					dynamicBuffer = gizmos.processedData.Get(index2).splitterOutputPtr,
					persistentBuffer = gizmos.processedData.Get(index3).splitterOutputPtr
				}.Schedule(default(JobHandle));
				gizmos.processedData.Get(index).SetSplitterJob(gizmos, this.splitterJob);
				gizmos.processedData.Get(index2).SetSplitterJob(gizmos, this.splitterJob);
				gizmos.processedData.Get(index3).SetSplitterJob(gizmos, this.splitterJob);
				if (this.meshes.Count > 0)
				{
					List<DrawingData.MeshWithType> list = gizmos.processedData.Get(index2).meshes;
					for (int i = 0; i < this.meshes.Count; i++)
					{
						list.Add(new DrawingData.MeshWithType
						{
							mesh = this.meshes[i].mesh,
							type = (DrawingData.MeshType.Solid | DrawingData.MeshType.Custom | (this.meshes[i].temporary ? DrawingData.MeshType.Pool : ((DrawingData.MeshType)0)))
						});
					}
					this.meshes.Clear();
				}
				this.state = DrawingData.BuilderData.State.WaitingForSplitter;
			}

			public void CheckJobDependency(DrawingData gizmos, bool allowBlocking)
			{
				if (this.state == DrawingData.BuilderData.State.WaitingForUserDefinedJob && (this.disposeDependency.IsCompleted || (allowBlocking && this.disposeDependencyDelay == AllowedDelay.EndOfFrame)))
				{
					this.disposeDependency.Complete();
					this.disposeDependency = default(JobHandle);
					this.disposeGCHandle.Free();
					this.state = DrawingData.BuilderData.State.Initialized;
					this.Submit(gizmos);
				}
			}

			public void Release()
			{
				if (this.state == DrawingData.BuilderData.State.Free)
				{
					throw new InvalidOperationException();
				}
				this.state = DrawingData.BuilderData.State.Free;
				this.ClearData();
			}

			private unsafe void ClearData()
			{
				this.disposeDependency.Complete();
				this.splitterJob.Complete();
				this.meta = default(DrawingData.BuilderData.Meta);
				this.disposeDependency = default(JobHandle);
				this.preventDispose = false;
				this.meshes.Clear();
				DrawingData.BuilderData.ResetAllBuffers((UnsafeAppendBuffer*)this.commandBuffers.GetUnsafePtr<UnsafeAppendBuffer>(), this.commandBuffers.Length);
			}

			public void Dispose()
			{
				if (this.state == DrawingData.BuilderData.State.WaitingForUserDefinedJob)
				{
					this.disposeDependency.Complete();
					this.disposeGCHandle.Free();
					this.state = DrawingData.BuilderData.State.WaitingForSplitter;
				}
				if (this.state == DrawingData.BuilderData.State.Reserved || this.state == DrawingData.BuilderData.State.Initialized || this.state == DrawingData.BuilderData.State.WaitingForUserDefinedJob)
				{
					Debug.LogError("Drawing data is being destroyed, but a drawing instance is still active. Are you sure you have called Dispose on all drawing instances? This will cause a memory leak!");
					return;
				}
				this.splitterJob.Complete();
				if (this.commandBuffers.IsCreated)
				{
					for (int i = 0; i < this.commandBuffers.Length; i++)
					{
						this.commandBuffers[i].Dispose();
					}
					this.commandBuffers.Dispose();
				}
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(DrawingData.BuilderData.AnyBuffersWrittenToDelegate))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe static bool AnyBuffersWrittenTo$BurstManaged(UnsafeAppendBuffer* buffers, int numBuffers)
			{
				bool flag = false;
				for (int i = 0; i < numBuffers; i++)
				{
					flag |= (buffers[i].Length > 0);
				}
				return flag;
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(DrawingData.BuilderData.AnyBuffersWrittenToDelegate))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe static void ResetAllBuffers$BurstManaged(UnsafeAppendBuffer* buffers, int numBuffers)
			{
				for (int i = 0; i < numBuffers; i++)
				{
					buffers[i].Reset();
				}
			}

			public DrawingData.BuilderData.BitPackedMeta packedMeta;

			public List<DrawingData.SubmittedMesh> meshes;

			public NativeArray<UnsafeAppendBuffer> commandBuffers;

			public bool preventDispose;

			private JobHandle splitterJob;

			private JobHandle disposeDependency;

			private AllowedDelay disposeDependencyDelay;

			private GCHandle disposeGCHandle;

			public DrawingData.BuilderData.Meta meta;

			private static int UniqueIDCounter = 0;

			private static readonly DrawingData.BuilderData.AnyBuffersWrittenToDelegate AnyBuffersWrittenToInvoke = BurstCompiler.CompileFunctionPointer<DrawingData.BuilderData.AnyBuffersWrittenToDelegate>(new DrawingData.BuilderData.AnyBuffersWrittenToDelegate(DrawingData.BuilderData.AnyBuffersWrittenTo)).Invoke;

			private static readonly DrawingData.BuilderData.ResetAllBuffersToDelegate ResetAllBuffersToInvoke = BurstCompiler.CompileFunctionPointer<DrawingData.BuilderData.ResetAllBuffersToDelegate>(new DrawingData.BuilderData.ResetAllBuffersToDelegate(DrawingData.BuilderData.ResetAllBuffers)).Invoke;

			public enum State
			{
				Free,
				Reserved,
				Initialized,
				WaitingForSplitter,
				WaitingForUserDefinedJob
			}

			public struct Meta
			{
				public DrawingData.Hasher hasher;

				public RedrawScope redrawScope1;

				public RedrawScope redrawScope2;

				public int version;

				public bool isGizmos;

				public int sceneModeVersion;

				public int drawOrderIndex;

				public Camera[] cameraTargets;
			}

			public struct BitPackedMeta
			{
				public BitPackedMeta(int dataIndex, int uniqueID, bool isBuiltInCommandBuilder)
				{
					if (dataIndex > 65535)
					{
						throw new Exception("Too many command builders active. Are some command builders not being disposed?");
					}
					this.flags = (uint)(dataIndex | uniqueID << 17 | (isBuiltInCommandBuilder ? 65536 : 0));
				}

				public int dataIndex
				{
					get
					{
						return (int)(this.flags & 65535U);
					}
				}

				public int uniqueID
				{
					get
					{
						return (int)(this.flags >> 17);
					}
				}

				public bool isBuiltInCommandBuilder
				{
					get
					{
						return (this.flags & 65536U) > 0U;
					}
				}

				public static bool operator ==(DrawingData.BuilderData.BitPackedMeta lhs, DrawingData.BuilderData.BitPackedMeta rhs)
				{
					return lhs.flags == rhs.flags;
				}

				public static bool operator !=(DrawingData.BuilderData.BitPackedMeta lhs, DrawingData.BuilderData.BitPackedMeta rhs)
				{
					return lhs.flags != rhs.flags;
				}

				public override bool Equals(object obj)
				{
					if (obj is DrawingData.BuilderData.BitPackedMeta)
					{
						DrawingData.BuilderData.BitPackedMeta bitPackedMeta = (DrawingData.BuilderData.BitPackedMeta)obj;
						return this.flags == bitPackedMeta.flags;
					}
					return false;
				}

				public override int GetHashCode()
				{
					return (int)this.flags;
				}

				private uint flags;

				private const int UniqueIDBitshift = 17;

				private const int IsBuiltInFlagIndex = 16;

				private const int IndexMask = 65535;

				private const int MaxDataIndex = 65535;

				public const int UniqueIdMask = 32767;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private unsafe delegate bool AnyBuffersWrittenToDelegate(UnsafeAppendBuffer* buffers, int numBuffers);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private unsafe delegate void ResetAllBuffersToDelegate(UnsafeAppendBuffer* buffers, int numBuffers);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal unsafe delegate bool AnyBuffersWrittenTo_000002FB$PostfixBurstDelegate(UnsafeAppendBuffer* buffers, int numBuffers);

			internal static class AnyBuffersWrittenTo_000002FB$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$BurstDirectCall.Pointer == 0)
					{
						DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$PostfixBurstDelegate>(new DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$PostfixBurstDelegate(DrawingData.BuilderData.AnyBuffersWrittenTo)).Value;
					}
					A_0 = DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr result = (IntPtr)0;
					DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$BurstDirectCall.GetFunctionPointerDiscard(ref result);
					return result;
				}

				public unsafe static bool Invoke(UnsafeAppendBuffer* buffers, int numBuffers)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = DrawingData.BuilderData.AnyBuffersWrittenTo_000002FB$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Boolean(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer*,System.Int32), buffers, numBuffers, functionPointer);
						}
					}
					return DrawingData.BuilderData.AnyBuffersWrittenTo$BurstManaged(buffers, numBuffers);
				}

				private static IntPtr Pointer;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal unsafe delegate void ResetAllBuffers_000002FC$PostfixBurstDelegate(UnsafeAppendBuffer* buffers, int numBuffers);

			internal static class ResetAllBuffers_000002FC$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (DrawingData.BuilderData.ResetAllBuffers_000002FC$BurstDirectCall.Pointer == 0)
					{
						DrawingData.BuilderData.ResetAllBuffers_000002FC$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<DrawingData.BuilderData.ResetAllBuffers_000002FC$PostfixBurstDelegate>(new DrawingData.BuilderData.ResetAllBuffers_000002FC$PostfixBurstDelegate(DrawingData.BuilderData.ResetAllBuffers)).Value;
					}
					A_0 = DrawingData.BuilderData.ResetAllBuffers_000002FC$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr result = (IntPtr)0;
					DrawingData.BuilderData.ResetAllBuffers_000002FC$BurstDirectCall.GetFunctionPointerDiscard(ref result);
					return result;
				}

				public unsafe static void Invoke(UnsafeAppendBuffer* buffers, int numBuffers)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = DrawingData.BuilderData.ResetAllBuffers_000002FC$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							calli(System.Void(Unity.Collections.LowLevel.Unsafe.UnsafeAppendBuffer*,System.Int32), buffers, numBuffers, functionPointer);
							return;
						}
					}
					DrawingData.BuilderData.ResetAllBuffers$BurstManaged(buffers, numBuffers);
				}

				private static IntPtr Pointer;
			}
		}

		internal struct BuilderDataContainer : IDisposable
		{
			public int memoryUsage
			{
				get
				{
					int num = 0;
					if (this.data != null)
					{
						for (int i = 0; i < this.data.Length; i++)
						{
							NativeArray<UnsafeAppendBuffer> commandBuffers = this.data[i].commandBuffers;
							for (int j = 0; j < commandBuffers.Length; j++)
							{
								num += commandBuffers[j].Capacity;
							}
							num += this.data[i].commandBuffers.Length * sizeof(UnsafeAppendBuffer);
						}
					}
					return num;
				}
			}

			public DrawingData.BuilderData.BitPackedMeta Reserve(bool isBuiltInCommandBuilder)
			{
				if (this.data == null)
				{
					this.data = new DrawingData.BuilderData[1];
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].state == DrawingData.BuilderData.State.Free)
					{
						this.data[i].Reserve(i, isBuiltInCommandBuilder);
						return this.data[i].packedMeta;
					}
				}
				DrawingData.BuilderData[] array = new DrawingData.BuilderData[this.data.Length * 2];
				this.data.CopyTo(array, 0);
				this.data = array;
				return this.Reserve(isBuiltInCommandBuilder);
			}

			public void Release(DrawingData.BuilderData.BitPackedMeta meta)
			{
				this.data[meta.dataIndex].Release();
			}

			public bool StillExists(DrawingData.BuilderData.BitPackedMeta meta)
			{
				int dataIndex = meta.dataIndex;
				return this.data != null && dataIndex < this.data.Length && this.data[dataIndex].packedMeta == meta;
			}

			public ref DrawingData.BuilderData Get(DrawingData.BuilderData.BitPackedMeta meta)
			{
				int dataIndex = meta.dataIndex;
				if (this.data[dataIndex].state == DrawingData.BuilderData.State.Free)
				{
					throw new ArgumentException("Data is not reserved");
				}
				if (this.data[dataIndex].packedMeta != meta)
				{
					throw new ArgumentException("This command builder has already been disposed");
				}
				return ref this.data[dataIndex];
			}

			public void DisposeCommandBuildersWithJobDependencies(DrawingData gizmos)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					this.data[i].CheckJobDependency(gizmos, false);
				}
				for (int j = 0; j < this.data.Length; j++)
				{
					this.data[j].CheckJobDependency(gizmos, true);
				}
			}

			public void ReleaseAllUnused()
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].state == DrawingData.BuilderData.State.WaitingForSplitter)
					{
						this.data[i].Release();
					}
				}
			}

			public void Dispose()
			{
				if (this.data != null)
				{
					for (int i = 0; i < this.data.Length; i++)
					{
						this.data[i].Dispose();
					}
				}
				this.data = null;
			}

			private DrawingData.BuilderData[] data;
		}

		internal struct ProcessedBuilderDataContainer
		{
			public int memoryUsage
			{
				get
				{
					int num = 0;
					if (this.data != null)
					{
						for (int i = 0; i < this.data.Length; i++)
						{
							NativeArray<DrawingData.ProcessedBuilderData.MeshBuffers> temporaryMeshBuffers = this.data[i].temporaryMeshBuffers;
							for (int j = 0; j < temporaryMeshBuffers.Length; j++)
							{
								int num2 = 0;
								num2 += temporaryMeshBuffers[j].textVertices.Capacity;
								num2 += temporaryMeshBuffers[j].textTriangles.Capacity;
								num2 += temporaryMeshBuffers[j].solidVertices.Capacity;
								num2 += temporaryMeshBuffers[j].solidTriangles.Capacity;
								num2 += temporaryMeshBuffers[j].vertices.Capacity;
								num2 += temporaryMeshBuffers[j].triangles.Capacity;
								num2 += temporaryMeshBuffers[j].capturedState.Capacity;
								num2 += temporaryMeshBuffers[j].splitterOutput.Capacity;
								num += num2;
								Debug.Log(string.Concat(new string[]
								{
									i.ToString(),
									":",
									j.ToString(),
									" ",
									num2.ToString()
								}));
							}
						}
					}
					return num;
				}
			}

			public int Reserve(DrawingData.ProcessedBuilderData.Type type, DrawingData.BuilderData.Meta meta)
			{
				if (this.data == null)
				{
					this.data = new DrawingData.ProcessedBuilderData[0];
					this.freeSlots = new Stack<int>();
					this.freeLists = new Stack<List<int>>();
					this.hash2index = new Dictionary<ulong, List<int>>();
				}
				if (this.freeSlots.Count == 0)
				{
					DrawingData.ProcessedBuilderData[] array = new DrawingData.ProcessedBuilderData[math.max(4, this.data.Length * 2)];
					this.data.CopyTo(array, 0);
					for (int i = this.data.Length; i < array.Length; i++)
					{
						this.freeSlots.Push(i);
					}
					this.data = array;
				}
				int num = this.freeSlots.Pop();
				this.data[num].Init(type, meta);
				if (!meta.hasher.Equals(DrawingData.Hasher.NotSupplied))
				{
					List<int> list;
					if (!this.hash2index.TryGetValue(meta.hasher.Hash, out list))
					{
						if (this.freeLists.Count == 0)
						{
							this.freeLists.Push(new List<int>());
						}
						list = (this.hash2index[meta.hasher.Hash] = this.freeLists.Pop());
					}
					list.Add(num);
				}
				return num;
			}

			public ref DrawingData.ProcessedBuilderData Get(int index)
			{
				if (!this.data[index].isValid)
				{
					throw new ArgumentException();
				}
				return ref this.data[index];
			}

			private void Release(DrawingData gizmos, int i)
			{
				ulong hash = this.data[i].meta.hasher.Hash;
				List<int> list;
				if (!this.data[i].meta.hasher.Equals(DrawingData.Hasher.NotSupplied) && this.hash2index.TryGetValue(hash, out list))
				{
					list.Remove(i);
					if (list.Count == 0)
					{
						this.freeLists.Push(list);
						this.hash2index.Remove(hash);
					}
				}
				this.data[i].Release(gizmos);
				this.freeSlots.Push(i);
			}

			public void SubmitMeshes(DrawingData gizmos, Camera camera, int versionThreshold, bool allowGizmos, bool allowCameraDefault)
			{
				if (this.data == null)
				{
					return;
				}
				GeometryBuilder.CameraInfo cameraInfo = new GeometryBuilder.CameraInfo(camera);
				int num = 0;
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid && this.data[i].meta.version >= versionThreshold && this.data[i].IsValidForCamera(camera, allowGizmos, allowCameraDefault))
					{
						num++;
						this.data[i].Schedule(gizmos, ref cameraInfo);
					}
				}
				JobHandle.ScheduleBatchedJobs();
				for (int j = 0; j < this.data.Length; j++)
				{
					if (this.data[j].isValid && this.data[j].meta.version >= versionThreshold && this.data[j].IsValidForCamera(camera, allowGizmos, allowCameraDefault))
					{
						this.data[j].BuildMeshes(gizmos);
					}
				}
			}

			public void PoolDynamicMeshes(DrawingData gizmos)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid)
					{
						this.data[i].PoolDynamicMeshes(gizmos);
					}
				}
			}

			public void CollectMeshes(int versionThreshold, List<DrawingData.RenderedMeshWithType> meshes, Camera camera, bool allowGizmos, bool allowCameraDefault)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid && this.data[i].meta.version >= versionThreshold && this.data[i].IsValidForCamera(camera, allowGizmos, allowCameraDefault))
					{
						this.data[i].CollectMeshes(meshes);
					}
				}
			}

			public void FilterOldPersistentCommands(int version, int lastTickVersion, float time, int sceneModeVersion)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid && this.data[i].type == DrawingData.ProcessedBuilderData.Type.Persistent)
					{
						this.data[i].SchedulePersistFilter(version, lastTickVersion, time, sceneModeVersion);
					}
				}
			}

			public bool SetVersion(DrawingData.Hasher hasher, int version)
			{
				if (this.data == null)
				{
					return false;
				}
				List<int> list;
				if (this.hash2index.TryGetValue(hasher.Hash, out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						int num = list[i];
						this.data[num].meta.version = version;
					}
					return true;
				}
				return false;
			}

			public bool SetVersion(RedrawScope scope, int version)
			{
				if (this.data == null)
				{
					return false;
				}
				bool result = false;
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid && (this.data[i].meta.redrawScope1.id == scope.id || this.data[i].meta.redrawScope2.id == scope.id))
					{
						this.data[i].meta.version = version;
						result = true;
					}
				}
				return result;
			}

			public bool SetCustomScope(DrawingData.Hasher hasher, RedrawScope scope)
			{
				if (this.data == null)
				{
					return false;
				}
				List<int> list;
				if (this.hash2index.TryGetValue(hasher.Hash, out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						int num = list[i];
						this.data[num].meta.redrawScope2 = scope;
					}
					return true;
				}
				return false;
			}

			public void ReleaseDataOlderThan(DrawingData gizmos, int version)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid && this.data[i].meta.version < version)
					{
						this.Release(gizmos, i);
					}
				}
			}

			public void ReleaseAllWithHash(DrawingData gizmos, DrawingData.Hasher hasher)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid && this.data[i].meta.hasher.Hash == hasher.Hash)
					{
						this.Release(gizmos, i);
					}
				}
			}

			public void Dispose(DrawingData gizmos)
			{
				if (this.data == null)
				{
					return;
				}
				for (int i = 0; i < this.data.Length; i++)
				{
					if (this.data[i].isValid)
					{
						this.Release(gizmos, i);
					}
					this.data[i].Dispose();
				}
				this.data = null;
			}

			private DrawingData.ProcessedBuilderData[] data;

			private Dictionary<ulong, List<int>> hash2index;

			private Stack<int> freeSlots;

			private Stack<List<int>> freeLists;
		}

		[Flags]
		internal enum MeshType
		{
			Solid = 1,
			Lines = 2,
			Text = 4,
			Custom = 8,
			Pool = 16,
			BaseType = 7
		}

		internal struct MeshWithType
		{
			public Mesh mesh;

			public DrawingData.MeshType type;
		}

		internal struct RenderedMeshWithType
		{
			public Mesh mesh;

			public DrawingData.MeshType type;

			public int drawingOrderIndex;

			public Color color;

			public Matrix4x4 matrix;
		}

		private struct Range
		{
			public int start;

			public int end;
		}

		private class MeshCompareByDrawingOrder : IComparer<DrawingData.RenderedMeshWithType>
		{
			public int Compare(DrawingData.RenderedMeshWithType a, DrawingData.RenderedMeshWithType b)
			{
				int num = (int)(a.type & DrawingData.MeshType.BaseType);
				int num2 = (int)(b.type & DrawingData.MeshType.BaseType);
				if (num == num2)
				{
					return a.drawingOrderIndex - b.drawingOrderIndex;
				}
				return num - num2;
			}
		}

		public struct CommandBufferWrapper
		{
			public void SetWireframe(bool enable)
			{
				if (this.cmd != null)
				{
					this.cmd.SetWireframe(enable);
					return;
				}
				if (this.cmd2 != null && this.allowDisablingWireframe)
				{
					this.cmd2.SetWireframe(enable);
				}
			}

			public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
			{
				if (this.cmd != null)
				{
					this.cmd.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
					return;
				}
				if (this.cmd2 != null)
				{
					this.cmd2.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
				}
			}

			public CommandBuffer cmd;

			public bool allowDisablingWireframe;

			public RasterCommandBuffer cmd2;
		}
	}
}
