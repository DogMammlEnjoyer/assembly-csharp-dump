using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

public class GameModeSelectorButtonLayout : MonoBehaviour
{
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += this.SetupButtons;
		if (this.superToggleButton != null)
		{
			this.superToggleButton.onPressed += this._OnPressedSuperToggleButton;
		}
	}

	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.SetupButtons;
		if (this.superToggleButton != null)
		{
			this.superToggleButton.onPressed -= this._OnPressedSuperToggleButton;
		}
	}

	public virtual void SetupButtons()
	{
		GameModeSelectorButtonLayout.<SetupButtons>d__9 <SetupButtons>d__;
		<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetupButtons>d__.<>4__this = this;
		<SetupButtons>d__.<>1__state = -1;
		<SetupButtons>d__.<>t__builder.Start<GameModeSelectorButtonLayout.<SetupButtons>d__9>(ref <SetupButtons>d__);
	}

	private void _OnPressedSuperToggleButton(GorillaPressableButton btn, bool isLeftHandPress)
	{
		if (GorillaComputer.instance == null)
		{
			Debug.Log("[GT/GameModeSelectorButtonLayout]  Tried pressing SUPER button but `GorillaComputer` is not ready.", this);
			return;
		}
		if (NetworkSystem.Instance == null)
		{
			Debug.Log("[GT/GameModeSelectorButtonLayout]  Tried pressing SUPER button but `NetworkSystem` is not ready.", this);
			return;
		}
		btn.isOn = !btn.isOn;
		PlayerPrefFlags.Set(PlayerPrefFlags.Flag.GAME_MODE_SELECTOR_IS_SUPER, btn.isOn);
		this.SetupButtons();
		HashSet<GameModeType> modesForZone = GameMode.GameModeZoneMapping.GetModesForZone(this.zone, NetworkSystem.Instance.SessionIsPrivate);
		GameModeType lastPressedGameModeType = GorillaComputer.instance.lastPressedGameModeType;
		GameModeType gameModeType;
		if ((lastPressedGameModeType == GameModeType.Casual || lastPressedGameModeType == GameModeType.SuperCasual) && modesForZone.Contains(GameModeType.Casual) && modesForZone.Contains(GameModeType.SuperCasual))
		{
			gameModeType = (btn.isOn ? GameModeType.SuperCasual : GameModeType.Casual);
		}
		else if ((lastPressedGameModeType == GameModeType.Infection || lastPressedGameModeType == GameModeType.SuperInfect) && modesForZone.Contains(GameModeType.Infection) && modesForZone.Contains(GameModeType.SuperInfect))
		{
			gameModeType = (btn.isOn ? GameModeType.SuperInfect : GameModeType.Infection);
		}
		else
		{
			gameModeType = lastPressedGameModeType;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(gameModeType.ToString(), isLeftHandPress);
	}

	private const string preLog = "[GT/GameModeSelectorButtonLayout]  ";

	private const string preErr = "ERROR!!!  ";

	[SerializeField]
	protected GorillaPressableButton superToggleButton;

	[SerializeField]
	protected ModeSelectButton pf_button;

	[SerializeField]
	protected GTZone zone;

	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	protected List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
