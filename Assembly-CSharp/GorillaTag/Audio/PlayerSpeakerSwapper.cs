using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	public class PlayerSpeakerSwapper : MonoBehaviour
	{
		private void OnEnable()
		{
			NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerCountChanged;
			NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerCountChanged;
			this.OnPlayerCountChanged(null);
		}

		private void OnDisable()
		{
			NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerCountChanged;
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerCountChanged;
		}

		private void OnPlayerCountChanged(NetPlayer _)
		{
			int num = NetworkSystem.Instance.AllNetPlayers.Length;
			this._lowPassFilter.enabled = (num >= 10);
		}

		[SerializeField]
		private Behaviour _lowPassFilter;
	}
}
