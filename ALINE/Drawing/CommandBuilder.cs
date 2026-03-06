using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Drawing
{
	[BurstCompile]
	public struct CommandBuilder : IDisposable
	{
		internal unsafe CommandBuilder(UnsafeAppendBuffer* buffer, GCHandle gizmos, int threadIndex, DrawingData.BuilderData.BitPackedMeta uniqueID)
		{
			this.buffer = buffer;
			this.gizmos = gizmos;
			this.threadIndex = threadIndex;
			this.uniqueID = uniqueID;
		}

		internal CommandBuilder(DrawingData gizmos, DrawingData.Hasher hasher, RedrawScope frameRedrawScope, RedrawScope customRedrawScope, bool isGizmos, bool isBuiltInCommandBuilder, int sceneModeVersion)
		{
			this.gizmos = GCHandle.Alloc(gizmos, GCHandleType.Normal);
			this.threadIndex = 0;
			this.uniqueID = gizmos.data.Reserve(isBuiltInCommandBuilder);
			gizmos.data.Get(this.uniqueID).Init(hasher, frameRedrawScope, customRedrawScope, isGizmos, gizmos.GetNextDrawOrderIndex(), sceneModeVersion);
			this.buffer = gizmos.data.Get(this.uniqueID).bufferPtr;
		}

		internal unsafe int BufferSize
		{
			get
			{
				return this.buffer->Length;
			}
			set
			{
				this.buffer->Length = value;
			}
		}

		public CommandBuilder2D xy
		{
			get
			{
				return new CommandBuilder2D(this, true);
			}
		}

		public CommandBuilder2D xz
		{
			get
			{
				return new CommandBuilder2D(this, false);
			}
		}

		public Camera[] cameraTargets
		{
			get
			{
				if (this.gizmos.IsAllocated && this.gizmos.Target != null)
				{
					DrawingData drawingData = this.gizmos.Target as DrawingData;
					if (drawingData.data.StillExists(this.uniqueID))
					{
						return drawingData.data.Get(this.uniqueID).meta.cameraTargets;
					}
				}
				throw new Exception("Cannot get cameraTargets because the command builder has already been disposed or does not exist.");
			}
			set
			{
				if (this.uniqueID.isBuiltInCommandBuilder)
				{
					throw new Exception("You cannot set the camera targets for a built-in command builder. Create a custom command builder instead.");
				}
				if (this.gizmos.IsAllocated && this.gizmos.Target != null)
				{
					DrawingData drawingData = this.gizmos.Target as DrawingData;
					if (!drawingData.data.StillExists(this.uniqueID))
					{
						throw new Exception("Cannot set cameraTargets because the command builder has already been disposed or does not exist.");
					}
					drawingData.data.Get(this.uniqueID).meta.cameraTargets = value;
				}
			}
		}

		public void Dispose()
		{
			if (this.uniqueID.isBuiltInCommandBuilder)
			{
				throw new Exception("You cannot dispose a built-in command builder");
			}
			this.DisposeInternal();
		}

		public void DisposeAfter(JobHandle dependency, AllowedDelay allowedDelay = AllowedDelay.EndOfFrame)
		{
			if (!this.gizmos.IsAllocated)
			{
				throw new Exception("You cannot dispose an invalid command builder. Are you trying to dispose it twice?");
			}
			try
			{
				if (this.gizmos.IsAllocated && this.gizmos.Target != null)
				{
					DrawingData drawingData = this.gizmos.Target as DrawingData;
					if (!drawingData.data.StillExists(this.uniqueID))
					{
						throw new Exception("Cannot dispose the command builder because the drawing manager has been destroyed");
					}
					drawingData.data.Get(this.uniqueID).SubmitWithDependency(this.gizmos, dependency, allowedDelay);
				}
			}
			finally
			{
				this = default(CommandBuilder);
			}
		}

		internal void DisposeInternal()
		{
			if (!this.gizmos.IsAllocated)
			{
				throw new Exception("You cannot dispose an invalid command builder. Are you trying to dispose it twice?");
			}
			try
			{
				if (this.gizmos.IsAllocated && this.gizmos.Target != null)
				{
					DrawingData drawingData = this.gizmos.Target as DrawingData;
					if (!drawingData.data.StillExists(this.uniqueID))
					{
						throw new Exception("Cannot dispose the command builder because the drawing manager has been destroyed");
					}
					drawingData.data.Get(this.uniqueID).Submit(this.gizmos.Target as DrawingData);
				}
			}
			finally
			{
				this.gizmos.Free();
				this = default(CommandBuilder);
			}
		}

		public void DiscardAndDispose()
		{
			if (this.uniqueID.isBuiltInCommandBuilder)
			{
				throw new Exception("You cannot dispose a built-in command builder");
			}
			this.DiscardAndDisposeInternal();
		}

		internal void DiscardAndDisposeInternal()
		{
			try
			{
				if (this.gizmos.IsAllocated && this.gizmos.Target != null)
				{
					DrawingData drawingData = this.gizmos.Target as DrawingData;
					if (!drawingData.data.StillExists(this.uniqueID))
					{
						throw new Exception("Cannot dispose the command builder because the drawing manager has been destroyed");
					}
					drawingData.data.Release(this.uniqueID);
				}
			}
			finally
			{
				if (this.gizmos.IsAllocated)
				{
					this.gizmos.Free();
				}
				this = default(CommandBuilder);
			}
		}

		public void Preallocate(int size)
		{
			this.Reserve(size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void Reserve(int additionalSpace)
		{
			if (Hint.Unlikely(this.threadIndex >= 0))
			{
				this.buffer += this.threadIndex;
				this.threadIndex = -1;
			}
			int num = this.buffer->Length + additionalSpace;
			if (num > this.buffer->Capacity)
			{
				this.buffer->SetCapacity(math.max(num, this.buffer->Length * 2));
			}
		}

		[BurstDiscard]
		private void AssertBufferExists()
		{
			if (!this.gizmos.IsAllocated || this.gizmos.Target == null || !(this.gizmos.Target as DrawingData).data.StillExists(this.uniqueID))
			{
				this = default(CommandBuilder);
				throw new Exception("This command builder no longer exists. Are you trying to draw to a command builder which has already been disposed?");
			}
		}

		[BurstDiscard]
		private static void AssertNotRendering()
		{
			if (!GizmoContext.drawingGizmos && !JobsUtility.IsExecutingJob && (Time.renderedFrameCount & 127) == 0 && StackTraceUtility.ExtractStackTrace().Contains("OnDrawGizmos"))
			{
				throw new Exception("You are trying to use Draw.* functions from within Unity's OnDrawGizmos function. Use this package's gizmo callbacks instead (see the documentation).");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Reserve<A>() where A : struct
		{
			this.Reserve(UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<A>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Reserve<A, B>() where A : struct where B : struct
		{
			this.Reserve(UnsafeUtility.SizeOf<CommandBuilder.Command>() * 2 + UnsafeUtility.SizeOf<A>() + UnsafeUtility.SizeOf<B>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Reserve<A, B, C>() where A : struct where B : struct where C : struct
		{
			this.Reserve(UnsafeUtility.SizeOf<CommandBuilder.Command>() * 3 + UnsafeUtility.SizeOf<A>() + UnsafeUtility.SizeOf<B>() + UnsafeUtility.SizeOf<C>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint ConvertColor(Color color)
		{
			if (X86.Sse2.IsSse2Supported)
			{
				int4 @int = (int4)(255f * new float4(color.r, color.g, color.b, color.a) + 0.5f);
				v128 v = new v128(@int.x, @int.y, @int.z, @int.w);
				v128 v2 = X86.Sse2.packs_epi32(v, v);
				return X86.Sse2.packus_epi16(v2, v2).UInt0;
			}
			uint num = (uint)Mathf.Clamp((int)(color.r * 255f + 0.5f), 0, 255);
			uint num2 = (uint)Mathf.Clamp((int)(color.g * 255f + 0.5f), 0, 255);
			uint num3 = (uint)Mathf.Clamp((int)(color.b * 255f + 0.5f), 0, 255);
			return (uint)(Mathf.Clamp((int)(color.a * 255f + 0.5f), 0, 255) << 24 | (int)((int)num3 << 16) | (int)((int)num2 << 8) | (int)num);
		}

		internal unsafe void Add<T>(T value) where T : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			UnsafeAppendBuffer* ptr = this.buffer;
			int length = ptr->Length;
			Hint.Assume(ptr->Ptr != null);
			Hint.Assume(ptr->Ptr + length != null);
			UnsafeUtility.CopyStructureToPtr<T>(ref value, (void*)(ptr->Ptr + length));
			ptr->Length = length + num;
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix WithMatrix(Matrix4x4 matrix)
		{
			this.PushMatrix(matrix);
			return new CommandBuilder.ScopeMatrix
			{
				builder = this
			};
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix WithMatrix(float3x3 matrix)
		{
			this.PushMatrix(new float4x4(matrix, float3.zero));
			return new CommandBuilder.ScopeMatrix
			{
				builder = this
			};
		}

		[BurstDiscard]
		public CommandBuilder.ScopeColor WithColor(Color color)
		{
			this.PushColor(color);
			return new CommandBuilder.ScopeColor
			{
				builder = this
			};
		}

		[BurstDiscard]
		public CommandBuilder.ScopePersist WithDuration(float duration)
		{
			this.PushDuration(duration);
			return new CommandBuilder.ScopePersist
			{
				builder = this
			};
		}

		[BurstDiscard]
		public CommandBuilder.ScopeLineWidth WithLineWidth(float pixels, bool automaticJoins = true)
		{
			this.PushLineWidth(pixels, automaticJoins);
			return new CommandBuilder.ScopeLineWidth
			{
				builder = this
			};
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix InLocalSpace(Transform transform)
		{
			return this.WithMatrix(transform.localToWorldMatrix);
		}

		[BurstDiscard]
		public CommandBuilder.ScopeMatrix InScreenSpace(Camera camera)
		{
			return this.WithMatrix(camera.cameraToWorldMatrix * camera.nonJitteredProjectionMatrix.inverse * Matrix4x4.TRS(new Vector3(-1f, -1f, 0f), Quaternion.identity, new Vector3(2f / (float)camera.pixelWidth, 2f / (float)camera.pixelHeight, 1f)));
		}

		public void PushMatrix(Matrix4x4 matrix)
		{
			this.Reserve<float4x4>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushMatrix);
			this.Add<Matrix4x4>(matrix);
		}

		public void PushMatrix(float4x4 matrix)
		{
			this.Reserve<float4x4>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushMatrix);
			this.Add<float4x4>(matrix);
		}

		public void PushSetMatrix(Matrix4x4 matrix)
		{
			this.Reserve<float4x4>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushSetMatrix);
			this.Add<float4x4>(matrix);
		}

		public void PushSetMatrix(float4x4 matrix)
		{
			this.Reserve<float4x4>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushSetMatrix);
			this.Add<float4x4>(matrix);
		}

		public void PopMatrix()
		{
			this.Reserve(4);
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PopMatrix);
		}

		public void PushColor(Color color)
		{
			this.Reserve<Color32>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColor);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
		}

		public void PopColor()
		{
			this.Reserve(4);
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PopColor);
		}

		public unsafe void PushDuration(float duration)
		{
			this.Reserve<CommandBuilder.PersistData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushPersist);
			this.Add<CommandBuilder.PersistData>(new CommandBuilder.PersistData
			{
				endTime = *SharedDrawingData.BurstTime.Data + duration
			});
		}

		public void PopDuration()
		{
			this.Reserve(4);
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PopPersist);
		}

		[Obsolete("Renamed to PushDuration for consistency")]
		public void PushPersist(float duration)
		{
			this.PushDuration(duration);
		}

		[Obsolete("Renamed to PopDuration for consistency")]
		public void PopPersist()
		{
			this.PopDuration();
		}

		public void PushLineWidth(float pixels, bool automaticJoins = true)
		{
			if (pixels < 0f)
			{
				throw new ArgumentOutOfRangeException("pixels", "Line width must be positive");
			}
			this.Reserve<CommandBuilder.LineWidthData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushLineWidth);
			this.Add<CommandBuilder.LineWidthData>(new CommandBuilder.LineWidthData
			{
				pixels = pixels,
				automaticJoins = automaticJoins
			});
		}

		public void PopLineWidth()
		{
			this.Reserve(4);
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PopLineWidth);
		}

		public void Line(float3 a, float3 b)
		{
			this.Reserve<CommandBuilder.LineData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Line);
			this.Add<CommandBuilder.LineData>(new CommandBuilder.LineData
			{
				a = a,
				b = b
			});
		}

		public unsafe void Line(Vector3 a, Vector3 b)
		{
			this.Reserve<CommandBuilder.LineData>();
			int bufferSize = this.BufferSize;
			int length = bufferSize + 4 + 24;
			byte* ptr = this.buffer->Ptr + bufferSize;
			*(int*)ptr = 5;
			CommandBuilder.LineDataV3* ptr2 = (CommandBuilder.LineDataV3*)(ptr + 4);
			ptr2->a = a;
			ptr2->b = b;
			this.buffer->Length = length;
		}

		public unsafe void Line(Vector3 a, Vector3 b, Color color)
		{
			this.Reserve<Color32, CommandBuilder.LineData>();
			int bufferSize = this.BufferSize;
			int length = bufferSize + 4 + 24 + 4;
			byte* ptr = this.buffer->Ptr + bufferSize;
			*(int*)ptr = 261;
			*(int*)(ptr + 4) = (int)CommandBuilder.ConvertColor(color);
			CommandBuilder.LineDataV3* ptr2 = (CommandBuilder.LineDataV3*)(ptr + 8);
			ptr2->a = a;
			ptr2->b = b;
			this.buffer->Length = length;
		}

		public void Ray(float3 origin, float3 direction)
		{
			this.Line(origin, origin + direction);
		}

		public void Ray(Ray ray, float length)
		{
			this.Line(ray.origin, ray.origin + ray.direction * length);
		}

		public void Arc(float3 center, float3 start, float3 end)
		{
			float3 @float = start - center;
			float3 float2 = end - center;
			float3 float3 = math.cross(float2, @float);
			if (math.any(float3 != 0f) && math.all(math.isfinite(float3)))
			{
				Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(@float, float3), Vector3.one);
				float num = Vector3.SignedAngle(@float, float2, float3) * 0.017453292f;
				this.PushMatrix(matrix);
				this.CircleXZInternal(float3.zero, math.length(@float), 1.5707964f, 1.5707964f - num);
				this.PopMatrix();
			}
		}

		[Obsolete("Use Draw.xz.Circle instead")]
		public void CircleXZ(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.CircleXZInternal(center, radius, startAngle, endAngle);
		}

		internal void CircleXZInternal(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.Reserve<CommandBuilder.CircleXZData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.CircleXZ);
			this.Add<CommandBuilder.CircleXZData>(new CommandBuilder.CircleXZData
			{
				center = center,
				radius = radius,
				startAngle = startAngle,
				endAngle = endAngle
			});
		}

		internal void CircleXZInternal(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.Reserve<Color32, CommandBuilder.CircleXZData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.PopMatrix);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.CircleXZData>(new CommandBuilder.CircleXZData
			{
				center = center,
				radius = radius,
				startAngle = startAngle,
				endAngle = endAngle
			});
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
			this.CircleXZ(new float3(center.x, -center.z, center.y), radius, startAngle, endAngle);
			this.PopMatrix();
		}

		public void Circle(float3 center, float3 normal, float radius)
		{
			this.Reserve<CommandBuilder.CircleData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Circle);
			this.Add<CommandBuilder.CircleData>(new CommandBuilder.CircleData
			{
				center = center,
				normal = normal,
				radius = radius
			});
		}

		public void SolidArc(float3 center, float3 start, float3 end)
		{
			float3 @float = start - center;
			float3 float2 = end - center;
			float3 float3 = math.cross(float2, @float);
			if (math.any(float3))
			{
				Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(@float, float3), Vector3.one);
				float num = Vector3.SignedAngle(@float, float2, float3) * 0.017453292f;
				this.PushMatrix(matrix);
				this.SolidCircleXZInternal(float3.zero, math.length(@float), 1.5707964f, 1.5707964f - num);
				this.PopMatrix();
			}
		}

		[Obsolete("Use Draw.xz.SolidCircle instead")]
		public void SolidCircleXZ(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.SolidCircleXZInternal(center, radius, startAngle, endAngle);
		}

		internal void SolidCircleXZInternal(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.Reserve<CommandBuilder.CircleXZData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.DiscXZ);
			this.Add<CommandBuilder.CircleXZData>(new CommandBuilder.CircleXZData
			{
				center = center,
				radius = radius,
				startAngle = startAngle,
				endAngle = endAngle
			});
		}

		internal void SolidCircleXZInternal(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.Reserve<Color32, CommandBuilder.CircleXZData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.CircleXZData>(new CommandBuilder.CircleXZData
			{
				center = center,
				radius = radius,
				startAngle = startAngle,
				endAngle = endAngle
			});
		}

		[Obsolete("Use Draw.xy.SolidCircle instead")]
		public void SolidCircleXY(float3 center, float radius, float startAngle = 0f, float endAngle = 6.2831855f)
		{
			this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
			this.SolidCircleXZInternal(new float3(center.x, -center.z, center.y), radius, startAngle, endAngle);
			this.PopMatrix();
		}

		public void SolidCircle(float3 center, float3 normal, float radius)
		{
			this.Reserve<CommandBuilder.CircleData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Disc);
			this.Add<CommandBuilder.CircleData>(new CommandBuilder.CircleData
			{
				center = center,
				normal = normal,
				radius = radius
			});
		}

		public void SphereOutline(float3 center, float radius)
		{
			this.Reserve<CommandBuilder.SphereData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.SphereOutline);
			this.Add<CommandBuilder.SphereData>(new CommandBuilder.SphereData
			{
				center = center,
				radius = radius
			});
		}

		public void WireCylinder(float3 bottom, float3 top, float radius)
		{
			this.WireCylinder(bottom, top - bottom, math.length(top - bottom), radius);
		}

		public void WireCylinder(float3 position, float3 up, float height, float radius)
		{
			up = math.normalizesafe(up, default(float3));
			if (math.all(up == 0f) || math.any(math.isnan(up)) || math.isnan(height) || math.isnan(radius))
			{
				return;
			}
			float3 lhs;
			float3 lhs2;
			CommandBuilder.OrthonormalBasis(up, out lhs, out lhs2);
			this.PushMatrix(new float4x4(new float4(lhs * radius, 0f), new float4(up * height, 0f), new float4(lhs2 * radius, 0f), new float4(position, 1f)));
			this.CircleXZInternal(float3.zero, 1f, 0f, 6.2831855f);
			if (height > 0f)
			{
				this.CircleXZInternal(new float3(0f, 1f, 0f), 1f, 0f, 6.2831855f);
				this.Line(new float3(1f, 0f, 0f), new float3(1f, 1f, 0f));
				this.Line(new float3(-1f, 0f, 0f), new float3(-1f, 1f, 0f));
				this.Line(new float3(0f, 0f, 1f), new float3(0f, 1f, 1f));
				this.Line(new float3(0f, 0f, -1f), new float3(0f, 1f, -1f));
			}
			this.PopMatrix();
		}

		private static void OrthonormalBasis(float3 normal, out float3 basis1, out float3 basis2)
		{
			basis1 = math.cross(normal, new float3(1f, 1f, 1f));
			if (math.all(basis1 == 0f))
			{
				basis1 = math.cross(normal, new float3(-1f, 1f, 1f));
			}
			basis1 = math.normalizesafe(basis1, default(float3));
			basis2 = math.cross(normal, basis1);
		}

		public void WireCapsule(float3 start, float3 end, float radius)
		{
			float3 @float = end - start;
			float num = math.length(@float);
			if ((double)num < 0.0001)
			{
				this.WireSphere(start, radius);
				return;
			}
			float3 float2 = @float / num;
			this.WireCapsule(start - float2 * radius, float2, num + 2f * radius, radius);
		}

		public void WireCapsule(float3 position, float3 direction, float length, float radius)
		{
			direction = math.normalizesafe(direction, default(float3));
			if (math.all(direction == 0f) || math.any(math.isnan(direction)) || math.isnan(length) || math.isnan(radius))
			{
				return;
			}
			if (radius <= 0f)
			{
				this.Line(position, position + direction * length);
				return;
			}
			length = math.max(length, radius * 2f);
			float3 xyz;
			float3 xyz2;
			CommandBuilder.OrthonormalBasis(direction, out xyz, out xyz2);
			this.PushMatrix(new float4x4(new float4(xyz, 0f), new float4(direction, 0f), new float4(xyz2, 0f), new float4(position, 1f)));
			this.CircleXZInternal(new float3(0f, radius, 0f), radius, 0f, 6.2831855f);
			this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
			this.CircleXZInternal(new float3(0f, 0f, radius), radius, 3.1415927f, 6.2831855f);
			this.PopMatrix();
			this.PushMatrix(CommandBuilder.XZtoYZPlaneMatrix);
			this.CircleXZInternal(new float3(radius, 0f, 0f), radius, 1.5707964f, 4.712389f);
			this.PopMatrix();
			if (length > 0f)
			{
				float num = length - radius;
				this.CircleXZInternal(new float3(0f, num, 0f), radius, 0f, 6.2831855f);
				this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
				this.CircleXZInternal(new float3(0f, 0f, num), radius, 0f, 3.1415927f);
				this.PopMatrix();
				this.PushMatrix(CommandBuilder.XZtoYZPlaneMatrix);
				this.CircleXZInternal(new float3(num, 0f, 0f), radius, -1.5707964f, 1.5707964f);
				this.PopMatrix();
				this.Line(new float3(radius, radius, 0f), new float3(radius, num, 0f));
				this.Line(new float3(-radius, radius, 0f), new float3(-radius, num, 0f));
				this.Line(new float3(0f, radius, radius), new float3(0f, num, radius));
				this.Line(new float3(0f, radius, -radius), new float3(0f, num, -radius));
			}
			this.PopMatrix();
		}

		public void WireSphere(float3 position, float radius)
		{
			this.SphereOutline(position, radius);
			this.Circle(position, new float3(1f, 0f, 0f), radius);
			this.Circle(position, new float3(0f, 1f, 0f), radius);
			this.Circle(position, new float3(0f, 0f, 1f), radius);
		}

		[BurstDiscard]
		public void Polyline(List<Vector3> points, bool cycle = false)
		{
			for (int i = 0; i < points.Count - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Count > 1)
			{
				this.Line(points[points.Count - 1], points[0]);
			}
		}

		public void Polyline<T>(T points, bool cycle = false) where T : IReadOnlyList<float3>
		{
			for (int i = 0; i < points.Count - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Count > 1)
			{
				this.Line(points[points.Count - 1], points[0]);
			}
		}

		[BurstDiscard]
		public void Polyline(Vector3[] points, bool cycle = false)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
		}

		[BurstDiscard]
		public void Polyline(float3[] points, bool cycle = false)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
		}

		public void Polyline(NativeArray<float3> points, bool cycle = false)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
		}

		public void DashedLine(float3 a, float3 b, float dash, float gap)
		{
			CommandBuilder.PolylineWithSymbol polylineWithSymbol = new CommandBuilder.PolylineWithSymbol(CommandBuilder.SymbolDecoration.None, gap, 0f, dash + gap, false);
			polylineWithSymbol.MoveTo(ref this, a);
			polylineWithSymbol.MoveTo(ref this, b);
		}

		public void DashedPolyline(List<Vector3> points, float dash, float gap)
		{
			CommandBuilder.PolylineWithSymbol polylineWithSymbol = new CommandBuilder.PolylineWithSymbol(CommandBuilder.SymbolDecoration.None, gap, 0f, dash + gap, false);
			for (int i = 0; i < points.Count; i++)
			{
				polylineWithSymbol.MoveTo(ref this, points[i]);
			}
		}

		public void WireBox(float3 center, float3 size)
		{
			this.Reserve<CommandBuilder.BoxData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.WireBox);
			this.Add<CommandBuilder.BoxData>(new CommandBuilder.BoxData
			{
				center = center,
				size = size
			});
		}

		public void WireBox(float3 center, quaternion rotation, float3 size)
		{
			this.PushMatrix(float4x4.TRS(center, rotation, size));
			this.WireBox(float3.zero, new float3(1f, 1f, 1f));
			this.PopMatrix();
		}

		public void WireBox(Bounds bounds)
		{
			this.WireBox(bounds.center, bounds.size);
		}

		public void WireMesh(Mesh mesh)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException();
			}
			Mesh.MeshDataArray meshDataArray = Mesh.AcquireReadOnlyMeshData(mesh);
			Mesh.MeshData meshData = meshDataArray[0];
			CommandBuilder.JobWireMesh.JobWireMeshFunctionPointer(ref meshData, ref this);
			meshDataArray.Dispose();
		}

		public unsafe void WireMesh(NativeArray<float3> vertices, NativeArray<int> triangles)
		{
			CommandBuilder.JobWireMesh.WireMesh((float3*)vertices.GetUnsafeReadOnlyPtr<float3>(), (int*)triangles.GetUnsafeReadOnlyPtr<int>(), vertices.Length, triangles.Length, ref this);
		}

		public void SolidMesh(Mesh mesh)
		{
			this.SolidMeshInternal(mesh, false);
		}

		private void SolidMeshInternal(Mesh mesh, bool temporary, Color color)
		{
			this.PushColor(color);
			this.SolidMeshInternal(mesh, temporary);
			this.PopColor();
		}

		private void SolidMeshInternal(Mesh mesh, bool temporary)
		{
			(this.gizmos.Target as DrawingData).data.Get(this.uniqueID).meshes.Add(new DrawingData.SubmittedMesh
			{
				mesh = mesh,
				temporary = temporary
			});
			this.Reserve(4);
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.CaptureState);
		}

		[BurstDiscard]
		public void SolidMesh(List<Vector3> vertices, List<int> triangles, List<Color> colors)
		{
			if (vertices.Count != colors.Count)
			{
				throw new ArgumentException("Number of colors must be the same as the number of vertices");
			}
			Mesh mesh = (this.gizmos.Target as DrawingData).GetMesh(vertices.Count);
			mesh.Clear();
			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles, 0);
			mesh.SetColors(colors);
			mesh.UploadMeshData(false);
			this.SolidMeshInternal(mesh, true);
		}

		[BurstDiscard]
		public void SolidMesh(Vector3[] vertices, int[] triangles, Color[] colors, int vertexCount, int indexCount)
		{
			if (vertices.Length != colors.Length)
			{
				throw new ArgumentException("Number of colors must be the same as the number of vertices");
			}
			Mesh mesh = (this.gizmos.Target as DrawingData).GetMesh(vertices.Length);
			mesh.Clear();
			mesh.SetVertices(vertices, 0, vertexCount);
			mesh.SetTriangles(triangles, 0, indexCount, 0, true, 0);
			mesh.SetColors(colors, 0, vertexCount);
			mesh.UploadMeshData(false);
			this.SolidMeshInternal(mesh, true);
		}

		public void Cross(float3 position, float size = 1f)
		{
			size *= 0.5f;
			this.Line(position - new float3(size, 0f, 0f), position + new float3(size, 0f, 0f));
			this.Line(position - new float3(0f, size, 0f), position + new float3(0f, size, 0f));
			this.Line(position - new float3(0f, 0f, size), position + new float3(0f, 0f, size));
		}

		[Obsolete("Use Draw.xz.Cross instead")]
		public void CrossXZ(float3 position, float size = 1f)
		{
			size *= 0.5f;
			this.Line(position - new float3(size, 0f, 0f), position + new float3(size, 0f, 0f));
			this.Line(position - new float3(0f, 0f, size), position + new float3(0f, 0f, size));
		}

		[Obsolete("Use Draw.xy.Cross instead")]
		public void CrossXY(float3 position, float size = 1f)
		{
			size *= 0.5f;
			this.Line(position - new float3(size, 0f, 0f), position + new float3(size, 0f, 0f));
			this.Line(position - new float3(0f, size, 0f), position + new float3(0f, size, 0f));
		}

		public static float3 EvaluateCubicBezier(float3 p0, float3 p1, float3 p2, float3 p3, float t)
		{
			t = math.clamp(t, 0f, 1f);
			float num = 1f - t;
			return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
		}

		public void Bezier(float3 p0, float3 p1, float3 p2, float3 p3)
		{
			float3 a = p0;
			for (int i = 1; i <= 20; i++)
			{
				float t = (float)i / 20f;
				float3 @float = CommandBuilder.EvaluateCubicBezier(p0, p1, p2, p3, t);
				this.Line(a, @float);
				a = @float;
			}
		}

		public void CatmullRom(List<Vector3> points)
		{
			if (points.Count < 2)
			{
				return;
			}
			if (points.Count == 2)
			{
				this.Line(points[0], points[1]);
				return;
			}
			int count = points.Count;
			this.CatmullRom(points[0], points[0], points[1], points[2]);
			int num = 0;
			while (num + 3 < count)
			{
				this.CatmullRom(points[num], points[num + 1], points[num + 2], points[num + 3]);
				num++;
			}
			this.CatmullRom(points[count - 3], points[count - 2], points[count - 1], points[count - 1]);
		}

		public void CatmullRom(float3 p0, float3 p1, float3 p2, float3 p3)
		{
			float3 p4 = (-p0 + 6f * p1 + 1f * p2) * 0.16666667f;
			float3 p5 = (p1 + 6f * p2 - p3) * 0.16666667f;
			this.Bezier(p1, p4, p5, p2);
		}

		public void Arrow(float3 from, float3 to)
		{
			this.ArrowRelativeSizeHead(from, to, CommandBuilder.DEFAULT_UP, 0.2f);
		}

		public void Arrow(float3 from, float3 to, float3 up, float headSize)
		{
			float num = math.lengthsq(to - from);
			if (num > 1E-06f)
			{
				this.ArrowRelativeSizeHead(from, to, up, headSize * math.rsqrt(num));
			}
		}

		public void ArrowRelativeSizeHead(float3 from, float3 to, float3 up, float headFraction)
		{
			this.Line(from, to);
			float3 @float = to - from;
			float3 float2 = math.cross(@float, up);
			if (math.all(float2 == 0f))
			{
				float2 = math.cross(new float3(1f, 0f, 0f), @float);
			}
			if (math.all(float2 == 0f))
			{
				float2 = math.cross(new float3(0f, 1f, 0f), @float);
			}
			float2 = math.normalizesafe(float2, default(float3)) * math.length(@float);
			this.Line(to, to - (@float + float2) * headFraction);
			this.Line(to, to - (@float - float2) * headFraction);
		}

		public void Arrowhead(float3 center, float3 direction, float radius)
		{
			this.Arrowhead(center, direction, CommandBuilder.DEFAULT_UP, radius);
		}

		public void Arrowhead(float3 center, float3 direction, float3 up, float radius)
		{
			if (math.all(direction == 0f))
			{
				return;
			}
			direction = math.normalizesafe(direction, default(float3));
			float3 rhs = math.cross(direction, up);
			float3 @float = center - radius * 0.5f * 0.5f * direction;
			float3 float2 = @float + radius * direction;
			float3 float3 = @float - radius * 0.5f * direction + radius * 0.866025f * rhs;
			float3 float4 = @float - radius * 0.5f * direction - radius * 0.866025f * rhs;
			this.Line(float2, float3);
			this.Line(float3, @float);
			this.Line(@float, float4);
			this.Line(float4, float2);
		}

		public void ArrowheadArc(float3 origin, float3 direction, float offset, float width = 60f)
		{
			if (!math.any(direction))
			{
				return;
			}
			if (offset < 0f)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset == 0f)
			{
				return;
			}
			Quaternion q = Quaternion.LookRotation(direction, CommandBuilder.DEFAULT_UP);
			this.PushMatrix(Matrix4x4.TRS(origin, q, Vector3.one));
			float num = 1.5707964f - width * 0.008726646f;
			float num2 = 1.5707964f + width * 0.008726646f;
			this.CircleXZInternal(float3.zero, offset, num, num2);
			float3 a = new float3(math.cos(num), 0f, math.sin(num)) * offset;
			float3 b = new float3(math.cos(num2), 0f, math.sin(num2)) * offset;
			float3 @float = new float3(0f, 0f, 1.4142f * offset);
			this.Line(a, @float);
			this.Line(@float, b);
			this.PopMatrix();
		}

		public void WireGrid(float3 center, quaternion rotation, int2 cells, float2 totalSize)
		{
			cells = math.max(cells, new int2(1, 1));
			this.PushMatrix(float4x4.TRS(center, rotation, new Vector3(totalSize.x, 0f, totalSize.y)));
			int x = cells.x;
			int y = cells.y;
			for (int i = 0; i <= x; i++)
			{
				this.Line(new float3((float)i / (float)x - 0.5f, 0f, -0.5f), new float3((float)i / (float)x - 0.5f, 0f, 0.5f));
			}
			for (int j = 0; j <= y; j++)
			{
				this.Line(new float3(-0.5f, 0f, (float)j / (float)y - 0.5f), new float3(0.5f, 0f, (float)j / (float)y - 0.5f));
			}
			this.PopMatrix();
		}

		public void WireTriangle(float3 a, float3 b, float3 c)
		{
			this.Line(a, b);
			this.Line(b, c);
			this.Line(c, a);
		}

		[Obsolete("Use Draw.xz.WireRectangle instead")]
		public void WireRectangleXZ(float3 center, float2 size)
		{
			this.WireRectangle(center, quaternion.identity, size);
		}

		public void WireRectangle(float3 center, quaternion rotation, float2 size)
		{
			this.WirePlane(center, rotation, size);
		}

		[Obsolete("Use Draw.xy.WireRectangle instead")]
		public void WireRectangle(Rect rect)
		{
			this.xy.WireRectangle(rect);
		}

		public void WireTriangle(float3 center, quaternion rotation, float radius)
		{
			this.WirePolygon(center, 3, rotation, radius);
		}

		public void WirePentagon(float3 center, quaternion rotation, float radius)
		{
			this.WirePolygon(center, 5, rotation, radius);
		}

		public void WireHexagon(float3 center, quaternion rotation, float radius)
		{
			this.WirePolygon(center, 6, rotation, radius);
		}

		public void WirePolygon(float3 center, int vertices, quaternion rotation, float radius)
		{
			this.PushMatrix(float4x4.TRS(center, rotation, new float3(radius, radius, radius)));
			float3 a = new float3(0f, 0f, 1f);
			for (int i = 1; i <= vertices; i++)
			{
				float x = 6.2831855f * ((float)i / (float)vertices);
				float3 @float = new float3(math.sin(x), 0f, math.cos(x));
				this.Line(a, @float);
				a = @float;
			}
			this.PopMatrix();
		}

		[Obsolete("Use Draw.xy.SolidRectangle instead")]
		public void SolidRectangle(Rect rect)
		{
			this.xy.SolidRectangle(rect);
		}

		public void SolidPlane(float3 center, float3 normal, float2 size)
		{
			if (math.any(normal))
			{
				this.SolidPlane(center, Quaternion.LookRotation(CommandBuilder.calculateTangent(normal), normal), size);
			}
		}

		public void SolidPlane(float3 center, quaternion rotation, float2 size)
		{
			this.PushMatrix(float4x4.TRS(center, rotation, new float3(size.x, 0f, size.y)));
			this.Reserve<CommandBuilder.BoxData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Box);
			this.Add<CommandBuilder.BoxData>(new CommandBuilder.BoxData
			{
				center = 0,
				size = 1
			});
			this.PopMatrix();
		}

		private static float3 calculateTangent(float3 normal)
		{
			float3 @float = math.cross(new float3(0f, 1f, 0f), normal);
			if (math.all(@float == 0f))
			{
				@float = math.cross(new float3(1f, 0f, 0f), normal);
			}
			return @float;
		}

		public void WirePlane(float3 center, float3 normal, float2 size)
		{
			if (math.any(normal))
			{
				this.WirePlane(center, Quaternion.LookRotation(CommandBuilder.calculateTangent(normal), normal), size);
			}
		}

		public void WirePlane(float3 center, quaternion rotation, float2 size)
		{
			this.Reserve<CommandBuilder.PlaneData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.WirePlane);
			this.Add<CommandBuilder.PlaneData>(new CommandBuilder.PlaneData
			{
				center = center,
				rotation = rotation,
				size = size
			});
		}

		public void PlaneWithNormal(float3 center, float3 normal, float2 size)
		{
			if (math.any(normal))
			{
				this.PlaneWithNormal(center, Quaternion.LookRotation(CommandBuilder.calculateTangent(normal), normal), size);
			}
		}

		public void PlaneWithNormal(float3 center, quaternion rotation, float2 size)
		{
			this.SolidPlane(center, rotation, size);
			this.WirePlane(center, rotation, size);
			this.ArrowRelativeSizeHead(center, center + math.mul(rotation, new float3(0f, 1f, 0f)) * 0.5f, math.mul(rotation, new float3(0f, 0f, 1f)), 0.2f);
		}

		public void SolidTriangle(float3 a, float3 b, float3 c)
		{
			this.Reserve<CommandBuilder.TriangleData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.SolidTriangle);
			this.Add<CommandBuilder.TriangleData>(new CommandBuilder.TriangleData
			{
				a = a,
				b = b,
				c = c
			});
		}

		public void SolidBox(float3 center, float3 size)
		{
			this.Reserve<CommandBuilder.BoxData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Box);
			this.Add<CommandBuilder.BoxData>(new CommandBuilder.BoxData
			{
				center = center,
				size = size
			});
		}

		public void SolidBox(Bounds bounds)
		{
			this.SolidBox(bounds.center, bounds.size);
		}

		public void SolidBox(float3 center, quaternion rotation, float3 size)
		{
			this.PushMatrix(float4x4.TRS(center, rotation, size));
			this.SolidBox(float3.zero, Vector3.one);
			this.PopMatrix();
		}

		public void Label3D(float3 position, quaternion rotation, string text, float size)
		{
			this.Label3D(position, rotation, text, size, LabelAlignment.MiddleLeft);
		}

		public void Label3D(float3 position, quaternion rotation, string text, float size, LabelAlignment alignment)
		{
			this.AssertBufferExists();
			DrawingData drawingData = this.gizmos.Target as DrawingData;
			this.Reserve<CommandBuilder.TextData3D>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Text3D);
			this.Add<CommandBuilder.TextData3D>(new CommandBuilder.TextData3D
			{
				center = position,
				rotation = rotation,
				numCharacters = text.Length,
				size = size,
				alignment = alignment
			});
			this.Reserve(UnsafeUtility.SizeOf<ushort>() * text.Length);
			foreach (char c in text)
			{
				ushort value = (ushort)drawingData.fontData.GetIndex(c);
				this.Add<ushort>(value);
			}
		}

		public void Label2D(float3 position, string text, float sizeInPixels = 14f)
		{
			this.Label2D(position, text, sizeInPixels, LabelAlignment.MiddleLeft);
		}

		public void Label2D(float3 position, string text, float sizeInPixels, LabelAlignment alignment)
		{
			this.AssertBufferExists();
			DrawingData drawingData = this.gizmos.Target as DrawingData;
			this.Reserve<CommandBuilder.TextData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Text);
			this.Add<CommandBuilder.TextData>(new CommandBuilder.TextData
			{
				center = position,
				numCharacters = text.Length,
				sizeInPixels = sizeInPixels,
				alignment = alignment
			});
			this.Reserve(UnsafeUtility.SizeOf<ushort>() * text.Length);
			foreach (char c in text)
			{
				ushort value = (ushort)drawingData.fontData.GetIndex(c);
				this.Add<ushort>(value);
			}
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels = 14f)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels, LabelAlignment alignment)
		{
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
		}

		internal unsafe void Label2D(float3 position, byte* text, int byteCount, float sizeInPixels, LabelAlignment alignment)
		{
			this.AssertBufferExists();
			this.Reserve<CommandBuilder.TextData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Text);
			this.Add<CommandBuilder.TextData>(new CommandBuilder.TextData
			{
				center = position,
				numCharacters = byteCount,
				sizeInPixels = sizeInPixels,
				alignment = alignment
			});
			this.Reserve(UnsafeUtility.SizeOf<ushort>() * byteCount);
			for (int i = 0; i < byteCount; i++)
			{
				ushort num = (ushort)text[i];
				if (num >= 128)
				{
					num = 63;
				}
				if (num == 10)
				{
					num = ushort.MaxValue;
				}
				if (num != 13)
				{
					this.Add<ushort>(num);
				}
			}
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString32Bytes text, float size)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString64Bytes text, float size)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString128Bytes text, float size)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString512Bytes text, float size)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString32Bytes text, float size, LabelAlignment alignment)
		{
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString64Bytes text, float size, LabelAlignment alignment)
		{
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString128Bytes text, float size, LabelAlignment alignment)
		{
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString512Bytes text, float size, LabelAlignment alignment)
		{
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
		}

		internal unsafe void Label3D(float3 position, quaternion rotation, byte* text, int byteCount, float size, LabelAlignment alignment)
		{
			this.AssertBufferExists();
			this.Reserve<CommandBuilder.TextData3D>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.Text3D);
			this.Add<CommandBuilder.TextData3D>(new CommandBuilder.TextData3D
			{
				center = position,
				rotation = rotation,
				numCharacters = byteCount,
				size = size,
				alignment = alignment
			});
			this.Reserve(UnsafeUtility.SizeOf<ushort>() * byteCount);
			for (int i = 0; i < byteCount; i++)
			{
				ushort num = (ushort)text[i];
				if (num >= 128)
				{
					num = 63;
				}
				if (num == 10)
				{
					num = ushort.MaxValue;
				}
				this.Add<ushort>(num);
			}
		}

		public void Line(float3 a, float3 b, Color color)
		{
			this.Reserve<Color32, CommandBuilder.LineData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.PopMatrix);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.LineData>(new CommandBuilder.LineData
			{
				a = a,
				b = b
			});
		}

		public void Ray(float3 origin, float3 direction, Color color)
		{
			this.Line(origin, origin + direction, color);
		}

		public void Ray(Ray ray, float length, Color color)
		{
			this.Line(ray.origin, ray.origin + ray.direction * length, color);
		}

		public void Arc(float3 center, float3 start, float3 end, Color color)
		{
			this.PushColor(color);
			float3 @float = start - center;
			float3 float2 = end - center;
			float3 float3 = math.cross(float2, @float);
			if (math.any(float3 != 0f) && math.all(math.isfinite(float3)))
			{
				Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(@float, float3), Vector3.one);
				float num = Vector3.SignedAngle(@float, float2, float3) * 0.017453292f;
				this.PushMatrix(matrix);
				this.CircleXZInternal(float3.zero, math.length(@float), 1.5707964f, 1.5707964f - num);
				this.PopMatrix();
			}
			this.PopColor();
		}

		[Obsolete("Use Draw.xz.Circle instead")]
		public void CircleXZ(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.CircleXZInternal(center, radius, startAngle, endAngle, color);
		}

		[Obsolete("Use Draw.xz.Circle instead")]
		public void CircleXZ(float3 center, float radius, Color color)
		{
			this.CircleXZ(center, radius, 0f, 6.2831855f, color);
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.PushColor(color);
			this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
			this.CircleXZ(new float3(center.x, -center.z, center.y), radius, startAngle, endAngle);
			this.PopMatrix();
			this.PopColor();
		}

		[Obsolete("Use Draw.xy.Circle instead")]
		public void CircleXY(float3 center, float radius, Color color)
		{
			this.CircleXY(center, radius, 0f, 6.2831855f, color);
		}

		public void Circle(float3 center, float3 normal, float radius, Color color)
		{
			this.Reserve<Color32, CommandBuilder.CircleData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.PopMatrix);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.CircleData>(new CommandBuilder.CircleData
			{
				center = center,
				normal = normal,
				radius = radius
			});
		}

		public void SolidArc(float3 center, float3 start, float3 end, Color color)
		{
			this.PushColor(color);
			float3 @float = start - center;
			float3 float2 = end - center;
			float3 float3 = math.cross(float2, @float);
			if (math.any(float3))
			{
				Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(@float, float3), Vector3.one);
				float num = Vector3.SignedAngle(@float, float2, float3) * 0.017453292f;
				this.PushMatrix(matrix);
				this.SolidCircleXZInternal(float3.zero, math.length(@float), 1.5707964f, 1.5707964f - num);
				this.PopMatrix();
			}
			this.PopColor();
		}

		[Obsolete("Use Draw.xz.SolidCircle instead")]
		public void SolidCircleXZ(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.SolidCircleXZInternal(center, radius, startAngle, endAngle, color);
		}

		[Obsolete("Use Draw.xz.SolidCircle instead")]
		public void SolidCircleXZ(float3 center, float radius, Color color)
		{
			this.SolidCircleXZ(center, radius, 0f, 6.2831855f, color);
		}

		[Obsolete("Use Draw.xy.SolidCircle instead")]
		public void SolidCircleXY(float3 center, float radius, float startAngle, float endAngle, Color color)
		{
			this.PushColor(color);
			this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
			this.SolidCircleXZInternal(new float3(center.x, -center.z, center.y), radius, startAngle, endAngle);
			this.PopMatrix();
			this.PopColor();
		}

		[Obsolete("Use Draw.xy.SolidCircle instead")]
		public void SolidCircleXY(float3 center, float radius, Color color)
		{
			this.SolidCircleXY(center, radius, 0f, 6.2831855f, color);
		}

		public void SolidCircle(float3 center, float3 normal, float radius, Color color)
		{
			this.Reserve<Color32, CommandBuilder.CircleData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.CircleData>(new CommandBuilder.CircleData
			{
				center = center,
				normal = normal,
				radius = radius
			});
		}

		public void SphereOutline(float3 center, float radius, Color color)
		{
			this.Reserve<Color32, CommandBuilder.SphereData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.SphereData>(new CommandBuilder.SphereData
			{
				center = center,
				radius = radius
			});
		}

		public void WireCylinder(float3 bottom, float3 top, float radius, Color color)
		{
			this.WireCylinder(bottom, top - bottom, math.length(top - bottom), radius, color);
		}

		public void WireCylinder(float3 position, float3 up, float height, float radius, Color color)
		{
			up = math.normalizesafe(up, default(float3));
			if (math.all(up == 0f) || math.any(math.isnan(up)) || math.isnan(height) || math.isnan(radius))
			{
				return;
			}
			this.PushColor(color);
			float3 lhs;
			float3 lhs2;
			CommandBuilder.OrthonormalBasis(up, out lhs, out lhs2);
			this.PushMatrix(new float4x4(new float4(lhs * radius, 0f), new float4(up * height, 0f), new float4(lhs2 * radius, 0f), new float4(position, 1f)));
			this.CircleXZInternal(float3.zero, 1f, 0f, 6.2831855f);
			if (height > 0f)
			{
				this.CircleXZInternal(new float3(0f, 1f, 0f), 1f, 0f, 6.2831855f);
				this.Line(new float3(1f, 0f, 0f), new float3(1f, 1f, 0f));
				this.Line(new float3(-1f, 0f, 0f), new float3(-1f, 1f, 0f));
				this.Line(new float3(0f, 0f, 1f), new float3(0f, 1f, 1f));
				this.Line(new float3(0f, 0f, -1f), new float3(0f, 1f, -1f));
			}
			this.PopMatrix();
			this.PopColor();
		}

		public void WireCapsule(float3 start, float3 end, float radius, Color color)
		{
			this.PushColor(color);
			float3 @float = end - start;
			float num = math.length(@float);
			if ((double)num < 0.0001)
			{
				this.WireSphere(start, radius);
			}
			else
			{
				float3 float2 = @float / num;
				this.WireCapsule(start - float2 * radius, float2, num + 2f * radius, radius);
			}
			this.PopColor();
		}

		public void WireCapsule(float3 position, float3 direction, float length, float radius, Color color)
		{
			direction = math.normalizesafe(direction, default(float3));
			if (math.all(direction == 0f) || math.any(math.isnan(direction)) || math.isnan(length) || math.isnan(radius))
			{
				return;
			}
			this.PushColor(color);
			if (radius <= 0f)
			{
				this.Line(position, position + direction * length);
			}
			else
			{
				length = math.max(length, radius * 2f);
				float3 xyz;
				float3 xyz2;
				CommandBuilder.OrthonormalBasis(direction, out xyz, out xyz2);
				this.PushMatrix(new float4x4(new float4(xyz, 0f), new float4(direction, 0f), new float4(xyz2, 0f), new float4(position, 1f)));
				this.CircleXZInternal(new float3(0f, radius, 0f), radius, 0f, 6.2831855f);
				this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
				this.CircleXZInternal(new float3(0f, 0f, radius), radius, 3.1415927f, 6.2831855f);
				this.PopMatrix();
				this.PushMatrix(CommandBuilder.XZtoYZPlaneMatrix);
				this.CircleXZInternal(new float3(radius, 0f, 0f), radius, 1.5707964f, 4.712389f);
				this.PopMatrix();
				if (length > 0f)
				{
					float num = length - radius;
					this.CircleXZInternal(new float3(0f, num, 0f), radius, 0f, 6.2831855f);
					this.PushMatrix(CommandBuilder.XZtoXYPlaneMatrix);
					this.CircleXZInternal(new float3(0f, 0f, num), radius, 0f, 3.1415927f);
					this.PopMatrix();
					this.PushMatrix(CommandBuilder.XZtoYZPlaneMatrix);
					this.CircleXZInternal(new float3(num, 0f, 0f), radius, -1.5707964f, 1.5707964f);
					this.PopMatrix();
					this.Line(new float3(radius, radius, 0f), new float3(radius, num, 0f));
					this.Line(new float3(-radius, radius, 0f), new float3(-radius, num, 0f));
					this.Line(new float3(0f, radius, radius), new float3(0f, num, radius));
					this.Line(new float3(0f, radius, -radius), new float3(0f, num, -radius));
				}
				this.PopMatrix();
			}
			this.PopColor();
		}

		public void WireSphere(float3 position, float radius, Color color)
		{
			this.PushColor(color);
			this.SphereOutline(position, radius);
			this.Circle(position, new float3(1f, 0f, 0f), radius);
			this.Circle(position, new float3(0f, 1f, 0f), radius);
			this.Circle(position, new float3(0f, 0f, 1f), radius);
			this.PopColor();
		}

		[BurstDiscard]
		public void Polyline(List<Vector3> points, bool cycle, Color color)
		{
			this.PushColor(color);
			for (int i = 0; i < points.Count - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Count > 1)
			{
				this.Line(points[points.Count - 1], points[0]);
			}
			this.PopColor();
		}

		[BurstDiscard]
		public void Polyline(List<Vector3> points, Color color)
		{
			this.Polyline(points, false, color);
		}

		[BurstDiscard]
		public void Polyline(Vector3[] points, bool cycle, Color color)
		{
			this.PushColor(color);
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
			this.PopColor();
		}

		[BurstDiscard]
		public void Polyline(Vector3[] points, Color color)
		{
			this.Polyline(points, false, color);
		}

		[BurstDiscard]
		public void Polyline(float3[] points, bool cycle, Color color)
		{
			this.PushColor(color);
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
			this.PopColor();
		}

		[BurstDiscard]
		public void Polyline(float3[] points, Color color)
		{
			this.Polyline(points, false, color);
		}

		public void Polyline(NativeArray<float3> points, bool cycle, Color color)
		{
			this.PushColor(color);
			for (int i = 0; i < points.Length - 1; i++)
			{
				this.Line(points[i], points[i + 1]);
			}
			if (cycle && points.Length > 1)
			{
				this.Line(points[points.Length - 1], points[0]);
			}
			this.PopColor();
		}

		public void Polyline(NativeArray<float3> points, Color color)
		{
			this.Polyline(points, false, color);
		}

		public void DashedLine(float3 a, float3 b, float dash, float gap, Color color)
		{
			this.PushColor(color);
			CommandBuilder.PolylineWithSymbol polylineWithSymbol = new CommandBuilder.PolylineWithSymbol(CommandBuilder.SymbolDecoration.None, gap, 0f, dash + gap, false);
			polylineWithSymbol.MoveTo(ref this, a);
			polylineWithSymbol.MoveTo(ref this, b);
			this.PopColor();
		}

		public void DashedPolyline(List<Vector3> points, float dash, float gap, Color color)
		{
			this.PushColor(color);
			CommandBuilder.PolylineWithSymbol polylineWithSymbol = new CommandBuilder.PolylineWithSymbol(CommandBuilder.SymbolDecoration.None, gap, 0f, dash + gap, false);
			for (int i = 0; i < points.Count; i++)
			{
				polylineWithSymbol.MoveTo(ref this, points[i]);
			}
			this.PopColor();
		}

		public void WireBox(float3 center, float3 size, Color color)
		{
			this.Reserve<Color32, CommandBuilder.BoxData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.PopMatrix | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.BoxData>(new CommandBuilder.BoxData
			{
				center = center,
				size = size
			});
		}

		public void WireBox(float3 center, quaternion rotation, float3 size, Color color)
		{
			this.PushColor(color);
			this.PushMatrix(float4x4.TRS(center, rotation, size));
			this.WireBox(float3.zero, new float3(1f, 1f, 1f));
			this.PopMatrix();
			this.PopColor();
		}

		public void WireBox(Bounds bounds, Color color)
		{
			this.WireBox(bounds.center, bounds.size, color);
		}

		public void WireMesh(Mesh mesh, Color color)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException();
			}
			this.PushColor(color);
			Mesh.MeshDataArray meshDataArray = Mesh.AcquireReadOnlyMeshData(mesh);
			Mesh.MeshData meshData = meshDataArray[0];
			CommandBuilder.JobWireMesh.JobWireMeshFunctionPointer(ref meshData, ref this);
			meshDataArray.Dispose();
			this.PopColor();
		}

		public unsafe void WireMesh(NativeArray<float3> vertices, NativeArray<int> triangles, Color color)
		{
			this.PushColor(color);
			CommandBuilder.JobWireMesh.WireMesh((float3*)vertices.GetUnsafeReadOnlyPtr<float3>(), (int*)triangles.GetUnsafeReadOnlyPtr<int>(), vertices.Length, triangles.Length, ref this);
			this.PopColor();
		}

		public void SolidMesh(Mesh mesh, Color color)
		{
			this.SolidMeshInternal(mesh, false, color);
		}

		public void Cross(float3 position, float size, Color color)
		{
			this.PushColor(color);
			size *= 0.5f;
			this.Line(position - new float3(size, 0f, 0f), position + new float3(size, 0f, 0f));
			this.Line(position - new float3(0f, size, 0f), position + new float3(0f, size, 0f));
			this.Line(position - new float3(0f, 0f, size), position + new float3(0f, 0f, size));
			this.PopColor();
		}

		public void Cross(float3 position, Color color)
		{
			this.Cross(position, 1f, color);
		}

		[Obsolete("Use Draw.xz.Cross instead")]
		public void CrossXZ(float3 position, float size, Color color)
		{
			this.PushColor(color);
			size *= 0.5f;
			this.Line(position - new float3(size, 0f, 0f), position + new float3(size, 0f, 0f));
			this.Line(position - new float3(0f, 0f, size), position + new float3(0f, 0f, size));
			this.PopColor();
		}

		[Obsolete("Use Draw.xz.Cross instead")]
		public void CrossXZ(float3 position, Color color)
		{
			this.CrossXZ(position, 1f, color);
		}

		[Obsolete("Use Draw.xy.Cross instead")]
		public void CrossXY(float3 position, float size, Color color)
		{
			this.PushColor(color);
			size *= 0.5f;
			this.Line(position - new float3(size, 0f, 0f), position + new float3(size, 0f, 0f));
			this.Line(position - new float3(0f, size, 0f), position + new float3(0f, size, 0f));
			this.PopColor();
		}

		[Obsolete("Use Draw.xy.Cross instead")]
		public void CrossXY(float3 position, Color color)
		{
			this.CrossXY(position, 1f, color);
		}

		public void Bezier(float3 p0, float3 p1, float3 p2, float3 p3, Color color)
		{
			this.PushColor(color);
			float3 a = p0;
			for (int i = 1; i <= 20; i++)
			{
				float t = (float)i / 20f;
				float3 @float = CommandBuilder.EvaluateCubicBezier(p0, p1, p2, p3, t);
				this.Line(a, @float);
				a = @float;
			}
			this.PopColor();
		}

		public void CatmullRom(List<Vector3> points, Color color)
		{
			if (points.Count < 2)
			{
				return;
			}
			this.PushColor(color);
			if (points.Count == 2)
			{
				this.Line(points[0], points[1]);
			}
			else
			{
				int count = points.Count;
				this.CatmullRom(points[0], points[0], points[1], points[2]);
				int num = 0;
				while (num + 3 < count)
				{
					this.CatmullRom(points[num], points[num + 1], points[num + 2], points[num + 3]);
					num++;
				}
				this.CatmullRom(points[count - 3], points[count - 2], points[count - 1], points[count - 1]);
			}
			this.PopColor();
		}

		public void CatmullRom(float3 p0, float3 p1, float3 p2, float3 p3, Color color)
		{
			this.PushColor(color);
			float3 p4 = (-p0 + 6f * p1 + 1f * p2) * 0.16666667f;
			float3 p5 = (p1 + 6f * p2 - p3) * 0.16666667f;
			this.Bezier(p1, p4, p5, p2);
			this.PopColor();
		}

		public void Arrow(float3 from, float3 to, Color color)
		{
			this.ArrowRelativeSizeHead(from, to, CommandBuilder.DEFAULT_UP, 0.2f, color);
		}

		public void Arrow(float3 from, float3 to, float3 up, float headSize, Color color)
		{
			this.PushColor(color);
			float num = math.lengthsq(to - from);
			if (num > 1E-06f)
			{
				this.ArrowRelativeSizeHead(from, to, up, headSize * math.rsqrt(num));
			}
			this.PopColor();
		}

		public void ArrowRelativeSizeHead(float3 from, float3 to, float3 up, float headFraction, Color color)
		{
			this.PushColor(color);
			this.Line(from, to);
			float3 @float = to - from;
			float3 float2 = math.cross(@float, up);
			if (math.all(float2 == 0f))
			{
				float2 = math.cross(new float3(1f, 0f, 0f), @float);
			}
			if (math.all(float2 == 0f))
			{
				float2 = math.cross(new float3(0f, 1f, 0f), @float);
			}
			float2 = math.normalizesafe(float2, default(float3)) * math.length(@float);
			this.Line(to, to - (@float + float2) * headFraction);
			this.Line(to, to - (@float - float2) * headFraction);
			this.PopColor();
		}

		public void Arrowhead(float3 center, float3 direction, float radius, Color color)
		{
			this.Arrowhead(center, direction, CommandBuilder.DEFAULT_UP, radius, color);
		}

		public void Arrowhead(float3 center, float3 direction, float3 up, float radius, Color color)
		{
			if (math.all(direction == 0f))
			{
				return;
			}
			this.PushColor(color);
			direction = math.normalizesafe(direction, default(float3));
			float3 rhs = math.cross(direction, up);
			float3 @float = center - radius * 0.5f * 0.5f * direction;
			float3 float2 = @float + radius * direction;
			float3 float3 = @float - radius * 0.5f * direction + radius * 0.866025f * rhs;
			float3 float4 = @float - radius * 0.5f * direction - radius * 0.866025f * rhs;
			this.Line(float2, float3);
			this.Line(float3, @float);
			this.Line(@float, float4);
			this.Line(float4, float2);
			this.PopColor();
		}

		public void ArrowheadArc(float3 origin, float3 direction, float offset, float width, Color color)
		{
			if (!math.any(direction))
			{
				return;
			}
			if (offset < 0f)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset == 0f)
			{
				return;
			}
			this.PushColor(color);
			Quaternion q = Quaternion.LookRotation(direction, CommandBuilder.DEFAULT_UP);
			this.PushMatrix(Matrix4x4.TRS(origin, q, Vector3.one));
			float num = 1.5707964f - width * 0.008726646f;
			float num2 = 1.5707964f + width * 0.008726646f;
			this.CircleXZInternal(float3.zero, offset, num, num2);
			float3 a = new float3(math.cos(num), 0f, math.sin(num)) * offset;
			float3 b = new float3(math.cos(num2), 0f, math.sin(num2)) * offset;
			float3 @float = new float3(0f, 0f, 1.4142f * offset);
			this.Line(a, @float);
			this.Line(@float, b);
			this.PopMatrix();
			this.PopColor();
		}

		public void ArrowheadArc(float3 origin, float3 direction, float offset, Color color)
		{
			this.ArrowheadArc(origin, direction, offset, 60f, color);
		}

		public void WireGrid(float3 center, quaternion rotation, int2 cells, float2 totalSize, Color color)
		{
			this.PushColor(color);
			cells = math.max(cells, new int2(1, 1));
			this.PushMatrix(float4x4.TRS(center, rotation, new Vector3(totalSize.x, 0f, totalSize.y)));
			int x = cells.x;
			int y = cells.y;
			for (int i = 0; i <= x; i++)
			{
				this.Line(new float3((float)i / (float)x - 0.5f, 0f, -0.5f), new float3((float)i / (float)x - 0.5f, 0f, 0.5f));
			}
			for (int j = 0; j <= y; j++)
			{
				this.Line(new float3(-0.5f, 0f, (float)j / (float)y - 0.5f), new float3(0.5f, 0f, (float)j / (float)y - 0.5f));
			}
			this.PopMatrix();
			this.PopColor();
		}

		public void WireTriangle(float3 a, float3 b, float3 c, Color color)
		{
			this.PushColor(color);
			this.Line(a, b);
			this.Line(b, c);
			this.Line(c, a);
			this.PopColor();
		}

		[Obsolete("Use Draw.xz.WireRectangle instead")]
		public void WireRectangleXZ(float3 center, float2 size, Color color)
		{
			this.WireRectangle(center, quaternion.identity, size, color);
		}

		public void WireRectangle(float3 center, quaternion rotation, float2 size, Color color)
		{
			this.WirePlane(center, rotation, size, color);
		}

		[Obsolete("Use Draw.xy.WireRectangle instead")]
		public void WireRectangle(Rect rect, Color color)
		{
			this.xy.WireRectangle(rect, color);
		}

		public void WireTriangle(float3 center, quaternion rotation, float radius, Color color)
		{
			this.WirePolygon(center, 3, rotation, radius, color);
		}

		public void WirePentagon(float3 center, quaternion rotation, float radius, Color color)
		{
			this.WirePolygon(center, 5, rotation, radius, color);
		}

		public void WireHexagon(float3 center, quaternion rotation, float radius, Color color)
		{
			this.WirePolygon(center, 6, rotation, radius, color);
		}

		public void WirePolygon(float3 center, int vertices, quaternion rotation, float radius, Color color)
		{
			this.PushColor(color);
			this.PushMatrix(float4x4.TRS(center, rotation, new float3(radius, radius, radius)));
			float3 a = new float3(0f, 0f, 1f);
			for (int i = 1; i <= vertices; i++)
			{
				float x = 6.2831855f * ((float)i / (float)vertices);
				float3 @float = new float3(math.sin(x), 0f, math.cos(x));
				this.Line(a, @float);
				a = @float;
			}
			this.PopMatrix();
			this.PopColor();
		}

		[Obsolete("Use Draw.xy.SolidRectangle instead")]
		public void SolidRectangle(Rect rect, Color color)
		{
			this.xy.SolidRectangle(rect, color);
		}

		public void SolidPlane(float3 center, float3 normal, float2 size, Color color)
		{
			this.PushColor(color);
			if (math.any(normal))
			{
				this.SolidPlane(center, Quaternion.LookRotation(CommandBuilder.calculateTangent(normal), normal), size);
			}
			this.PopColor();
		}

		public void SolidPlane(float3 center, quaternion rotation, float2 size, Color color)
		{
			this.PushMatrix(float4x4.TRS(center, rotation, new float3(size.x, 0f, size.y)));
			this.Reserve<Color32, CommandBuilder.BoxData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.BoxData>(new CommandBuilder.BoxData
			{
				center = 0,
				size = 1
			});
			this.PopMatrix();
		}

		public void WirePlane(float3 center, float3 normal, float2 size, Color color)
		{
			this.PushColor(color);
			if (math.any(normal))
			{
				this.WirePlane(center, Quaternion.LookRotation(CommandBuilder.calculateTangent(normal), normal), size);
			}
			this.PopColor();
		}

		public void WirePlane(float3 center, quaternion rotation, float2 size, Color color)
		{
			this.Reserve<Color32, CommandBuilder.PlaneData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopMatrix | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.PlaneData>(new CommandBuilder.PlaneData
			{
				center = center,
				rotation = rotation,
				size = size
			});
		}

		public void PlaneWithNormal(float3 center, float3 normal, float2 size, Color color)
		{
			this.PushColor(color);
			if (math.any(normal))
			{
				this.PlaneWithNormal(center, Quaternion.LookRotation(CommandBuilder.calculateTangent(normal), normal), size);
			}
			this.PopColor();
		}

		public void PlaneWithNormal(float3 center, quaternion rotation, float2 size, Color color)
		{
			this.PushColor(color);
			this.SolidPlane(center, rotation, size);
			this.WirePlane(center, rotation, size);
			this.ArrowRelativeSizeHead(center, center + math.mul(rotation, new float3(0f, 1f, 0f)) * 0.5f, math.mul(rotation, new float3(0f, 0f, 1f)), 0.2f);
			this.PopColor();
		}

		public void SolidTriangle(float3 a, float3 b, float3 c, Color color)
		{
			this.Reserve<Color32, CommandBuilder.TriangleData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.PopMatrix | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.TriangleData>(new CommandBuilder.TriangleData
			{
				a = a,
				b = b,
				c = c
			});
		}

		public void SolidBox(float3 center, float3 size, Color color)
		{
			this.Reserve<Color32, CommandBuilder.BoxData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.Disc);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.BoxData>(new CommandBuilder.BoxData
			{
				center = center,
				size = size
			});
		}

		public void SolidBox(Bounds bounds, Color color)
		{
			this.SolidBox(bounds.center, bounds.size, color);
		}

		public void SolidBox(float3 center, quaternion rotation, float3 size, Color color)
		{
			this.PushColor(color);
			this.PushMatrix(float4x4.TRS(center, rotation, size));
			this.SolidBox(float3.zero, Vector3.one);
			this.PopMatrix();
			this.PopColor();
		}

		public void Label3D(float3 position, quaternion rotation, string text, float size, Color color)
		{
			this.Label3D(position, rotation, text, size, LabelAlignment.MiddleLeft, color);
		}

		public void Label3D(float3 position, quaternion rotation, string text, float size, LabelAlignment alignment, Color color)
		{
			this.AssertBufferExists();
			DrawingData drawingData = this.gizmos.Target as DrawingData;
			this.Reserve<Color32, CommandBuilder.TextData3D>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PushMatrix | CommandBuilder.Command.PopPersist);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.TextData3D>(new CommandBuilder.TextData3D
			{
				center = position,
				rotation = rotation,
				numCharacters = text.Length,
				size = size,
				alignment = alignment
			});
			this.Reserve(UnsafeUtility.SizeOf<ushort>() * text.Length);
			foreach (char c in text)
			{
				ushort value = (ushort)drawingData.fontData.GetIndex(c);
				this.Add<ushort>(value);
			}
		}

		public void Label2D(float3 position, string text, float sizeInPixels, Color color)
		{
			this.Label2D(position, text, sizeInPixels, LabelAlignment.MiddleLeft, color);
		}

		public void Label2D(float3 position, string text, Color color)
		{
			this.Label2D(position, text, 14f, color);
		}

		public void Label2D(float3 position, string text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.AssertBufferExists();
			DrawingData drawingData = this.gizmos.Target as DrawingData;
			this.Reserve<Color32, CommandBuilder.TextData>();
			this.Add<CommandBuilder.Command>(CommandBuilder.Command.PushColorInline | CommandBuilder.Command.PopColor | CommandBuilder.Command.PopPersist);
			this.Add<uint>(CommandBuilder.ConvertColor(color));
			this.Add<CommandBuilder.TextData>(new CommandBuilder.TextData
			{
				center = position,
				numCharacters = text.Length,
				sizeInPixels = sizeInPixels,
				alignment = alignment
			});
			this.Reserve(UnsafeUtility.SizeOf<ushort>() * text.Length);
			foreach (char c in text)
			{
				ushort value = (ushort)drawingData.fontData.GetIndex(c);
				this.Add<ushort>(value);
			}
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft, color);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft, color);
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft, color);
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels, Color color)
		{
			this.Label2D(position, ref text, sizeInPixels, LabelAlignment.MiddleLeft, color);
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, Color color)
		{
			this.Label2D(position, ref text, 14f, color);
		}

		public void Label2D(float3 position, ref FixedString32Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
			this.PopColor();
		}

		public void Label2D(float3 position, ref FixedString64Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
			this.PopColor();
		}

		public void Label2D(float3 position, ref FixedString128Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
			this.PopColor();
		}

		public void Label2D(float3 position, ref FixedString512Bytes text, float sizeInPixels, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label2D(position, text.GetUnsafePtr(), text.Length, sizeInPixels, alignment);
			this.PopColor();
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString32Bytes text, float size, Color color)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft, color);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString64Bytes text, float size, Color color)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft, color);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString128Bytes text, float size, Color color)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft, color);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString512Bytes text, float size, Color color)
		{
			this.Label3D(position, rotation, ref text, size, LabelAlignment.MiddleLeft, color);
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString32Bytes text, float size, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
			this.PopColor();
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString64Bytes text, float size, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
			this.PopColor();
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString128Bytes text, float size, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
			this.PopColor();
		}

		public void Label3D(float3 position, quaternion rotation, ref FixedString512Bytes text, float size, LabelAlignment alignment, Color color)
		{
			this.PushColor(color);
			this.Label3D(position, rotation, text.GetUnsafePtr(), text.Length, size, alignment);
			this.PopColor();
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeAppendBuffer* buffer;

		private GCHandle gizmos;

		[NativeSetThreadIndex]
		private int threadIndex;

		private DrawingData.BuilderData.BitPackedMeta uniqueID;

		private static readonly float3 DEFAULT_UP = new float3(0f, 1f, 0f);

		internal static readonly float4x4 XZtoXYPlaneMatrix = float4x4.RotateX(-1.5707964f);

		internal static readonly float4x4 XZtoYZPlaneMatrix = float4x4.RotateZ(1.5707964f);

		[Flags]
		internal enum Command
		{
			PushColorInline = 256,
			PushColor = 0,
			PopColor = 1,
			PushMatrix = 2,
			PushSetMatrix = 3,
			PopMatrix = 4,
			Line = 5,
			Circle = 6,
			CircleXZ = 7,
			Disc = 8,
			DiscXZ = 9,
			SphereOutline = 10,
			Box = 11,
			WirePlane = 12,
			WireBox = 13,
			SolidTriangle = 14,
			PushPersist = 15,
			PopPersist = 16,
			Text = 17,
			Text3D = 18,
			PushLineWidth = 19,
			PopLineWidth = 20,
			CaptureState = 21
		}

		internal struct TriangleData
		{
			public float3 a;

			public float3 b;

			public float3 c;
		}

		internal struct LineData
		{
			public float3 a;

			public float3 b;
		}

		internal struct LineDataV3
		{
			public Vector3 a;

			public Vector3 b;
		}

		internal struct CircleXZData
		{
			public float3 center;

			public float radius;

			public float startAngle;

			public float endAngle;
		}

		internal struct CircleData
		{
			public float3 center;

			public float3 normal;

			public float radius;
		}

		internal struct SphereData
		{
			public float3 center;

			public float radius;
		}

		internal struct BoxData
		{
			public float3 center;

			public float3 size;
		}

		internal struct PlaneData
		{
			public float3 center;

			public quaternion rotation;

			public float2 size;
		}

		internal struct PersistData
		{
			public float endTime;
		}

		internal struct LineWidthData
		{
			public float pixels;

			public bool automaticJoins;
		}

		internal struct TextData
		{
			public float3 center;

			public LabelAlignment alignment;

			public float sizeInPixels;

			public int numCharacters;
		}

		internal struct TextData3D
		{
			public float3 center;

			public quaternion rotation;

			public LabelAlignment alignment;

			public float size;

			public int numCharacters;
		}

		public struct ScopeMatrix : IDisposable
		{
			public void Dispose()
			{
				this.builder.PopMatrix();
				this.builder.buffer = null;
			}

			internal CommandBuilder builder;
		}

		public struct ScopeColor : IDisposable
		{
			public void Dispose()
			{
				this.builder.PopColor();
				this.builder.buffer = null;
			}

			internal CommandBuilder builder;
		}

		public struct ScopePersist : IDisposable
		{
			public void Dispose()
			{
				this.builder.PopDuration();
				this.builder.buffer = null;
			}

			internal CommandBuilder builder;
		}

		public struct ScopeEmpty : IDisposable
		{
			public void Dispose()
			{
			}
		}

		public struct ScopeLineWidth : IDisposable
		{
			public void Dispose()
			{
				this.builder.PopLineWidth();
				this.builder.buffer = null;
			}

			internal CommandBuilder builder;
		}

		public enum SymbolDecoration
		{
			None,
			ArrowHead,
			Circle
		}

		public struct PolylineWithSymbol
		{
			public PolylineWithSymbol(CommandBuilder.SymbolDecoration symbol, float symbolSize, float symbolPadding, float symbolSpacing, bool reverseSymbols = false)
			{
				if (symbolSpacing <= 1.1754944E-38f)
				{
					throw new ArgumentOutOfRangeException("symbolSpacing", "Symbol spacing must be greater than zero");
				}
				if (symbolSize <= 1.1754944E-38f)
				{
					throw new ArgumentOutOfRangeException("symbolSize", "Symbol size must be greater than zero");
				}
				if (symbolPadding < 0f)
				{
					throw new ArgumentOutOfRangeException("symbolPadding", "Symbol padding must non-negative");
				}
				this.prev = float3.zero;
				this.symbol = symbol;
				this.symbolSize = symbolSize;
				this.symbolPadding = symbolPadding;
				this.symbolSpacing = math.max(0f, symbolSpacing - symbolPadding * 2f - symbolSize);
				this.reverseSymbols = reverseSymbols;
				this.symbolOffset = ((symbol == CommandBuilder.SymbolDecoration.ArrowHead) ? (-0.25f * symbolSize) : 0f);
				if (reverseSymbols)
				{
					this.symbolOffset = -this.symbolOffset;
				}
				this.symbolOffset += 0.5f * symbolSize;
				this.offset = -1f;
				this.odd = false;
			}

			public void MoveTo(ref CommandBuilder draw, float3 next)
			{
				if (this.offset == -1f)
				{
					this.offset = this.symbolSpacing * 0.5f;
					this.prev = next;
					return;
				}
				float num = math.length(next - this.prev);
				float num2 = math.rcp(num);
				float3 @float = next - this.prev;
				float3 float2 = default(float3);
				if (this.symbol != CommandBuilder.SymbolDecoration.None)
				{
					float2 = math.normalizesafe(math.cross(@float, math.cross(@float, new float3(0f, 1f, 0f))), default(float3));
					if (math.all(float2 == 0f))
					{
						float2 = new float3(0f, 0f, 1f);
					}
				}
				if (this.reverseSymbols)
				{
					@float = -@float;
				}
				if (this.offset > 0f && !this.odd)
				{
					draw.Line(this.prev, math.lerp(this.prev, next, math.min(this.offset * num2, 1f)));
				}
				while (this.offset < num)
				{
					if (!this.odd)
					{
						float3 center = math.lerp(this.prev, next, (this.offset + this.symbolOffset) * num2);
						switch (this.symbol)
						{
						case CommandBuilder.SymbolDecoration.None:
							break;
						case CommandBuilder.SymbolDecoration.ArrowHead:
							draw.Arrowhead(center, @float, float2, this.symbolSize);
							break;
						case CommandBuilder.SymbolDecoration.Circle:
							goto IL_1CA;
						default:
							goto IL_1CA;
						}
						IL_1DF:
						this.offset += this.symbolSize + this.symbolPadding;
						goto IL_1F9;
						IL_1CA:
						draw.Circle(center, float2, this.symbolSize * 0.5f);
						goto IL_1DF;
					}
					float3 a = math.lerp(this.prev, next, this.offset * num2);
					this.offset += this.symbolSpacing;
					float3 b = math.lerp(this.prev, next, math.min(this.offset * num2, 1f));
					draw.Line(a, b);
					this.offset += this.symbolPadding;
					IL_1F9:
					this.odd = !this.odd;
				}
				this.offset -= num;
				this.prev = next;
			}

			private float3 prev;

			private float offset;

			private readonly float symbolSize;

			private readonly float symbolSpacing;

			private readonly float symbolPadding;

			private readonly float symbolOffset;

			private readonly CommandBuilder.SymbolDecoration symbol;

			private readonly bool reverseSymbols;

			private bool odd;
		}

		[BurstCompile]
		private class JobWireMesh
		{
			[BurstCompile]
			[MonoPInvokeCallback(typeof(CommandBuilder.JobWireMesh.WireMesh_00000109$PostfixBurstDelegate))]
			public unsafe static void WireMesh(float3* verts, int* indices, int vertexCount, int indexCount, ref CommandBuilder draw)
			{
				CommandBuilder.JobWireMesh.WireMesh_00000109$BurstDirectCall.Invoke(verts, indices, vertexCount, indexCount, ref draw);
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(CommandBuilder.JobWireMesh.JobWireMeshDelegate))]
			private static void Execute(ref Mesh.MeshData rawMeshData, ref CommandBuilder draw)
			{
				CommandBuilder.JobWireMesh.Execute_0000010A$BurstDirectCall.Invoke(ref rawMeshData, ref draw);
			}

			[BurstCompile]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe static void WireMesh$BurstManaged(float3* verts, int* indices, int vertexCount, int indexCount, ref CommandBuilder draw)
			{
				NativeHashMap<int2, bool> nativeHashMap = new NativeHashMap<int2, bool>(indexCount, Allocator.Temp);
				for (int i = 0; i < indexCount; i += 3)
				{
					int num = indices[i];
					int num2 = indices[i + 1];
					int num3 = indices[i + 2];
					if (num < 0 || num2 < 0 || num3 < 0 || num >= vertexCount || num2 >= vertexCount || num3 >= vertexCount)
					{
						throw new Exception("Invalid vertex index. Index out of bounds");
					}
					int num4 = math.min(num, num2);
					int num5 = math.max(num, num2);
					if (!nativeHashMap.ContainsKey(new int2(num4, num5)))
					{
						nativeHashMap.Add(new int2(num4, num5), true);
						draw.Line(verts[num4], verts[num5]);
					}
					num4 = math.min(num2, num3);
					num5 = math.max(num2, num3);
					if (!nativeHashMap.ContainsKey(new int2(num4, num5)))
					{
						nativeHashMap.Add(new int2(num4, num5), true);
						draw.Line(verts[num4], verts[num5]);
					}
					num4 = math.min(num3, num);
					num5 = math.max(num3, num);
					if (!nativeHashMap.ContainsKey(new int2(num4, num5)))
					{
						nativeHashMap.Add(new int2(num4, num5), true);
						draw.Line(verts[num4], verts[num5]);
					}
				}
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(CommandBuilder.JobWireMesh.JobWireMeshDelegate))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe static void Execute$BurstManaged(ref Mesh.MeshData rawMeshData, ref CommandBuilder draw)
			{
				int num = 0;
				for (int i = 0; i < rawMeshData.subMeshCount; i++)
				{
					num = math.max(num, rawMeshData.GetSubMesh(i).indexCount);
				}
				NativeArray<int> nativeArray = new NativeArray<int>(num, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				NativeArray<Vector3> nativeArray2 = new NativeArray<Vector3>(rawMeshData.vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				rawMeshData.GetVertices(nativeArray2);
				for (int j = 0; j < rawMeshData.subMeshCount; j++)
				{
					SubMeshDescriptor subMesh = rawMeshData.GetSubMesh(j);
					rawMeshData.GetIndices(nativeArray, j, true);
					CommandBuilder.JobWireMesh.WireMesh((float3*)nativeArray2.GetUnsafeReadOnlyPtr<Vector3>(), (int*)nativeArray.GetUnsafeReadOnlyPtr<int>(), nativeArray2.Length, subMesh.indexCount, ref draw);
				}
			}

			public static readonly CommandBuilder.JobWireMesh.JobWireMeshDelegate JobWireMeshFunctionPointer = BurstCompiler.CompileFunctionPointer<CommandBuilder.JobWireMesh.JobWireMeshDelegate>(new CommandBuilder.JobWireMesh.JobWireMeshDelegate(CommandBuilder.JobWireMesh.Execute)).Invoke;

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void JobWireMeshDelegate(ref Mesh.MeshData rawMeshData, ref CommandBuilder draw);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal unsafe delegate void WireMesh_00000109$PostfixBurstDelegate(float3* verts, int* indices, int vertexCount, int indexCount, ref CommandBuilder draw);

			internal static class WireMesh_00000109$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (CommandBuilder.JobWireMesh.WireMesh_00000109$BurstDirectCall.Pointer == 0)
					{
						CommandBuilder.JobWireMesh.WireMesh_00000109$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CommandBuilder.JobWireMesh.WireMesh_00000109$PostfixBurstDelegate>(new CommandBuilder.JobWireMesh.WireMesh_00000109$PostfixBurstDelegate(CommandBuilder.JobWireMesh.WireMesh)).Value;
					}
					A_0 = CommandBuilder.JobWireMesh.WireMesh_00000109$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr result = (IntPtr)0;
					CommandBuilder.JobWireMesh.WireMesh_00000109$BurstDirectCall.GetFunctionPointerDiscard(ref result);
					return result;
				}

				public unsafe static void Invoke(float3* verts, int* indices, int vertexCount, int indexCount, ref CommandBuilder draw)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = CommandBuilder.JobWireMesh.WireMesh_00000109$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							calli(System.Void(Unity.Mathematics.float3*,System.Int32*,System.Int32,System.Int32,Drawing.CommandBuilder&), verts, indices, vertexCount, indexCount, ref draw, functionPointer);
							return;
						}
					}
					CommandBuilder.JobWireMesh.WireMesh$BurstManaged(verts, indices, vertexCount, indexCount, ref draw);
				}

				private static IntPtr Pointer;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void Execute_0000010A$PostfixBurstDelegate(ref Mesh.MeshData rawMeshData, ref CommandBuilder draw);

			internal static class Execute_0000010A$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (CommandBuilder.JobWireMesh.Execute_0000010A$BurstDirectCall.Pointer == 0)
					{
						CommandBuilder.JobWireMesh.Execute_0000010A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CommandBuilder.JobWireMesh.Execute_0000010A$PostfixBurstDelegate>(new CommandBuilder.JobWireMesh.Execute_0000010A$PostfixBurstDelegate(CommandBuilder.JobWireMesh.Execute)).Value;
					}
					A_0 = CommandBuilder.JobWireMesh.Execute_0000010A$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr result = (IntPtr)0;
					CommandBuilder.JobWireMesh.Execute_0000010A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
					return result;
				}

				public static void Invoke(ref Mesh.MeshData rawMeshData, ref CommandBuilder draw)
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = CommandBuilder.JobWireMesh.Execute_0000010A$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							calli(System.Void(UnityEngine.Mesh/MeshData&,Drawing.CommandBuilder&), ref rawMeshData, ref draw, functionPointer);
							return;
						}
					}
					CommandBuilder.JobWireMesh.Execute$BurstManaged(ref rawMeshData, ref draw);
				}

				private static IntPtr Pointer;
			}
		}
	}
}
