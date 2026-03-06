using System;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Audio Spectrum Binder")]
	[VFXBinder("Audio/Audio Spectrum to AttributeMap")]
	internal class VFXAudioSpectrumBinder : VFXBinderBase
	{
		public string CountProperty
		{
			get
			{
				return (string)this.m_CountProperty;
			}
			set
			{
				this.m_CountProperty = value;
			}
		}

		public string TextureProperty
		{
			get
			{
				return (string)this.m_TextureProperty;
			}
			set
			{
				this.m_TextureProperty = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			bool flag = this.Mode != VFXAudioSpectrumBinder.AudioSourceMode.AudioSource || this.AudioSource != null;
			bool flag2 = component.HasTexture(this.TextureProperty);
			bool flag3 = component.HasUInt(this.CountProperty);
			return flag && flag2 && flag3;
		}

		private void UpdateTexture()
		{
			if (this.m_Texture == null || (long)this.m_Texture.width != (long)((ulong)this.Samples))
			{
				this.m_Texture = new Texture2D((int)this.Samples, 1, TextureFormat.RFloat, false);
				this.m_AudioCache = new float[this.Samples];
				this.m_ColorCache = new Color[this.Samples];
			}
			if (this.Mode == VFXAudioSpectrumBinder.AudioSourceMode.AudioListener)
			{
				AudioListener.GetSpectrumData(this.m_AudioCache, 0, this.FFTWindow);
			}
			else
			{
				if (this.Mode != VFXAudioSpectrumBinder.AudioSourceMode.AudioSource)
				{
					throw new NotImplementedException();
				}
				this.AudioSource.GetSpectrumData(this.m_AudioCache, 0, this.FFTWindow);
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.Samples))
			{
				this.m_ColorCache[num] = new Color(this.m_AudioCache[num], 0f, 0f, 0f);
				num++;
			}
			this.m_Texture.SetPixels(this.m_ColorCache);
			this.m_Texture.name = "AudioSpectrum" + this.Samples.ToString();
			this.m_Texture.Apply();
		}

		public override void UpdateBinding(VisualEffect component)
		{
			this.UpdateTexture();
			component.SetTexture(this.TextureProperty, this.m_Texture);
			component.SetUInt(this.CountProperty, this.Samples);
		}

		public override string ToString()
		{
			return string.Format("Audio Spectrum : '{0} samples' -> {1}", this.m_CountProperty, (this.Mode == VFXAudioSpectrumBinder.AudioSourceMode.AudioSource) ? "AudioSource" : "AudioListener");
		}

		[VFXPropertyBinding(new string[]
		{
			"System.UInt32"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_CountParameter")]
		protected ExposedProperty m_CountProperty = "Count";

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Texture2D"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_TextureParameter")]
		protected ExposedProperty m_TextureProperty = "SpectrumTexture";

		public FFTWindow FFTWindow = FFTWindow.BlackmanHarris;

		public uint Samples = 64U;

		public VFXAudioSpectrumBinder.AudioSourceMode Mode;

		public AudioSource AudioSource;

		private Texture2D m_Texture;

		private float[] m_AudioCache;

		private Color[] m_ColorCache;

		public enum AudioSourceMode
		{
			AudioSource,
			AudioListener
		}
	}
}
