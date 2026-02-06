using System;
using UnityEngine;

namespace Photon.Pun
{
	public class MonoBehaviourPun : MonoBehaviour
	{
		public PhotonView photonView
		{
			get
			{
				if (this.pvCache == null)
				{
					this.pvCache = PhotonView.Get(this);
				}
				return this.pvCache;
			}
		}

		private PhotonView pvCache;
	}
}
