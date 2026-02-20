using System;
using UnityEngine;
using UnityEngine.XR;

public class GameButtonActivatable : MonoBehaviour, IGameActivatable
{
	public bool CheckInput(XRNode xrNode, float sensitivity = 0.25f)
	{
		switch (this.inputButton)
		{
		case GameButtonActivatable.InputButton.Trigger:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.ButtonA:
			return ControllerInputPoller.PrimaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.ButtonB:
			return ControllerInputPoller.SecondaryButtonPress(xrNode);
		case GameButtonActivatable.InputButton.Grip:
			return ControllerInputPoller.GripFloat(xrNode) > sensitivity;
		case GameButtonActivatable.InputButton.Joystick:
			return ControllerInputPoller.TriggerFloat(xrNode) > sensitivity;
		default:
			return false;
		}
	}

	public bool CheckInput(float sensitivity = 0.25f)
	{
		int equippedSlotIndex = this.gameEntity.EquippedSlotIndex;
		if (equippedSlotIndex == -1 || !this.gameEntity.IsHeldOrSnappedByLocalPlayer)
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (this.gameEntity.IsSnappedToHand)
		{
			int num;
			if (equippedSlotIndex != 2)
			{
				if (equippedSlotIndex != 3)
				{
					num = -1;
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
			}
			int num2 = num;
			GameEntity gameEntity;
			IGameActivatable gameActivatable;
			if (gamePlayer.TryGetSlotEntity(num2, out gameEntity) && gameEntity.TryGetComponent<IGameActivatable>(out gameActivatable))
			{
				return false;
			}
			if (this.inputButton == GameButtonActivatable.InputButton.Trigger && GameTriggerInteractable.LocalInteractableTriggers.Count > 0)
			{
				Vector3 position = gamePlayer.GetHandTransform(num2).position;
				for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
				{
					if (GameTriggerInteractable.LocalInteractableTriggers[i].PointWithinInteractableArea(position))
					{
						return false;
					}
				}
			}
		}
		return this.CheckInput(this.gameEntity.EquippedHandXRNode, sensitivity);
	}

	[SerializeField]
	public GameButtonActivatable.InputButton inputButton;

	public GameEntity gameEntity;

	public enum InputButton
	{
		Trigger,
		ButtonA,
		ButtonB,
		Grip,
		Joystick
	}
}
