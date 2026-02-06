using System;
using UnityEngine;

namespace Photon.Pun
{
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu("Photon Networking/Photon Rigidbody View")]
	public class PhotonRigidbodyView : MonoBehaviourPun, IPunObservable
	{
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody>();
			this.m_NetworkPosition = default(Vector3);
			this.m_NetworkRotation = default(Quaternion);
		}

		public void FixedUpdate()
		{
			if (!base.photonView.IsMine)
			{
				this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.m_Body.position);
				stream.SendNext(this.m_Body.rotation);
				if (this.m_SynchronizeVelocity)
				{
					stream.SendNext(this.m_Body.linearVelocity);
				}
				if (this.m_SynchronizeAngularVelocity)
				{
					stream.SendNext(this.m_Body.angularVelocity);
					return;
				}
			}
			else
			{
				this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
				this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
				if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
				{
					this.m_Body.position = this.m_NetworkPosition;
				}
				if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
				{
					float d = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
					if (this.m_SynchronizeVelocity)
					{
						this.m_Body.linearVelocity = (Vector3)stream.ReceiveNext();
						this.m_NetworkPosition += this.m_Body.linearVelocity * d;
						this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
					}
					if (this.m_SynchronizeAngularVelocity)
					{
						this.m_Body.angularVelocity = (Vector3)stream.ReceiveNext();
						this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * d) * this.m_NetworkRotation;
						this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
					}
				}
			}
		}

		private float m_Distance;

		private float m_Angle;

		private Rigidbody m_Body;

		private Vector3 m_NetworkPosition;

		private Quaternion m_NetworkRotation;

		[HideInInspector]
		public bool m_SynchronizeVelocity = true;

		[HideInInspector]
		public bool m_SynchronizeAngularVelocity;

		[HideInInspector]
		public bool m_TeleportEnabled;

		[HideInInspector]
		public float m_TeleportIfDistanceGreaterThan = 3f;
	}
}
