using System;
using UnityEngine;

namespace Photon.Pun
{
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Photon Networking/Photon Rigidbody 2D View")]
	public class PhotonRigidbody2DView : MonoBehaviourPun, IPunObservable
	{
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody2D>();
			this.m_NetworkPosition = default(Vector2);
		}

		public void FixedUpdate()
		{
			if (!base.photonView.IsMine)
			{
				this.m_Body.position = Vector2.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Mathf.MoveTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
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
				this.m_NetworkPosition = (Vector2)stream.ReceiveNext();
				this.m_NetworkRotation = (float)stream.ReceiveNext();
				if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
				{
					this.m_Body.position = this.m_NetworkPosition;
				}
				if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
				{
					float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
					if (this.m_SynchronizeVelocity)
					{
						this.m_Body.linearVelocity = (Vector2)stream.ReceiveNext();
						this.m_NetworkPosition += this.m_Body.linearVelocity * num;
						this.m_Distance = Vector2.Distance(this.m_Body.position, this.m_NetworkPosition);
					}
					if (this.m_SynchronizeAngularVelocity)
					{
						this.m_Body.angularVelocity = (float)stream.ReceiveNext();
						this.m_NetworkRotation += this.m_Body.angularVelocity * num;
						this.m_Angle = Mathf.Abs(this.m_Body.rotation - this.m_NetworkRotation);
					}
				}
			}
		}

		private float m_Distance;

		private float m_Angle;

		private Rigidbody2D m_Body;

		private Vector2 m_NetworkPosition;

		private float m_NetworkRotation;

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
