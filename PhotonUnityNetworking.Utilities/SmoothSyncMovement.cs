using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	[RequireComponent(typeof(PhotonView))]
	public class SmoothSyncMovement : MonoBehaviourPun, IPunObservable
	{
		public void Awake()
		{
			bool flag = false;
			using (List<Component>.Enumerator enumerator = base.photonView.ObservedComponents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				Debug.LogWarning(((this != null) ? this.ToString() : null) + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.rotation);
				return;
			}
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}

		public void Update()
		{
			if (!base.photonView.IsMine)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
			}
		}

		public float SmoothingDelay = 5f;

		private Vector3 correctPlayerPos = Vector3.zero;

		private Quaternion correctPlayerRot = Quaternion.identity;
	}
}
