using System;
using UnityEngine.Playables;

namespace UnityEngine.VFX
{
	internal class VisualEffectControlTrackMixerBehaviour : PlayableBehaviour
	{
		public void Init(VisualEffectControlTrack parentTrack, bool reinitWithBinding, bool reinitWithUnbinding)
		{
			this.m_ReinitWithBinding = reinitWithBinding;
			this.m_ReinitWithUnbinding = reinitWithUnbinding;
		}

		private void ApplyFrame(Playable playable, FrameData data)
		{
			if (this.m_Target == null)
			{
				return;
			}
			if (this.m_ScrubbingCacheHelper == null)
			{
				this.m_ScrubbingCacheHelper = new VisualEffectControlTrackController();
				VisualEffectControlTrack parentTrack = null;
				this.m_ScrubbingCacheHelper.Init(playable, this.m_Target, parentTrack);
			}
			double duration = playable.GetOutput(0).GetDuration<Playable>();
			double num = playable.GetTime<Playable>();
			int num2 = (int)(num / duration);
			num -= (double)num2 * duration;
			float deltaTime = data.deltaTime;
			this.m_ScrubbingCacheHelper.Update(num, deltaTime);
		}

		private void BindVFX(VisualEffect vfx)
		{
			this.m_Target = vfx;
			if (this.m_Target != null && this.m_ReinitWithBinding)
			{
				this.m_Target.Reinit(false);
			}
		}

		private void UnbindVFX()
		{
			if (this.m_Target != null && this.m_ReinitWithUnbinding)
			{
				this.m_Target.Reinit(true);
			}
			this.m_Target = null;
		}

		public override void PrepareFrame(Playable playable, FrameData data)
		{
			VisualEffect visualEffect = data.output.GetUserData<PlayableOutput>() as VisualEffect;
			if (this.m_Target != visualEffect)
			{
				this.UnbindVFX();
				if (visualEffect != null)
				{
					if (visualEffect.visualEffectAsset == null)
					{
						visualEffect = null;
					}
					else if (!visualEffect.isActiveAndEnabled)
					{
						visualEffect = null;
					}
				}
				this.BindVFX(visualEffect);
				this.InvalidateScrubbingHelper();
			}
			this.ApplyFrame(playable, data);
		}

		public override void OnBehaviourPause(Playable playable, FrameData data)
		{
			base.OnBehaviourPause(playable, data);
			this.ApplyFrame(playable, data);
		}

		private void InvalidateScrubbingHelper()
		{
			if (this.m_ScrubbingCacheHelper != null)
			{
				this.m_ScrubbingCacheHelper.Release();
				this.m_ScrubbingCacheHelper = null;
			}
		}

		public override void OnPlayableCreate(Playable playable)
		{
			this.InvalidateScrubbingHelper();
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			this.InvalidateScrubbingHelper();
			this.UnbindVFX();
		}

		private VisualEffectControlTrackController m_ScrubbingCacheHelper;

		private VisualEffect m_Target;

		private bool m_ReinitWithBinding;

		private bool m_ReinitWithUnbinding;
	}
}
