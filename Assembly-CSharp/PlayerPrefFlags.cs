using System;
using UnityEngine;

public class PlayerPrefFlags
{
	internal static bool Check(PlayerPrefFlags.Flag flag)
	{
		return (PlayerPrefs.GetInt("PlayerPrefFlags0", 5) & (int)flag) == (int)flag;
	}

	internal static void Touch(PlayerPrefFlags.Flag flag)
	{
		bool arg = (PlayerPrefs.GetInt("PlayerPrefFlags0", 5) & (int)flag) == (int)flag;
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, arg);
		}
	}

	internal static void TouchIf(PlayerPrefFlags.Flag flag, bool value)
	{
		int @int = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		if (value == ((@int & (int)flag) == (int)flag) && PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, value);
		}
	}

	internal static void Set(PlayerPrefFlags.Flag flag, bool value)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		if (value)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, value);
		}
	}

	internal static bool Flip(PlayerPrefFlags.Flag flag)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		bool flag2 = (num & (int)flag) != (int)flag;
		if (flag2)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, flag2);
		}
		return flag2;
	}

	public static Action<PlayerPrefFlags.Flag, bool> OnFlagChange;

	private const int defaultValue = 5;

	public enum Flag
	{
		SHOW_1P_COSMETICS = 1,
		SWAP_HELD_COSMETICS,
		GAME_MODE_SELECTOR_IS_SUPER = 4
	}
}
