using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActivateGO : MonoBehaviour
{
	private void OnEnable()
	{
		this.active = PlayerPrefFlags.Check(this.flag);
		this.SetGOsActive(0);
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Combine(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	private void OnDisable()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	private void OnDestroy()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	public void OnFlagChange(PlayerPrefFlags.Flag f, bool value)
	{
		if (f != this.flag)
		{
			return;
		}
		this.active = value;
		this.SetGOsActive(this.flashes);
	}

	private void SetGOsActive(int fls)
	{
		ActivateGO.<SetGOsActive>d__10 <SetGOsActive>d__;
		<SetGOsActive>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetGOsActive>d__.<>4__this = this;
		<SetGOsActive>d__.fls = fls;
		<SetGOsActive>d__.<>1__state = -1;
		<SetGOsActive>d__.<>t__builder.Start<ActivateGO.<SetGOsActive>d__10>(ref <SetGOsActive>d__);
	}

	private void toggle(List<Renderer> renderers, bool state)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			if ((this.layerMask.value & 1 << renderers[i].gameObject.layer) != 0)
			{
				renderers[i].forceRenderingOff = !state;
			}
		}
	}

	[SerializeField]
	private GameObject targetGO;

	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	[SerializeField]
	private int flashes;

	[SerializeField]
	private LayerMask layerMask;

	private bool active;

	private bool flashing;
}
