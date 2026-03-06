using System;
using System.IO;
using UnityEngine;

public class OVRMixedRealityCaptureSettings : ScriptableObject, OVRMixedRealityCaptureConfiguration
{
	bool OVRMixedRealityCaptureConfiguration.enableMixedReality
	{
		get
		{
			return this.enableMixedReality;
		}
		set
		{
			this.enableMixedReality = value;
		}
	}

	LayerMask OVRMixedRealityCaptureConfiguration.extraHiddenLayers
	{
		get
		{
			return this.extraHiddenLayers;
		}
		set
		{
			this.extraHiddenLayers = value;
		}
	}

	LayerMask OVRMixedRealityCaptureConfiguration.extraVisibleLayers
	{
		get
		{
			return this.extraVisibleLayers;
		}
		set
		{
			this.extraVisibleLayers = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.dynamicCullingMask
	{
		get
		{
			return this.dynamicCullingMask;
		}
		set
		{
			this.dynamicCullingMask = value;
		}
	}

	OVRManager.CompositionMethod OVRMixedRealityCaptureConfiguration.compositionMethod
	{
		get
		{
			return this.compositionMethod;
		}
		set
		{
			this.compositionMethod = value;
		}
	}

	Color OVRMixedRealityCaptureConfiguration.externalCompositionBackdropColorRift
	{
		get
		{
			return this.externalCompositionBackdropColorRift;
		}
		set
		{
			this.externalCompositionBackdropColorRift = value;
		}
	}

	Color OVRMixedRealityCaptureConfiguration.externalCompositionBackdropColorQuest
	{
		get
		{
			return this.externalCompositionBackdropColorQuest;
		}
		set
		{
			this.externalCompositionBackdropColorQuest = value;
		}
	}

	[Obsolete("Deprecated", false)]
	OVRManager.CameraDevice OVRMixedRealityCaptureConfiguration.capturingCameraDevice
	{
		get
		{
			return this.capturingCameraDevice;
		}
		set
		{
			this.capturingCameraDevice = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.flipCameraFrameHorizontally
	{
		get
		{
			return this.flipCameraFrameHorizontally;
		}
		set
		{
			this.flipCameraFrameHorizontally = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.flipCameraFrameVertically
	{
		get
		{
			return this.flipCameraFrameVertically;
		}
		set
		{
			this.flipCameraFrameVertically = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.handPoseStateLatency
	{
		get
		{
			return this.handPoseStateLatency;
		}
		set
		{
			this.handPoseStateLatency = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.sandwichCompositionRenderLatency
	{
		get
		{
			return this.sandwichCompositionRenderLatency;
		}
		set
		{
			this.sandwichCompositionRenderLatency = value;
		}
	}

	int OVRMixedRealityCaptureConfiguration.sandwichCompositionBufferedFrames
	{
		get
		{
			return this.sandwichCompositionBufferedFrames;
		}
		set
		{
			this.sandwichCompositionBufferedFrames = value;
		}
	}

	Color OVRMixedRealityCaptureConfiguration.chromaKeyColor
	{
		get
		{
			return this.chromaKeyColor;
		}
		set
		{
			this.chromaKeyColor = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.chromaKeySimilarity
	{
		get
		{
			return this.chromaKeySimilarity;
		}
		set
		{
			this.chromaKeySimilarity = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.chromaKeySmoothRange
	{
		get
		{
			return this.chromaKeySmoothRange;
		}
		set
		{
			this.chromaKeySmoothRange = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.chromaKeySpillRange
	{
		get
		{
			return this.chromaKeySpillRange;
		}
		set
		{
			this.chromaKeySpillRange = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.useDynamicLighting
	{
		get
		{
			return this.useDynamicLighting;
		}
		set
		{
			this.useDynamicLighting = value;
		}
	}

	[Obsolete("Deprecated", false)]
	OVRManager.DepthQuality OVRMixedRealityCaptureConfiguration.depthQuality
	{
		get
		{
			return this.depthQuality;
		}
		set
		{
			this.depthQuality = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.dynamicLightingSmoothFactor
	{
		get
		{
			return this.dynamicLightingSmoothFactor;
		}
		set
		{
			this.dynamicLightingSmoothFactor = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.dynamicLightingDepthVariationClampingValue
	{
		get
		{
			return this.dynamicLightingDepthVariationClampingValue;
		}
		set
		{
			this.dynamicLightingDepthVariationClampingValue = value;
		}
	}

	[Obsolete("Deprecated", false)]
	OVRManager.VirtualGreenScreenType OVRMixedRealityCaptureConfiguration.virtualGreenScreenType
	{
		get
		{
			return this.virtualGreenScreenType;
		}
		set
		{
			this.virtualGreenScreenType = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.virtualGreenScreenTopY
	{
		get
		{
			return this.virtualGreenScreenTopY;
		}
		set
		{
			this.virtualGreenScreenTopY = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.virtualGreenScreenBottomY
	{
		get
		{
			return this.virtualGreenScreenBottomY;
		}
		set
		{
			this.virtualGreenScreenBottomY = value;
		}
	}

	bool OVRMixedRealityCaptureConfiguration.virtualGreenScreenApplyDepthCulling
	{
		get
		{
			return this.virtualGreenScreenApplyDepthCulling;
		}
		set
		{
			this.virtualGreenScreenApplyDepthCulling = value;
		}
	}

	float OVRMixedRealityCaptureConfiguration.virtualGreenScreenDepthTolerance
	{
		get
		{
			return this.virtualGreenScreenDepthTolerance;
		}
		set
		{
			this.virtualGreenScreenDepthTolerance = value;
		}
	}

	OVRManager.MrcActivationMode OVRMixedRealityCaptureConfiguration.mrcActivationMode
	{
		get
		{
			return this.mrcActivationMode;
		}
		set
		{
			this.mrcActivationMode = value;
		}
	}

	OVRManager.InstantiateMrcCameraDelegate OVRMixedRealityCaptureConfiguration.instantiateMixedRealityCameraGameObject { get; set; }

	public void WriteToConfigurationFile()
	{
		string contents = JsonUtility.ToJson(this, true);
		try
		{
			string text = Path.Combine(Application.dataPath, "mrc.config");
			Debug.Log("Write OVRMixedRealityCaptureSettings to " + text);
			File.WriteAllText(text, contents);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception caught " + ex.Message);
		}
	}

	public void CombineWithConfigurationFile()
	{
		try
		{
			string text = Path.Combine(Application.dataPath, "mrc.config");
			if (File.Exists(text))
			{
				Debug.Log("MixedRealityCapture configuration file found at " + text);
				string json = File.ReadAllText(text);
				Debug.Log("Apply MixedRealityCapture configuration");
				JsonUtility.FromJsonOverwrite(json, this);
			}
			else
			{
				Debug.Log("MixedRealityCapture configuration file doesn't exist at " + text);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception caught " + ex.Message);
		}
	}

	public bool enableMixedReality;

	public LayerMask extraHiddenLayers;

	public LayerMask extraVisibleLayers;

	public bool dynamicCullingMask = true;

	public OVRManager.CompositionMethod compositionMethod;

	public Color externalCompositionBackdropColorRift = Color.green;

	public Color externalCompositionBackdropColorQuest = Color.clear;

	[Obsolete("Deprecated", false)]
	public OVRManager.CameraDevice capturingCameraDevice;

	public bool flipCameraFrameHorizontally;

	public bool flipCameraFrameVertically;

	public float handPoseStateLatency;

	public float sandwichCompositionRenderLatency;

	public int sandwichCompositionBufferedFrames = 8;

	public Color chromaKeyColor = Color.green;

	public float chromaKeySimilarity = 0.6f;

	public float chromaKeySmoothRange = 0.03f;

	public float chromaKeySpillRange = 0.04f;

	public bool useDynamicLighting;

	[Obsolete("Deprecated", false)]
	public OVRManager.DepthQuality depthQuality = OVRManager.DepthQuality.Medium;

	public float dynamicLightingSmoothFactor = 8f;

	public float dynamicLightingDepthVariationClampingValue = 0.001f;

	[Obsolete("Deprecated", false)]
	public OVRManager.VirtualGreenScreenType virtualGreenScreenType;

	public float virtualGreenScreenTopY;

	public float virtualGreenScreenBottomY;

	public bool virtualGreenScreenApplyDepthCulling;

	public float virtualGreenScreenDepthTolerance = 0.2f;

	public OVRManager.MrcActivationMode mrcActivationMode;

	private const string configFileName = "mrc.config";
}
