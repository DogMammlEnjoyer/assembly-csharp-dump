using System;
using POpusCodec.Enums;

namespace Photon.Voice
{
	public struct VoiceInfo
	{
		public static VoiceInfo CreateAudioOpus(SamplingRate samplingRate, int channels, OpusCodec.FrameDuration frameDurationUs, int bitrate, object userdata = null)
		{
			return new VoiceInfo
			{
				Codec = Codec.AudioOpus,
				SamplingRate = (int)samplingRate,
				Channels = channels,
				FrameDurationUs = (int)frameDurationUs,
				Bitrate = bitrate,
				UserData = userdata
			};
		}

		public static VoiceInfo CreateAudio(Codec codec, int samplingRate, int channels, int frameDurationUs, object userdata = null)
		{
			return new VoiceInfo
			{
				Codec = codec,
				SamplingRate = samplingRate,
				Channels = channels,
				FrameDurationUs = frameDurationUs,
				UserData = userdata
			};
		}

		public override string ToString()
		{
			string[] array = new string[22];
			array[0] = "c=";
			array[1] = this.Codec.ToString();
			array[2] = " f=";
			array[3] = this.SamplingRate.ToString();
			array[4] = " ch=";
			array[5] = this.Channels.ToString();
			array[6] = " d=";
			array[7] = this.FrameDurationUs.ToString();
			array[8] = " s=";
			array[9] = this.FrameSize.ToString();
			array[10] = " b=";
			array[11] = this.Bitrate.ToString();
			array[12] = " w=";
			array[13] = this.Width.ToString();
			array[14] = " h=";
			array[15] = this.Height.ToString();
			array[16] = " fps=";
			array[17] = this.FPS.ToString();
			array[18] = " kfi=";
			array[19] = this.KeyFrameInt.ToString();
			array[20] = " ud=";
			int num = 21;
			object userData = this.UserData;
			array[num] = ((userData != null) ? userData.ToString() : null);
			return string.Concat(array);
		}

		public Codec Codec { readonly get; set; }

		public int SamplingRate { readonly get; set; }

		public int Channels { readonly get; set; }

		public int FrameDurationUs { readonly get; set; }

		public int Bitrate { readonly get; set; }

		public int Width { readonly get; set; }

		public int Height { readonly get; set; }

		public int FPS { readonly get; set; }

		public int KeyFrameInt { readonly get; set; }

		public object UserData { readonly get; set; }

		public int FrameDurationSamples
		{
			get
			{
				return (int)((long)this.SamplingRate * (long)this.FrameDurationUs / 1000000L);
			}
		}

		public int FrameSize
		{
			get
			{
				return this.FrameDurationSamples * this.Channels;
			}
		}
	}
}
