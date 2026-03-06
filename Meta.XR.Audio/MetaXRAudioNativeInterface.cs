using System;
using System.Runtime.InteropServices;
using Meta.XR.Audio;
using UnityEngine;

public class MetaXRAudioNativeInterface
{
	public static MetaXRAudioNativeInterface.NativeInterface Interface
	{
		get
		{
			if (MetaXRAudioNativeInterface.CachedInterface == null)
			{
				MetaXRAudioNativeInterface.CachedInterface = MetaXRAudioNativeInterface.FindInterface();
			}
			return MetaXRAudioNativeInterface.CachedInterface;
		}
	}

	private static MetaXRAudioNativeInterface.NativeInterface FindInterface()
	{
		try
		{
			IntPtr orCreateGlobalOvrAudioContext = MetaXRAudioNativeInterface.WwisePluginInterface.getOrCreateGlobalOvrAudioContext();
			Debug.Log("Meta XR Audio Native Interface initialized with Wwise plugin");
			return new MetaXRAudioNativeInterface.WwisePluginInterface();
		}
		catch (DllNotFoundException)
		{
		}
		try
		{
			IntPtr orCreateGlobalOvrAudioContext;
			MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_GetPluginContext(out orCreateGlobalOvrAudioContext);
			Debug.Log("Meta XR Audio Native Interface initialized with FMOD plugin");
			return new MetaXRAudioNativeInterface.FMODPluginInterface();
		}
		catch (DllNotFoundException)
		{
		}
		Debug.Log("Meta XR Audio Native Interface initialized with Unity plugin");
		return new MetaXRAudioNativeInterface.UnityNativeInterface();
	}

	private static MetaXRAudioNativeInterface.NativeInterface CachedInterface;

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

	public interface NativeInterface
	{
		int SetAdvancedBoxRoomParameters(float width, float height, float depth, bool lockToListenerPosition, Vector3 position, float[] wallMaterials);

		int SetSharedReverbWetLevel(float linearLevel);

		int SetEnabled(int feature, bool enabled);

		int SetRoomClutterFactor(float[] clutterFactor);

		int SetDynamicRoomRaysPerSecond(int RaysPerSecond);

		int SetDynamicRoomInterpSpeed(float InterpSpeed);

		int SetDynamicRoomMaxWallDistance(float MaxWallDistance);

		int SetDynamicRoomRaysRayCacheSize(int RayCacheSize);

		int GetRoomDimensions(float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position);

		int GetRaycastHits(Vector3[] points, Vector3[] normals, int length);
	}

	public class UnityNativeInterface : MetaXRAudioNativeInterface.NativeInterface
	{
		private IntPtr context
		{
			get
			{
				if (this.context_ == IntPtr.Zero)
				{
					MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_GetPluginContext(out this.context_);
				}
				return this.context_;
			}
		}

		[DllImport("MetaXRAudioUnity")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context);

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetAdvancedBoxRoomParametersUnity(IntPtr context, float width, float height, float depth, bool lockToListenerPosition, float positionX, float positionY, float positionZ, float[] wallMaterials);

		public int SetAdvancedBoxRoomParameters(float width, float height, float depth, bool lockToListenerPosition, Vector3 position, float[] wallMaterials)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetAdvancedBoxRoomParametersUnity(this.context, width, height, depth, lockToListenerPosition, position.x, position.y, -position.z, wallMaterials);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetRoomClutterFactor(IntPtr context, float[] clutterFactor);

		public int SetRoomClutterFactor(float[] clutterFactor)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetRoomClutterFactor(this.context, clutterFactor);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetSharedReverbWetLevel(IntPtr context, float linearLevel);

		public int SetSharedReverbWetLevel(float linearLevel)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetSharedReverbWetLevel(this.context, linearLevel);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);

		public int SetEnabled(int feature, bool enabled)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_Enable(IntPtr context, EnableFlag what, int enable);

		public int SetEnabled(EnableFlag feature, bool enabled)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetDynamicRoomRaysPerSecond(IntPtr context, int RaysPerSecond);

