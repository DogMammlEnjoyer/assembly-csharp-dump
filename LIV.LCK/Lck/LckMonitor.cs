using System;
using UnityEngine;

namespace Liv.Lck
{
	public class LckMonitor : MonoBehaviour, ILckMonitor
	{
		public event LckMonitor.LckMonitorRenderTextureSetDelegate OnRenderTextureSet;

		public string MonitorId
		{
			get
			{
				return this._monitorId;
			}
		}

		protected virtual void OnEnable()
		{
			if (string.IsNullOrEmpty(this._monitorId))
			{
				this._monitorId = Guid.NewGuid().ToString();
			}
			LckMediator.RegisterMonitor(this);
		}

		public virtual void SetRenderTexture(RenderTexture renderTexture)
		{
			LckMonitor.LckMonitorRenderTextureSetDelegate onRenderTextureSet = this.OnRenderTextureSet;
			if (onRenderTextureSet == null)
			{
				return;
			}
			onRenderTextureSet(renderTexture);
		}

		protected virtual void OnDestroy()
		{
			LckMediator.UnregisterMonitor(this);
		}

		[SerializeField]
		protected string _monitorId;

		public delegate void LckMonitorRenderTextureSetDelegate(RenderTexture renderTexture);
	}
}
