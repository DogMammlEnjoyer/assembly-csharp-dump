using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Unity.Cinemachine
{
	internal sealed class CinemachinePlayableMixer : PlayableBehaviour
	{
		public override void OnPlayableDestroy(Playable playable)
		{
			if (this.m_BrainOverrideStack != null)
			{
				this.m_BrainOverrideStack.ReleaseCameraOverride(this.m_BrainOverrideId);
			}
			this.m_BrainOverrideId = -1;
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			this.m_PreviewPlay = false;
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			base.ProcessFrame(playable, info, playerData);
			this.m_BrainOverrideStack = (playerData as ICameraOverrideStack);
			if (this.m_BrainOverrideStack == null)
			{
				return;
			}
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			bool flag = false;
			float num4 = 1f;
			for (int i = 0; i < playable.GetInputCount<Playable>(); i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				ScriptPlayable<CinemachineShotPlayable> playable2 = (ScriptPlayable<T>)playable.GetInput(i);
				CinemachineShotPlayable behaviour = playable2.GetBehaviour();
				if (behaviour != null && behaviour.IsValid && playable.GetPlayState<Playable>() == PlayState.Playing && inputWeight > 0f)
				{
					num2 = num3;
					num3 = i;
					num4 = inputWeight;
					if (++num == 2)
					{
						Playable input = playable.GetInput(num2);
						flag = (playable2.GetTime<ScriptPlayable<CinemachineShotPlayable>>() >= input.GetTime<Playable>());
						if (playable2.GetTime<ScriptPlayable<CinemachineShotPlayable>>() == input.GetTime<Playable>())
						{
							flag = (playable2.GetDuration<ScriptPlayable<CinemachineShotPlayable>>() < input.GetDuration<Playable>());
							break;
						}
						break;
					}
				}
			}
			if (num == 1 && num4 < 1f && playable.GetInput(num3).GetTime<Playable>() > playable.GetInput(num3).GetDuration<Playable>() / 2.0)
			{
				flag = true;
			}
			if (flag)
			{
				int num5 = num3;
				int num6 = num2;
				num2 = num5;
				num3 = num6;
				num4 = 1f - num4;
			}
			ICinemachineCamera camA = null;
			if (num2 >= 0)
			{
				camA = ((ScriptPlayable<T>)playable.GetInput(num2)).GetBehaviour().VirtualCamera;
			}
			ICinemachineCamera camB = null;
			if (num3 >= 0)
			{
				camB = ((ScriptPlayable<T>)playable.GetInput(num3)).GetBehaviour().VirtualCamera;
			}
			this.m_BrainOverrideId = this.m_BrainOverrideStack.SetCameraOverride(this.m_BrainOverrideId, this.Priority, camA, camB, num4, this.GetDeltaTime(info.deltaTime));
		}

		private float GetDeltaTime(float deltaTime)
		{
			if (this.m_PreviewPlay || Application.isPlaying)
			{
				return deltaTime;
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback && TargetPositionCache.HasCurrentTime)
			{
				return 0f;
			}
			return -1f;
		}

		public int Priority;

		public static CinemachinePlayableMixer.MasterDirectorDelegate GetMasterPlayableDirector;

		private ICameraOverrideStack m_BrainOverrideStack;

		private int m_BrainOverrideId = -1;

		private bool m_PreviewPlay;

		public delegate PlayableDirector MasterDirectorDelegate();
	}
}
