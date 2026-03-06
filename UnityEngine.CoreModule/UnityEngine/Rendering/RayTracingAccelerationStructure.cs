using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public sealed class RayTracingAccelerationStructure : IDisposable
	{
		~RayTracingAccelerationStructure()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				RayTracingAccelerationStructure.Destroy(this);
			}
			this.m_Ptr = IntPtr.Zero;
		}

		public RayTracingAccelerationStructure(RayTracingAccelerationStructure.Settings settings)
		{
			this.m_Ptr = RayTracingAccelerationStructure.Create(settings);
		}

		public RayTracingAccelerationStructure()
		{
			RayTracingAccelerationStructure.Settings desc = new RayTracingAccelerationStructure.Settings
			{
				rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything,
				managementMode = RayTracingAccelerationStructure.ManagementMode.Manual,
				layerMask = -1,
				buildFlagsStaticGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace,
				buildFlagsDynamicGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace
			};
			this.m_Ptr = RayTracingAccelerationStructure.Create(desc);
		}

		public void Release()
		{
			this.Dispose();
		}

		public void Build()
		{
			this.Build(new RayTracingAccelerationStructure.BuildSettings());
		}

		public void Build(Vector3 relativeOrigin)
		{
			RayTracingAccelerationStructure.BuildSettings buildSettings = new RayTracingAccelerationStructure.BuildSettings
			{
				relativeOrigin = relativeOrigin
			};
			this.Build(buildSettings);
		}

		public int AddInstance(Renderer targetRenderer, RayTracingSubMeshFlags[] subMeshFlags, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255U, uint id = 4294967295U)
		{
			return this.AddInstanceSubMeshFlagsArray(targetRenderer, subMeshFlags, enableTriangleCulling, frontTriangleCounterClockwise, mask, id);
		}

		public int AddInstance(RayTracingAABBsInstanceConfig config, Matrix4x4 matrix, uint id = 4294967295U)
		{
			bool flag = config.aabbBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("config.aabbBuffer.");
			}
			bool flag2 = config.aabbBuffer.target != GraphicsBuffer.Target.Structured;
			if (flag2)
			{
				throw new ArgumentException("config.aabbBuffer.target must be GraphicsBuffer.Target.Structured.");
			}
			bool flag3 = config.aabbBuffer.stride != 24;
			if (flag3)
			{
				throw new ArgumentException("config.aabbBuffer.stride must be 6 floats.");
			}
			bool flag4 = config.aabbCount == 0;
			if (flag4)
			{
				throw new ArgumentException("config.aabbCount cannot be 0.");
			}
			return this.AddAABBsInstance(config, matrix, id);
		}

		public unsafe int AddInstance(in RayTracingMeshInstanceConfig config, Matrix4x4 matrix, [DefaultValue("null")] Matrix4x4? prevMatrix = null, uint id = 4294967295U)
		{
			bool flag = config.mesh == null;
			if (flag)
			{
				throw new ArgumentNullException("config.mesh");
			}
			bool flag2 = (ulong)config.subMeshIndex >= (ulong)((long)config.mesh.subMeshCount);
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
			}
			bool flag3 = config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null;
			if (flag3)
			{
				throw new ArgumentException("config.lightProbeProxyVolume must not be null if config.lightProbeUsage is set to UseProxyVolume.");
			}
			bool flag4 = config.meshLod > 0 && config.meshLod >= config.mesh.lodCount;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
			}
			bool flag5 = prevMatrix != null;
			int result;
			if (flag5)
			{
				Matrix4x4 value = prevMatrix.Value;
				result = this.AddMeshInstance(config, matrix, &value, id);
			}
			else
			{
				result = this.AddMeshInstance(config, matrix, null, id);
			}
			return result;
		}

		public unsafe int AddInstance(in RayTracingGeometryInstanceConfig config, Matrix4x4 matrix, [DefaultValue("null")] Matrix4x4? prevMatrix = null, uint id = 4294967295U)
		{
			bool flag = config.vertexBuffer == null;
			if (flag)
			{
				throw new ArgumentException("config.vertexBuffer must not be null.");
			}
			bool flag2 = config.vertexCount == -1 && (ulong)config.vertexStart >= (ulong)((long)config.vertexBuffer.count);
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("config.vertexStart", string.Format("config.vertexStart ({0}) is out of range. Not enough vertices in the vertex buffer ({1}).", config.vertexStart, config.vertexBuffer.count));
			}
			bool flag3 = config.vertexCount != -1 && (ulong)config.vertexStart + (ulong)((long)config.vertexCount) > (ulong)((long)config.vertexBuffer.count);
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("config.vertexStart", string.Format("config.vertexStart ({0}) + config.vertexCount ({1}) is out of range. Not enough vertices in the vertex buffer ({2}).", config.vertexStart, config.vertexCount, config.vertexBuffer.count));
			}
			int num = (config.vertexCount < 0) ? config.vertexBuffer.count : config.vertexCount;
			bool flag4 = num == 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("config.vertexCount", "The amount of vertices used must be greater than 0.");
			}
			bool flag5 = config.indexBuffer != null;
			if (flag5)
			{
				bool flag6 = config.indexBuffer.count < 3;
				if (flag6)
				{
					throw new ArgumentOutOfRangeException("config.indexBuffer", "config.indexBuffer must contain at least 3 indices.");
				}
				bool flag7 = config.indexCount == -1 && (ulong)config.indexStart >= (ulong)((long)config.indexBuffer.count);
				if (flag7)
				{
					throw new ArgumentOutOfRangeException("config.indexStart", string.Format("config.indexStart ({0}) is out of range. Not enough indices in the index buffer ({1}).", config.indexStart, config.indexBuffer.count));
				}
				bool flag8 = config.indexCount != -1 && (ulong)config.indexStart + (ulong)((long)config.indexCount) > (ulong)((long)config.indexBuffer.count);
				if (flag8)
				{
					throw new ArgumentOutOfRangeException("config.indexStart", string.Format("config.indexStart ({0}) + config.indexCount ({1}) is out of range. Not enough indices in the index buffer ({2}).", config.indexStart, config.indexCount, config.indexBuffer.count));
				}
				int num2 = (config.indexCount < 0) ? config.indexBuffer.count : config.indexCount;
				bool flag9 = num2 % 3 != 0;
				if (flag9)
				{
					throw new ArgumentOutOfRangeException("config.indexBuffer", string.Format("The amount of indices used must be a multiple of 3. Only triangle geometries are supported. Currently using {0} indices.", num2));
				}
			}
			else
			{
				bool flag10 = num % 3 != 0;
				if (flag10)
				{
					throw new ArgumentOutOfRangeException("config.vertexBuffer", string.Format("When config.indexBuffer is null, the amount of vertices used must be a multiple of 3. Only triangle geometries are supported. Currently using {0} vertices.", num));
				}
			}
			bool flag11 = config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null;
			if (flag11)
			{
				throw new ArgumentException("config.lightProbeProxyVolume must not be null if config.lightProbeUsage is set to UseProxyVolume.");
			}
			bool flag12 = config.vertexAttributes == null;
			if (flag12)
			{
				throw new ArgumentNullException("config.vertexAttributes");
			}
			bool flag13 = config.vertexAttributes.Length == 0;
			if (flag13)
			{
				throw new ArgumentException("config.vertexAttributes must contain at least one entry.");
			}
			bool flag14 = prevMatrix != null;
			int result;
			if (flag14)
			{
				Matrix4x4 value = prevMatrix.Value;
				result = this.AddGeometryInstance(config, matrix, &value, id);
			}
			else
			{
				result = this.AddGeometryInstance(config, matrix, null, id);
			}
			return result;
		}

		public unsafe int AddInstances<[IsUnmanaged] T>(in RayTracingMeshInstanceConfig config, T[] instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0, uint id = 4294967295U) where T : struct, ValueType
		{
			bool flag = instanceData == null;
			if (flag)
			{
				throw new ArgumentNullException("instanceData");
			}
			bool flag2 = config.material == null;
			if (flag2)
			{
				throw new ArgumentNullException("config.material");
			}
			bool flag3 = !RayTracingAccelerationStructure.CheckMaterialEnablesInstancing(config.material);
			if (flag3)
			{
				throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
			}
			bool flag4 = config.mesh == null;
			if (flag4)
			{
				throw new ArgumentNullException("config.mesh");
			}
			bool flag5 = (ulong)config.subMeshIndex >= (ulong)((long)config.mesh.subMeshCount);
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
			}
			bool flag6 = config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null;
			if (flag6)
			{
				throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
			}
			bool flag7 = config.meshLod > 0 && config.meshLod >= config.mesh.lodCount;
			if (flag7)
			{
				throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
			}
			RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
			instanceCount = ((instanceCount == -1) ? instanceData.Length : instanceCount);
			startInstance = Math.Clamp(startInstance, 0, Math.Max(0, instanceData.Length - 1));
			instanceCount = Math.Clamp(instanceCount, 0, Math.Max(0, instanceData.Length - startInstance));
			bool flag8 = instanceCount > Graphics.kMaxDrawMeshInstanceCount;
			if (flag8)
			{
				throw new InvalidOperationException(string.Format("Instance count cannot exceed {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			T* ptr;
			if (instanceData == null || instanceData.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &instanceData[0];
			}
			return this.AddMeshInstances(config, (IntPtr)((void*)(ptr + (IntPtr)startInstance * (IntPtr)sizeof(T) / (IntPtr)sizeof(T))), cachedRenderInstancedDataLayout, (uint)instanceCount, id);
		}

		public unsafe int AddInstances<[IsUnmanaged] T>(in RayTracingMeshInstanceConfig config, List<T> instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0, uint id = 4294967295U) where T : struct, ValueType
		{
			bool flag = instanceData == null;
			if (flag)
			{
				throw new ArgumentNullException("instanceData");
			}
			bool flag2 = config.material == null;
			if (flag2)
			{
				throw new ArgumentNullException("config.material");
			}
			bool flag3 = !RayTracingAccelerationStructure.CheckMaterialEnablesInstancing(config.material);
			if (flag3)
			{
				throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
			}
			bool flag4 = config.mesh == null;
			if (flag4)
			{
				throw new ArgumentNullException("config.mesh");
			}
			bool flag5 = (ulong)config.subMeshIndex >= (ulong)((long)config.mesh.subMeshCount);
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
			}
			bool flag6 = config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null;
			if (flag6)
			{
				throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
			}
			bool flag7 = config.meshLod > 0 && config.meshLod >= config.mesh.lodCount;
			if (flag7)
			{
				throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
			}
			RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
			instanceCount = ((instanceCount == -1) ? instanceData.Count : instanceCount);
			startInstance = Math.Clamp(startInstance, 0, Math.Max(0, instanceData.Count - 1));
			instanceCount = Math.Clamp(instanceCount, 0, Math.Max(0, instanceData.Count - startInstance));
			bool flag8 = instanceCount > Graphics.kMaxDrawMeshInstanceCount;
			if (flag8)
			{
				throw new InvalidOperationException(string.Format("Instance count cannot exceed {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			T[] array;
			T* ptr;
			if ((array = NoAllocHelpers.ExtractArrayFromList<T>(instanceData)) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			return this.AddMeshInstances(config, (IntPtr)((void*)(ptr + (IntPtr)startInstance * (IntPtr)sizeof(T) / (IntPtr)sizeof(T))), cachedRenderInstancedDataLayout, (uint)instanceCount, id);
		}

		public unsafe int AddInstances<[IsUnmanaged] T>(in RayTracingMeshInstanceConfig config, NativeArray<T> instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0, uint id = 4294967295U) where T : struct, ValueType
		{
			bool flag = config.material == null;
			if (flag)
			{
				throw new ArgumentNullException("config.material");
			}
			bool flag2 = !RayTracingAccelerationStructure.CheckMaterialEnablesInstancing(config.material);
			if (flag2)
			{
				throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
			}
			bool flag3 = config.mesh == null;
			if (flag3)
			{
				throw new ArgumentNullException("config.mesh");
			}
			bool flag4 = (ulong)config.subMeshIndex >= (ulong)((long)config.mesh.subMeshCount);
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
			}
			bool flag5 = config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null;
			if (flag5)
			{
				throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
			}
			bool flag6 = config.meshLod > 0 && config.meshLod >= config.mesh.lodCount;
			if (flag6)
			{
				throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
			}
			RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
			instanceCount = ((instanceCount == -1) ? instanceData.Length : instanceCount);
			startInstance = Math.Clamp(startInstance, 0, Math.Max(0, instanceData.Length - 1));
			instanceCount = Math.Clamp(instanceCount, 0, Math.Max(0, instanceData.Length - startInstance));
			bool flag7 = instanceCount > Graphics.kMaxDrawMeshInstanceCount;
			if (flag7)
			{
				throw new InvalidOperationException(string.Format("Instance count cannot exceed {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			return this.AddMeshInstances(config, (IntPtr)((void*)((byte*)instanceData.GetUnsafeReadOnlyPtr<T>() + (IntPtr)startInstance * (IntPtr)sizeof(T))), cachedRenderInstancedDataLayout, (uint)instanceCount, id);
		}

		public int AddInstancesIndirect(in RayTracingMeshInstanceConfig config, GraphicsBuffer instanceMatrices, int maxInstanceCount, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset = 0U, uint id = 4294967295U)
		{
			bool flag = config.material == null;
			if (flag)
			{
				throw new ArgumentNullException("config.material");
			}
			bool flag2 = config.mesh == null;
			if (flag2)
			{
				throw new ArgumentNullException("config.mesh");
			}
			bool flag3 = instanceMatrices == null;
			if (flag3)
			{
				throw new ArgumentNullException("instanceMatrices");
			}
			bool flag4 = argsBuffer == null;
			if (flag4)
			{
				throw new ArgumentNullException("argsBuffer");
			}
			bool flag5 = !RayTracingAccelerationStructure.CheckMaterialEnablesInstancing(config.material);
			if (flag5)
			{
				throw new InvalidOperationException("config.material needs to enable instancing for use with AddInstancesIndirect.");
			}
			bool flag6 = (ulong)config.subMeshIndex >= (ulong)((long)config.mesh.subMeshCount);
			if (flag6)
			{
				throw new ArgumentOutOfRangeException("config.subMeshIndex", string.Format("The Mesh contains only {0} sub-meshes.", config.mesh.subMeshCount));
			}
			bool flag7 = config.lightProbeUsage > LightProbeUsage.Off;
			if (flag7)
			{
				throw new ArgumentException("config.lightProbeUsage must be LightProbeUsage.Off. This method doesn't support light probe setup.");
			}
			bool flag8 = config.lightProbeProxyVolume != null;
			if (flag8)
			{
				throw new ArgumentException("config.lightProbeProxyVolume must be null. This method doesn't support Light Probe Proxy Volume.");
			}
			bool flag9 = instanceMatrices.stride != 64;
			if (flag9)
			{
				throw new ArgumentException(string.Format("{0} ({1}) must be 64 bytes.", "stride", instanceMatrices.stride));
			}
			bool flag10 = (instanceMatrices.target & GraphicsBuffer.Target.Structured) == (GraphicsBuffer.Target)0 && (instanceMatrices.target & GraphicsBuffer.Target.Append) == (GraphicsBuffer.Target)0;
			if (flag10)
			{
				throw new ArgumentException("target must use GraphicsBuffer.Target.Structured or GraphicsBuffer.Target.Append flag.");
			}
			bool flag11 = maxInstanceCount > instanceMatrices.count;
			if (flag11)
			{
				throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, string.Format("The value cannot exceed {0}.", instanceMatrices.count));
			}
			bool flag12 = maxInstanceCount > Graphics.kMaxDrawMeshInstanceCount;
			if (flag12)
			{
				throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, string.Format("The value cannot exceed {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			bool flag13 = maxInstanceCount < -1 || maxInstanceCount == 0;
			if (flag13)
			{
				throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, "The parameter must be either -1 or a positive value.");
			}
			bool flag14 = argsBuffer.target != GraphicsBuffer.Target.Raw;
			if (flag14)
			{
				throw new ArgumentException("argsBuffer buffer must use GraphicsBuffer.Target.Raw flag.");
			}
			bool flag15 = argsBuffer.count * argsBuffer.stride < 8;
			if (flag15)
			{
				throw new ArgumentException(string.Format("{0} buffer must contain at least 2 uints at the {1} byte offset. The current size of the buffer is {2}.", "argsBuffer", argsOffset, argsBuffer.count * argsBuffer.stride));
			}
			bool flag16 = maxInstanceCount == -1;
			if (flag16)
			{
				maxInstanceCount = instanceMatrices.count;
			}
			return this.AddMeshInstancesIndirect(config, instanceMatrices, (uint)maxInstanceCount, argsBuffer, argsOffset, id);
		}

		public int AddInstancesIndirect(in RayTracingGeometryInstanceConfig config, GraphicsBuffer instanceMatrices, int maxInstanceCount, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset = 0U, uint id = 4294967295U)
		{
			bool flag = config.material == null;
			if (flag)
			{
				throw new ArgumentNullException("config.material");
			}
			bool flag2 = instanceMatrices == null;
			if (flag2)
			{
				throw new ArgumentNullException("instanceMatrices");
			}
			bool flag3 = argsBuffer == null;
			if (flag3)
			{
				throw new ArgumentNullException("argsBuffer");
			}
			bool flag4 = !RayTracingAccelerationStructure.CheckMaterialEnablesInstancing(config.material);
			if (flag4)
			{
				throw new InvalidOperationException("config.material needs to enable instancing for use with AddInstancesIndirect.");
			}
			bool flag5 = config.vertexBuffer == null;
			if (flag5)
			{
				throw new ArgumentNullException("config.vertexBuffer");
			}
			bool flag6 = config.vertexCount == -1 && (ulong)config.vertexStart >= (ulong)((long)config.vertexBuffer.count);
			if (flag6)
			{
				throw new ArgumentOutOfRangeException("config.vertexStart", config.vertexStart, string.Format("Not enough vertices in the vertex buffer ({0}).", config.vertexBuffer.count));
			}
			bool flag7 = config.vertexCount != -1 && (ulong)config.vertexStart + (ulong)((long)config.vertexCount) > (ulong)((long)config.vertexBuffer.count);
			if (flag7)
			{
				throw new ArgumentOutOfRangeException("config.vertexStart", string.Format("config.vertexStart ({0}) + config.vertexCount ({1}) is out of range. Not enough vertices in the vertex buffer ({2}).", config.vertexStart, config.vertexCount, config.vertexBuffer.count));
			}
			int num = (config.vertexCount < 0) ? config.vertexBuffer.count : config.vertexCount;
			bool flag8 = num == 0;
			if (flag8)
			{
				throw new ArgumentException("The amount of vertices used must be greater than 0.", "config.vertexCount");
			}
			bool flag9 = config.indexBuffer != null;
			if (flag9)
			{
				bool flag10 = config.indexBuffer.count < 3;
				if (flag10)
				{
					throw new ArgumentOutOfRangeException("config.indexBuffer", config.indexBuffer.count, "The index buffer must contain at least 3 indices.");
				}
				bool flag11 = config.indexCount == -1 && (ulong)config.indexStart >= (ulong)((long)config.indexBuffer.count);
				if (flag11)
				{
					throw new ArgumentOutOfRangeException("config.indexStart", config.indexStart, string.Format("The value exceeds the amount of indices ({0}) in the index buffer.", config.indexBuffer.count));
				}
				bool flag12 = config.indexCount != -1 && (ulong)config.indexStart + (ulong)((long)config.indexCount) > (ulong)((long)config.indexBuffer.count);
				if (flag12)
				{
					bool flag13 = config.indexStart == 0U;
					if (flag13)
					{
						throw new ArgumentOutOfRangeException("config.indexCount", string.Format("The value exceeds the amount of indices ({0}) in the index buffer.", config.indexBuffer.count));
					}
					throw new ArgumentOutOfRangeException("config.indexStart", string.Format("{0}.{1} ({2}) + {3}.{4} ({5}) is out of range. The result exceeds the amount of indices ({6}) in the index buffer.", new object[]
					{
						"config",
						"indexStart",
						config.indexStart,
						"config",
						"indexCount",
						config.indexCount,
						config.indexBuffer.count
					}));
				}
				else
				{
					int num2 = (config.indexCount < 0) ? config.indexBuffer.count : config.indexCount;
					bool flag14 = num2 % 3 != 0;
					if (flag14)
					{
						throw new ArgumentOutOfRangeException("config.indexBuffer", string.Format("The amount of indices used must be a multiple of 3. Only triangle geometries are supported. Currently using {0} indices.", num2));
					}
				}
			}
			else
			{
				bool flag15 = num % 3 != 0;
				if (flag15)
				{
					throw new ArgumentOutOfRangeException("config.vertexBuffer", string.Format("When {0}.{1} is null, the amount of vertices used must be a multiple of 3. Only triangle geometries are supported. Currently using {2} vertices.", "config", "indexBuffer", num));
				}
			}
			bool flag16 = config.lightProbeUsage > LightProbeUsage.Off;
			if (flag16)
			{
				throw new ArgumentException("config.lightProbeUsage must be LightProbeUsage.Off. This method doesn't support light probe setup.");
			}
			bool flag17 = config.lightProbeProxyVolume != null;
			if (flag17)
			{
				throw new ArgumentException("config.lightProbeProxyVolume must be null. This method doesn't support Light Probe Proxy Volume.");
			}
			bool flag18 = config.vertexAttributes == null;
			if (flag18)
			{
				throw new ArgumentNullException("config.vertexAttributes");
			}
			bool flag19 = config.vertexAttributes.Length == 0;
			if (flag19)
			{
				throw new ArgumentException("config.vertexAttributes must contain at least one entry.");
			}
			bool flag20 = instanceMatrices.stride != 64;
			if (flag20)
			{
				throw new ArgumentException(string.Format("{0} ({1}) must be 64 bytes.", "stride", instanceMatrices.stride));
			}
			bool flag21 = (instanceMatrices.target & GraphicsBuffer.Target.Structured) == (GraphicsBuffer.Target)0 && (instanceMatrices.target & GraphicsBuffer.Target.Append) == (GraphicsBuffer.Target)0;
			if (flag21)
			{
				throw new ArgumentException("target must use GraphicsBuffer.Target.Structured or GraphicsBuffer.Target.Append flag.");
			}
			bool flag22 = maxInstanceCount > instanceMatrices.count;
			if (flag22)
			{
				throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, string.Format("The value cannot exceed {0}.", instanceMatrices.count));
			}
			bool flag23 = maxInstanceCount > Graphics.kMaxDrawMeshInstanceCount;
			if (flag23)
			{
				throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, string.Format("The value cannot exceed {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			bool flag24 = maxInstanceCount < -1 || maxInstanceCount == 0;
			if (flag24)
			{
				throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, "The parameter must be either -1 or a positive value.");
			}
			bool flag25 = argsBuffer.target != GraphicsBuffer.Target.Raw;
			if (flag25)
			{
				throw new ArgumentException("argsBuffer buffer must use GraphicsBuffer.Target.Raw flag.");
			}
			bool flag26 = argsBuffer.count * argsBuffer.stride < 8;
			if (flag26)
			{
				throw new ArgumentException(string.Format("{0} buffer must contain at least 2 uints at the {1} byte offset. The current size of the buffer is {2}.", "argsBuffer", argsOffset, argsBuffer.count * argsBuffer.stride));
			}
			bool flag27 = maxInstanceCount == -1;
			if (flag27)
			{
				maxInstanceCount = instanceMatrices.count;
			}
			return this.AddGeometryInstancesIndirect(config, instanceMatrices, (uint)maxInstanceCount, argsBuffer, argsOffset, id);
		}

		public int AddInstances<[IsUnmanaged] T>(in RayTracingMeshInstanceConfig config, NativeSlice<T> instanceData, uint id = 4294967295U) where T : struct, ValueType
		{
			bool flag = config.material == null;
			if (flag)
			{
				throw new ArgumentNullException("config.material");
			}
			bool flag2 = !RayTracingAccelerationStructure.CheckMaterialEnablesInstancing(config.material);
			if (flag2)
			{
				throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
			}
			bool flag3 = config.mesh == null;
			if (flag3)
			{
				throw new ArgumentNullException("config.mesh");
			}
			bool flag4 = (ulong)config.subMeshIndex >= (ulong)((long)config.mesh.subMeshCount);
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
			}
			bool flag5 = config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null;
			if (flag5)
			{
				throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
			}
			bool flag6 = config.meshLod > 0 && config.meshLod >= config.mesh.lodCount;
			if (flag6)
			{
				throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
			}
			RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
			bool flag7 = instanceData.Length > Graphics.kMaxDrawMeshInstanceCount;
			if (flag7)
			{
				throw new InvalidOperationException(string.Format("Instance count cannot exceed {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			return this.AddMeshInstances(config, (IntPtr)instanceData.GetUnsafeReadOnlyPtr<T>(), cachedRenderInstancedDataLayout, (uint)instanceData.Length, id);
		}

		public void RemoveInstance(Renderer targetRenderer)
		{
			this.RemoveInstance_Renderer(targetRenderer);
		}

		public void RemoveInstance(int handle)
		{
			this.RemoveInstance_InstanceID(handle);
		}

		public void UpdateInstanceGeometry(Renderer renderer)
		{
			this.UpdateInstanceGeometry_Renderer(renderer);
		}

		public void UpdateInstanceGeometry(int handle)
		{
			this.UpdateInstanceGeometry_Handle(handle);
		}

		public void UpdateInstanceTransform(Renderer renderer)
		{
			this.UpdateInstanceTransform_Renderer(renderer);
		}

		public void UpdateInstanceTransform(int handle, Matrix4x4 matrix)
		{
			this.UpdateInstanceTransform_Handle(handle, matrix);
		}

		public void UpdateInstanceID(Renderer renderer, uint instanceID)
		{
			this.UpdateInstanceID_Renderer(renderer, instanceID);
		}

		public void UpdateInstanceID(int handle, uint instanceID)
		{
			this.UpdateInstanceID_Handle(handle, instanceID);
		}

		public void UpdateInstanceMask(Renderer renderer, uint mask)
		{
			this.UpdateInstanceMask_Renderer(renderer, mask);
		}

		public void UpdateInstanceMask(int handle, uint mask)
		{
			this.UpdateInstanceMask_Handle(handle, mask);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::Build", HasExplicitThis = true)]
		public void Build(RayTracingAccelerationStructure.BuildSettings buildSettings)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.Build_Injected(intPtr, ref buildSettings);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::AddVFXInstances", HasExplicitThis = true)]
		public unsafe void AddVFXInstances([NotNull] Renderer targetRenderer, uint[] vfxSystemMasks)
		{
			if (targetRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(targetRenderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			Span<uint> span = new Span<uint>(vfxSystemMasks);
			fixed (uint* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				RayTracingAccelerationStructure.AddVFXInstances_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveVFXInstances", HasExplicitThis = true)]
		public void RemoveVFXInstances([NotNull] Renderer targetRenderer)
		{
			if (targetRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(targetRenderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			RayTracingAccelerationStructure.RemoveVFXInstances_Injected(intPtr, intPtr2);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstancePropertyBlock", HasExplicitThis = true)]
		public void UpdateInstancePropertyBlock(int handle, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.UpdateInstancePropertyBlock_Injected(intPtr, handle, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::GetSize", HasExplicitThis = true)]
		public ulong GetSize()
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.GetSize_Injected(intPtr);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::GetInstanceCount", HasExplicitThis = true)]
		public uint GetInstanceCount()
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.GetInstanceCount_Injected(intPtr);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::ClearInstances", HasExplicitThis = true)]
		public void ClearInstances()
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.ClearInstances_Injected(intPtr);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveInstances", HasExplicitThis = true)]
		public void RemoveInstances(int layerMask, RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.RemoveInstances_Injected(intPtr, layerMask, rayTracingModeMask);
		}

		public RayTracingInstanceCullingResults CullInstances(ref RayTracingInstanceCullingConfig cullingConfig)
		{
			return this.Internal_CullInstances(cullingConfig);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::Create")]
		private static IntPtr Create(RayTracingAccelerationStructure.Settings desc)
		{
			return RayTracingAccelerationStructure.Create_Injected(ref desc);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::Destroy")]
		private static void Destroy(RayTracingAccelerationStructure accelStruct)
		{
			RayTracingAccelerationStructure.Destroy_Injected((accelStruct == null) ? ((IntPtr)0) : RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelStruct));
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveInstance", HasExplicitThis = true)]
		private void RemoveInstance_Renderer([NotNull] Renderer targetRenderer)
		{
			if (targetRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(targetRenderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			RayTracingAccelerationStructure.RemoveInstance_Renderer_Injected(intPtr, intPtr2);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveInstance", HasExplicitThis = true)]
		private void RemoveInstance_InstanceID(int instanceID)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.RemoveInstance_InstanceID_Injected(intPtr, instanceID);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceTransform", HasExplicitThis = true)]
		private void UpdateInstanceTransform_Renderer([NotNull] Renderer renderer)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(renderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			RayTracingAccelerationStructure.UpdateInstanceTransform_Renderer_Injected(intPtr, intPtr2);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceTransform", HasExplicitThis = true)]
		private void UpdateInstanceTransform_Handle(int handle, Matrix4x4 matrix)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.UpdateInstanceTransform_Handle_Injected(intPtr, handle, ref matrix);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceGeometry", HasExplicitThis = true)]
		private void UpdateInstanceGeometry_Renderer([NotNull] Renderer renderer)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(renderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			RayTracingAccelerationStructure.UpdateInstanceGeometry_Renderer_Injected(intPtr, intPtr2);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceGeometry", HasExplicitThis = true)]
		private void UpdateInstanceGeometry_Handle(int handle)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.UpdateInstanceGeometry_Handle_Injected(intPtr, handle);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceMask", HasExplicitThis = true)]
		private void UpdateInstanceMask_Renderer([NotNull] Renderer renderer, uint mask)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(renderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			RayTracingAccelerationStructure.UpdateInstanceMask_Renderer_Injected(intPtr, intPtr2, mask);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceMask", HasExplicitThis = true)]
		private void UpdateInstanceMask_Handle(int handle, uint mask)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.UpdateInstanceMask_Handle_Injected(intPtr, handle, mask);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceID", HasExplicitThis = true)]
		private void UpdateInstanceID_Renderer([NotNull] Renderer renderer, uint id)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(renderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			RayTracingAccelerationStructure.UpdateInstanceID_Renderer_Injected(intPtr, intPtr2, id);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceID", HasExplicitThis = true)]
		private void UpdateInstanceID_Handle(int handle, uint id)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingAccelerationStructure.UpdateInstanceID_Handle_Injected(intPtr, handle, id);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::AddInstanceSubMeshFlagsArray", HasExplicitThis = true)]
		private unsafe int AddInstanceSubMeshFlagsArray([NotNull] Renderer targetRenderer, RayTracingSubMeshFlags[] subMeshFlags, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255U, uint id = 4294967295U)
		{
			if (targetRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(targetRenderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
			}
			Span<RayTracingSubMeshFlags> span = new Span<RayTracingSubMeshFlags>(subMeshFlags);
			int result;
			fixed (RayTracingSubMeshFlags* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				result = RayTracingAccelerationStructure.AddInstanceSubMeshFlagsArray_Injected(intPtr, intPtr2, ref managedSpanWrapper, enableTriangleCulling, frontTriangleCounterClockwise, mask, id);
			}
			return result;
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMeshInstance", HasExplicitThis = true)]
		private unsafe int AddMeshInstance(RayTracingMeshInstanceConfig config, Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id = 4294967295U)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.AddMeshInstance_Injected(intPtr, ref config, ref matrix, prevMatrix, id);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::AddGeometryInstance", HasExplicitThis = true)]
		private unsafe int AddGeometryInstance(in RayTracingGeometryInstanceConfig config, Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id = 4294967295U)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.AddGeometryInstance_Injected(intPtr, config, ref matrix, prevMatrix, id);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMeshInstances", HasExplicitThis = true)]
		private int AddMeshInstances(RayTracingMeshInstanceConfig config, IntPtr instancedData, RenderInstancedDataLayout layout, uint instanceCount, uint id = 4294967295U)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.AddMeshInstances_Injected(intPtr, ref config, instancedData, ref layout, instanceCount, id);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMeshInstancesIndirect", HasExplicitThis = true)]
		private int AddMeshInstancesIndirect(in RayTracingMeshInstanceConfig config, GraphicsBuffer instanceMatrices, uint maxInstanceCount, GraphicsBuffer argsBuffer, uint argsOffset, uint id = 4294967295U)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.AddMeshInstancesIndirect_Injected(intPtr, config, (instanceMatrices == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(instanceMatrices), maxInstanceCount, (argsBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer), argsOffset, id);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::AddGeometryInstancesIndirect", HasExplicitThis = true)]
		private int AddGeometryInstancesIndirect(in RayTracingGeometryInstanceConfig config, GraphicsBuffer instanceMatrices, uint maxInstanceCount, GraphicsBuffer argsBuffer, uint argsOffset, uint id = 4294967295U)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.AddGeometryInstancesIndirect_Injected(intPtr, config, (instanceMatrices == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(instanceMatrices), maxInstanceCount, (argsBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer), argsOffset, id);
		}

		[FreeFunction("RayTracingAccelerationStructure_Bindings::AddAABBsInstance", HasExplicitThis = true)]
		private int AddAABBsInstance(RayTracingAABBsInstanceConfig config, Matrix4x4 matrix, uint id = 4294967295U)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RayTracingAccelerationStructure.AddAABBsInstance_Injected(intPtr, ref config, ref matrix, id);
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::CullInstances", HasExplicitThis = true)]
		private RayTracingInstanceCullingResults Internal_CullInstances(in RayTracingInstanceCullingConfig cullingConfig)
		{
			IntPtr intPtr = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RayTracingInstanceCullingResults result;
			RayTracingAccelerationStructure.Internal_CullInstances_Injected(intPtr, cullingConfig, out result);
			return result;
		}

		[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::CheckMaterialEnablesInstancing")]
		private static bool CheckMaterialEnablesInstancing(Material material)
		{
			return RayTracingAccelerationStructure.CheckMaterialEnablesInstancing_Injected(Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		[Obsolete("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build()", true)]
		public void Update()
		{
			new NotSupportedException("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build()");
		}

		[Obsolete("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build(*)", true)]
		public void Update(Vector3 relativeOrigin)
		{
			new NotSupportedException("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build(*)");
		}

		[Obsolete("This AddInstance method is deprecated and will be removed and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding Renderers to the acceleration structure.", true)]
		public void AddInstance(Renderer targetRenderer, bool[] subMeshMask = null, bool[] subMeshTransparencyFlags = null, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255U, uint id = 4294967295U)
		{
			new NotSupportedException("This AddInstance method is deprecated and will be removed and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding Renderers to the acceleration structure.");
		}

		[Obsolete("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.", true)]
		public void AddInstance(GraphicsBuffer aabbBuffer, uint numElements, Material material, bool isCutOff, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255U, bool reuseBounds = false, uint id = 4294967295U)
		{
			new NotSupportedException("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.");
		}

		[Obsolete("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.", true)]
		public int AddInstance(GraphicsBuffer aabbBuffer, uint aabbCount, bool dynamicData, Matrix4x4 matrix, Material material, bool opaqueMaterial, MaterialPropertyBlock properties, uint mask = 255U, uint id = 4294967295U)
		{
			throw new NotSupportedException("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Build_Injected(IntPtr _unity_self, [In] ref RayTracingAccelerationStructure.BuildSettings buildSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddVFXInstances_Injected(IntPtr _unity_self, IntPtr targetRenderer, ref ManagedSpanWrapper vfxSystemMasks);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveVFXInstances_Injected(IntPtr _unity_self, IntPtr targetRenderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstancePropertyBlock_Injected(IntPtr _unity_self, int handle, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong GetSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetInstanceCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearInstances_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveInstances_Injected(IntPtr _unity_self, int layerMask, RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected([In] ref RayTracingAccelerationStructure.Settings desc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy_Injected(IntPtr accelStruct);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveInstance_Renderer_Injected(IntPtr _unity_self, IntPtr targetRenderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveInstance_InstanceID_Injected(IntPtr _unity_self, int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceTransform_Renderer_Injected(IntPtr _unity_self, IntPtr renderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceTransform_Handle_Injected(IntPtr _unity_self, int handle, [In] ref Matrix4x4 matrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceGeometry_Renderer_Injected(IntPtr _unity_self, IntPtr renderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceGeometry_Handle_Injected(IntPtr _unity_self, int handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceMask_Renderer_Injected(IntPtr _unity_self, IntPtr renderer, uint mask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceMask_Handle_Injected(IntPtr _unity_self, int handle, uint mask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceID_Renderer_Injected(IntPtr _unity_self, IntPtr renderer, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateInstanceID_Handle_Injected(IntPtr _unity_self, int handle, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddInstanceSubMeshFlagsArray_Injected(IntPtr _unity_self, IntPtr targetRenderer, ref ManagedSpanWrapper subMeshFlags, bool enableTriangleCulling, bool frontTriangleCounterClockwise, uint mask, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int AddMeshInstance_Injected(IntPtr _unity_self, [In] ref RayTracingMeshInstanceConfig config, [In] ref Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int AddGeometryInstance_Injected(IntPtr _unity_self, in RayTracingGeometryInstanceConfig config, [In] ref Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddMeshInstances_Injected(IntPtr _unity_self, [In] ref RayTracingMeshInstanceConfig config, IntPtr instancedData, [In] ref RenderInstancedDataLayout layout, uint instanceCount, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddMeshInstancesIndirect_Injected(IntPtr _unity_self, in RayTracingMeshInstanceConfig config, IntPtr instanceMatrices, uint maxInstanceCount, IntPtr argsBuffer, uint argsOffset, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddGeometryInstancesIndirect_Injected(IntPtr _unity_self, in RayTracingGeometryInstanceConfig config, IntPtr instanceMatrices, uint maxInstanceCount, IntPtr argsBuffer, uint argsOffset, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddAABBsInstance_Injected(IntPtr _unity_self, [In] ref RayTracingAABBsInstanceConfig config, [In] ref Matrix4x4 matrix, uint id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CullInstances_Injected(IntPtr _unity_self, in RayTracingInstanceCullingConfig cullingConfig, out RayTracingInstanceCullingResults ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckMaterialEnablesInstancing_Injected(IntPtr material);

		internal IntPtr m_Ptr;

		private const string obsoleteBuildMsg1 = "Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build()";

		private const string obsoleteBuildMsg2 = "Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build(*)";

		private const string obsoleteRendererMsg = "This AddInstance method is deprecated and will be removed and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding Renderers to the acceleration structure.";

		private const string obsoleteAABBMsg1 = "This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.";

		private const string obsoleteAABBMsg2 = "This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.";

		[Flags]
		public enum RayTracingModeMask
		{
			Nothing = 0,
			Static = 2,
			DynamicTransform = 4,
			DynamicGeometry = 8,
			DynamicGeometryManualUpdate = 16,
			Everything = 30
		}

		public enum ManagementMode
		{
			Manual,
			Automatic
		}

		public struct BuildSettings
		{
			public RayTracingAccelerationStructureBuildFlags buildFlags { readonly get; set; }

			public Vector3 relativeOrigin { readonly get; set; }

			public BuildSettings()
			{
				this.buildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
				this.relativeOrigin = Vector3.zero;
			}

			public BuildSettings(RayTracingAccelerationStructureBuildFlags buildFlags, Vector3 relativeOrigin)
			{
				this.buildFlags = buildFlags;
				this.relativeOrigin = relativeOrigin;
			}
		}

		[Obsolete("RayTracingAccelerationStructure.RASSettings is deprecated. Use RayTracingAccelerationStructure.Settings instead. (UnityUpgradable) -> RayTracingAccelerationStructure/Settings", false)]
		public struct RASSettings
		{
			public RASSettings(RayTracingAccelerationStructure.ManagementMode sceneManagementMode, RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask, int layerMask)
			{
				this.managementMode = sceneManagementMode;
				this.rayTracingModeMask = rayTracingModeMask;
				this.layerMask = layerMask;
			}

			public RayTracingAccelerationStructure.ManagementMode managementMode;

			public RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask;

			public int layerMask;
		}

		public struct Settings
		{
			public RayTracingAccelerationStructureBuildFlags buildFlagsStaticGeometries { readonly get; set; }

			public RayTracingAccelerationStructureBuildFlags buildFlagsDynamicGeometries { readonly get; set; }

			public bool enableCompaction { readonly get; set; }

			public Settings()
			{
				this.managementMode = RayTracingAccelerationStructure.ManagementMode.Manual;
				this.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;
				this.layerMask = -1;
				this.buildFlagsStaticGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
				this.buildFlagsDynamicGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
				this.enableCompaction = true;
			}

			public Settings(RayTracingAccelerationStructure.ManagementMode sceneManagementMode, RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask, int layerMask)
			{
				this.managementMode = sceneManagementMode;
				this.rayTracingModeMask = rayTracingModeMask;
				this.layerMask = layerMask;
				this.buildFlagsStaticGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
				this.buildFlagsDynamicGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
				this.enableCompaction = true;
			}

			public Settings(RayTracingAccelerationStructure.ManagementMode sceneManagementMode, RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask, int layerMask, RayTracingAccelerationStructureBuildFlags buildFlagsStaticGeometries, RayTracingAccelerationStructureBuildFlags buildFlagsDynamicGeometries)
			{
				this.managementMode = sceneManagementMode;
				this.rayTracingModeMask = rayTracingModeMask;
				this.layerMask = layerMask;
				this.buildFlagsStaticGeometries = buildFlagsStaticGeometries;
				this.buildFlagsDynamicGeometries = buildFlagsDynamicGeometries;
				this.enableCompaction = true;
			}

			public RayTracingAccelerationStructure.ManagementMode managementMode;

			public RayTracingAccelerationStructure.RayTracingModeMask rayTracingModeMask;

			public int layerMask;
		}

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(RayTracingAccelerationStructure rayTracingAccelerationStructure)
			{
				return rayTracingAccelerationStructure.m_Ptr;
			}
		}
	}
}
