using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-haptics-apis/")]
public static class OVRHaptics
{
	static OVRHaptics()
	{
		OVRHaptics.Config.Load();
		OVRHaptics.m_outputs = new OVRHaptics.OVRHapticsOutput[]
		{
			new OVRHaptics.OVRHapticsOutput(1U),
			new OVRHaptics.OVRHapticsOutput(2U)
		};
		OVRHaptics.Channels = new OVRHaptics.OVRHapticsChannel[]
		{
			OVRHaptics.LeftChannel = new OVRHaptics.OVRHapticsChannel(0U),
			OVRHaptics.RightChannel = new OVRHaptics.OVRHapticsChannel(1U)
		};
	}

	public static void Process()
	{
		OVRHaptics.Config.Load();
		for (int i = 0; i < OVRHaptics.m_outputs.Length; i++)
		{
			OVRHaptics.m_outputs[i].Process();
		}
	}

	public static readonly OVRHaptics.OVRHapticsChannel[] Channels;

	public static readonly OVRHaptics.OVRHapticsChannel LeftChannel;

	public static readonly OVRHaptics.OVRHapticsChannel RightChannel;

	private static readonly OVRHaptics.OVRHapticsOutput[] m_outputs;

	public static class Config
	{
		public static int SampleRateHz { get; private set; }

		public static int SampleSizeInBytes { get; private set; }

		public static int MinimumSafeSamplesQueued { get; private set; }

		public static int MinimumBufferSamplesCount { get; private set; }

		public static int OptimalBufferSamplesCount { get; private set; }

		public static int MaximumBufferSamplesCount { get; private set; }

		static Config()
		{
			OVRHaptics.Config.Load();
		}

		public static void Load()
		{
			OVRPlugin.HapticsDesc controllerHapticsDesc = OVRPlugin.GetControllerHapticsDesc(2U);
			OVRHaptics.Config.SampleRateHz = controllerHapticsDesc.SampleRateHz;
			OVRHaptics.Config.SampleSizeInBytes = controllerHapticsDesc.SampleSizeInBytes;
			OVRHaptics.Config.MinimumSafeSamplesQueued = controllerHapticsDesc.MinimumSafeSamplesQueued;
			OVRHaptics.Config.MinimumBufferSamplesCount = controllerHapticsDesc.MinimumBufferSamplesCount;
			OVRHaptics.Config.OptimalBufferSamplesCount = controllerHapticsDesc.OptimalBufferSamplesCount;
			OVRHaptics.Config.MaximumBufferSamplesCount = controllerHapticsDesc.MaximumBufferSamplesCount;
		}
	}

	public class OVRHapticsChannel
	{
		public OVRHapticsChannel(uint outputIndex)
		{
			this.m_output = OVRHaptics.m_outputs[(int)outputIndex];
		}

		public void Preempt(OVRHapticsClip clip)
		{
			this.m_output.Preempt(clip);
		}

		public void Queue(OVRHapticsClip clip)
		{
			this.m_output.Queue(clip);
		}

		public void Mix(OVRHapticsClip clip)
		{
			this.m_output.Mix(clip);
		}

		public void Clear()
		{
			this.m_output.Clear();
		}

		private OVRHaptics.OVRHapticsOutput m_output;
	}

	private class OVRHapticsOutput
	{
		public OVRHapticsOutput(uint controller)
		{
			this.m_controller = controller;
		}

		public void Process()
		{
			if (OVRHaptics.Config.SampleRateHz == 0)
			{
				if (this.PrevSampleRateHz != 0)
				{
					Debug.Log("Unable to process a controller whose SampleRateHz is 0 now.");
					this.PrevSampleRateHz = 0;
				}
				return;
			}
			this.PrevSampleRateHz = OVRHaptics.Config.SampleRateHz;
			if (this.m_nativeBuffer.GetCapacity() != OVRHaptics.Config.MaximumBufferSamplesCount * OVRHaptics.Config.SampleSizeInBytes)
			{
				this.m_nativeBuffer.Reset(OVRHaptics.Config.MaximumBufferSamplesCount * OVRHaptics.Config.SampleSizeInBytes);
			}
			OVRPlugin.HapticsState controllerHapticsState = OVRPlugin.GetControllerHapticsState(this.m_controller);
			float num = Time.realtimeSinceStartup - this.m_prevSamplesQueuedTime;
			if (this.m_prevSamplesQueued > 0)
			{
				int num2 = this.m_prevSamplesQueued - (int)(num * (float)OVRHaptics.Config.SampleRateHz + 0.5f);
				if (num2 < 0)
				{
					num2 = 0;
				}
				if (controllerHapticsState.SamplesQueued - num2 == 0)
				{
					this.m_numPredictionHits++;
				}
				else
				{
					this.m_numPredictionMisses++;
				}
				if (num2 > 0 && controllerHapticsState.SamplesQueued == 0)
				{
					this.m_numUnderruns++;
				}
				this.m_prevSamplesQueued = controllerHapticsState.SamplesQueued;
				this.m_prevSamplesQueuedTime = Time.realtimeSinceStartup;
			}
			int num3 = OVRHaptics.Config.OptimalBufferSamplesCount;
			if (this.m_lowLatencyMode)
			{
				float num4 = 1000f / (float)OVRHaptics.Config.SampleRateHz;
				int num5 = (int)Mathf.Ceil(num * 1000f / num4);
				int num6 = OVRHaptics.Config.MinimumSafeSamplesQueued + num5;
				if (num6 < num3)
				{
					num3 = num6;
				}
			}
			if (controllerHapticsState.SamplesQueued > num3)
			{
				return;
			}
			if (num3 > OVRHaptics.Config.MaximumBufferSamplesCount)
			{
				num3 = OVRHaptics.Config.MaximumBufferSamplesCount;
			}
			if (num3 > controllerHapticsState.SamplesAvailable)
			{
				num3 = controllerHapticsState.SamplesAvailable;
			}
			int num7 = 0;
			int num8 = 0;
			while (num7 < num3 && num8 < this.m_pendingClips.Count)
			{
				int num9 = num3 - num7;
				int num10 = this.m_pendingClips[num8].Clip.Count - this.m_pendingClips[num8].ReadCount;
				if (num9 > num10)
				{
					num9 = num10;
				}
				if (num9 > 0)
				{
					int length = num9 * OVRHaptics.Config.SampleSizeInBytes;
					int byteOffset = num7 * OVRHaptics.Config.SampleSizeInBytes;
					int startIndex = this.m_pendingClips[num8].ReadCount * OVRHaptics.Config.SampleSizeInBytes;
					Marshal.Copy(this.m_pendingClips[num8].Clip.Samples, startIndex, this.m_nativeBuffer.GetPointer(byteOffset), length);
					this.m_pendingClips[num8].ReadCount += num9;
					num7 += num9;
				}
				num8++;
			}
			int num11 = this.m_pendingClips.Count - 1;
			while (num11 >= 0 && this.m_pendingClips.Count > 0)
			{
				if (this.m_pendingClips[num11].ReadCount >= this.m_pendingClips[num11].Clip.Count)
				{
					this.m_pendingClips.RemoveAt(num11);
				}
				num11--;
			}
			if (num7 > 0)
			{
				OVRPlugin.HapticsBuffer hapticsBuffer;
				hapticsBuffer.Samples = this.m_nativeBuffer.GetPointer(0);
				hapticsBuffer.SamplesCount = num7;
				OVRPlugin.SetControllerHaptics(this.m_controller, hapticsBuffer);
				controllerHapticsState = OVRPlugin.GetControllerHapticsState(this.m_controller);
				this.m_prevSamplesQueued = controllerHapticsState.SamplesQueued;
				this.m_prevSamplesQueuedTime = Time.realtimeSinceStartup;
			}
		}

