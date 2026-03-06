using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal static class CameraUpdateManager
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeModule()
		{
			CameraUpdateManager.s_UpdateStatus = new Dictionary<CinemachineVirtualCameraBase, CameraUpdateManager.UpdateStatus>();
		}

		public static int VirtualCameraCount
		{
			get
			{
				return CameraUpdateManager.s_CameraRegistry.ActiveCameraCount;
			}
		}

		public static CinemachineVirtualCameraBase GetVirtualCamera(int index)
		{
			return CameraUpdateManager.s_CameraRegistry.GetActiveCamera(index);
		}

		public static void AddActiveCamera(CinemachineVirtualCameraBase vcam)
		{
			CameraUpdateManager.s_CameraRegistry.AddActiveCamera(vcam);
		}

		public static void RemoveActiveCamera(CinemachineVirtualCameraBase vcam)
		{
			CameraUpdateManager.s_CameraRegistry.RemoveActiveCamera(vcam);
		}

		public static void CameraDestroyed(CinemachineVirtualCameraBase vcam)
		{
			CameraUpdateManager.s_CameraRegistry.CameraDestroyed(vcam);
			if (CameraUpdateManager.s_UpdateStatus != null && CameraUpdateManager.s_UpdateStatus.ContainsKey(vcam))
			{
				CameraUpdateManager.s_UpdateStatus.Remove(vcam);
			}
		}

		public static void CameraEnabled(CinemachineVirtualCameraBase vcam)
		{
			CameraUpdateManager.s_CameraRegistry.CameraEnabled(vcam);
		}

		public static void CameraDisabled(CinemachineVirtualCameraBase vcam)
		{
			CameraUpdateManager.s_CameraRegistry.CameraDisabled(vcam);
		}

		public static void ForgetContext(object context)
		{
			if (CameraUpdateManager.s_LastFixedUpdateContext == context)
			{
				CameraUpdateManager.s_LastFixedUpdateContext = null;
			}
		}

		public static void UpdateAllActiveVirtualCameras(uint channelMask, Vector3 worldUp, float deltaTime, object context)
		{
			if ((CameraUpdateManager.s_CurrentUpdateFilter & (CameraUpdateManager.UpdateFilter)(-9)) == CameraUpdateManager.UpdateFilter.Fixed && (CameraUpdateManager.s_LastFixedUpdateContext == null || CameraUpdateManager.s_LastFixedUpdateContext == context))
			{
				CameraUpdateManager.s_FixedFrameCount++;
				CameraUpdateManager.s_LastFixedUpdateContext = context;
			}
			List<List<CinemachineVirtualCameraBase>> allCamerasSortedByNestingLevel = CameraUpdateManager.s_CameraRegistry.AllCamerasSortedByNestingLevel;
			float currentTime = CinemachineCore.CurrentTime;
			if (currentTime != CameraUpdateManager.s_LastUpdateTime)
			{
				CameraUpdateManager.s_LastUpdateTime = currentTime;
				if (allCamerasSortedByNestingLevel.Count > 0)
				{
					if (CameraUpdateManager.s_RoundRobinIndex >= allCamerasSortedByNestingLevel.Count)
					{
						CameraUpdateManager.s_RoundRobinIndex = 0;
					}
					if (++CameraUpdateManager.s_RoundRobinSubIndex >= allCamerasSortedByNestingLevel[CameraUpdateManager.s_RoundRobinIndex].Count)
					{
						CameraUpdateManager.s_RoundRobinSubIndex = 0;
						if (++CameraUpdateManager.s_RoundRobinIndex >= allCamerasSortedByNestingLevel.Count)
						{
							CameraUpdateManager.s_RoundRobinIndex = 0;
						}
					}
				}
			}
			for (int i = allCamerasSortedByNestingLevel.Count - 1; i >= 0; i--)
			{
				List<CinemachineVirtualCameraBase> list = allCamerasSortedByNestingLevel[i];
				for (int j = list.Count - 1; j >= 0; j--)
				{
					CinemachineVirtualCameraBase cinemachineVirtualCameraBase = list[j];
					if (cinemachineVirtualCameraBase == null)
					{
						list.RemoveAt(j);
					}
					else if ((cinemachineVirtualCameraBase.OutputChannel & (OutputChannels)channelMask) != (OutputChannels)0)
					{
						if (CinemachineCore.IsLive(cinemachineVirtualCameraBase) || cinemachineVirtualCameraBase.StandbyUpdate == CinemachineVirtualCameraBase.StandbyUpdateMode.Always)
						{
							CameraUpdateManager.UpdateVirtualCamera(cinemachineVirtualCameraBase, worldUp, deltaTime);
						}
						else if (cinemachineVirtualCameraBase.StandbyUpdate == CinemachineVirtualCameraBase.StandbyUpdateMode.RoundRobin && CameraUpdateManager.s_RoundRobinIndex == i && CameraUpdateManager.s_RoundRobinSubIndex == j && cinemachineVirtualCameraBase.isActiveAndEnabled)
						{
							CameraUpdateManager.UpdateVirtualCamera(cinemachineVirtualCameraBase, worldUp, deltaTime);
						}
					}
				}
			}
		}

		public static void UpdateVirtualCamera(CinemachineVirtualCameraBase vcam, Vector3 worldUp, float deltaTime)
		{
			if (vcam == null)
			{
				return;
			}
			bool flag = (CameraUpdateManager.s_CurrentUpdateFilter & CameraUpdateManager.UpdateFilter.Smart) == CameraUpdateManager.UpdateFilter.Smart;
			UpdateTracker.UpdateClock updateClock = (UpdateTracker.UpdateClock)(CameraUpdateManager.s_CurrentUpdateFilter & (CameraUpdateManager.UpdateFilter)(-9));
			if (flag)
			{
				Transform updateTarget = CameraUpdateManager.GetUpdateTarget(vcam);
				if (updateTarget == null)
				{
					return;
				}
				if (UpdateTracker.GetPreferredUpdate(updateTarget) != updateClock)
				{
					return;
				}
			}
			if (CameraUpdateManager.s_UpdateStatus == null)
			{
				CameraUpdateManager.s_UpdateStatus = new Dictionary<CinemachineVirtualCameraBase, CameraUpdateManager.UpdateStatus>();
			}
			CameraUpdateManager.UpdateStatus updateStatus;
			if (!CameraUpdateManager.s_UpdateStatus.TryGetValue(vcam, out updateStatus))
			{
				updateStatus = new CameraUpdateManager.UpdateStatus
				{
					lastUpdateMode = UpdateTracker.UpdateClock.Late,
					lastUpdateFrame = CinemachineCore.CurrentUpdateFrame + 2,
					lastUpdateFixedFrame = CameraUpdateManager.s_FixedFrameCount + 2
				};
				CameraUpdateManager.s_UpdateStatus.Add(vcam, updateStatus);
			}
			int num = (updateClock == UpdateTracker.UpdateClock.Late) ? (CinemachineCore.CurrentUpdateFrame - updateStatus.lastUpdateFrame) : (CameraUpdateManager.s_FixedFrameCount - updateStatus.lastUpdateFixedFrame);
			if (deltaTime >= 0f)
			{
				if (num == 0 && updateStatus.lastUpdateMode == updateClock)
				{
					return;
				}
				if (!CinemachineCore.UnitTestMode && num > 0)
				{
					deltaTime *= (float)num;
				}
			}
			vcam.InternalUpdateCameraState(worldUp, deltaTime);
			updateStatus.lastUpdateFrame = CinemachineCore.CurrentUpdateFrame;
			updateStatus.lastUpdateFixedFrame = CameraUpdateManager.s_FixedFrameCount;
			updateStatus.lastUpdateMode = updateClock;
		}

		private static Transform GetUpdateTarget(CinemachineVirtualCameraBase vcam)
		{
			if (vcam == null || vcam.gameObject == null)
			{
				return null;
			}
			Transform transform = vcam.LookAt;
			if (transform != null)
			{
				return transform;
			}
			transform = vcam.Follow;
			if (transform != null)
			{
				return transform;
			}
			return vcam.transform;
		}

		public static UpdateTracker.UpdateClock GetVcamUpdateStatus(CinemachineVirtualCameraBase vcam)
		{
			CameraUpdateManager.UpdateStatus updateStatus;
			if (CameraUpdateManager.s_UpdateStatus == null || !CameraUpdateManager.s_UpdateStatus.TryGetValue(vcam, out updateStatus))
			{
				return UpdateTracker.UpdateClock.Late;
			}
			return updateStatus.lastUpdateMode;
		}

		private static readonly VirtualCameraRegistry s_CameraRegistry = new VirtualCameraRegistry();

		private static int s_RoundRobinIndex = 0;

		private static int s_RoundRobinSubIndex = 0;

		private static object s_LastFixedUpdateContext;

		private static float s_LastUpdateTime = 0f;

		private static int s_FixedFrameCount = 0;

		private static Dictionary<CinemachineVirtualCameraBase, CameraUpdateManager.UpdateStatus> s_UpdateStatus;

		public static CameraUpdateManager.UpdateFilter s_CurrentUpdateFilter;

		private class UpdateStatus
		{
			public int lastUpdateFrame;

			public int lastUpdateFixedFrame;

			public UpdateTracker.UpdateClock lastUpdateMode;
		}

		public enum UpdateFilter
		{
			Fixed = 1,
			Late,
			Smart = 8,
			SmartFixed,
			SmartLate
		}
	}
}
