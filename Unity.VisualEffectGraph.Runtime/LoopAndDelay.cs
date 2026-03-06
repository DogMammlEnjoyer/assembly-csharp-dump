using System;

namespace UnityEngine.VFX
{
	internal class LoopAndDelay : VFXSpawnerCallbacks
	{
		public sealed override void OnPlay(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			this.m_LoopMaxCount = vfxValues.GetInt(LoopAndDelay.loopCountPropertyID);
			this.m_WaitingForTotalTime = vfxValues.GetFloat(LoopAndDelay.loopDurationPropertyID);
			this.m_LoopCurrentIndex = 0;
			if (this.m_LoopMaxCount == this.m_LoopCurrentIndex)
			{
				state.playing = false;
			}
		}

		public sealed override void OnUpdate(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			if (this.m_LoopCurrentIndex != this.m_LoopMaxCount && state.totalTime > this.m_WaitingForTotalTime)
			{
				if (state.playing)
				{
					this.m_WaitingForTotalTime = state.totalTime + vfxValues.GetFloat(LoopAndDelay.delayPropertyID);
					state.playing = false;
					this.m_LoopCurrentIndex = ((this.m_LoopCurrentIndex + 1 > 0) ? (this.m_LoopCurrentIndex + 1) : 0);
					return;
				}
				this.m_WaitingForTotalTime = vfxValues.GetFloat(LoopAndDelay.loopDurationPropertyID);
				state.totalTime = 0f;
				state.playing = true;
			}
		}

		public sealed override void OnStop(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			this.m_LoopCurrentIndex = this.m_LoopMaxCount;
		}

		private int m_LoopMaxCount;

		private int m_LoopCurrentIndex;

		private float m_WaitingForTotalTime;

		private static readonly int loopCountPropertyID = Shader.PropertyToID("LoopCount");

		private static readonly int loopDurationPropertyID = Shader.PropertyToID("LoopDuration");

		private static readonly int delayPropertyID = Shader.PropertyToID("Delay");

		public class InputProperties
		{
			[Tooltip("Number of Loops (< 0 for infinite), evaluated when Context Start is hit")]
			public int LoopCount = 1;

			[Tooltip("Duration of one loop, evaluated every loop")]
			public float LoopDuration = 4f;

			[Tooltip("Duration of in-between delay (after each loop), evaluated every loop")]
			public float Delay = 1f;
		}
	}
}
