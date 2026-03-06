using System;
using Meta.XR.Acoustics;
using UnityEngine;

public class MetaXRAcousticSettings : ScriptableObject
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoadRuntimeMethod()
	{
		MetaXRAcousticSettings.Instance.ApplyAllSettings();
	}

	public AcousticModel AcousticModel
	{
		get
		{
			return this.acousticModel;
		}
		set
		{
			if (value != this.acousticModel)
			{
				this.acousticModel = value;
				MetaXRAcousticNativeInterface.Interface.SetAcousticModel(value);
			}
		}
	}

	internal bool DiffractionEnabled
	{
		get
		{
			return this.diffractionEnabled;
		}
		set
		{
			if (value != this.diffractionEnabled)
			{
				this.diffractionEnabled = value;
				MetaXRAcousticNativeInterface.Interface.SetEnabled(EnableFlagInternal.DIFFRACTION, value);
			}
		}
	}

	internal string[] ExcludeTags
	{
		get
		{
			return this.excludeTags;
		}
		set
		{
			this.excludeTags = value;
		}
	}

	[Tooltip("If enabled, acoustic geometry files will also be written when baking an acoustic map")]
	internal bool MapBakeWriteGeo
	{
		get
		{
			return this.mapBakeWriteGeo;
		}
		set
		{
			this.mapBakeWriteGeo = value;
		}
	}

	internal void ApplyAllSettings()
	{
		Debug.Log("Applying Acoustic Propagation Settings: " + string.Format("[acoustic model = {0}], ", this.AcousticModel) + string.Format("[diffraction = {0}], ", this.DiffractionEnabled));
		MetaXRAcousticNativeInterface.Interface.SetAcousticModel(this.AcousticModel);
		MetaXRAcousticNativeInterface.Interface.SetEnabled(EnableFlagInternal.DIFFRACTION, this.DiffractionEnabled);
	}

	public static MetaXRAcousticSettings Instance
	{
		get
		{
			if (MetaXRAcousticSettings.instance == null)
			{
				MetaXRAcousticSettings.instance = Resources.Load<MetaXRAcousticSettings>("MetaXRAcousticSettings");
				if (MetaXRAcousticSettings.instance == null)
				{
					MetaXRAcousticSettings.instance = ScriptableObject.CreateInstance<MetaXRAcousticSettings>();
				}
			}
			return MetaXRAcousticSettings.instance;
		}
	}

	[Tooltip("This is the path inside your Unity project which will store all baked geometry files.")]
	internal const string AcousticFileRootDir = "StreamingAssets/Acoustics";

	[SerializeField]
	[Tooltip("Select which type of acoustic modeling system is used to generate reverb and reflections.")]
	private AcousticModel acousticModel = AcousticModel.Automatic;

	[SerializeField]
	[Tooltip("When enabled and using geometry, all spatailized AudioSources will diffract (propagate around corners and obstructions)")]
	private bool diffractionEnabled = true;

	[SerializeField]
	[Tooltip("Geometry will exclude children with these tags")]
	private string[] excludeTags = new string[0];

	[SerializeField]
	[Tooltip("When you bake an acoustic map, also bake all the acoustic geometry files")]
	private bool mapBakeWriteGeo = true;

	private static MetaXRAcousticSettings instance;
}
