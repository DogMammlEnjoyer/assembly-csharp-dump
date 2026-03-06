using System;

namespace UnityEngine.Rendering.Universal
{
	internal sealed class StpHistory : CameraHistoryItem
	{
		public override void OnCreate(BufferedRTHandleSystem owner, uint typeId)
		{
			base.OnCreate(owner, typeId);
			for (int i = 0; i < 2; i++)
			{
				this.m_historyContexts[i] = new STP.HistoryContext();
			}
		}

		public override void Reset()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_historyContexts[i].Dispose();
			}
		}

		internal STP.HistoryContext GetHistoryContext(int eyeIndex)
		{
			return this.m_historyContexts[eyeIndex];
		}

		internal bool Update(UniversalCameraData cameraData)
		{
			STP.HistoryUpdateInfo historyUpdateInfo;
			historyUpdateInfo.preUpscaleSize = new Vector2Int(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
			historyUpdateInfo.postUpscaleSize = new Vector2Int(cameraData.pixelWidth, cameraData.pixelHeight);
			historyUpdateInfo.useHwDrs = false;
			historyUpdateInfo.useTexArray = (cameraData.xr.enabled && cameraData.xr.singlePassEnabled);
			int eyeIndex = (cameraData.xr.enabled && !cameraData.xr.singlePassEnabled) ? cameraData.xr.multipassId : 0;
			return !this.GetHistoryContext(eyeIndex).Update(ref historyUpdateInfo);
		}

		private STP.HistoryContext[] m_historyContexts = new STP.HistoryContext[2];
	}
}
