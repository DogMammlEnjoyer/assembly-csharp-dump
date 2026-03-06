using System;
using System.Runtime.InteropServices;
using Meta.XR.Acoustics;
using UnityEngine;

public class MetaXRAcousticNativeInterface
{
	public static MetaXRAcousticNativeInterface.INativeInterface Interface
	{
		get
		{
			if (MetaXRAcousticNativeInterface.CachedInterface == null)
			{
				MetaXRAcousticNativeInterface.CachedInterface = MetaXRAcousticNativeInterface.FindInterface();
			}
			return MetaXRAcousticNativeInterface.CachedInterface;
		}
	}

	private static MetaXRAcousticNativeInterface.INativeInterface FindInterface()
	{
		try
		{
			MetaXRAcousticNativeInterface.WwisePluginInterface.getOrCreateGlobalOvrAudioContext();
			int num;
			int num2;
			int num3;
			MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_GetVersion(out num, out num2, out num3);
			if (num2 < 92)
			{
				Debug.LogError("Incompatible SDK version, update your MetaXRAudioWwise plugin");
				return new MetaXRAcousticNativeInterface.DummyInterface();
			}
			Debug.Log("Meta XR Audio Native Interface initialized with Wwise plugin");
			return new MetaXRAcousticNativeInterface.WwisePluginInterface();
		}
		catch (DllNotFoundException)
		{
		}
		try
		{
			IntPtr intPtr;
			MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_GetPluginContext(out intPtr);
			int num4;
			int num5;
			int num6;
			MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_GetVersion(out num4, out num5, out num6);
			if (num5 < 92)
			{
				Debug.LogError("Incompatible SDK version, update your MetaXRAudioFMOD plugin");
				return new MetaXRAcousticNativeInterface.DummyInterface();
			}
			Debug.Log("Meta XR Audio Native Interface initialized with FMOD plugin");
			return new MetaXRAcousticNativeInterface.FMODPluginInterface();
		}
		catch (DllNotFoundException)
		{
		}
		try
		{
			IntPtr intPtr2;
			MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_GetPluginContext(out intPtr2);
			int num7;
			int num8;
			int num9;
			MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_GetVersion(out num7, out num8, out num9);
			if (num8 < 92)
			{
				Debug.LogError("Incompatible SDK version, update your MetaXRAudioFMOD plugin");
				return new MetaXRAcousticNativeInterface.DummyInterface();
			}
			Debug.Log("Meta XR Audio Native Interface initialized with Unity plugin");
			return new MetaXRAcousticNativeInterface.UnityNativeInterface();
		}
		catch
		{
			Debug.LogError("Unable to locate MetaXRAudio plugin for MetaXRAcoustics!\nIf you're using Unity audio make sure you have imported the MetaXRAudioUnity package\nIf you're using Wwise or FMOD make sure you have their Unity integration in your project and the MetaXRAudioWwise or MetaXRAudioFMOD plugins in correct location in the Assets folder");
		}
		return new MetaXRAcousticNativeInterface.DummyInterface();
	}

	private static MetaXRAcousticNativeInterface.INativeInterface CachedInterface;

	public enum ovrAudioScalarType : uint
	{
		Int8,
		UInt8,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Float16,
		Float32,
		Float64
	}

	public interface INativeInterface
	{
		int SetAcousticModel(AcousticModel model);

		int ResetReverb();

		int SetEnabled(int feature, bool enabled);

		int SetEnabled(EnableFlagInternal feature, bool enabled);

		int CreateAudioGeometry(out IntPtr geometry);

		int DestroyAudioGeometry(IntPtr geometry);

		int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled);

