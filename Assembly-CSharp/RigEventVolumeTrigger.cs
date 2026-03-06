using System;
using UnityEngine;

public class RigEventVolumeTrigger : MonoBehaviour
{
	public VRRig Rig
	{
		get
		{
			return this._rig;
		}
	}

	[SerializeField]
	private VRRig _rig;
}
