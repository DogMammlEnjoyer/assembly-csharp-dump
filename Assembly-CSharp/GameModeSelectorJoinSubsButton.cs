using System;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameModeSelectorJoinSubsButton : MonoBehaviour
{
	private void OnEnable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeaveRoom);
		this.CheckSubscribed();
	}

	private void OnDisable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeaveRoom);
	}

	[ContextMenu("Check Subscribed")]
	private void CheckSubscribed()
	{
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			this.DisableButtonSubscribers();
			return;
		}
		if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.MaxPlayers <= 10)
		{
			this.ShowButton();
			return;
		}
		this.DisableButtonInPublicRoom();
	}

	private void OnJoinRoom()
	{
		if (!RoomSystem.WasRoomPrivate)
		{
			this.CheckSubscribed();
			return;
		}
		this.DisableButtonPrivate();
	}

	private void OnLeaveRoom()
	{
		this.CheckSubscribed();
	}

	private void ShowButton()
	{
		this.subsPublicButton.enabled = true;
		this.subsPublicButton.SetUnpressedMaterial();
		this.disabledObject.SetActive(false);
	}

	private void DisableButtonSubscribers()
	{
		this.DisableButton("ONLY FOR\nSUBSCRIBERS");
	}

	private void DisableButtonPrivate()
	{
		this.DisableButton("IN PRIVATE ROOM");
	}

	private void DisableButtonInPublicRoom()
	{
		this.DisableButton("ALREADY IN\nPUBLIC ROOM");
	}

	private void DisableButton(string disabled)
	{
		this.subsPublicButton.enabled = false;
		this.subsPublicButton.SetRendererMaterial(this.DisabledButtonMaterial);
		this.disabledObject.SetActive(true);
		this.disabledText.text = disabled;
	}

	public Material DisabledButtonMaterial;

	[SerializeField]
	private GorillaPressableButton subsPublicButton;

	[SerializeField]
	private GameObject disabledObject;

	[SerializeField]
	private TextMeshPro disabledText;
}
