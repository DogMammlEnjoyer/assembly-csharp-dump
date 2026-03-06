using System;
using System.Collections.Generic;

namespace Unity.Cinemachine
{
	internal sealed class VirtualCameraRegistry
	{
		public List<List<CinemachineVirtualCameraBase>> AllCamerasSortedByNestingLevel
		{
			get
			{
				return this.m_AllCameras;
			}
		}

		public int ActiveCameraCount
		{
			get
			{
				return this.m_ActiveCameras.Count;
			}
		}

		public CinemachineVirtualCameraBase GetActiveCamera(int index)
		{
			if (!this.m_ActiveCamerasAreSorted && this.m_ActiveCameras.Count > 1)
			{
				this.m_ActiveCameras.Sort(delegate(CinemachineVirtualCameraBase x, CinemachineVirtualCameraBase y)
				{
					if (x.Priority.Value != y.Priority.Value)
					{
						return y.Priority.Value.CompareTo(x.Priority.Value);
					}
					return y.ActivationId.CompareTo(x.ActivationId);
				});
				this.m_ActiveCamerasAreSorted = true;
			}
			return this.m_ActiveCameras[index];
		}

		public void AddActiveCamera(CinemachineVirtualCameraBase vcam)
		{
			int activationSequence = this.m_ActivationSequence;
			this.m_ActivationSequence = activationSequence + 1;
			vcam.ActivationId = activationSequence;
			this.m_ActiveCameras.Add(vcam);
			this.m_ActiveCamerasAreSorted = false;
		}

		public void RemoveActiveCamera(CinemachineVirtualCameraBase vcam)
		{
			if (this.m_ActiveCameras.Contains(vcam))
			{
				this.m_ActiveCameras.Remove(vcam);
			}
		}

		public void CameraDestroyed(CinemachineVirtualCameraBase vcam)
		{
			if (this.m_ActiveCameras.Contains(vcam))
			{
				this.m_ActiveCameras.Remove(vcam);
			}
		}

		public void CameraEnabled(CinemachineVirtualCameraBase vcam)
		{
			int num = 0;
			for (ICinemachineMixer parentCamera = vcam.ParentCamera; parentCamera != null; parentCamera = parentCamera.ParentCamera)
			{
				num++;
			}
			while (this.m_AllCameras.Count <= num)
			{
				this.m_AllCameras.Add(new List<CinemachineVirtualCameraBase>());
			}
			this.m_AllCameras[num].Add(vcam);
		}

		public void CameraDisabled(CinemachineVirtualCameraBase vcam)
		{
			for (int i = 0; i < this.m_AllCameras.Count; i++)
			{
				this.m_AllCameras[i].Remove(vcam);
			}
		}

		private readonly List<CinemachineVirtualCameraBase> m_ActiveCameras = new List<CinemachineVirtualCameraBase>();

		private readonly List<List<CinemachineVirtualCameraBase>> m_AllCameras = new List<List<CinemachineVirtualCameraBase>>();

		private bool m_ActiveCamerasAreSorted;

		private int m_ActivationSequence;
	}
}
