using System;
using Unity.Profiling;
using UnityEngine.Profiling;

namespace UnityEngine.Rendering
{
	[IgnoredByDeepProfiler]
	public class ProfilingSampler
	{
		public static ProfilingSampler Get<TEnum>(TEnum marker) where TEnum : Enum
		{
			return null;
		}

		public ProfilingSampler(string name)
		{
			this.sampler = CustomSampler.Create(name, true);
			this.inlineSampler = CustomSampler.Create("Inl_" + name, false);
			this.name = name;
			this.m_Recorder = this.sampler.GetRecorder();
			this.m_Recorder.enabled = false;
			this.m_InlineRecorder = this.inlineSampler.GetRecorder();
			this.m_InlineRecorder.enabled = false;
		}

		public void Begin(CommandBuffer cmd)
		{
			if (cmd != null)
			{
				if (this.sampler != null && this.sampler.isValid)
				{
					cmd.BeginSample(this.sampler);
					return;
				}
				cmd.BeginSample(this.name);
			}
		}

		public void End(CommandBuffer cmd)
		{
			if (cmd != null)
			{
				if (this.sampler != null && this.sampler.isValid)
				{
					cmd.EndSample(this.sampler);
					return;
				}
				cmd.EndSample(this.name);
			}
		}

		internal bool IsValid()
		{
			return this.sampler != null && this.inlineSampler != null;
		}

		internal CustomSampler sampler { get; private set; }

		internal CustomSampler inlineSampler { get; private set; }

		public string name { get; private set; }

		public bool enableRecording
		{
			set
			{
				this.m_Recorder.enabled = value;
				this.m_InlineRecorder.enabled = value;
			}
		}

		public float gpuElapsedTime
		{
			get
			{
				if (!this.m_Recorder.enabled)
				{
					return 0f;
				}
				return (float)this.m_Recorder.gpuElapsedNanoseconds / 1000000f;
			}
		}

		public int gpuSampleCount
		{
			get
			{
				if (!this.m_Recorder.enabled)
				{
					return 0;
				}
				return this.m_Recorder.gpuSampleBlockCount;
			}
		}

		public float cpuElapsedTime
		{
			get
			{
				if (!this.m_Recorder.enabled)
				{
					return 0f;
				}
				return (float)this.m_Recorder.elapsedNanoseconds / 1000000f;
			}
		}

		public int cpuSampleCount
		{
			get
			{
				if (!this.m_Recorder.enabled)
				{
					return 0;
				}
				return this.m_Recorder.sampleBlockCount;
			}
		}

		public float inlineCpuElapsedTime
		{
			get
			{
				if (!this.m_InlineRecorder.enabled)
				{
					return 0f;
				}
				return (float)this.m_InlineRecorder.elapsedNanoseconds / 1000000f;
			}
		}

		public int inlineCpuSampleCount
		{
			get
			{
				if (!this.m_InlineRecorder.enabled)
				{
					return 0;
				}
				return this.m_InlineRecorder.sampleBlockCount;
			}
		}

		private ProfilingSampler()
		{
		}

		private Recorder m_Recorder;

		private Recorder m_InlineRecorder;
	}
}
