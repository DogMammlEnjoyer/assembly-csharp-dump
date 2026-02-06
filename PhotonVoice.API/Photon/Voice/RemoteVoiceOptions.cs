using System;

namespace Photon.Voice
{
	public struct RemoteVoiceOptions
	{
		public RemoteVoiceOptions(ILogger logger, string logPrefix, VoiceInfo voiceInfo)
		{
			this.logger = logger;
			this.logPrefix = logPrefix;
			this.voiceInfo = voiceInfo;
			this.Decoder = null;
			this.OnRemoteVoiceRemoveAction = null;
		}

		public void SetOutput(Action<FrameOut<float>> output)
		{
			if (this.voiceInfo.Codec == Codec.Raw)
			{
				this.Decoder = new RawCodec.Decoder<short>(new Action<FrameOut<short>>(new RawCodec.ShortToFloat(output).Output));
				return;
			}
			this.setOutput<float>(output);
		}

		public void SetOutput(Action<FrameOut<short>> output)
		{
			if (this.voiceInfo.Codec == Codec.Raw)
			{
				this.Decoder = new RawCodec.Decoder<short>(output);
				return;
			}
			this.setOutput<short>(output);
		}

		private void setOutput<T>(Action<FrameOut<T>> output)
		{
			ILogger logger = this.logger;
			string[] array = new string[6];
			array[0] = this.logPrefix;
			array[1] = ": Creating default decoder ";
			array[2] = this.voiceInfo.Codec.ToString();
			array[3] = " for output FrameOut<";
			int num = 4;
			Type typeFromHandle = typeof(T);
			array[num] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
			array[5] = ">";
			logger.LogInfo(string.Concat(array), Array.Empty<object>());
			if (this.voiceInfo.Codec == Codec.AudioOpus)
			{
				this.Decoder = new OpusCodec.Decoder<T>(output, this.logger);
				return;
			}
			ILogger logger2 = this.logger;
			string[] array2 = new string[5];
			array2[0] = this.logPrefix;
			array2[1] = ": FrameOut<";
			int num2 = 2;
			Type typeFromHandle2 = typeof(T);
			array2[num2] = ((typeFromHandle2 != null) ? typeFromHandle2.ToString() : null);
			array2[3] = "> output set for non-audio decoder ";
			array2[4] = this.voiceInfo.Codec.ToString();
			logger2.LogError(string.Concat(array2), Array.Empty<object>());
		}

		public Action OnRemoteVoiceRemoveAction { readonly get; set; }

		public IDecoder Decoder { readonly get; set; }

		internal readonly string logPrefix { get; }

		private readonly ILogger logger;

		private readonly VoiceInfo voiceInfo;
	}
}
