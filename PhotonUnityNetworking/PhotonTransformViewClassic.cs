using System;
using UnityEngine;

namespace Photon.Pun
{
	[AddComponentMenu("Photon Networking/Photon Transform View Classic")]
	public class PhotonTransformViewClassic : MonoBehaviourPun, IPunObservable
	{
		private void Awake()
		{
			this.m_PhotonView = base.GetComponent<PhotonView>();
			this.m_PositionControl = new PhotonTransformViewPositionControl(this.m_PositionModel);
			this.m_RotationControl = new PhotonTransformViewRotationControl(this.m_RotationModel);
			this.m_ScaleControl = new PhotonTransformViewScaleControl(this.m_ScaleModel);
		}

		private void OnEnable()
		{
			this.m_firstTake = true;
		}

		private void Update()
		{
			if (this.m_PhotonView == null || this.m_PhotonView.IsMine || !PhotonNetwork.IsConnectedAndReady)
			{
				return;
			}
			this.UpdatePosition();
			this.UpdateRotation();
			this.UpdateScale();
		}

		private void UpdatePosition()
		{
			if (!this.m_PositionModel.SynchronizeEnabled || !this.m_ReceivedNetworkUpdate)
			{
				return;
			}
			base.transform.localPosition = this.m_PositionControl.UpdatePosition(base.transform.localPosition);
		}

		private void UpdateRotation()
		{
			if (!this.m_RotationModel.SynchronizeEnabled || !this.m_ReceivedNetworkUpdate)
			{
				return;
			}
			base.transform.localRotation = this.m_RotationControl.GetRotation(base.transform.localRotation);
		}

		private void UpdateScale()
		{
			if (!this.m_ScaleModel.SynchronizeEnabled || !this.m_ReceivedNetworkUpdate)
			{
				return;
			}
			base.transform.localScale = this.m_ScaleControl.GetScale(base.transform.localScale);
		}

		public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
		{
			this.m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			this.m_PositionControl.OnPhotonSerializeView(base.transform.localPosition, stream, info);
			this.m_RotationControl.OnPhotonSerializeView(base.transform.localRotation, stream, info);
			this.m_ScaleControl.OnPhotonSerializeView(base.transform.localScale, stream, info);
			if (stream.IsReading)
			{
				this.m_ReceivedNetworkUpdate = true;
				if (this.m_firstTake)
				{
					this.m_firstTake = false;
					if (this.m_PositionModel.SynchronizeEnabled)
					{
						base.transform.localPosition = this.m_PositionControl.GetNetworkPosition();
					}
					if (this.m_RotationModel.SynchronizeEnabled)
					{
						base.transform.localRotation = this.m_RotationControl.GetNetworkRotation();
					}
					if (this.m_ScaleModel.SynchronizeEnabled)
					{
						base.transform.localScale = this.m_ScaleControl.GetNetworkScale();
					}
				}
			}
		}

		[HideInInspector]
		public PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

		[HideInInspector]
		public PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

		[HideInInspector]
		public PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

		private PhotonTransformViewPositionControl m_PositionControl;

		private PhotonTransformViewRotationControl m_RotationControl;

		private PhotonTransformViewScaleControl m_ScaleControl;

		private PhotonView m_PhotonView;

		private bool m_ReceivedNetworkUpdate;

		private bool m_firstTake;
	}
}
