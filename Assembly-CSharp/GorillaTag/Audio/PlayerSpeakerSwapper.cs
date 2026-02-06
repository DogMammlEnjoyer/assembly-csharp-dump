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
			Debug.Log(string.Format("Detected {0} players, low pass filter enabled is {1}", num, this._lowPassFilter.enabled));
		}

		[SerializeField]
		private Behaviour _lowPassFilter;
	}
}
