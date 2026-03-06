using System;
using Liv.Lck.DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

namespace Liv.Lck.Tablet
{
	[DefaultExecutionOrder(1000)]
	public class LCKMicVolume : MonoBehaviour
	{
		private void Awake()
		{
			if (this._micVolumeImage)
			{
				this._micVolumeImage.transform.SetSiblingIndex(0);
			}
		}

		private void Update()
		{
			if (this._lckService == null)
			{
				return;
			}
			this._incomingVolume = Mathf.Clamp01(this._lckService.GetMicrophoneOutputLevel().Result * 10f);
			if (this._micVolumeImage)
			{
				this._micVolumeImage.fillAmount = this._incomingVolume;
			}
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private float _incomingVolume;

		[SerializeField]
		private Image _micVolumeImage;
	}
}
