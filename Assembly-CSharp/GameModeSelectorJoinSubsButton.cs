using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

public class GameModeSelectorJoinSubsButton : MonoBehaviour
{
	private void OnEnable()
	{
		this.CheckSubscribed();
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeaveRoom);
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
		if (SubscriptionManager.IsLocalSubscribed())
		{
			this.ShowButton();
			return;
		}
		this.DisableButtonSubscribers();
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
		this.subsPublicButton.enabled = false;
		this.subsPublicButton.SetRendererMaterial(this.DisabledButtonMaterial);
		this.disabledObject.SetActive(true);
		this.disabledText.text = "ONLY FOR\nSUBSCRIBERS";
	}

	private void DisableButtonPrivate()
	{
		this.subsPublicButton.enabled = false;
		this.subsPublicButton.SetRendererMaterial(this.DisabledButtonMaterial);
		this.disabledObject.SetActive(true);
		this.disabledText.text = "IN PRIVATE ROOM";
	}

	public Material DisabledButtonMaterial;

	[SerializeField]
	private GorillaPressableButton subsPublicButton;

	[SerializeField]
	private GameObject disabledObject;

	[SerializeField]
	private TextMeshPro disabledText;
}
