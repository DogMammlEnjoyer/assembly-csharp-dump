using System;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	[RequireComponent(typeof(Recorder))]
	public class MicAmplifier : VoiceComponent
	{
		public float AmplificationFactor
		{
			get
			{
				return this.amplificationFactor;
			}
			set
			{
				if (this.amplificationFactor.Equals(value))
				{
					return;
				}
				this.amplificationFactor = value;
				if (this.floatProcessor != null)
				{
					this.floatProcessor.AmplificationFactor = this.amplificationFactor;
				}
				if (this.shortProcessor != null)
				{
					this.shortProcessor.AmplificationFactor = (short)this.amplificationFactor;
				}
			}
		}

		public float BoostValue
		{
			get
			{
				return this.boostValue;
			}
			set
			{
				if (this.boostValue.Equals(value))
				{
					return;
				}
				this.boostValue = value;
				if (this.floatProcessor != null)
				{
					this.floatProcessor.BoostValue = this.boostValue;
				}
				if (this.shortProcessor != null)
				{
					this.shortProcessor.BoostValue = (short)this.boostValue;
				}
			}
		}

		private void OnEnable()
		{
			if (this.floatProcessor != null)
			{
				this.floatProcessor.Disabled = false;
			}
			if (this.shortProcessor != null)
			{
				this.shortProcessor.Disabled = false;
			}
		}

		private void OnDisable()
		{
			if (this.floatProcessor != null)
			{
				this.floatProcessor.Disabled = true;
			}
			if (this.shortProcessor != null)
			{
				this.shortProcessor.Disabled = true;
			}
		}

		private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			if (p.Voice is LocalVoiceAudioFloat)
			{
				LocalVoiceFramed<float> localVoiceFramed = p.Voice as LocalVoiceAudioFloat;
				this.floatProcessor = new MicAmplifierFloat(this.AmplificationFactor, this.BoostValue);
				localVoiceFramed.AddPostProcessor(new IProcessor<float>[]
				{
					this.floatProcessor
				});
				return;
			}
			if (p.Voice is LocalVoiceAudioShort)
			{
				LocalVoiceFramed<short> localVoiceFramed2 = p.Voice as LocalVoiceAudioShort;
				this.shortProcessor = new MicAmplifierShort((short)this.AmplificationFactor, (short)this.BoostValue);
				localVoiceFramed2.AddPostProcessor(new IProcessor<short>[]
				{
					this.shortProcessor
				});
				return;
			}
			if (base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("LocalVoice object has unexpected value/type: {0}", new object[]
				{
					(p.Voice == null) ? "null" : p.Voice.GetType().ToString()
				});
			}
		}

		[SerializeField]
		private float boostValue;

		[SerializeField]
		private float amplificationFactor = 1f;

		private MicAmplifierFloat floatProcessor;

		private MicAmplifierShort shortProcessor;
	}
}
