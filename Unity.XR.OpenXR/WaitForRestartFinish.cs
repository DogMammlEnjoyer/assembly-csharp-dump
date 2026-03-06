using System;

namespace UnityEngine.XR.OpenXR
{
	internal sealed class WaitForRestartFinish : CustomYieldInstruction
	{
		public WaitForRestartFinish(float timeout = 5f)
		{
			this.m_Timeout = Time.realtimeSinceStartup + timeout;
		}

		public override bool keepWaiting
		{
			get
			{
				if (!OpenXRRestarter.Instance.isRunning)
				{
					return false;
				}
				if (Time.realtimeSinceStartup > this.m_Timeout)
				{
					Debug.LogError("WaitForRestartFinish: Timeout");
					return false;
				}
				return true;
			}
		}

		private float m_Timeout;
	}
}
