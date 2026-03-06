using System;
using UnityEngine.Playables;

namespace Unity.Cinemachine
{
	internal sealed class CinemachineShotPlayable : PlayableBehaviour
	{
		public bool IsValid
		{
			get
			{
				return this.VirtualCamera != null;
			}
		}

		public CinemachineVirtualCameraBase VirtualCamera;
	}
}
