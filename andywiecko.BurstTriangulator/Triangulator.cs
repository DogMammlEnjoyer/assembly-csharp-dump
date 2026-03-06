using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace andywiecko.BurstTriangulator
{
	public class Triangulator : IDisposable
	{
		public Triangulator.TriangulationSettings Settings { get; } = new Triangulator.TriangulationSettings();

		public Triangulator.InputData Input { get; set; } = new Triangulator.InputData();

		public Triangulator.OutputData Output { get; }

		public Triangulator(int capacity, Allocator allocator)
		{
			this.outputPositions = new NativeList<float2>(capacity, allocator);
			this.triangles = new NativeList<int>(6 * capacity, allocator);
			this.halfedges = new NativeList<int>(6 * capacity, allocator);
			this.circles = new NativeList<Triangulator.Circle>(capacity, allocator);
			this.constraintEdges = new NativeList<Triangulator.Edge>(capacity, allocator);
			this.status = new NativeReference<Triangulator.Status>(Triangulator.Status.OK, allocator);
			this.localPositions = new NativeList<float2>(capacity, allocator);
			this.localHoleSeeds = new NativeList<float2>(capacity, allocator);
			this.com = new NativeReference<float2>(allocator, NativeArrayOptions.ClearMemory);
			this.scale = new NativeReference<float2>(1, allocator);
			this.pcaCenter = new NativeReference<float2>(allocator, NativeArrayOptions.ClearMemory);
			this.pcaMatrix = new NativeReference<float2x2>(float2x2.identity, allocator);
			this.Output = new Triangulator.OutputData(this);
		}

		public Triangulator(Allocator allocator) : this(16384, allocator)
		{
		}

		public void Dispose()
		{
			this.outputPositions.Dispose();
			this.triangles.Dispose();
			this.halfedges.Dispose();
			this.circles.Dispose();
			this.constraintEdges.Dispose();
			this.status.Dispose();
			this.localPositions.Dispose();
			this.localHoleSeeds.Dispose();
			this.com.Dispose();
			this.scale.Dispose();
			this.pcaCenter.Dispose();
			this.pcaMatrix.Dispose();
		}

		public void Run()
		{
			this.Schedule(default(JobHandle)).Complete();
		}

		public JobHandle Schedule(JobHandle dependencies = default(JobHandle))
		{
			dependencies = new Triangulator.ClearDataJob(this).Schedule(dependencies);
			JobHandle jobHandle;
			switch (this.Settings.Preprocessor)
			{
			case Triangulator.Preprocessor.None:
				jobHandle = dependencies;
				break;
			case Triangulator.Preprocessor.COM:
				jobHandle = this.ScheduleWorldToLocalTransformation(dependencies);
				break;
			case Triangulator.Preprocessor.PCA:
				jobHandle = this.SchedulePCATransformation(dependencies);
				break;
			default:
				throw new NotImplementedException();
			}
			dependencies = jobHandle;
			if (this.Settings.ValidateInput)
			{
				dependencies = new Triangulator.ValidateInputPositionsJob(this).Schedule(dependencies);
				dependencies = (this.Settings.ConstrainEdges ? new Triangulator.ValidateInputConstraintEdges(this).Schedule(dependencies) : dependencies);
			}
			dependencies = new Triangulator.DelaunayTriangulationJob(this).Schedule(dependencies);
			dependencies = (this.Settings.ConstrainEdges ? new Triangulator.ConstrainEdgesJob(this).Schedule(dependencies) : dependencies);
			NativeArray<float2> holeSeeds = this.Input.HoleSeeds;
			if (this.Settings.RestoreBoundary && this.Settings.ConstrainEdges)
			{
				dependencies = (holeSeeds.IsCreated ? new Triangulator.PlantingSeedsJob<Triangulator.PlantBoundaryAndHoles>(this).Schedule(dependencies) : new Triangulator.PlantingSeedsJob<Triangulator.PlantBoundary>(this).Schedule(dependencies));
			}
			else if (holeSeeds.IsCreated && this.Settings.ConstrainEdges)
			{
				dependencies = new Triangulator.PlantingSeedsJob<Triangulator.PlantHoles>(this).Schedule(dependencies);
			}
			dependencies = (this.Settings.RefineMesh ? new Triangulator.RefineMeshJob(this, this.Settings.ConstrainEdges ? this.constraintEdges : default(NativeList<Triangulator.Edge>)).Schedule(dependencies) : dependencies);
			switch (this.Settings.Preprocessor)
			{
			case Triangulator.Preprocessor.None:
				jobHandle = dependencies;
				break;
			case Triangulator.Preprocessor.COM:
				jobHandle = this.ScheduleLocalToWorldTransformation(dependencies);
				break;
			case Triangulator.Preprocessor.PCA:
				jobHandle = this.SchedulePCAInverseTransformation(dependencies);
				break;
			default:
				throw new NotImplementedException();
			}
			dependencies = jobHandle;
			return dependencies;
		}

		private JobHandle SchedulePCATransformation(JobHandle dependencies)
		{
			this.tmpInputPositions = this.Input.Positions;
			this.Input.Positions = this.localPositions.AsDeferredJobArray();
			if (this.Input.HoleSeeds.IsCreated)
			{
				this.tmpInputHoleSeeds = this.Input.HoleSeeds;
				this.Input.HoleSeeds = this.localHoleSeeds.AsDeferredJobArray();
			}
			dependencies = new Triangulator.PCATransformationJob(this).Schedule(dependencies);
			if (this.tmpInputHoleSeeds.IsCreated)
			{
				dependencies = new Triangulator.PCATransformationHolesJob(this).Schedule(dependencies);
			}
			return dependencies;
		}

		private JobHandle SchedulePCAInverseTransformation(JobHandle dependencies)
		{
			dependencies = new Triangulator.PCAInverseTransformationJob(this).Schedule(this, dependencies);
			this.Input.Positions = this.tmpInputPositions;
			this.tmpInputPositions = default(NativeArray<float2>);
			if (this.tmpInputHoleSeeds.IsCreated)
			{
				this.Input.HoleSeeds = this.tmpInputHoleSeeds;
				this.tmpInputHoleSeeds = default(NativeArray<float2>);
			}
			return dependencies;
		}

		private JobHandle ScheduleWorldToLocalTransformation(JobHandle dependencies)
		{
			this.tmpInputPositions = this.Input.Positions;
			this.Input.Positions = this.localPositions.AsDeferredJobArray();
			if (this.Input.HoleSeeds.IsCreated)
			{
				this.tmpInputHoleSeeds = this.Input.HoleSeeds;
				this.Input.HoleSeeds = this.localHoleSeeds.AsDeferredJobArray();
			}
			dependencies = new Triangulator.InitialLocalTransformationJob(this).Schedule(dependencies);
			if (this.tmpInputHoleSeeds.IsCreated)
			{
				dependencies = new Triangulator.CalculateLocalHoleSeedsJob(this).Schedule(dependencies);
			}
			dependencies = new Triangulator.CalculateLocalPositionsJob(this).Schedule(this, dependencies);
			return dependencies;
		}

		private JobHandle ScheduleLocalToWorldTransformation(JobHandle dependencies)
		{
			dependencies = new Triangulator.LocalToWorldTransformationJob(this).Schedule(this, dependencies);
			this.Input.Positions = this.tmpInputPositions;
			this.tmpInputPositions = default(NativeArray<float2>);
			if (this.tmpInputHoleSeeds.IsCreated)
			{
				this.Input.HoleSeeds = this.tmpInputHoleSeeds;
				this.tmpInputHoleSeeds = default(NativeArray<float2>);
			}
			return dependencies;
		}

		private static int NextHalfedge(int he)
		{
			if (he % 3 != 2)
			{
				return he + 1;
			}
			return he - 2;
		}

		private static float Angle(float2 a, float2 b)
		{
			return math.atan2(Triangulator.Cross(a, b), math.dot(a, b));
		}

		private unsafe static float Area2(int i, int j, int k, ReadOnlySpan<float2> positions)
		{
			float2 @float = *positions[i];
			float2 float2 = *positions[j];
			float2 float3 = *positions[k];
			float2 rhs = @float;
			float2 lhs = float2;
			float2 lhs2 = float3;
			float2 a = lhs - rhs;
			float2 b = lhs2 - rhs;
			return math.abs(Triangulator.Cross(a, b));
		}

		private static float Cross(float2 a, float2 b)
		{
			return a.x * b.y - a.y * b.x;
		}

		private static Triangulator.Circle CalculateCircumCircle(int i, int j, int k, NativeArray<float2> positions)
		{
			float2 @float = positions[i];
			float2 float2 = positions[j];
			float2 float3 = positions[k];
			float2 a = @float;
			float2 b = float2;
			float2 c = float3;
			return new Triangulator.Circle(Triangulator.CircumCenter(a, b, c), Triangulator.CircumRadius(a, b, c));
		}

		private static float CircumRadius(float2 a, float2 b, float2 c)
		{
			return math.distance(Triangulator.CircumCenter(a, b, c), a);
		}

		private static float CircumRadiusSq(float2 a, float2 b, float2 c)
		{
			return math.distancesq(Triangulator.CircumCenter(a, b, c), a);
		}

		private static float2 CircumCenter(float2 a, float2 b, float2 c)
		{
			float num = b.x - a.x;
			float num2 = b.y - a.y;
			float num3 = c.x - a.x;
			float num4 = c.y - a.y;
			float num5 = num * num + num2 * num2;
			float num6 = num3 * num3 + num4 * num4;
			float num7 = 0.5f / (num * num4 - num2 * num3);
			float x = a.x + (num4 * num5 - num2 * num6) * num7;
			float y = a.y + (num * num6 - num3 * num5) * num7;
			return new float2(x, y);
		}

		private static float Orient2dFast(float2 a, float2 b, float2 c)
		{
			return (a.y - c.y) * (b.x - c.x) - (a.x - c.x) * (b.y - c.y);
		}

		private static bool InCircle(float2 a, float2 b, float2 c, float2 p)
		{
			float num = a.x - p.x;
			float num2 = a.y - p.y;
			float num3 = b.x - p.x;
			float num4 = b.y - p.y;
			float num5 = c.x - p.x;
			float num6 = c.y - p.y;
			float num7 = num * num + num2 * num2;
			float num8 = num3 * num3 + num4 * num4;
			float num9 = num5 * num5 + num6 * num6;
			return num * (num4 * num9 - num8 * num6) - num2 * (num3 * num9 - num8 * num5) + num7 * (num3 * num6 - num4 * num5) < 0f;
		}

		private static float3 Barycentric(float2 a, float2 b, float2 c, float2 p)
		{
			float2 @float = b - a;
			float2 float2 = c - a;
			float2 float3 = p - a;
			float2 a2 = @float;
			float2 b2 = float2;
			float2 float4 = float3;
			float num = 1f / Triangulator.Cross(a2, b2);
			float num2 = num * Triangulator.Cross(float4, b2);
			float num3 = num * Triangulator.Cross(a2, float4);
			return math.float3(1f - num2 - num3, num2, num3);
		}

		private static void Eigen(float2x2 matrix, out float2 eigval, out float2x2 eigvec)
		{
			float num = matrix[0][0];
			float num2 = matrix[1][1];
			float num3 = matrix[0][1];
			float num4 = num - num2;
			float num5 = num + num2;
			float num6 = (float)((num4 >= 0f) ? 1 : -1) * math.sqrt(num4 * num4 + 4f * num3 * num3);
			float x = num5 + num6;
			float y = num5 - num6;
			eigval = 0.5f * math.float2(x, y);
			float x2 = 0.5f * math.atan2(2f * num3, num4);
			eigvec = math.float2x2(math.cos(x2), -math.sin(x2), math.sin(x2), math.cos(x2));
		}

		private static float2x2 Kron(float2 a, float2 b)
		{
			return math.float2x2(a * b[0], a * b[1]);
		}

		private static bool PointInsideTriangle(float2 p, float2 a, float2 b, float2 c)
		{
			return math.cmax(-Triangulator.Barycentric(a, b, c, p)) <= 0f;
		}

		private static float CCW(float2 a, float2 b, float2 c)
		{
			return math.sign(Triangulator.Cross(b - a, b - c));
		}

		private static bool PointLineSegmentIntersection(float2 a, float2 b0, float2 b1)
		{
			return Triangulator.CCW(b0, b1, a) == 0f && math.all(a >= math.min(b0, b1) & a <= math.max(b0, b1));
		}

		private static bool EdgeEdgeIntersection(float2 a0, float2 a1, float2 b0, float2 b1)
		{
			return Triangulator.CCW(a0, a1, b0) != Triangulator.CCW(a0, a1, b1) && Triangulator.CCW(b0, b1, a0) != Triangulator.CCW(b0, b1, a1);
		}

		private static bool IsConvexQuadrilateral(float2 a, float2 b, float2 c, float2 d)
		{
			return Triangulator.CCW(a, c, b) != 0f && Triangulator.CCW(a, c, d) != 0f && Triangulator.CCW(b, d, a) != 0f && Triangulator.CCW(b, d, c) != 0f && Triangulator.CCW(a, c, b) != Triangulator.CCW(a, c, d) && Triangulator.CCW(b, d, a) != Triangulator.CCW(b, d, c);
		}

		private NativeList<float2> outputPositions;

		private NativeList<int> triangles;

		private NativeList<int> halfedges;

		private NativeList<Triangulator.Circle> circles;

		private NativeList<Triangulator.Edge> constraintEdges;

		private NativeReference<Triangulator.Status> status;

		private NativeArray<float2> tmpInputPositions;

		private NativeArray<float2> tmpInputHoleSeeds;

		private NativeList<float2> localPositions;

		private NativeList<float2> localHoleSeeds;

		private NativeReference<float2> com;

		private NativeReference<float2> scale;

		private NativeReference<float2> pcaCenter;

		private NativeReference<float2x2> pcaMatrix;

		private readonly struct Circle
		{
			public Circle(float2 center, float radius)
			{
				float radiusSq = radius * radius;
				this.Center = center;
				this.Radius = radius;
				this.RadiusSq = radiusSq;
			}

			public void Deconstruct(out float2 center, out float radius)
			{
				float2 center2 = this.Center;
				float radius2 = this.Radius;
				center = center2;
				radius = radius2;
			}

			public readonly float2 Center;

			public readonly float Radius;

			public readonly float RadiusSq;
		}

		private readonly struct Edge : IEquatable<Triangulator.Edge>, IComparable<Triangulator.Edge>
		{
			public Edge(int idA, int idB)
			{
				if (idA >= idB)
				{
					this.IdA = idB;
					this.IdB = idA;
					return;
				}
				this.IdA = idA;
				this.IdB = idB;
			}

			public static implicit operator Triangulator.Edge([TupleElementNames(new string[]
			{
				"a",
				"b"
			})] ValueTuple<int, int> ids)
			{
				return new Triangulator.Edge(ids.Item1, ids.Item2);
			}

			public void Deconstruct(out int idA, out int idB)
			{
				int idA2 = this.IdA;
				int idB2 = this.IdB;
				idA = idA2;
				idB = idB2;
			}

			public bool Equals(Triangulator.Edge other)
			{
				return this.IdA == other.IdA && this.IdB == other.IdB;
			}

			public bool Contains(int id)
			{
				return this.IdA == id || this.IdB == id;
			}

			public bool ContainsCommonPointWith(Triangulator.Edge other)
			{
				return this.Contains(other.IdA) || this.Contains(other.IdB);
			}

			public override int GetHashCode()
			{
				if (this.IdA >= this.IdB)
				{
					return this.IdA * this.IdA + this.IdA + this.IdB;
				}
				return this.IdB * this.IdB + this.IdA;
			}

			public int CompareTo(Triangulator.Edge other)
			{
				if (this.IdA == other.IdA)
				{
					return this.IdB.CompareTo(other.IdB);
				}
				return this.IdA.CompareTo(other.IdA);
			}

			public override string ToString()
			{
				return string.Format("({0}, {1})", this.IdA, this.IdB);
			}

			public readonly int IdA;

			public readonly int IdB;
		}

		public enum Status
		{
			OK,
			ERR
		}

		public enum Preprocessor
		{
			None,
			COM,
			PCA
		}

		[Serializable]
		public class RefinementThresholds
		{
			public float Area { get; set; } = 1f;

			public float Angle { get; set; } = math.radians(5f);
		}

		[Serializable]
		public class TriangulationSettings
		{
			public Triangulator.RefinementThresholds RefinementThresholds { get; } = new Triangulator.RefinementThresholds();

			public int BatchCount { get; set; } = 64;

			[Obsolete]
			public float MinimumAngle
			{
				get
				{
					return this.RefinementThresholds.Angle;
				}
				set
				{
					this.RefinementThresholds.Angle = value;
				}
			}

			[Obsolete]
			public float MinimumArea
			{
				get
				{
					return this.RefinementThresholds.Area;
				}
				set
				{
					this.RefinementThresholds.Area = value;
				}
			}

			[Obsolete]
			public float MaximumArea
			{
				get
				{
					return this.RefinementThresholds.Area;
				}
				set
				{
					this.RefinementThresholds.Area = value;
				}
			}

			public bool RefineMesh { get; set; }

			public bool ConstrainEdges { get; set; }

			public bool ValidateInput { get; set; } = true;

			public bool RestoreBoundary { get; set; }

			public int SloanMaxIters { get; set; } = 1000000;

			public float ConcentricShellsParameter { get; set; } = 0.001f;

			public Triangulator.Preprocessor Preprocessor { get; set; }
		}

		public class InputData
		{
			public NativeArray<float2> Positions { get; set; }

			public NativeArray<int> ConstraintEdges { get; set; }

			public NativeArray<float2> HoleSeeds { get; set; }
		}

		public class OutputData
		{
			public NativeList<float2> Positions
			{
				get
				{
					return this.owner.outputPositions;
				}
			}

			public NativeList<int> Triangles
			{
				get
				{
					return this.owner.triangles;
				}
			}

			public NativeReference<Triangulator.Status> Status
			{
				get
				{
					return this.owner.status;
				}
			}

			public OutputData(Triangulator triangulator)
			{
				this.owner = triangulator;
			}

			private readonly Triangulator owner;
		}

		[BurstCompile]
		private struct ValidateInputPositionsJob : IJob
		{
			public ValidateInputPositionsJob(Triangulator triangulator)
			{
				this.positions = triangulator.Input.Positions;
				this.status = triangulator.status;
			}

			public void Execute()
			{
				if (this.positions.Length < 3)
				{
					Debug.LogError("[Triangulator]: Positions.Length is less then 3!");
					this.status.Value = (this.status.Value | Triangulator.Status.ERR);
				}
				for (int i = 0; i < this.positions.Length; i++)
				{
					if (!this.PointValidation(i))
					{
						Debug.LogError(string.Format("[Triangulator]: Positions[{0}] does not contain finite value: {1}!", i, this.positions[i]));
						this.status.Value = (this.status.Value | Triangulator.Status.ERR);
					}
					if (!this.PointPointValidation(i))
					{
						this.status.Value = (this.status.Value | Triangulator.Status.ERR);
					}
				}
			}

			private bool PointValidation(int i)
			{
				return math.all(math.isfinite(this.positions[i]));
			}

			private bool PointPointValidation(int i)
			{
				float2 @float = this.positions[i];
				for (int j = i + 1; j < this.positions.Length; j++)
				{
					float2 rhs = this.positions[j];
					if (math.all(@float == rhs))
					{
						Debug.LogError(string.Format("[Triangulator]: Positions[{0}] and [{1}] are duplicated with value: {2}!", i, j, @float));
						return false;
					}
				}
				return true;
			}

			[ReadOnly]
			private NativeArray<float2> positions;

			private NativeReference<Triangulator.Status> status;
		}

		[BurstCompile]
		private struct PCATransformationJob : IJob
		{
			public PCATransformationJob(Triangulator triangulator)
			{
				this.positions = triangulator.tmpInputPositions;
				this.scaleRef = triangulator.scale;
				this.comRef = triangulator.com;
				this.cRef = triangulator.pcaCenter;
				this.URef = triangulator.pcaMatrix;
				this.localPositions = triangulator.localPositions;
			}

			public void Execute()
			{
				int length = this.positions.Length;
				float2 @float = 0;
				foreach (float2 rhs in this.positions)
				{
					@float += rhs;
				}
				@float /= (float)length;
				this.comRef.Value = @float;
				float2x2 float2x = float2x2.zero;
				foreach (float2 lhs in this.positions)
				{
					float2 float2 = lhs - @float;
					this.localPositions.Add(float2);
					float2x += Triangulator.Kron(float2, float2);
				}
				float2x /= (float)length;
				float2 float3;
				float2x2 float2x2;
				Triangulator.Eigen(float2x, out float3, out float2x2);
				this.URef.Value = float2x2;
				for (int i = 0; i < length; i++)
				{
					this.localPositions[i] = math.mul(math.transpose(float2x2), this.localPositions[i]);
				}
				float2 float4 = float.MaxValue;
				float2 float5 = float.MinValue;
				foreach (float2 x in this.localPositions)
				{
					float4 = math.min(x, float4);
					float5 = math.max(x, float5);
				}
				float3 = (this.cRef.Value = 0.5f * (float4 + float5));
				float2 rhs2 = float3;
				float3 = (this.scaleRef.Value = 2f / (float5 - float4));
				float2 rhs3 = float3;
				for (int j = 0; j < length; j++)
				{
					float2 lhs2 = this.localPositions[j];
					this.localPositions[j] = (lhs2 - rhs2) * rhs3;
				}
			}

			[ReadOnly]
			private NativeArray<float2> positions;

			private NativeReference<float2> scaleRef;

			private NativeReference<float2> comRef;

			private NativeReference<float2> cRef;

			private NativeReference<float2x2> URef;

			private NativeList<float2> localPositions;
		}

		[BurstCompile]
		private struct PCATransformationHolesJob : IJob
		{
			public PCATransformationHolesJob(Triangulator triangulator)
			{
				this.holeSeeds = triangulator.tmpInputHoleSeeds;
				this.localHoleSeeds = triangulator.localHoleSeeds;
				this.scaleRef = triangulator.scale.AsReadOnly();
				this.comRef = triangulator.com.AsReadOnly();
				this.cRef = triangulator.pcaCenter.AsReadOnly();
				this.URef = triangulator.pcaMatrix.AsReadOnly();
			}

			public void Execute()
			{
				float2 value = this.comRef.Value;
				float2 value2 = this.scaleRef.Value;
				float2 value3 = this.cRef.Value;
				float2x2 a = math.transpose(this.URef.Value);
				this.localHoleSeeds.Resize(this.holeSeeds.Length, NativeArrayOptions.UninitializedMemory);
				for (int i = 0; i < this.holeSeeds.Length; i++)
				{
					float2 lhs = this.holeSeeds[i];
					this.localHoleSeeds[i] = value2 * (math.mul(a, lhs - value) - value3);
				}
			}

			[ReadOnly]
			private NativeArray<float2> holeSeeds;

			private NativeList<float2> localHoleSeeds;

			private NativeReference<float2>.ReadOnly scaleRef;

			private NativeReference<float2>.ReadOnly comRef;

			private NativeReference<float2>.ReadOnly cRef;

			private NativeReference<float2x2>.ReadOnly URef;
		}

		[BurstCompile]
		private struct PCAInverseTransformationJob : IJobParallelForDefer
		{
			public PCAInverseTransformationJob(Triangulator triangulator)
			{
				this.positions = triangulator.Output.Positions.AsDeferredJobArray();
				this.comRef = triangulator.com.AsReadOnly();
				this.scaleRef = triangulator.scale.AsReadOnly();
				this.cRef = triangulator.pcaCenter.AsReadOnly();
				this.URef = triangulator.pcaMatrix.AsReadOnly();
			}

			public JobHandle Schedule(Triangulator triangulator, JobHandle dependencies)
			{
				return this.Schedule(triangulator.Output.Positions, triangulator.Settings.BatchCount, dependencies);
			}

			public void Execute(int i)
			{
				float2 lhs = this.positions[i];
				float2 value = this.comRef.Value;
				float2 value2 = this.scaleRef.Value;
				float2 value3 = this.cRef.Value;
				float2x2 value4 = this.URef.Value;
				this.positions[i] = math.mul(value4, lhs / value2 + value3) + value;
			}

			private NativeArray<float2> positions;

			private NativeReference<float2>.ReadOnly comRef;

			private NativeReference<float2>.ReadOnly scaleRef;

			private NativeReference<float2>.ReadOnly cRef;

			private NativeReference<float2x2>.ReadOnly URef;
		}

		[BurstCompile]
		private struct InitialLocalTransformationJob : IJob
		{
			public InitialLocalTransformationJob(Triangulator triangulator)
			{
				this.positions = triangulator.tmpInputPositions;
				this.comRef = triangulator.com;
				this.scaleRef = triangulator.scale;
				this.localPositions = triangulator.localPositions;
			}

			public void Execute()
			{
				float2 @float = 0;
				float2 float2 = 0;
				float2 float3 = 0;
				foreach (float2 float4 in this.positions)
				{
					@float = math.min(float4, @float);
					float2 = math.max(float4, float2);
					float3 += float4;
				}
				float3 /= (float)this.positions.Length;
				this.comRef.Value = float3;
				this.scaleRef.Value = 1f / math.cmax(math.max(math.abs(float2 - float3), math.abs(@float - float3)));
				this.localPositions.Resize(this.positions.Length, NativeArrayOptions.UninitializedMemory);
			}

			[ReadOnly]
			private NativeArray<float2> positions;

			private NativeReference<float2> comRef;

			private NativeReference<float2> scaleRef;

			private NativeList<float2> localPositions;
		}

		[BurstCompile]
		private struct CalculateLocalHoleSeedsJob : IJob
		{
			public CalculateLocalHoleSeedsJob(Triangulator triangulator)
			{
				this.holeSeeds = triangulator.tmpInputHoleSeeds;
				this.localHoleSeeds = triangulator.localHoleSeeds;
				this.comRef = triangulator.com.AsReadOnly();
				this.scaleRef = triangulator.scale.AsReadOnly();
			}

			public void Execute()
			{
				float2 value = this.comRef.Value;
				float2 value2 = this.scaleRef.Value;
				this.localHoleSeeds.Resize(this.holeSeeds.Length, NativeArrayOptions.UninitializedMemory);
				for (int i = 0; i < this.holeSeeds.Length; i++)
				{
					this.localHoleSeeds[i] = value2 * (this.holeSeeds[i] - value);
				}
			}

			[ReadOnly]
			private NativeArray<float2> holeSeeds;

			private NativeList<float2> localHoleSeeds;

			private NativeReference<float2>.ReadOnly comRef;

			private NativeReference<float2>.ReadOnly scaleRef;
		}

		[BurstCompile]
		private struct CalculateLocalPositionsJob : IJobParallelForDefer
		{
			public CalculateLocalPositionsJob(Triangulator triangulator)
			{
				this.comRef = triangulator.com.AsReadOnly();
				this.scaleRef = triangulator.scale.AsReadOnly();
				this.localPositions = triangulator.localPositions.AsDeferredJobArray();
				this.positions = triangulator.tmpInputPositions;
			}

			public JobHandle Schedule(Triangulator triangulator, JobHandle dependencies)
			{
				return this.Schedule(triangulator.localPositions, triangulator.Settings.BatchCount, dependencies);
			}

			public void Execute(int i)
			{
				float2 lhs = this.positions[i];
				float2 value = this.comRef.Value;
				float2 value2 = this.scaleRef.Value;
				this.localPositions[i] = value2 * (lhs - value);
			}

			private NativeReference<float2>.ReadOnly comRef;

			private NativeReference<float2>.ReadOnly scaleRef;

			private NativeArray<float2> localPositions;

			[ReadOnly]
			private NativeArray<float2> positions;
		}

		[BurstCompile]
		private struct LocalToWorldTransformationJob : IJobParallelForDefer
		{
			public LocalToWorldTransformationJob(Triangulator triangulator)
			{
				this.positions = triangulator.Output.Positions.AsDeferredJobArray();
				this.comRef = triangulator.com.AsReadOnly();
				this.scaleRef = triangulator.scale.AsReadOnly();
			}

			public JobHandle Schedule(Triangulator triangulator, JobHandle dependencies)
			{
				return this.Schedule(triangulator.Output.Positions, triangulator.Settings.BatchCount, dependencies);
			}

			public void Execute(int i)
			{
				float2 lhs = this.positions[i];
				float2 value = this.comRef.Value;
				float2 value2 = this.scaleRef.Value;
				this.positions[i] = lhs / value2 + value;
			}

			private NativeArray<float2> positions;

			private NativeReference<float2>.ReadOnly comRef;

			private NativeReference<float2>.ReadOnly scaleRef;
		}

		[BurstCompile]
		private struct ClearDataJob : IJob
		{
			public ClearDataJob(Triangulator triangulator)
			{
				this.outputPositions = triangulator.outputPositions;
				this.triangles = triangulator.triangles;
				this.halfedges = triangulator.halfedges;
				this.circles = triangulator.circles;
				this.constraintEdges = triangulator.constraintEdges;
				this.status = triangulator.status;
				this.scaleRef = triangulator.scale;
				this.comRef = triangulator.com;
				this.cRef = triangulator.pcaCenter;
				this.URef = triangulator.pcaMatrix;
			}

			public void Execute()
			{
				this.outputPositions.Clear();
				this.triangles.Clear();
				this.halfedges.Clear();
				this.circles.Clear();
				this.constraintEdges.Clear();
				this.status.Value = Triangulator.Status.OK;
				this.scaleRef.Value = 1;
				this.comRef.Value = 0;
				this.cRef.Value = 0;
				this.URef.Value = float2x2.identity;
			}

			private NativeReference<float2> scaleRef;

			private NativeReference<float2> comRef;

			private NativeReference<float2> cRef;

			private NativeReference<float2x2> URef;

			private NativeList<float2> outputPositions;

			private NativeList<int> triangles;

			private NativeList<int> halfedges;

			private NativeList<Triangulator.Circle> circles;

			private NativeList<Triangulator.Edge> constraintEdges;

			private NativeReference<Triangulator.Status> status;
		}

		[BurstCompile]
		private struct DelaunayTriangulationJob : IJob
		{
			public DelaunayTriangulationJob(Triangulator triangulator)
			{
				this.status = triangulator.status;
				this.inputPositions = triangulator.Input.Positions;
				this.outputPositions = triangulator.outputPositions;
				this.triangles = triangulator.triangles;
				this.halfedges = triangulator.halfedges;
				this.positions = default(NativeArray<float2>);
				this.ids = default(NativeArray<int>);
				this.dists = default(NativeArray<float>);
				this.hullNext = default(NativeArray<int>);
				this.hullPrev = default(NativeArray<int>);
				this.hullTri = default(NativeArray<int>);
				this.hullHash = default(NativeArray<int>);
				this.EDGE_STACK = default(NativeArray<int>);
				this.hullStart = int.MaxValue;
				this.trianglesLen = 0;
				this.hashSize = 0;
				this.c = float.MaxValue;
			}

			private readonly int HashKey(float2 p)
			{
				return (int)math.floor(Triangulator.DelaunayTriangulationJob.<HashKey>g__pseudoAngle|19_0(p.x - this.c.x, p.y - this.c.y) * (float)this.hashSize) % this.hashSize;
			}

			public void Execute()
			{
				if (this.status.Value == Triangulator.Status.ERR)
				{
					return;
				}
				this.outputPositions.CopyFrom(this.inputPositions);
				this.positions = this.outputPositions.AsArray();
				int length = this.positions.Length;
				int num = math.max(2 * length - 5, 0);
				this.triangles.Length = 3 * num;
				this.halfedges.Length = 3 * num;
				this.hashSize = (int)math.ceil(math.sqrt((float)length));
				using (this.hullPrev = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
				{
					using (this.hullNext = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
					{
						using (this.hullTri = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
						{
							using (this.hullHash = new NativeArray<int>(this.hashSize, Allocator.Temp, NativeArrayOptions.ClearMemory))
							{
								using (this.ids = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
								{
									using (this.dists = new NativeArray<float>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
									{
										using (this.EDGE_STACK = new NativeArray<int>(512, Allocator.Temp, NativeArrayOptions.ClearMemory))
										{
											float2 @float = float.MaxValue;
											float2 float2 = float.MinValue;
											for (int i = 0; i < this.positions.Length; i++)
											{
												float2 y = this.positions[i];
												@float = math.min(@float, y);
												float2 = math.max(float2, y);
												this.ids[i] = i;
											}
											float2 x = 0.5f * (@float + float2);
											int num2 = int.MaxValue;
											int num3 = int.MaxValue;
											int num4 = int.MaxValue;
											float num5 = float.MaxValue;
											for (int j = 0; j < this.positions.Length; j++)
											{
												float num6 = math.distancesq(x, this.positions[j]);
												if (num6 < num5)
												{
													num2 = j;
													num5 = num6;
												}
											}
											float2 float3 = this.positions[num2];
											num5 = float.MaxValue;
											for (int k = 0; k < this.positions.Length; k++)
											{
												if (k != num2)
												{
													float num7 = math.distancesq(float3, this.positions[k]);
													if (num7 < num5)
													{
														num3 = k;
														num5 = num7;
													}
												}
											}
											float2 float4 = this.positions[num3];
											float num8 = float.MaxValue;
											for (int l = 0; l < this.positions.Length; l++)
											{
												if (l != num2 && l != num3)
												{
													float2 float5 = this.positions[l];
													float num9 = Triangulator.CircumRadiusSq(float3, float4, float5);
													if (num9 < num8)
													{
														num4 = l;
														num8 = num9;
													}
												}
											}
											float2 float6 = this.positions[num4];
											if (num8 == 3.4028235E+38f)
											{
												Debug.LogError("[Triangulator]: Provided input is not supported!");
												this.status.Value = (this.status.Value | Triangulator.Status.ERR);
											}
											else
											{
												if (Triangulator.Orient2dFast(float3, float4, float6) < 0f)
												{
													int num10 = num4;
													int num11 = num3;
													num3 = num10;
													num4 = num11;
													float2 float7 = float6;
													float2 float8 = float4;
													float4 = float7;
													float6 = float8;
												}
												this.c = Triangulator.CircumCenter(float3, float4, float6);
												for (int m = 0; m < this.positions.Length; m++)
												{
													this.dists[m] = math.distancesq(this.c, this.positions[m]);
												}
												this.ids.Sort(new Triangulator.DelaunayTriangulationJob.DistComparer(this.dists));
												this.hullStart = num2;
												this.hullNext[num2] = (this.hullPrev[num4] = num3);
												this.hullNext[num3] = (this.hullPrev[num2] = num4);
												this.hullNext[num4] = (this.hullPrev[num3] = num2);
												this.hullTri[num2] = 0;
												this.hullTri[num3] = 1;
												this.hullTri[num4] = 2;
												this.hullHash[this.HashKey(float3)] = num2;
												this.hullHash[this.HashKey(float4)] = num3;
												this.hullHash[this.HashKey(float6)] = num4;
												this.AddTriangle(num2, num3, num4, -1, -1, -1);
												for (int n = 0; n < this.ids.Length; n++)
												{
													int num12 = this.ids[n];
													if (num12 != num2 && num12 != num3 && num12 != num4)
													{
														float2 float9 = this.positions[num12];
														int num13 = 0;
														for (int num14 = 0; num14 < this.hashSize; num14++)
														{
															int num15 = this.HashKey(float9);
															num13 = this.hullHash[(num15 + num14) % this.hashSize];
															if (num13 != -1 && num13 != this.hullNext[num13])
															{
																break;
															}
														}
														num13 = this.hullPrev[num13];
														int num16 = num13;
														int num17 = this.hullNext[num16];
														while (Triangulator.Orient2dFast(float9, this.positions[num16], this.positions[num17]) >= 0f)
														{
															num16 = num17;
															if (num16 == num13)
															{
																num16 = int.MaxValue;
																break;
															}
															num17 = this.hullNext[num16];
														}
														if (num16 != 2147483647)
														{
															int num18 = this.AddTriangle(num16, num12, this.hullNext[num16], -1, -1, this.hullTri[num16]);
															this.hullTri[num12] = this.Legalize(num18 + 2);
															this.hullTri[num16] = num18;
															int num19 = this.hullNext[num16];
															num17 = this.hullNext[num19];
															while (Triangulator.Orient2dFast(float9, this.positions[num19], this.positions[num17]) < 0f)
															{
																num18 = this.AddTriangle(num19, num12, num17, this.hullTri[num12], -1, this.hullTri[num19]);
																this.hullTri[num12] = this.Legalize(num18 + 2);
																this.hullNext[num19] = num19;
																num19 = num17;
																num17 = this.hullNext[num19];
															}
															if (num16 == num13)
															{
																num17 = this.hullPrev[num16];
																while (Triangulator.Orient2dFast(float9, this.positions[num17], this.positions[num16]) < 0f)
																{
																	num18 = this.AddTriangle(num17, num12, num16, -1, this.hullTri[num16], this.hullTri[num17]);
																	this.Legalize(num18 + 2);
																	this.hullTri[num17] = num18;
																	this.hullNext[num16] = num16;
																	num16 = num17;
																	num17 = this.hullPrev[num16];
																}
															}
															this.hullStart = (this.hullPrev[num12] = num16);
															this.hullNext[num16] = (this.hullPrev[num19] = num12);
															this.hullNext[num12] = num19;
															this.hullHash[this.HashKey(float9)] = num12;
															this.hullHash[this.HashKey(this.positions[num16])] = num16;
														}
													}
												}
												this.triangles.Length = this.trianglesLen;
												this.halfedges.Length = this.trianglesLen;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			private int Legalize(int a)
			{
				int num = 0;
				int num4;
				for (;;)
				{
					int num2 = this.halfedges[a];
					int num3 = a - a % 3;
					num4 = num3 + (a + 2) % 3;
					if (num2 == -1)
					{
						if (num == 0)
						{
							break;
						}
						a = this.EDGE_STACK[--num];
					}
					else
					{
						int num5 = num2 - num2 % 3;
						int index = num3 + (a + 1) % 3;
						int num6 = num5 + (num2 + 2) % 3;
						int num7 = this.triangles[num4];
						int index2 = this.triangles[a];
						int index3 = this.triangles[index];
						int num8 = this.triangles[num6];
						if (Triangulator.InCircle(this.positions[num7], this.positions[index2], this.positions[index3], this.positions[num8]))
						{
							this.triangles[a] = num8;
							this.triangles[num2] = num7;
							int num9 = this.halfedges[num6];
							if (num9 == -1)
							{
								int num10 = this.hullStart;
								while (this.hullTri[num10] != num6)
								{
									num10 = this.hullPrev[num10];
									if (num10 == this.hullStart)
									{
										goto IL_13F;
									}
								}
								this.hullTri[num10] = a;
							}
							IL_13F:
							this.Link(a, num9);
							this.Link(num2, this.halfedges[num4]);
							this.Link(num4, num6);
							int value = num5 + (num2 + 1) % 3;
							if (num < this.EDGE_STACK.Length)
							{
								this.EDGE_STACK[num++] = value;
							}
						}
						else
						{
							if (num == 0)
							{
								break;
							}
							a = this.EDGE_STACK[--num];
						}
					}
				}
				return num4;
			}

			private int AddTriangle(int i0, int i1, int i2, int a, int b, int c)
			{
				int num = this.trianglesLen;
				this.triangles[num] = i0;
				this.triangles[num + 1] = i1;
				this.triangles[num + 2] = i2;
				this.Link(num, a);
				this.Link(num + 1, b);
				this.Link(num + 2, c);
				this.trianglesLen += 3;
				return num;
			}

			private void Link(int a, int b)
			{
				this.halfedges[a] = b;
				if (b != -1)
				{
					this.halfedges[b] = a;
				}
			}

			[CompilerGenerated]
			internal static float <HashKey>g__pseudoAngle|19_0(float dx, float dy)
			{
				float num = dx / (math.abs(dx) + math.abs(dy));
				return ((dy > 0f) ? (3f - num) : (1f + num)) / 4f;
			}

			private NativeReference<Triangulator.Status> status;

			[ReadOnly]
			private NativeArray<float2> inputPositions;

			private NativeList<float2> outputPositions;

			private NativeList<int> triangles;

			private NativeList<int> halfedges;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<float2> positions;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> ids;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<float> dists;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> hullNext;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> hullPrev;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> hullTri;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> hullHash;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> EDGE_STACK;

			private int hullStart;

			private int trianglesLen;

			private int hashSize;

			private float2 c;

			private struct DistComparer : IComparer<int>
			{
				public DistComparer(NativeArray<float> dist)
				{
					this.dist = dist;
				}

				public int Compare(int x, int y)
				{
					return this.dist[x].CompareTo(this.dist[y]);
				}

				private NativeArray<float> dist;
			}
		}

		[BurstCompile]
		private struct ValidateInputConstraintEdges : IJob
		{
			public ValidateInputConstraintEdges(Triangulator triangulator)
			{
				this.constraints = triangulator.Input.ConstraintEdges;
				this.positions = triangulator.Input.Positions;
				this.status = triangulator.status;
			}

			public void Execute()
			{
				if (this.constraints.Length % 2 == 1)
				{
					Debug.LogError("[Triangulator]: Constraint input buffer does not contain even number of elements!");
					this.status.Value = (this.status.Value | Triangulator.Status.ERR);
					return;
				}
				for (int i = 0; i < this.constraints.Length / 2; i++)
				{
					if (!this.EdgePositionsRangeValidation(i) || !this.EdgeValidation(i) || !this.EdgePointValidation(i) || !this.EdgeEdgeValidation(i))
					{
						this.status.Value = (this.status.Value | Triangulator.Status.ERR);
						return;
					}
				}
			}

			private bool EdgePositionsRangeValidation(int i)
			{
				int num = this.constraints[2 * i];
				int num2 = this.constraints[2 * i + 1];
				int num3 = num;
				int num4 = num2;
				int length = this.positions.Length;
				if (num3 >= length || num3 < 0 || num4 >= length || num4 < 0)
				{
					Debug.LogError(string.Format("[Triangulator]: ConstraintEdges[{0}] = ({1}, {2}) is out of range Positions.Length = {3}!", new object[]
					{
						i,
						num3,
						num4,
						length
					}));
					return false;
				}
				return true;
			}

			private bool EdgeValidation(int i)
			{
				int num = this.constraints[2 * i];
				int num2 = this.constraints[2 * i + 1];
				int num3 = num;
				int num4 = num2;
				if (num3 == num4)
				{
					Debug.LogError(string.Format("[Triangulator]: ConstraintEdges[{0}] is length zero!", i));
					return false;
				}
				return true;
			}

			private bool EdgePointValidation(int i)
			{
				int num = this.constraints[2 * i];
				int num2 = this.constraints[2 * i + 1];
				int num3 = num;
				int num4 = num2;
				float2 @float = this.positions[num3];
				float2 float2 = this.positions[num4];
				float2 b = @float;
				float2 b2 = float2;
				for (int j = 0; j < this.positions.Length; j++)
				{
					if (j != num3 && j != num4 && Triangulator.PointLineSegmentIntersection(this.positions[j], b, b2))
					{
						Debug.LogError(string.Format("[Triangulator]: ConstraintEdges[{0}] and Positions[{1}] are collinear!", i, j));
						return false;
					}
				}
				return true;
			}

			private bool EdgeEdgeValidation(int i)
			{
				for (int j = i + 1; j < this.constraints.Length / 2; j++)
				{
					if (!this.ValidatePair(i, j))
					{
						return false;
					}
				}
				return true;
			}

			private bool ValidatePair(int i, int j)
			{
				int num = this.constraints[2 * i];
				int num2 = this.constraints[2 * i + 1];
				int num3 = num;
				int num4 = num2;
				num = this.constraints[2 * j];
				int num5 = this.constraints[2 * j + 1];
				int num6 = num;
				int num7 = num5;
				if ((num3 == num6 && num4 == num7) || (num3 == num7 && num4 == num6))
				{
					Debug.LogError(string.Format("[Triangulator]: ConstraintEdges[{0}] and [{1}] are equivalent!", i, j));
					return false;
				}
				if (num3 == num6 || num3 == num7 || num4 == num6 || num4 == num7)
				{
					return true;
				}
				float2 @float = this.positions[num3];
				float2 float2 = this.positions[num4];
				float2 float3 = this.positions[num6];
				float2 float4 = this.positions[num7];
				float2 a = @float;
				float2 a2 = float2;
				float2 b = float3;
				float2 b2 = float4;
				if (Triangulator.EdgeEdgeIntersection(a, a2, b, b2))
				{
					Debug.LogError(string.Format("[Triangulator]: ConstraintEdges[{0}] and [{1}] intersect!", i, j));
					return false;
				}
				return true;
			}

			[ReadOnly]
			private NativeArray<int> constraints;

			[ReadOnly]
			private NativeArray<float2> positions;

			private NativeReference<Triangulator.Status> status;
		}

		[BurstCompile]
		private struct ConstrainEdgesJob : IJob
		{
			public ConstrainEdgesJob(Triangulator triangulator)
			{
				this.status = triangulator.status;
				this.outputPositions = triangulator.outputPositions.AsDeferredJobArray();
				this.triangles = triangulator.triangles.AsDeferredJobArray();
				this.maxIters = triangulator.Settings.SloanMaxIters;
				this.inputConstraintEdges = triangulator.Input.ConstraintEdges;
				this.internalConstraints = triangulator.constraintEdges;
				this.halfedges = triangulator.halfedges.AsDeferredJobArray();
				this.intersections = default(NativeList<int>);
				this.unresolvedIntersections = default(NativeList<int>);
				this.pointToHalfedge = default(NativeArray<int>);
			}

			public void Execute()
			{
				if (this.status.Value != Triangulator.Status.OK)
				{
					return;
				}
				using (this.intersections = new NativeList<int>(Allocator.Temp))
				{
					using (this.unresolvedIntersections = new NativeList<int>(Allocator.Temp))
					{
						using (this.pointToHalfedge = new NativeArray<int>(this.outputPositions.Length, Allocator.Temp, NativeArrayOptions.ClearMemory))
						{
							for (int i = 0; i < this.triangles.Length; i++)
							{
								this.pointToHalfedge[this.triangles[i]] = i;
							}
							this.BuildInternalConstraints();
							foreach (Triangulator.Edge c in this.internalConstraints)
							{
								this.TryApplyConstraint(c);
							}
						}
					}
				}
			}

			private void BuildInternalConstraints()
			{
				this.internalConstraints.Length = this.inputConstraintEdges.Length / 2;
				for (int i = 0; i < this.internalConstraints.Length; i++)
				{
					this.internalConstraints[i] = new Triangulator.Edge(this.inputConstraintEdges[2 * i], this.inputConstraintEdges[2 * i + 1]);
				}
			}

			private void TryApplyConstraint(Triangulator.Edge c)
			{
				this.intersections.Clear();
				this.unresolvedIntersections.Clear();
				this.CollectIntersections(c);
				int num = 0;
				while ((this.status.Value & Triangulator.Status.ERR) != Triangulator.Status.ERR)
				{
					NativeList<int> nativeList = this.unresolvedIntersections;
					NativeList<int> nativeList2 = this.intersections;
					this.intersections = nativeList;
					this.unresolvedIntersections = nativeList2;
					this.TryResolveIntersections(c, ref num);
					if (this.unresolvedIntersections.IsEmpty)
					{
						return;
					}
				}
			}

			private void TryResolveIntersections(Triangulator.Edge c, ref int iter)
			{
				for (int i = 0; i < this.intersections.Length; i++)
				{
					int num = iter;
					iter = num + 1;
					if (this.IsMaxItersExceeded(num, this.maxIters))
					{
						return;
					}
					int num2 = this.intersections[i];
					int num3 = Triangulator.NextHalfedge(num2);
					int num4 = Triangulator.NextHalfedge(num3);
					int num5 = this.halfedges[num2];
					int num6 = Triangulator.NextHalfedge(Triangulator.NextHalfedge(num5));
					int index = this.triangles[num2];
					int index2 = this.triangles[num3];
					int num7 = this.triangles[num4];
					int num8 = this.triangles[num6];
					float2 @float = this.outputPositions[index];
					float2 float2 = this.outputPositions[num8];
					float2 float3 = this.outputPositions[index2];
					float2 float4 = this.outputPositions[num7];
					float2 a = @float;
					float2 b = float2;
					float2 c2 = float3;
					float2 d = float4;
					if (!Triangulator.IsConvexQuadrilateral(a, b, c2, d))
					{
						this.unresolvedIntersections.Add(num2);
					}
					else
					{
						this.triangles[num2] = num8;
						this.triangles[num5] = num7;
						this.pointToHalfedge[num8] = num2;
						this.pointToHalfedge[num7] = num5;
						int num9 = this.halfedges[num6];
						this.halfedges[num2] = num9;
						if (num9 != -1)
						{
							this.halfedges[num9] = num2;
						}
						int num10 = this.halfedges[num4];
						this.halfedges[num5] = num10;
						if (num10 != -1)
						{
							this.halfedges[num10] = num5;
						}
						this.halfedges[num4] = num6;
						this.halfedges[num6] = num4;
						for (int j = i + 1; j < this.intersections.Length; j++)
						{
							int num11 = this.intersections[j];
							this.intersections[j] = ((num11 == num4) ? num5 : ((num11 == num6) ? num2 : num11));
						}
						for (int k = 0; k < this.unresolvedIntersections.Length; k++)
						{
							int num12 = this.unresolvedIntersections[k];
							this.unresolvedIntersections[k] = ((num12 == num4) ? num5 : ((num12 == num6) ? num2 : num12));
						}
						Triangulator.Edge e = new Triangulator.Edge(num7, num8);
						if (this.EdgeEdgeIntersection(c, e))
						{
							this.unresolvedIntersections.Add(num4);
						}
					}
				}
				this.intersections.Clear();
			}

			private bool EdgeEdgeIntersection(Triangulator.Edge e1, Triangulator.Edge e2)
			{
				float2 @float = this.outputPositions[e1.IdA];
				float2 float2 = this.outputPositions[e1.IdB];
				float2 a = @float;
				float2 a2 = float2;
				@float = this.outputPositions[e2.IdA];
				float2 float3 = this.outputPositions[e2.IdB];
				float2 b = @float;
				float2 b2 = float3;
				return !e1.ContainsCommonPointWith(e2) && Triangulator.EdgeEdgeIntersection(a, a2, b, b2);
			}

			private void CollectIntersections(Triangulator.Edge edge)
			{
				int num = -1;
				Triangulator.Edge edge2 = edge;
				int num2;
				int num3;
				edge2.Deconstruct(out num2, out num3);
				int index = num2;
				int num4 = num3;
				int num5 = this.pointToHalfedge[index];
				int num6 = num5;
				int num7;
				for (;;)
				{
					num7 = Triangulator.NextHalfedge(num6);
					if (this.triangles[num7] == num4)
					{
						goto IL_A6;
					}
					int index2 = Triangulator.NextHalfedge(num7);
					if (this.EdgeEdgeIntersection(edge, new Triangulator.Edge(this.triangles[num7], this.triangles[index2])))
					{
						break;
					}
					num6 = this.halfedges[index2];
					if (num6 == -1 || num6 == num5)
					{
						goto IL_A6;
					}
				}
				this.unresolvedIntersections.Add(num7);
				num = this.halfedges[num7];
				IL_A6:
				num6 = this.halfedges[num5];
				if (num == -1 && num6 != -1)
				{
					num6 = Triangulator.NextHalfedge(num6);
					int num8;
					for (;;)
					{
						num8 = Triangulator.NextHalfedge(num6);
						if (this.triangles[num8] == num4)
						{
							goto IL_218;
						}
						int index3 = Triangulator.NextHalfedge(num8);
						if (this.EdgeEdgeIntersection(edge, new Triangulator.Edge(this.triangles[num8], this.triangles[index3])))
						{
							break;
						}
						num6 = this.halfedges[num6];
						if (num6 == -1)
						{
							goto IL_218;
						}
						num6 = Triangulator.NextHalfedge(num6);
						if (num6 == num5)
						{
							goto Block_9;
						}
					}
					this.unresolvedIntersections.Add(num8);
					num = this.halfedges[num8];
					Block_9:;
				}
				IL_218:
				while (num != -1)
				{
					int num9 = num;
					num = -1;
					int num10 = Triangulator.NextHalfedge(num9);
					int index4 = Triangulator.NextHalfedge(num10);
					if (this.triangles[index4] == num4)
					{
						break;
					}
					if (this.EdgeEdgeIntersection(edge, new Triangulator.Edge(this.triangles[num10], this.triangles[index4])))
					{
						this.unresolvedIntersections.Add(num10);
						num = this.halfedges[num10];
					}
					else if (this.EdgeEdgeIntersection(edge, new Triangulator.Edge(this.triangles[index4], this.triangles[num9])))
					{
						this.unresolvedIntersections.Add(index4);
						num = this.halfedges[index4];
					}
				}
			}

			private bool IsMaxItersExceeded(int iter, int maxIters)
			{
				if (iter >= maxIters)
				{
					Debug.LogError("[Triangulator]: Sloan max iterations exceeded! This may suggest that input data is hard to resolve by Sloan's algorithm. It usually happens when the scale of the input positions is not uniform. Please try to post-process input data or increase SloanMaxIters value.");
					this.status.Value = (this.status.Value | Triangulator.Status.ERR);
					return true;
				}
				return false;
			}

			private NativeReference<Triangulator.Status> status;

			[ReadOnly]
			private NativeArray<float2> outputPositions;

			private NativeArray<int> triangles;

			[ReadOnly]
			private NativeArray<int> inputConstraintEdges;

			private NativeList<Triangulator.Edge> internalConstraints;

			private NativeArray<int> halfedges;

			private readonly int maxIters;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<int> intersections;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<int> unresolvedIntersections;

			[NativeDisableContainerSafetyRestriction]
			private NativeArray<int> pointToHalfedge;
		}

		[BurstCompile]
		private struct RefineMeshJob : IJob
		{
			public RefineMeshJob(Triangulator triangulator, NativeList<Triangulator.Edge> constraints)
			{
				this.restoreBoundary = triangulator.Settings.RestoreBoundary;
				this.maximumArea2 = 2f * triangulator.Settings.RefinementThresholds.Area;
				this.minimumAngle = triangulator.Settings.RefinementThresholds.Angle;
				this.D = triangulator.Settings.ConcentricShellsParameter;
				this.scaleRef = triangulator.scale.AsReadOnly();
				this.status = triangulator.status.AsReadOnly();
				this.triangles = triangulator.triangles;
				this.outputPositions = triangulator.outputPositions;
				this.circles = triangulator.circles;
				this.halfedges = triangulator.halfedges;
				this.initialPointsCount = 0;
				this.trianglesQueue = default(NativeQueue<int>);
				this.badTriangles = default(NativeList<int>);
				this.pathPoints = default(NativeList<int>);
				this.pathHalfedges = default(NativeList<int>);
				this.visitedTriangles = default(NativeList<bool>);
				this.constrainedHalfedges = default(NativeList<bool>);
				this.constraints = constraints;
			}

			public void Execute()
			{
				if (this.status.Value != Triangulator.Status.OK)
				{
					return;
				}
				this.initialPointsCount = this.outputPositions.Length;
				this.circles.Length = this.triangles.Length / 3;
				for (int i = 0; i < this.triangles.Length / 3; i++)
				{
					int num = this.triangles[3 * i];
					int num2 = this.triangles[3 * i + 1];
					int num3 = this.triangles[3 * i + 2];
					int i2 = num;
					int j = num2;
					int k = num3;
					this.circles[i] = Triangulator.CalculateCircumCircle(i2, j, k, this.outputPositions.AsArray());
				}
				using (this.trianglesQueue = new NativeQueue<int>(Allocator.Temp))
				{
					using (this.badTriangles = new NativeList<int>(this.triangles.Length / 3, Allocator.Temp))
					{
						using (this.pathPoints = new NativeList<int>(Allocator.Temp))
						{
							using (this.pathHalfedges = new NativeList<int>(Allocator.Temp))
							{
								using (this.visitedTriangles = new NativeList<bool>(this.triangles.Length / 3, Allocator.Temp))
								{
									using (this.constrainedHalfedges = new NativeList<bool>(this.triangles.Length, Allocator.Temp)
									{
										Length = this.triangles.Length
									})
									{
										using (NativeList<int> heQueue = new NativeList<int>(this.triangles.Length, Allocator.Temp))
										{
											using (NativeList<int> tQueue = new NativeList<int>(this.triangles.Length, Allocator.Temp))
											{
												NativeList<Triangulator.Edge> nativeList6 = default(NativeList<Triangulator.Edge>);
												if (!this.constraints.IsCreated)
												{
													nativeList6 = (this.constraints = new NativeList<Triangulator.Edge>(Allocator.Temp));
													for (int l = 0; l < this.halfedges.Length; l++)
													{
														if (this.halfedges[l] == -1)
														{
															Triangulator.Edge edge = new Triangulator.Edge(this.triangles[l], this.triangles[Triangulator.NextHalfedge(l)]);
															this.constraints.Add(edge);
														}
													}
												}
												if (!this.restoreBoundary)
												{
													for (int m = 0; m < this.halfedges.Length; m++)
													{
														Triangulator.Edge value = new Triangulator.Edge(this.triangles[m], this.triangles[Triangulator.NextHalfedge(m)]);
														if (this.halfedges[m] == -1 && !this.constraints.Contains(value))
														{
															this.constraints.Add(value);
														}
													}
												}
												for (int n = 0; n < this.constrainedHalfedges.Length; n++)
												{
													for (int num4 = 0; num4 < this.constraints.Length; num4++)
													{
														Triangulator.Edge edge = this.constraints[num4];
														int num;
														int num2;
														edge.Deconstruct(out num2, out num);
														int num5 = num2;
														int num6 = num;
														num = this.triangles[n];
														int num7 = this.triangles[Triangulator.NextHalfedge(n)];
														int num8 = num;
														int num9 = num7;
														if (num8 >= num9)
														{
															int num10 = num9;
															num = num8;
															num8 = num10;
															num9 = num;
														}
														else
														{
															int num11 = num8;
															num = num9;
															num8 = num11;
															num9 = num;
														}
														if (num5 == num8 && num6 == num9)
														{
															this.constrainedHalfedges[n] = true;
															break;
														}
													}
												}
												for (int num12 = 0; num12 < this.constrainedHalfedges.Length; num12++)
												{
													if (this.constrainedHalfedges[num12] && this.IsEncroached(num12))
													{
														heQueue.Add(num12);
													}
												}
												this.SplitEncroachedEdges(heQueue, default(NativeList<int>));
												for (int num13 = 0; num13 < this.triangles.Length / 3; num13++)
												{
													if (this.IsBadTriangle(num13))
													{
														tQueue.Add(num13);
													}
												}
												for (int num14 = 0; num14 < tQueue.Length; num14++)
												{
													int num15 = tQueue[num14];
													if (num15 != -1)
													{
														this.SplitTriangle(num15, heQueue, tQueue);
													}
												}
												if (nativeList6.IsCreated)
												{
													nativeList6.Dispose();
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			private void SplitEncroachedEdges(NativeList<int> heQueue, NativeList<int> tQueue)
			{
				for (int i = 0; i < heQueue.Length; i++)
				{
					int num = heQueue[i];
					if (num != -1)
					{
						this.SplitEdge(num, heQueue, tQueue);
					}
				}
				heQueue.Clear();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool IsEncroached(int he0)
			{
				int num = Triangulator.NextHalfedge(he0);
				int index = Triangulator.NextHalfedge(num);
				float2 lhs = this.outputPositions[this.triangles[he0]];
				float2 lhs2 = this.outputPositions[this.triangles[num]];
				float2 rhs = this.outputPositions[this.triangles[index]];
				return math.dot(lhs - rhs, lhs2 - rhs) <= 0f;
			}

			private void SplitEdge(int he, NativeList<int> heQueue, NativeList<int> tQueue)
			{
				int num = this.triangles[he];
				int num2 = this.triangles[Triangulator.NextHalfedge(he)];
				int num3 = num;
				int num4 = num2;
				float2 @float = this.outputPositions[num3];
				float2 float2 = this.outputPositions[num4];
				float2 float3 = @float;
				float2 float4 = float2;
				float2 p;
				if ((num3 < this.initialPointsCount && num4 < this.initialPointsCount) || (num3 >= this.initialPointsCount && num4 >= this.initialPointsCount))
				{
					p = 0.5f * (float3 + float4);
				}
				else
				{
					float num5 = math.distance(float3, float4);
					int num6 = (int)math.round(math.log2(0.5f * num5 / this.D));
					float num7 = this.D / num5 * (float)(1 << num6);
					num7 = ((num3 < this.initialPointsCount) ? num7 : (1f - num7));
					p = (1f - num7) * float3 + num7 * float4;
				}
				this.constrainedHalfedges[he] = false;
				int num8 = this.halfedges[he];
				if (num8 != -1)
				{
					this.constrainedHalfedges[num8] = false;
				}
				if (this.halfedges[he] != -1)
				{
					this.UnsafeInsertPointBulk(p, he / 3, heQueue, tQueue);
					int num9 = this.triangles.Length - 3;
					int num10 = -1;
					int num11 = -1;
					while (num10 == -1 || num11 == -1)
					{
						int num12 = Triangulator.NextHalfedge(num9);
						if (this.triangles[num12] == num3)
						{
							num10 = num9;
						}
						if (this.triangles[num12] == num4)
						{
							num11 = num9;
						}
						int index = Triangulator.NextHalfedge(num12);
						num9 = this.halfedges[index];
					}
					if (this.IsEncroached(num10))
					{
						heQueue.Add(num10);
					}
					int num13 = this.halfedges[num10];
					if (this.IsEncroached(num13))
					{
						heQueue.Add(num13);
					}
					if (this.IsEncroached(num11))
					{
						heQueue.Add(num11);
					}
					int num14 = this.halfedges[num11];
					if (this.IsEncroached(num14))
					{
						heQueue.Add(num14);
					}
					this.constrainedHalfedges[num10] = true;
					this.constrainedHalfedges[num13] = true;
					this.constrainedHalfedges[num11] = true;
					this.constrainedHalfedges[num14] = true;
					return;
				}
				this.UnsafeInsertPointBoundary(p, he, heQueue, tQueue);
				int num15 = 3 * (this.pathPoints.Length - 1);
				int num16 = this.halfedges.Length - 1;
				int num17 = this.halfedges.Length - num15;
				if (this.IsEncroached(num16))
				{
					heQueue.Add(num16);
				}
				if (this.IsEncroached(num17))
				{
					heQueue.Add(num17);
				}
				this.constrainedHalfedges[num16] = true;
				this.constrainedHalfedges[num17] = true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool IsBadTriangle(int tId)
			{
				int num = this.triangles[3 * tId];
				int num2 = this.triangles[3 * tId + 1];
				int num3 = this.triangles[3 * tId + 2];
				int num4 = num;
				int num5 = num2;
				int num6 = num3;
				float2 value = this.scaleRef.Value;
				int i = num4;
				int j = num5;
				int k = num6;
				NativeArray<float2> nativeArray = this.outputPositions.AsArray();
				return Triangulator.Area2(i, j, k, nativeArray) > this.maximumArea2 * value.x * value.y || this.AngleIsTooSmall(tId, this.minimumAngle);
			}

			private void SplitTriangle(int tId, NativeList<int> heQueue, NativeList<int> tQueue)
			{
				Triangulator.Circle circle = this.circles[tId];
				NativeList<int> nativeList = new NativeList<int>(Allocator.Temp);
				int num;
				for (int i = 0; i < this.constrainedHalfedges.Length; i++)
				{
					if (this.constrainedHalfedges[i])
					{
						num = this.triangles[i];
						int num2 = this.triangles[Triangulator.NextHalfedge(i)];
						int num3 = num;
						int num4 = num2;
						if (this.halfedges[i] == -1 || num3 < num4)
						{
							float2 @float = this.outputPositions[num3];
							float2 float2 = this.outputPositions[num4];
							float2 lhs = @float;
							float2 lhs2 = float2;
							if (math.dot(lhs - circle.Center, lhs2 - circle.Center) <= 0f)
							{
								nativeList.Add(i);
							}
						}
					}
				}
				if (nativeList.IsEmpty)
				{
					this.UnsafeInsertPointBulk(circle.Center, tId, heQueue, tQueue);
					return;
				}
				float2 value = this.scaleRef.Value;
				num = this.triangles[3 * tId];
				int num5 = this.triangles[3 * tId + 1];
				int num6 = this.triangles[3 * tId + 2];
				int num7 = num;
				int num8 = num5;
				int num9 = num6;
				int i2 = num7;
				int j = num8;
				int k = num9;
				NativeArray<float2> nativeArray = this.outputPositions.AsArray();
				if (Triangulator.Area2(i2, j, k, nativeArray) > this.maximumArea2 * value.x * value.y)
				{
					foreach (int num10 in nativeList.AsReadOnly())
					{
						heQueue.Add(num10);
					}
				}
				if (!heQueue.IsEmpty)
				{
					tQueue.Add(tId);
					this.SplitEncroachedEdges(heQueue, tQueue);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool AngleIsTooSmall(int tId, float minimumAngle)
			{
				int num = this.triangles[3 * tId];
				int num2 = this.triangles[3 * tId + 1];
				int num3 = this.triangles[3 * tId + 2];
				int index = num;
				int index2 = num2;
				int index3 = num3;
				float2 @float = this.outputPositions[index];
				float2 float2 = this.outputPositions[index2];
				float2 float3 = this.outputPositions[index3];
				float2 float4 = @float;
				float2 float5 = float2;
				float2 float6 = float3;
				float2 float7 = float5 - float4;
				float2 float8 = float6 - float5;
				float2 float9 = float4 - float6;
				return math.any(math.abs(math.float3(Triangulator.Angle(float7, -float9), Triangulator.Angle(float8, -float7), Triangulator.Angle(float9, -float8))) < minimumAngle);
			}

			private int UnsafeInsertPointCommon(float2 p, int initTriangle)
			{
				int length = this.outputPositions.Length;
				this.outputPositions.Add(p);
				this.badTriangles.Clear();
				this.trianglesQueue.Clear();
				this.pathPoints.Clear();
				this.pathHalfedges.Clear();
				this.visitedTriangles.Clear();
				this.visitedTriangles.Length = this.triangles.Length / 3;
				this.trianglesQueue.Enqueue(initTriangle);
				this.badTriangles.Add(initTriangle);
				this.visitedTriangles[initTriangle] = true;
				this.RecalculateBadTriangles(p);
				return length;
			}

			private void UnsafeInsertPointBulk(float2 p, int initTriangle, NativeList<int> heQueue = default(NativeList<int>), NativeList<int> tQueue = default(NativeList<int>))
			{
				int pId = this.UnsafeInsertPointCommon(p, initTriangle);
				this.BuildStarPolygon();
				this.ProcessBadTriangles(heQueue, tQueue);
				this.BuildNewTrianglesForStar(pId, heQueue, tQueue);
			}

			private void UnsafeInsertPointBoundary(float2 p, int initHe, NativeList<int> heQueue = default(NativeList<int>), NativeList<int> tQueue = default(NativeList<int>))
			{
				int pId = this.UnsafeInsertPointCommon(p, initHe / 3);
				this.BuildAmphitheaterPolygon(initHe);
				this.ProcessBadTriangles(heQueue, tQueue);
				this.BuildNewTrianglesForAmphitheater(pId, heQueue, tQueue);
			}

			private void RecalculateBadTriangles(float2 p)
			{
				int num;
				while (this.trianglesQueue.TryDequeue(out num))
				{
					this.VisitEdge(p, 3 * num);
					this.VisitEdge(p, 3 * num + 1);
					this.VisitEdge(p, 3 * num + 2);
				}
			}

			private void VisitEdge(float2 p, int t0)
			{
				int num = this.halfedges[t0];
				if (num == -1 || this.constrainedHalfedges[num])
				{
					return;
				}
				int num2 = num / 3;
				if (this.visitedTriangles[num2])
				{
					return;
				}
				Triangulator.Circle circle = this.circles[num2];
				if (math.distancesq(circle.Center, p) <= circle.RadiusSq)
				{
					this.badTriangles.Add(num2);
					this.trianglesQueue.Enqueue(num2);
					this.visitedTriangles[num2] = true;
				}
			}

			private void BuildAmphitheaterPolygon(int initHe)
			{
				int num = initHe;
				int num2 = this.triangles[num];
				int num4;
				for (;;)
				{
					num = Triangulator.NextHalfedge(num);
					if (this.triangles[num] == num2)
					{
						break;
					}
					int num3 = this.halfedges[num];
					if (num3 == -1 || !this.badTriangles.Contains(num3 / 3))
					{
						num4 = this.triangles[num];
						this.pathPoints.Add(num4);
						this.pathHalfedges.Add(num3);
					}
					else
					{
						num = num3;
					}
				}
				num4 = this.triangles[initHe];
				this.pathPoints.Add(num4);
				num4 = -1;
				this.pathHalfedges.Add(num4);
			}

			private void BuildStarPolygon()
			{
				int num = -1;
				for (int i = 0; i < this.badTriangles.Length; i++)
				{
					int num2 = this.badTriangles[i];
					for (int j = 0; j < 3; j++)
					{
						int num3 = 3 * num2 + j;
						int num4 = this.halfedges[num3];
						if (num4 == -1 || !this.badTriangles.Contains(num4 / 3))
						{
							int num5 = this.triangles[num3];
							this.pathPoints.Add(num5);
							this.pathHalfedges.Add(num4);
							num = num3;
							break;
						}
					}
					if (num != -1)
					{
						break;
					}
				}
				int num6 = num;
				int num7 = this.pathPoints[0];
				for (;;)
				{
					num6 = Triangulator.NextHalfedge(num6);
					if (this.triangles[num6] == num7)
					{
						break;
					}
					int num8 = this.halfedges[num6];
					if (num8 == -1 || !this.badTriangles.Contains(num8 / 3))
					{
						int num5 = this.triangles[num6];
						this.pathPoints.Add(num5);
						this.pathHalfedges.Add(num8);
					}
					else
					{
						num6 = num8;
					}
				}
			}

			private void ProcessBadTriangles(NativeList<int> heQueue, NativeList<int> tQueue)
			{
				this.badTriangles.Sort<int>();
				for (int i = this.badTriangles.Length - 1; i >= 0; i--)
				{
					int num = this.badTriangles[i];
					this.triangles.RemoveAt(3 * num + 2);
					this.triangles.RemoveAt(3 * num + 1);
					this.triangles.RemoveAt(3 * num);
					this.circles.RemoveAt(num);
					this.RemoveHalfedge(3 * num + 2, 0);
					this.RemoveHalfedge(3 * num + 1, 1);
					this.RemoveHalfedge(3 * num, 2);
					this.constrainedHalfedges.RemoveAt(3 * num + 2);
					this.constrainedHalfedges.RemoveAt(3 * num + 1);
					this.constrainedHalfedges.RemoveAt(3 * num);
					for (int j = 3 * num; j < this.halfedges.Length; j++)
					{
						int num2 = this.halfedges[j];
						if (num2 != -1)
						{
							ref NativeList<int> ptr = ref this.halfedges;
							int index = (num2 < 3 * num) ? num2 : j;
							ptr[index] -= 3;
						}
					}
					for (int k = 0; k < this.pathHalfedges.Length; k++)
					{
						if (this.pathHalfedges[k] > 3 * num + 2)
						{
							ref NativeList<int> ptr = ref this.pathHalfedges;
							int index = k;
							ptr[index] -= 3;
						}
					}
					if (heQueue.IsCreated)
					{
						for (int l = 0; l < heQueue.Length; l++)
						{
							int num3 = heQueue[l];
							if (num3 == 3 * num || num3 == 3 * num + 1 || num3 == 3 * num + 2)
							{
								heQueue[l] = -1;
							}
							else if (num3 > 3 * num + 2)
							{
								ref NativeList<int> ptr = ref heQueue;
								int index = l;
								ptr[index] -= 3;
							}
						}
					}
					if (tQueue.IsCreated)
					{
						for (int m = 0; m < tQueue.Length; m++)
						{
							int num4 = tQueue[m];
							if (num4 == num)
							{
								tQueue[m] = -1;
							}
							else if (num4 > num)
							{
								int index = m;
								int num5 = tQueue[index];
								tQueue[index] = num5 - 1;
							}
						}
					}
				}
			}

			private void RemoveHalfedge(int he, int offset)
			{
				int num = this.halfedges[he];
				int num2 = (num > he) ? (num - offset) : num;
				if (num2 > -1)
				{
					this.halfedges[num2] = -1;
				}
				this.halfedges.RemoveAt(he);
			}

			private void BuildNewTrianglesForStar(int pId, NativeList<int> heQueue, NativeList<int> tQueue)
			{
				int length = this.triangles.Length;
				this.triangles.Length = this.triangles.Length + 3 * this.pathPoints.Length;
				this.circles.Length = this.circles.Length + this.pathPoints.Length;
				for (int i = 0; i < this.pathPoints.Length - 1; i++)
				{
					this.triangles[length + 3 * i] = pId;
					this.triangles[length + 3 * i + 1] = this.pathPoints[i];
					this.triangles[length + 3 * i + 2] = this.pathPoints[i + 1];
					this.circles[length / 3 + i] = Triangulator.CalculateCircumCircle(pId, this.pathPoints[i], this.pathPoints[i + 1], this.outputPositions.AsArray());
				}
				this.triangles[this.triangles.Length - 3] = pId;
				this.triangles[this.triangles.Length - 2] = this.pathPoints[this.pathPoints.Length - 1];
				this.triangles[this.triangles.Length - 1] = this.pathPoints[0];
				this.circles[this.circles.Length - 1] = Triangulator.CalculateCircumCircle(pId, this.pathPoints[this.pathPoints.Length - 1], this.pathPoints[0], this.outputPositions.AsArray());
				int length2 = this.halfedges.Length;
				this.halfedges.Length = this.halfedges.Length + 3 * this.pathPoints.Length;
				this.constrainedHalfedges.Length = this.constrainedHalfedges.Length + 3 * this.pathPoints.Length;
				for (int j = 0; j < this.pathPoints.Length - 1; j++)
				{
					int num = this.pathHalfedges[j];
					this.halfedges[3 * j + 1 + length2] = num;
					if (num != -1)
					{
						this.halfedges[num] = 3 * j + 1 + length2;
						this.constrainedHalfedges[3 * j + 1 + length2] = this.constrainedHalfedges[num];
					}
					else
					{
						this.constrainedHalfedges[3 * j + 1 + length2] = true;
					}
					this.halfedges[3 * j + 2 + length2] = 3 * j + 3 + length2;
					this.halfedges[3 * j + 3 + length2] = 3 * j + 2 + length2;
				}
				int num2 = this.pathHalfedges[this.pathHalfedges.Length - 1];
				this.halfedges[length2 + 3 * (this.pathPoints.Length - 1) + 1] = num2;
				if (num2 != -1)
				{
					this.halfedges[num2] = length2 + 3 * (this.pathPoints.Length - 1) + 1;
					this.constrainedHalfedges[length2 + 3 * (this.pathPoints.Length - 1) + 1] = this.constrainedHalfedges[num2];
				}
				else
				{
					this.constrainedHalfedges[length2 + 3 * (this.pathPoints.Length - 1) + 1] = true;
				}
				this.halfedges[length2] = length2 + 3 * (this.pathPoints.Length - 1) + 2;
				this.halfedges[length2 + 3 * (this.pathPoints.Length - 1) + 2] = length2;
				if (heQueue.IsCreated)
				{
					for (int k = 0; k < this.pathPoints.Length - 1; k++)
					{
						int num3 = length2 + 3 * k + 1;
						if (this.constrainedHalfedges[num3] && this.IsEncroached(num3))
						{
							heQueue.Add(num3);
						}
						else if (tQueue.IsCreated && this.IsBadTriangle(num3 / 3))
						{
							int num4 = num3 / 3;
							tQueue.Add(num4);
						}
					}
				}
			}

			private void BuildNewTrianglesForAmphitheater(int pId, NativeList<int> heQueue, NativeList<int> tQueue)
			{
				int length = this.triangles.Length;
				this.triangles.Length = this.triangles.Length + 3 * (this.pathPoints.Length - 1);
				this.circles.Length = this.circles.Length + (this.pathPoints.Length - 1);
				for (int i = 0; i < this.pathPoints.Length - 1; i++)
				{
					this.triangles[length + 3 * i] = pId;
					this.triangles[length + 3 * i + 1] = this.pathPoints[i];
					this.triangles[length + 3 * i + 2] = this.pathPoints[i + 1];
					this.circles[length / 3 + i] = Triangulator.CalculateCircumCircle(pId, this.pathPoints[i], this.pathPoints[i + 1], this.outputPositions.AsArray());
				}
				int length2 = this.halfedges.Length;
				this.halfedges.Length = this.halfedges.Length + 3 * (this.pathPoints.Length - 1);
				this.constrainedHalfedges.Length = this.constrainedHalfedges.Length + 3 * (this.pathPoints.Length - 1);
				for (int j = 0; j < this.pathPoints.Length - 2; j++)
				{
					int num = this.pathHalfedges[j];
					this.halfedges[3 * j + 1 + length2] = num;
					if (num != -1)
					{
						this.halfedges[num] = 3 * j + 1 + length2;
						this.constrainedHalfedges[3 * j + 1 + length2] = this.constrainedHalfedges[num];
					}
					else
					{
						this.constrainedHalfedges[3 * j + 1 + length2] = true;
					}
					this.halfedges[3 * j + 2 + length2] = 3 * j + 3 + length2;
					this.halfedges[3 * j + 3 + length2] = 3 * j + 2 + length2;
				}
				int num2 = this.pathHalfedges[this.pathHalfedges.Length - 2];
				this.halfedges[length2 + 3 * (this.pathPoints.Length - 2) + 1] = num2;
				if (num2 != -1)
				{
					this.halfedges[num2] = length2 + 3 * (this.pathPoints.Length - 2) + 1;
					this.constrainedHalfedges[length2 + 3 * (this.pathPoints.Length - 2) + 1] = this.constrainedHalfedges[num2];
				}
				else
				{
					this.constrainedHalfedges[length2 + 3 * (this.pathPoints.Length - 2) + 1] = true;
				}
				this.halfedges[length2] = -1;
				this.halfedges[length2 + 3 * (this.pathPoints.Length - 2) + 2] = -1;
				if (heQueue.IsCreated)
				{
					for (int k = 0; k < this.pathPoints.Length - 1; k++)
					{
						int num3 = length2 + 3 * k + 1;
						if (this.constrainedHalfedges[num3] && this.IsEncroached(num3))
						{
							heQueue.Add(num3);
						}
						else if (tQueue.IsCreated && this.IsBadTriangle(num3 / 3))
						{
							int num4 = num3 / 3;
							tQueue.Add(num4);
						}
					}
				}
			}

			private int initialPointsCount;

			private readonly bool restoreBoundary;

			private readonly float maximumArea2;

			private readonly float minimumAngle;

			private readonly float D;

			private NativeReference<float2>.ReadOnly scaleRef;

			private NativeReference<Triangulator.Status>.ReadOnly status;

			private NativeList<int> triangles;

			private NativeList<float2> outputPositions;

			private NativeList<Triangulator.Circle> circles;

			private NativeList<int> halfedges;

			[NativeDisableContainerSafetyRestriction]
			private NativeQueue<int> trianglesQueue;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<int> badTriangles;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<int> pathPoints;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<int> pathHalfedges;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<bool> visitedTriangles;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<Triangulator.Edge> constraints;

			[NativeDisableContainerSafetyRestriction]
			private NativeList<bool> constrainedHalfedges;
		}

		private interface IPlantingSeedJobMode<TSelf>
		{
			TSelf Create(Triangulator triangulator);

			bool PlantBoundarySeed { get; }

			bool PlantHolesSeed { get; }

			NativeArray<float2> HoleSeeds { get; }
		}

		private readonly struct PlantBoundary : Triangulator.IPlantingSeedJobMode<Triangulator.PlantBoundary>
		{
			public bool PlantBoundarySeed
			{
				get
				{
					return true;
				}
			}

			public bool PlantHolesSeed
			{
				get
				{
					return false;
				}
			}

			public NativeArray<float2> HoleSeeds
			{
				get
				{
					return default(NativeArray<float2>);
				}
			}

			public Triangulator.PlantBoundary Create(Triangulator _)
			{
				return default(Triangulator.PlantBoundary);
			}
		}

		private struct PlantHoles : Triangulator.IPlantingSeedJobMode<Triangulator.PlantHoles>
		{
			public readonly bool PlantBoundarySeed
			{
				get
				{
					return false;
				}
			}

			public readonly bool PlantHolesSeed
			{
				get
				{
					return true;
				}
			}

			public readonly NativeArray<float2> HoleSeeds
			{
				get
				{
					return this.holeSeeds;
				}
			}

			public Triangulator.PlantHoles Create(Triangulator triangulator)
			{
				return new Triangulator.PlantHoles
				{
					holeSeeds = triangulator.Input.HoleSeeds
				};
			}

			private NativeArray<float2> holeSeeds;
		}

		private struct PlantBoundaryAndHoles : Triangulator.IPlantingSeedJobMode<Triangulator.PlantBoundaryAndHoles>
		{
			public readonly bool PlantBoundarySeed
			{
				get
				{
					return true;
				}
			}

			public readonly bool PlantHolesSeed
			{
				get
				{
					return true;
				}
			}

			public readonly NativeArray<float2> HoleSeeds
			{
				get
				{
					return this.holeSeeds;
				}
			}

			public Triangulator.PlantBoundaryAndHoles Create(Triangulator triangulator)
			{
				return new Triangulator.PlantBoundaryAndHoles
				{
					holeSeeds = triangulator.Input.HoleSeeds
				};
			}

			private NativeArray<float2> holeSeeds;
		}

		[BurstCompile]
		private struct PlantingSeedsJob<T> : IJob where T : struct, Triangulator.IPlantingSeedJobMode<T>
		{
			public PlantingSeedsJob(Triangulator triangulator)
			{
				this.positions = triangulator.Input.Positions;
				T t = default(T);
				this.mode = t.Create(triangulator);
				this.status = triangulator.status.AsReadOnly();
				this.triangles = triangulator.triangles;
				this.outputPositions = triangulator.outputPositions;
				this.circles = triangulator.circles;
				this.constraintEdges = triangulator.constraintEdges;
				this.halfedges = triangulator.halfedges;
			}

			public void Execute()
			{
				if (this.status.Value != Triangulator.Status.OK)
				{
					return;
				}
				if (this.circles.Length != this.triangles.Length / 3)
				{
					this.circles.Length = this.triangles.Length / 3;
					for (int i = 0; i < this.triangles.Length / 3; i++)
					{
						int num = this.triangles[3 * i];
						int num2 = this.triangles[3 * i + 1];
						int num3 = this.triangles[3 * i + 2];
						int i2 = num;
						int j = num2;
						int k = num3;
						this.circles[i] = Triangulator.CalculateCircumCircle(i2, j, k, this.outputPositions.AsArray());
					}
				}
				using (NativeArray<bool> visitedTriangles = new NativeArray<bool>(this.triangles.Length / 3, Allocator.Temp, NativeArrayOptions.ClearMemory))
				{
					using (NativeList<int> badTriangles = new NativeList<int>(this.triangles.Length / 3, Allocator.Temp))
					{
						using (NativeQueue<int> trianglesQueue = new NativeQueue<int>(Allocator.Temp))
						{
							this.PlantSeeds(visitedTriangles, badTriangles, trianglesQueue);
							using (NativeHashSet<int> potentialPointsToRemove = new NativeHashSet<int>(3 * badTriangles.Length, Allocator.Temp))
							{
								this.GeneratePotentialPointsToRemove(this.positions.Length, potentialPointsToRemove, badTriangles);
								this.RemoveBadTriangles(badTriangles);
								this.RemovePoints(potentialPointsToRemove);
							}
						}
					}
				}
			}

			private void PlantSeeds(NativeArray<bool> visitedTriangles, NativeList<int> badTriangles, NativeQueue<int> trianglesQueue)
			{
				T t = this.mode;
				if (t.PlantBoundarySeed)
				{
					for (int i = 0; i < this.halfedges.Length; i++)
					{
						if (this.halfedges[i] == -1 && !visitedTriangles[i / 3] && !this.constraintEdges.Contains(new Triangulator.Edge(this.triangles[i], this.triangles[Triangulator.NextHalfedge(i)])))
						{
							this.PlantSeed(i / 3, visitedTriangles, badTriangles, trianglesQueue);
						}
					}
				}
				t = this.mode;
				if (t.PlantHolesSeed)
				{
					t = this.mode;
					foreach (float2 p in t.HoleSeeds)
					{
						int num = this.FindTriangle(p);
						if (num != -1)
						{
							this.PlantSeed(num, visitedTriangles, badTriangles, trianglesQueue);
						}
					}
				}
			}

			private void PlantSeed(int tId, NativeArray<bool> visitedTriangles, NativeList<int> badTriangles, NativeQueue<int> trianglesQueue)
			{
				Triangulator.PlantingSeedsJob<T>.<>c__DisplayClass11_0 CS$<>8__locals1;
				CS$<>8__locals1.visitedTriangles = visitedTriangles;
				CS$<>8__locals1.trianglesQueue = trianglesQueue;
				CS$<>8__locals1.badTriangles = badTriangles;
				if (CS$<>8__locals1.visitedTriangles[tId])
				{
					return;
				}
				CS$<>8__locals1.visitedTriangles[tId] = true;
				CS$<>8__locals1.trianglesQueue.Enqueue(tId);
				CS$<>8__locals1.badTriangles.Add(tId);
				while (CS$<>8__locals1.trianglesQueue.TryDequeue(out tId))
				{
					int num = this.triangles[3 * tId];
					int num2 = this.triangles[3 * tId + 1];
					int num3 = this.triangles[3 * tId + 2];
					int num4 = num;
					int num5 = num2;
					int num6 = num3;
					Triangulator.PlantingSeedsJob<T>.<PlantSeed>g__TryEnqueue|11_0(new Triangulator.Edge(num4, num5), 3 * tId, this.constraintEdges, this.halfedges, ref CS$<>8__locals1);
					Triangulator.PlantingSeedsJob<T>.<PlantSeed>g__TryEnqueue|11_0(new Triangulator.Edge(num5, num6), 3 * tId + 1, this.constraintEdges, this.halfedges, ref CS$<>8__locals1);
					Triangulator.PlantingSeedsJob<T>.<PlantSeed>g__TryEnqueue|11_0(new Triangulator.Edge(num6, num4), 3 * tId + 2, this.constraintEdges, this.halfedges, ref CS$<>8__locals1);
				}
			}

			private int FindTriangle(float2 p)
			{
				for (int i = 0; i < this.triangles.Length / 3; i++)
				{
					int num = this.triangles[3 * i];
					int num2 = this.triangles[3 * i + 1];
					int num3 = this.triangles[3 * i + 2];
					int index = num;
					int index2 = num2;
					int index3 = num3;
					float2 @float = this.outputPositions[index];
					float2 float2 = this.outputPositions[index2];
					float2 float3 = this.outputPositions[index3];
					float2 a = @float;
					float2 b = float2;
					float2 c = float3;
					if (Triangulator.PointInsideTriangle(p, a, b, c))
					{
						return i;
					}
				}
				return -1;
			}

			private void GeneratePotentialPointsToRemove(int initialPointsCount, NativeHashSet<int> potentialPointsToRemove, NativeList<int> badTriangles)
			{
				foreach (int num in badTriangles.AsReadOnly())
				{
					for (int i = 0; i < 3; i++)
					{
						int num2 = this.triangles[3 * num + i];
						if (num2 >= initialPointsCount)
						{
							potentialPointsToRemove.Add(num2);
						}
					}
				}
			}

			private void RemoveBadTriangles(NativeList<int> badTriangles)
			{
				badTriangles.Sort<int>();
				for (int i = badTriangles.Length - 1; i >= 0; i--)
				{
					int num = badTriangles[i];
					this.triangles.RemoveAt(3 * num + 2);
					this.triangles.RemoveAt(3 * num + 1);
					this.triangles.RemoveAt(3 * num);
					this.circles.RemoveAt(num);
					this.RemoveHalfedge(3 * num + 2, 0);
					this.RemoveHalfedge(3 * num + 1, 1);
					this.RemoveHalfedge(3 * num, 2);
					for (int j = 3 * num; j < this.halfedges.Length; j++)
					{
						int num2 = this.halfedges[j];
						if (num2 != -1)
						{
							ref NativeList<int> ptr = ref this.halfedges;
							int index = (num2 < 3 * num) ? num2 : j;
							ptr[index] -= 3;
						}
					}
				}
			}

			private void RemoveHalfedge(int he, int offset)
			{
				int num = this.halfedges[he];
				int num2 = (num > he) ? (num - offset) : num;
				if (num2 > -1)
				{
					this.halfedges[num2] = -1;
				}
				this.halfedges.RemoveAt(he);
			}

			private void RemovePoints(NativeHashSet<int> potentialPointsToRemove)
			{
				NativeArray<int> nativeArray = new NativeArray<int>(this.outputPositions.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<int> nativeArray2 = new NativeArray<int>(this.outputPositions.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					nativeArray[i] = -1;
				}
				for (int j = 0; j < this.triangles.Length; j++)
				{
					nativeArray[this.triangles[j]] = j;
				}
				using (NativeArray<int> array = potentialPointsToRemove.ToNativeArray(Allocator.Temp))
				{
					array.Sort<int>();
					for (int k = array.Length - 1; k >= 0; k--)
					{
						int num = array[k];
						if (nativeArray[num] == -1)
						{
							this.outputPositions.RemoveAt(num);
							for (int l = num; l < nativeArray2.Length; l++)
							{
								int index = l;
								int num2 = nativeArray2[index];
								nativeArray2[index] = num2 - 1;
							}
						}
					}
					for (int m = 0; m < this.triangles.Length; m++)
					{
						ref NativeList<int> ptr = ref this.triangles;
						int num2 = m;
						ptr[num2] += nativeArray2[this.triangles[m]];
					}
					nativeArray.Dispose();
					nativeArray2.Dispose();
				}
			}

			[CompilerGenerated]
			internal static void <PlantSeed>g__TryEnqueue|11_0(Triangulator.Edge e, int he, NativeList<Triangulator.Edge> constraintEdges, NativeList<int> halfedges, ref Triangulator.PlantingSeedsJob<T>.<>c__DisplayClass11_0 A_4)
			{
				int num = halfedges[he];
				if (constraintEdges.Contains(e) || num == -1)
				{
					return;
				}
				int num2 = num / 3;
				if (!A_4.visitedTriangles[num2])
				{
					A_4.visitedTriangles[num2] = true;
					A_4.trianglesQueue.Enqueue(num2);
					A_4.badTriangles.Add(num2);
				}
			}

			[ReadOnly]
			private NativeArray<float2> positions;

			private readonly T mode;

			private NativeReference<Triangulator.Status>.ReadOnly status;

			private NativeList<int> triangles;

			private NativeList<float2> outputPositions;

			private NativeList<Triangulator.Circle> circles;

			private NativeList<Triangulator.Edge> constraintEdges;

			private NativeList<int> halfedges;
		}
	}
}
