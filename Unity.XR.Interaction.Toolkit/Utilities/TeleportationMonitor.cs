using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal class TeleportationMonitor
	{
		public event Action<Pose, Pose, Pose> teleported;

		private void Initialize()
		{
			TeleportationMonitor.ProviderMonitor<TeleportationProvider> providerMonitor = new TeleportationMonitor.ProviderMonitor<TeleportationProvider>();
			providerMonitor.providerStepped += this.OnTeleportedAlways;
			TeleportationMonitor.ProviderMonitor<SnapTurnProvider> providerMonitor2 = new TeleportationMonitor.ProviderMonitor<SnapTurnProvider>();
			providerMonitor2.providerStepped += this.OnTeleportedAlways;
			TeleportationMonitor.ProviderMonitor<ContinuousTurnProvider> providerMonitor3 = new TeleportationMonitor.ProviderMonitor<ContinuousTurnProvider>();
			providerMonitor3.providerStepped += this.OnTeleportedTurnAround;
			TeleportationMonitor.ProviderMonitor<TeleportationProvider>.InitializeProvidersList();
			TeleportationMonitor.ProviderMonitor<SnapTurnProvider>.InitializeProvidersList();
			TeleportationMonitor.ProviderMonitor<ContinuousTurnProvider>.InitializeProvidersList();
			this.m_Monitors = new TeleportationMonitor.ProviderMonitor[]
			{
				providerMonitor,
				providerMonitor2,
				providerMonitor3
			};
		}

		public void AddInteractor(IXRInteractor interactor)
		{
			if (this.m_Monitors == null)
			{
				this.Initialize();
			}
			TeleportationMonitor.ProviderMonitor[] monitors = this.m_Monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].AddInteractor(interactor);
			}
		}

		public void RemoveInteractor(IXRInteractor interactor)
		{
			TeleportationMonitor.ProviderMonitor[] monitors = this.m_Monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].RemoveInteractor(interactor);
			}
		}

		private void OnTeleportedAlways(TeleportationMonitor.PoseContainer poseContainer)
		{
			int frameCount = Time.frameCount;
			if (this.m_TeleportedFrame == frameCount)
			{
				return;
			}
			this.m_TeleportedFrame = frameCount;
			poseContainer.CalculateDeltaPose();
			Action<Pose, Pose, Pose> action = this.teleported;
			if (action == null)
			{
				return;
			}
			action(poseContainer.beforePose, poseContainer.afterPose, poseContainer.deltaPose);
		}

		private void OnTeleportedTurnAround(TeleportationMonitor.PoseContainer poseContainer)
		{
			int frameCount = Time.frameCount;
			if (this.m_TeleportedFrame == frameCount)
			{
				return;
			}
			if (Vector3.Dot(poseContainer.beforePose.forward, poseContainer.afterPose.forward) >= 0f)
			{
				return;
			}
			this.m_TeleportedFrame = frameCount;
			poseContainer.CalculateDeltaPose();
			Action<Pose, Pose, Pose> action = this.teleported;
			if (action == null)
			{
				return;
			}
			action(poseContainer.beforePose, poseContainer.afterPose, poseContainer.deltaPose);
		}

		private int m_TeleportedFrame = -1;

		private TeleportationMonitor.ProviderMonitor[] m_Monitors;

		private class PoseContainer
		{
			public void CaptureBeforePose(XRBodyTransformer bodyTransformer)
			{
				int frameCount = Time.frameCount;
				if (this.m_BeforeFrame == frameCount)
				{
					return;
				}
				Transform transform;
				if (!LocomotionUtility.TryGetOriginTransform(bodyTransformer, out transform))
				{
					return;
				}
				this.m_BeforeFrame = frameCount;
				this.beforePose = transform.GetWorldPose();
			}

			public void CaptureAfterPose(XRBodyTransformer bodyTransformer)
			{
				int frameCount = Time.frameCount;
				if (this.m_AfterFrame == frameCount)
				{
					return;
				}
				Transform transform;
				if (!LocomotionUtility.TryGetOriginTransform(bodyTransformer, out transform))
				{
					return;
				}
				this.m_AfterFrame = frameCount;
				this.afterPose = transform.GetWorldPose();
			}

			public void CalculateDeltaPose()
			{
				int frameCount = Time.frameCount;
				if (this.m_DeltaFrame == frameCount)
				{
					return;
				}
				Vector3 position = this.afterPose.position - this.beforePose.position;
				Quaternion rotation = this.afterPose.rotation * Quaternion.Inverse(this.beforePose.rotation);
				this.m_DeltaFrame = frameCount;
				this.deltaPose = new Pose(position, rotation);
			}

			public Pose beforePose;

			public Pose afterPose;

			public Pose deltaPose;

			private int m_BeforeFrame = -1;

			private int m_AfterFrame = -1;

			private int m_DeltaFrame = -1;
		}

		private abstract class ProviderMonitor
		{
			public abstract void AddInteractor(IXRInteractor interactor);

			public abstract void RemoveInteractor(IXRInteractor interactor);

			protected static Dictionary<XRBodyTransformer, TeleportationMonitor.PoseContainer> s_OriginPoses;
		}

		private class ProviderMonitor<T> : TeleportationMonitor.ProviderMonitor where T : LocomotionProvider
		{
			public event Action<TeleportationMonitor.PoseContainer> providerStepped;

			public static void InitializeProvidersList()
			{
				if (TeleportationMonitor.ProviderMonitor<T>.s_Providers != null)
				{
					return;
				}
				TeleportationMonitor.ProviderMonitor<T>.s_Providers = new List<T>();
				foreach (LocomotionProvider locomotionProvider in LocomotionProvider.locomotionProviders)
				{
					if (!(locomotionProvider == null))
					{
						T t = locomotionProvider as T;
						if (t != null)
						{
							TeleportationMonitor.ProviderMonitor<T>.s_Providers.Add(t);
						}
					}
				}
				LocomotionProvider.locomotionProvidersChanged += TeleportationMonitor.ProviderMonitor<T>.<InitializeProvidersList>g__OnLocomotionProvidersChanged|6_0;
			}

			public override void AddInteractor(IXRInteractor interactor)
			{
				if (interactor == null)
				{
					throw new ArgumentNullException("interactor");
				}
				Transform transform = interactor.transform;
				if (transform == null)
				{
					return;
				}
				if (TeleportationMonitor.ProviderMonitor<T>.s_Providers == null)
				{
					TeleportationMonitor.ProviderMonitor<T>.InitializeProvidersList();
				}
				foreach (T t in TeleportationMonitor.ProviderMonitor<T>.s_Providers)
				{
					Transform parent;
					if (!(t == null) && LocomotionUtility.TryGetOriginTransform(t, out parent) && transform.IsChildOf(parent))
					{
						if (this.m_ProviderInteractors == null)
						{
							this.m_ProviderInteractors = TeleportationMonitor.ProviderMonitor<T>.s_ProviderInteractorsPool.Get();
						}
						List<IXRInteractor> list;
						if (!this.m_ProviderInteractors.TryGetValue(t, out list))
						{
							list = new List<IXRInteractor>();
							this.m_ProviderInteractors.Add(t, list);
						}
						list.Add(interactor);
						if (list.Count == 1)
						{
							t.beforeStepLocomotion += TeleportationMonitor.ProviderMonitor<T>.OnBeforeStepLocomotion;
							t.afterStepLocomotion += this.OnAfterStepLocomotion;
						}
					}
				}
			}

			public override void RemoveInteractor(IXRInteractor interactor)
			{
				if (interactor == null)
				{
					throw new ArgumentNullException("interactor");
				}
				int num = 0;
				if (this.m_ProviderInteractors != null)
				{
					foreach (KeyValuePair<T, List<IXRInteractor>> keyValuePair in this.m_ProviderInteractors)
					{
						T key = keyValuePair.Key;
						List<IXRInteractor> value = keyValuePair.Value;
						if (!(key == null))
						{
							if (value.Remove(interactor) && value.Count == 0)
							{
								key.beforeStepLocomotion -= TeleportationMonitor.ProviderMonitor<T>.OnBeforeStepLocomotion;
								key.afterStepLocomotion -= this.OnAfterStepLocomotion;
							}
							num += value.Count;
						}
					}
				}
				if (num == 0 && this.m_ProviderInteractors != null)
				{
					TeleportationMonitor.ProviderMonitor<T>.s_ProviderInteractorsPool.Release(this.m_ProviderInteractors);
					this.m_ProviderInteractors = null;
				}
			}

			private static void CaptureOriginPoseBefore(XRBodyTransformer bodyTransformer)
			{
				if (TeleportationMonitor.ProviderMonitor.s_OriginPoses == null)
				{
					TeleportationMonitor.ProviderMonitor.s_OriginPoses = new Dictionary<XRBodyTransformer, TeleportationMonitor.PoseContainer>();
				}
				TeleportationMonitor.PoseContainer poseContainer;
				if (!TeleportationMonitor.ProviderMonitor.s_OriginPoses.TryGetValue(bodyTransformer, out poseContainer))
				{
					poseContainer = new TeleportationMonitor.PoseContainer();
					TeleportationMonitor.ProviderMonitor.s_OriginPoses[bodyTransformer] = poseContainer;
				}
				poseContainer.CaptureBeforePose(bodyTransformer);
			}

			private static TeleportationMonitor.PoseContainer CaptureOriginPoseAfter(XRBodyTransformer bodyTransformer)
			{
				if (TeleportationMonitor.ProviderMonitor.s_OriginPoses == null)
				{
					TeleportationMonitor.ProviderMonitor.s_OriginPoses = new Dictionary<XRBodyTransformer, TeleportationMonitor.PoseContainer>();
				}
				TeleportationMonitor.PoseContainer poseContainer;
				if (!TeleportationMonitor.ProviderMonitor.s_OriginPoses.TryGetValue(bodyTransformer, out poseContainer))
				{
					poseContainer = new TeleportationMonitor.PoseContainer();
					TeleportationMonitor.ProviderMonitor.s_OriginPoses[bodyTransformer] = poseContainer;
				}
				poseContainer.CaptureAfterPose(bodyTransformer);
				return poseContainer;
			}

			private static void OnBeforeStepLocomotion(LocomotionProvider provider)
			{
				if (provider.mediator == null)
				{
					return;
				}
				TeleportationMonitor.ProviderMonitor<T>.CaptureOriginPoseBefore(provider.mediator.bodyTransformer);
			}

			private void OnAfterStepLocomotion(LocomotionProvider provider)
			{
				if (provider.mediator == null)
				{
					return;
				}
				TeleportationMonitor.PoseContainer obj = TeleportationMonitor.ProviderMonitor<T>.CaptureOriginPoseAfter(provider.mediator.bodyTransformer);
				Action<TeleportationMonitor.PoseContainer> action = this.providerStepped;
				if (action == null)
				{
					return;
				}
				action(obj);
			}

			[CompilerGenerated]
			internal static void <InitializeProvidersList>g__OnLocomotionProvidersChanged|6_0(LocomotionProvider provider)
			{
				T t = provider as T;
				if (t != null)
				{
					TeleportationMonitor.ProviderMonitor<T>.s_Providers.Add(t);
				}
				TeleportationMonitor.ProviderMonitor<T>.s_Providers.RemoveAll((T p) => p == null);
			}

			private Dictionary<T, List<IXRInteractor>> m_ProviderInteractors;

			private static List<T> s_Providers;

			private static readonly LinkedPool<Dictionary<T, List<IXRInteractor>>> s_ProviderInteractorsPool = new LinkedPool<Dictionary<T, List<IXRInteractor>>>(() => new Dictionary<T, List<IXRInteractor>>(), null, null, null, true, 10000);
		}
	}
}
