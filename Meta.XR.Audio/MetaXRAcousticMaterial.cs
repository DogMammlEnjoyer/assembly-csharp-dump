using System;
using System.Linq;
using Meta.XR.Acoustics;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MetaXRAcousticMaterial : MonoBehaviour, IMaterialDataProvider
{
	internal MetaXRAcousticMaterialProperties Properties
	{
		get
		{
			return this.properties;
		}
		set
		{
			this.properties = value;
		}
	}

	public MaterialData Data
	{
		get
		{
			if (this.hasCustomData)
			{
				return this.customData;
			}
			MetaXRAcousticMaterialProperties metaXRAcousticMaterialProperties = this.properties;
			if (metaXRAcousticMaterialProperties == null)
			{
				return null;
			}
			return metaXRAcousticMaterialProperties.Data;
		}
	}

	internal Color Color
	{
		get
		{
			if (this.Data == null)
			{
				return Color.magenta;
			}
			return this.Data.color;
		}
	}

	internal void CopyPresetToCustomData(MetaXRAcousticMaterialProperties.BuiltinPreset preset)
	{
		if (!this.hasCustomData)
		{
			Debug.LogError("Material doesn't have custom data", base.gameObject);
			return;
		}
		MetaXRAcousticMaterialProperties.SetPreset(preset, ref this.customData);
	}

	private void Start()
	{
		if (!base.gameObject.isStatic)
		{
			this.StartInternal();
		}
	}

	internal bool StartInternal()
	{
		if (this.materialHandle != IntPtr.Zero)
		{
			return true;
		}
		this.materialHandle = MetaXRAcousticMaterial.CreateMaterialNativeHandle(this.Data);
		return true;
	}

	private void OnDestroy()
	{
		this.DestroyInternal();
	}

	internal void DestroyInternal()
	{
		if (this.materialHandle != IntPtr.Zero)
		{
			MetaXRAcousticMaterial.DestroyMaterialNativeHandle(this.materialHandle);
			this.materialHandle = IntPtr.Zero;
		}
	}

	internal bool ApplyMaterialProperties()
	{
		return MetaXRAcousticMaterial.ApplyPropertiesToNative(this.materialHandle, this.Data);
	}

	internal static IntPtr CreateMaterialNativeHandle(MaterialData data = null)
	{
		IntPtr zero = IntPtr.Zero;
		if (MetaXRAcousticNativeInterface.Interface.CreateAudioMaterial(out zero) != 0)
		{
			Debug.LogError("Unable to create internal audio material");
			return zero;
		}
		if (data != null)
		{
			MetaXRAcousticMaterial.ApplyPropertiesToNative(zero, data);
		}
		return zero;
	}

	internal static void DestroyMaterialNativeHandle(IntPtr handle)
	{
		MetaXRAcousticNativeInterface.Interface.DestroyAudioMaterial(handle);
	}

	private static bool ApplyPropertiesToNative(IntPtr handle, MaterialData data)
	{
		return MetaXRAcousticMaterial.ApplyPropertiesToNative(handle, data, null);
	}

	private static bool ApplyPropertiesToNative(IntPtr handle, MaterialData data, GameObject gameObject)
	{
		if (handle == IntPtr.Zero || data == null)
		{
			if (gameObject != null)
			{
				Scene scene = gameObject.scene;
				string str = gameObject.scene.name + ":" + string.Join("/", (from t in gameObject.GetComponentsInParent<Transform>()
				select t.name).Reverse<string>().ToArray<string>());
				Debug.LogWarning("Acoustic Material configured with empty properties: " + str, gameObject);
			}
			return false;
		}
		MetaXRAcousticNativeInterface.Interface.AudioMaterialReset(handle, MaterialProperty.ABSORPTION);
		foreach (Spectrum.Point point in data.absorption.points)
		{
			MetaXRAcousticNativeInterface.Interface.AudioMaterialSetFrequency(handle, MaterialProperty.ABSORPTION, point.frequency, point.data);
		}
		MetaXRAcousticNativeInterface.Interface.AudioMaterialReset(handle, MaterialProperty.TRANSMISSION);
		foreach (Spectrum.Point point2 in data.transmission.points)
		{
			MetaXRAcousticNativeInterface.Interface.AudioMaterialSetFrequency(handle, MaterialProperty.TRANSMISSION, point2.frequency, point2.data);
		}
		MetaXRAcousticNativeInterface.Interface.AudioMaterialReset(handle, MaterialProperty.SCATTERING);
		foreach (Spectrum.Point point3 in data.scattering.points)
		{
			MetaXRAcousticNativeInterface.Interface.AudioMaterialSetFrequency(handle, MaterialProperty.SCATTERING, point3.frequency, point3.data);
		}
		return true;
	}

	string IMaterialDataProvider.get_name()
	{
		return base.name;
	}

	[SerializeField]
	private MetaXRAcousticMaterialProperties properties;

	[SerializeField]
	private bool hasCustomData;

	[SerializeField]
	internal MaterialData customData;

	[NonSerialized]
	private IntPtr materialHandle = IntPtr.Zero;
}
