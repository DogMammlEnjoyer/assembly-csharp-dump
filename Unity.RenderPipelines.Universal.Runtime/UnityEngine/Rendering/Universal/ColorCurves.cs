using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Color Curves")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class ColorCurves : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return true;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		public ColorCurves()
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			};
			float zeroValue = 0f;
			bool loop = false;
			Vector2 vector = new Vector2(0f, 1f);
			this.master = new TextureCurveParameter(new TextureCurve(keys, zeroValue, loop, ref vector), false);
			Keyframe[] keys2 = new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			};
			float zeroValue2 = 0f;
			bool loop2 = false;
			vector = new Vector2(0f, 1f);
			this.red = new TextureCurveParameter(new TextureCurve(keys2, zeroValue2, loop2, ref vector), false);
			Keyframe[] keys3 = new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			};
			float zeroValue3 = 0f;
			bool loop3 = false;
			vector = new Vector2(0f, 1f);
			this.green = new TextureCurveParameter(new TextureCurve(keys3, zeroValue3, loop3, ref vector), false);
			Keyframe[] keys4 = new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			};
			float zeroValue4 = 0f;
			bool loop4 = false;
			vector = new Vector2(0f, 1f);
			this.blue = new TextureCurveParameter(new TextureCurve(keys4, zeroValue4, loop4, ref vector), false);
			Keyframe[] keys5 = new Keyframe[0];
			float zeroValue5 = 0.5f;
			bool loop5 = true;
			vector = new Vector2(0f, 1f);
			this.hueVsHue = new TextureCurveParameter(new TextureCurve(keys5, zeroValue5, loop5, ref vector), false);
			Keyframe[] keys6 = new Keyframe[0];
			float zeroValue6 = 0.5f;
			bool loop6 = true;
			vector = new Vector2(0f, 1f);
			this.hueVsSat = new TextureCurveParameter(new TextureCurve(keys6, zeroValue6, loop6, ref vector), false);
			Keyframe[] keys7 = new Keyframe[0];
			float zeroValue7 = 0.5f;
			bool loop7 = false;
			vector = new Vector2(0f, 1f);
			this.satVsSat = new TextureCurveParameter(new TextureCurve(keys7, zeroValue7, loop7, ref vector), false);
			Keyframe[] keys8 = new Keyframe[0];
			float zeroValue8 = 0.5f;
			bool loop8 = false;
			vector = new Vector2(0f, 1f);
			this.lumVsSat = new TextureCurveParameter(new TextureCurve(keys8, zeroValue8, loop8, ref vector), false);
			base..ctor();
		}

		[Tooltip("Affects the luminance across the whole image.")]
		public TextureCurveParameter master;

		[Tooltip("Affects the red channel intensity across the whole image.")]
		public TextureCurveParameter red;

		[Tooltip("Affects the green channel intensity across the whole image.")]
		public TextureCurveParameter green;

		[Tooltip("Affects the blue channel intensity across the whole image.")]
		public TextureCurveParameter blue;

		[Tooltip("Shifts the input hue (x-axis) according to the output hue (y-axis).")]
		public TextureCurveParameter hueVsHue;

		[Tooltip("Adjusts saturation (y-axis) according to the input hue (x-axis).")]
		public TextureCurveParameter hueVsSat;

		[Tooltip("Adjusts saturation (y-axis) according to the input saturation (x-axis).")]
		public TextureCurveParameter satVsSat;

		[Tooltip("Adjusts saturation (y-axis) according to the input luminance (x-axis).")]
		public TextureCurveParameter lumVsSat;
	}
}