		public int SetDynamicRoomRaysPerSecond(int RaysPerSecond)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetDynamicRoomRaysPerSecond(this.context, RaysPerSecond);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetDynamicRoomInterpSpeed(IntPtr context, float InterpSpeed);

		public int SetDynamicRoomInterpSpeed(float InterpSpeed)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetDynamicRoomInterpSpeed(this.context, InterpSpeed);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetDynamicRoomMaxWallDistance(IntPtr context, float MaxWallDistance);

		public int SetDynamicRoomMaxWallDistance(float MaxWallDistance)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetDynamicRoomMaxWallDistance(this.context, MaxWallDistance);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_SetDynamicRoomRaysRayCacheSize(IntPtr context, int RayCacheSize);

		public int SetDynamicRoomRaysRayCacheSize(int RayCacheSize)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_SetDynamicRoomRaysRayCacheSize(this.context, RayCacheSize);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_GetRoomDimensions(IntPtr context, float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position);

		public int GetRoomDimensions(float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_GetRoomDimensions(this.context, roomDimensions, reflectionsCoefs, out position);
		}

		[DllImport("MetaXRAudioUnity")]
		private static extern int ovrAudio_GetRaycastHits(IntPtr context, Vector3[] points, Vector3[] normals, int length);

		public int GetRaycastHits(Vector3[] points, Vector3[] normals, int length)
		{
			return MetaXRAudioNativeInterface.UnityNativeInterface.ovrAudio_GetRaycastHits(this.context, points, normals, length);
		}

		public const string binaryName = "MetaXRAudioUnity";