		int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount);

		int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount, ref MeshSimplification simplification);

		int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix);

		int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		int AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength);

		int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices);

		int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		int CreateAudioMaterial(out IntPtr material);

		int DestroyAudioMaterial(IntPtr material);

		int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		int AudioMaterialReset(IntPtr material, MaterialProperty property);

		int CreateAudioSceneIR(out IntPtr sceneIR);

		int DestroyAudioSceneIR(IntPtr sceneIR);

		int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled);

		int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled);

		int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);

		int InitializeAudioSceneIRParameters(out MapParameters parameters);

		int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);

		int AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters);

		int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);

		int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);

		int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix);

		int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);

		int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);

		int AudioSceneIRReadFile(IntPtr sceneIR, string filePath);

		int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength);

		int CreateControlZone(out IntPtr control);

		int DestroyControlZone(IntPtr control);

		int ControlZoneSetEnabled(IntPtr control, bool enabled);

		int ControlZoneGetEnabled(IntPtr control, out bool enabled);

		int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix);

		int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);

		int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		int ControlZoneReset(IntPtr control, ControlZoneProperty property);
	}

	public class UnityNativeInterface : MetaXRAcousticNativeInterface.INativeInterface
	{
		private IntPtr context
		{
			get
			{
				if (this.context_ == IntPtr.Zero)
				{
					MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_GetPluginContext(out this.context_);
					int num;
					int num2;
					MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_GetVersion(out num, out this.version, out num2);
				}
				return this.context_;
			}
		}

		[DllImport("MetaXRAudioUnity")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context);

		[DllImport("MetaXRAudioUnity")]
		public static extern IntPtr ovrAudio_GetVersion(out int Major, out int Minor, out int Patch);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetAcousticModel(IntPtr context, AcousticModel quality);

		public int SetAcousticModel(AcousticModel model)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_SetAcousticModel(this.context, model);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ResetSharedReverb(IntPtr context);

		public int ResetReverb()
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ResetSharedReverb(this.context);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);

		public int SetEnabled(int feature, bool enabled)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_Enable(IntPtr context, EnableFlagInternal what, int enable);

		public int SetEnabled(EnableFlagInternal feature, bool enabled)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_CreateAudioGeometry(this.context, out geometry);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_DestroyAudioGeometry(geometry);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, int enabled);

		public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
		{
			if (this.version < 94)
			{
				return -1;
			}
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometrySetObjectFlag(geometry, flag, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, MetaXRAcousticNativeInterface.ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, MetaXRAcousticNativeInterface.ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount);

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryUploadMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)((ulong)((long)vertexCount)), UIntPtr.Zero, MetaXRAcousticNativeInterface.ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)((ulong)((long)indexCount)), MetaXRAcousticNativeInterface.ovrAudioScalarType.UInt32, groups, (UIntPtr)((ulong)((long)groupCount)));
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, MetaXRAcousticNativeInterface.ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, MetaXRAcousticNativeInterface.ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount, ref MeshSimplification simplification);

		public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount, ref MeshSimplification simplification)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)((ulong)((long)vertexCount)), UIntPtr.Zero, MetaXRAcousticNativeInterface.ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)((ulong)((long)indexCount)), MetaXRAcousticNativeInterface.ovrAudioScalarType.UInt32, groups, (UIntPtr)((ulong)((long)groupCount)), ref simplification);
		}

		[DllImport("MetaXRAudioUnity")]
		private unsafe static extern int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float* matrix4x4);

		public unsafe int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometrySetTransform(geometry, ptr);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength);

		public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryReadMeshMemory(geometry, data, dataLength);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, IntPtr unused1, out uint numVertices, IntPtr unused2, IntPtr unused3, out uint numTriangles);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, float[] vertices, ref uint numVertices, uint[] indices, uint[] materialIndices, ref uint numTriangles);

		public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
		{
			uint num2;
			uint num3;
			int num = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, IntPtr.Zero, out num2, IntPtr.Zero, IntPtr.Zero, out num3);
			if (num != 0)
			{
				Debug.LogError("unexpected error getting simplified mesh array sizes");
				vertices = null;
				indices = null;
				materialIndices = null;
				return num;
			}
			vertices = new float[num2 * 3U];
			indices = new uint[num3 * 3U];
			materialIndices = new uint[num3];
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, vertices, ref num2, indices, materialIndices, ref num3);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);

		public int CreateAudioMaterial(out IntPtr material)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_CreateAudioMaterial(this.context, out material);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);

		public int DestroyAudioMaterial(IntPtr material)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_DestroyAudioMaterial(material);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioMaterialReset(material, property);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_CreateAudioSceneIR(IntPtr context, out IntPtr sceneIR);

		public int CreateAudioSceneIR(out IntPtr sceneIR)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_CreateAudioSceneIR(this.context, out sceneIR);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_DestroyAudioSceneIR(IntPtr sceneIR);

		public int DestroyAudioSceneIR(IntPtr sceneIR)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_DestroyAudioSceneIR(sceneIR);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRSetEnabled(IntPtr sceneIR, int enabled);

		public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRSetEnabled(sceneIR, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRGetEnabled(IntPtr sceneIR, out int enabled);

		public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
		{
			int num;
			int result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRGetEnabled(sceneIR, out num);
			enabled = (num != 0);
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);

		public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRGetStatus(sceneIR, out status);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_InitializeAudioSceneIRParameters(out MapParameters parameters);

		public int InitializeAudioSceneIRParameters(out MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_InitializeAudioSceneIRParameters(out parameters);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);

		public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRCompute(sceneIR, ref parameters);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters);

		public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRComputeCustomPoints(sceneIR, points, pointCount, ref parameters);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);

		public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRGetPointCount(sceneIR, out pointCount);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);

		public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRGetPoints(sceneIR, points, maxPointCount);
		}

		[DllImport("MetaXRAudioUnity")]
		private unsafe static extern int ovrAudio_AudioSceneIRSetTransform(IntPtr sceneIR, float* matrix4x4);

		public unsafe int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRSetTransform(sceneIR, ptr);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);

		public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRGetTransform(sceneIR, out matrix4x4);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);

		public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRWriteFile(sceneIR, filePath);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRReadFile(IntPtr sceneIR, string filePath);

		public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRReadFile(sceneIR, filePath);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength);

		public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength)
		{
			return MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_AudioSceneIRReadMemory(sceneIR, data, dataLength);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_CreateControlZone(IntPtr context, out IntPtr control);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_CreateControlVolume(IntPtr context, out IntPtr control);

		public int CreateControlZone(out IntPtr control)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_CreateControlZone(this.context, out control);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_CreateControlVolume(this.context, out control);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_DestroyControlZone(IntPtr control);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_DestroyControlVolume(IntPtr control);

		public int DestroyControlZone(IntPtr control)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_DestroyControlZone(control);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_DestroyControlVolume(control);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneSetEnabled(IntPtr control, int enabled);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeSetEnabled(IntPtr control, int enabled);

		public int ControlZoneSetEnabled(IntPtr control, bool enabled)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneSetEnabled(control, enabled ? 1 : 0);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeSetEnabled(control, enabled ? 1 : 0);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneGetEnabled(IntPtr control, out int enabled);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeGetEnabled(IntPtr control, out int enabled);

		public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
		{
			int num = 0;
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneGetEnabled(control, out num);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeGetEnabled(control, out num);
			}
			enabled = (num != 0);
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private unsafe static extern int ovrAudio_ControlZoneSetTransform(IntPtr control, float* matrix4x4);

		[DllImport("MetaXRAudioUnity")]
		private unsafe static extern int ovrAudio_ControlVolumeSetTransform(IntPtr control, float* matrix4x4);

		public unsafe int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneSetTransform(control, ptr);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeSetTransform(control, ptr);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeGetTransform(IntPtr control, out float[] matrix4x4);

		public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneGetTransform(control, out matrix4x4);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeGetTransform(control, out matrix4x4);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneSetBox(control, sizeX, sizeY, sizeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeSetBox(control, sizeX, sizeY, sizeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneGetBox(control, out sizeX, out sizeY, out sizeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeGetBox(control, out sizeX, out sizeY, out sizeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneSetFadeDistance(control, fadeX, fadeY, fadeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeSetFadeDistance(control, fadeX, fadeY, fadeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneSetFrequency(control, property, frequency, value);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeSetFrequency(control, property, frequency, value);
			}
			return result;
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlZoneReset(IntPtr control, ControlZoneProperty property);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_ControlVolumeReset(IntPtr control, ControlZoneProperty property);

		public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlZoneReset(control, property);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.UnityNativeInterface.ovrAudio_ControlVolumeReset(control, property);
			}
			return result;
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			return this.AudioGeometrySetTransform(geometry, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			return this.AudioSceneIRSetTransform(sceneIR, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			return this.ControlZoneSetTransform(control, matrix);
		}

		public const string binaryName = "MetaXRAudioUnity";

		private IntPtr context_ = IntPtr.Zero;

		private int version;
	}

	public class WwisePluginInterface : MetaXRAcousticNativeInterface.INativeInterface
	{
		private IntPtr context
		{
			get
			{
				if (this.context_ == IntPtr.Zero)
				{
					this.context_ = MetaXRAcousticNativeInterface.WwisePluginInterface.getOrCreateGlobalOvrAudioContext();
					int num;
					int num2;
					MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_GetVersion(out num, out this.version, out num2);
				}
				return this.context_;
			}
		}

		[DllImport("MetaXRAudioWwise")]
		public static extern IntPtr getOrCreateGlobalOvrAudioContext();

		[DllImport("MetaXRAudioWwise")]
		public static extern IntPtr ovrAudio_GetVersion(out int Major, out int Minor, out int Patch);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetAcousticModel(IntPtr context, AcousticModel quality);

		public int SetAcousticModel(AcousticModel model)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_SetAcousticModel(this.context, model);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ResetSharedReverb(IntPtr context);

		public int ResetReverb()
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ResetSharedReverb(this.context);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);

		public int SetEnabled(int feature, bool enabled)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_Enable(IntPtr context, EnableFlagInternal what, int enable);

		public int SetEnabled(EnableFlagInternal feature, bool enabled)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_CreateAudioGeometry(this.context, out geometry);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_DestroyAudioGeometry(geometry);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, int enabled);

		public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
		{
			if (this.version < 94)
			{
				return -1;
			}
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometrySetObjectFlag(geometry, flag, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, MetaXRAcousticNativeInterface.ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, MetaXRAcousticNativeInterface.ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount);

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryUploadMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)((ulong)((long)vertexCount)), UIntPtr.Zero, MetaXRAcousticNativeInterface.ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)((ulong)((long)indexCount)), MetaXRAcousticNativeInterface.ovrAudioScalarType.UInt32, groups, (UIntPtr)((ulong)((long)groupCount)));
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, MetaXRAcousticNativeInterface.ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, MetaXRAcousticNativeInterface.ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount, ref MeshSimplification simplification);

		public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount, ref MeshSimplification simplification)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)((ulong)((long)vertexCount)), UIntPtr.Zero, MetaXRAcousticNativeInterface.ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)((ulong)((long)indexCount)), MetaXRAcousticNativeInterface.ovrAudioScalarType.UInt32, groups, (UIntPtr)((ulong)((long)groupCount)), ref simplification);
		}

		[DllImport("MetaXRAudioWwise")]
		private unsafe static extern int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float* matrix4x4);

		public unsafe int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometrySetTransform(geometry, ptr);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength);

		public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryReadMeshMemory(geometry, data, dataLength);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, IntPtr unused1, out uint numVertices, IntPtr unused2, IntPtr unused3, out uint numTriangles);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, float[] vertices, ref uint numVertices, uint[] indices, uint[] materialIndices, ref uint numTriangles);

		public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
		{
			uint num2;
			uint num3;
			int num = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, IntPtr.Zero, out num2, IntPtr.Zero, IntPtr.Zero, out num3);
			if (num != 0)
			{
				Debug.LogError("unexpected error getting simplified mesh array sizes");
				vertices = null;
				indices = null;
				materialIndices = null;
				return num;
			}
			vertices = new float[num2 * 3U];
			indices = new uint[num3 * 3U];
			materialIndices = new uint[num3];
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, vertices, ref num2, indices, materialIndices, ref num3);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);

		public int CreateAudioMaterial(out IntPtr material)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_CreateAudioMaterial(this.context, out material);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);

		public int DestroyAudioMaterial(IntPtr material)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_DestroyAudioMaterial(material);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioMaterialReset(material, property);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_CreateAudioSceneIR(IntPtr context, out IntPtr sceneIR);

		public int CreateAudioSceneIR(out IntPtr sceneIR)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_CreateAudioSceneIR(this.context, out sceneIR);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_DestroyAudioSceneIR(IntPtr sceneIR);

		public int DestroyAudioSceneIR(IntPtr sceneIR)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_DestroyAudioSceneIR(sceneIR);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRSetEnabled(IntPtr sceneIR, int enabled);

		public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRSetEnabled(sceneIR, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRGetEnabled(IntPtr sceneIR, out int enabled);

		public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
		{
			int num;
			int result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRGetEnabled(sceneIR, out num);
			enabled = (num != 0);
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);

		public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRGetStatus(sceneIR, out status);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_InitializeAudioSceneIRParameters(out MapParameters parameters);

		public int InitializeAudioSceneIRParameters(out MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_InitializeAudioSceneIRParameters(out parameters);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);

		public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRCompute(sceneIR, ref parameters);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters);

		public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRComputeCustomPoints(sceneIR, points, pointCount, ref parameters);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);

		public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRGetPointCount(sceneIR, out pointCount);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);

		public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRGetPoints(sceneIR, points, maxPointCount);
		}

		[DllImport("MetaXRAudioWwise")]
		private unsafe static extern int ovrAudio_AudioSceneIRSetTransform(IntPtr sceneIR, float* matrix4x4);

		public unsafe int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRSetTransform(sceneIR, ptr);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);

		public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRGetTransform(sceneIR, out matrix4x4);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);

		public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRWriteFile(sceneIR, filePath);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRReadFile(IntPtr sceneIR, string filePath);

		public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRReadFile(sceneIR, filePath);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength);

		public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength)
		{
			return MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_AudioSceneIRReadMemory(sceneIR, data, dataLength);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_CreateControlZone(IntPtr context, out IntPtr control);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_CreateControlVolume(IntPtr context, out IntPtr control);

		public int CreateControlZone(out IntPtr control)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_CreateControlZone(this.context, out control);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_CreateControlVolume(this.context, out control);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_DestroyControlZone(IntPtr control);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_DestroyControlVolume(IntPtr control);

		public int DestroyControlZone(IntPtr control)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_DestroyControlZone(control);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_DestroyControlVolume(control);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneSetEnabled(IntPtr control, int enabled);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeSetEnabled(IntPtr control, int enabled);

		public int ControlZoneSetEnabled(IntPtr control, bool enabled)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneSetEnabled(control, enabled ? 1 : 0);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeSetEnabled(control, enabled ? 1 : 0);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneGetEnabled(IntPtr control, out int enabled);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeGetEnabled(IntPtr control, out int enabled);

		public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
		{
			int num = 0;
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneGetEnabled(control, out num);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeGetEnabled(control, out num);
			}
			enabled = (num != 0);
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private unsafe static extern int ovrAudio_ControlZoneSetTransform(IntPtr control, float* matrix4x4);

		[DllImport("MetaXRAudioWwise")]
		private unsafe static extern int ovrAudio_ControlVolumeSetTransform(IntPtr control, float* matrix4x4);

		public unsafe int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneSetTransform(control, ptr);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeSetTransform(control, ptr);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeGetTransform(IntPtr control, out float[] matrix4x4);

		public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneGetTransform(control, out matrix4x4);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeGetTransform(control, out matrix4x4);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneSetBox(control, sizeX, sizeY, sizeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeSetBox(control, sizeX, sizeY, sizeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneGetBox(control, out sizeX, out sizeY, out sizeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeGetBox(control, out sizeX, out sizeY, out sizeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneSetFadeDistance(control, fadeX, fadeY, fadeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeSetFadeDistance(control, fadeX, fadeY, fadeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneSetFrequency(control, property, frequency, value);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeSetFrequency(control, property, frequency, value);
			}
			return result;
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlZoneReset(IntPtr control, ControlZoneProperty property);

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_ControlVolumeReset(IntPtr control, ControlZoneProperty property);

		public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlZoneReset(control, property);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.WwisePluginInterface.ovrAudio_ControlVolumeReset(control, property);
			}
			return result;
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			return this.AudioGeometrySetTransform(geometry, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			return this.AudioSceneIRSetTransform(sceneIR, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			return this.ControlZoneSetTransform(control, matrix);
		}

		public const string binaryName = "MetaXRAudioWwise";

		private IntPtr context_ = IntPtr.Zero;

		private int version;
	}

	public class FMODPluginInterface : MetaXRAcousticNativeInterface.INativeInterface
	{
		private IntPtr context
		{
			get
			{
				if (this.context_ == IntPtr.Zero)
				{
					MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_GetPluginContext(out this.context_);
					int num;
					int num2;
					MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_GetVersion(out num, out this.version, out num2);
				}
				return this.context_;
			}
		}

		[DllImport("MetaXRAudioFMOD")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context);

		[DllImport("MetaXRAudioFMOD")]
		public static extern IntPtr ovrAudio_GetVersion(out int Major, out int Minor, out int Patch);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetAcousticModel(IntPtr context, AcousticModel quality);

		public int SetAcousticModel(AcousticModel model)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_SetAcousticModel(this.context, model);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ResetSharedReverb(IntPtr context);

		public int ResetReverb()
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ResetSharedReverb(this.context);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);

		public int SetEnabled(int feature, bool enabled)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_Enable(IntPtr context, EnableFlagInternal what, int enable);

		public int SetEnabled(EnableFlagInternal feature, bool enabled)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_CreateAudioGeometry(this.context, out geometry);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_DestroyAudioGeometry(geometry);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, int enabled);

		public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
		{
			if (this.version < 94)
			{
				return -1;
			}
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometrySetObjectFlag(geometry, flag, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, MetaXRAcousticNativeInterface.ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, MetaXRAcousticNativeInterface.ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount);

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryUploadMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)((ulong)((long)vertexCount)), UIntPtr.Zero, MetaXRAcousticNativeInterface.ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)((ulong)((long)indexCount)), MetaXRAcousticNativeInterface.ovrAudioScalarType.UInt32, groups, (UIntPtr)((ulong)((long)groupCount)));
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, MetaXRAcousticNativeInterface.ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, MetaXRAcousticNativeInterface.ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount, ref MeshSimplification simplification);

		public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount, ref MeshSimplification simplification)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)((ulong)((long)vertexCount)), UIntPtr.Zero, MetaXRAcousticNativeInterface.ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)((ulong)((long)indexCount)), MetaXRAcousticNativeInterface.ovrAudioScalarType.UInt32, groups, (UIntPtr)((ulong)((long)groupCount)), ref simplification);
		}

		[DllImport("MetaXRAudioFMOD")]
		private unsafe static extern int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float* matrix4x4);

		public unsafe int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometrySetTransform(geometry, ptr);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength);

		public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryReadMeshMemory(geometry, data, dataLength);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, IntPtr unused1, out uint numVertices, IntPtr unused2, IntPtr unused3, out uint numTriangles);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, float[] vertices, ref uint numVertices, uint[] indices, uint[] materialIndices, ref uint numTriangles);

		public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
		{
			uint num2;
			uint num3;
			int num = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, IntPtr.Zero, out num2, IntPtr.Zero, IntPtr.Zero, out num3);
			if (num != 0)
			{
				Debug.LogError("unexpected error getting simplified mesh array sizes");
				vertices = null;
				indices = null;
				materialIndices = null;
				return num;
			}
			vertices = new float[num2 * 3U];
			indices = new uint[num3 * 3U];
			materialIndices = new uint[num3];
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, vertices, ref num2, indices, materialIndices, ref num3);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);

		public int CreateAudioMaterial(out IntPtr material)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_CreateAudioMaterial(this.context, out material);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);

		public int DestroyAudioMaterial(IntPtr material)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_DestroyAudioMaterial(material);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioMaterialReset(material, property);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_CreateAudioSceneIR(IntPtr context, out IntPtr sceneIR);

		public int CreateAudioSceneIR(out IntPtr sceneIR)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_CreateAudioSceneIR(this.context, out sceneIR);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_DestroyAudioSceneIR(IntPtr sceneIR);

		public int DestroyAudioSceneIR(IntPtr sceneIR)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_DestroyAudioSceneIR(sceneIR);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRSetEnabled(IntPtr sceneIR, int enabled);

		public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRSetEnabled(sceneIR, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRGetEnabled(IntPtr sceneIR, out int enabled);

		public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
		{
			int num;
			int result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRGetEnabled(sceneIR, out num);
			enabled = (num != 0);
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);

		public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRGetStatus(sceneIR, out status);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_InitializeAudioSceneIRParameters(out MapParameters parameters);

		public int InitializeAudioSceneIRParameters(out MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_InitializeAudioSceneIRParameters(out parameters);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);

		public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRCompute(sceneIR, ref parameters);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters);

		public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRComputeCustomPoints(sceneIR, points, pointCount, ref parameters);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);

		public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRGetPointCount(sceneIR, out pointCount);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);

		public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRGetPoints(sceneIR, points, maxPointCount);
		}

		[DllImport("MetaXRAudioFMOD")]
		private unsafe static extern int ovrAudio_AudioSceneIRSetTransform(IntPtr sceneIR, float* matrix4x4);

		public unsafe int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRSetTransform(sceneIR, ptr);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);

		public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRGetTransform(sceneIR, out matrix4x4);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);

		public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRWriteFile(sceneIR, filePath);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRReadFile(IntPtr sceneIR, string filePath);

		public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRReadFile(sceneIR, filePath);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength);

		public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength)
		{
			return MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_AudioSceneIRReadMemory(sceneIR, data, dataLength);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_CreateControlZone(IntPtr context, out IntPtr control);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_CreateControlVolume(IntPtr context, out IntPtr control);

		public int CreateControlZone(out IntPtr control)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_CreateControlZone(this.context, out control);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_CreateControlVolume(this.context, out control);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_DestroyControlZone(IntPtr control);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_DestroyControlVolume(IntPtr control);

		public int DestroyControlZone(IntPtr control)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_DestroyControlZone(control);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_DestroyControlVolume(control);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneSetEnabled(IntPtr control, int enabled);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeSetEnabled(IntPtr control, int enabled);

		public int ControlZoneSetEnabled(IntPtr control, bool enabled)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneSetEnabled(control, enabled ? 1 : 0);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeSetEnabled(control, enabled ? 1 : 0);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneGetEnabled(IntPtr control, out int enabled);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeGetEnabled(IntPtr control, out int enabled);

		public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
		{
			int num = 0;
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneGetEnabled(control, out num);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeGetEnabled(control, out num);
			}
			enabled = (num != 0);
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private unsafe static extern int ovrAudio_ControlZoneSetTransform(IntPtr control, float* matrix4x4);

		[DllImport("MetaXRAudioFMOD")]
		private unsafe static extern int ovrAudio_ControlVolumeSetTransform(IntPtr control, float* matrix4x4);

		public unsafe int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			float* ptr = stackalloc float[(UIntPtr)64];
			*ptr = matrix.m00;
			ptr[1] = matrix.m10;
			ptr[2] = -matrix.m20;
			ptr[3] = matrix.m30;
			ptr[4] = matrix.m01;
			ptr[5] = matrix.m11;
			ptr[6] = -matrix.m21;
			ptr[7] = matrix.m31;
			ptr[8] = matrix.m02;
			ptr[9] = matrix.m12;
			ptr[10] = -matrix.m22;
			ptr[11] = matrix.m32;
			ptr[12] = matrix.m03;
			ptr[13] = matrix.m13;
			ptr[14] = -matrix.m23;
			ptr[15] = matrix.m33;
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneSetTransform(control, ptr);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeSetTransform(control, ptr);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeGetTransform(IntPtr control, out float[] matrix4x4);

		public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneGetTransform(control, out matrix4x4);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeGetTransform(control, out matrix4x4);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

		public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneSetBox(control, sizeX, sizeY, sizeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeSetBox(control, sizeX, sizeY, sizeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

		public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneGetBox(control, out sizeX, out sizeY, out sizeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeGetBox(control, out sizeX, out sizeY, out sizeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

		public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneSetFadeDistance(control, fadeX, fadeY, fadeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeSetFadeDistance(control, fadeX, fadeY, fadeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

		public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

		public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneSetFrequency(control, property, frequency, value);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeSetFrequency(control, property, frequency, value);
			}
			return result;
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlZoneReset(IntPtr control, ControlZoneProperty property);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_ControlVolumeReset(IntPtr control, ControlZoneProperty property);

		public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
		{
			int result;
			try
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlZoneReset(control, property);
			}
			catch
			{
				result = MetaXRAcousticNativeInterface.FMODPluginInterface.ovrAudio_ControlVolumeReset(control, property);
			}
			return result;
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			return this.AudioGeometrySetTransform(geometry, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			return this.AudioSceneIRSetTransform(sceneIR, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			return this.ControlZoneSetTransform(control, matrix);
		}

		public const string binaryName = "MetaXRAudioFMOD";

		private IntPtr context_ = IntPtr.Zero;

		private int version;
	}

	public class DummyInterface : MetaXRAcousticNativeInterface.INativeInterface
	{
		public int SetAcousticModel(AcousticModel model)
		{
			return -1;
		}

		public int ResetReverb()
		{
			return -1;
		}

		public int SetEnabled(int feature, bool enabled)
		{
			return -1;
		}

		public int SetEnabled(EnableFlagInternal feature, bool enabled)
		{
			return -1;
		}

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			geometry = IntPtr.Zero;
			return -1;
		}

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return -1;
		}

		public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
		{
			return -1;
		}

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return -1;
		}

		public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount, ref MeshSimplification simplification)
		{
			return -1;
		}

		public int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			return -1;
		}

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			matrix4x4 = null;
			return -1;
		}

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return -1;
		}

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return -1;
		}

		public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, ulong dataLength)
		{
			return -1;
		}

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return -1;
		}

		public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
		{
			vertices = null;
			indices = null;
			materialIndices = null;
			return -1;
		}

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			value = 0f;
			return -1;
		}

		public int CreateAudioMaterial(out IntPtr material)
		{
			material = IntPtr.Zero;
			return -1;
		}

		public int DestroyAudioMaterial(IntPtr material)
		{
			return -1;
		}

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return -1;
		}

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return -1;
		}

		public int CreateAudioSceneIR(out IntPtr sceneIR)
		{
			sceneIR = IntPtr.Zero;
			return -1;
		}

		public int DestroyAudioSceneIR(IntPtr sceneIR)
		{
			return -1;
		}

		public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
		{
			return -1;
		}

		public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
		{
			enabled = false;
			return -1;
		}

		public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
		{
			status = AcousticMapStatus.EMPTY;
			return -1;
		}

		public int InitializeAudioSceneIRParameters(out MapParameters parameters)
		{
			parameters = default(MapParameters);
			return -1;
		}

		public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
		{
			return -1;
		}

		public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR, float[] points, UIntPtr pointCount, ref MapParameters parameters)
		{
			return -1;
		}

		public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
		{
			pointCount = UIntPtr.Zero;
			return -1;
		}

		public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
		{
			return -1;
		}

		public int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			return -1;
		}

		public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
		{
			matrix4x4 = new float[16];
			return -1;
		}

		public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
		{
			return -1;
		}

		public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
		{
			return -1;
		}

		public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, ulong dataLength)
		{
			return -1;
		}

		public int CreateControlZone(out IntPtr control)
		{
			control = IntPtr.Zero;
			return -1;
		}

		public int DestroyControlZone(IntPtr control)
		{
			return -1;
		}

		public int ControlZoneSetEnabled(IntPtr control, bool enabled)
		{
			return -1;
		}

		public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
		{
			enabled = false;
			return -1;
		}

		public int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			return -1;
		}

		public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
		{
			matrix4x4 = new float[16];
			return -1;
		}

		public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
		{
			return -1;
		}

		public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
		{
			sizeX = 0f;
			sizeY = 0f;
			sizeZ = 0f;
			return -1;
		}

		public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
		{
			return -1;
		}

		public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
		{
			fadeX = 0f;
			fadeY = 0f;
			fadeZ = 0f;
			return -1;
		}

		public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
		{
			return -1;
		}

		public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
		{
			return -1;
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
		{
			return this.AudioGeometrySetTransform(geometry, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
		{
			return this.AudioSceneIRSetTransform(sceneIR, matrix);
		}

		int MetaXRAcousticNativeInterface.INativeInterface.ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
		{
			return this.ControlZoneSetTransform(control, matrix);
		}
	}
}
