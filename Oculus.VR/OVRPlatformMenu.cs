using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_platform_menu")]
public class OVRPlatformMenu : MonoBehaviour
{
	private OVRPlatformMenu.eBackButtonAction HandleBackButtonState()
	{
		OVRPlatformMenu.eBackButtonAction result = OVRPlatformMenu.eBackButtonAction.NONE;
		if (OVRInput.GetDown(this.inputCode, OVRInput.Controller.Active))
		{
			result = OVRPlatformMenu.eBackButtonAction.SHORT_PRESS;
		}
		return result;
	}

	private void Awake()
	{
		if (this.shortPressHandler == OVRPlatformMenu.eHandler.RetreatOneLevel && this.OnShortPress == null)
		{
			this.OnShortPress = new Func<bool>(OVRPlatformMenu.RetreatOneLevel);
		}
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
			return;
		}
		OVRPlatformMenu.sceneStack.Push(SceneManager.GetActiveScene().name);
	}

	private void ShowConfirmQuitMenu()
	{
	}

	private static bool RetreatOneLevel()
	{
		if (OVRPlatformMenu.sceneStack.Count > 1)
		{
			SceneManager.LoadSceneAsync(OVRPlatformMenu.sceneStack.Pop());
			return false;
		}
		return true;
	}

	private void Update()
	{
	}

	private OVRInput.RawButton inputCode = OVRInput.RawButton.Back;

	public OVRPlatformMenu.eHandler shortPressHandler;

	public Func<bool> OnShortPress;

	private static Stack<string> sceneStack = new Stack<string>();

	public enum eHandler
	{
		ShowConfirmQuit,
		RetreatOneLevel
	}

	private enum eBackButtonAction
	{
		NONE,
		SHORT_PRESS
	}
}
