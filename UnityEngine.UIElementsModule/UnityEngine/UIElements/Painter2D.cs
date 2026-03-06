using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	public class Painter2D : IDisposable
	{
		internal bool isDetached
		{
			get
			{
				return this.m_DetachedAllocator != null;
			}
		}

		internal Painter2D(MeshGenerationContext ctx)
		{
			this.m_Handle = new SafeHandleAccess(UIPainter2D.Create(false));
			this.m_Ctx = ctx;
			this.m_JobSnapshots = new List<Painter2D.Painter2DJobData>(32);
			this.m_OnMeshGenerationDelegate = new MeshGenerationCallback(this.OnMeshGeneration);
			this.Reset();
		}

		public Painter2D()
		{
			this.m_Handle = new SafeHandleAccess(UIPainter2D.Create(true));
			this.m_DetachedAllocator = new DetachedAllocator();
			Painter2D.isPainterActive = true;
			this.m_OnMeshGenerationDelegate = new MeshGenerationCallback(this.OnMeshGeneration);
			this.Reset();
		}

		internal void Reset()
		{
			UIPainter2D.Reset(this.m_Handle);
		}

		internal MeshWriteData Allocate(int vertexCount, int indexCount)
		{
			bool isDetached = this.isDetached;
			MeshWriteData result;
			if (isDetached)
			{
				result = this.m_DetachedAllocator.Alloc(vertexCount, indexCount);
			}
			else
			{
				result = this.m_Ctx.Allocate(vertexCount, indexCount, null);
			}
			return result;
		}

		public void Clear()
		{
			bool flag = !this.isDetached;
			if (flag)
			{
				Debug.LogError("Clear() cannot be called on a Painter2D associated with a MeshGenerationContext. You should create your own instance of Painter2D instead.");
			}
			else
			{
				this.m_DetachedAllocator.Clear();
				this.Reset();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool disposed = this.m_Disposed;
			if (!disposed)
			{
				if (disposing)
				{
					bool flag = !this.m_Handle.IsNull();
					if (flag)
					{
						UIPainter2D.Destroy(this.m_Handle);
						this.m_Handle = new SafeHandleAccess(IntPtr.Zero);
					}
					bool flag2 = this.m_DetachedAllocator != null;
					if (flag2)
					{
						this.m_DetachedAllocator.Dispose();
					}
					this.m_JobParameters.Dispose();
				}
				this.m_Disposed = true;
			}
		}

		public float lineWidth
		{
			get
			{
				return UIPainter2D.GetLineWidth(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetLineWidth(this.m_Handle, value);
			}
		}

		public Color strokeColor
		{
			get
			{
				return UIPainter2D.GetStrokeColor(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetStrokeColor(this.m_Handle, value);
			}
		}

		public Gradient strokeGradient
		{
			get
			{
				return UIPainter2D.GetStrokeGradient(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetStrokeGradient(this.m_Handle, value);
			}
		}

		public Color fillColor
		{
			get
			{
				return UIPainter2D.GetFillColor(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetFillColor(this.m_Handle, value);
			}
		}

		public LineJoin lineJoin
		{
			get
			{
				return UIPainter2D.GetLineJoin(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetLineJoin(this.m_Handle, value);
			}
		}

		public LineCap lineCap
		{
			get
			{
				return UIPainter2D.GetLineCap(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetLineCap(this.m_Handle, value);
			}
		}

		public float miterLimit
		{
			get
			{
				return UIPainter2D.GetMiterLimit(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetMiterLimit(this.m_Handle, value);
			}
		}

		internal static bool isPainterActive { get; set; }

		private bool ValidateState()
		{
			bool flag = this.isDetached || Painter2D.isPainterActive;
			bool flag2 = !flag;
			if (flag2)
			{
				Debug.LogError("Cannot issue vector graphics commands outside of generateVisualContent callback");
			}
			return flag;
		}

		public void BeginPath()
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.BeginPath(this.m_Handle);
			}
		}

		public void ClosePath()
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.ClosePath(this.m_Handle);
			}
		}

		public void MoveTo(Vector2 pos)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.MoveTo(this.m_Handle, pos);
			}
		}

		public void LineTo(Vector2 pos)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.LineTo(this.m_Handle, pos);
			}
		}

		public void ArcTo(Vector2 p1, Vector2 p2, float radius)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.ArcTo(this.m_Handle, p1, p2, radius);
			}
		}

		public void Arc(Vector2 center, float radius, Angle startAngle, Angle endAngle, ArcDirection direction = ArcDirection.Clockwise)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.Arc(this.m_Handle, center, radius, startAngle.ToRadians(), endAngle.ToRadians(), direction);
			}
		}

		public void BezierCurveTo(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.BezierCurveTo(this.m_Handle, p1, p2, p3);
			}
		}

		public void QuadraticCurveTo(Vector2 p1, Vector2 p2)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.QuadraticCurveTo(this.m_Handle, p1, p2);
			}
		}

		public unsafe void Stroke()
		{
			using (Painter2D.s_StrokeMarker.Auto())
			{
				bool flag = !this.ValidateState();
				if (!flag)
				{
					bool isDetached = this.isDetached;
					if (isDetached)
					{
						MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.Stroke(this.m_Handle);
						bool flag2 = meshWriteDataInterface.vertexCount == 0;
						if (!flag2)
						{
							MeshWriteData meshWriteData = this.Allocate(meshWriteDataInterface.vertexCount, meshWriteDataInterface.indexCount);
							NativeSlice<Vertex> allVertices = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
							NativeSlice<ushort> allIndices = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
							meshWriteData.SetAllVertices(allVertices);
							meshWriteData.SetAllIndices(allIndices);
						}
					}
					else
					{
						UnsafeMeshGenerationNode node;
						this.m_Ctx.InsertUnsafeMeshGenerationNode(out node);
						int snapshotIndex = UIPainter2D.TakeStrokeSnapshot(this.m_Handle);
						this.m_JobSnapshots.Add(new Painter2D.Painter2DJobData
						{
							node = node,
							snapshotIndex = snapshotIndex
						});
					}
				}
			}
		}

		public unsafe void Fill(FillRule fillRule = FillRule.NonZero)
		{
			using (Painter2D.s_FillMarker.Auto())
			{
				bool flag = !this.ValidateState();
				if (!flag)
				{
					bool isDetached = this.isDetached;
					if (isDetached)
					{
						MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.Fill(this.m_Handle, fillRule);
						bool flag2 = meshWriteDataInterface.vertexCount == 0;
						if (!flag2)
						{
							MeshWriteData meshWriteData = this.Allocate(meshWriteDataInterface.vertexCount, meshWriteDataInterface.indexCount);
							NativeSlice<Vertex> allVertices = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
							NativeSlice<ushort> allIndices = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
							meshWriteData.SetAllVertices(allVertices);
							meshWriteData.SetAllIndices(allIndices);
						}
					}
					else
					{
						UnsafeMeshGenerationNode node;
						this.m_Ctx.InsertUnsafeMeshGenerationNode(out node);
						int snapshotIndex = UIPainter2D.TakeFillSnapshot(this.m_Handle, fillRule);
						this.m_JobSnapshots.Add(new Painter2D.Painter2DJobData
						{
							node = node,
							snapshotIndex = snapshotIndex
						});
					}
				}
			}
		}

		internal void ScheduleJobs(MeshGenerationContext mgc)
		{
			int count = this.m_JobSnapshots.Count;
			bool flag = count == 0;
			if (!flag)
			{
				bool flag2 = this.m_JobParameters.Length < count;
				if (flag2)
				{
					this.m_JobParameters.Dispose();
					this.m_JobParameters = new NativeArray<Painter2D.Painter2DJobData>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				}
				for (int i = 0; i < count; i++)
				{
					this.m_JobParameters[i] = this.m_JobSnapshots[i];
				}
				this.m_JobSnapshots.Clear();
				Painter2D.Painter2DJob jobData = new Painter2D.Painter2DJob
				{
					painterHandle = this.m_Handle,
					jobParameters = this.m_JobParameters.Slice(0, count)
				};
				mgc.GetTempMeshAllocator(out jobData.allocator);
				JobHandle jobHandle = jobData.ScheduleOrRunJob(count, 1, default(JobHandle));
				mgc.AddMeshGenerationJob(jobHandle);
				mgc.AddMeshGenerationCallback(this.m_OnMeshGenerationDelegate, null, MeshGenerationCallbackType.Work, true);
			}
		}

		private void OnMeshGeneration(MeshGenerationContext ctx, object data)
		{
			UIPainter2D.ClearSnapshots(this.m_Handle);
		}

		public bool SaveToVectorImage(VectorImage vectorImage)
		{
			bool flag = !this.isDetached;
			bool result;
			if (flag)
			{
				Debug.LogError("SaveToVectorImage cannot be called on a Painter2D associated with a MeshGenerationContext. You should create your own instance of Painter2D instead.");
				result = false;
			}
			else
			{
				bool flag2 = vectorImage == null;
				if (flag2)
				{
					throw new NullReferenceException("The provided vectorImage is null");
				}
				List<MeshWriteData> meshes = this.m_DetachedAllocator.meshes;
				int num = 0;
				int num2 = 0;
				foreach (MeshWriteData meshWriteData in meshes)
				{
					num += meshWriteData.m_Vertices.Length;
					num2 += meshWriteData.m_Indices.Length;
				}
				Rect bbox = UIPainter2D.GetBBox(this.m_Handle);
				VectorImageVertex[] array = new VectorImageVertex[num];
				ushort[] array2 = new ushort[num2];
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				foreach (MeshWriteData meshWriteData2 in meshes)
				{
					NativeSlice<Vertex> vertices = meshWriteData2.m_Vertices;
					for (int i = 0; i < vertices.Length; i++)
					{
						Vertex vertex = vertices[i];
						Vector3 position = vertex.position;
						position.x -= bbox.x;
						position.y -= bbox.y;
						array[num3++] = new VectorImageVertex
						{
							position = new Vector3(position.x, position.y, Vertex.nearZ),
							tint = vertex.tint,
							uv = vertex.uv,
							flags = vertex.flags,
							circle = vertex.circle
						};
					}
					NativeSlice<ushort> indices = meshWriteData2.m_Indices;
					for (int j = 0; j < indices.Length; j++)
					{
						array2[num4++] = (ushort)((int)indices[j] + num5);
					}
					num5 += vertices.Length;
				}
				vectorImage.version = 0;
				vectorImage.vertices = array;
				vectorImage.indices = array2;
				vectorImage.size = bbox.size;
				result = true;
			}
			return result;
		}

		private MeshGenerationContext m_Ctx;

		internal DetachedAllocator m_DetachedAllocator;

		internal SafeHandleAccess m_Handle;

		private List<Painter2D.Painter2DJobData> m_JobSnapshots = null;

		private NativeArray<Painter2D.Painter2DJobData> m_JobParameters;

		private bool m_Disposed;

		private static readonly ProfilerMarker s_StrokeMarker = new ProfilerMarker("Painter2D.Stroke");

		private static readonly ProfilerMarker s_FillMarker = new ProfilerMarker("Painter2D.Fill");

		private MeshGenerationCallback m_OnMeshGenerationDelegate;

		private struct Painter2DJobData
		{
			public UnsafeMeshGenerationNode node;

			public int snapshotIndex;
		}

		private struct Painter2DJob : IJobParallelFor
		{
			public unsafe void Execute(int i)
			{
				Painter2D.Painter2DJobData painter2DJobData = this.jobParameters[i];
				MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.ExecuteSnapshotFromJob(this.painterHandle, painter2DJobData.snapshotIndex);
				NativeSlice<Vertex> slice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
				NativeSlice<ushort> slice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
				bool flag = slice.Length == 0 || slice2.Length == 0;
				if (!flag)
				{
					NativeSlice<Vertex> vertices;
					NativeSlice<ushort> indices;
					this.allocator.AllocateTempMesh(slice.Length, slice2.Length, out vertices, out indices);
					Debug.Assert(vertices.Length == slice.Length);
					Debug.Assert(indices.Length == slice2.Length);
					vertices.CopyFrom(slice);
					indices.CopyFrom(slice2);
					painter2DJobData.node.DrawMesh(vertices, indices, null);
				}
			}

			[NativeDisableUnsafePtrRestriction]
			public IntPtr painterHandle;

			[ReadOnly]
			public TempMeshAllocator allocator;

			[ReadOnly]
			public NativeSlice<Painter2D.Painter2DJobData> jobParameters;
		}
	}
}