		public void Preempt(OVRHapticsClip clip)
		{
			this.m_pendingClips.Clear();
			this.m_pendingClips.Add(new OVRHaptics.OVRHapticsOutput.ClipPlaybackTracker(clip));
		}

		public void Queue(OVRHapticsClip clip)
		{
			this.m_pendingClips.Add(new OVRHaptics.OVRHapticsOutput.ClipPlaybackTracker(clip));
		}

		public void Mix(OVRHapticsClip clip)
		{
			int num = 0;
			int num2 = 0;
			int num3 = clip.Count;
			while (num3 > 0 && num < this.m_pendingClips.Count)
			{
				int num4 = this.m_pendingClips[num].Clip.Count - this.m_pendingClips[num].ReadCount;
				num3 -= num4;
				num2 += num4;
				num++;
			}
			if (num3 > 0)
			{
				num2 += num3;
			}
			if (num > 0)
			{
				OVRHapticsClip ovrhapticsClip = new OVRHapticsClip(num2);
				int i = 0;
				for (int j = 0; j < num; j++)
				{
					OVRHapticsClip clip2 = this.m_pendingClips[j].Clip;
					for (int k = this.m_pendingClips[j].ReadCount; k < clip2.Count; k++)
					{
						if (OVRHaptics.Config.SampleSizeInBytes == 1)
						{
							byte sample = 0;
							if (i < clip.Count && k < clip2.Count)
							{
								sample = (byte)Mathf.Clamp((int)(clip.Samples[i] + clip2.Samples[k]), 0, 255);
								i++;
							}
							else if (k < clip2.Count)
							{
								sample = clip2.Samples[k];
							}
							ovrhapticsClip.WriteSample(sample);
						}
					}
				}
				while (i < clip.Count)
				{
					if (OVRHaptics.Config.SampleSizeInBytes == 1)
					{
						ovrhapticsClip.WriteSample(clip.Samples[i]);
					}
					i++;
				}
				this.m_pendingClips[0] = new OVRHaptics.OVRHapticsOutput.ClipPlaybackTracker(ovrhapticsClip);
				for (int l = 1; l < num; l++)
				{
					this.m_pendingClips.RemoveAt(1);
				}
				return;
			}
			this.m_pendingClips.Add(new OVRHaptics.OVRHapticsOutput.ClipPlaybackTracker(clip));
		}

		public void Clear()
		{
			this.m_pendingClips.Clear();
		}

		private bool m_lowLatencyMode = true;

		private int m_prevSamplesQueued;

		private float m_prevSamplesQueuedTime;

		private int m_numPredictionHits;

		private int m_numPredictionMisses;

		private int m_numUnderruns;

		private List<OVRHaptics.OVRHapticsOutput.ClipPlaybackTracker> m_pendingClips = new List<OVRHaptics.OVRHapticsOutput.ClipPlaybackTracker>();

		private uint m_controller;

		private OVRNativeBuffer m_nativeBuffer = new OVRNativeBuffer(OVRHaptics.Config.MaximumBufferSamplesCount * OVRHaptics.Config.SampleSizeInBytes);

		private int PrevSampleRateHz = -1;

		private class ClipPlaybackTracker
		{
			public int ReadCount { get; set; }

			public OVRHapticsClip Clip { get; set; }

			public ClipPlaybackTracker(OVRHapticsClip clip)
			{
				this.Clip = clip;
			}
		}
	}
}