		private IntPtr context_ = IntPtr.Zero;
	}

	public class WwisePluginInterface : MetaXRAudioNativeInterface.NativeInterface
	{
		private IntPtr context
		{
			get
			{
				if (this.context_ == IntPtr.Zero)
				{
					this.context_ = MetaXRAudioNativeInterface.WwisePluginInterface.getOrCreateGlobalOvrAudioContext();
				}
				return this.context_;
			}
		}

		[DllImport("MetaXRAudioWwise")]
		public static extern IntPtr getOrCreateGlobalOvrAudioContext();

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetAdvancedBoxRoomParametersUnity(IntPtr context, float width, float height, float depth, bool lockToListenerPosition, float positionX, float positionY, float positionZ, float[] wallMaterials);

		public int SetAdvancedBoxRoomParameters(float width, float height, float depth, bool lockToListenerPosition, Vector3 position, float[] wallMaterials)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetAdvancedBoxRoomParametersUnity(this.context, width, height, depth, lockToListenerPosition, position.x, position.y, -position.z, wallMaterials);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetRoomClutterFactor(IntPtr context, float[] clutterFactor);

		public int SetRoomClutterFactor(float[] clutterFactor)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetRoomClutterFactor(this.context, clutterFactor);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetSharedReverbWetLevel(IntPtr context, float linearLevel);

		public int SetSharedReverbWetLevel(float linearLevel)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetSharedReverbWetLevel(this.context, linearLevel);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);

		public int SetEnabled(int feature, bool enabled)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_Enable(IntPtr context, EnableFlag what, int enable);

		public int SetEnabled(EnableFlag feature, bool enabled)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetDynamicRoomRaysPerSecond(IntPtr context, int RaysPerSecond);

		public int SetDynamicRoomRaysPerSecond(int RaysPerSecond)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetDynamicRoomRaysPerSecond(this.context, RaysPerSecond);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetDynamicRoomInterpSpeed(IntPtr context, float InterpSpeed);

		public int SetDynamicRoomInterpSpeed(float InterpSpeed)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetDynamicRoomInterpSpeed(this.context, InterpSpeed);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetDynamicRoomMaxWallDistance(IntPtr context, float MaxWallDistance);

		public int SetDynamicRoomMaxWallDistance(float MaxWallDistance)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetDynamicRoomMaxWallDistance(this.context, MaxWallDistance);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_SetDynamicRoomRaysRayCacheSize(IntPtr context, int RayCacheSize);

		public int SetDynamicRoomRaysRayCacheSize(int RayCacheSize)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_SetDynamicRoomRaysRayCacheSize(this.context, RayCacheSize);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_GetRoomDimensions(IntPtr context, float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position);

		public int GetRoomDimensions(float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_GetRoomDimensions(this.context, roomDimensions, reflectionsCoefs, out position);
		}

		[DllImport("MetaXRAudioWwise")]
		private static extern int ovrAudio_GetRaycastHits(IntPtr context, Vector3[] points, Vector3[] normals, int length);

		public int GetRaycastHits(Vector3[] points, Vector3[] normals, int length)
		{
			return MetaXRAudioNativeInterface.WwisePluginInterface.ovrAudio_GetRaycastHits(this.context, points, normals, length);
		}

		public const string binaryName = "MetaXRAudioWwise";

		private IntPtr context_ = IntPtr.Zero;
	}

	public class FMODPluginInterface : MetaXRAudioNativeInterface.NativeInterface
	{
		private IntPtr context
		{
			get
			{
				if (this.context_ == IntPtr.Zero)
				{
					MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_GetPluginContext(out this.context_);
				}
				return this.context_;
			}
		}

		[DllImport("MetaXRAudioFMOD")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context);

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetAdvancedBoxRoomParametersUnity(IntPtr context, float width, float height, float depth, bool lockToListenerPosition, float positionX, float positionY, float positionZ, float[] wallMaterials);

		public int SetAdvancedBoxRoomParameters(float width, float height, float depth, bool lockToListenerPosition, Vector3 position, float[] wallMaterials)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetAdvancedBoxRoomParametersUnity(this.context, width, height, depth, lockToListenerPosition, position.x, position.y, -position.z, wallMaterials);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetRoomClutterFactor(IntPtr context, float[] clutterFactor);

		public int SetRoomClutterFactor(float[] clutterFactor)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetRoomClutterFactor(this.context, clutterFactor);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetSharedReverbWetLevel(IntPtr context, float linearLevel);

		public int SetSharedReverbWetLevel(float linearLevel)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetSharedReverbWetLevel(this.context, linearLevel);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);

		public int SetEnabled(int feature, bool enabled)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_Enable(IntPtr context, EnableFlag what, int enable);

		public int SetEnabled(EnableFlag feature, bool enabled)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_Enable(this.context, feature, enabled ? 1 : 0);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetDynamicRoomRaysPerSecond(IntPtr context, int RaysPerSecond);

		public int SetDynamicRoomRaysPerSecond(int RaysPerSecond)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetDynamicRoomRaysPerSecond(this.context, RaysPerSecond);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetDynamicRoomInterpSpeed(IntPtr context, float InterpSpeed);

		public int SetDynamicRoomInterpSpeed(float InterpSpeed)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetDynamicRoomInterpSpeed(this.context, InterpSpeed);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetDynamicRoomMaxWallDistance(IntPtr context, float MaxWallDistance);

		public int SetDynamicRoomMaxWallDistance(float MaxWallDistance)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetDynamicRoomMaxWallDistance(this.context, MaxWallDistance);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_SetDynamicRoomRaysRayCacheSize(IntPtr context, int RayCacheSize);

		public int SetDynamicRoomRaysRayCacheSize(int RayCacheSize)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_SetDynamicRoomRaysRayCacheSize(this.context, RayCacheSize);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_GetRoomDimensions(IntPtr context, float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position);

		public int GetRoomDimensions(float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_GetRoomDimensions(this.context, roomDimensions, reflectionsCoefs, out position);
		}

		[DllImport("MetaXRAudioFMOD")]
		private static extern int ovrAudio_GetRaycastHits(IntPtr context, Vector3[] points, Vector3[] normals, int length);

		public int GetRaycastHits(Vector3[] points, Vector3[] normals, int length)
		{
			return MetaXRAudioNativeInterface.FMODPluginInterface.ovrAudio_GetRaycastHits(this.context, points, normals, length);
		}

		public const string binaryName = "MetaXRAudioFMOD";

		private IntPtr context_ = IntPtr.Zero;
	}
}
