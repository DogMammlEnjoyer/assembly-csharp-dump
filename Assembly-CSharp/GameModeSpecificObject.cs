using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

public class GameModeSpecificObject : MonoBehaviour
{
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnAwake;

	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnDestroyed;

	public GameModeSpecificObject.ValidationMethod Validation
	{
		get
		{
			return this.validationMethod;
		}
	}

	public List<GameModeType> GameModes
	{
		get
		{
			return this.gameModes;
		}
	}

	private void Awake()
	{
		GameModeSpecificObject.<Awake>d__15 <Awake>d__;
		<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Awake>d__.<>4__this = this;
		<Awake>d__.<>1__state = -1;
		<Awake>d__.<>t__builder.Start<GameModeSpecificObject.<Awake>d__15>(ref <Awake>d__);
	}

	private void OnDestroy()
	{
		if (GameModeSpecificObject.OnDestroyed != null)
		{
			GameModeSpecificObject.OnDestroyed(this);
		}
	}

	public bool CheckValid(GameModeType gameMode)
	{
		if (this.validationMethod == GameModeSpecificObject.ValidationMethod.Exclusion)
		{
			return !this.gameModes.Contains(gameMode);
		}
		return this.gameModes.Contains(gameMode);
	}

	[SerializeField]
	private GameModeSpecificObject.ValidationMethod validationMethod;

	[SerializeField]
	private GameModeType[] _gameModes;

	private List<GameModeType> gameModes;

	public delegate void GameModeSpecificObjectDelegate(GameModeSpecificObject gameModeSpecificObject);

	[Serializable]
	public enum ValidationMethod
	{
		Inclusion,
		Exclusion
	}
}
