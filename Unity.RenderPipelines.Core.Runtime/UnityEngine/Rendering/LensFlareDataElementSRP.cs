using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[Serializable]
	public sealed class LensFlareDataElementSRP
	{
		public LensFlareDataElementSRP()
		{
			this.lensFlareDataSRP = null;
			this.visible = true;
			this.localIntensity = 1f;
			this.position = 0f;
			this.positionOffset = new Vector2(0f, 0f);
			this.angularOffset = 0f;
			this.translationScale = new Vector2(1f, 1f);
			this.lensFlareTexture = null;
			this.uniformScale = 1f;
			this.sizeXY = Vector2.one;
			this.allowMultipleElement = false;
			this.count = 5;
			this.rotation = 0f;
			this.preserveAspectRatio = false;
			this.ringThickness = 0.25f;
			this.hoopFactor = 1f;
			this.noiseAmplitude = 1f;
			this.noiseFrequency = 1;
			this.noiseSpeed = 0f;
			this.shapeCutOffSpeed = 0f;
			this.shapeCutOffRadius = 10f;
			this.tintColorType = SRPLensFlareColorType.Constant;
			this.tint = new Color(1f, 1f, 1f, 0.5f);
			this.tintGradient = new TextureGradient(new GradientColorKey[]
			{
				new GradientColorKey(Color.black, 0f),
				new GradientColorKey(Color.white, 1f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(0f, 0f),
				new GradientAlphaKey(1f, 1f)
			}, GradientMode.PerceptualBlend, ColorSpace.Uninitialized, -1, false);
			this.blendMode = SRPLensFlareBlendMode.Additive;
			this.autoRotate = false;
			this.isFoldOpened = true;
			this.flareType = SRPLensFlareType.Circle;
			this.distribution = SRPLensFlareDistribution.Uniform;
			this.lengthSpread = 1f;
			this.colorGradient = new Gradient();
			this.colorGradient.SetKeys(new GradientColorKey[]
			{
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(Color.white, 1f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
			this.positionCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, -1f)
			});
			this.scaleCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f),
				new Keyframe(1f, 1f)
			});
			this.uniformAngle = 0f;
			this.uniformAngleCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 0f)
			});
			this.seed = 0;
			this.intensityVariation = 0.75f;
			this.positionVariation = new Vector2(1f, 0f);
			this.scaleVariation = 1f;
			this.rotationVariation = 180f;
			this.enableRadialDistortion = false;
			this.targetSizeDistortion = Vector2.one;
			this.distortionCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, -1f)
			});
			this.distortionRelativeToCenter = false;
			this.fallOff = 1f;
			this.edgeOffset = 0.1f;
			this.sdfRoundness = 0f;
			this.sideCount = 6;
			this.inverseSDF = false;
		}

		public LensFlareDataElementSRP Clone()
		{
			LensFlareDataElementSRP lensFlareDataElementSRP = new LensFlareDataElementSRP();
			lensFlareDataElementSRP.lensFlareDataSRP = this.lensFlareDataSRP;
			lensFlareDataElementSRP.visible = this.visible;
			lensFlareDataElementSRP.localIntensity = this.localIntensity;
			lensFlareDataElementSRP.position = this.position;
			lensFlareDataElementSRP.positionOffset = this.positionOffset;
			lensFlareDataElementSRP.angularOffset = this.angularOffset;
			lensFlareDataElementSRP.translationScale = this.translationScale;
			lensFlareDataElementSRP.lensFlareTexture = this.lensFlareTexture;
			lensFlareDataElementSRP.uniformScale = this.uniformScale;
			lensFlareDataElementSRP.sizeXY = this.sizeXY;
			lensFlareDataElementSRP.allowMultipleElement = this.allowMultipleElement;
			lensFlareDataElementSRP.count = this.count;
			lensFlareDataElementSRP.rotation = this.rotation;
			lensFlareDataElementSRP.preserveAspectRatio = this.preserveAspectRatio;
			lensFlareDataElementSRP.ringThickness = this.ringThickness;
			lensFlareDataElementSRP.hoopFactor = this.hoopFactor;
			lensFlareDataElementSRP.noiseAmplitude = this.noiseAmplitude;
			lensFlareDataElementSRP.noiseFrequency = this.noiseFrequency;
			lensFlareDataElementSRP.noiseSpeed = this.noiseSpeed;
			lensFlareDataElementSRP.shapeCutOffSpeed = this.shapeCutOffSpeed;
			lensFlareDataElementSRP.shapeCutOffRadius = this.shapeCutOffRadius;
			lensFlareDataElementSRP.tintColorType = this.tintColorType;
			lensFlareDataElementSRP.tint = this.tint;
			lensFlareDataElementSRP.tintGradient = new TextureGradient(this.tintGradient.colorKeys, this.tintGradient.alphaKeys, this.tintGradient.mode, this.tintGradient.colorSpace, this.tintGradient.textureSize, false);
			lensFlareDataElementSRP.tintGradient = new TextureGradient(this.tintGradient.colorKeys, this.tintGradient.alphaKeys, GradientMode.PerceptualBlend, ColorSpace.Uninitialized, -1, false);
			lensFlareDataElementSRP.blendMode = this.blendMode;
			lensFlareDataElementSRP.autoRotate = this.autoRotate;
			lensFlareDataElementSRP.isFoldOpened = this.isFoldOpened;
			lensFlareDataElementSRP.flareType = this.flareType;
			lensFlareDataElementSRP.distribution = this.distribution;
			lensFlareDataElementSRP.lengthSpread = this.lengthSpread;
			lensFlareDataElementSRP.colorGradient = new Gradient();
			lensFlareDataElementSRP.colorGradient.SetKeys(this.colorGradient.colorKeys, this.colorGradient.alphaKeys);
			lensFlareDataElementSRP.colorGradient.mode = this.colorGradient.mode;
			lensFlareDataElementSRP.colorGradient.colorSpace = this.colorGradient.colorSpace;
			lensFlareDataElementSRP.positionCurve = new AnimationCurve(this.positionCurve.keys);
			lensFlareDataElementSRP.scaleCurve = new AnimationCurve(this.scaleCurve.keys);
			lensFlareDataElementSRP.uniformAngle = this.uniformAngle;
			lensFlareDataElementSRP.uniformAngleCurve = new AnimationCurve(this.uniformAngleCurve.keys);
			lensFlareDataElementSRP.seed = this.seed;
			lensFlareDataElementSRP.intensityVariation = this.intensityVariation;
			lensFlareDataElementSRP.positionVariation = this.positionVariation;
			lensFlareDataElementSRP.scaleVariation = this.scaleVariation;
			lensFlareDataElementSRP.rotationVariation = this.rotationVariation;
			lensFlareDataElementSRP.enableRadialDistortion = this.enableRadialDistortion;
			lensFlareDataElementSRP.targetSizeDistortion = this.targetSizeDistortion;
			lensFlareDataElementSRP.distortionCurve = new AnimationCurve(this.distortionCurve.keys);
			lensFlareDataElementSRP.distortionRelativeToCenter = this.distortionRelativeToCenter;
			lensFlareDataElementSRP.fallOff = this.fallOff;
			lensFlareDataElementSRP.edgeOffset = this.edgeOffset;
			lensFlareDataElementSRP.sdfRoundness = this.sdfRoundness;
			lensFlareDataElementSRP.sideCount = this.sideCount;
			lensFlareDataElementSRP.inverseSDF = this.inverseSDF;
			return lensFlareDataElementSRP;
		}

		public float localIntensity
		{
			get
			{
				return this.m_LocalIntensity;
			}
			set
			{
				this.m_LocalIntensity = Mathf.Max(0f, value);
			}
		}

		public int count
		{
			get
			{
				return this.m_Count;
			}
			set
			{
				this.m_Count = Mathf.Max(1, value);
			}
		}

		public float intensityVariation
		{
			get
			{
				return this.m_IntensityVariation;
			}
			set
			{
				this.m_IntensityVariation = Mathf.Max(0f, value);
			}
		}

		public float fallOff
		{
			get
			{
				return this.m_FallOff;
			}
			set
			{
				this.m_FallOff = Mathf.Clamp01(value);
			}
		}

		public float edgeOffset
		{
			get
			{
				return this.m_EdgeOffset;
			}
			set
			{
				this.m_EdgeOffset = Mathf.Clamp01(value);
			}
		}

		public int sideCount
		{
			get
			{
				return this.m_SideCount;
			}
			set
			{
				this.m_SideCount = Mathf.Max(3, value);
			}
		}

		public float sdfRoundness
		{
			get
			{
				return this.m_SdfRoundness;
			}
			set
			{
				this.m_SdfRoundness = Mathf.Clamp01(value);
			}
		}

		public LensFlareDataSRP lensFlareDataSRP;

		public bool visible;

		public float position;

		public Vector2 positionOffset;

		public float angularOffset;

		public Vector2 translationScale;

		[Range(0f, 1f)]
		public float ringThickness;

		[Range(-1f, 1f)]
		public float hoopFactor;

		public float noiseAmplitude;

		public int noiseFrequency;

		public float noiseSpeed;

		public float shapeCutOffSpeed;

		public float shapeCutOffRadius;

		[Min(0f)]
		[SerializeField]
		[FormerlySerializedAs("localIntensity")]
		private float m_LocalIntensity;

		public Texture lensFlareTexture;

		public float uniformScale;

		public Vector2 sizeXY;

		public bool allowMultipleElement;

		[Min(1f)]
		[SerializeField]
		[FormerlySerializedAs("count")]
		private int m_Count;

		public bool preserveAspectRatio;

		public float rotation;

		public SRPLensFlareColorType tintColorType;

		public Color tint;

		public TextureGradient tintGradient;

		public SRPLensFlareBlendMode blendMode;

		public bool autoRotate;

		public SRPLensFlareType flareType;

		public bool modulateByLightColor;

		[SerializeField]
		private bool isFoldOpened;

		public SRPLensFlareDistribution distribution;

		public float lengthSpread;

		public AnimationCurve positionCurve;

		public AnimationCurve scaleCurve;

		public int seed;

		public Gradient colorGradient;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("intensityVariation")]
		private float m_IntensityVariation;

		public Vector2 positionVariation;

		public float scaleVariation;

		public float rotationVariation;

		public bool enableRadialDistortion;

		public Vector2 targetSizeDistortion;

		public AnimationCurve distortionCurve;

		public bool distortionRelativeToCenter;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("fallOff")]
		private float m_FallOff;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("edgeOffset")]
		private float m_EdgeOffset;

		[Min(3f)]
		[SerializeField]
		[FormerlySerializedAs("sideCount")]
		private int m_SideCount;

		[Range(0f, 1f)]
		[SerializeField]
		[FormerlySerializedAs("sdfRoundness")]
		private float m_SdfRoundness;

		public bool inverseSDF;

		public float uniformAngle;

		public AnimationCurve uniformAngleCurve;
	}
}
