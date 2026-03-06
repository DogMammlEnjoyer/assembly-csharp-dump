using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	public class UnsafeCommandBuffer : BaseCommandBuffer, IUnsafeCommandBuffer, IBaseCommandBuffer, IRasterCommandBuffer, IComputeCommandBuffer
	{
		internal UnsafeCommandBuffer(CommandBuffer wrapped, RenderGraphPass executingPass, bool isAsync) : base(wrapped, executingPass, isAsync)
		{
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, size, offset, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, size, offset, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, mipIndex, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, mipIndex, dstFormat, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, mipIndex, dstFormat, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, mipIndex, x, width, y, height, z, depth, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, mipIndex, x, width, y, height, z, depth, dstFormat, callback);
		}

		public void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.m_WrappedCommandBuffer.RequestAsyncReadbackIntoNativeArray<T>(ref output, src, mipIndex, x, width, y, height, z, depth, dstFormat, callback);
		}

		public void SetInvertCulling(bool invertCulling)
		{
			this.m_WrappedCommandBuffer.SetInvertCulling(invertCulling);
		}

		public void SetComputeFloatParam(ComputeShader computeShader, int nameID, float val)
		{
			this.m_WrappedCommandBuffer.SetComputeFloatParam(computeShader, nameID, val);
		}

		public void SetComputeIntParam(ComputeShader computeShader, int nameID, int val)
		{
			this.m_WrappedCommandBuffer.SetComputeIntParam(computeShader, nameID, val);
		}

		public void SetComputeVectorParam(ComputeShader computeShader, int nameID, Vector4 val)
		{
			this.m_WrappedCommandBuffer.SetComputeVectorParam(computeShader, nameID, val);
		}

		public void SetComputeVectorArrayParam(ComputeShader computeShader, int nameID, Vector4[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeVectorArrayParam(computeShader, nameID, values);
		}

		public void SetComputeMatrixParam(ComputeShader computeShader, int nameID, Matrix4x4 val)
		{
			this.m_WrappedCommandBuffer.SetComputeMatrixParam(computeShader, nameID, val);
		}

		public void SetComputeMatrixArrayParam(ComputeShader computeShader, int nameID, Matrix4x4[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeMatrixArrayParam(computeShader, nameID, values);
		}

		public void Clear()
		{
			this.m_WrappedCommandBuffer.Clear();
		}

		public void SetViewport(Rect pixelRect)
		{
			this.m_WrappedCommandBuffer.SetViewport(pixelRect);
		}

		public void EnableScissorRect(Rect scissor)
		{
			this.m_WrappedCommandBuffer.EnableScissorRect(scissor);
		}

		public void DisableScissorRect()
		{
			this.m_WrappedCommandBuffer.DisableScissorRect();
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor)
		{
			this.m_WrappedCommandBuffer.ClearRenderTarget(clearDepth, clearColor, backgroundColor);
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, float depth)
		{
			this.m_WrappedCommandBuffer.ClearRenderTarget(clearDepth, clearColor, backgroundColor, depth);
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, float depth, uint stencil)
		{
			this.m_WrappedCommandBuffer.ClearRenderTarget(clearDepth, clearColor, backgroundColor, depth, stencil);
		}

		public void ClearRenderTarget(RTClearFlags clearFlags, Color backgroundColor, float depth, uint stencil)
		{
			this.m_WrappedCommandBuffer.ClearRenderTarget(clearFlags, backgroundColor, depth, stencil);
		}

		public void ClearRenderTarget(RTClearFlags clearFlags, Color[] backgroundColors, float depth, uint stencil)
		{
			this.m_WrappedCommandBuffer.ClearRenderTarget(clearFlags, backgroundColors, depth, stencil);
		}

		public void SetGlobalFloat(int nameID, float value)
		{
			this.m_WrappedCommandBuffer.SetGlobalFloat(nameID, value);
		}

		public void SetGlobalInt(int nameID, int value)
		{
			this.m_WrappedCommandBuffer.SetGlobalInt(nameID, value);
		}

		public void SetGlobalInteger(int nameID, int value)
		{
			this.m_WrappedCommandBuffer.SetGlobalInteger(nameID, value);
		}

		public void SetGlobalVector(int nameID, Vector4 value)
		{
			this.m_WrappedCommandBuffer.SetGlobalVector(nameID, value);
		}

		public void SetGlobalColor(int nameID, Color value)
		{
			this.m_WrappedCommandBuffer.SetGlobalColor(nameID, value);
		}

		public void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			this.m_WrappedCommandBuffer.SetGlobalMatrix(nameID, value);
		}

		public void EnableShaderKeyword(string keyword)
		{
			this.m_WrappedCommandBuffer.EnableShaderKeyword(keyword);
		}

		public void EnableKeyword(in GlobalKeyword keyword)
		{
			this.m_WrappedCommandBuffer.EnableKeyword(keyword);
		}

		public void EnableKeyword(Material material, in LocalKeyword keyword)
		{
			this.m_WrappedCommandBuffer.EnableKeyword(material, keyword);
		}

		public void EnableKeyword(ComputeShader computeShader, in LocalKeyword keyword)
		{
			this.m_WrappedCommandBuffer.EnableKeyword(computeShader, keyword);
		}

		public void DisableShaderKeyword(string keyword)
		{
			this.m_WrappedCommandBuffer.DisableShaderKeyword(keyword);
		}

		public void DisableKeyword(in GlobalKeyword keyword)
		{
			this.m_WrappedCommandBuffer.DisableKeyword(keyword);
		}

		public void DisableKeyword(Material material, in LocalKeyword keyword)
		{
			this.m_WrappedCommandBuffer.DisableKeyword(material, keyword);
		}

		public void DisableKeyword(ComputeShader computeShader, in LocalKeyword keyword)
		{
			this.m_WrappedCommandBuffer.DisableKeyword(computeShader, keyword);
		}

		public void SetKeyword(in GlobalKeyword keyword, bool value)
		{
			this.m_WrappedCommandBuffer.SetKeyword(keyword, value);
		}

		public void SetKeyword(Material material, in LocalKeyword keyword, bool value)
		{
			this.m_WrappedCommandBuffer.SetKeyword(material, keyword, value);
		}

		public void SetKeyword(ComputeShader computeShader, in LocalKeyword keyword, bool value)
		{
			this.m_WrappedCommandBuffer.SetKeyword(computeShader, keyword, value);
		}

		public void SetViewProjectionMatrices(Matrix4x4 view, Matrix4x4 proj)
		{
			this.m_WrappedCommandBuffer.SetViewProjectionMatrices(view, proj);
		}

		public void SetGlobalDepthBias(float bias, float slopeBias)
		{
			this.m_WrappedCommandBuffer.SetGlobalDepthBias(bias, slopeBias);
		}

		public void SetGlobalFloatArray(int nameID, float[] values)
		{
			this.m_WrappedCommandBuffer.SetGlobalFloatArray(nameID, values);
		}

		public void SetGlobalVectorArray(int nameID, Vector4[] values)
		{
			this.m_WrappedCommandBuffer.SetGlobalVectorArray(nameID, values);
		}

		public void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
		{
			this.m_WrappedCommandBuffer.SetGlobalMatrixArray(nameID, values);
		}

		public void SetLateLatchProjectionMatrices(Matrix4x4[] projectionMat)
		{
			this.m_WrappedCommandBuffer.SetLateLatchProjectionMatrices(projectionMat);
		}

		public void MarkLateLatchMatrixShaderPropertyID(CameraLateLatchMatrixType matrixPropertyType, int shaderPropertyID)
		{
			this.m_WrappedCommandBuffer.MarkLateLatchMatrixShaderPropertyID(matrixPropertyType, shaderPropertyID);
		}

		public void UnmarkLateLatchMatrix(CameraLateLatchMatrixType matrixPropertyType)
		{
			this.m_WrappedCommandBuffer.UnmarkLateLatchMatrix(matrixPropertyType);
		}

		public void BeginSample(string name)
		{
			this.m_WrappedCommandBuffer.BeginSample(name);
		}

		public void EndSample(string name)
		{
			this.m_WrappedCommandBuffer.EndSample(name);
		}

		public void BeginSample(CustomSampler sampler)
		{
			this.m_WrappedCommandBuffer.BeginSample(sampler);
		}

		public void EndSample(CustomSampler sampler)
		{
			this.m_WrappedCommandBuffer.EndSample(sampler);
		}

		public void BeginSample(ProfilerMarker marker)
		{
		}

		public void EndSample(ProfilerMarker marker)
		{
		}

		public void IncrementUpdateCount(RenderTargetIdentifier dest)
		{
			this.m_WrappedCommandBuffer.IncrementUpdateCount(dest);
		}

		public void SetInstanceMultiplier(uint multiplier)
		{
			this.m_WrappedCommandBuffer.SetInstanceMultiplier(multiplier);
		}

		public void SetFoveatedRenderingMode(FoveatedRenderingMode foveatedRenderingMode)
		{
			this.m_WrappedCommandBuffer.SetFoveatedRenderingMode(foveatedRenderingMode);
		}

		public void SetWireframe(bool enable)
		{
			this.m_WrappedCommandBuffer.SetWireframe(enable);
		}

		public void ConfigureFoveatedRendering(IntPtr platformData)
		{
			this.m_WrappedCommandBuffer.ConfigureFoveatedRendering(platformData);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(rt);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(rt, loadAction, storeAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(rt, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(rt, mipLevel);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(rt, mipLevel, cubemapFace);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(rt, mipLevel, cubemapFace, depthSlice);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(color, depth);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(color, depth, mipLevel);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(color, depth, mipLevel, cubemapFace);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(color, depth, mipLevel, cubemapFace, depthSlice);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(color, colorLoadAction, colorStoreAction, depth, depthLoadAction, depthStoreAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(colors, depth);
		}

		public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(colors, depth, mipLevel, cubemapFace, depthSlice);
		}

		public void SetRenderTarget(RenderTargetBinding binding, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(binding, mipLevel, cubemapFace, depthSlice);
		}

		public void SetRenderTarget(RenderTargetBinding binding)
		{
			this.m_WrappedCommandBuffer.SetRenderTarget(binding);
		}

		public void SetBufferData(ComputeBuffer buffer, Array data)
		{
			this.m_WrappedCommandBuffer.SetBufferData(buffer, data);
		}

		public void SetBufferData<T>(ComputeBuffer buffer, List<T> data) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data);
		}

		public void SetBufferData<T>(ComputeBuffer buffer, NativeArray<T> data) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data);
		}

		public void SetBufferData(ComputeBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
		{
			this.m_WrappedCommandBuffer.SetBufferData(buffer, data, managedBufferStartIndex, graphicsBufferStartIndex, count);
		}

		public void SetBufferData<T>(ComputeBuffer buffer, List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data, managedBufferStartIndex, graphicsBufferStartIndex, count);
		}

		public void SetBufferData<T>(ComputeBuffer buffer, NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data, nativeBufferStartIndex, graphicsBufferStartIndex, count);
		}

		public void SetBufferCounterValue(ComputeBuffer buffer, uint counterValue)
		{
			this.m_WrappedCommandBuffer.SetBufferCounterValue(buffer, counterValue);
		}

		public void SetBufferData(GraphicsBuffer buffer, Array data)
		{
			this.m_WrappedCommandBuffer.SetBufferData(buffer, data);
		}

		public void SetBufferData<T>(GraphicsBuffer buffer, List<T> data) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data);
		}

		public void SetBufferData<T>(GraphicsBuffer buffer, NativeArray<T> data) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data);
		}

		public void SetBufferData(GraphicsBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
		{
			this.m_WrappedCommandBuffer.SetBufferData(buffer, data, managedBufferStartIndex, graphicsBufferStartIndex, count);
		}

		public void SetBufferData<T>(GraphicsBuffer buffer, List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data, managedBufferStartIndex, graphicsBufferStartIndex, count);
		}

		public void SetBufferData<T>(GraphicsBuffer buffer, NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			this.m_WrappedCommandBuffer.SetBufferData<T>(buffer, data, nativeBufferStartIndex, graphicsBufferStartIndex, count);
		}

		public void SetBufferCounterValue(GraphicsBuffer buffer, uint counterValue)
		{
			this.m_WrappedCommandBuffer.SetBufferCounterValue(buffer, counterValue);
		}

		public void SetupCameraProperties(Camera camera)
		{
			this.m_WrappedCommandBuffer.SetupCameraProperties(camera);
		}

		public void InvokeOnRenderObjectCallbacks()
		{
			this.m_WrappedCommandBuffer.InvokeOnRenderObjectCallbacks();
		}

		public void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize)
		{
			this.m_WrappedCommandBuffer.SetShadingRateFragmentSize(shadingRateFragmentSize);
		}

		public void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
		{
			this.m_WrappedCommandBuffer.SetShadingRateCombiner(stage, combiner);
		}

		public void SetComputeFloatParam(ComputeShader computeShader, string name, float val)
		{
			this.m_WrappedCommandBuffer.SetComputeFloatParam(computeShader, name, val);
		}

		public void SetComputeIntParam(ComputeShader computeShader, string name, int val)
		{
			this.m_WrappedCommandBuffer.SetComputeIntParam(computeShader, name, val);
		}

		public void SetComputeVectorParam(ComputeShader computeShader, string name, Vector4 val)
		{
			this.m_WrappedCommandBuffer.SetComputeVectorParam(computeShader, name, val);
		}

		public void SetComputeVectorArrayParam(ComputeShader computeShader, string name, Vector4[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeVectorArrayParam(computeShader, name, values);
		}

		public void SetComputeMatrixParam(ComputeShader computeShader, string name, Matrix4x4 val)
		{
			this.m_WrappedCommandBuffer.SetComputeMatrixParam(computeShader, name, val);
		}

		public void SetComputeMatrixArrayParam(ComputeShader computeShader, string name, Matrix4x4[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeMatrixArrayParam(computeShader, name, values);
		}

		public void SetComputeFloatParams(ComputeShader computeShader, string name, params float[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeFloatParams(computeShader, name, values);
		}

		public void SetComputeFloatParams(ComputeShader computeShader, int nameID, params float[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeFloatParams(computeShader, nameID, values);
		}

		public void SetComputeIntParams(ComputeShader computeShader, string name, params int[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeIntParams(computeShader, name, values);
		}

		public void SetComputeIntParams(ComputeShader computeShader, int nameID, params int[] values)
		{
			this.m_WrappedCommandBuffer.SetComputeIntParams(computeShader, nameID, values);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, TextureHandle rt)
		{
			this.m_WrappedCommandBuffer.SetComputeTextureParam(computeShader, kernelIndex, name, rt);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, TextureHandle rt)
		{
			this.m_WrappedCommandBuffer.SetComputeTextureParam(computeShader, kernelIndex, nameID, rt);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, TextureHandle rt, int mipLevel)
		{
			this.m_WrappedCommandBuffer.SetComputeTextureParam(computeShader, kernelIndex, name, rt, mipLevel);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, TextureHandle rt, int mipLevel)
		{
			this.m_WrappedCommandBuffer.SetComputeTextureParam(computeShader, kernelIndex, nameID, rt, mipLevel);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, TextureHandle rt, int mipLevel, RenderTextureSubElement element)
		{
			this.m_WrappedCommandBuffer.SetComputeTextureParam(computeShader, kernelIndex, name, rt, mipLevel, element);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, TextureHandle rt, int mipLevel, RenderTextureSubElement element)
		{
			this.m_WrappedCommandBuffer.SetComputeTextureParam(computeShader, kernelIndex, nameID, rt, mipLevel, element);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, ComputeBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetComputeBufferParam(computeShader, kernelIndex, nameID, buffer);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, ComputeBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetComputeBufferParam(computeShader, kernelIndex, name, buffer);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, GraphicsBufferHandle bufferHandle)
		{
			this.m_WrappedCommandBuffer.SetComputeBufferParam(computeShader, kernelIndex, nameID, bufferHandle);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, GraphicsBufferHandle bufferHandle)
		{
			this.m_WrappedCommandBuffer.SetComputeBufferParam(computeShader, kernelIndex, name, bufferHandle);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, GraphicsBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetComputeBufferParam(computeShader, kernelIndex, nameID, buffer);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, GraphicsBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetComputeBufferParam(computeShader, kernelIndex, name, buffer);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, int nameID, ComputeBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetComputeConstantBufferParam(computeShader, nameID, buffer, offset, size);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, string name, ComputeBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetComputeConstantBufferParam(computeShader, name, buffer, offset, size);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetComputeConstantBufferParam(computeShader, nameID, buffer, offset, size);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, string name, GraphicsBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetComputeConstantBufferParam(computeShader, name, buffer, offset, size);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
		{
			this.m_WrappedCommandBuffer.DispatchCompute(computeShader, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset)
		{
			this.m_WrappedCommandBuffer.DispatchCompute(computeShader, kernelIndex, indirectBuffer, argsOffset);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, GraphicsBuffer indirectBuffer, uint argsOffset)
		{
			this.m_WrappedCommandBuffer.DispatchCompute(computeShader, kernelIndex, indirectBuffer, argsOffset);
		}

		public void BuildRayTracingAccelerationStructure(RayTracingAccelerationStructure accelerationStructure)
		{
			this.m_WrappedCommandBuffer.BuildRayTracingAccelerationStructure(accelerationStructure);
		}

		public void BuildRayTracingAccelerationStructure(RayTracingAccelerationStructure accelerationStructure, Vector3 relativeOrigin)
		{
			this.m_WrappedCommandBuffer.BuildRayTracingAccelerationStructure(accelerationStructure, relativeOrigin);
		}

		public void SetRayTracingAccelerationStructure(RayTracingShader rayTracingShader, string name, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.m_WrappedCommandBuffer.SetRayTracingAccelerationStructure(rayTracingShader, name, rayTracingAccelerationStructure);
		}

		public void SetRayTracingAccelerationStructure(RayTracingShader rayTracingShader, int nameID, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.m_WrappedCommandBuffer.SetRayTracingAccelerationStructure(rayTracingShader, nameID, rayTracingAccelerationStructure);
		}

		public void SetRayTracingAccelerationStructure(ComputeShader computeShader, int kernelIndex, string name, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.m_WrappedCommandBuffer.SetRayTracingAccelerationStructure(computeShader, kernelIndex, name, rayTracingAccelerationStructure);
		}

		public void SetRayTracingAccelerationStructure(ComputeShader computeShader, int kernelIndex, int nameID, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.m_WrappedCommandBuffer.SetRayTracingAccelerationStructure(computeShader, kernelIndex, nameID, rayTracingAccelerationStructure);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, string name, ComputeBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetRayTracingBufferParam(rayTracingShader, name, buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, int nameID, ComputeBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetRayTracingBufferParam(rayTracingShader, nameID, buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, string name, GraphicsBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetRayTracingBufferParam(rayTracingShader, name, buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, int nameID, GraphicsBuffer buffer)
		{
			this.m_WrappedCommandBuffer.SetRayTracingBufferParam(rayTracingShader, nameID, buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, string name, GraphicsBufferHandle bufferHandle)
		{
			this.m_WrappedCommandBuffer.SetRayTracingBufferParam(rayTracingShader, name, bufferHandle);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, int nameID, GraphicsBufferHandle bufferHandle)
		{
			this.m_WrappedCommandBuffer.SetRayTracingBufferParam(rayTracingShader, nameID, bufferHandle);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, int nameID, ComputeBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetRayTracingConstantBufferParam(rayTracingShader, nameID, buffer, offset, size);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, string name, ComputeBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetRayTracingConstantBufferParam(rayTracingShader, name, buffer, offset, size);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetRayTracingConstantBufferParam(rayTracingShader, nameID, buffer, offset, size);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, string name, GraphicsBuffer buffer, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetRayTracingConstantBufferParam(rayTracingShader, name, buffer, offset, size);
		}

		public void SetRayTracingTextureParam(RayTracingShader rayTracingShader, string name, TextureHandle rt)
		{
			this.m_WrappedCommandBuffer.SetRayTracingTextureParam(rayTracingShader, name, rt);
		}

		public void SetRayTracingTextureParam(RayTracingShader rayTracingShader, int nameID, TextureHandle rt)
		{
			this.m_WrappedCommandBuffer.SetRayTracingTextureParam(rayTracingShader, nameID, rt);
		}

		public void SetRayTracingFloatParam(RayTracingShader rayTracingShader, string name, float val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingFloatParam(rayTracingShader, name, val);
		}

		public void SetRayTracingFloatParam(RayTracingShader rayTracingShader, int nameID, float val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingFloatParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingFloatParams(RayTracingShader rayTracingShader, string name, params float[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingFloatParams(rayTracingShader, name, values);
		}

		public void SetRayTracingFloatParams(RayTracingShader rayTracingShader, int nameID, params float[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingFloatParams(rayTracingShader, nameID, values);
		}

		public void SetRayTracingIntParam(RayTracingShader rayTracingShader, string name, int val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingIntParam(rayTracingShader, name, val);
		}

		public void SetRayTracingIntParam(RayTracingShader rayTracingShader, int nameID, int val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingIntParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingIntParams(RayTracingShader rayTracingShader, string name, params int[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingIntParams(rayTracingShader, name, values);
		}

		public void SetRayTracingIntParams(RayTracingShader rayTracingShader, int nameID, params int[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingIntParams(rayTracingShader, nameID, values);
		}

		public void SetRayTracingVectorParam(RayTracingShader rayTracingShader, string name, Vector4 val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingVectorParam(rayTracingShader, name, val);
		}

		public void SetRayTracingVectorParam(RayTracingShader rayTracingShader, int nameID, Vector4 val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingVectorParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingVectorArrayParam(RayTracingShader rayTracingShader, string name, params Vector4[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingVectorArrayParam(rayTracingShader, name, values);
		}

		public void SetRayTracingVectorArrayParam(RayTracingShader rayTracingShader, int nameID, params Vector4[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingVectorArrayParam(rayTracingShader, nameID, values);
		}

		public void SetRayTracingMatrixParam(RayTracingShader rayTracingShader, string name, Matrix4x4 val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingMatrixParam(rayTracingShader, name, val);
		}

		public void SetRayTracingMatrixParam(RayTracingShader rayTracingShader, int nameID, Matrix4x4 val)
		{
			this.m_WrappedCommandBuffer.SetRayTracingMatrixParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingMatrixArrayParam(RayTracingShader rayTracingShader, string name, params Matrix4x4[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingMatrixArrayParam(rayTracingShader, name, values);
		}

		public void SetRayTracingMatrixArrayParam(RayTracingShader rayTracingShader, int nameID, params Matrix4x4[] values)
		{
			this.m_WrappedCommandBuffer.SetRayTracingMatrixArrayParam(rayTracingShader, nameID, values);
		}

		public void DispatchRays(RayTracingShader rayTracingShader, string rayGenName, uint width, uint height, uint depth, Camera camera)
		{
			this.m_WrappedCommandBuffer.DispatchRays(rayTracingShader, rayGenName, width, height, depth, camera);
		}

		public void DispatchRays(RayTracingShader rayTracingShader, string rayGenName, GraphicsBuffer argsBuffer, uint argsOffset, Camera camera)
		{
			this.m_WrappedCommandBuffer.DispatchRays(rayTracingShader, rayGenName, argsBuffer, argsOffset, camera);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass)
		{
			this.m_WrappedCommandBuffer.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex)
		{
			this.m_WrappedCommandBuffer.DrawMesh(mesh, matrix, material, submeshIndex);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material)
		{
			this.m_WrappedCommandBuffer.DrawMesh(mesh, matrix, material);
		}

		public void DrawMultipleMeshes(Matrix4x4[] matrices, Mesh[] meshes, int[] subsetIndices, int count, Material material, int shaderPass, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawMultipleMeshes(matrices, meshes, subsetIndices, count, material, shaderPass, properties);
		}

		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex, int shaderPass)
		{
			this.m_WrappedCommandBuffer.DrawRenderer(renderer, material, submeshIndex, shaderPass);
		}

		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			this.m_WrappedCommandBuffer.DrawRenderer(renderer, material, submeshIndex);
		}

		public void DrawRenderer(Renderer renderer, Material material)
		{
			this.m_WrappedCommandBuffer.DrawRenderer(renderer, material);
		}

		public void DrawRendererList(RendererList rendererList)
		{
			this.m_WrappedCommandBuffer.DrawRendererList(rendererList);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount)
		{
			this.m_WrappedCommandBuffer.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount)
		{
			this.m_WrappedCommandBuffer.DrawProcedural(matrix, material, shaderPass, topology, vertexCount);
		}

		public void DrawProcedural(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawProcedural(indexBuffer, matrix, material, shaderPass, topology, indexCount, instanceCount, properties);
		}

		public void DrawProcedural(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount, int instanceCount)
		{
			this.m_WrappedCommandBuffer.DrawProcedural(indexBuffer, matrix, material, shaderPass, topology, indexCount, instanceCount);
		}

		public void DrawProcedural(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount)
		{
			this.m_WrappedCommandBuffer.DrawProcedural(indexBuffer, matrix, material, shaderPass, topology, indexCount);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs)
		{
			this.m_WrappedCommandBuffer.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, properties);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices);
		}

		public void DrawMeshInstancedProcedural(Mesh mesh, int submeshIndex, Material material, int shaderPass, int count, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedProcedural(mesh, submeshIndex, material, shaderPass, count, properties);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs, int argsOffset)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs)
		{
			this.m_WrappedCommandBuffer.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs);
		}

		public void DrawOcclusionMesh(RectInt normalizedCamViewport)
		{
			this.m_WrappedCommandBuffer.DrawOcclusionMesh(normalizedCamViewport);
		}

		public void CopyCounterValue(ComputeBuffer src, ComputeBuffer dst, uint dstOffsetBytes)
		{
			this.m_WrappedCommandBuffer.CopyCounterValue(src, dst, dstOffsetBytes);
		}

		public void CopyCounterValue(GraphicsBuffer src, ComputeBuffer dst, uint dstOffsetBytes)
		{
			this.m_WrappedCommandBuffer.CopyCounterValue(src, dst, dstOffsetBytes);
		}

		public void CopyCounterValue(ComputeBuffer src, GraphicsBuffer dst, uint dstOffsetBytes)
		{
			this.m_WrappedCommandBuffer.CopyCounterValue(src, dst, dstOffsetBytes);
		}

		public void CopyCounterValue(GraphicsBuffer src, GraphicsBuffer dst, uint dstOffsetBytes)
		{
			this.m_WrappedCommandBuffer.CopyCounterValue(src, dst, dstOffsetBytes);
		}

		public void SetGlobalFloat(string name, float value)
		{
			this.m_WrappedCommandBuffer.SetGlobalFloat(name, value);
		}

		public void SetGlobalInt(string name, int value)
		{
			this.m_WrappedCommandBuffer.SetGlobalInt(name, value);
		}

		public void SetGlobalInteger(string name, int value)
		{
			this.m_WrappedCommandBuffer.SetGlobalInteger(name, value);
		}

		public void SetGlobalVector(string name, Vector4 value)
		{
			this.m_WrappedCommandBuffer.SetGlobalVector(name, value);
		}

		public void SetGlobalColor(string name, Color value)
		{
			this.m_WrappedCommandBuffer.SetGlobalColor(name, value);
		}

		public void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			this.m_WrappedCommandBuffer.SetGlobalMatrix(name, value);
		}

		public void SetGlobalFloatArray(string propertyName, List<float> values)
		{
			this.m_WrappedCommandBuffer.SetGlobalFloatArray(propertyName, values);
		}

		public void SetGlobalFloatArray(int nameID, List<float> values)
		{
			this.m_WrappedCommandBuffer.SetGlobalFloatArray(nameID, values);
		}

		public void SetGlobalFloatArray(string propertyName, float[] values)
		{
			this.m_WrappedCommandBuffer.SetGlobalFloatArray(propertyName, values);
		}

		public void SetGlobalVectorArray(string propertyName, List<Vector4> values)
		{
			this.m_WrappedCommandBuffer.SetGlobalVectorArray(propertyName, values);
		}

		public void SetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			this.m_WrappedCommandBuffer.SetGlobalVectorArray(nameID, values);
		}

		public void SetGlobalVectorArray(string propertyName, Vector4[] values)
		{
			this.m_WrappedCommandBuffer.SetGlobalVectorArray(propertyName, values);
		}

		public void SetGlobalMatrixArray(string propertyName, List<Matrix4x4> values)
		{
			this.m_WrappedCommandBuffer.SetGlobalMatrixArray(propertyName, values);
		}

		public void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			this.m_WrappedCommandBuffer.SetGlobalMatrixArray(nameID, values);
		}

		public void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
		{
			this.m_WrappedCommandBuffer.SetGlobalMatrixArray(propertyName, values);
		}

		public void SetGlobalTexture(string name, TextureHandle value)
		{
			this.m_WrappedCommandBuffer.SetGlobalTexture(name, value);
		}

		public void SetGlobalTexture(int nameID, TextureHandle value)
		{
			this.m_WrappedCommandBuffer.SetGlobalTexture(nameID, value);
		}

		public void SetGlobalTexture(string name, TextureHandle value, RenderTextureSubElement element)
		{
			this.m_WrappedCommandBuffer.SetGlobalTexture(name, value, element);
		}

		public void SetGlobalTexture(int nameID, TextureHandle value, RenderTextureSubElement element)
		{
			this.m_WrappedCommandBuffer.SetGlobalTexture(nameID, value, element);
		}

		public void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			this.m_WrappedCommandBuffer.SetGlobalBuffer(name, value);
		}

		public void SetGlobalBuffer(int nameID, ComputeBuffer value)
		{
			this.m_WrappedCommandBuffer.SetGlobalBuffer(nameID, value);
		}

		public void SetGlobalBuffer(string name, GraphicsBuffer value)
		{
			this.m_WrappedCommandBuffer.SetGlobalBuffer(name, value);
		}

		public void SetGlobalBuffer(int nameID, GraphicsBuffer value)
		{
			this.m_WrappedCommandBuffer.SetGlobalBuffer(nameID, value);
		}

		public void SetGlobalConstantBuffer(ComputeBuffer buffer, int nameID, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetGlobalConstantBuffer(buffer, nameID, offset, size);
		}

		public void SetGlobalConstantBuffer(ComputeBuffer buffer, string name, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetGlobalConstantBuffer(buffer, name, offset, size);
		}

		public void SetGlobalConstantBuffer(GraphicsBuffer buffer, int nameID, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetGlobalConstantBuffer(buffer, nameID, offset, size);
		}

		public void SetGlobalConstantBuffer(GraphicsBuffer buffer, string name, int offset, int size)
		{
			this.m_WrappedCommandBuffer.SetGlobalConstantBuffer(buffer, name, offset, size);
		}

		public void SetShadowSamplingMode(RenderTargetIdentifier shadowmap, ShadowSamplingMode mode)
		{
			this.m_WrappedCommandBuffer.SetShadowSamplingMode(shadowmap, mode);
		}

		public void SetSinglePassStereo(SinglePassStereoMode mode)
		{
			this.m_WrappedCommandBuffer.SetSinglePassStereo(mode);
		}

		public void IssuePluginEvent(IntPtr callback, int eventID)
		{
			this.m_WrappedCommandBuffer.IssuePluginEvent(callback, eventID);
		}

		public void IssuePluginEventAndData(IntPtr callback, int eventID, IntPtr data)
		{
			this.m_WrappedCommandBuffer.IssuePluginEventAndData(callback, eventID, data);
		}

		public void IssuePluginCustomBlit(IntPtr callback, uint command, RenderTargetIdentifier source, RenderTargetIdentifier dest, uint commandParam, uint commandFlags)
		{
			this.m_WrappedCommandBuffer.IssuePluginCustomBlit(callback, command, source, dest, commandParam, commandFlags);
		}

		public void IssuePluginCustomTextureUpdateV2(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.m_WrappedCommandBuffer.IssuePluginCustomTextureUpdateV2(callback, targetTexture, userData);
		}

		void IBaseCommandBuffer.EnableKeyword(in GlobalKeyword keyword)
		{
			this.EnableKeyword(keyword);
		}

		void IBaseCommandBuffer.EnableKeyword(Material material, in LocalKeyword keyword)
		{
			this.EnableKeyword(material, keyword);
		}

		void IBaseCommandBuffer.EnableKeyword(ComputeShader computeShader, in LocalKeyword keyword)
		{
			this.EnableKeyword(computeShader, keyword);
		}

		void IBaseCommandBuffer.DisableKeyword(in GlobalKeyword keyword)
		{
			this.DisableKeyword(keyword);
		}

		void IBaseCommandBuffer.DisableKeyword(Material material, in LocalKeyword keyword)
		{
			this.DisableKeyword(material, keyword);
		}

		void IBaseCommandBuffer.DisableKeyword(ComputeShader computeShader, in LocalKeyword keyword)
		{
			this.DisableKeyword(computeShader, keyword);
		}

		void IBaseCommandBuffer.SetKeyword(in GlobalKeyword keyword, bool value)
		{
			this.SetKeyword(keyword, value);
		}

		void IBaseCommandBuffer.SetKeyword(Material material, in LocalKeyword keyword, bool value)
		{
			this.SetKeyword(material, keyword, value);
		}

		void IBaseCommandBuffer.SetKeyword(ComputeShader computeShader, in LocalKeyword keyword, bool value)
		{
			this.SetKeyword(computeShader, keyword, value);
		}
	}
}
